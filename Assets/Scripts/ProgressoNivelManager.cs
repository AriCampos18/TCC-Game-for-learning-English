using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressoNivelManager : MonoBehaviour
{
    public static ProgressoNivelManager Instance;
    public RectTransform fillBarra;
    public float larguraMaxima = 347.616f;
    public TextMeshProUGUI textoInicioNivel;
    public TextMeshProUGUI textoFimNivel;

    private string nivelAtual;
    private bool barraInicializada;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (!barraInicializada)
            InicializarBarra();
    }

    public void InicializarBarra()
    {
        barraInicializada = true;
        nivelAtual = DadosJogador.nivelUsuario;
        Debug.Log("Nível atual da barra: " + DadosJogador.nivelUsuario);

        var (inicio, fim) = nivelAtual switch
        {
            "A1" => ("A1", "A1-A2"),
            "A1-A2" => ("A1-A2", "A2"),
            "A2" => ("A2", "A2-B1"),
            "A2-B1" => ("A2-B1", "B1"),
            "B1" => ("B1", ""),
            _ => ("", "")
        };
        ExibirRotulo(textoInicioNivel, inicio);
        ExibirRotulo(textoFimNivel, fim);
        AtualizarVisual(GerenciadorProgressao.Instance?.Aproveitamento ?? 0f);
    }

    private static void ExibirRotulo(TextMeshProUGUI rotulo, string nivel)
    {
        if (rotulo == null)
            return;

        if (!rotulo.enableAutoSizing)
        {
            rotulo.fontSizeMax = rotulo.fontSize;
            rotulo.enableAutoSizing = true;
        }
        rotulo.textWrappingMode = TextWrappingModes.NoWrap;
        rotulo.text = nivel;
    }

    internal void AtualizarVisual(float porcentagem)
    {
        float progresso = porcentagem / 100f;

        if (fillBarra != null)
        {
            progresso = Mathf.Clamp01(progresso);

            fillBarra.anchorMin = new Vector2(0f, 0.5f);
            fillBarra.anchorMax = new Vector2(0f, 0.5f);
            fillBarra.pivot = new Vector2(0f, 0.5f);

            fillBarra.anchoredPosition = new Vector2(0f, fillBarra.anchoredPosition.y);

            fillBarra.sizeDelta = new Vector2(
                larguraMaxima * progresso,
                fillBarra.sizeDelta.y
            );
        }
    }
}
