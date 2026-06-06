using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public class ModalExercicio : MonoBehaviour
{
    public bool exercicioFinalizado = false;
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
            confirmarResposta.onClick.AddListener(() => Confirmar()); // Sintaxe lambda para chamar a função
    }

    public async void Confirmar()
    {
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
                }
                else
                {
                    alternativasUI.ReduzirTentativa();
                    int chances = alternativasUI.ObterTentativasRestantes();

                    Debug.Log($"Resposta incorreta nas Alternativas. Tentativas restantes: {chances}");

                    // ✨ Ativa a ajuda ao errar pela 2ª vez (resta 1 chance de 3)
                    if (chances == 1 && botaoRepetirVoz != null)
                    {
                        Debug.Log("Errou pela 2ª vez nas Alternativas! Ativando botão de repetição...");
                        botaoRepetirVoz.SetActive(true);
                    }

                    if (chances > 0)
                    {
                        confirmarResposta.interactable = true;
                        return;
                    }
                    else
                    {
                        Debug.Log("Acabaram as chances das Alternativas, fechando exercício...");
                    }
                }
            }
        }
        else if (exercicioSpeaking.activeSelf)
        {
            if (speakingUI != null)
            {
                // 1. Chama a verificação que envia o áudio ao backend e aguarda o JSON
                SpeakingResult resultado = await speakingUI.VerificarRespostaWhisper();

                if (resultado == null)
                {
                    Debug.LogError("Falha ao se comunicar com o servidor de Voz.");
                    confirmarResposta.interactable = true;
                    return;
                }

                int respostaCorretaDoExercicio = speakingUI.ObterIndiceCorreto();

                // Regra 1: O usuário tentou falar uma das opções erradas
                if (resultado.indice_detectado != respostaCorretaDoExercicio)
                {
                    speakingUI.ReduzirTentativa();
                    int chances = speakingUI.ObterTentativasRestantes();
                    
                    string erroTexto = "Você escolheu ou pronunciou a alternativa errada. Tente responder novamente!";
                    speakingUI.AtualizarTextoFeedback(erroTexto, resultado.palavras_erradas);
                    
                    Debug.Log($"Opção errada detectada ({resultado.indice_detectado}). Esperada: {respostaCorretaDoExercicio}");

                    if (chances == 1 && botaoRepetirVoz != null)
                        botaoRepetirVoz.SetActive(true);

                    if (chances > 0)
                    {
                        confirmarResposta.interactable = true;
                        return; // Trava a tela para tentar de novo
                    }
                }
                // Regra 2: Acertou a alternativa, mas a pronúncia/acurácia foi baixa (70% ou menos)
                else if (resultado.indice_detectado == respostaCorretaDoExercicio && resultado.acuracia <= 70f)
                {
                    string feedbackFeedback = $"A alternativa está correta! Mas sua acurácia foi de {resultado.acuracia}%. Vamos repetir para praticar a pronúncia?";
                    speakingUI.AtualizarTextoFeedback(feedbackFeedback, resultado.palavras_erradas);
                    
                    confirmarResposta.interactable = true;
                    return; // Retorna sem fechar o modal, obrigando a gravar de novo
                }
                // Regra 3: Acertou a alternativa e a acurácia foi excelente (> 70%)
                else
                {
                    Debug.Log($"Speaking completado com sucesso! Acurácia: {resultado.acuracia}%");
                    // ✨ Ajuste: Mostra a mensagem de sucesso na UI antes de fechar
                    speakingUI.MostrarSucessoNativo(resultado.acuracia); 
                    
                    // Pequeno delay opcional para o usuário ver que acertou antes do modal sumir
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
                        Debug.Log("Acabaram as chances, fechando exercício e continuando o papo...");
                    }
                }
            }
        }

        // Fecha o modal se acertou OU se esgotou as 3 chances
        confirmarResposta.interactable = true;
        Fechar();
    }

    public void Abrir(TipoExercicio tipo, ExercicioBase ex, InteracaoNPC npc)
    {
        crosshair.SetActive(false); // ✨ Esconde a mira ao abrir o modal de exercício

         // ✨ Correção geral: Garante que o modal de legenda esteja fechado ao abrir um exercício, para evitar sobreposição de UI
        exercicioFinalizado = false;
        panel.SetActive(true);

        exercicioAlternativas.SetActive(false);
        exercicioSpeaking.SetActive(false);
        exercicioBlocos.SetActive(false);

        if (tipo == TipoExercicio.Alternativas)
        {
            exercicioAlternativas.SetActive(true);
            alternativasUI.Setup((ExercicioAlternativas)ex);
        }
        else if (tipo == TipoExercicio.Speaking)
        {
            exercicioSpeaking.SetActive(true);
            // ✨ Repassa o NPC genérico para a UI de fala
            speakingUI.InicializarExercicio((ExercicioSpeaking)ex, npc); 
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