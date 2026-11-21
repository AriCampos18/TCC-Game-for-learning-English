using System.Collections.Generic;
using System.Threading.Tasks;
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

                if (item.tipo == TipoExercicio.Speaking
                    || item.tipo == TipoExercicio.Alternativas)
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
                            item.exercicio))
                    {
                        return false;
                    }

                    exerciciosErrados.Remove(item);
                }
                finally
                {
                    item.exercicio.enunciado = enunciadoOriginal;
                }
            }
        }

        Limpar(cenarioAtual);
        return true;
    }

    public async Task<bool> ExecutarRevisaoEExibirModalFinal(
        InteracaoNPC npcApresentador)
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
            revisaoConcluida = await FazerRevisao(npcApresentador);
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
        bool faseAssociacao = false)
    {
        if (modalLegenda != null)
            modalLegenda.SetActive(false);

        RectTransform rect = modalExercicio.panel.GetComponent<RectTransform>();
        if (rect != null)
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
            faseAssociacao);

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
