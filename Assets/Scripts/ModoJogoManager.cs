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

using UnityEngine;
using UnityEngine.XR;

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
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.screenMatchMode = UnityEngine.UI.CanvasScaler.ScreenMatchMode.Expand;
            }
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
                loginAjustado.anchoredPosition = cadastro.anchoredPosition;
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
            DontDestroyOnLoad(gameObject);
            AtualizarApresentacao();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // No PC, mantém o atalho original para liberar/travar o cursor.
        // No VR, a interação com a interface é feita pelo Ray Interactor
        // dos controles, então não usamos o cursor do mouse.
        AtualizarApresentacao();
        if (!ModoVR && AuthenticationUIController.Instance?.EstaAberta != true
            && Input.GetKeyDown(KeyCode.LeftAlt))
        {
            AlternarModoUI();
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
