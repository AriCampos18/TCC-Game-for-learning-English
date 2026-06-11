using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BlocosUI : MonoBehaviour
{
    public GeradorDeBlocos gerador;
    public TextMeshProUGUI textoEnunciado;
    public TextMeshProUGUI textoFeedback;

    private ExercicioBlocos exAtual;
    private int tentativasRestantes = 3;
    private string feedbackAtualString = "";

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

        List<string> palavrasEscolhidas = ObterPalavrasEscolhidas();

        if (palavrasEscolhidas.Count == 0)
        {
            feedbackAtualString = "Choose at least one word!";
            AtualizarFeedback(feedbackAtualString, Color.yellow);
            return false;
        }

        List<string> respostaCorreta = SepararPalavras(exAtual.respostaCorreta);

        float porcentagem = CalcularPorcentagemAcerto(palavrasEscolhidas, respostaCorreta);
        bool acertou = porcentagem >= 100f;

        if (acertou)
        {
            feedbackAtualString =
                $"Great job. You are right!\n";

            AtualizarFeedback(feedbackAtualString, Color.green);
            return true;
        }

        tentativasRestantes--;

        string detalhes = MontarDetalhesBlocos(palavrasEscolhidas, respostaCorreta);
        bool ultimaTentativa = tentativasRestantes <= 0;

        if (porcentagem <= 70f)
        {
            feedbackAtualString = ultimaTentativa
                ? $"Incorrect answer.{detalhes}"
                : $"Incorrect answer, try again.{detalhes}";
        }
        else
        {
            feedbackAtualString = ultimaTentativa
                ? $"Almost there.{detalhes}"
                : $"Almost there. Try again.{detalhes}";
        }

        AtualizarFeedback(feedbackAtualString, Color.red);
        return false;
    }

    private List<string> ObterPalavrasEscolhidas()
    {
        List<string> palavras = new List<string>();

        foreach (Transform filho in gerador.containerMontagem)
        {
            BlocoItem bloco = filho.GetComponent<BlocoItem>();

            if (bloco != null)
            {
                palavras.Add(bloco.ObterPalavra());
            }
        }

        return palavras;
    }

    private List<string> SepararPalavras(string frase)
    {
        string fraseLimpa = NormalizarFrase(frase);
        string[] partes = fraseLimpa.Split(' ');

        List<string> palavras = new List<string>();

        foreach (string p in partes)
        {
            if (!string.IsNullOrWhiteSpace(p))
                palavras.Add(p.Trim());
        }

        return palavras;
    }

    private string NormalizarFrase(string frase)
    {
        return frase
            .Trim()
            .ToLower()
            .Replace(".", "")
            .Replace("?", "")
            .Replace("!", "")
            .Replace(",", "")
            .Replace("\n", "")
            .Replace("\r", "");
    }

    private float CalcularPorcentagemAcerto(List<string> jogador, List<string> correta)
    {
        if (correta.Count == 0) return 0f;

        int acertos = 0;
        int total = correta.Count;

        for (int i = 0; i < total; i++)
        {
            if (i < jogador.Count)
            {
                string palavraJogador = NormalizarFrase(jogador[i]);
                string palavraCorreta = NormalizarFrase(correta[i]);

                if (palavraJogador == palavraCorreta)
                {
                    acertos++;
                }
            }
        }

        return (acertos / (float)total) * 100f;
    }

    private string MontarDetalhesBlocos(List<string> jogador, List<string> correta)
    {
        List<string> certos = new List<string>();
        List<string> faltando = new List<string>();

        List<string> jogadorNormalizado = new List<string>();

        foreach (string palavra in jogador)
        {
            jogadorNormalizado.Add(NormalizarFrase(palavra));
        }

        foreach (string palavraCorreta in correta)
        {
            string normalCorreta = NormalizarFrase(palavraCorreta);

            if (jogadorNormalizado.Contains(normalCorreta))
            {
                certos.Add(palavraCorreta);
                jogadorNormalizado.Remove(normalCorreta);
            }
            else
            {
                faltando.Add(palavraCorreta);
            }
        }

        string texto = "";

        if (certos.Count > 0)
            texto += "\nCorrect blocks: " + string.Join(", ", certos);

        if (faltando.Count > 0)
            texto += "\nMissing blocks: " + string.Join(", ", faltando);

        return texto;
    }

    private void AtualizarFeedback(string texto, Color cor)
    {
        if (textoFeedback != null)
        {
            textoFeedback.color = cor;
            textoFeedback.text = texto;
        }
    }
}