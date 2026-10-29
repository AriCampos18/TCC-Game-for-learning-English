using TMPro;
using UnityEngine;

public class WritingUI : MonoBehaviour
{
    public TMP_InputField campoResposta;
    public TextMeshProUGUI textoFeedback;

    private ExercicioWriting exercicioAtual;

    public void InicializarExercicio(ExercicioWriting exercicio)
    {
        exercicioAtual = exercicio;

        if (campoResposta == null)
        {
            Debug.LogError("Associe o campo de resposta do WritingUI no Inspector.");
            return;
        }

        campoResposta.text = exercicio.respostaJogador ?? string.Empty;
        if (textoFeedback != null)
            textoFeedback.text = string.Empty;
    }

    public bool SalvarResposta()
    {
        if (exercicioAtual == null || campoResposta == null)
            return false;

        string resposta = campoResposta.text.Trim();
        if (string.IsNullOrWhiteSpace(resposta))
        {
            if (textoFeedback != null)
                textoFeedback.text = "Write an answer before continuing.";
            return false;
        }

        exercicioAtual.respostaJogador = resposta;
        if (textoFeedback != null)
            textoFeedback.text = "Answer saved.";
        return true;
    }
}
