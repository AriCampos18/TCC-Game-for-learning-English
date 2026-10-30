using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AssociacaoUI : MonoBehaviour
{
    [SerializeField] private Button prefabBotaoPalavra;
    [SerializeField] private Transform containerPortugues;
    [SerializeField] private Transform containerIngles;
    public TextMeshProUGUI textoFeedback;
    public TMP_Dropdown[] seletores;
    public TextMeshProUGUI[] rotulosPortugues;

    private readonly List<Button> botoesPortugues = new List<Button>();
    private readonly List<Button> botoesIngles = new List<Button>();
    private readonly List<Color> coresPortugues = new List<Color>();
    private readonly List<Color> coresIngles = new List<Color>();
    private bool[] portuguesConcluido;
    private bool[] inglesConcluido;
    private ExercicioAssociacao exercicio;
    private int indicePortuguesSelecionado = -1;
    private int indiceInglesSelecionado = -1;
    private int tentativasRestantes;
    private bool processandoPar;

    public event Action EstadoFinalizado;

    public bool TodasAssociacoesConcluidas { get; private set; }
    public bool TentativasEsgotadas { get; private set; }
    public bool PodeContinuar { get; private set; }

    public bool InicializarExercicio(ExercicioAssociacao novoExercicio)
    {
        LimparBotoes();
        exercicio = novoExercicio;
        TodasAssociacoesConcluidas = false;
        TentativasEsgotadas = false;
        PodeContinuar = false;
        tentativasRestantes = 3;
        indicePortuguesSelecionado = -1;
        indiceInglesSelecionado = -1;
        processandoPar = false;
        OcultarControlesAntigos();

        if (!ValidarExercicio(novoExercicio))
        {
            ExibirFeedback("Este exercício de associação está configurado incorretamente.");
            return false;
        }

        portuguesConcluido = new bool[novoExercicio.opcoesPort.Count];
        inglesConcluido = new bool[novoExercicio.opcoesIngles.Count];
        CriarContainersSeNecessario();

        for (int i = 0; i < novoExercicio.opcoesPort.Count; i++)
        {
            int indice = i;
            Button botao = CriarBotao(novoExercicio.opcoesPort[i], containerPortugues);
            botoesPortugues.Add(botao);
            coresPortugues.Add(botao.image.color);
            botao.onClick.AddListener(() => SelecionarPortugues(indice));
        }

        for (int i = 0; i < novoExercicio.opcoesIngles.Count; i++)
        {
            int indice = i;
            Button botao = CriarBotao(novoExercicio.opcoesIngles[i], containerIngles);
            botoesIngles.Add(botao);
            coresIngles.Add(botao.image.color);
            botao.onClick.AddListener(() => SelecionarIngles(indice));
        }

        ExibirFeedback("Selecione uma palavra em cada coluna para formar um par.");
        return true;
    }

    private bool ValidarExercicio(ExercicioAssociacao exercicioAssociacao)
    {
        if (exercicioAssociacao == null
            || exercicioAssociacao.opcoesPort == null
            || exercicioAssociacao.opcoesIngles == null
            || exercicioAssociacao.associacaoCorreta == null
            || exercicioAssociacao.opcoesPort.Count == 0
            || exercicioAssociacao.opcoesIngles.Count == 0
            || exercicioAssociacao.opcoesPort.Count != exercicioAssociacao.associacaoCorreta.Count)
        {
            Debug.LogError("Exercício de associação com listas vazias ou quantidades incompatíveis.");
            return false;
        }

        HashSet<string> portugues = new HashSet<string>(exercicioAssociacao.opcoesPort);
        HashSet<string> ingles = new HashSet<string>(exercicioAssociacao.opcoesIngles);
        if (portugues.Count != exercicioAssociacao.opcoesPort.Count
            || ingles.Count != exercicioAssociacao.opcoesIngles.Count)
        {
            Debug.LogError("As palavras de cada coluna de associação devem ser únicas.");
            return false;
        }

        HashSet<string> portuguesAssociado = new HashSet<string>();
        HashSet<string> inglesAssociado = new HashSet<string>();
        foreach (ParAssociacao par in exercicioAssociacao.associacaoCorreta)
        {
            if (par == null
                || !portugues.Contains(par.palavraPortugues)
                || !ingles.Contains(par.palavraIngles)
                || !portuguesAssociado.Add(par.palavraPortugues)
                || !inglesAssociado.Add(par.palavraIngles))
            {
                Debug.LogError("A chave de associação contém palavras ausentes ou repetidas.");
                return false;
            }
        }

        return portuguesAssociado.Count == portugues.Count
            && inglesAssociado.Count == exercicioAssociacao.associacaoCorreta.Count;
    }

    private void CriarContainersSeNecessario()
    {
        if (containerPortugues != null && containerIngles != null)
        {
            GarantirTextoFeedback();
            return;
        }

        GameObject colunas = new GameObject(
            "ColunasAssociacao",
            typeof(RectTransform),
            typeof(HorizontalLayoutGroup));
        colunas.transform.SetParent(transform, false);

        RectTransform rectColunas = colunas.GetComponent<RectTransform>();
        rectColunas.anchorMin = Vector2.zero;
        rectColunas.anchorMax = Vector2.one;
        rectColunas.offsetMin = new Vector2(20f, 72f);
        rectColunas.offsetMax = new Vector2(-20f, -62f);

        HorizontalLayoutGroup layoutColunas = colunas.GetComponent<HorizontalLayoutGroup>();
        layoutColunas.spacing = 20f;
        layoutColunas.childControlWidth = true;
        layoutColunas.childControlHeight = true;
        layoutColunas.childForceExpandWidth = true;
        layoutColunas.childForceExpandHeight = false;

        containerPortugues = CriarColuna("ColunaPortugues", colunas.transform);
        containerIngles = CriarColuna("ColunaIngles", colunas.transform);
        GarantirTextoFeedback();
    }

    private Transform CriarColuna(string nome, Transform pai)
    {
        GameObject coluna = new GameObject(
            nome,
            typeof(RectTransform),
            typeof(VerticalLayoutGroup),
            typeof(LayoutElement));
        coluna.transform.SetParent(pai, false);

        VerticalLayoutGroup layout = coluna.GetComponent<VerticalLayoutGroup>();
        layout.spacing = 8f;
        layout.childControlWidth = true;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        coluna.GetComponent<LayoutElement>().flexibleWidth = 1f;
        CriarCabecalhoColuna(
            nome == "ColunaPortugues" ? "Português" : "English",
            coluna.transform);
        return coluna.transform;
    }

    private void CriarCabecalhoColuna(string rotulo, Transform container)
    {
        GameObject objetoTexto = new GameObject(
            "RotuloColuna",
            typeof(RectTransform),
            typeof(TextMeshProUGUI),
            typeof(LayoutElement));
        objetoTexto.transform.SetParent(container, false);

        TextMeshProUGUI texto = objetoTexto.GetComponent<TextMeshProUGUI>();
        texto.font = TMP_Settings.defaultFontAsset;
        texto.text = rotulo;
        texto.alignment = TextAlignmentOptions.Center;
        texto.fontSize = 22f;
        texto.color = Color.white;

        objetoTexto.GetComponent<LayoutElement>().preferredHeight = 32f;
    }

    private Button CriarBotao(string palavra, Transform container)
    {
        Button botao;
        if (prefabBotaoPalavra != null)
        {
            botao = Instantiate(prefabBotaoPalavra, container);
        }
        else
        {
            GameObject objetoBotao = new GameObject(
                "BotaoPalavra",
                typeof(RectTransform),
                typeof(Image),
                typeof(Button),
                typeof(LayoutElement));
            objetoBotao.transform.SetParent(container, false);
            botao = objetoBotao.GetComponent<Button>();

            GameObject objetoTexto = new GameObject(
                "Texto",
                typeof(RectTransform),
                typeof(TextMeshProUGUI));
            objetoTexto.transform.SetParent(objetoBotao.transform, false);

            RectTransform rectTexto = objetoTexto.GetComponent<RectTransform>();
            rectTexto.anchorMin = Vector2.zero;
            rectTexto.anchorMax = Vector2.one;
            rectTexto.offsetMin = new Vector2(8f, 4f);
            rectTexto.offsetMax = new Vector2(-8f, -4f);

            TextMeshProUGUI texto = objetoTexto.GetComponent<TextMeshProUGUI>();
            texto.font = TMP_Settings.defaultFontAsset;
            texto.alignment = TextAlignmentOptions.Center;
            texto.fontSize = 24f;
            texto.color = Color.black;

            LayoutElement layout = objetoBotao.GetComponent<LayoutElement>();
            layout.minHeight = 48f;
            layout.flexibleWidth = 1f;
        }

        if (botao.image == null)
        {
            Image imagem = botao.GetComponent<Image>();
            if (imagem == null)
                imagem = botao.gameObject.AddComponent<Image>();
            botao.targetGraphic = imagem;
        }

        LayoutElement elementoLayout = botao.GetComponent<LayoutElement>();
        if (elementoLayout == null)
            elementoLayout = botao.gameObject.AddComponent<LayoutElement>();
        elementoLayout.minHeight = 48f;
        elementoLayout.flexibleWidth = 1f;
        botao.gameObject.SetActive(true);
        botao.interactable = true;

        TMP_Text rotulo = botao.GetComponentInChildren<TMP_Text>();
        if (rotulo == null)
        {
            Debug.LogError("O prefab do botão de associação precisa conter um componente TMP_Text.");
            botao.interactable = false;
            return botao;
        }

        rotulo.text = palavra;
        return botao;
    }

    private void SelecionarPortugues(int indice)
    {
        if (!PodeSelecionar() || portuguesConcluido[indice])
            return;

        if (indicePortuguesSelecionado >= 0)
            RestaurarBotao(botoesPortugues[indicePortuguesSelecionado], coresPortugues[indicePortuguesSelecionado]);

        indicePortuguesSelecionado = indice;
        botoesPortugues[indice].image.color = new Color(0.72f, 0.86f, 1f);
        ValidarParSelecionado();
    }

    private void SelecionarIngles(int indice)
    {
        if (!PodeSelecionar() || inglesConcluido[indice])
            return;

        if (indiceInglesSelecionado >= 0)
            RestaurarBotao(botoesIngles[indiceInglesSelecionado], coresIngles[indiceInglesSelecionado]);

        indiceInglesSelecionado = indice;
        botoesIngles[indice].image.color = new Color(0.72f, 0.86f, 1f);
        ValidarParSelecionado();
    }

    private bool PodeSelecionar()
    {
        return exercicio != null && !processandoPar && !PodeContinuar;
    }

    private void ValidarParSelecionado()
    {
        if (indicePortuguesSelecionado < 0 || indiceInglesSelecionado < 0)
            return;

        string palavraPortugues = exercicio.opcoesPort[indicePortuguesSelecionado];
        string palavraIngles = exercicio.opcoesIngles[indiceInglesSelecionado];
        bool correto = false;
        foreach (ParAssociacao par in exercicio.associacaoCorreta)
        {
            if (par.palavraPortugues == palavraPortugues && par.palavraIngles == palavraIngles)
            {
                correto = true;
                break;
            }
        }

        if (correto)
        {
            portuguesConcluido[indicePortuguesSelecionado] = true;
            inglesConcluido[indiceInglesSelecionado] = true;
            MarcarParConcluido(botoesPortugues[indicePortuguesSelecionado]);
            MarcarParConcluido(botoesIngles[indiceInglesSelecionado]);
            indicePortuguesSelecionado = -1;
            indiceInglesSelecionado = -1;

            int paresConcluidos = 0;
            foreach (bool concluido in portuguesConcluido)
            {
                if (concluido)
                    paresConcluidos++;
            }

            TodasAssociacoesConcluidas = paresConcluidos == portuguesConcluido.Length;
            if (TodasAssociacoesConcluidas)
            {
                DesabilitarPalavrasRestantes();
                ExibirFeedback("Todos os pares foram associados corretamente.");
                FinalizarExercicio();
            }
            else
            {
                ExibirFeedback("Par correto. Continue associando as palavras.");
            }

            return;
        }

        processandoPar = true;
        tentativasRestantes--;
        ExibirFeedback("Par incorreto. Tentativas restantes: " + tentativasRestantes + ".");
        StartCoroutine(PiscarParIncorreto(
            botoesPortugues[indicePortuguesSelecionado],
            botoesIngles[indiceInglesSelecionado]));
    }

    private IEnumerator PiscarParIncorreto(Button botaoPortugues, Button botaoIngles)
    {
        Color vermelhoClaro = new Color(1f, 0.65f, 0.65f);
        for (int i = 0; i < 8; i++)
        {
            botaoPortugues.image.color = i % 2 == 0
                ? vermelhoClaro
                : coresPortugues[indicePortuguesSelecionado];
            botaoIngles.image.color = i % 2 == 0
                ? vermelhoClaro
                : coresIngles[indiceInglesSelecionado];
            yield return new WaitForSeconds(0.25f);
        }

        indicePortuguesSelecionado = -1;
        indiceInglesSelecionado = -1;
        processandoPar = false;

        if (tentativasRestantes == 0)
        {
            TentativasEsgotadas = true;
            DesabilitarPalavrasRestantes();
            ExibirFeedback("As tentativas acabaram. Use Continue para prosseguir.");
            FinalizarExercicio();
        }
        else
        {
            RestaurarBotoesDisponiveis();
        }
    }

    private void RestaurarBotoesDisponiveis()
    {
        for (int i = 0; i < botoesPortugues.Count; i++)
        {
            if (!portuguesConcluido[i])
                RestaurarBotao(botoesPortugues[i], coresPortugues[i]);
        }

        for (int i = 0; i < botoesIngles.Count; i++)
        {
            if (!inglesConcluido[i])
                RestaurarBotao(botoesIngles[i], coresIngles[i]);
        }
    }

    private void DesabilitarPalavrasRestantes()
    {
        foreach (Button botao in botoesPortugues)
        {
            if (botao.interactable)
                botao.interactable = false;
        }

        foreach (Button botao in botoesIngles)
        {
            if (botao.interactable)
                botao.interactable = false;
        }
    }

    private void MarcarParConcluido(Button botao)
    {
        botao.image.color = new Color(0.72f, 0.9f, 0.74f);
        botao.interactable = false;
    }

    private void RestaurarBotao(Button botao, Color cor)
    {
        if (botao != null && botao.interactable)
            botao.image.color = cor;
    }

    private void FinalizarExercicio()
    {
        PodeContinuar = true;
        EstadoFinalizado?.Invoke();
    }

    private void ExibirFeedback(string mensagem)
    {
        if (textoFeedback != null)
            textoFeedback.text = mensagem;
    }

    private void GarantirTextoFeedback()
    {
        if (textoFeedback != null)
            return;

        GameObject objetoTexto = new GameObject(
            "FeedbackAssociacao",
            typeof(RectTransform),
            typeof(TextMeshProUGUI));
        objetoTexto.transform.SetParent(transform, false);

        RectTransform rect = objetoTexto.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0f, -12f);
        rect.sizeDelta = new Vector2(-40f, 42f);

        TextMeshProUGUI texto = objetoTexto.GetComponent<TextMeshProUGUI>();
        texto.font = TMP_Settings.defaultFontAsset;
        texto.alignment = TextAlignmentOptions.Center;
        texto.fontSize = 20f;
        texto.color = Color.white;
        texto.raycastTarget = false;
        textoFeedback = texto;
    }

    private void OcultarControlesAntigos()
    {
        if (seletores != null)
        {
            foreach (TMP_Dropdown seletor in seletores)
            {
                if (seletor != null)
                    seletor.gameObject.SetActive(false);
            }
        }

        if (rotulosPortugues != null)
        {
            foreach (TextMeshProUGUI rotulo in rotulosPortugues)
            {
                if (rotulo != null)
                    rotulo.gameObject.SetActive(false);
            }
        }
    }

    private void LimparBotoes()
    {
        foreach (Button botao in botoesPortugues)
        {
            if (botao != null)
                Destroy(botao.gameObject);
        }

        foreach (Button botao in botoesIngles)
        {
            if (botao != null)
                Destroy(botao.gameObject);
        }

        botoesPortugues.Clear();
        botoesIngles.Clear();
        coresPortugues.Clear();
        coresIngles.Clear();
    }
}
