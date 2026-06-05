using UnityEngine;
using UnityEngine.EventSystems;

public class HoverTooltipScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string translation;
    
    [Header("Configuração de Cor")]
    public bool usarCorCustomizada = false;
    public Color corDoTooltip = Color.white;
    public Color corDoFundo = Color.white;
    public Color corDoTexto = Color.black; // Nova variável para a cor do texto

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (TooltipScript.instance != null)
        {
            if (usarCorCustomizada)
            {
                // Passa a cor do fundo E a cor do texto
                TooltipScript.instance.Show(translation, corDoFundo, corDoTexto);
            }
            else
            {
                TooltipScript.instance.Show(translation);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (TooltipScript.instance != null)
        {
            TooltipScript.instance.Hide();
        }
    }

    private void OnDisable()
    {
        if (TooltipScript.instance != null)
        {
            TooltipScript.instance.Hide();
        }
    }
}