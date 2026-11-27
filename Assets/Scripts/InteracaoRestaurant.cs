using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public sealed class InteracaoRestaurant : MonoBehaviour
{
    internal enum Etapa { Levantar, Retornar, Comer }
    internal enum EstadoRefeicao { Oculta, Servida, Restante }

    [SerializeField] private RestaurantWaiterNPC garcom;
    [SerializeField] private Collider mesa;
    [SerializeField] private string checkpointKey;
    [SerializeField] private Collider prato;
    [SerializeField] private GameObject refeicao;
    [SerializeField] private Transform jogador;
    [SerializeField] private Transform assento;
    [SerializeField] private Transform posicaoEmPe;
    [SerializeField] private CanvasGroup telaPreta;
    [SerializeField, Min(0f)] private float duracaoFade = 0.35f;
    [SerializeField, Min(0f)] private float duracaoRefeicao = 2.5f;
    [SerializeField] private Behaviour[] suspenderDuranteFade;
    [SerializeField] private Behaviour[] suspenderEnquantoSentado;

    private readonly List<Behaviour> suspensosNoAssento = new List<Behaviour>();

    internal static InteracaoRestaurant Pendente { get; private set; }
    private Etapa etapa;
    private Collider alvo;
    private bool confirmado;
    private bool sentado;
    private int quadroLiberado;
    private int visitaEtapa;
    private Transform inteiro, parcial, fritas, uva, laranja, limao;
    private readonly List<Collider> alvosRefeicao = new();
    internal DestacarObjeto DestaqueRefeicao { get; private set; }
    private Canvas canvasFadeGerado;
    private CanvasGroup grupoFadeGerado;
    private static InteracaoRestaurant transicao;
    internal static bool EmTransicaoRefeicao => transicao != null;

    private void Awake()
    {
        if (mesa == null && TryGetComponent<MeshFilter>(out var filtro) && filtro.sharedMesh != null)
        {
            var colisorMesa = gameObject.AddComponent<MeshCollider>();
            colisorMesa.sharedMesh = filtro.sharedMesh;
            mesa = colisorMesa;
        }
        int camada = LayerMask.NameToLayer("Interativo");
        if (mesa != null && camada >= 0)
            mesa.gameObject.layer = camada;
        if (prato != null && prato != mesa)
            prato.enabled = false;
        if (refeicao == null) return;

        inteiro = refeicao.transform.Find("hamburger");
        parcial = refeicao.transform.Find("umamusume_half_eaten_burger");
        fritas = refeicao.transform.Find("french_fries_from_poly_by_google");
        uva = refeicao.transform.Find("glass_of_tea");
        laranja = refeicao.transform.Find("Orange Juice");
        if (laranja == null) laranja = refeicao.transform.Find("Orange juice");
        limao = refeicao.transform.Find("drink");
        DestaqueRefeicao = refeicao.GetComponent<DestacarObjeto>();
        if (camada >= 0)
        {
            refeicao.layer = camada;
            foreach (var modelo in refeicao.GetComponentsInChildren<Renderer>(true))
            {
                if (modelo is not MeshRenderer && modelo is not SkinnedMeshRenderer) continue;
                var objeto = modelo.gameObject;
                objeto.layer = camada;
                if (!objeto.TryGetComponent<Collider>(out var colisor))
                {
                    var caixa = objeto.AddComponent<BoxCollider>();
                    caixa.center = modelo.localBounds.center;
                    caixa.size = Vector3.Max(modelo.localBounds.size, Vector3.one * 0.001f);
                    colisor = caixa;
                }
                colisor.isTrigger = true;
                colisor.enabled = false;
                alvosRefeicao.Add(colisor);
                if (DestaqueRefeicao != null && !objeto.TryGetComponent<Outline>(out _))
                    objeto.AddComponent<Outline>().enabled = false;
            }
        }
        refeicao.SetActive(false);
    }

    internal void ApresentarRefeicao(EstadoRefeicao estado)
    {
        if (refeicao == null) return;
        if (DestaqueRefeicao != null) DestaqueRefeicao.SetHighlight(false);
        HabilitarColisoresRefeicao(false);
        bool servida = estado == EstadoRefeicao.Servida;
        bool restante = estado == EstadoRefeicao.Restante;
        string nivel = SessaoCenario.Atual.Nivel;
        Ativar(inteiro, servida || (restante && parcial == null));
        Ativar(parcial, restante);
        Ativar(fritas, servida && nivel == "A2-B1");
        Ativar(uva, servida && nivel == "A1");
        Ativar(laranja, servida && (nivel == "A1-A2" || nivel == "A2"));
        Ativar(limao, servida && nivel == "B1");
        refeicao.SetActive(estado != EstadoRefeicao.Oculta);
    }

    private static void Ativar(Transform item, bool ativo)
    {
        if (item != null) item.gameObject.SetActive(ativo);
    }

    private void HabilitarColisoresRefeicao(bool habilitar)
    {
        foreach (var colisor in alvosRefeicao)
            if (colisor != null)
                colisor.enabled = habilitar && colisor.gameObject.activeInHierarchy;
    }

    internal bool PertenceAoGarcom(RestaurantWaiterNPC candidato) => garcom == candidato;

    internal string ChaveCheckpoint => !string.IsNullOrEmpty(checkpointKey)
        && System.Text.RegularExpressions.Regex.IsMatch(checkpointKey, @"\A[a-zA-Z0-9_-]{1,64}\z")
        ? checkpointKey : null;

    internal bool Sentado => sentado;

    internal void RetomarPosicao(bool estavaSentado)
    {
        if (estavaSentado)
            Sentar();
        else
        {
            Encerrar();
            MoverJogador(posicaoEmPe);
        }
    }

    internal bool PodeSentar(Collider colisor) => mesa != null && colisor == mesa;

    internal void TentarSentar()
    {
        if (garcom != null && garcom.PodeIniciarAtendimento())
            garcom.IniciarAtendimento(this);
    }

    internal void Sentar()
    {
        if (mesa == null || (prato == null && alvosRefeicao.Count == 0) || mesa == prato)
            throw new InvalidOperationException("Configure a mesa e a refeição do Restaurant.");

        MoverJogador(assento);
        sentado = true;
        Suspender(suspenderEnquantoSentado, suspensosNoAssento);
    }

    internal async Task Aguardar(Etapa proxima)
    {
        bool deveEstarSentado = proxima != Etapa.Retornar;
        Collider colisor = proxima == Etapa.Comer ? prato : mesa;
        bool refeicaoComModelos = proxima == Etapa.Comer && alvosRefeicao.Count != 0;
        if (!isActiveAndEnabled || Pendente != null || sentado != deveEstarSentado
            || (!refeicaoComModelos && (colisor == null || (proxima != Etapa.Comer && !colisor.enabled)
                || !colisor.gameObject.activeInHierarchy))
            || (refeicaoComModelos && !alvosRefeicao.Exists(c => c != null && c.gameObject.activeInHierarchy)))
            throw new InvalidOperationException($"Interação do Restaurant indisponível: {proxima}.");

        etapa = proxima;
        alvo = colisor;
        if (refeicaoComModelos)
            HabilitarColisoresRefeicao(true);
        else if (proxima == Etapa.Comer)
            colisor.enabled = true;
        confirmado = false;
        quadroLiberado = Time.frameCount;
        visitaEtapa = SessaoCenario.Atual.Versao;
        Pendente = this;
        try
        {
            while (!confirmado)
            {
                await ProximoFrame();
                if (!refeicaoComModelos && (alvo == null || !alvo.enabled || !alvo.gameObject.activeInHierarchy))
                    throw new InvalidOperationException($"Collider indisponível durante a etapa {etapa}.");
            }
        }
        finally
        {
            if (Pendente == this)
                Pendente = null;
            alvo = null;
            if (refeicaoComModelos) HabilitarColisoresRefeicao(false);
            if (proxima == Etapa.Comer && colisor != null)
                colisor.enabled = false;
        }

        if (proxima == Etapa.Levantar)
        {
            MoverJogador(posicaoEmPe);
            sentado = false;
            Restaurar(suspensosNoAssento);
        }
        else if (proxima == Etapa.Retornar)
        {
            MoverJogador(assento);
            sentado = true;
            Suspender(suspenderEnquantoSentado, suspensosNoAssento);
        }
    }

    internal bool AlvoDaRefeicao(Collider colisor) => Pendente == this && etapa == Etapa.Comer
        && !confirmado && colisor != null && alvosRefeicao.Contains(colisor);

    internal bool MirarRefeicao(Ray raio, float alcance, LayerMask mascara, out RaycastHit acerto)
    {
        acerto = default;
        bool encontrou = false;
        if (Pendente != this || etapa != Etapa.Comer || confirmado) return false;
        foreach (var colisor in alvosRefeicao)
            if (colisor != null && colisor.enabled && colisor.gameObject.activeInHierarchy
                && (mascara.value & (1 << colisor.gameObject.layer)) != 0
                && colisor.Raycast(raio, out var hit, alcance))
            {
                acerto = hit;
                alcance = hit.distance;
                encontrou = true;
            }
        return encontrou;
    }

    internal void TentarConfirmar(Collider colisor)
    {
        if (SessaoCenario.Atual.Permite("Restaurant", visitaEtapa) && Pendente == this && !confirmado
            && (colisor == alvo || AlvoDaRefeicao(colisor)) && Time.frameCount > quadroLiberado)
            confirmado = true;
    }

    internal void Encerrar()
    {
        sentado = false;
        ApresentarRefeicao(EstadoRefeicao.Oculta);
        Restaurar(suspensosNoAssento);
    }

    private CanvasGroup PrepararTelaPreta()
    {
        bool vr = UnityEngine.XR.XRSettings.isDeviceActive;
        if (!vr && telaPreta != null) return telaPreta;

        if (grupoFadeGerado == null)
        {
            var tela = new GameObject("Fade da refeição", typeof(RectTransform), typeof(Canvas), typeof(CanvasGroup),
                typeof(UnityEngine.UI.Image), typeof(UnityEngine.UI.GraphicRaycaster));
            tela.transform.SetParent(transform, false);
            canvasFadeGerado = tela.GetComponent<Canvas>();
            grupoFadeGerado = tela.GetComponent<CanvasGroup>();
            grupoFadeGerado.alpha = 0f;
            grupoFadeGerado.blocksRaycasts = false;
            tela.GetComponent<UnityEngine.UI.Image>().color = Color.black;
        }
        canvasFadeGerado.sortingLayerName = "ConfiguracoesFrente";
        canvasFadeGerado.sortingOrder = short.MaxValue;
        if (vr)
        {
            var modo = ModoJogoManager.Instance;
            var camera = modo != null ? modo.CameraDaInterfaceRV : null;
            if (camera == null)
                throw new InvalidOperationException("A câmera rastreada do Restaurant está indisponível.");
            canvasFadeGerado.renderMode = RenderMode.WorldSpace;
            canvasFadeGerado.worldCamera = camera;
            var area = (RectTransform)canvasFadeGerado.transform;
            area.SetParent(camera.transform, false);
            float distancia = camera.nearClipPlane + 0.02f;
            area.SetLocalPositionAndRotation(Vector3.forward * distancia, Quaternion.identity);
            area.localScale = Vector3.one * 0.001f;
            area.sizeDelta = TamanhoDoFadeStereo(camera, distancia) / area.localScale.x;
        }
        else
        {
            canvasFadeGerado.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasFadeGerado.transform.SetParent(null, false);
        }
        return grupoFadeGerado;
    }

    private static Vector2 TamanhoDoFadeStereo(Camera camera, float distancia)
    {
        var vista = camera.transform;
        var plano = new Plane(vista.forward, vista.TransformPoint(Vector3.forward * distancia));
        var metade = Vector2.one * Mathf.Max(0.5f, distancia * 2f);
        void Medir(Camera.MonoOrStereoscopicEye olho)
        {
            for (int x = 0; x <= 1; x++)
                for (int y = 0; y <= 1; y++)
                {
                    var raio = camera.ViewportPointToRay(new Vector3(x, y), olho);
                    if (!plano.Raycast(raio, out float alcance)) continue;
                    var canto = vista.InverseTransformPoint(raio.GetPoint(alcance));
                    metade = Vector2.Max(metade, new Vector2(Mathf.Abs(canto.x), Mathf.Abs(canto.y)));
                }
        }
        if (camera.stereoEnabled)
        {
            Medir(Camera.MonoOrStereoscopicEye.Left);
            Medir(Camera.MonoOrStereoscopicEye.Right);
        }
        else
            Medir(Camera.MonoOrStereoscopicEye.Mono);
        return metade * 2.2f;
    }

    private void OnDestroy()
    {
        if (transicao == this) transicao = null;
        if (canvasFadeGerado != null) Destroy(canvasFadeGerado.gameObject);
    }

    private void Suspender(Behaviour[] componentes, List<Behaviour> suspensos)
    {
        if (componentes == null)
            return;

        foreach (Behaviour item in componentes)
        {
            if (item == null || !item.enabled || item == this || item == telaPreta)
                continue;

            suspensos.Add(item);
            item.enabled = false;
        }
    }

    private static void Restaurar(List<Behaviour> suspensos)
    {
        foreach (Behaviour item in suspensos)
            if (item != null)
                item.enabled = true;

        suspensos.Clear();
    }

    private void MoverJogador(Transform destino)
    {
        if (jogador == null || destino == null)
            throw new InvalidOperationException("Configure o jogador, o assento e a posição em pé no Restaurant.");

        CharacterController controlador = jogador.GetComponent<CharacterController>();
        bool habilitado = controlador != null && controlador.enabled;
        if (habilitado)
            controlador.enabled = false;
        try
        {
            jogador.position = destino.position;
        }
        finally
        {
            if (habilitado)
                controlador.enabled = true;
        }
    }

    internal async Task SimularRefeicao(EstadoRefeicao estadoFinal, Action confirmarConsumo)
    {
        if (EmTransicaoRefeicao)
            throw new InvalidOperationException("Já existe uma refeição em transição.");
        var tela = PrepararTelaPreta();
        if (tela == null || !tela.gameObject.activeInHierarchy)
            throw new InvalidOperationException("A tela preta do Restaurant precisa estar ativa.");

        int visita = SessaoCenario.Atual.Versao;
        float alphaAnterior = tela.alpha;
        bool bloqueava = tela.blocksRaycasts;
        var suspensos = new List<Behaviour>();
        transicao = this;
        try
        {
            tela.blocksRaycasts = true;
            if (!UnityEngine.XR.XRSettings.isDeviceActive)
                Suspender(suspenderDuranteFade, suspensos);

            await Fade(tela, 0f, 1f, visita);
            await ProximoFrame(visita);
            ApresentarRefeicao(estadoFinal);
            float fim = Time.unscaledTime + duracaoRefeicao;
            while (Time.unscaledTime < fim)
                await ProximoFrame(visita);
            confirmarConsumo();
            await Fade(tela, 1f, 0f, visita);
        }
        finally
        {
            if (transicao == this) transicao = null;
            if (tela != null)
            {
                tela.alpha = alphaAnterior;
                tela.blocksRaycasts = bloqueava;
            }
            Restaurar(suspensos);
        }
    }

    private async Task Fade(CanvasGroup tela, float inicio, float fim, int visita)
    {
        float comecou = Time.unscaledTime;
        while (Time.unscaledTime - comecou < duracaoFade)
        {
            float t = (Time.unscaledTime - comecou) / duracaoFade;
            tela.alpha = Mathf.SmoothStep(inicio, fim, t);
            await ProximoFrame(visita);
            if (tela == null || !tela.gameObject.activeInHierarchy)
                throw new OperationCanceledException();
        }

        tela.alpha = fim;
    }

    private async Task ProximoFrame(int? versao = null)
    {
        int visita = versao ?? SessaoCenario.Atual.Versao;
        int quadro = Time.frameCount;
        do
        {
            await Task.Yield();
            if (!SessaoCenario.Atual.Permite("Restaurant", visita) || this == null || !isActiveAndEnabled || MissionManager.Instance?.UsaCatalogoRestaurant != true)
                throw new OperationCanceledException();
        }
        while (Time.frameCount == quadro);
    }
}
