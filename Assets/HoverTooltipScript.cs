using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class HoverTooltipScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string translation;
    public Color corDoFundo = new Color(0f, 0f, 0f, 0.85f);
    public Color corDoTexto = Color.white;

    private string ultimoTextoOriginal = "";
    private TextMeshProUGUI textoLocal;
    private BackendManager backend;
    private bool requisitando = false;
    private bool mouseEmCima = false;

    void Awake()
    {
        textoLocal = GetComponent<TextMeshProUGUI>();
        backend = new BackendManager();
    }

    public async void OnPointerEnter(PointerEventData eventData)
    {
        mouseEmCima = true;

        if (TooltipScript.instance == null) return;

        // CASO 1: já existe tradução preenchida manualmente
        if (!string.IsNullOrEmpty(translation))
        {
            TooltipScript.instance.Show(translation, corDoFundo, corDoTexto);
            return;
        }

        // CASO 2: tooltip baseado no texto do TMP
        if (textoLocal == null || string.IsNullOrWhiteSpace(textoLocal.text))
            return;

        if (requisitando) return;

        string textoOriginal = textoLocal.text;

        // usa cache local
        if (!string.IsNullOrEmpty(ultimoTextoOriginal) &&
            ultimoTextoOriginal == textoOriginal &&
            !string.IsNullOrEmpty(translation))
        {
            TooltipScript.instance.Show(translation, corDoFundo, corDoTexto);
            return;
        }

        requisitando = true;

        string resultadoTraducao = await backend.TraduzirTextoDeepL(textoOriginal);

        requisitando = false;

        // evita tooltip fantasma
        if (!mouseEmCima) return;
        if (!gameObject.activeInHierarchy) return;
        if (textoLocal == null || textoLocal.text != textoOriginal) return;

        if (!string.IsNullOrEmpty(resultadoTraducao))
        {
            ultimoTextoOriginal = textoOriginal;
            translation = resultadoTraducao;

            TooltipScript.instance.Show(translation, corDoFundo, corDoTexto);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseEmCima = false;

        if (TooltipScript.instance != null)
            TooltipScript.instance.Hide();
    }

    private void OnDisable()
    {
        mouseEmCima = false;

        if (TooltipScript.instance != null)
            TooltipScript.instance.Hide();
    }
}