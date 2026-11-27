using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public sealed class DocumentoCursoCollege : MonoBehaviour
{
    [SerializeField] private Button botaoAbrir;
    [SerializeField] private GameObject painel;
    [SerializeField] private TMP_Text textoDocumento;
    [SerializeField] private Button botaoFechar;

    private int visita;
    private bool documentoSomenteRV;
    private bool consultaOferecida;
    private bool bPressionado;
    private readonly DocumentoNaVistaRV vistaRV = new();
    internal event System.Action ConsultaConfirmada;

    internal bool Disponivel { get; private set; }
    internal bool Consultado { get; private set; }
    internal bool Aberto => painel != null && painel.activeInHierarchy;
    internal int UltimoQuadroInteracao { get; private set; } = -1;

    internal static DocumentoCursoCollege CriarParaRV(GameObject dono)
    {
        var documento = dono.AddComponent<DocumentoCursoCollege>();
        documento.documentoSomenteRV = true;
        documento.painel = DocumentoNaVistaRV.Criar(dono.transform, "Schedule RV", out documento.textoDocumento);
        return documento;
    }

    private void Update()
    {
        if (!XRSettings.isDeviceActive)
        {
            bPressionado = false;
            vistaRV.Restaurar();
            return;
        }
        if (Aberto) vistaRV.Acompanhar(painel);
        var controle = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        bool pressionado = controle.TryGetFeatureValue(CommonUsages.secondaryButton, out bool valor) && valor;
        bool acionado = pressionado && !bPressionado;
        bPressionado = pressionado;
        if (!acionado || BotaoConfiguracoes.DialogoSuspenso || !Disponivel
            || !SessaoCenario.Atual.Permite("College", visita)) return;

        if (Aberto) Fechar();
        else if (consultaOferecida) Abrir();
    }

    private void Awake()
    {
        if (botaoAbrir != null)
            botaoAbrir.onClick.AddListener(Abrir);
        if (botaoFechar != null)
            botaoFechar.onClick.AddListener(Fechar);
        Resetar();
    }

    private void OnDestroy()
    {
        if (botaoAbrir != null)
            botaoAbrir.onClick.RemoveListener(Abrir);
        if (botaoFechar != null)
            botaoFechar.onClick.RemoveListener(Fechar);
    }

    internal void Resetar()
    {
        Disponivel = false;
        Consultado = false;
        vistaRV.Restaurar();
        if (painel != null)
            painel.SetActive(false);
        MostrarBotao(false);
    }

    internal bool Disponibilizar(string nivel)
    {
        bool somenteRV = documentoSomenteRV && XRSettings.isDeviceActive;
        if (!SessaoCenario.Atual.Permite("College") || !isActiveAndEnabled || painel == null || textoDocumento == null
            || (!somenteRV && (botaoAbrir == null || botaoFechar == null
                || botaoAbrir.transform.IsChildOf(painel.transform) || !botaoFechar.transform.IsChildOf(painel.transform)))
            || !textoDocumento.transform.IsChildOf(painel.transform)
            || (painel.transform.parent != null && !painel.transform.parent.gameObject.activeInHierarchy))
            return false;

        string data;
        string sala;
        if (nivel == "A1-A2")
        {
            data = "February 10";
            sala = "Room 204, second floor";
        }
        else if (nivel == "A2-B1")
        {
            data = "February 15";
            sala = "Room 325, third floor";
        }
        else
            return false;

        textoDocumento.text = $"Course: Computer Science\nTuition: R$ 500 per month\nClasses start: {data}\nFirst class: {sala}";
        if (somenteRV)
            textoDocumento.text = $"Schedule\n\n{textoDocumento.text}\n\nB: Close";
        visita = SessaoCenario.Atual.Versao;
        Disponivel = true;
        return true;
    }

    internal void RestaurarConsulta(bool consultado)
    {
        if (Disponivel && SessaoCenario.Atual.Permite("College", visita))
            Consultado = consultado;
    }

    internal void MostrarBotao(bool mostrar)
    {
        consultaOferecida = mostrar && Disponivel;
        if (botaoAbrir != null)
            botaoAbrir.gameObject.SetActive(mostrar && Disponivel && !Aberto);
    }

    internal bool PodeAguardarConsulta => SessaoCenario.Atual.Permite("College", visita) && isActiveAndEnabled && Disponivel
        && ((XRSettings.isDeviceActive && (consultaOferecida || Aberto))
            || (botaoAbrir != null && botaoAbrir.isActiveAndEnabled && botaoAbrir.IsInteractable())
            || (Aberto && botaoFechar != null && botaoFechar.isActiveAndEnabled && botaoFechar.IsInteractable()));

    private void Abrir()
    {
        if (!SessaoCenario.Atual.Permite("College", visita) || !isActiveAndEnabled || !Disponivel || Aberto
            || !consultaOferecida || (!XRSettings.isDeviceActive && (botaoAbrir == null || !botaoAbrir.isActiveAndEnabled)))
            return;

        painel.SetActive(true);
        if (XRSettings.isDeviceActive) vistaRV.Acompanhar(painel);
        if (!Aberto)
            return;

        UltimoQuadroInteracao = Time.frameCount;
        MostrarBotao(false);
        if (!Consultado)
        {
            Consultado = true;
            MissionManager.Instance?.RegistrarEventoCollege(MissoesCollege.Marco, MissoesCollege.DocumentoConsultado);
            ConsultaConfirmada?.Invoke();
        }
    }

    private void Fechar()
    {
        if (!Aberto)
            return;

        UltimoQuadroInteracao = Time.frameCount;
        vistaRV.Restaurar();
        painel.SetActive(false);
        MostrarBotao(true);
    }
}

internal sealed class DocumentoNaVistaRV
{
    private RectTransform rect;
    private Transform pai;
    private int indice;
    private Vector2 ancoraMin, ancoraMax, tamanho;
    private Vector3 posicao, escala;
    private Quaternion rotacao;
    private Canvas canvas;
    private bool canvasCriado;
    private RenderMode modo;
    private Camera cameraOriginal;

    internal static GameObject Criar(Transform dono, string nome, out TMP_Text texto)
    {
        var painel = new GameObject(nome, typeof(RectTransform), typeof(Canvas), typeof(Image));
        painel.SetActive(false);
        var area = (RectTransform)painel.transform;
        area.SetParent(dono, false);
        area.sizeDelta = new Vector2(560f, 340f);
        painel.GetComponent<Image>().color = new Color(0.04f, 0.06f, 0.09f, 0.94f);
        painel.GetComponent<Image>().raycastTarget = false;
        var rotulo = new GameObject("Texto", typeof(RectTransform), typeof(TextMeshProUGUI));
        var rectTexto = (RectTransform)rotulo.transform;
        rectTexto.SetParent(area, false);
        rectTexto.anchorMin = Vector2.zero;
        rectTexto.anchorMax = Vector2.one;
        rectTexto.offsetMin = new Vector2(24f, 16f);
        rectTexto.offsetMax = -rectTexto.offsetMin;
        texto = rotulo.GetComponent<TextMeshProUGUI>();
        texto.fontSize = 28f;
        texto.color = Color.white;
        texto.alignment = TextAlignmentOptions.Center;
        texto.raycastTarget = false;
        return painel;
    }

    internal void Acompanhar(GameObject painel)
    {
        if (rect != null || painel == null || painel.transform is not RectTransform area) return;
        var gerenciador = ModoJogoManager.Instance;
        var camera = gerenciador != null ? gerenciador.CameraDaInterfaceRV : null;
        if (camera == null) return;
        canvasCriado = !painel.TryGetComponent<Canvas>(out canvas) || canvas == null;
        if (canvasCriado) canvas = painel.AddComponent<Canvas>();
        if (canvas == null) return;
        rect = area;
        pai = rect.parent;
        indice = rect.GetSiblingIndex();
        ancoraMin = rect.anchorMin;
        ancoraMax = rect.anchorMax;
        tamanho = rect.sizeDelta;
        posicao = rect.anchoredPosition3D;
        escala = rect.localScale;
        rotacao = rect.localRotation;
        modo = canvas.renderMode;
        cameraOriginal = canvas.worldCamera;
        var dimensoes = rect.rect.size;
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = camera;
        rect.SetParent(camera.transform, false);
        rect.anchorMin = rect.anchorMax = Vector2.one * 0.5f;
        rect.sizeDelta = dimensoes;
        rect.SetLocalPositionAndRotation(new Vector3(0f, -0.12f, 1.15f), Quaternion.identity);
        rect.localScale = Vector3.one * (0.62f / Mathf.Max(dimensoes.x, 1f));
    }

    internal void Restaurar()
    {
        if (rect == null) return;
        rect.SetParent(pai, false);
        rect.SetSiblingIndex(indice);
        rect.anchorMin = ancoraMin;
        rect.anchorMax = ancoraMax;
        rect.sizeDelta = tamanho;
        rect.anchoredPosition3D = posicao;
        rect.localRotation = rotacao;
        rect.localScale = escala;
        if (canvas != null)
        {
            canvas.renderMode = modo;
            canvas.worldCamera = cameraOriginal;
            if (canvasCriado) Object.Destroy(canvas);
        }
        rect = null;
    }
}
