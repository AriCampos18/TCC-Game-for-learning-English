using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompletarFraseUI : MonoBehaviour
{
    public TextMeshProUGUI textoFeedback;
    public Transform containerFrase;
    public TextMeshProUGUI prefabTrechoFrase;
    public GameObject prefabDropZone;
    public Transform containerOpcoes;
    public GameObject blocoPrefab;

    private readonly List<PalavraArrastavel> blocos = new List<PalavraArrastavel>();
    private readonly List<GameObject> elementosFrase = new List<GameObject>();
    private readonly List<string> respostasCorretas = new List<string>();
    private readonly List<PalavraArrastavel> respostasSelecionadas = new List<PalavraArrastavel>();
    private ExercicioCompletarFrase exercicioAtual;
    private int tentativasRestantes;
    private bool configuracaoValida;

    public void InicializarExercicio(ExercicioCompletarFrase exercicio)
    {
        LimparOpcoes();
        LimparFrase();
        exercicioAtual = exercicio;
        respostasCorretas.Clear();
        respostasSelecionadas.Clear();
        tentativasRestantes = 3;
        configuracaoValida = false;

        MostrarFeedback(string.Empty, Color.white);

        if (exercicio == null || exercicio.opcoes == null || exercicio.opcoes.Count == 0 ||
            string.IsNullOrWhiteSpace(exercicio.respostaCorreta))
        {
            Debug.LogError("O exercício de completar frase precisa de uma resposta correta e opções.");
            return;
        }

        if (containerFrase == null || prefabTrechoFrase == null || prefabDropZone == null ||
            containerOpcoes == null || blocoPrefab == null)
        {
            Debug.LogError("Configure as referências visuais do CompletarFraseUI no Inspector.");
            return;
        }

        List<string> respostas = SepararRespostas(exercicio.respostaCorreta);
        List<string> partes = ObterPartesFrase(exercicio, respostas.Count);
        if (respostas.Count == 0 || partes == null || partes.Count != respostas.Count + 1)
        {
            Debug.LogError("Informe uma parte de frase antes, entre e depois de cada resposta.");
            return;
        }

        if (prefabDropZone.GetComponent<Graphic>() == null ||
            prefabDropZone.GetComponent<DropZoneCompletarFrase>() == null)
        {
            Debug.LogError("O prefab da DropZone precisa ter um Graphic e DropZoneCompletarFrase no objeto raiz.");
            return;
        }

        Dictionary<string, int> opcoesDisponiveis =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (string opcao in exercicio.opcoes)
        {
            if (string.IsNullOrWhiteSpace(opcao))
            {
                Debug.LogError("As opções do exercício de completar frase não podem estar vazias.");
                return;
            }

            string opcaoLimpa = opcao.Trim();
            if (!opcoesDisponiveis.ContainsKey(opcaoLimpa))
                opcoesDisponiveis.Add(opcaoLimpa, 0);

            opcoesDisponiveis[opcaoLimpa]++;
        }

        foreach (string resposta in respostas)
        {
            int quantidadeDisponivel;
            if (!opcoesDisponiveis.TryGetValue(resposta, out quantidadeDisponivel) ||
                quantidadeDisponivel == 0)
            {
                Debug.LogError("As opções precisam incluir cada resposta correta, incluindo repetições: " + resposta);
                return;
            }

            opcoesDisponiveis[resposta] = quantidadeDisponivel - 1;
        }

        respostasCorretas.AddRange(respostas);
        for (int i = 0; i < respostas.Count; i++)
            respostasSelecionadas.Add(null);

        for (int i = 0; i < partes.Count; i++)
        {
            CriarTrechoFrase(partes[i]);
            if (i < respostas.Count)
                CriarDropZone(i);
        }

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

            palavra.Configurar(containerOpcoes, this);
            blocos.Add(palavra);
        }
    }

    public bool ProcessarSoltura(PalavraArrastavel palavra, DropZoneCompletarFrase zona)
    {
        int indice = zona != null ? zona.IndiceResposta : -1;
        if (!configuracaoValida || palavra == null || indice < 0 ||
            indice >= respostasCorretas.Count ||
            tentativasRestantes <= 0 || respostasSelecionadas[indice] != null)
            return false;

        string palavraSolta = palavra.ObterPalavra();
        if (string.Equals(palavraSolta.Trim(), respostasCorretas[indice], StringComparison.OrdinalIgnoreCase))
        {
            respostasSelecionadas[indice] = palavra;
            MostrarFeedback(string.Empty, Color.white);
            return true;
        }

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
        if (!configuracaoValida)
            return false;

        foreach (PalavraArrastavel resposta in respostasSelecionadas)
        {
            if (resposta == null)
                return false;
        }

        return true;
    }

    public bool EstaCorreto()
    {
        if (!Respondeu())
            return false;

        for (int i = 0; i < respostasSelecionadas.Count; i++)
        {
            if (!string.Equals(
                    respostasSelecionadas[i].ObterPalavra().Trim(),
                    respostasCorretas[i],
                    StringComparison.OrdinalIgnoreCase))
                return false;
        }

        return true;
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

    private static List<string> SepararRespostas(string respostaCorreta)
    {
        List<string> respostas = new List<string>();
        if (string.IsNullOrWhiteSpace(respostaCorreta))
            return respostas;

        string[] separadas = respostaCorreta.Split(',');
        foreach (string resposta in separadas)
        {
            string respostaLimpa = resposta.Trim();
            if (respostaLimpa.Length == 0)
                return new List<string>();

            respostas.Add(respostaLimpa);
        }

        return respostas;
    }

    private static List<string> ObterPartesFrase(ExercicioCompletarFrase exercicio, int quantidadeRespostas)
    {
        if (exercicio.partesFrase != null && exercicio.partesFrase.Count > 0)
            return exercicio.partesFrase;

        if (quantidadeRespostas == 1)
        {
            return new List<string>
            {
                exercicio.fraseAntes ?? string.Empty,
                exercicio.fraseDepois ?? string.Empty
            };
        }

        return null;
    }

    private void CriarTrechoFrase(string texto)
    {
        TextMeshProUGUI trecho = Instantiate(prefabTrechoFrase, containerFrase);
        trecho.gameObject.SetActive(true);
        trecho.text = texto;
        elementosFrase.Add(trecho.gameObject);
    }

    private void CriarDropZone(int indice)
    {
        GameObject objeto = Instantiate(prefabDropZone, containerFrase);
        objeto.SetActive(true);
        Graphic grafico = objeto.GetComponent<Graphic>();
        grafico.raycastTarget = true;

        DropZoneCompletarFrase zona = objeto.GetComponent<DropZoneCompletarFrase>();
        zona.Configurar(this, indice);
        elementosFrase.Add(objeto);
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

    private void LimparFrase()
    {
        foreach (GameObject elemento in elementosFrase)
        {
            if (elemento != null)
                Destroy(elemento);
        }

        elementosFrase.Clear();
    }
}
