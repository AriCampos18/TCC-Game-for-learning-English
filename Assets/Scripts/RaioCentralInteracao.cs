using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class RaioCentralInteracao : MonoBehaviour
{
    [Header("Interação desktop")]
    public float distance = 3f;
    public LayerMask interactLayer;
    public GameObject modalAvisoMouse;

    [Header("Interação VR")]
    [Tooltip("Transform do ponto de origem do raio do controle direito.")]
    public Transform vrRayOrigin;

    [Tooltip("Input Action vinculada ao botão A do controle direito.")]
    public InputActionReference vrInteractAction;

    private DestacarObjeto lastHighlighted;
    private InteracaoNPC lastNPC;
    private GameObject objetoAtual;

    private bool ModoVR => XRSettings.isDeviceActive;

    private void Update()
    {
        // Enquanto houver diálogo/exercício ativo, não permite novas interações.
        if (GameProgress.EstaEmDialogo)
        {
            if (modalAvisoMouse != null)
                modalAvisoMouse.SetActive(false);

            ClearLast();
            return;
        }

        Ray ray;

        if (ModoVR)
        {
            // No VR, o raio parte do controle direito.
            //Aqui, a interação é bloqueada caso o raio do controle direito não esteja definido, e assim, evita erros de referência nula.
            if (vrRayOrigin == null)
            {
                if (modalAvisoMouse != null)
                    modalAvisoMouse.SetActive(false);

                ClearLast();
                return;
            }

            ray = new Ray(vrRayOrigin.position, vrRayOrigin.forward);
        }
        else
        {
            // No desktop, mantém o raio no centro da câmera.
            if (Camera.main == null)
                return;

            ray = Camera.main.ViewportPointToRay(
                new Vector3(0.5f, 0.5f, 0f)
            );
        }

        RaycastHit hit; //armazena a colisão
        bool hitSomething = Physics.Raycast(
            ray,
            out hit,
            distance,
            interactLayer
        ); //guarda um bool que indica se houve colisão com algum objeto na layer de interação

        DestacarObjeto h = null;
        InteracaoNPC npc = null; //caso colida com um NPC
        GameObject novoObjeto = null; //caso colida com um objeto interativo
        bool podeInteragirObjeto = false;

        //se colidiu com algo
        if (hitSomething) 
        {
            GameObject obj = hit.collider.gameObject;

            h = obj.GetComponentInParent<DestacarObjeto>();
            npc = obj.GetComponentInParent<InteracaoNPC>();

            if (npc != null)
            {
                // A missão da cesta é uma exigência exclusiva do Market.
                bool npcDoMercado =
                    npc is ShelfNPC ||
                    npc is BakeryNPC ||
                    npc is CashierNPC;

                bool missaoCestaConcluida =
                    MissionManager.Instance != null &&
                    MissionManager.Instance.MissaoConcluida("pegar_cesta");

                if ((npcDoMercado && !missaoCestaConcluida) ||
                    !npc.PodeInteragir())
                {
                    h = null;
                    npc = null;
                }
            }
            else if (h != null)
            {
                novoObjeto = obj;
                podeInteragirObjeto = true;
            }
        }

        // Atualiza o destaque e os avisos quando o alvo muda.
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
                    lastNPC.MostrarAviso(true); //mostrar aviso é o 'Clicar em F'

                    if (modalAvisoMouse != null)
                        modalAvisoMouse.SetActive(false);
                }
                else if (modalAvisoMouse != null)
                {
                    // No VR, o aviso visual pode ser adaptado depois para
                    // mostrar o botão A, em vez do aviso de mouse.
                    modalAvisoMouse.SetActive(podeInteragirObjeto);
                }
            }
        }

        // No PC: F interage com NPC.
        // No VR: botão A interage com NPC ou pega objetos.
        bool solicitouInteracaoVR =
            ModoVR &&
            vrInteractAction != null &&
            vrInteractAction.action != null &&
            vrInteractAction.action.WasPressedThisFrame();

        //Esse if lida com a interação com avatar pelo desktop e com o interação com o avatar ou pegar objetos pelo VR.
        if ((!ModoVR && Input.GetKeyDown(KeyCode.F)) || solicitouInteracaoVR)
        {
            if (lastNPC != null)
            {
                _ = lastNPC.Interagir();
            }
            else if (ModoVR && objetoAtual != null)
            {
                PegarObjetoAtual();
            }
        }

        // No PC, mantém o clique esquerdo para pegar objetos.
        if (!ModoVR && Input.GetMouseButtonDown(0))
        {
            if (objetoAtual != null && lastNPC == null)
                PegarObjetoAtual();
        }

        if (!hitSomething)
        {
            if (modalAvisoMouse != null)
                modalAvisoMouse.SetActive(false);

            ClearLast();
        }
    }

    private void PegarObjetoAtual()
    {
        if (objetoAtual == null)
            return;

        SacolaInterativaScript sacola =
            objetoAtual.GetComponentInParent<SacolaInterativaScript>();

        if (sacola != null)
        {
            sacola.PegarSacola();

            if (modalAvisoMouse != null)
                modalAvisoMouse.SetActive(false);

            ClearLast();
            return;
        }

        ProdutoInterativo produto =
            objetoAtual.GetComponentInParent<ProdutoInterativo>();

        if (produto != null)
        {
            if (ProdutoManager.Instance != null)
                ProdutoManager.Instance.RegistrarProduto(produto);

            produto.gameObject.SetActive(false);
        }
        else
        {
            Destroy(objetoAtual);
        }

        // Mantém a lógica original de conclusão da primeira missão.
        if (MissionManager.Instance != null &&
            !MissionManager.Instance.MissaoConcluida("pegar_cesta"))
        {
            MissionManager.Instance.ConcluirMissao("pegar_cesta");

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

        if (modalAvisoMouse != null)
            modalAvisoMouse.SetActive(false);

        ClearLast();
    }

    private void ClearLast()
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