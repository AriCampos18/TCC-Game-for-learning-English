using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class ExercicioRevisao
{
    public TipoExercicio tipo;
    public ExercicioBase exercicio;
    public string contextoNpc;
    public string cenario;
    public InteracaoNPC npcOrigem;
    public bool aguardandoAssociacao;
}

public class RevisaoManager : MonoBehaviour
{
    public static RevisaoManager Instance;

    private readonly List<ExercicioRevisao> exerciciosErrados =
        new List<ExercicioRevisao>();

    private readonly JArray revisadosCheckpoint = new JArray();
    internal JArray CapturarRevisadosCheckpoint() => (JArray)revisadosCheckpoint.DeepClone();

    private string cenarioAtual;
    private int visitaAtual;
    private bool finalizacaoEmAndamento;
    private bool cenarioFinalizado;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void IniciarCenario(string cenario)
    {
        if (string.IsNullOrWhiteSpace(cenario))
        {
            Debug.LogError("Não é possível iniciar a revisão sem identificar o cenário.");
            return;
        }

        if (!SessaoCenario.Atual.Permite(cenario))
            return;

        cenarioAtual = cenario;
        visitaAtual = SessaoCenario.Atual.Versao;
        finalizacaoEmAndamento = false;
        cenarioFinalizado = false;
        exerciciosErrados.Clear();
        revisadosCheckpoint.RemoveAll();
    }

    public bool DeveAdiarAssociacoes
    {
        get { return cenarioAtual == "Market"; }
    }

    public void RegistrarErro(
        TipoExercicio tipo,
        ExercicioBase exercicio,
        string contextoNpc,
        InteracaoNPC npcOrigem)
    {
        if (!ValidarContextoRegistro(exercicio, npcOrigem))
            return;

        bool jaRegistrado = exerciciosErrados.Exists(item =>
            item.cenario == cenarioAtual
            && ReferenceEquals(item.exercicio, exercicio)
            && !item.aguardandoAssociacao);

        if (jaRegistrado)
            return;

        exerciciosErrados.Add(new ExercicioRevisao()
        {
            tipo = tipo,
            exercicio = exercicio,
            contextoNpc = contextoNpc,
            cenario = cenarioAtual,
            npcOrigem = npcOrigem,
            aguardandoAssociacao = false
        });
    }

    public void RegistrarAssociacao(
        ExercicioAssociacao exercicio,
        InteracaoNPC npcOrigem)
    {
        if (!ValidarContextoRegistro(exercicio, npcOrigem))
            return;

        bool jaRegistrado = exerciciosErrados.Exists(item =>
            item.cenario == cenarioAtual
            && item.aguardandoAssociacao
            && ReferenceEquals(item.exercicio, exercicio));

        if (jaRegistrado)
            return;

        exerciciosErrados.Add(new ExercicioRevisao()
        {
            tipo = TipoExercicio.Associacao,
            exercicio = exercicio,
            cenario = cenarioAtual,
            npcOrigem = npcOrigem,
            aguardandoAssociacao = true
        });
    }

    public List<ExercicioRevisao> GetAssociacoesPendentes(string cenario)
    {
        return exerciciosErrados.FindAll(item =>
            item.cenario == cenario && item.aguardandoAssociacao);
    }

    public void ConcluirAssociacao(ExercicioBase exercicio)
    {
        exerciciosErrados.RemoveAll(item =>
            item.cenario == cenarioAtual
            && item.aguardandoAssociacao
            && ReferenceEquals(item.exercicio, exercicio));
    }

    public List<ExercicioRevisao> GetExerciciosErrados(string cenario)
    {
        return exerciciosErrados.FindAll(item =>
            item.cenario == cenario && !item.aguardandoAssociacao);
    }

    internal JArray CapturarFilaCheckpoint(string cenario)
    {
        var fila = new JArray();
        foreach (var item in GetExerciciosErrados(cenario))
        {
            var ex = item.exercicio;
            var chave = CheckpointJogo.ChaveExercicio(ex, item.npcOrigem, false);
            if (chave == null || ex.idExercise <= 0 || ex.nivelBanco != SessaoCenario.Atual.Nivel
                || item.contextoNpc?.Length > 2048 || item.contextoNpc?.Contains("\0") == true)
                return null;
            var dados = new JObject();
            if (item.contextoNpc != null) dados["npc_line"] = item.contextoNpc;
            if (ex.reviewGreeting != null) dados["greeting"] = ex.reviewGreeting;
            if (ex.reviewTotal != null) dados["total"] = ex.reviewTotal;
            if (ex.reviewPeriod != null) dados["period"] = ex.reviewPeriod;
            if (dados.HasValues) chave["context"] = dados;
            fila.Add(chave);
        }
        return fila;
    }

    internal async Task<bool> RestaurarFilaCheckpoint(string cenario, int visita, JArray fila, JArray revisados, CancellationToken token)
    {
        var itens = new List<ExercicioRevisao>();
        var chaves = new HashSet<string>();
        foreach (var chave in fila)
        {
            if (!SessaoCenario.Atual.Permite(cenario, visita) || token.IsCancellationRequested)
                return false;
            string npcKey = (string)chave["npc_key"];
            string ponto = (string)chave["point_key"];
            string nivel = SessaoCenario.Atual.Nivel;
            int ordem = (int)chave["ordem"];
            if (!chaves.Add($"{npcKey}/{ponto}/{ordem}") || ExerciseManager.Instance == null)
                return false;
            var npc = FindObjectsByType<InteracaoNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .FirstOrDefault(n => n.Cenario == cenario && n.CheckpointNpcKey == npcKey);
            if (npc == null)
                return false;

            var catalogo = await ExerciseManager.Instance.LoadForPointAsync(npcKey, ponto, nivel, token);
            if (!SessaoCenario.Atual.Permite(cenario, visita) || token.IsCancellationRequested
                || !catalogo.Success || !catalogo.ByPoint.TryGetValue(ponto, out var exercicios))
                return false;
            int? id = (int?)chave["exercise_id"];
            var item = exercicios.SingleOrDefault(e => e.Exercise.ordemBanco == ordem
                && e.Exercise.nivelBanco == nivel && (!id.HasValue || e.Exercise.idExercise == id.Value));
            if (item == null)
                return false;

            var ex = item.Exercise.CopiarParaRevisaoCheckpoint();
            var dados = chave["context"];
            if (ex is ExercicioSpeaking speaking && npc is CashierNPC)
            {
                string saudacao = (string)dados?["greeting"];
                string total = (string)dados?["total"];
                if (saudacao != null && (ponto != "greeting" || ordem != 1
                    || (nivel != "A1-A2" && nivel != "A2" && nivel != "B1")
                    || (saudacao != "Good morning!" && saudacao != "Good afternoon!" && saudacao != "Good evening!")))
                    return false;
                if (total != null && (ponto != "purchase_total" || ordem != 2 || (nivel != "A2" && nivel != "B1")
                    || !System.Text.RegularExpressions.Regex.IsMatch(total, @"\A[0-9]{1,8}(?:[.,][0-9]{1,2})?\z")))
                    return false;
                speaking = CashierNPC.PreencherSpeaking(speaking, saudacao, total);
                bool TemMarcador(string texto) => texto?.Contains("{{total}}") == true || texto?.Contains("{{greeting}}") == true;
                if (TemMarcador(speaking.enunciado) || speaking.opcoesFala?.Any(TemMarcador) == true)
                    return false;
                ex = speaking;
            }
            if (ex is ExercicioBlocos blocos && npc is RecepcionistHotelNPC
                && ponto == "reservation" && nivel == "A2" && ordem == 1)
            {
                string periodo = (string)dados?["period"];
                if (periodo != "morning" && periodo != "afternoon" && periodo != "night")
                    return false;
                ex = RecepcionistHotelNPC.PreencherPeriodo(blocos, periodo);
            }
            if (ex is ExercicioWriting writing)
                writing.respostaJogador = null;

            itens.Add(new ExercicioRevisao { tipo = item.Tipo, exercicio = ex, contextoNpc = (string)dados?["npc_line"],
                cenario = cenario, npcOrigem = npc });
        }

        if (!SessaoCenario.Atual.Permite(cenario, visita) || token.IsCancellationRequested || visitaAtual != visita)
            return false;
        exerciciosErrados.AddRange(itens);
        foreach (var chave in revisados)
            revisadosCheckpoint.Add(chave.DeepClone());
        return true;
    }

    public bool TemRevisao(string cenario)
    {
        return exerciciosErrados.Exists(item =>
            item.cenario == cenario && !item.aguardandoAssociacao);
    }

    public async Task<bool> FazerRevisao(
        InteracaoNPC npcApresentador,
        ModalExercicio modalOverride = null,
        GameObject avisoRevisaoOverride = null,
        GameObject legendaOverride = null)
    {
        int visita = visitaAtual;
        string cenario = cenarioAtual;
        if (!SessaoCenario.Atual.Permite(cenario, visita))
        {
            Debug.LogError("Não é possível executar a revisão sem identificar o cenário atual.");
            return false;
        }

        if (npcApresentador == null || npcApresentador.Cenario != cenarioAtual)
        {
            Debug.LogError("Não é possível executar a revisão sem um NPC apresentador.");
            return false;
        }

        List<ExercicioRevisao> associacoes =
            GetAssociacoesPendentes(cenarioAtual);

        foreach (ExercicioRevisao item in associacoes)
        {
            ModalExercicio modalExercicio = ObterModalExercicio(modalOverride);
            if (modalExercicio == null)
                return false;

            GameObject modalLegenda = ObterModalLegenda(legendaOverride);
            InteracaoNPC npcDoExercicio = ObterNpcDoExercicio(
                item,
                npcApresentador
            );
            if (npcDoExercicio == null)
                return false;

            if (!await AbrirExercicioRevisao(
                    modalExercicio,
                    npcDoExercicio,
                    modalLegenda,
                    TipoExercicio.Associacao,
                    item.exercicio,
                    false,
                    true))
            {
                return false;
            }

            exerciciosErrados.Remove(item);
        }

        CheckpointJogo.Instance?.CapturarRevisao(cenario, visita);
        List<ExercicioRevisao> revisoes =
            GetExerciciosErrados(cenarioAtual);

        if (revisoes.Count > 0)
        {
            ModalExercicio modalExercicio = ObterModalExercicio(modalOverride);
            if (modalExercicio == null)
                return false;

            GameObject modalLegenda = ObterModalLegenda(legendaOverride);
            GameObject modalAvisoRevisao =
                ObterModalAvisoRevisao(avisoRevisaoOverride);

            if (modalAvisoRevisao != null)
            {
                modalAvisoRevisao.SetActive(true);

                while (modalAvisoRevisao != null && modalAvisoRevisao.activeSelf
                    && SessaoCenario.Atual.Permite(cenario, visita))
                    await Task.Yield();

                if (!SessaoCenario.Atual.Permite(cenario, visita))
                {
                    if (visitaAtual == visita && modalAvisoRevisao != null)
                        modalAvisoRevisao.SetActive(false);
                    return false;
                }
            }

            foreach (ExercicioRevisao item in revisoes)
            {
                string enunciadoOriginal = item.exercicio.enunciado;
                InteracaoNPC npcDoExercicio = ObterNpcDoExercicio(
                    item,
                    npcApresentador
                );
                if (npcDoExercicio == null)
                    return false;

                bool speakingLivre = item.tipo == TipoExercicio.Speaking
                    && item.exercicio is ExercicioSpeaking speaking && speaking.modoLivre;
                bool contextoVisual = item.tipo == TipoExercicio.Alternativas
                    || item.tipo == TipoExercicio.Writing
                    || (item.tipo == TipoExercicio.Speaking && !speakingLivre);
                if (speakingLivre)
                {
                    item.exercicio.enunciado =
                        "Context: " + item.contextoNpc + "\n\n" + enunciadoOriginal;
                }

                try
                {
                    if (!await AbrirExercicioRevisao(
                            modalExercicio,
                            npcDoExercicio,
                            modalLegenda,
                            item.tipo,
                            item.exercicio,
                            contextoVisual: contextoVisual ? item.contextoNpc : null))
                    {
                        return false;
                    }

                    exerciciosErrados.Remove(item);
                    var chave = CheckpointJogo.ChaveExercicio(item.exercicio, item.npcOrigem, false);
                    if (chave != null)
                        revisadosCheckpoint.Add(chave);
                }
                finally
                {
                    item.exercicio.enunciado = enunciadoOriginal;
                }

                CheckpointJogo.Instance?.CapturarRevisao(cenario, visita);
            }
        }

        Limpar(cenarioAtual);
        return true;
    }

    public async Task<bool> ExecutarRevisaoEExibirModalFinal(
        InteracaoNPC npcApresentador,
        ModalExercicio modalOverride = null,
        GameObject avisoRevisaoOverride = null,
        GameObject legendaOverride = null)
    {
        int visita = visitaAtual;
        if (!SessaoCenario.Atual.Permite(cenarioAtual, visita) || finalizacaoEmAndamento || cenarioFinalizado)
            return false;

        if (npcApresentador == null || npcApresentador.Cenario != cenarioAtual)
        {
            Debug.LogError("Não é possível finalizar o cenário sem um NPC apresentador.");
            return false;
        }

        GerenciadorProgressao gerenciador =
            GerenciadorProgressao.Instance;
        if (gerenciador == null || gerenciador.modalFinalManager == null)
        {
            Debug.LogError("GerenciadorProgressao ou ModalFinalManager não está configurado.");
            return false;
        }

        FirstPlayerController jogador = npcApresentador.movimentoPlayer;
        if (jogador == null)
            jogador = FindObjectOfType<FirstPlayerController>();

        if (jogador == null)
        {
            Debug.LogError("FirstPlayerController não está disponível para bloquear o jogador.");
            return false;
        }

        finalizacaoEmAndamento = true;
        GameProgress.EstaEmDialogo = true;
        jogador.DesativarControle();

        bool revisaoConcluida = false;
        try
        {
            revisaoConcluida = await FazerRevisao(npcApresentador, modalOverride, avisoRevisaoOverride, legendaOverride);
        }
        finally
        {
            if (SessaoCenario.Atual.Permite(cenarioAtual, visita) && !cenarioFinalizado)
            {
                finalizacaoEmAndamento = false;
                GameProgress.EstaEmDialogo = false;
                jogador.AtivarControle();

                if (npcApresentador.crosshair != null)
                    npcApresentador.crosshair.SetActive(true);
            }
        }

        if (!revisaoConcluida || !SessaoCenario.Atual.Permite(cenarioAtual, visita))
            return false;

        cenarioFinalizado = true;
        gerenciador.modalFinalManager.MostrarResultadoDaVisita(
            visita, "ContinuarCenarios",
            DadosJogador.nivelUsuario,
            DadosJogador.nivelUsuario
        );

        return true;
    }

    public void Limpar(string cenario)
    {
        exerciciosErrados.RemoveAll(item => item.cenario == cenario);
    }

    private bool ValidarContextoRegistro(
        ExercicioBase exercicio,
        InteracaoNPC npcOrigem)
    {
        if (!SessaoCenario.Atual.Permite(cenarioAtual, visitaAtual))
        {
            Debug.LogError("Não é possível registrar exercício sem identificar o cenário atual.");
            return false;
        }

        if (exercicio == null)
        {
            Debug.LogError("Não é possível registrar um exercício nulo para revisão.");
            return false;
        }

        if (npcOrigem == null || npcOrigem.Cenario != cenarioAtual)
        {
            Debug.LogError("Não é possível registrar exercício sem identificar o NPC de origem.");
            return false;
        }

        return true;
    }

    private ModalExercicio ObterModalExercicio(ModalExercicio modalOverride)
    {
        ModalExercicio modal = modalOverride;
        if (modal == null)
            modal = FindObjectOfType<ModalExercicio>();

        if (modal == null || modal.panel == null)
        {
            Debug.LogError("ModalExercicio ou painel não está configurado para a revisão.");
            return null;
        }

        return modal;
    }

    private GameObject ObterModalAvisoRevisao(GameObject modalOverride)
    {
        if (modalOverride != null)
            return modalOverride;

        ModalBoasVindasGerenciador gerenciadorBoasVindas =
            FindObjectOfType<ModalBoasVindasGerenciador>();

        if (gerenciadorBoasVindas == null)
            return null;

        return gerenciadorBoasVindas.modalAvisoRevisao;
    }

    private GameObject ObterModalLegenda(GameObject legendaOverride)
    {
        if (legendaOverride != null)
            return legendaOverride;

        ModalLegenda legenda = FindObjectOfType<ModalLegenda>();
        return legenda != null ? legenda.gameObject : null;
    }

    private InteracaoNPC ObterNpcDoExercicio(
        ExercicioRevisao exercicio,
        InteracaoNPC npcApresentador)
    {
        if (cenarioAtual == "Market")
            return npcApresentador;

        if (exercicio.npcOrigem == null)
        {
            Debug.LogError(
                "O exercício de revisão não tem NPC de origem associado."
            );
            return null;
        }

        return exercicio.npcOrigem;
    }

    private async Task<bool> AbrirExercicioRevisao(
        ModalExercicio modalExercicio,
        InteracaoNPC npcApresentador,
        GameObject modalLegenda,
        TipoExercicio tipo,
        ExercicioBase exercicio,
        bool revisao = true,
        bool faseAssociacao = false,
        string contextoVisual = null)
    {
        if (modalLegenda != null)
            modalLegenda.SetActive(false);

        RectTransform rect = modalExercicio.panel.GetComponent<RectTransform>();
        if (rect != null && tipo != TipoExercicio.Associacao)
        {
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
        }

        modalExercicio.Abrir(
            tipo,
            exercicio,
            npcApresentador,
            revisao,
            faseAssociacao,
            contextoVisual);

        int abertura = modalExercicio.VersaoAbertura;
        try
        {
            return await AguardarExercicioRevisao(modalExercicio);
        }
        finally
        {
            if (contextoVisual != null && modalExercicio != null)
                modalExercicio.RestaurarEnunciadoExibido(abertura);
        }
    }

    private async Task<bool> AguardarExercicioRevisao(ModalExercicio modalExercicio)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        int visita = visitaAtual;
        while (modalExercicio != null && !modalExercicio.exercicioFinalizado && SessaoCenario.Atual.Permite(cenarioAtual, visita))
            await Task.Yield();

        if (modalExercicio == null || !SessaoCenario.Atual.Permite(cenarioAtual, visita))
            return false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        return true;
    }
}
