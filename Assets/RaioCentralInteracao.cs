using UnityEngine;

public class RaioCentralInteracao : MonoBehaviour
{
    public float distance = 3f;
    public LayerMask interactLayer;

    public GameObject modalAvisoMouse;

    private DestacarObjeto lastHighlighted;
    private InteracaoNPC lastNPC;
    private GameObject objetoAtual;

    void Update()
{
    // ✨ NOVA TRAVA GLOBAL: Se já estiver em diálogo (Shelf andando, fazendo exercício, etc), 
    // desativa os avisos visuais, limpa o último alvo e impede qualquer nova interação.
    if (GameProgress.EstaEmDialogo)
    {
        if (modalAvisoMouse != null) modalAvisoMouse.SetActive(false);
        ClearLast();
        return; 
    }

    Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
    RaycastHit hit;

    bool hitSomething = Physics.Raycast(ray, out hit, distance, interactLayer);

    DestacarObjeto h = null;
    InteracaoNPC npc = null;
    GameObject novoObjeto = null;

    bool podeInteragirObjeto = false;

    if (hitSomething)
    {
        GameObject obj = hit.collider.gameObject;

        h = obj.GetComponentInParent<DestacarObjeto>();
        npc = obj.GetComponentInParent<InteracaoNPC>();
        if (npc != null)
        {
            bool podeInteragirNPC = npc.PodeInteragir();

            if (!MissionManager.Instance.MissaoConcluida("pegar_cesta") || !podeInteragirNPC)
            {
                h = null;
                npc = null;
            }
        }
        else if (npc == null && h != null)
        {
            novoObjeto = obj;
            podeInteragirObjeto = true;
        }
    }

    // mudou alvo
    if (h != lastHighlighted)
    {
        ClearLast();

        if (h != null)
        {
            lastHighlighted = h;
            lastNPC = npc;
            objetoAtual = novoObjeto;

            lastHighlighted.SetHighlight(true);

            if (lastNPC != null)
            {
                lastNPC.MostrarAviso(true);
                modalAvisoMouse.SetActive(false);
            }
            else
            {
                modalAvisoMouse.SetActive(podeInteragirObjeto);
            }
        }
    }

    // INTERAÇÃO NPC
    if (Input.GetKeyDown(KeyCode.F))
    {
        if (lastNPC != null)
        {
            _ = lastNPC.Interagir();
        }
    }

    // PEGAR OBJETO
    if (Input.GetMouseButtonDown(0))
    {
        if (objetoAtual != null && lastNPC == null)
        {
            SacolaInterativaScript sacola = objetoAtual.GetComponentInParent<SacolaInterativaScript>();

            if (sacola != null)
            {
                sacola.PegarSacola();

                if (modalAvisoMouse != null)
                    modalAvisoMouse.SetActive(false);

                ClearLast();
                return;
            }

            ProdutoInterativo produto = objetoAtual.GetComponentInParent<ProdutoInterativo>();

            if (produto != null)
            {
                ProdutoManager.Instance.RegistrarProduto(produto);
                produto.gameObject.SetActive(false);
            }
            else
            {
                Destroy(objetoAtual);
            }

            // missão 1 completa
            if (!MissionManager.Instance.MissaoConcluida("pegar_cesta"))
            {
                MissionManager.Instance.ConcluirMissao("pegar_cesta");

                // libera próximas missões
                MissionManager.Instance.AdicionarMissao(
                    "falar_atendente",
                    "Talk to the market attendant",
                    "Fale com o atendente do mercado"
                );

                MissionManager.Instance.AdicionarMissao(
                    "falar_padaria",
                    "Talk to the bakery attendant",
                    "Fale com o atendente da padaria"
                );
            }

            modalAvisoMouse.SetActive(false);
            ClearLast();
        }
    }

    if (!hitSomething)
    {
        modalAvisoMouse.SetActive(false);
        ClearLast();
    }
}

    void ClearLast()
    {
        if (lastHighlighted != null)
            lastHighlighted.SetHighlight(false);

        if (lastNPC != null)
            lastNPC.MostrarAviso(false);

        lastHighlighted = null;
        lastNPC = null;
        objetoAtual = null;
    }
}