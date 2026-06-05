using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BlocosUI : MonoBehaviour
{
    public GeradorDeBlocos gerador;
    public TextMeshProUGUI textoEnunciado;
    public TextMeshProUGUI textoFeedback; // Arraste seu texto de feedback aqui no Inspector

    private ExercicioBlocos exAtual;
    private int tentativasRestantes = 3;
    private string feedbackAtualString = ""; // Guarda o texto do feedback para o NPC falar

    public void InicializarExercicio(ExercicioBlocos exercicio)
    {
        if (exercicio != null)
        {
            exAtual = exercicio;
            tentativasRestantes = 3;
            feedbackAtualString = "";

            if (textoFeedback != null) textoFeedback.text = "";
            if (textoEnunciado != null) textoEnunciado.text = exercicio.enunciado;
            
            string[] palavrasEmbaralhadas = exercicio.blocosPalavras.ToArray();
            gerador.CriarBlocosDasPalavras(palavrasEmbaralhadas);
        }
    }

    // Método para o ModalExercicio pegar o texto que o NPC deve falar
    public string ObterTextoFeedback()
    {
        return feedbackAtualString;
    }

    public int ObterTentativasRestantes()
    {
        return tentativasRestantes;
    }

    public bool VerificarResposta()
    {
        if (exAtual == null || gerador == null || gerador.containerMontagem == null) 
            return false;

        List<string> palavrasEscolhidas = new List<string>();

        foreach (Transform filho in gerador.containerMontagem)
        {
            BlocoItem bloco = filho.GetComponent<BlocoItem>();
            if (bloco != null)
            {
                palavrasEscolhidas.Add(bloco.ObterPalavra());
            }
        }

        if (palavrasEscolhidas.Count == 0)
        {
            feedbackAtualString = "Choose at least one word!";
            if (textoFeedback != null)
            {
                textoFeedback.color = Color.yellow;
                textoFeedback.text = feedbackAtualString;
            }
            return false;
        }

        string fraseMontadaPeloJogador = string.Join(" ", palavrasEscolhidas);
        string respostaFinalJogador = fraseMontadaPeloJogador.Trim().ToLower().Replace(".", "").Replace("?", "").Replace("!", "");
        string respostaCorretaNPC = exAtual.respostaCorreta.Trim().ToLower().Replace(".", "").Replace("?", "").Replace("!", "");

        if (respostaFinalJogador == respostaCorretaNPC)
        {
            feedbackAtualString = "Correct sentence! Great job!";
            if (textoFeedback != null) 
            {
                textoFeedback.color = Color.green;
                textoFeedback.text = feedbackAtualString;
            }
            return true;
        }
        else
        {
            tentativasRestantes--;

            if (tentativasRestantes > 0)
            {
                if(tentativasRestantes == 2)
                    feedbackAtualString = $"Wrong answer. You have {tentativasRestantes} more chance.";
                else
                    feedbackAtualString = $"Wrong answer. Don't worry, you can try again.";
            }
            else
            {
                feedbackAtualString = "Incorrect! Don't worry. You can try again later.";
            }

            if (textoFeedback != null)
            {
                textoFeedback.color = Color.red;
                textoFeedback.text = feedbackAtualString;
            }
            return false;
        }
    }
}