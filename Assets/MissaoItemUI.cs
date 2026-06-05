using TMPro;
using UnityEngine;

public class MissaoItemUI : MonoBehaviour
{
    public TextMeshProUGUI textoMissao;
    public HoverTooltipScript tooltip;

    private Missao missao;

    // Removeu o Awake daqui para evitar problemas com objeto desativado

    public void Configurar(Missao m)
    {
        missao = m;
        tooltip = GetComponentInChildren<HoverTooltipScript>();
        Debug.Log("Tooltip encontrado: " + (tooltip != null)); 

        AtualizarVisual();

        if (tooltip != null)
        {
            tooltip.translation = m.traducao;
            
            // Ativa a customização para as missões
            tooltip.usarCorCustomizada = true;
            
            // Define as cores exatamente como você pediu:
            tooltip.corDoFundo = Color.white; // Fundo Branco
            tooltip.corDoTexto = Color.black; // Texto Preto
        }
    }

    public void AtualizarVisual()
    {
        string status = missao.concluida ? "[X] " : "[ ] ";
        textoMissao.text = status + missao.texto;
    }
}