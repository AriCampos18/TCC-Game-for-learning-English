using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompletarFraseUI : MonoBehaviour
{
    [Header("Interface")]
    [SerializeField] private TMP_Text textoEnunciado;
    [SerializeField] private Transform areaFrase;
    [SerializeField] private Transform containerOpcoes;
    [SerializeField] private Button prefabBotaoPalavra;
    [SerializeField] private TMP_Text textoFeedback;

    [Header("Configuração")]
    [SerializeField] private int tentativasIniciais = 3;
    [SerializeField] private string marcadorEspacoVazio = "________";
    [SerializeField] private Color corSelecionado = new Color(1f, 0.85f, 0.35f);
    [SerializeField] private Color corNormal = Color.white;

    private ExercicioCompletarFrase exercicioAtual;
    private readonly List<Button> botoesOpcoes = new List<Button>();
    private readonly List<Button> botoesLacunas = new List<Button>();
    private readonly List<string> respostasSelecionadas = new List<string>();
    private readonly List<string> respostasCorretas = new List<string>();

    private string palavraSelecionada;
    private int tentativasRestantes;
    private bool bloqueado;

    public void InicializarExercicio(ExercicioCompletarFrase exercicio)
    {
        if (exercicio == null)
        {
            Debug.LogError("CompletarFraseUI recebeu um exercício nulo.");
            return;
        }

        if (areaFrase == null || containerOpcoes == null || prefabBotaoPalavra == null)
        {
            Debug.LogError("Configure AreaFrase, ContainerOpcoes e PrefabBotaoPalavra no Inspector.");
            return;
        }

        exercicioAtual = exercicio;
        tentativasRestantes = Mathf.Max(1, tentativasIniciais);
        palavraSelecionada = null;
        bloqueado = false;

        LimparFilhos(areaFrase);
        LimparFilhos(containerOpcoes);
        botoesOpcoes.Clear();
        botoesLacunas.Clear();
        respostasSelecionadas.Clear();
        respostasCorretas.Clear();

        if (textoEnunciado != null)
            textoEnunciado.text = exercicio.enunciado;

        PrepararRespostasCorretas(exercicio);
        CriarFrase(exercicio);
        CriarOpcoes(exercicio);

        if (botoesLacunas.Count == 0)
            Debug.LogError("O exercício de Completar Frase precisa ter pelo menos uma lacuna.");

        if (botoesOpcoes.Count == 0)
            Debug.LogError("O exercício de Completar Frase precisa ter pelo menos uma opção.");

        MostrarFeedback(string.Empty, Color.white);
    }

    private void PrepararRespostasCorretas(ExercicioCompletarFrase exercicio)
    {
        // Para uma lacuna, use respostaCorreta normalmente.
        // Para várias lacunas, separe as respostas com | na mesma ordem das lacunas.
        if (exercicio.partesFrase != null && exercicio.partesFrase.Count > 1)
        {
            respostasCorretas.AddRange(
                (exercicio.respostaCorreta ?? string.Empty)
                .Split('|')
                .Select(r => r.Trim())
            );

            int quantidadeLacunas = exercicio.partesFrase.Count - 1;

            while (respostasCorretas.Count < quantidadeLacunas)
                respostasCorretas.Add(string.Empty);

            if (respostasCorretas.Count > quantidadeLacunas)
                respostasCorretas.RemoveRange(quantidadeLacunas, respostasCorretas.Count - quantidadeLacunas);
        }
        else
        {
            respostasCorretas.Add((exercicio.respostaCorreta ?? string.Empty).Trim());
        }

        for (int i = 0; i < respostasCorretas.Count; i++)
            respostasSelecionadas.Add(string.Empty);
    }

    private void CriarFrase(ExercicioCompletarFrase exercicio)
    {
        // Com partesFrase, cada item representa um trecho fixo.
        // Ex.: ["She ", " to work and ", " lunch."] cria duas lacunas.
        if (exercicio.partesFrase != null && exercicio.partesFrase.Count > 1)
        {
            for (int i = 0; i < exercicio.partesFrase.Count; i++)
            {
                CriarTrechoTexto(exercicio.partesFrase[i]);

                if (i < exercicio.partesFrase.Count - 1)
                    CriarBotaoLacuna(i);
            }
        }
        else
        {
            CriarTrechoTexto(exercicio.fraseAntes);
            CriarBotaoLacuna(0);
            CriarTrechoTexto(exercicio.fraseDepois);
        }
    }

    private void CriarTrechoTexto(string trecho)
    {
        GameObject objetoTexto = new GameObject("TrechoFrase", typeof(RectTransform), typeof(TextMeshProUGUI));
        objetoTexto.transform.SetParent(areaFrase, false);

        TextMeshProUGUI texto = objetoTexto.GetComponent<TextMeshProUGUI>();
        texto.text = trecho ?? string.Empty;
        texto.fontSize = 28;
        texto.color = Color.black;
        texto.alignment = TextAlignmentOptions.Midline;
        texto.textWrappingMode = TextWrappingModes.Normal;
        texto.raycastTarget = false;

        LayoutElement layout = objetoTexto.AddComponent<LayoutElement>();
        layout.minWidth = 10;
        layout.flexibleWidth = 0;
    }

    private void CriarBotaoLacuna(int indice)
    {
        Button botao = Instantiate(prefabBotaoPalavra, areaFrase);
        botao.name = $"CampoResposta_{indice + 1}";
        botao.gameObject.SetActive(true);

        TMP_Text texto = botao.GetComponentInChildren<TMP_Text>();
        if (texto != null)
            texto.text = marcadorEspacoVazio;

        int indiceCapturado = indice;
        botao.onClick.RemoveAllListeners();
        botao.onClick.AddListener(() => ClicarNaLacuna(indiceCapturado));

        botoesLacunas.Add(botao);
    }

    private void CriarOpcoes(ExercicioCompletarFrase exercicio)
    {
        if (exercicio.opcoes == null)
        {
            Debug.LogWarning("O exercício não possui opções cadastradas.");
            return;
        }

        foreach (string opcao in exercicio.opcoes)
        {
            Button botao = Instantiate(prefabBotaoPalavra, containerOpcoes);
            botao.gameObject.SetActive(true);
            botao.name = $"Opcao_{opcao}";

            TMP_Text texto = botao.GetComponentInChildren<TMP_Text>();
            if (texto != null)
                texto.text = opcao;

            string opcaoCapturada = opcao;
            botao.onClick.RemoveAllListeners();
            botao.onClick.AddListener(() => SelecionarPalavra(opcaoCapturada, botao));

            botoesOpcoes.Add(botao);
        }
    }

    private void SelecionarPalavra(string palavra, Button botao)
    {
        if (bloqueado)
            return;

        palavraSelecionada = palavra;

        foreach (Button opcao in botoesOpcoes)
        {
            Image imagem = opcao.GetComponent<Image>();
            if (imagem != null)
                imagem.color = opcao == botao ? corSelecionado : corNormal;
        }

        MostrarFeedback($"Selected: {palavra}. Now choose a blank.", Color.white);
    }

    private void ClicarNaLacuna(int indice)
    {
        if (bloqueado)
            return;

        // Clicar em uma lacuna preenchida sem palavra selecionada limpa aquela resposta.
        if (string.IsNullOrEmpty(palavraSelecionada))
        {
            if (!string.IsNullOrEmpty(respostasSelecionadas[indice]))
            {
                respostasSelecionadas[indice] = string.Empty;
                AtualizarVisualLacuna(indice);
            }
            return;
        }

        respostasSelecionadas[indice] = palavraSelecionada;
        AtualizarVisualLacuna(indice);
        palavraSelecionada = null;

        foreach (Button opcao in botoesOpcoes)
        {
            Image imagem = opcao.GetComponent<Image>();
            if (imagem != null)
                imagem.color = corNormal;
        }

        MostrarFeedback(string.Empty, Color.white);
    }

    private void AtualizarVisualLacuna(int indice)
    {
        TMP_Text texto = botoesLacunas[indice].GetComponentInChildren<TMP_Text>();
        if (texto != null)
            texto.text = string.IsNullOrEmpty(respostasSelecionadas[indice])
                ? marcadorEspacoVazio
                : respostasSelecionadas[indice];
    }

    public bool Respondeu()
    {
        return respostasSelecionadas.Count > 0 &&
               respostasSelecionadas.All(r => !string.IsNullOrWhiteSpace(r));
    }

    public bool EstaCorreto()
    {
        if (!Respondeu() || respostasCorretas.Count != respostasSelecionadas.Count)
            return false;

        for (int i = 0; i < respostasSelecionadas.Count; i++)
        {
            if (!string.Equals(
                respostasSelecionadas[i].Trim(),
                respostasCorretas[i].Trim(),
                System.StringComparison.OrdinalIgnoreCase))
                return false;
        }

        return true;
    }

    public void ReduzirTentativa()
    {
        if (!bloqueado && tentativasRestantes > 0)
            tentativasRestantes--;
    }

    public int ObterTentativasRestantes()
    {
        return tentativasRestantes;
    }

    public void MostrarFeedback(string mensagem, Color cor)
    {
        if (textoFeedback == null)
            return;

        textoFeedback.text = mensagem;
        textoFeedback.color = cor;
    }

    public void BloquearOpcoes()
    {
        bloqueado = true;

        foreach (Button botao in botoesOpcoes)
            botao.interactable = false;

        foreach (Button botao in botoesLacunas)
            botao.interactable = false;
    }

    private void LimparFilhos(Transform pai)
    {
        for (int i = pai.childCount - 1; i >= 0; i--)
            Destroy(pai.GetChild(i).gameObject);
    }
}
