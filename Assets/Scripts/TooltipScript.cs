using UnityEngine;
using TMPro;
using UnityEngine.UI; // <-- ADICIONE ISSO para usar o componente Image

public class TooltipScript : MonoBehaviour
{
    public RectTransform panelRect; 
    public TextMeshProUGUI text;
    
    // Adicione esta variável para arrastar o componente de Imagem do seu painel
    public Image panelImage; 
    
    // Guarde a cor original/padrão do painel para resetar depois
    private Color corPadrao;

    public static TooltipScript instance;
    private Canvas canvas;
    private Color corTextoPadrao;

    void Awake()
    {
        instance = this;
        canvas = GetComponentInParent<Canvas>();

        if (panelRect != null)
        {
            panelRect.gameObject.SetActive(false);
            // Salva as cores originais (das instruções do jogo)
            if (panelImage != null) corPadrao = panelImage.color;
            if (text != null) corTextoPadrao = text.color;
        }
    }

    void Update()
    {
        if (panelRect == null || !panelRect.gameObject.activeSelf || canvas == null) return;

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out localPoint
        );

        panelRect.anchoredPosition = localPoint + new Vector2(20, -20);
    }

    // Sobrecarga 1: Função original (usa a cor padrão das instruções)

// Sobrecarga 1: Instruções normais do jogo (Volta para o padrão)

    public void Show(string message, Color corFundo, Color corTexto)
    {
        if (panelImage != null) panelImage.color = corFundo;
        if (text != null) text.color = corTexto;

        ExibirPainel(message, false);
    }

    public void ShowTooltipGrande(string message, Color corFundo, Color corTexto)
    {
        if (panelImage != null) panelImage.color = corFundo;
        if (text != null) text.color = corTexto;

        ExibirPainel(message, true);
    }

    private void ExibirPainel(string message, bool tooltipGrande)
    {
        text.text = message;
        text.ForceMeshUpdate();

        float larguraMaxima = tooltipGrande ? 700f : 350f;

        Vector2 tamanhoTexto = text.GetPreferredValues(
            text.text,
            larguraMaxima,
            Mathf.Infinity
        );

        panelRect.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal,
            Mathf.Min(tamanhoTexto.x + 30f, larguraMaxima + 30f)
        );

        panelRect.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Vertical,
            tamanhoTexto.y + 20f
        );

        if (!panelRect.gameObject.activeSelf)
            panelRect.gameObject.SetActive(true);
    }

    public void Hide()
    {
        if (panelRect != null && panelRect.gameObject.activeSelf)
            panelRect.gameObject.SetActive(false);
    }
}