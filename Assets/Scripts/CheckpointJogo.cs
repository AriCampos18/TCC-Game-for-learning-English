using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

internal sealed partial class CheckpointJogo : MonoBehaviour
{
    internal static CheckpointJogo Instance { get; private set; }
    internal bool RetomouFase { get; private set; }
    internal string Status { get; private set; } = "Preparando checkpoint";
    internal static bool PodeAvancar => Instance == null || (!Instance.restaurando && !Instance.encerrando && !Instance.semPosse);
    private readonly CancellationTokenSource vida = new CancellationTokenSource();
    private readonly string escritor = Guid.NewGuid().ToString();
    private SessionManager sessao;
    private CheckpointApi api;
    private CheckpointLocal arquivo;
    private CheckpointDispositivo dispositivo;
    private int conta;
    private bool vinculoConferido;
    private bool encerrando, quitLiberado, semPosse, possuiEscrita, restaurando, descarregado, preparado;
    private bool retomadaProtegida;
    private float proximaTentativa;
    private bool pendenteRejeitado;
    private Task sincronizacao;
    private string claimId;
    private long claimBase, claimGeracao;

    internal static async Task<bool> Conectar(AuthenticationUIController auth)
    {
        if (!(auth.Session.AuthenticatedUserId is int id) || !auth.Session.IsAuthenticated)
            return false;
        if (Instance != null)
        {
            var anterior = Instance;
            anterior.Desativar();
            Destroy(anterior.gameObject);
        }

        var novo = new GameObject("Checkpoint v1").AddComponent<CheckpointJogo>();
        Instance = novo;
        novo.conta = id;
        novo.sessao = auth.Session;
        novo.restaurando = true;
        try
        {
            novo.api = new CheckpointApi(auth.BackendBaseUrl, auth.Session, id);
            novo.arquivo = new CheckpointLocal(Application.persistentDataPath, id, novo.api.Ambiente);
            novo.dispositivo = await Task.Run(novo.arquivo.Ler);
            novo.ValidarConta();
            try { await novo.AssumirPosse(inicial: true); }
            catch (Exception e) when (Temporaria(e) && novo.vinculoConferido && novo.dispositivo?.Atual != null
                && !novo.vida.IsCancellationRequested)
            {
                novo.Status = "Salvo localmente • sincronização pendente";
            }
            novo.ValidarConta();
            if (novo.dispositivo?.Atual != null)
            {
                var atual = novo.dispositivo.Atual;
                try { await novo.Restaurar(atual); }
                catch (InvalidDataException erro) when (MarcoAnteriorCompativel(atual, novo.dispositivo.MarcoAnterior))
                {
                    novo.ValidarConta();
                    novo.retomadaProtegida = true;
                    Debug.LogWarning($"Checkpoint diagnóstico: revisão {novo.dispositivo.Revisao} incompatível. "
                        + $"{erro.Message} Tentando o marco anterior com gravação bloqueada.", novo);
                    await novo.Restaurar(novo.dispositivo.MarcoAnterior);
                    novo.modoGiro = (string)atual["preferences"]?["turn_mode"] ?? novo.modoGiro;
                    novo.Status = "Marco anterior retomado • checkpoint avançado protegido";
                }
                novo.RetomouFase = true;
            }
            novo.preparado = true;
            return true;
        }
        catch (Exception e) when (Prevista(e))
        {
            if (novo != null && !novo.descarregado)
                Debug.LogWarning("Checkpoint: " + e.Message, novo);
            return false;
        }
        finally
        {
            novo.restaurando = false;
            if (novo != null && !novo.preparado && !novo.descarregado)
                novo.Descartar();
        }
    }

    internal void Descartar()
    {
        Desativar();
        Destroy(gameObject);
    }

    private static bool MarcoAnteriorCompativel(JObject atual, JObject anterior)
    {
        var a = atual?["visit"];
        var b = anterior?["visit"];
        return (string)a?["step_key"] == RoteiroNpcCheckpoint.Passo && b is JObject
            && (string)b["step_key"] != RoteiroNpcCheckpoint.Passo
            && (string)a["visit_id"] == (string)b["visit_id"]
            && (int?)a["scenario_id"] == (int?)b["scenario_id"]
            && (string)a["level"] == (string)b["level"];
    }

    private void OnEnable() => Application.wantsToQuit += AoSolicitarSaida;

    private void OnDestroy()
    {
        Application.wantsToQuit -= AoSolicitarSaida;
        Desativar();
    }

    private void Desativar()
    {
        if (descarregado)
            return;
        descarregado = true;
        vida.Cancel();
        if (Instance == this)
            Instance = null;
    }

    private void ValidarConta()
    {
        vida.Token.ThrowIfCancellationRequested();
        if (descarregado || Instance != this || sessao.AuthenticatedUserId != conta)
            throw new OperationCanceledException("A conta do checkpoint mudou.");
    }

    private async Task AssumirPosse(bool inicial)
    {
        if (claimId == null)
        {
            var estado = await api.Estado(vida.Token);
            ValidarConta();
            long revisao = (long)estado["revision"];
            long geracao = (long)estado["writer_epoch"];
            string vinculo = (string)estado["checkpoint_binding"];
            var remoto = estado["checkpoint"] as JObject;
            if ((int?)estado["schema_version"] != 1 || vinculo == null || vinculo.Length != 64
                || revisao < 0 || geracao < 0 || (revisao > 0) != (remoto != null))
                throw new InvalidDataException("Resposta inválida do checkpoint.");
            if (revisao == 0 && estado["legacy_base"]?.Type == JTokenType.String)
                throw new InvalidDataException("Dados legados requerem reconciliação antes de adotar o checkpoint.");

            var anterior = dispositivo?.Atual;
            long? revisaoLocal = dispositivo?.Revisao;
            if (dispositivo != null && !CheckpointSincronizacao.Reconciliar(dispositivo, vinculo, revisao, geracao, remoto))
            {
                if (!inicial || revisao < dispositivo.Revisao && dispositivo.Vinculo == vinculo)
                    throw new CheckpointApiException(409, "revision_conflict");
                if (dispositivo.Alterado)
                {
                    var conflitado = dispositivo;
                    await Task.Run(() => arquivo.Arquivar(conflitado));
                    ValidarConta();
                }
                dispositivo = null;
            }

            if (!inicial && anterior != null && !JToken.DeepEquals(anterior, dispositivo?.Atual))
                throw new CheckpointApiException(409, "revision_conflict");

            dispositivo ??= new CheckpointDispositivo
            {
                Conta = conta, Ambiente = api.Ambiente, Vinculo = vinculo, Atual = remoto,
                Revisao = revisao, Geracao = geracao
            };
            if (inicial)
                Debug.Log($"Checkpoint diagnóstico: revisão local {revisaoLocal}, remota {revisao}, "
                    + $"seleção {(ReferenceEquals(dispositivo.Atual, remoto) ? "remota" : "local pendente")}, "
                    + $"fase {dispositivo.Atual?["phase"]}, marco {dispositivo.Atual?["visit"]?["step_key"]}.", this);
            vinculoConferido = true;
            claimId = Guid.NewGuid().ToString();
            claimBase = revisao;
            claimGeracao = geracao;
        }
        var posse = await api.Assumir(escritor, claimId, claimGeracao, vida.Token);
        ValidarConta();
        if ((long?)posse["revision"] != claimBase || (long?)posse["writer_epoch"] != claimGeracao + 1)
            throw new CheckpointApiException(409, "revision_conflict");

        dispositivo.Geracao = claimGeracao + 1;
        possuiEscrita = true;
        claimId = null;
        await GravarLocal();
        AtualizarStatus();
    }

    private void Update()
    {
        if (!preparado || restaurando || encerrando || semPosse || retomadaProtegida || pendenteRejeitado || Time.unscaledTime < proximaTentativa)
            return;
        proximaTentativa = Time.unscaledTime + 5f;
        if (!possuiEscrita || dispositivo?.Alterado == true)
            _ = SolicitarSincronizacao();
    }

    private Task SolicitarSincronizacao()
    {
        if (sincronizacao == null || sincronizacao.IsCompleted)
            sincronizacao = Sincronizar();
        return sincronizacao;
    }

    private async Task Sincronizar()
    {
        try
        {
            if (semPosse || descarregado || retomadaProtegida || pendenteRejeitado || dispositivo == null)
                return;
            if (!possuiEscrita)
                await AssumirPosse(inicial: false);

            while (!semPosse && !descarregado && dispositivo.Alterado)
            {
                var envio = CheckpointSincronizacao.Preparar(dispositivo);
                await GravarLocal();
                ValidarConta();
                var resposta = await api.Salvar(escritor, dispositivo.Vinculo, dispositivo.Geracao, envio, vida.Token);
                ValidarConta();
                if ((string)resposta["save_id"] != envio.Id)
                    throw new InvalidDataException("Recibo de checkpoint de outro envio.");
                CheckpointSincronizacao.Confirmar(dispositivo, envio, (long)resposta["revision"]);
                await GravarLocal();
                if (!descarregado)
                    AtualizarStatus();
            }
        }
        catch (Exception e) when (e is CheckpointLocalObsoletoException
            || e is CheckpointApiException falha && (falha.Status == 409 || falha.Status == 401))
        {
            BloquearEscritor();
        }
        catch (CheckpointApiException e) when (e.Status == 400 || e.Status == 422)
        {
            if (!descarregado)
            {
                pendenteRejeitado = dispositivo.Pendente == null
                    || JToken.DeepEquals(dispositivo.Pendente.Estado, dispositivo.Atual);
                if (pendenteRejeitado)
                    Status = "Salvo localmente • checkpoint rejeitado pelo servidor";
                else
                {
                    dispositivo.Pendente = null;
                    await Persistir();
                }
                Debug.LogWarning(e.Message, this);
            }
        }
        catch (Exception e) when (Prevista(e))
        {
            if (!descarregado)
            {
                Status = e is IOException || e is UnauthorizedAccessException
                    ? "Falha ao salvar no dispositivo" : "Salvo localmente • sincronização pendente";
                Debug.LogWarning("Checkpoint: " + e.Message, this);
            }
        }
    }

    private void BloquearEscritor()
    {
        if (descarregado)
            return;
        semPosse = true;
        SessaoCenario.Atual.Bloqueada = true;
        Status = "Sessão escritora perdida • pendente preservado";
        FindAnyObjectByType<FirstPlayerController>()?.DesativarControle();
    }

    private Task GravarLocal()
    {
        long numero = CheckpointLocal.ProximaSequencia();
        string conteudo = JsonConvert.SerializeObject(dispositivo);
        return Task.Run(() => arquivo.Gravar(conteudo, numero));
    }

    private void AtualizarStatus()
    {
        if (pendenteRejeitado)
            return;
        Status = dispositivo.Atual == null ? "Ainda não há ponto seguro para salvar"
            : dispositivo.Alterado ? "Salvo localmente • sincronização pendente" : "Sincronizado com o servidor";
    }

    private async Task Persistir()
    {
        if (retomadaProtegida) return;
        try
        {
            await GravarLocal();
            if (!descarregado && !semPosse && !encerrando)
            {
                AtualizarStatus();
                if (dispositivo.Alterado)
                    _ = SolicitarSincronizacao();
            }
        }
        catch (CheckpointLocalObsoletoException)
        {
            BloquearEscritor();
        }
        catch (Exception e) when (Prevista(e))
        {
            if (!descarregado && !semPosse)
            {
                Status = "Falha ao salvar no dispositivo";
                Debug.LogWarning("Checkpoint: " + e.Message, this);
            }
        }
    }

    private static bool Temporaria(Exception e) =>
        e is HttpRequestException || e is OperationCanceledException || (e is CheckpointApiException api && api.Status >= 500);

    private static bool Prevista(Exception e) =>
        Temporaria(e) || e is CheckpointApiException || e is IOException || e is InvalidOperationException
        || e is JsonException || e is ArgumentException || e is OverflowException
        || e is UnauthorizedAccessException || e is InvalidCastException || e is FormatException;

    internal Task SalvarManual()
    {
        if (retomadaProtegida) return Task.CompletedTask;
        AtualizarCheckpointMissoes();
        if (dispositivo?.Atual == null)
            Status = "Ainda não há ponto seguro para salvar";
        return !preparado || semPosse || dispositivo?.Atual == null ? Task.CompletedTask : Persistir();
    }

    internal async Task Logout()
    {
        if (!await PrepararEncerramento())
            return;
        vida.Cancel();
        using (var limite = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
        {
            try
            {
                var resultado = await sessao.LogoutCheckpointAsync(limite.Token);
                if (!resultado.IsSuccess && resultado.Error?.Type == AuthenticationFailureType.Storage)
                {
                    encerrando = false;
                    semPosse = true;
                    SessaoCenario.Atual.Bloqueada = true;
                    Status = "Falha ao limpar o login • tente sair da conta novamente";
                    return;
                }
            }
            catch (OperationCanceledException) { }
        }

        if (descarregado || Instance != this)
            return;
        SessaoCenario.Atual.Encerrar();
        SessaoCenario.Atual.Bloqueada = false;
        DadosJogador.nivelUsuario = null;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    internal async Task Sair()
    {
        if (!await PrepararEncerramento())
            return;
        quitLiberado = true;
        Application.Quit();
    }

    private async Task<bool> PrepararEncerramento()
    {
        if (encerrando)
            return false;
        AtualizarCheckpointMissoes();
        encerrando = true;
        if (retomadaProtegida || dispositivo?.Atual == null)
            return true;
        try { await GravarLocal(); }
        catch (CheckpointLocalObsoletoException)
        {
            BloquearEscritor();
        }
        catch (Exception e) when (Prevista(e))
        {
            encerrando = false;
            Status = "Falha ao salvar no dispositivo • saída cancelada";
            Debug.LogWarning("Checkpoint: " + e.Message, this);
            return false;
        }

        if (!semPosse && dispositivo.Alterado)
            await Task.WhenAny(SolicitarSincronizacao(), Task.Delay(2000));
        return true;
    }

    private bool AoSolicitarSaida()
    {
        if (quitLiberado || dispositivo?.Atual == null)
            return true;
        _ = Sair();
        return false;
    }

    private void OnApplicationPause(bool pausado)
    {
        if (pausado && preparado && !encerrando && !semPosse && dispositivo?.Atual != null)
        {
            AtualizarCheckpointMissoes();
            _ = Persistir();
        }
    }

    private void OnApplicationQuit()
    {
        if (!quitLiberado && !retomadaProtegida && dispositivo?.Atual != null)
        {
            try { arquivo.Gravar(JsonConvert.SerializeObject(dispositivo), CheckpointLocal.ProximaSequencia()); }
            catch (Exception e) when (Prevista(e)) { Debug.LogWarning(e.Message, this); }
        }
    }
}
