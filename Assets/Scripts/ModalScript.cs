using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public class ModalExercicio : MonoBehaviour
{
    public bool exercicioFinalizado = false;

    private TipoExercicio tipoAtual;
    private ExercicioBase exercicioAtual;
    private InteracaoNPC npcAtual;
    private bool aguardandoOkFinal = false;
    private string textoOriginalBotao = "Confirm your Answer";
    private bool modoRevisao = false;

    public Vector2 posicaoLegendaRepeticao = new Vector2(350f, 50f);

    public GameObject panel;
    public Button confirmarResposta;
    public TextMeshProUGUI titulo, enunciado;

    public Button botaoRepetirFalaNPC; 
    public GameObject modalAvisoCampoVazio;

    public GameObject exercicioAlternativas;
    public GameObject exercicioSpeaking;
    public GameObject exercicioBlocos;
    public GameObject exercicioWriting;
    public GameObject exercicioAssociacao;
    public GameObject crosshair;

    public AlternativasUI alternativasUI;
    public SpeakingUI speakingUI;
    public BlocosUI blocosUI;
    public WritingUI writingUI;
    public AssociacaoUI associacaoUI;

    void Start()
    {
        panel.SetActive(false);
        if (modalAvisoCampoVazio != null)
            modalAvisoCampoVazio.SetActive(false);

        if (confirmarResposta != null)
        {
            confirmarResposta.onClick.AddListener(() => Confirmar());
        }

        if (botaoRepetirFalaNPC != null)
        {
            botaoRepetirFalaNPC.onClick.RemoveAllListeners();
            botaoRepetirFalaNPC.onClick.AddListener(RepetirUltimaFala);
            botaoRepetirFalaNPC.gameObject.SetActive(false); 
        }
    }

    public async void Confirmar()
    {
        if (tipoAtual == TipoExercicio.Associacao &&
            exercicioAssociacao != null &&
            exercicioAssociacao.activeSelf)
        {
            if (associacaoUI == null || !associacaoUI.PodeContinuar)
                return;

            if (!associacaoUI.TodasAssociacoesConcluidas)
                RegistrarErroParaRevisao();
            else if (ProgressoNivelManager.Instance != null)
                ProgressoNivelManager.Instance.RegistrarAcerto(exercicioAtual);

            if (RevisaoManager.Instance != null)
                RevisaoManager.Instance.ConcluirAssociacao(exercicioAtual);

            Fechar();
            return;
        }

        if (aguardandoOkFinal)
        {
            aguardandoOkFinal = false;
            Fechar();
        }
        else
        {
            bool acertouExercicio = false;
            confirmarResposta.interactable = false;

            if (exercicioAlternativas.activeSelf)
            {
                if (!alternativasUI.Respondeu())
                {
                    Debug.Log("Escolha uma alternativa primeiro");
                    confirmarResposta.interactable = true;
                    return;
                }
                else
                {
                    if (alternativasUI.EstaCorreto())
                    {
                        alternativasUI.MostrarResultadoVisual();
                        alternativasUI.MostrarFeedback("Good job! You got the answer right.");
                        alternativasUI.BloquearAlternativas();

                        Debug.Log("Good. You got the answer right!");
                        acertouExercicio = true;
                    }
                    else
                    {
                        alternativasUI.MostrarResultadoVisual();
                        alternativasUI.ReduzirTentativa();
                        int chances = alternativasUI.ObterTentativasRestantes();

                        Debug.Log($"Resposta incorreta nas Alternativas. Tentativas restantes: {chances}");

                        if (!modoRevisao)
                        {
                            if (chances == 1 && botaoRepetirFalaNPC != null)
                            {
                                Debug.Log("Errou pela 2ª vez nas Alternativas! Ativando botão de repetição...");
                                botaoRepetirFalaNPC.gameObject.SetActive(true);
                            }
                        }

                        if (chances > 0)
                        {
                            confirmarResposta.interactable = true;
                            return;
                        }
                        else
                        {
                            alternativasUI.MostrarFeedback("You have used all your chances. You can try again later in the revision section.");
                            alternativasUI.BloquearAlternativas();

                            RegistrarErroParaRevisao();
                        }
                    }
                }
            }
            else if (exercicioSpeaking.activeSelf)
            {
                if (speakingUI != null)
                {
                    SpeakingResult resultado = await speakingUI.VerificarRespostaWhisper();

                    if (resultado == null)
                    {
                        Debug.LogError("Falha ao se comunicar com o servidor de Voz.");
                        confirmarResposta.interactable = true;
                        return;
                    }

                    if (speakingUI.tipoSpeaking == SpeakingUI.TipoSpeaking.Livre)
                    {
                        speakingUI.MostrarFeedbackSpeakingLivre(resultado.feedback, resultado.correto);

                        if (!resultado.correto)
                        {
                            speakingUI.ReduzirTentativa();
                            int chances = speakingUI.ObterTentativasRestantes();

                            MostrarBotaoRepetirNaUltimaTentativa(chances);

                            if (chances > 0)
                            {
                                confirmarResposta.interactable = true;
                                return;
                            }

                            if (speakingUI.statusGravacao != null)
                                speakingUI.statusGravacao.text = "No attempts remaining.";
                            speakingUI.BloquearSpeaking();
                            RegistrarErroParaRevisao();
                        }
                        else
                        {
                            speakingUI.BloquearSpeaking();
                            acertouExercicio = true;
                        }
                    }
                    else
                    {
                        int respostaCorretaDoExercicio = speakingUI.ObterIndiceCorreto();

                        if (resultado.indice_detectado != respostaCorretaDoExercicio)
                        {
                            speakingUI.ReduzirTentativa();
                            int chances = speakingUI.ObterTentativasRestantes();

                            string erroTexto = "Você escolheu ou pronunciou a alternativa errada. Tente responder novamente!";

                            // Não mostra palavras erradas quando a opção detectada foi outra
                            speakingUI.AtualizarTextoFeedback(erroTexto, null);

                            if (!modoRevisao)
                            {
                                if (chances == 1 && botaoRepetirFalaNPC != null)
                                    botaoRepetirFalaNPC.gameObject.SetActive(true);
                            }

                            if (chances > 0)
                            {
                                confirmarResposta.interactable = true;
                                return;
                            }
                            else
                            {
                                speakingUI.MostrarFeedbackFimTentativas();
                                speakingUI.BloquearSpeaking();
                                RegistrarErroParaRevisao();
                            }
                        }
                        else if (resultado.acuracia < 70f)
                        {
                            speakingUI.ReduzirTentativa();
                            int chances = speakingUI.ObterTentativasRestantes();

                            speakingUI.MostrarFeedbackPronuncia(resultado.palavras_erradas, false);

                            if (chances > 0)
                            {
                                confirmarResposta.interactable = true;
                                return;
                            }
                            else
                            {
                                speakingUI.MostrarFeedbackFimTentativas();
                                speakingUI.BloquearSpeaking();
                                RegistrarErroParaRevisao();
                            }
                        }
                        else
                        {
                            speakingUI.MostrarFeedbackPronuncia(resultado.palavras_erradas, true);
                            speakingUI.BloquearSpeaking();
                            acertouExercicio = true;
                        }
                    }
                }
            }
            else if (exercicioBlocos.activeSelf)
            {
                if (blocosUI != null)
                {
                    bool correto = blocosUI.VerificarResposta();

                    if (correto)
                    {
                        Debug.Log("Exercicio acertado!");

                        blocosUI.BloquearBlocos();

                        acertouExercicio = true;
                    }
                    else
                    {
                        int chances = blocosUI.ObterTentativasRestantes();

                        if (chances > 0)
                        {
                            confirmarResposta.interactable = true;
                            return;
                        }
                        else
                        {
                            blocosUI.BloquearBlocos();

                            if (blocosUI.textoFeedback != null)
                            {
                                blocosUI.textoFeedback.text +=
                                    "\nYou have used all your chances. You can try again later in the revision section.";
                            }

                            RegistrarErroParaRevisao();

                            Debug.Log("Acabaram as chances nos blocos.");
                        }
                    }
                }
            }
            else if (exercicioWriting != null && exercicioWriting.activeSelf)
            {
                if (writingUI == null)
                {
                    Debug.LogError("Associe o WritingUI no ModalExercicio.");
                    confirmarResposta.interactable = true;
                    return;
                }

                if (!writingUI.SalvarResposta())
                {
                    if (writingUI.RespostaVazia() && modalAvisoCampoVazio != null)
                        modalAvisoCampoVazio.SetActive(true);

                    confirmarResposta.interactable = true;
                    return;
                }

                BackendManager backendManager = new BackendManager();
                string respostaJson = await backendManager.AvaliarWriting(
                    exercicioAtual.enunciado,
                    ((ExercicioWriting)exercicioAtual).respostaJogador,
                    DadosJogador.nivelUsuario
                );

                if (string.IsNullOrWhiteSpace(respostaJson))
                {
                    writingUI.MostrarFeedback("The answer could not be evaluated. Please try again.");
                    confirmarResposta.interactable = true;
                    return;
                }

                SpeakingResult resultadoWriting = JsonUtility.FromJson<SpeakingResult>(respostaJson);
                if (resultadoWriting == null || string.IsNullOrWhiteSpace(resultadoWriting.feedback))
                {
                    Debug.LogError("Resposta inválida recebida do servidor de Writing.");
                    writingUI.MostrarFeedback("The answer could not be evaluated. Please try again.");
                    confirmarResposta.interactable = true;
                    return;
                }

                writingUI.MostrarFeedback(resultadoWriting.feedback);
                if (!resultadoWriting.correto)
                {
                    writingUI.ReduzirTentativa();
                    int chances = writingUI.ObterTentativasRestantes();

                    MostrarBotaoRepetirNaUltimaTentativa(chances);

                    if (chances > 0)
                    {
                        confirmarResposta.interactable = true;
                        return;
                    }

                    writingUI.MostrarFeedback(
                        resultadoWriting.feedback +
                        "\nYou have used all your chances. You can try again later in the revision section."
                    );
                    writingUI.BloquearResposta();
                    RegistrarErroParaRevisao();
                }
                else
                {
                    writingUI.BloquearResposta();
                    acertouExercicio = true;
                }
            }
            if (acertouExercicio)
            {
                if (ProgressoNivelManager.Instance != null)
                {
                    ProgressoNivelManager.Instance.RegistrarAcerto(exercicioAtual);
                }
            }
            confirmarResposta.interactable = true;
            MudarBotaoParaOK();
        }
    }

    private void MudarBotaoParaOK()
    {
        aguardandoOkFinal = true;

        if (confirmarResposta != null)
        {
            confirmarResposta.interactable = true;

            TextMeshProUGUI textoBotao = confirmarResposta.GetComponentInChildren<TextMeshProUGUI>();
            if (textoBotao != null)
            {
                textoBotao.text = "OK";
            }
        }
    }

    private void MostrarBotaoRepetirNaUltimaTentativa(int tentativasRestantes)
    {
        if (modoRevisao ||
            tentativasRestantes != 1 ||
            botaoRepetirFalaNPC == null ||
            npcAtual == null ||
            string.IsNullOrWhiteSpace(npcAtual.ultimaFraseDita))
            return;

        botaoRepetirFalaNPC.gameObject.SetActive(true);
    }

    private async void RepetirUltimaFala()
    {
        if (npcAtual == null) return;

        botaoRepetirFalaNPC.interactable = false;

        // esconde legenda grande
        ModalLegenda legendaGrande = FindObjectOfType<ModalLegenda>();

        if (legendaGrande != null)
            legendaGrande.gameObject.SetActive(false);

        // mostra legenda pequena
        if (speakingUI != null && speakingUI.modalLegendaNPC != null)
        {
            GameObject legendaPequena = speakingUI.modalLegendaNPC;

            legendaPequena.SetActive(true);
            legendaPequena.transform.SetAsLastSibling();

            RectTransform rect = legendaPequena.GetComponent<RectTransform>();

            ModalLegenda legenda =
                legendaPequena.GetComponent<ModalLegenda>();

            legenda.fonteLegenda.text = npcAtual.nomeExibicaoLegenda;
            legenda.textoLegenda.text = npcAtual.ultimaFraseDita;
        }

        await npcAtual.FalarFraseCustomizada(npcAtual.ultimaFraseDita);

        botaoRepetirFalaNPC.interactable =  true;
    }

    private void RegistrarErroParaRevisao()
    {
        if (!modoRevisao)
        {
            if (RevisaoManager.Instance != null)
            {
                if (exercicioAtual != null)
                {
                    string contexto = "";

                    if (tipoAtual == TipoExercicio.Speaking ||
                        tipoAtual == TipoExercicio.Alternativas ||
                        tipoAtual == TipoExercicio.Writing)
                    {
                        if (npcAtual != null)
                        {
                            contexto = npcAtual.ultimaFraseDita;
                        }
                    }

                    RevisaoManager.Instance.RegistrarErro(
                        tipoAtual,
                        exercicioAtual,
                        contexto
                    );
                }
            }
        }
    }

    public void Abrir(
        TipoExercicio tipo,
        ExercicioBase ex,
        InteracaoNPC npc,
        bool revisao = false,
        bool faseAssociacao = false)
    {
        tipoAtual = tipo;
        exercicioAtual = ex;
        npcAtual = npc;
        modoRevisao = revisao;

        if (tipo == TipoExercicio.Associacao && !revisao && !faseAssociacao)
        {
            if (RevisaoManager.Instance != null)
            {
                RevisaoManager.Instance.RegistrarAssociacao(ex as ExercicioAssociacao);
                exercicioFinalizado = true;
                return;
            }

            Debug.LogError("RevisaoManager não está disponível para adiar o exercício de associação.");
        }

        if (tipo == TipoExercicio.Associacao)
            GarantirPainelAssociacao();

        if (enunciado != null && ex != null)
        {
            enunciado.text = ex.enunciado;
        }

        if (crosshair != null)
        {
            crosshair.SetActive(false);
        }

        exercicioFinalizado = false;
        panel.SetActive(true);

        aguardandoOkFinal = false;

        if (confirmarResposta != null)
        {
            TextMeshProUGUI textoBotao = confirmarResposta.GetComponentInChildren<TextMeshProUGUI>();
            if (textoBotao != null)
                textoBotao.text = tipo == TipoExercicio.Associacao ? "Continue" : textoOriginalBotao;

            confirmarResposta.interactable = tipo != TipoExercicio.Associacao;
        }

        if (exercicioAlternativas != null)
            exercicioAlternativas.SetActive(false);
        if (exercicioSpeaking != null)
            exercicioSpeaking.SetActive(false);
        if (exercicioBlocos != null)
            exercicioBlocos.SetActive(false);
        if (exercicioWriting != null)
            exercicioWriting.SetActive(false);
        if (exercicioAssociacao != null)
            exercicioAssociacao.SetActive(false);

        if (botaoRepetirFalaNPC != null)
        {
            botaoRepetirFalaNPC.gameObject.SetActive(false);
        }
        if (modalAvisoCampoVazio != null)
            modalAvisoCampoVazio.SetActive(false);

        if (tipo == TipoExercicio.Alternativas)
        {
            titulo.text = "Multiple Choice Exercise";
            exercicioAlternativas.SetActive(true);
            alternativasUI.Setup((ExercicioAlternativas)ex);
        }
        else if (tipo == TipoExercicio.Speaking)
        {
            titulo.text = "Speaking Exercise";
            exercicioSpeaking.SetActive(true);
            speakingUI.InicializarExercicio((ExercicioSpeaking)ex, npc);

            if (modoRevisao)
            {
                if (speakingUI.botaoRepetirFalaNPC != null)
                {
                    speakingUI.botaoRepetirFalaNPC.gameObject.SetActive(false);
                }
            }
        }
        else if (tipo == TipoExercicio.Blocos)
        {
            titulo.text = "Ordering Exercise";
            exercicioBlocos.SetActive(true);
            blocosUI.InicializarExercicio((ExercicioBlocos)ex);
        }
        else if (tipo == TipoExercicio.Writing)
        {
            if (exercicioWriting == null || writingUI == null)
                throw new System.InvalidOperationException("Configure o painel e o WritingUI no ModalExercicio.");

            titulo.text = "Writing Exercise";
            exercicioWriting.SetActive(true);
            writingUI.InicializarExercicio((ExercicioWriting)ex);
        }
        else if (tipo == TipoExercicio.Associacao)
        {
            titulo.text = "Matching Exercise";
            exercicioAssociacao.SetActive(true);
            associacaoUI.EstadoFinalizado -= AtualizarBotaoContinueAssociacao;
            associacaoUI.EstadoFinalizado += AtualizarBotaoContinueAssociacao;
            if (!associacaoUI.InicializarExercicio((ExercicioAssociacao)ex))
                Debug.LogError("Não foi possível inicializar o exercício de associação.");
        }
    }

    private void GarantirPainelAssociacao()
    {
        if (exercicioAssociacao == null)
        {
            GameObject painel = new GameObject(
                "ExercicioAssociacao",
                typeof(RectTransform));
            painel.transform.SetParent(panel.transform, false);

            RectTransform rect = painel.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            exercicioAssociacao = painel;
        }

        if (associacaoUI == null)
            associacaoUI = exercicioAssociacao.GetComponent<AssociacaoUI>();
        if (associacaoUI == null)
            associacaoUI = exercicioAssociacao.AddComponent<AssociacaoUI>();
    }

    private void AtualizarBotaoContinueAssociacao()
    {
        if (confirmarResposta != null && associacaoUI != null)
            confirmarResposta.interactable = associacaoUI.PodeContinuar;
    }

    public void Fechar()
    {
        exercicioFinalizado = true;
        panel.SetActive(false);
        Time.timeScale = 1f;

        // Esconde a legenda pequena do Repeat
        if (speakingUI != null && speakingUI.modalLegendaNPC != null)
        {
            speakingUI.modalLegendaNPC.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void FinalizarExercicio()
    {
        Fechar();
    }
}