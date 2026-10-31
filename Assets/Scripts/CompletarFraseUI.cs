using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompletarFraseUI : MonoBehaviour
{
    public TextMeshProUGUI textoAntes;
    public TextMeshProUGUI textoDepois;
    public TextMeshProUGUI textoFeedback;
    public Transform containerOpcoes;
    public GameObject blocoPrefab;
    public DropZoneCompletarFrase dropZone;

    private readonly List<PalavraArrastavel> blocos = new List<PalavraArrastavel>();
    private ExercicioCompletarFrase exercicioAtual;
    private PalavraArrastavel respostaSelecionada;
    private int tentativasRestantes;
    private bool configuracaoValida;

    public bool HouveTentativaIncorreta { get; private set; }

    public void InicializarExercicio(ExercicioCompletarFrase exercicio)
    {
        LimparOpcoes();
        exercicioAtual = exercicio;
        respostaSelecionada = null;
        tentativasRestantes = 3;
        configuracaoValida = false;
        HouveTentativaIncorreta = false;

        MostrarFeedback(string.Empty, Color.white);

        if (exercicio == null || exercicio.opcoes == null || exercicio.opcoes.Count == 0 ||
            string.IsNullOrWhiteSpace(exercicio.respostaCorreta))
        {
            Debug.LogError("O exercício de completar frase precisa de uma resposta correta e opções.");
            return;
        }

        if (textoAntes == null || textoDepois == null || containerOpcoes == null ||
            blocoPrefab == null || dropZone == null)
        {
            Debug.LogError("Configure as referências visuais do CompletarFraseUI no Inspector.");
            return;
        }

        Graphic graficoDropZone = dropZone.GetComponent<Graphic>();
        if (graficoDropZone == null)
        {
            Debug.LogError("A DropZone precisa ter um componente Graphic, como Image, para receber o raycast.");
            return;
        }

        graficoDropZone.raycastTarget = true;

        bool respostaDisponivel = false;
        foreach (string opcao in exercicio.opcoes)
        {
            if (string.IsNullOrWhiteSpace(opcao))
            {
                Debug.LogError("As opções do exercício de completar frase não podem estar vazias.");
                return;
            }

            if (string.Equals(opcao.Trim(), exercicio.respostaCorreta.Trim(), StringComparison.OrdinalIgnoreCase))
                respostaDisponivel = true;
        }

        if (!respostaDisponivel)
        {
            Debug.LogError("A resposta correta precisa estar entre as opções do exercício de completar frase.");
            return;
        }

        textoAntes.text = exercicio.fraseAntes ?? string.Empty;
        textoDepois.text = exercicio.fraseDepois ?? string.Empty;
        dropZone.Configurar(this);
        configuracaoValida = true;

        foreach (string opcao in exercicio.opcoes)
        {
            GameObject novoBloco = Instantiate(blocoPrefab, containerOpcoes);
            TextMeshProUGUI texto = novoBloco.GetComponentInChildren<TextMeshProUGUI>();
            Graphic grafico = novoBloco.GetComponentInChildren<Graphic>();

            if (texto == null || grafico == null)
            {
                Debug.LogError("O prefab de bloco precisa conter um Graphic raycastável e um TextMeshProUGUI.");
                Destroy(novoBloco);
                LimparOpcoes();
                configuracaoValida = false;
                return;
            }

            texto.text = opcao;
            grafico.raycastTarget = true;
            CanvasGroup canvasGroup = novoBloco.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = novoBloco.AddComponent<CanvasGroup>();
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            PalavraArrastavel palavra = novoBloco.GetComponent<PalavraArrastavel>();
            if (palavra == null)
                palavra = novoBloco.AddComponent<PalavraArrastavel>();

            palavra.Configurar(dropZone, containerOpcoes, this);
            blocos.Add(palavra);
        }
    }

    public bool ProcessarSoltura(PalavraArrastavel palavra, DropZoneCompletarFrase zona)
    {
        if (!configuracaoValida || palavra == null || zona != dropZone ||
            tentativasRestantes <= 0 || respostaSelecionada != null)
            return false;

        string palavraSolta = palavra.ObterPalavra();
        if (string.Equals(palavraSolta.Trim(), exercicioAtual.respostaCorreta.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            respostaSelecionada = palavra;
            MostrarFeedback(string.Empty, Color.white);
            return true;
        }

        HouveTentativaIncorreta = true;
        tentativasRestantes--;
        if (tentativasRestantes > 0)
        {
            MostrarFeedback("Incorrect answer. Try again.", Color.red);
        }
        else
        {
            MostrarFeedback(
                "You have used all your chances. You can try again later in the revision section.",
                Color.red);
            BloquearOpcoes();
        }

        return false;
    }

    public bool Respondeu()
    {
        return configuracaoValida && respostaSelecionada != null;
    }

    public bool EstaCorreto()
    {
        return Respondeu() &&
            string.Equals(
                respostaSelecionada.ObterPalavra().Trim(),
                exercicioAtual.respostaCorreta.Trim(),
                StringComparison.OrdinalIgnoreCase);
    }

    public int ObterTentativasRestantes()
    {
        return tentativasRestantes;
    }

    public void BloquearOpcoes()
    {
        foreach (PalavraArrastavel bloco in blocos)
        {
            if (bloco != null)
                bloco.Bloquear();
        }
    }

    public void MostrarFeedback(string mensagem, Color cor)
    {
        if (textoFeedback == null)
            return;

        textoFeedback.text = mensagem;
        textoFeedback.color = cor;
    }

    private void LimparOpcoes()
    {
        foreach (PalavraArrastavel bloco in blocos)
        {
            if (bloco != null)
                Destroy(bloco.gameObject);
        }

        blocos.Clear();
    }
}
