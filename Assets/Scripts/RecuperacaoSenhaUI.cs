using System;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

internal sealed class RecuperacaoSenhaUI : IDisposable
{
    private readonly GameObject email, codigo, senha;
    private readonly TMP_InputField endereco, codigoDigitado, novaSenha, confirmacao;
    private readonly TMP_Text mensagemEmail, mensagemCodigo, mensagemSenha, rotuloCodigo;
    private readonly Button enviar, validar, redefinir;
    private readonly Func<AuthenticationManager> api;
    private readonly Action voltar, ocultarTeclado;
    private readonly RectTransform rectValidar;
    private readonly float larguraValidar;
    private readonly TextWrappingModes quebraRotulo;
    private readonly TextAlignmentOptions alinhamentoRotulo;
    private readonly Color corStatusSenha;
    private readonly System.Collections.Generic.List<Action> removerListeners = new();
    private readonly bool configurada;
    private CancellationTokenSource visita;
    private string emailSolicitado, solicitacao, token;
    private DateTimeOffset reenviarEm;
    private bool ocupado;

    internal TMP_InputField[] Campos => new[] { endereco, codigoDigitado, novaSenha, confirmacao };

    internal RecuperacaoSenhaUI(GameObject origem, Func<AuthenticationManager> api, Action voltar,
        Action ocultarTeclado)
    {
        this.api = api;
        this.voltar = voltar;
        this.ocultarTeclado = ocultarTeclado;
        email = VinculosModaisTcc.Painel(origem, "ModalEsqueciSenha");
        codigo = VinculosModaisTcc.Painel(origem, "ModalCodigoEmail");
        senha = VinculosModaisTcc.Painel(origem, "ModalNovaSenha");
        endereco = Campo(email, "EmailUsername");
        codigoDigitado = Campo(codigo, "EmailUsername");
        novaSenha = Campo(senha, "NovaSenha");
        confirmacao = Campo(senha, "ConfirmarNovaSenha");
        mensagemEmail = Mensagem(email);
        mensagemCodigo = Mensagem(codigo);
        mensagemSenha = Mensagem(senha);
        enviar = Botao(email, "BotaoContinuar");
        validar = Botao(codigo, "BotaoContinuar");
        redefinir = Botao(senha, "BotaoContinuar");
        rotuloCodigo = validar != null ? validar.GetComponentInChildren<TMP_Text>(true) : null;
        rectValidar = validar != null ? (RectTransform)validar.transform : null;
        if (rectValidar != null) larguraValidar = rectValidar.sizeDelta.x;
        if (rotuloCodigo != null)
        {
            quebraRotulo = rotuloCodigo.textWrappingMode;
            alinhamentoRotulo = rotuloCodigo.alignment;
        }
        if (mensagemSenha != null) corStatusSenha = mensagemSenha.color;

        if (novaSenha != null) novaSenha.contentType = TMP_InputField.ContentType.Password;
        if (confirmacao != null) confirmacao.contentType = TMP_InputField.ContentType.Password;
        if (codigoDigitado != null)
        {
            codigoDigitado.contentType = TMP_InputField.ContentType.IntegerNumber;
            codigoDigitado.characterLimit = 6;
            codigoDigitado.onValueChanged.AddListener(AtualizarCodigo);
            removerListeners.Add(() => { if (codigoDigitado != null) codigoDigitado.onValueChanged.RemoveListener(AtualizarCodigo); });
        }

        configurada = endereco != null && codigoDigitado != null && novaSenha != null && confirmacao != null
            && mensagemEmail != null && mensagemCodigo != null && mensagemSenha != null && rotuloCodigo != null;
        configurada &= Ligar(enviar, Solicitar);
        configurada &= Ligar(validar, ValidarOuReenviar);
        configurada &= Ligar(redefinir, Redefinir);
        foreach (var painel in new[] { email, codigo, senha })
            configurada &= Ligar(Botao(painel, "BotaoVoltarLogin"), Voltar);
        Limpar();
    }

    private bool Ligar(Button botao, UnityEngine.Events.UnityAction acao)
    {
        if (!VinculosModaisTcc.Ligar(botao, acao)) return false;
        removerListeners.Add(() => { if (botao != null) botao.onClick.RemoveListener(acao); });
        return true;
    }

    internal bool Abrir()
    {
        Limpar();
        if (!configurada || api() == null) return false;
        visita = new CancellationTokenSource();
        endereco.SetTextWithoutNotify("");
        VinculosModaisTcc.Texto(mensagemEmail, "");
        Mostrar(email);
        return true;
    }

    private void Mostrar(GameObject painel)
    {
        ocultarTeclado();
        email.SetActive(painel == email);
        codigo.SetActive(painel == codigo);
        senha.SetActive(painel == senha);
    }

    private async void Solicitar()
    {
        string valor = endereco.text.Trim();
        try
        {
            if (valor.Length > 254 || new MailAddress(valor).Address != valor)
                throw new FormatException();
        }
        catch (Exception ex) when (ex is FormatException || ex is ArgumentException)
        {
            VinculosModaisTcc.Texto(mensagemEmail, "Informe um endereço de e-mail válido.");
            return;
        }
        await Emitir(valor, false);
    }

    private async Task Emitir(string valor, bool reenvio)
    {
        await Executar(reenvio ? mensagemCodigo : mensagemEmail, reenvio ? "resend" : "request",
            new { email = valor }, dados =>
            {
                string id = dados.Value<string>("request_id");
                int espera = dados.Value<int>("resend_after");
                if (string.IsNullOrEmpty(id) || espera < 1)
                    throw new InvalidOperationException();
                emailSolicitado = valor;
                solicitacao = id;
                reenviarEm = DateTimeOffset.UtcNow.AddSeconds(espera);
                codigoDigitado.SetTextWithoutNotify("");
                VinculosModaisTcc.Texto(mensagemCodigo, dados.Value<string>("message"));
                Mostrar(codigo);
                _ = AcompanharReenvio(visita.Token);
            });
    }

    private async void ValidarOuReenviar()
    {
        var valor = codigoDigitado.text;
        if (valor.Length == 0 && DateTimeOffset.UtcNow >= reenviarEm)
        {
            await Emitir(emailSolicitado, true);
            return;
        }
        if (!Regex.IsMatch(valor, @"\A[0-9]{6}\z")) return;
        await Executar(mensagemCodigo, "verify", new { request_id = solicitacao, code = valor }, dados =>
        {
            token = dados.Value<string>("reset_token");
            if (string.IsNullOrEmpty(token)) throw new InvalidOperationException();
            codigoDigitado.SetTextWithoutNotify("");
            VinculosModaisTcc.Texto(mensagemSenha, "");
            Mostrar(senha);
        });
    }

    private async void Redefinir()
    {
        if (ocupado || visita == null) return;
        var valor = novaSenha.text;
        if (valor.Length < 8 || valor != confirmacao.text)
        {
            VinculosModaisTcc.Texto(mensagemSenha, "Informe senhas iguais com ao menos 8 caracteres.");
            return;
        }
        await Executar(mensagemSenha, "reset",
            new { request_id = solicitacao, reset_token = token, new_password = valor },
            null,
            erro =>
            {
                if (erro.Code != "invalid_reset_token") return false;
                token = null;
                novaSenha.SetTextWithoutNotify("");
                confirmacao.SetTextWithoutNotify("");
                Mostrar(codigo);
                VinculosModaisTcc.Texto(mensagemCodigo, erro.Message);
                return true;
            },
            ConfirmarRedefinicao);
    }

    private async Task ConfirmarRedefinicao(CancellationToken cancelamento)
    {
        novaSenha.SetTextWithoutNotify("");
        confirmacao.SetTextWithoutNotify("");
        ocultarTeclado();
        mensagemSenha.color = new Color32(0, 128, 55, 255);
        VinculosModaisTcc.Texto(mensagemSenha, "Senha redefinida com sucesso!");
        await Task.Delay(TimeSpan.FromSeconds(3), cancelamento);
        if (!cancelamento.IsCancellationRequested && senha != null && senha.activeInHierarchy)
            voltar();
    }

    private async Task Executar(TMP_Text status, string operacao, object corpo, Action<JObject> sucesso,
        Func<AuthenticationError, bool> tratarErro = null, Func<CancellationToken, Task> aposSucesso = null)
    {
        if (ocupado || visita == null) return;
        var atual = visita.Token;
        ocupado = true;
        AtualizarBotoes();
        VinculosModaisTcc.Texto(status, "Aguarde…");
        try
        {
            using var limite = CancellationTokenSource.CreateLinkedTokenSource(atual);
            limite.CancelAfter(TimeSpan.FromSeconds(20));
            var resultado = await api().RecuperarSenhaAsync(operacao, corpo, limite.Token);
            if (atual.IsCancellationRequested) return;
            if (resultado.IsSuccess)
            {
                sucesso?.Invoke(resultado.Value);
                if (aposSucesso != null) await aposSucesso(atual);
            }
            else if (tratarErro?.Invoke(resultado.Error) != true)
                VinculosModaisTcc.Texto(status, resultado.Error.Message);
        }
        catch (OperationCanceledException)
        {
            if (!atual.IsCancellationRequested)
                VinculosModaisTcc.Texto(status, "A conexão expirou. Verifique sua conexão e tente novamente.");
        }
        catch (Exception)
        {
            if (!atual.IsCancellationRequested)
                VinculosModaisTcc.Texto(status, "Não foi possível completar a recuperação agora.");
        }
        finally
        {
            if (!atual.IsCancellationRequested)
            {
                ocupado = false;
                AtualizarBotoes();
            }
        }
    }

    private async Task AcompanharReenvio(CancellationToken cancelamento)
    {
        try
        {
            while (!cancelamento.IsCancellationRequested && DateTimeOffset.UtcNow < reenviarEm)
            {
                AtualizarBotoes();
                await Task.Delay(1000, cancelamento);
            }
            if (!cancelamento.IsCancellationRequested) AtualizarBotoes();
        }
        catch (OperationCanceledException) { }
    }

    private void AtualizarCodigo(string _) => AtualizarBotoes();

    private void AtualizarBotoes()
    {
        if (enviar != null) enviar.interactable = !ocupado;
        if (redefinir != null) redefinir.interactable = !ocupado;
        if (validar == null || codigoDigitado == null) return;
        int segundos = Math.Max(0, (int)Math.Ceiling((reenviarEm - DateTimeOffset.UtcNow).TotalSeconds));
        string valor = codigoDigitado.text;
        validar.interactable = !ocupado && (valor.Length == 0 ? segundos == 0 : Regex.IsMatch(valor, @"\A[0-9]{6}\z"));
        bool contagem = valor.Length == 0 && segundos > 0;
        VinculosModaisTcc.Texto(rotuloCodigo, contagem ? $"Reenviar em {segundos}s"
            : valor.Length == 0 ? "Reenviar código" : "Validar código");
        AjustarRotuloCodigo(contagem);
    }

    private void AjustarRotuloCodigo(bool contagem)
    {
        if (rotuloCodigo == null || rectValidar == null) return;
        rotuloCodigo.textWrappingMode = contagem ? TextWrappingModes.NoWrap : quebraRotulo;
        rotuloCodigo.alignment = contagem ? TextAlignmentOptions.Center : alinhamentoRotulo;
        float largura = contagem ? Mathf.Max(larguraValidar, rotuloCodigo.preferredWidth + 24f) : larguraValidar;
        rectValidar.sizeDelta = new Vector2(largura, rectValidar.sizeDelta.y);
    }

    internal void Limpar()
    {
        visita?.Cancel();
        visita?.Dispose();
        visita = null;
        emailSolicitado = solicitacao = token = null;
        reenviarEm = default;
        ocupado = false;
        AjustarRotuloCodigo(false);
        if (mensagemSenha != null) mensagemSenha.color = corStatusSenha;
        foreach (var campo in Campos)
            if (campo != null) campo.SetTextWithoutNotify("");
        foreach (var painel in new[] { email, codigo, senha })
            if (painel != null) painel.SetActive(false);
    }

    public void Dispose()
    {
        Limpar();
        foreach (var remover in removerListeners) remover();
        removerListeners.Clear();
    }

    private void Voltar() => voltar();
    private static TMP_InputField Campo(GameObject painel, string nome) =>
        VinculosModaisTcc.Filho<TMP_InputField>(painel, nome);
    private static TMP_Text Mensagem(GameObject painel) =>
        VinculosModaisTcc.Filho<TMP_Text>(painel, "TxtMensagens");
    private static Button Botao(GameObject painel, string nome) =>
        VinculosModaisTcc.Filho<Button>(painel, nome);
}
