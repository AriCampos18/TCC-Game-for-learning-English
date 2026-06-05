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
    public void Show(string message)
    {
        if (panelImage != null) panelImage.color = corPadrao;
        if (text != null) text.color = corTextoPadrao; // Texto volta à cor original
        ExibirPainel(message);
    }

    // Sobrecarga 2: Missões (Aplica fundo branco e texto preto)
    public void Show(string message, Color corFundo, Color corTexto)
    {
        if (panelImage != null) panelImage.color = corFundo;   // Aplica Branco
        if (text != null) text.color = corTexto;               // Aplica Preto
        ExibirPainel(message);
    }

    private void ExibirPainel(string message)
    {
        // Adiciona a tag <b> antes e </b> depois da mensagem para forçar o negrito
        text.text = "<b>" + message + "</b>";

        if (panelRect != null && !panelRect.gameObject.activeSelf)
            panelRect.gameObject.SetActive(true);
    }

    public void Hide()
    {
        if (panelRect != null && panelRect.gameObject.activeSelf)
            panelRect.gameObject.SetActive(false);
    }
}