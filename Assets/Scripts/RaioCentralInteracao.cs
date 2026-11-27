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

    private readonly System.Collections.Generic.List<GameObject> cestasRecolhidas =
        new System.Collections.Generic.List<GameObject>();

    internal void RestaurarCestas()
    {
        foreach (var cesta in cestasRecolhidas)
            if (cesta != null)
                cesta.SetActive(true);
        cestasRecolhidas.Clear();
        ClearLast();
    }

    private bool ModoVR => XRSettings.isDeviceActive;

    private void Update()
    {
        var sessao = SessaoCenario.Atual;
        if (!sessao.EmJogo || !CheckpointJogo.PodeAvancar)
        {
            ClearLast();
            if (modalAvisoMouse != null)
                modalAvisoMouse.SetActive(false);
            return;
        }

        // Enquanto houver diálogo/exercício ativo, não permite novas interações.
        CartaoQuartoHotel cartaoHotel = sessao.Permite("Hotel") ? CartaoQuartoHotel.Disponivel : null;
        InteracaoHome mesaHome = sessao.Permite("Home") ? InteracaoHome.MesaDisponivel : null;
        InteracaoRestaurant etapaRestaurant = sessao.Permite("Restaurant") ? InteracaoRestaurant.Pendente : null;
        if (GameProgress.EstaEmDialogo && cartaoHotel == null && mesaHome == null && etapaRestaurant == null)
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

        // Alguns avatares mantêm o collider na camada Default do modelo importado.
        // Considere apenas o primeiro sólido fora da máscara, para não mirar um NPC através de uma parede.
        if (TentarAtingirNpcForaDaMascara(ray, hitSomething ? hit.distance : distance, out var hitNpc))
        {
            hit = hitNpc;
            hitSomething = true;
        }

        // No PC: F interage com NPC.
        // No VR: botão A interage com NPC ou pega objetos.
        bool solicitouInteracaoVR =
            ModoVR &&
            ModoJogoManager.Instance?.InterfaceConsomeInteracaoVR != true &&
            vrInteractAction != null &&
            vrInteractAction.action != null &&
            vrInteractAction.action.WasPressedThisFrame();

        if (cartaoHotel != null)
        {
            ClearLast();
            if (modalAvisoMouse != null)
                modalAvisoMouse.SetActive(false);

            bool solicitouColeta = ModoVR ? solicitouInteracaoVR : Input.GetMouseButtonDown(1);
            if (solicitouColeta && hitSomething && hit.collider.GetComponentInParent<CartaoQuartoHotel>() == cartaoHotel)
                cartaoHotel.TentarRecolher();

            return;
        }

        if (mesaHome != null)
        {
            ClearLast();
            if (modalAvisoMouse != null)
                modalAvisoMouse.SetActive(false);

            if (hitSomething && (ModoVR ? solicitouInteracaoVR : Input.GetMouseButtonDown(0)))
                mesaHome.TentarInteragirComMesa(hit.collider);

            return;
        }

        if (etapaRestaurant != null)
        {
            ClearLast();
            if (modalAvisoMouse != null)
                modalAvisoMouse.SetActive(false);

            if (hitSomething && (ModoVR ? solicitouInteracaoVR : Input.GetMouseButtonDown(0)))
                etapaRestaurant.TentarConfirmar(hit.collider);
            return;
        }

        if (!GameProgress.EstaEmDialogo && hitSomething && MissionManager.Instance?.UsaCatalogoRestaurant == true
            && ModoJogoManager.Instance?.uiMode != true)
        {
            InteracaoRestaurant mesaRestaurant = hit.collider.GetComponentInParent<InteracaoRestaurant>();
            if (mesaRestaurant != null && mesaRestaurant.PodeSentar(hit.collider))
            {
                ClearLast();
                if (modalAvisoMouse != null)
                    modalAvisoMouse.SetActive(false);

                if (ModoVR ? solicitouInteracaoVR : Input.GetMouseButtonDown(0))
                    mesaRestaurant.TentarSentar();
                return;
            }
        }

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
                if (!NpcDisponivel(npc))
                {
                    h = null;
                    npc = null;
                }
            }
            else if (h != null && sessao.Permite("Market") && AlvoMarket(obj))
            {
                novoObjeto = obj;
                podeInteragirObjeto = true;
            }
            else
            {
                h = null;
            }
        }

        // Atualiza o destaque e os avisos quando o alvo muda.
        if (h != lastHighlighted || npc != lastNPC || novoObjeto != objetoAtual)
        {
            ClearLast();

            lastNPC = npc;
            objetoAtual = novoObjeto;
            lastHighlighted = h;
            if (lastHighlighted != null)
                lastHighlighted.SetHighlight(true);

            if (lastNPC != null)
                lastNPC.MostrarAviso(true);

            if (modalAvisoMouse != null)
                modalAvisoMouse.SetActive(podeInteragirObjeto);
        }

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

    private bool TentarAtingirNpcForaDaMascara(Ray raio, float alcance, out RaycastHit hit)
    {
        int outrasCamadas = Physics.DefaultRaycastLayers & ~interactLayer.value;
        if (!Physics.Raycast(raio, out hit, alcance, outrasCamadas, QueryTriggerInteraction.Ignore))
            return false;

        var npc = hit.collider.GetComponentInParent<InteracaoNPC>();
        return NpcDisponivel(npc);
    }

    private static bool NpcDisponivel(InteracaoNPC npc)
    {
        if (npc == null || !npc.PertenceAoCenarioAtivo)
            return false;

        // A cesta faz parte da entrada no Market, independentemente da camada do NPC.
        if (npc.Cenario == "Market" && MissionManager.Instance?.MissaoConcluida("pegar_cesta") != true)
            return false;

        return npc.PodeInteragir();
    }

    private static GameObject EncontrarCesta(GameObject objeto)
    {
        for (Transform alvo = objeto.transform; alvo != null; alvo = alvo.parent)
        {
            if (alvo.name == "shopping_basket" || alvo.name.StartsWith("shopping_basket (", System.StringComparison.Ordinal))
                return alvo.gameObject;
        }
        return null;
    }

    private static bool AlvoMarket(GameObject objeto)
    {
        var sacola = objeto.GetComponentInParent<SacolaInterativaScript>();
        var produto = objeto.GetComponentInParent<ProdutoInterativo>();
        return (sacola != null && sacola.PodeRecolher)
            || (produto != null && produto.podePegar)
            || (MissionManager.Instance?.MissaoAtiva("pegar_cesta") == true && EncontrarCesta(objeto) != null);
    }

    private void PegarObjetoAtual()
    {
        if (!SessaoCenario.Atual.Permite("Market") || objetoAtual == null || !AlvoMarket(objetoAtual))
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

        GameObject cesta = produto == null ? EncontrarCesta(objetoAtual) : null;

        if (produto != null)
        {
            if (ProdutoManager.Instance != null)
                ProdutoManager.Instance.RegistrarProduto(produto);

            produto.gameObject.SetActive(false);
        }
        else if (cesta != null)
        {
            cestasRecolhidas.Add(cesta);
            cesta.SetActive(false);
        }

        if (MissionManager.NivelMarketIntermediario(DadosJogador.nivelUsuario))
        {
            if (produto == null && cesta != null)
                MissionManager.Instance?.RegistrarCestaMarketIntermediario();
        }
        // Mantém a lógica original de conclusão da primeira missão nos demais níveis.
        else if (cesta != null && MissionManager.Instance?.MissaoAtiva("pegar_cesta") == true)
        {
            MissionManager.Instance.ConcluirMissaoMarket("pegar_cesta", "marco_fluxo", "cesta_coletada");
            if (!MissionManager.Instance.UsaCatalogoMarket)
            {

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