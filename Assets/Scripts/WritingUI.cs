using TMPro;
using UnityEngine;

public class WritingUI : MonoBehaviour
{
    public TMP_InputField campoResposta;
    public TextMeshProUGUI textoFeedback;

    private ExercicioWriting exercicioAtual;
    private int tentativasRestantes;

    public void InicializarExercicio(ExercicioWriting exercicio)
    {
        if (exercicio == null)
        {
            Debug.LogError("O WritingUI recebeu um exercício nulo.");
            return;
        }

        exercicioAtual = exercicio;
        tentativasRestantes = 3;

        if (campoResposta == null)
        {
            Debug.LogError("Associe o campo de resposta do WritingUI no Inspector.");
            return;
        }

        campoResposta.interactable = true;
        campoResposta.text = exercicio.respostaJogador ?? string.Empty;
        if (textoFeedback != null)
            textoFeedback.text = string.Empty;
    }

    public bool SalvarResposta()
    {
        if (exercicioAtual == null)
        {
            Debug.LogError("Inicialize o WritingUI com um exercício antes de salvar a resposta.");
            return false;
        }

        if (campoResposta == null)
        {
            Debug.LogError("Associe o campo de resposta do WritingUI no Inspector.");
            return false;
        }

        string resposta = campoResposta.text == null ? string.Empty : campoResposta.text.Trim();
        if (string.IsNullOrWhiteSpace(resposta))
        {
            if (textoFeedback != null)
                textoFeedback.text = "Write an answer before continuing.";
            return false;
        }

        exercicioAtual.respostaJogador = resposta;
        return true;
    }

    public bool RespostaVazia()
    {
        return campoResposta != null && string.IsNullOrWhiteSpace(campoResposta.text);
    }

    public void ReduzirTentativa()
    {
        tentativasRestantes--;
    }

    public int ObterTentativasRestantes()
    {
        return tentativasRestantes;
    }

    public void MostrarFeedback(string mensagem)
    {
        if (textoFeedback != null)
            textoFeedback.text = mensagem;
    }

    public void BloquearResposta()
    {
        if (campoResposta != null)
            campoResposta.interactable = false;
    }
}
