using UnityEngine;
using UnityEngine.EventSystems;
using System.Threading.Tasks;
using TMPro;

public class HoverTooltipScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string translation; // Preenchido dinamicamente por MissaoItemUI
    
    [Header("Configuração de Cor")]
    public Color corDoFundo = new Color(0f, 0f, 0f, 0.85f);
    public Color corDoTexto = Color.white;

    private TextMeshProUGUI textoLocal;
    private BackendManager backend;
    private bool requisitando = false;

    void Awake()
    {
        // Pega o componente de texto do próprio objeto (se houver)
        textoLocal = GetComponent<TextMeshProUGUI>();
        backend = new BackendManager(); 
    }

    public async void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("ENTROU NO HOVER: " + gameObject.name);

        if (TooltipScript.instance == null)
        {
            Debug.LogError("TooltipScript.instance está null");
            return;
        }

        if (string.IsNullOrEmpty(translation))
        {
            if (textoLocal == null || string.IsNullOrEmpty(textoLocal.text))
            {
                Debug.LogWarning("Texto local vazio ou sem TextMeshProUGUI: " + gameObject.name);
                return;
            }

            if (requisitando) return; 

            requisitando = true;

            string textoOriginal = textoLocal.text;
            Debug.Log("Texto original para traduzir: " + textoOriginal);

            string resultadoTraducao = await backend.TraduzirTextoDeepL(textoOriginal);

            Debug.Log("Tradução recebida no Unity: " + resultadoTraducao);

            if (!string.IsNullOrEmpty(resultadoTraducao))
            {
                translation = resultadoTraducao; 
            }
            else
            {
                requisitando = false;
                return; 
            }

            requisitando = false;
        }

        Debug.Log("Chamando Tooltip Show com: " + translation);

        TooltipScript.instance.Show(translation, corDoFundo, corDoTexto);
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