
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverTextoCenario : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TMP_Text texto;
    public Color corNormal = Color.white;
    public Color corHover = Color.yellow;

    private void Awake()
    {
        if (texto != null)
            texto.color = corNormal;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (texto != null)
            texto.color = corHover;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (texto != null)
            texto.color = corNormal;
    }
}
