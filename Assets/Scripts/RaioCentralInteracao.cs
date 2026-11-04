// using UnityEngine;

// public class RaioCentralInteracao : MonoBehaviour
// {
//     public float distance = 3f;
//     public LayerMask interactLayer;

//     public GameObject modalAvisoMouse;

//     private DestacarObjeto lastHighlighted;
//     private InteracaoNPC lastNPC;
//     private GameObject objetoAtual;

//     void Update()
// {
//     // ✨ NOVA TRAVA GLOBAL: Se já estiver em diálogo (Shelf andando, fazendo exercício, etc), 
//     // desativa os avisos visuais, limpa o último alvo e impede qualquer nova interação.
//     if (GameProgress.EstaEmDialogo)
//     {
//         if (modalAvisoMouse != null) modalAvisoMouse.SetActive(false);
//         ClearLast();
//         return; 
//     }

//     Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
//     RaycastHit hit;

//     bool hitSomething = Physics.Raycast(ray, out hit, distance, interactLayer);

//     DestacarObjeto h = null;
//     InteracaoNPC npc = null;
//     GameObject novoObjeto = null;

//     bool podeInteragirObjeto = false;

//     if (hitSomething)
//     {
//         GameObject obj = hit.collider.gameObject;

//         h = obj.GetComponentInParent<DestacarObjeto>();
//         npc = obj.GetComponentInParent<InteracaoNPC>();
//         if (npc != null)
//         {
//             bool podeInteragirNPC = npc.PodeInteragir();

//             if (!MissionManager.Instance.MissaoConcluida("pegar_cesta") || !podeInteragirNPC)
//             {
//                 h = null;
//                 npc = null;
//             }
//         }
//         else if (npc == null && h != null)
//         {
//             novoObjeto = obj;
//             podeInteragirObjeto = true;
//         }
//     }

//     // mudou alvo
//     if (h != lastHighlighted)
//     {
//         ClearLast();

//         if (h != null)
//         {
//             lastHighlighted = h;
//             lastNPC = npc;
//             objetoAtual = novoObjeto;

//             lastHighlighted.SetHighlight(true);

//             if (lastNPC != null)
//             {
//                 lastNPC.MostrarAviso(true);
//                 modalAvisoMouse.SetActive(false);
//             }
//             else
//             {
//                 modalAvisoMouse.SetActive(podeInteragirObjeto);
//             }
//         }
//     }

//     // INTERAÇÃO NPC
//     if (Input.GetKeyDown(KeyCode.F))
//     {
//         if (lastNPC != null)
//         {
//             _ = lastNPC.Interagir();
//         }
//     }

//     // PEGAR OBJETO
//     if (Input.GetMouseButtonDown(0))
//     {
//         if (objetoAtual != null && lastNPC == null)
//         {
//             SacolaInterativaScript sacola = objetoAtual.GetComponentInParent<SacolaInterativaScript>();

//             if (sacola != null)
//             {
//                 sacola.PegarSacola();

//                 if (modalAvisoMouse != null)
//                     modalAvisoMouse.SetActive(false);

//                 ClearLast();
//                 return;
//             }

//             ProdutoInterativo produto = objetoAtual.GetComponentInParent<ProdutoInterativo>();

//             if (produto != null)
//             {
//                 ProdutoManager.Instance.RegistrarProduto(produto);
//                 produto.gameObject.SetActive(false);
//             }
//             else
//             {
//                 Destroy(objetoAtual);
//             }

//             // missão 1 completa
//             if (!MissionManager.Instance.MissaoConcluida("pegar_cesta"))
//             {
//                 MissionManager.Instance.ConcluirMissao("pegar_cesta");

//                 // libera próximas missões
//                 MissionManager.Instance.AdicionarMissao(
//                     "falar_atendente",
//                     "Talk to the market attendant",
//                     "Fale com o atendente do mercado"
//                 );

//                 MissionManager.Instance.AdicionarMissao(
//                     "falar_padaria",
//                     "Talk to the bakery attendant",
//                     "Fale com o atendente da padaria"
//                 );
//             }

//             modalAvisoMouse.SetActive(false);
//             ClearLast();
//         }
//     }

//     if (!hitSomething)
//     {
//         modalAvisoMouse.SetActive(false);
//         ClearLast();
//     }
// }

//     void ClearLast()
//     {
//         if (lastHighlighted != null)
//             lastHighlighted.SetHighlight(false);

//         if (lastNPC != null)
//             lastNPC.MostrarAviso(false);

//         lastHighlighted = null;
//         lastNPC = null;
//         objetoAtual = null;
//     }
// }

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

        RaycastHit hit;
        bool hitSomething = Physics.Raycast(
            ray,
            out hit,
            distance,
            interactLayer
        );

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
                bool missaoCestaConcluida =
                    MissionManager.Instance != null &&
                    MissionManager.Instance.MissaoConcluida("pegar_cesta");

                if (!missaoCestaConcluida || !npc.PodeInteragir())
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
                    lastNPC.MostrarAviso(true);

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

        if ((!ModoVR && Input.GetKeyDown(KeyCode.F)) ||
            solicitouInteracaoVR)
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
