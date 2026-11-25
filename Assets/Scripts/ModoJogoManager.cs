// using UnityEngine;

// public class ModoJogoManager : MonoBehaviour
// {
//     public static ModoJogoManager Instance;
//     public bool uiMode;

//     void Awake()
//     {
//         if (Instance == null)
//         {
//             Instance = this;
//             uiMode = false;
//             DontDestroyOnLoad(gameObject);
//         }
//         else
//         {
//             Destroy(gameObject);
//         }
//     }

//     void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.LeftAlt))
//         {
//             uiMode = !uiMode;

//             Cursor.lockState = uiMode ? CursorLockMode.None : CursorLockMode.Locked;
//             Cursor.visible = uiMode;
//         }
//     }
// }

using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;

public class ModoJogoManager : MonoBehaviour
{
    public static ModoJogoManager Instance;

    [Header("Estado da interface")]
    public bool uiMode;

    [Header("Apresentação desktop")]
    [SerializeField] private Camera cameraCompartilhada;
    [SerializeField] private Transform jogador;
    [SerializeField] private Canvas[] canvasesInterface;

    private Transform paiCameraVR;
    private Vector3 posicaoCameraVR, escalaCameraVR;
    private Quaternion rotacaoCameraVR;
    private ApresentacaoCanvas[] apresentacoes;
    private bool apresentacaoDesktop;
    private RectTransform loginAjustado;
    private Vector2 posicaoLoginVR;
    private readonly System.Collections.Generic.List<Canvas> interfacesSimulador = new();
    private ModalSelecaoCenario selecaoCenario;
    private ModalConfiguracoes configuracoes;
    private FirstPlayerController controleJogador;
    private LocomotionMediator mediadorXR;
    private ContinuousMoveProvider movimentoXR;
    private Transform rigMeta;
    private Vector3 posicaoOriginalRig;
    private VRVirtualKeyboard tecladoVR;
    private bool apresentacaoXRAtiva;
    private readonly List<(GameObject raio, Canvas canvas)> raiosModais = new();

    private static readonly string[] PaineisComRaioVR =
    {
        "AuthenticationUI/ModalLogin", "AuthenticationUI/ModalRegister", "VirtualKeyboard",
        "ModalEsqueciSenha", "ModalCodigoEmail", "ModalNovaSenha", "ModalConfiguracoes",
        "ModalGerenciarExercicios", "ModalFormFalaNPC", "ModalSelecionarTipoEx", "ModalSelecionarExFala",
        "FormWriting", "FormularioSpeaking", "FormularioAlternativas", "FormularioCriarFrase",
        "FormularioCompletarFrase", "FormularioAssociacao", "ModalErroAviso",
        "ModalAvisoErroLoginRegister", "Configuracoes", "ModalEscolhaCenario", "ModalInstrucoes",
        "ModalAvisoCamposVazio", "ModalAvisoRevisao", "ModalErroSpeaking", "ModalFinal"
    };

    private sealed class ApresentacaoCanvas
    {
        private readonly Canvas canvas;
        private readonly Transform pai;
        private readonly int indice;
        private readonly UnityEngine.UI.CanvasScaler scaler;
        private readonly UnityEngine.UI.CanvasScaler.ScaleMode escalaUi;
        private readonly UnityEngine.UI.CanvasScaler.ScreenMatchMode correspondencia;
        private readonly Vector2 resolucao;
        private readonly RectTransform rect;
        private readonly RenderMode modo;
        private readonly Camera camera;
        private readonly float distancia;
        private readonly Vector3 posicao, escala;
        private readonly Quaternion rotacao;
        private readonly Vector2 ancoraMin, ancoraMax, tamanho, pivo;
        private readonly Vector2 resolucaoDesktop;
        private readonly ModalSelecaoCenario selecao;
        private readonly RectTransform botaoConfiguracoes;
        private readonly Vector2 ancoraMinBotaoVR, ancoraMaxBotaoVR, posicaoBotaoVR;
        private readonly RectTransform instrucoes;
        private readonly Vector2 posicaoInstrucoesVR;
        private readonly List<PainelRecuperacaoDesktop> paineisRecuperacao = new();

        internal ApresentacaoCanvas(Canvas canvas)
        {
            this.canvas = canvas;
            rect = (RectTransform)canvas.transform;
            pai = rect.parent;
            indice = rect.GetSiblingIndex();
            scaler = canvas.GetComponent<UnityEngine.UI.CanvasScaler>();
            if (scaler != null)
            {
                escalaUi = scaler.uiScaleMode;
                correspondencia = scaler.screenMatchMode;
                resolucao = scaler.referenceResolution;
            }
            modo = canvas.renderMode;
            camera = canvas.worldCamera;
            distancia = canvas.planeDistance;
            posicao = rect.anchoredPosition3D;
            escala = rect.localScale;
            rotacao = rect.localRotation;
            ancoraMin = rect.anchorMin;
            ancoraMax = rect.anchorMax;
            tamanho = rect.sizeDelta;
            pivo = rect.pivot;
            bool canvasDaInterfaceInicial = canvas.GetComponentInChildren<AuthenticationUIController>(true) != null;
            resolucaoDesktop = canvasDaInterfaceInicial ? new Vector2(1280, 720) : new Vector2(1760, 1000);
            selecao = canvas.GetComponentInChildren<ModalSelecaoCenario>(true);
            instrucoes = canvas.GetComponentInChildren<ModalInstrucoes>(true)?.transform as RectTransform;
            if (instrucoes != null) posicaoInstrucoesVR = instrucoes.anchoredPosition;
            if (canvasDaInterfaceInicial)
            {
                AdicionarPainelRecuperacao("ModalEsqueciSenha", 416f);
                AdicionarPainelRecuperacao("ModalCodigoEmail", 448f);
                AdicionarPainelRecuperacao("ModalNovaSenha", 460f);
            }
            botaoConfiguracoes = canvas.GetComponentInChildren<BotaoConfiguracoes>(true)
                ?.botaoConfiguracoes?.transform as RectTransform;
            if (botaoConfiguracoes != null)
            {
                ancoraMinBotaoVR = botaoConfiguracoes.anchorMin;
                ancoraMaxBotaoVR = botaoConfiguracoes.anchorMax;
                posicaoBotaoVR = botaoConfiguracoes.anchoredPosition;
            }
        }

        private void AdicionarPainelRecuperacao(string nome, float altura)
        {
            if (rect.Find(nome) is RectTransform painel)
                paineisRecuperacao.Add(new PainelRecuperacaoDesktop(painel, altura));
        }

        internal void DefinirDesktop()
        {
            // An Overlay canvas must be a root; the original parent is restored in XR.
            if (pai != null) rect.SetParent(null, false);
            rect.localRotation = Quaternion.identity;
            rect.localScale = Vector3.one;
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            if (scaler != null)
            {
                scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = resolucaoDesktop;
                scaler.screenMatchMode = UnityEngine.UI.CanvasScaler.ScreenMatchMode.Expand;
            }
            if (botaoConfiguracoes != null)
            {
                botaoConfiguracoes.anchorMin = Vector2.one;
                botaoConfiguracoes.anchorMax = Vector2.one;
                botaoConfiguracoes.anchoredPosition = new Vector2(-56, -56);
            }
            if (selecao != null) selecao.DefinirLayoutDesktop(true);
            if (instrucoes != null) instrucoes.anchoredPosition = Vector2.zero;
            foreach (var painel in paineisRecuperacao) painel.Aplicar();
        }

        internal void Restaurar()
        {
            if (canvas == null) return;
            rect.SetParent(pai, false);
            rect.SetSiblingIndex(indice);
            canvas.renderMode = modo;
            canvas.worldCamera = camera;
            if (scaler != null)
            {
                scaler.uiScaleMode = escalaUi;
                scaler.referenceResolution = resolucao;
                scaler.screenMatchMode = correspondencia;
            }
            canvas.planeDistance = distancia;
            rect.anchorMin = ancoraMin;
            rect.anchorMax = ancoraMax;
            rect.sizeDelta = tamanho;
            rect.pivot = pivo;
            rect.anchoredPosition3D = posicao;
            rect.localRotation = rotacao;
            rect.localScale = escala;
            if (botaoConfiguracoes != null)
            {
                botaoConfiguracoes.anchorMin = ancoraMinBotaoVR;
                botaoConfiguracoes.anchorMax = ancoraMaxBotaoVR;
                botaoConfiguracoes.anchoredPosition = posicaoBotaoVR;
            }
            if (selecao != null) selecao.DefinirLayoutDesktop(false);
            if (instrucoes != null) instrucoes.anchoredPosition = posicaoInstrucoesVR;
            foreach (var painel in paineisRecuperacao) painel.Restaurar();
        }
    }

    private sealed class PainelRecuperacaoDesktop
    {
        private const float Largura = 520f;
        private readonly RectTransform painel;
        private readonly Vector2 ancoraMin, ancoraMax, tamanho, posicao;
        private readonly float altura;
        private readonly float deslocamentoConteudo;
        private readonly List<(RectTransform rect, Vector2 posicao)> filhos = new();

        internal PainelRecuperacaoDesktop(RectTransform painel, float altura)
        {
            this.painel = painel;
            this.altura = altura;
            ancoraMin = painel.anchorMin;
            ancoraMax = painel.anchorMax;
            tamanho = painel.sizeDelta;
            posicao = painel.anchoredPosition;
            deslocamentoConteudo = (Largura - painel.rect.width) * 0.5f;
            foreach (Transform filho in painel)
                if (filho is RectTransform rect && rect.anchorMin.x == 0f && rect.anchorMax.x == 0f)
                    filhos.Add((rect, rect.anchoredPosition));
        }

        internal void Aplicar()
        {
            painel.anchorMin = painel.anchorMax = Vector2.one * 0.5f;
            painel.sizeDelta = new Vector2(Largura, altura);
            painel.anchoredPosition = Vector2.zero;
            foreach (var filho in filhos)
                filho.rect.anchoredPosition = filho.posicao + Vector2.right * deslocamentoConteudo;
        }

        internal void Restaurar()
        {
            painel.anchorMin = ancoraMin;
            painel.anchorMax = ancoraMax;
            painel.sizeDelta = tamanho;
            painel.anchoredPosition = posicao;
            foreach (var filho in filhos)
                filho.rect.anchoredPosition = filho.posicao;
        }
    }

    private void AtualizarApresentacao()
    {
        bool desktop = !ModoVR;
        if (desktop == apresentacaoDesktop || cameraCompartilhada == null || jogador == null)
            return;

        var cameraTransform = cameraCompartilhada.transform;
        if (desktop)
        {
            paiCameraVR = cameraTransform.parent;
            posicaoCameraVR = cameraTransform.localPosition;
            rotacaoCameraVR = cameraTransform.localRotation;
            escalaCameraVR = cameraTransform.localScale;
            cameraTransform.SetParent(jogador, false);
            cameraTransform.SetLocalPositionAndRotation(posicaoCameraVR, Quaternion.identity);
            var estados = new System.Collections.Generic.List<ApresentacaoCanvas>();
            if (canvasesInterface != null)
                foreach (var canvas in canvasesInterface)
                {
                    if (canvas == null) continue;
                    var estado = new ApresentacaoCanvas(canvas);
                    estados.Add(estado);
                    estado.DefinirDesktop();
                }
            apresentacoes = estados.ToArray();
            loginAjustado = VinculosModaisTcc.Painel(cameraCompartilhada.gameObject, "ModalLogin")?.transform as RectTransform;
            var cadastro = VinculosModaisTcc.Painel(cameraCompartilhada.gameObject, "ModalRegister")?.transform as RectTransform;
            if (loginAjustado != null && cadastro != null && cadastro.parent == loginAjustado.parent
                && cadastro.anchorMin == loginAjustado.anchorMin && cadastro.anchorMax == loginAjustado.anchorMax
                && cadastro.pivot == loginAjustado.pivot)
            {
                posicaoLoginVR = loginAjustado.anchoredPosition;
                loginAjustado.anchoredPosition = Vector2.zero;
            }
            else
                loginAjustado = null;

            OcultarInterfaceSimulador();
        }
        else
        {
            cameraTransform.SetParent(paiCameraVR, false);
            cameraTransform.SetLocalPositionAndRotation(posicaoCameraVR, rotacaoCameraVR);
            cameraTransform.localScale = escalaCameraVR;
            foreach (var estado in apresentacoes)
                estado.Restaurar();
            apresentacoes = null;
            if (loginAjustado != null) loginAjustado.anchoredPosition = posicaoLoginVR;
            loginAjustado = null;
            foreach (var canvas in interfacesSimulador)
                if (canvas != null) canvas.enabled = true;
            interfacesSimulador.Clear();
        }

        apresentacaoDesktop = desktop;
        if (desktop)
            jogador.GetComponent<FirstPlayerController>()?.SincronizarRotacaoInterna();
    }

    private System.Collections.IEnumerator Start()
    {
        // The simulator can instantiate its UI on the first frame.
        yield return null;
        if (apresentacaoDesktop) OcultarInterfaceSimulador();
    }

    private void OcultarInterfaceSimulador()
    {
        foreach (var nome in new[] { "XR Device Simulator", "XR Device Simulator UI", "XR Device Simulator UI(Clone)" })
        {
            var objeto = VinculosModaisTcc.Painel(cameraCompartilhada.gameObject, nome);
            if (objeto == null) continue;
            foreach (var canvas in objeto.GetComponentsInChildren<Canvas>(true))
            {
                if (!canvas.enabled) continue;
                if (!interfacesSimulador.Contains(canvas)) interfacesSimulador.Add(canvas);
                canvas.enabled = false;
            }
        }
    }

    // Detecta se um dispositivo XR está ativo (ex.: Meta Quest via Quest Link).
    public bool ModoVR => XRSettings.isDeviceActive;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            uiMode = false;
            if (canvasesInterface != null)
                foreach (var canvas in canvasesInterface)
                {
                    if (canvas == null) continue;
                    selecaoCenario ??= canvas.GetComponentInChildren<ModalSelecaoCenario>(true);
                    configuracoes ??= canvas.GetComponentInChildren<ModalConfiguracoes>(true);
                    tecladoVR ??= canvas.GetComponentInChildren<VRVirtualKeyboard>(true);
                }
            if (jogador != null)
            {
                controleJogador = jogador.GetComponent<FirstPlayerController>();
                mediadorXR = jogador.GetComponent<LocomotionMediator>();
                movimentoXR = jogador.GetComponent<ContinuousMoveProvider>();
                rigMeta = jogador.Find("[BuildingBlock] Camera Rig");
                if (rigMeta != null) posicaoOriginalRig = rigMeta.localPosition;
            }
            AtualizarApresentacao();
            AtualizarInterfaceXR(ModoVR);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void Update()
    {
        // No PC, mantém o atalho original para liberar/travar o cursor.
        // No VR, a interação com a interface é feita pelo Ray Interactor
        // dos controles, então não usamos o cursor do mouse.
        AtualizarApresentacao();
        bool vr = ModoVR;
        AtualizarInterfaceXR(vr);
        AtualizarSuperficiesXR(vr);
        if (vr && tecladoVR != null) tecladoVR.ObservarSelecao(cameraCompartilhada);
        bool entradaAberta = selecaoCenario != null && selecaoCenario.EntradaAberta;
        bool autenticacaoAberta = AuthenticationUIController.Instance?.EstaAberta == true;
        bool permitirLocomocao = vr && !entradaAberta && !autenticacaoAberta
            && (controleJogador == null || controleJogador.ControleAtivo);
        if (mediadorXR != null) mediadorXR.enabled = vr;
        if (movimentoXR != null) movimentoXR.enabled = permitirLocomocao;
        configuracoes?.AplicarPreferenciaTurn(CheckpointJogo.Instance?.ModoGiro ?? "snap", permitirLocomocao);
        if (!ModoVR && !entradaAberta && !autenticacaoAberta
            && Input.GetKeyDown(KeyCode.LeftAlt))
        {
            AlternarModoUI();
        }
    }

    private void AtualizarInterfaceXR(bool vr)
    {
        if (vr == apresentacaoXRAtiva)
            return;

        apresentacaoXRAtiva = vr;
        if (rigMeta != null)
            rigMeta.localPosition = vr ? Vector3.zero : posicaoOriginalRig;

        if (vr && raiosModais.Count == 0 && selecaoCenario != null && canvasesInterface != null)
        {
            var modelo = selecaoCenario.transform.Find("ISDK_RayCanvasInteraction");
            if (modelo != null)
            {
                foreach (var raiz in canvasesInterface)
                {
                    if (raiz == null) continue;
                    foreach (string caminho in PaineisComRaioVR)
                    {
                        if (raiz.transform.Find(caminho) is not RectTransform painel
                            || painel.Find(modelo.name) != null)
                            continue;
                        var canvas = painel.GetComponent<Canvas>();
                        if (canvas == null) canvas = painel.gameObject.AddComponent<Canvas>();
                        if (painel.GetComponent<GraphicRaycaster>() == null)
                            painel.gameObject.AddComponent<GraphicRaycaster>();
                        var raio = Instantiate(modelo.gameObject, painel, false);
                        raio.name = modelo.name;
                        raio.GetComponent<PointableCanvas>().InjectCanvas(canvas);
                        raiosModais.Add((raio, canvas));
                    }
                }
            }
        }

        foreach (var item in raiosModais)
        {
            if (item.canvas == null) continue;
            item.canvas.renderMode = vr ? RenderMode.WorldSpace : RenderMode.ScreenSpaceOverlay;
            item.canvas.worldCamera = vr ? cameraCompartilhada : null;
        }
        if (!vr && tecladoVR != null) tecladoVR.HideKeyboard();
    }

    private void AtualizarSuperficiesXR(bool vr)
    {
        foreach (var item in raiosModais)
        {
            if (item.raio == null || item.canvas == null) continue;
            var grupo = item.canvas.GetComponent<CanvasGroup>();
            bool ativo = vr && (grupo == null || (grupo.blocksRaycasts && grupo.interactable));
            if (item.raio.activeSelf != ativo) item.raio.SetActive(ativo);
        }
    }

    public void AlternarModoUI()
    {
        DefinirModoUI(!uiMode);
    }

    public void AtivarModoUI()
    {
        DefinirModoUI(true);
    }

    public void DesativarModoUI()
    {
        DefinirModoUI(false);
    }

    public void DefinirModoUI(bool ativar)
    {
        uiMode = ativar;

        // Cursor só existe no fluxo desktop.
        if (!ModoVR)
        {
            Cursor.lockState = uiMode
                ? CursorLockMode.None
                : CursorLockMode.Locked;

            Cursor.visible = uiMode;
        }
    }
}
