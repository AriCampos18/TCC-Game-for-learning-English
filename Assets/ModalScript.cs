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
    private bool modoRevisao = false;

    public GameObject panel;
    public Button confirmarResposta;
    public TextMeshProUGUI titulo;

    public GameObject botaoRepetirVoz;

    public GameObject exercicioAlternativas;
    public GameObject exercicioSpeaking;
    public GameObject exercicioBlocos;
    public GameObject crosshair;

    public AlternativasUI alternativasUI;
    public SpeakingUI speakingUI;
    public BlocosUI blocosUI;

    void Start()
    {
        panel.SetActive(false);

        if (confirmarResposta != null)
        {
            confirmarResposta.onClick.AddListener(() => Confirmar());
        }
    }

    public async void Confirmar()
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
                    Debug.Log("Good. You got the answer right!");
                    acertouExercicio = true;
                }
                else
                {
                    alternativasUI.ReduzirTentativa();
                    int chances = alternativasUI.ObterTentativasRestantes();

                    Debug.Log($"Resposta incorreta nas Alternativas. Tentativas restantes: {chances}");

                    if (!modoRevisao)
                    {
                        if (chances == 1 && botaoRepetirVoz != null)
                        {
                            Debug.Log("Errou pela 2ª vez nas Alternativas! Ativando botão de repetição...");
                            botaoRepetirVoz.SetActive(true);
                        }
                    }

                    if (chances > 0)
                    {
                        confirmarResposta.interactable = true;
                        return;
                    }
                    else
                    {
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

                int respostaCorretaDoExercicio = speakingUI.ObterIndiceCorreto();

                if (resultado.indice_detectado != respostaCorretaDoExercicio)
                {
                    speakingUI.ReduzirTentativa();
                    int chances = speakingUI.ObterTentativasRestantes();

                    string erroTexto = "Você escolheu ou pronunciou a alternativa errada. Tente responder novamente!";
                    speakingUI.AtualizarTextoFeedback(erroTexto, resultado.palavras_erradas);

                    Debug.Log($"Opção errada detectada ({resultado.indice_detectado}). Esperada: {respostaCorretaDoExercicio}");

                    if (!modoRevisao)
                    {
                        if (chances == 1 && botaoRepetirVoz != null)
                        {
                            botaoRepetirVoz.SetActive(true);
                        }
                    }

                    if (chances > 0)
                    {
                        confirmarResposta.interactable = true;
                        return;
                    }
                    else
                    {
                        RegistrarErroParaRevisao();
                    }
                }
                else if (resultado.indice_detectado == respostaCorretaDoExercicio && resultado.acuracia <= 70f)
                {
                    string feedbackTexto = $"A alternativa está correta! Mas sua acurácia foi de {resultado.acuracia}%. Vamos repetir para praticar a pronúncia?";
                    speakingUI.AtualizarTextoFeedback(feedbackTexto, resultado.palavras_erradas);

                    confirmarResposta.interactable = true;
                    return;
                }
                else
                {
                    Debug.Log($"Speaking completado com sucesso! Acurácia: {resultado.acuracia}%");
                    speakingUI.MostrarSucessoNativo(resultado.acuracia);

                    acertouExercicio = true;

                    await Task.Delay(1500);
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
                        RegistrarErroParaRevisao();
                        Debug.Log("Acabaram as chances, fechando exercício e continuando o papo...");
                    }
                }
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
        Fechar();
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

                    if (tipoAtual == TipoExercicio.Speaking || tipoAtual == TipoExercicio.Alternativas)
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

    public void Abrir(TipoExercicio tipo, ExercicioBase ex, InteracaoNPC npc, bool revisao = false)
    {
        tipoAtual = tipo;
        exercicioAtual = ex;
        npcAtual = npc;
        modoRevisao = revisao;

        if (crosshair != null)
        {
            crosshair.SetActive(false);
        }

        exercicioFinalizado = false;
        panel.SetActive(true);

        exercicioAlternativas.SetActive(false);
        exercicioSpeaking.SetActive(false);
        exercicioBlocos.SetActive(false);

        if (botaoRepetirVoz != null)
        {
            botaoRepetirVoz.SetActive(false);
        }

        if (tipo == TipoExercicio.Alternativas)
        {
            exercicioAlternativas.SetActive(true);
            alternativasUI.Setup((ExercicioAlternativas)ex);
        }
        else if (tipo == TipoExercicio.Speaking)
        {
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
            exercicioBlocos.SetActive(true);
            blocosUI.InicializarExercicio((ExercicioBlocos)ex);
        }
    }

    public void Fechar()
    {
        exercicioFinalizado = true;
        panel.SetActive(false);
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void FinalizarExercicio()
    {
        Fechar();
    }
}