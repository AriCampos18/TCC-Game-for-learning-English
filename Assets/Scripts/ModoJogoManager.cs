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
using Oculus.Interaction.Input;
using Oculus.Interaction.Surfaces;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;

[DefaultExecutionOrder(-101)]
public class ModoJogoManager : MonoBehaviour
{
    public static ModoJogoManager Instance;

    [Header("Estado da interface")]
    public bool uiMode;

    [Header("Apresentação desktop")]
    [SerializeField] private Camera cameraCompartilhada;
    [SerializeField] private Transform jogador;
    [SerializeField] private Canvas[] canvasesInterface;

    [Header("Autenticação")]
    [SerializeField] private Transform loginCameraAnchor;

    private bool enquadramentoLogin, poseLoginAplicada, poseLoginVR;
    private Transform paiAntesDoLogin;
    private Vector3 posicaoAntesDoLogin;
    private Quaternion rotacaoAntesDoLogin;
    internal bool EnquadramentoLoginAtivo => enquadramentoLogin;

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
    private BotaoConfiguracoes aberturaConfiguracoes;
    private ModalFinalManager hud;
    private FirstPlayerController controleJogador;
    private LocomotionMediator mediadorXR;
    private ContinuousMoveProvider movimentoXR;
    private Transform rigMeta;
    private Vector3 posicaoOriginalRig;
    private Quaternion rotacaoOriginalRig, rotacaoOriginalOffset;
    private Transform offsetCamera;
    private ApresentacaoCanvas[] apresentacoesRV;
    private Canvas canvasGameplayRV;
    private ApresentacaoCanvas gameplayRV;
    private GameObject[] paineisExclusivosGameplay;
    private PointableCanvasModule moduloMeta;
    private InputSystemUIInputModule moduloDesktop;
    private bool? eventosConfiguradosRV;
    private readonly Dictionary<Canvas, PonteiroCanvasRaiz> ponteirosRaiz = new();
    private readonly List<int> ponteirosEncerrados = new();
    private readonly Dictionary<Canvas, Camera> camerasInterfaceAntesDoRaycast = new();

    private sealed class PonteiroCanvasRaiz : IPointableCanvas
    {
        public Canvas Canvas { get; }
        public event System.Action<PointerEvent> WhenPointerEventRaised;
        public PonteiroCanvasRaiz(Canvas canvas) => Canvas = canvas;
        public void ProcessPointerEvent(PointerEvent evento) => WhenPointerEventRaised?.Invoke(evento);
    }
    private RayInteractor raioDireito;
    private ControllerSelector seletorDireito;
    private ControllerButtonUsage botoesDireito;
    private int quadroInteracaoUi = -1;
    private const string NomeRaioCanvas = "ISDK_RayCanvasInteraction";
    private VRVirtualKeyboard tecladoVR;
    internal GameObject TecladoPara(Transform painel) => tecladoVR != null ? tecladoVR.PainelPara(painel) : null;
    private bool apresentacaoXRAtiva;
    private readonly List<(PointableCanvas ponto, RectTransform painel, Canvas canvas, BoundsClipper limites,
        RenderMode modo, Camera camera)> raiosModais = new();
    private readonly List<GameObject> raiosCriados = new();

    private static readonly string[] PaineisComRaioVR =
    {
        "AuthenticationUI/ModalLogin", "AuthenticationUI/ModalRegister", "VirtualKeyboard",
        "ModalEsqueciSenha", "ModalCodigoEmail", "ModalNovaSenha", "ModalConfiguracoes",
        "ModalGerenciarExercicios", "ModalFormFalaNPC", "ModalSelecionarTipoEx", "ModalSelecionarExFala",
        "FormWriting", "FormularioSpeaking", "FormularioAlternativas", "FormularioCriarFrase",
        "FormularioCompletarFrase", "FormularioAssociacao", "ModalErroAviso",
        "AuthenticationUI/VirtualKeyboard", "ModalBoasVindas", "ModalNivelamento", "ModalRespostaNivelamento",
        "ModalExercíciosPanel", "ModalAvisoErroLoginRegister", "Configuracoes", "ModalEscolhaCenario", "ModalInstrucoes",
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
        }

        private void AdicionarPainelRecuperacao(string nome, float altura)
        {
            if (rect.Find(nome) is RectTransform painel)
                paineisRecuperacao.Add(new PainelRecuperacaoDesktop(painel, altura));
        }

        internal void DefinirVR(Camera cameraVR)
        {
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = cameraVR;
            rect.SetParent(cameraVR.transform, false);
            rect.SetLocalPositionAndRotation(Vector3.forward * 1.5f, Quaternion.identity);
            rect.localScale = Vector3.one * 0.002f;
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
            AtualizarInterfaceXR(false);
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
        else if (ModoVR)
        {
            ConfigurarEventoUi(true);
            PrepararSuperficiesXR();
            AtualizarSuperficiesXR(true);
        }
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
            hud = FindAnyObjectByType<ModalFinalManager>(FindObjectsInactive.Include);
            uiMode = false;
            if (canvasesInterface != null)
                foreach (var canvas in canvasesInterface)
                {
                    if (canvas == null) continue;
                    if (selecaoCenario == null)
                        selecaoCenario = canvas.GetComponentInChildren<ModalSelecaoCenario>(true);
                    if (configuracoes == null)
                        configuracoes = canvas.GetComponentInChildren<ModalConfiguracoes>(true);
                    if (aberturaConfiguracoes == null)
                        aberturaConfiguracoes = canvas.GetComponentInChildren<BotaoConfiguracoes>(true);
                    if (tecladoVR == null)
                        tecladoVR = canvas.GetComponentInChildren<VRVirtualKeyboard>(true);
                }
            if (jogador != null)
            {
                controleJogador = jogador.GetComponent<FirstPlayerController>();
                mediadorXR = jogador.GetComponent<LocomotionMediator>();
                movimentoXR = jogador.GetComponent<ContinuousMoveProvider>();
                rigMeta = jogador.Find("[BuildingBlock] Camera Rig");
                if (rigMeta != null)
                {
                    posicaoOriginalRig = rigMeta.localPosition;
                    rotacaoOriginalRig = rigMeta.localRotation;
                }
            }
            if (cameraCompartilhada != null)
            {
                var cameraTransform = cameraCompartilhada.transform;
                paiCameraVR = cameraTransform.parent;
                posicaoCameraVR = cameraTransform.localPosition;
                rotacaoCameraVR = cameraTransform.localRotation;
                escalaCameraVR = cameraTransform.localScale;
                offsetCamera = paiCameraVR != null && paiCameraVR.name == "CameraOffSet" ? paiCameraVR : null;
                if (offsetCamera != null) rotacaoOriginalOffset = offsetCamera.localRotation;
            }
            DesabilitarUiDosInteratoresXri();
            ConfigurarEventoUi(ModoVR);
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += AoCarregarCena;
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
        if (Instance != this) return;
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= AoCarregarCena;
        LimparPonteirosRaiz();
        Instance = null;
    }

    private void Update()
    {
        // No PC, mantém o atalho original para liberar/travar o cursor.
        // No VR, a interação com a interface é feita pelo Ray Interactor
        // dos controles, então não usamos o cursor do mouse.
        bool vr = ModoVR;
        if (poseLoginAplicada && poseLoginVR != vr)
            RestaurarPoseAntesDoLogin();
        AtualizarApresentacao();
        AtualizarInterfaceXR(vr);
        AtualizarPoseLogin(vr);
        if (eventosConfiguradosRV != vr) ConfigurarEventoUi(vr);
        if (vr && canvasesInterface != null)
            foreach (var canvas in canvasesInterface)
                if (canvas != null) camerasInterfaceAntesDoRaycast[canvas] = canvas.worldCamera;
        AtualizarBotaoDireito(vr);
        if (vr && tecladoVR != null) tecladoVR.ObservarSelecao(cameraCompartilhada);
        bool entradaAberta = selecaoCenario != null && selecaoCenario.EntradaAberta;
        bool autenticacaoAberta = AuthenticationUIController.Instance?.EstaAberta == true;
        bool permitirLocomocao = vr && !entradaAberta && !autenticacaoAberta
            && (controleJogador == null || controleJogador.ControleAtivo);
        if (mediadorXR != null) mediadorXR.enabled = vr;
        if (movimentoXR != null) movimentoXR.enabled = permitirLocomocao;
        var checkpoint = CheckpointJogo.Instance;
        aberturaConfiguracoes?.AtualizarVisibilidade(!autenticacaoAberta && !entradaAberta
            && AuthenticationUIController.Instance?.Session?.IsAuthenticated == true
            && checkpoint != null && CheckpointJogo.PodeAvancar && hud != null
            && hud.modalBarraProgressao != null && hud.modalBarraProgressao.activeInHierarchy
            && hud.modalMissoes != null && hud.modalMissoes.activeInHierarchy);
        AtualizarSuperficiesXR(vr);
        if (configuracoes != null)
            configuracoes.AplicarPreferenciaTurn(checkpoint != null ? checkpoint.ModoGiro : "snap", permitirLocomocao);
        if (!ModoVR && !entradaAberta && !autenticacaoAberta && !BotaoConfiguracoes.DialogoSuspenso
            && Input.GetKeyDown(KeyCode.LeftAlt))
        {
            AlternarModoUI();
        }
    }

    internal void DefinirEnquadramentoLogin(bool ativo)
    {
        enquadramentoLogin = ativo;
        if (ativo)
            AtualizarPoseLogin(ModoVR);
        else
            RestaurarPoseAntesDoLogin();
    }

    internal void PrepararDestinoDoCheckpoint()
    {
        // Em RV, o enquadramento é dono da origem do jogador até o transporte.
        // No desktop, a câmera permanece na âncora enquanto o Player muda de posição.
        if (ModoVR || (poseLoginAplicada && poseLoginVR))
            DefinirEnquadramentoLogin(false);
    }

    private void AtualizarPoseLogin(bool vr)
    {
        if (!enquadramentoLogin || loginCameraAnchor == null || cameraCompartilhada == null
            || jogador == null || vr != apresentacaoXRAtiva || vr == apresentacaoDesktop
            || poseLoginAplicada)
            return;

        if (vr)
        {
            var headset = InputDevices.GetDeviceAtXRNode(XRNode.Head);
            if (!headset.isValid
                || (headset.TryGetFeatureValue(CommonUsages.isTracked, out bool rastreado) && !rastreado))
                return;
        }

        var cameraTransform = cameraCompartilhada.transform;
        var objeto = vr ? jogador : cameraTransform;
        posicaoAntesDoLogin = vr ? objeto.position : objeto.localPosition;
        rotacaoAntesDoLogin = vr ? objeto.rotation : objeto.localRotation;
        poseLoginAplicada = true;
        poseLoginVR = vr;

        if (!vr)
        {
            paiAntesDoLogin = cameraTransform.parent;
            cameraTransform.SetParent(loginCameraAnchor, true);
            cameraTransform.SetPositionAndRotation(loginCameraAnchor.position, loginCameraAnchor.rotation);
            return;
        }

        // Aplica somente o rumo à origem do jogador. A pose local do headset é dada pelo rastreamento.
        var rumo = Quaternion.Euler(0f, loginCameraAnchor.eulerAngles.y, 0f);
        var deslocamento = rumo * (Quaternion.Inverse(jogador.rotation) * (cameraTransform.position - jogador.position));
        PosicionarOrigemJogador(loginCameraAnchor.position - deslocamento, rumo);
    }

    private void RestaurarPoseAntesDoLogin()
    {
        if (!poseLoginAplicada)
            return;

        poseLoginAplicada = false;
        if (poseLoginVR && jogador != null)
            PosicionarOrigemJogador(posicaoAntesDoLogin, rotacaoAntesDoLogin);
        else if (!poseLoginVR && cameraCompartilhada != null)
        {
            var cameraTransform = cameraCompartilhada.transform;
            cameraTransform.SetParent(paiAntesDoLogin, true);
            cameraTransform.SetLocalPositionAndRotation(posicaoAntesDoLogin, rotacaoAntesDoLogin);
            cameraTransform.localScale = escalaCameraVR;
            if (controleJogador != null) controleJogador.SincronizarRotacaoInterna();
        }
    }

    private void PosicionarOrigemJogador(Vector3 posicao, Quaternion rotacao)
    {
        var controlador = jogador.GetComponent<CharacterController>();
        bool reativar = controlador != null && controlador.enabled;
        if (reativar) controlador.enabled = false;
        jogador.SetPositionAndRotation(posicao, rotacao);
        if (reativar) controlador.enabled = true;
    }

    private void LateUpdate()
    {
        // The Meta module temporarily assigns its projection camera to the root Canvas.
        // Restore the presentation camera after the EventSystem has processed the pointers.
        foreach (var item in camerasInterfaceAntesDoRaycast)
            if (item.Key != null && item.Key.worldCamera == RaycasterModalVR.CameraDeProjecao)
                item.Key.worldCamera = item.Value;
        camerasInterfaceAntesDoRaycast.Clear();
    }

    private void AtualizarInterfaceXR(bool vr)
    {
        if (vr == apresentacaoXRAtiva || cameraCompartilhada == null)
            return;

        apresentacaoXRAtiva = vr;
        if (rigMeta != null)
            rigMeta.SetLocalPositionAndRotation(vr ? Vector3.zero : posicaoOriginalRig,
                vr ? Quaternion.identity : rotacaoOriginalRig);
        if (offsetCamera != null)
            offsetCamera.localRotation = vr ? Quaternion.identity : rotacaoOriginalOffset;
        cameraCompartilhada.transform.localPosition = vr ? Vector3.zero : posicaoCameraVR;

        if (vr)
        {
            ConfigurarEventoUi(true);
            PrepararSuperficiesXR();
            foreach (var item in raiosModais)
                if (item.ponto != null && item.canvas != null)
                {
                    item.canvas.renderMode = RenderMode.WorldSpace;
                    item.canvas.worldCamera = cameraCompartilhada;
                }
            var estados = new List<ApresentacaoCanvas>();
            if (canvasesInterface != null)
                foreach (var raiz in canvasesInterface)
                {
                    if (raiz == null) continue;
                    if (raiz.GetComponentInChildren<AuthenticationUIController>(true) != null)
                    {
                        var estado = new ApresentacaoCanvas(raiz);
                        estados.Add(estado);
                        estado.DefinirVR(cameraCompartilhada);
                    }
                    else
                    {
                        canvasGameplayRV = raiz;
                        var paineis = new List<GameObject>();
                        foreach (string nome in new[] { "ModalBoasVindas", "ModalNivelamento", "ModalRespostaNivelamento",
                            "ModalInstrucoes", "ModalFinal", "ModalExercíciosPanel", "ModalErroSpeaking",
                            "ModalAvisoCamposVazio", "ModalAvisoRevisao" })
                        {
                            var painel = raiz.transform.Find(nome);
                            if (painel != null) paineis.Add(painel.gameObject);
                        }
                        paineisExclusivosGameplay = paineis.ToArray();
                    }
                }
            apresentacoesRV = estados.ToArray();
        }
        else
        {
            RestaurarGameplayRV();
            if (apresentacoesRV != null)
                foreach (var estado in apresentacoesRV) estado.Restaurar();
            apresentacoesRV = null;
            foreach (var item in raiosModais)
                if (item.ponto != null && item.canvas != null)
                {
                    item.canvas.renderMode = item.modo;
                    item.canvas.worldCamera = item.camera;
                }
            LimparPonteirosRaiz();
            foreach (var raio in raiosCriados)
                if (raio != null) raio.SetActive(false);
            if (seletorDireito != null) seletorDireito.ControllerButtonUsage = botoesDireito;
            if (tecladoVR != null) tecladoVR.HideKeyboard();
        }
    }

    private void RestaurarGameplayRV()
    {
        gameplayRV?.Restaurar();
        gameplayRV = null;
    }

    private void PrepararSuperficiesXR()
    {
        if (canvasesInterface == null) return;
        var preparados = new HashSet<RectTransform>();
        foreach (var item in raiosModais)
            if (item.painel != null && item.ponto != null)
                preparados.Add(item.painel);

        int totalAntes = raiosModais.Count;
        foreach (var raiz in canvasesInterface)
        {
            if (raiz == null) continue;
            foreach (var raycaster in raiz.GetComponentsInChildren<GraphicRaycaster>(true))
                if (raycaster is not RaycasterModalVR)
                    PrepararRaycaster(raycaster.GetComponent<Canvas>());
            foreach (string caminho in PaineisComRaioVR)
            {
                if (raiz.transform.Find(caminho) is not RectTransform painel || !preparados.Add(painel))
                    continue;
                PrepararSuperficieXR(painel);
            }
        }
        if (raiosModais.Count != totalAntes)
            Debug.Log($"Interface RV: {raiosModais.Count} painéis com superfícies Meta configuradas.", this);
    }

    private void PrepararSuperficieXR(RectTransform painel)
    {
        string componente = NomeRaioCanvas;
        T ObterOuCriar<T>(GameObject objeto) where T : Component
        {
            componente = typeof(T).Name;
            var instancia = objeto.GetComponent<T>();
            if (instancia == null)
                instancia = objeto.AddComponent<T>();
            if (instancia == null)
                throw new MissingComponentException($"Não foi possível obter ou adicionar {componente} em {objeto.name}.");
            return instancia;
        }

        try
        {
            var raio = painel.Find(NomeRaioCanvas);
            if (raio == null)
            {
                var objeto = new GameObject(NomeRaioCanvas);
                objeto.SetActive(false);
                raio = objeto.transform;
                raio.SetParent(painel, false);
                raiosCriados.Add(objeto);
            }
            else
                raio.gameObject.SetActive(false);

            var canvas = ObterOuCriar<Canvas>(painel.gameObject);
            PrepararRaycaster(canvas);
            var ponto = ObterOuCriar<PointableCanvas>(raio.gameObject);
            var interacao = ObterOuCriar<RayInteractable>(raio.gameObject);
            var superficie = raio.GetComponentInChildren<ClippedPlaneSurface>(true);
            var limites = raio.GetComponentInChildren<BoundsClipper>(true);
            componente = nameof(ClippedPlaneSurface);
            var suporte = superficie != null ? superficie.transform : limites != null ? limites.transform : raio.Find("Surface");
            if (suporte == null)
            {
                suporte = new GameObject("Surface").transform;
                suporte.SetParent(raio, false);
            }
            if (superficie == null || limites == null || !(superficie.BackingSurface is PlaneSurface existente) || existente == null)
            {
                var plano = ObterOuCriar<PlaneSurface>(suporte.gameObject);
                if (limites == null)
                    limites = ObterOuCriar<BoundsClipper>(suporte.gameObject);
                if (superficie == null)
                    superficie = ObterOuCriar<ClippedPlaneSurface>(suporte.gameObject);
                componente = nameof(ClippedPlaneSurface);
                plano.InjectAllPlaneSurface(PlaneSurface.NormalFacing.Backward, false);
                superficie.InjectAllClippedPlaneSurface(plano, new IBoundsClipper[] { limites });
            }
            else
            {
                // A previous attempt may have created the clipper without wiring it.
                var clippers = superficie.GetClippers();
                bool possuiLimites = false;
                if (clippers != null)
                    foreach (var clipper in clippers)
                        if (clipper is BoundsClipper vinculado && vinculado == limites)
                            possuiLimites = true;
                if (!possuiLimites)
                    superficie.InjectClippers(new IBoundsClipper[] { limites });
            }

            componente = nameof(PointableCanvas);
            Canvas raiz = canvas.rootCanvas;
            ponto.enabled = false;
            ponto.InjectCanvas(raiz);
            ponto.WhenPointerEventRaised += evento => EncaminharPonteiro(raiz, painel, evento);
            componente = nameof(RayInteractable);
            interacao.InjectSurface(superficie);
            interacao.InjectOptionalPointableElement(ponto);
            componente = nameof(Canvas);
            var modoAnterior = canvas.renderMode;
            var cameraAnterior = canvas.worldCamera;
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = cameraCompartilhada;
            raiosModais.Add((ponto, painel, canvas, limites, modoAnterior, cameraAnterior));
        }
        catch (System.Exception erro)
        {
            Debug.LogError($"Interface RV [{painel.name}] — {componente}: {erro.GetType().Name}: {erro.Message}", painel);
        }
    }

    private void AtualizarSuperficiesXR(bool vr)
    {
        if (!vr) return;
        if (canvasGameplayRV != null && paineisExclusivosGameplay != null
            && System.Array.Exists(paineisExclusivosGameplay, painel => painel.activeInHierarchy))
        {
            if (gameplayRV == null)
            {
                gameplayRV = new ApresentacaoCanvas(canvasGameplayRV);
                gameplayRV.DefinirVR(cameraCompartilhada);
            }
        }
        else
            RestaurarGameplayRV();

        foreach (var item in raiosModais)
        {
            if (item.ponto == null || item.painel == null || item.canvas == null) continue;
            bool ativo = PermitePonteiro(item.painel);
            if (ativo && item.limites != null)
            {
                var rect = item.painel.rect;
                item.limites.Position = item.limites.transform.InverseTransformPoint(item.painel.TransformPoint(rect.center));
                var largura = item.limites.transform.InverseTransformVector(item.painel.TransformVector(Vector3.right * rect.width));
                var altura = item.limites.transform.InverseTransformVector(item.painel.TransformVector(Vector3.up * rect.height));
                item.limites.Size = new Vector3(largura.magnitude, altura.magnitude, item.limites.Size.z);
            }
            if (!ativo) CancelarPonteirosDoPainel(item.painel, item.canvas.rootCanvas);
            if (item.ponto.gameObject.activeSelf != ativo) item.ponto.gameObject.SetActive(ativo);
        }
    }

    internal static bool PermitePonteiro(Transform painel)
    {
        if (!painel.gameObject.activeInHierarchy) return false;
        if (!BotaoConfiguracoes.PermitePonteiro(painel))
            return false;
        for (var atual = painel; atual != null; atual = atual.parent)
            if (atual.TryGetComponent<CanvasGroup>(out var grupo))
            {
                if (!grupo.blocksRaycasts || !grupo.interactable) return false;
                if (grupo.ignoreParentGroups) break;
            }
        return true;
    }

    internal bool GarantirSuperficieConfiguracoesRV(Canvas canvas)
    {
        if (canvas == null || canvas.transform is not RectTransform painel)
            return false;

        bool SuperficieVinculada() => raiosModais.Exists(item =>
            item.painel == painel && item.canvas == canvas && item.ponto != null && item.limites != null);

        if (!SuperficieVinculada())
            PrepararSuperficieXR(painel);
        return SuperficieVinculada() && canvas.TryGetComponent<RaycasterModalVR>(out var raycaster)
            && raycaster != null && raycaster.enabled;
    }

    private void PrepararRaycaster(Canvas canvas)
    {
        if (canvas == null || canvas.TryGetComponent<RaycasterModalVR>(out _)) return;

        var anterior = canvas.GetComponent<GraphicRaycaster>();
        var scoped = canvas.gameObject.AddComponent<RaycasterModalVR>();
        if (scoped == null)
            throw new MissingComponentException($"Não foi possível preparar o raycaster de {canvas.name}.");
        if (anterior == null) return;

        scoped.ignoreReversedGraphics = anterior.ignoreReversedGraphics;
        scoped.blockingObjects = anterior.blockingObjects;
        scoped.blockingMask = anterior.blockingMask;
        scoped.enabled = anterior.enabled;
        anterior.enabled = false;
        Destroy(anterior);
    }

    internal RectTransform PainelDe(Transform alvo)
    {
        for (var atual = alvo; atual != null; atual = atual.parent)
            foreach (var item in raiosModais)
                if (item.painel == atual) return item.painel;
        return null;
    }

    private void EncaminharPonteiro(Canvas raiz, RectTransform painel, PointerEvent evento)
    {
        if (!ModoVR || moduloMeta == null || !moduloMeta.isActiveAndEnabled) return;
        if (!PermitePonteiro(painel) && evento.Type != PointerEventType.Unhover && evento.Type != PointerEventType.Cancel)
        {
            CancelarPonteirosDoPainel(painel, raiz);
            return;
        }
        var paineis = RaycasterModalVR.PaineisPorPonteiro;
        if (evento.Type == PointerEventType.Hover)
        {
            if (!PermitePonteiro(painel)) return;
            if (!ponteirosRaiz.TryGetValue(raiz, out var ponteiro))
            {
                ponteiro = new PonteiroCanvasRaiz(raiz);
                PointableCanvasModule.RegisterPointableCanvas(ponteiro);
                ponteirosRaiz.Add(raiz, ponteiro);
            }
            if (paineis.ContainsKey(evento.Identifier)) return;
            paineis.Add(evento.Identifier, painel);
            ponteiro.ProcessPointerEvent(evento);
            return;
        }

        if (!paineis.TryGetValue(evento.Identifier, out var painelAtual) || painelAtual != painel ||
            !ponteirosRaiz.TryGetValue(raiz, out var existente))
            return;
        if (evento.Type == PointerEventType.Unhover || evento.Type == PointerEventType.Cancel)
            paineis.Remove(evento.Identifier);
        existente.ProcessPointerEvent(evento);
    }

    private void CancelarPonteirosDoPainel(RectTransform painel, Canvas raiz)
    {
        if (!ponteirosRaiz.TryGetValue(raiz, out var ponteiro)) return;

        var paineis = RaycasterModalVR.PaineisPorPonteiro;
        foreach (var item in paineis)
            if (item.Value == painel) ponteirosEncerrados.Add(item.Key);

        foreach (var id in ponteirosEncerrados)
        {
            paineis.Remove(id);
            ponteiro.ProcessPointerEvent(new PointerEvent(id, PointerEventType.Cancel, Pose.identity));
        }
        ponteirosEncerrados.Clear();
    }

    private void LimparPonteirosRaiz()
    {
        RaycasterModalVR.PaineisPorPonteiro.Clear();
        foreach (var ponteiro in ponteirosRaiz.Values)
            PointableCanvasModule.UnregisterPointableCanvas(ponteiro);
        ponteirosRaiz.Clear();
    }

    private void AoCarregarCena(UnityEngine.SceneManagement.Scene cena,
        UnityEngine.SceneManagement.LoadSceneMode modo)
    {
        DesabilitarUiDosInteratoresXri();
        ConfigurarEventoUi(ModoVR);
    }

    private static void DesabilitarUiDosInteratoresXri()
    {
        // XRI creates XRUIInputModule on registration when its UI integration is enabled.
        foreach (var raio in FindObjectsByType<XRRayInteractor>(FindObjectsInactive.Include))
            raio.enableUIInteraction = false;
        foreach (var toque in FindObjectsByType<XRPokeInteractor>(FindObjectsInactive.Include))
            toque.enableUIInteraction = false;
        foreach (var interator in FindObjectsByType<NearFarInteractor>(FindObjectsInactive.Include))
            interator.enableUIInteraction = false;
    }

    private void ConfigurarEventoUi(bool vr)
    {
        if (moduloMeta == null)
            moduloMeta = FindAnyObjectByType<PointableCanvasModule>(FindObjectsInactive.Include);
        if (moduloMeta == null) return;
        var sistema = moduloMeta.GetComponent<EventSystem>();
        if (sistema == null) return;
        if (moduloDesktop == null)
            moduloDesktop = sistema.GetComponent<InputSystemUIInputModule>();
        BaseInputModule esperado = vr ? moduloMeta : (BaseInputModule)moduloDesktop;
        if (esperado == null || (eventosConfiguradosRV == vr && esperado.enabled
            && EventSystem.current == sistema && sistema.currentInputModule == esperado
            && (vr ? moduloDesktop == null || !moduloDesktop.enabled : !moduloMeta.enabled))) return;
        eventosConfiguradosRV = vr;

        foreach (var outro in FindObjectsByType<BaseInputModule>(FindObjectsSortMode.None))
            if (outro != esperado && (outro.gameObject == sistema.gameObject
                || outro is UnityEngine.XR.Interaction.Toolkit.UI.XRUIInputModule))
                outro.enabled = false;
        EventSystem.current = sistema;
        esperado.enabled = true;
    }

    private void AtualizarBotaoDireito(bool vr)
    {
        if (!vr || jogador == null) return;
        if (seletorDireito == null)
            foreach (var raio in jogador.GetComponentsInChildren<RayInteractor>(true))
            {
                var seletor = raio.GetComponentInChildren<ControllerSelector>(true);
                if (seletor == null || seletor.Controller?.Handedness != Handedness.Right) continue;
                raioDireito = raio;
                seletorDireito = seletor;
                botoesDireito = seletor.ControllerButtonUsage;
                break;
            }
        if (seletorDireito == null) return;
        bool interfaceAlvo = RaioDireitoNaInterface();
        if (interfaceAlvo) quadroInteracaoUi = Time.frameCount;
        // The stock controller ray uses Any with TriggerButton.
        if (seletorDireito.RequireButtonUsages == ControllerSelector.ControllerSelectorLogicOperator.Any)
            seletorDireito.ControllerButtonUsage = interfaceAlvo
                ? botoesDireito | ControllerButtonUsage.PrimaryButton : botoesDireito;
    }

    private bool RaioDireitoNaInterface()
    {
        if (raioDireito == null || !raioDireito.isActiveAndEnabled) return false;
        var alvo = raioDireito.SelectedInteractable != null
            ? raioDireito.SelectedInteractable : raioDireito.Interactable;
        if (alvo == null || !alvo.TryGetComponent<PointableCanvas>(out var ponto))
            return false;
        foreach (var item in raiosModais)
            if (item.ponto == ponto)
                return item.painel != null && PermitePonteiro(item.painel);
        return false;
    }

    internal bool InterfaceConsomeInteracaoVR =>
        ModoVR && (quadroInteracaoUi == Time.frameCount || RaioDireitoNaInterface());

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
