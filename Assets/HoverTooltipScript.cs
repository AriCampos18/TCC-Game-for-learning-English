using UnityEngine;
using UnityEngine.EventSystems;
using System.Threading.Tasks;
using TMPro;

public class HoverTooltipScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string translation; // Preenchido dinamicamente por MissaoItemUI
    
    [Header("Configuração de Cor")]
    public bool usarCorCustomizada = false;
    public Color corDoTooltip = Color.white;
    public Color corDoFundo = Color.white;
    public Color corDoTexto = Color.black; 

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
        if (TooltipScript.instance == null) return;

        // 🛡️ TRAVA BLINDADA PARA MISSÕES: 
        // Se a variável 'translation' JÁ FOI PREENCHIDA pelo script MissaoItemUI,
        // nós NÃO ENCONTRAMOS no bloco do DeepL. Pulamos direto para exibir o Tooltip.
        if (string.IsNullOrEmpty(translation))
        {
            // Se o objeto não tiver texto próprio para ler, não faz nada
            if (textoLocal == null || string.IsNullOrEmpty(textoLocal.text)) return;
            if (requisitando) return; 

            requisitando = true;
            
            string textoOriginal = textoLocal.text;
            // Só chama o backend se for um texto genérico sem tradução prévia
            string resultadoTraducao = await backend.TraduzirTextoDeepL(textoOriginal);

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

        // 🎯 A sua lógica de renderização original (Suas missões rodam exatamente aqui!)
        if (usarCorCustomizada)
        {
            // Passa o texto da sua missão com fundo Branco e texto Preto definidos no MissaoItemUI
            TooltipScript.instance.Show(translation, corDoFundo, corDoTexto);
        }
        else
        {
            // Passa o Tooltip padrão para enunciados genéricos
            TooltipScript.instance.Show(translation);
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