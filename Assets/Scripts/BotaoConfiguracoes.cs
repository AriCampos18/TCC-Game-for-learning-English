using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR;

public class BotaoConfiguracoes : MonoBehaviour
{
    public Button botaoConfiguracoes;
    public GameObject modalConfiguracoes;
    public FirstPlayerController firstPlayerControllerScript;

    private int visitaAoAbrir;
    private bool aberto;
    private bool controleAnterior;
    private bool emJogoAnterior;
    private bool emDialogoAnterior;
    private bool modoInterfaceAnterior;
    private CursorLockMode cursorAnterior;
    private bool cursorEstavaVisivel;
    private Canvas canvasModal;
    private GameObject bloqueadorDesktop;
    private ModalConfiguracoes controladorModal;
    private Transform painelSuperior;
    private Transform tecladoSuperior;
    private readonly Dictionary<Canvas, (bool sobrescrevia, int camada, int ordem)> ordemOriginal = new();
    private readonly Dictionary<Canvas, int> ordemAtual = new();
    private readonly List<Canvas> canvasesRetirados = new();
    private static BotaoConfiguracoes donoDaPausa;
    private static int versaoDaPausa;
    internal static bool DialogoSuspenso => donoDaPausa != null;
    internal static bool PermitePonteiro(Transform alvo)
    {
        if (donoDaPausa == null || donoDaPausa.painelSuperior == null)
            return true;
        var painel = donoDaPausa.painelSuperior;
        var teclado = donoDaPausa.tecladoSuperior;
        return alvo == painel || alvo.IsChildOf(painel)
            || (teclado != null && (alvo == teclado || alvo.IsChildOf(teclado)));
    }

    internal static async Task AguardarComandoDialogo(Func<bool> pressionado, Action validar, Action aoPressionar = null)
    {
        async Task ProximoQuadro()
        {
            await Awaitable.NextFrameAsync();
            validar?.Invoke();
        }

        while (true)
        {
            while (DialogoSuspenso || pressionado())
                await ProximoQuadro();
            int versao = versaoDaPausa;
            while (!DialogoSuspenso && !pressionado())
                await ProximoQuadro();
            if (DialogoSuspenso || versao != versaoDaPausa)
                continue;

            aoPressionar?.Invoke();
            while (!DialogoSuspenso && pressionado())
                await ProximoQuadro();
            // A UI também pode tratar a soltura. Confirma só depois de ela processar esse quadro.
            await ProximoQuadro();
            if (!DialogoSuspenso && versao == versaoDaPausa)
                return;
        }
    }

    private void Start()
    {
        if (botaoConfiguracoes != null)
            botaoConfiguracoes.onClick.AddListener(AbrirConfiguracoes);
    }

    internal void AtualizarVisibilidade(bool interfaceDisponivel)
    {
        bool visivel = interfaceDisponivel && !aberto;
        if (botaoConfiguracoes != null && botaoConfiguracoes.gameObject.activeSelf != visivel)
            botaoConfiguracoes.gameObject.SetActive(visivel);

        if (aberto)
        {
            try
            {
                if (modalConfiguracoes == null || canvasModal == null)
                    throw new MissingComponentException("O painel ou o Canvas das configurações deixou de estar disponível.");
                AtualizarPrimeiroPlano();
            }
            catch (Exception erro) { CancelarAbertura(erro); }
        }
    }

    private void AtualizarPrimeiroPlano()
    {
        ordemAtual.Clear();
        painelSuperior = tecladoSuperior = null;
        Canvas frente = null;
        void ElevarPainel(GameObject painel, int ordem)
        {
            if (painel == null || !painel.activeInHierarchy) return;
            var canvas = painel == modalConfiguracoes ? canvasModal : PrepararCanvas(painel);
            var raiz = canvas.rootCanvas;
            if (raiz == null)
                throw new MissingComponentException("O painel de configurações está sem Canvas raiz.");
            ordemAtual[raiz] = ordem;
            ordemAtual[canvas] = ordem + 2;
            foreach (var filho in canvas.GetComponentsInChildren<Canvas>(true))
                if (filho != null && filho != canvas && filho.overrideSorting)
                    ordemAtual[filho] = ordem + 2;
            frente = canvas;
            painelSuperior = painel.transform;
        }

        ElevarPainel(modalConfiguracoes, 100);
        ElevarPainel(controladorModal.modalInstrucoes, 110);
        var painelAdmin = controladorModal.modalGerenciarExs;
        if (painelAdmin != null && painelAdmin.activeInHierarchy &&
            painelAdmin.TryGetComponent<ModalGerenciarExercicios>(out var admin) && admin != null)
        {
            ElevarPainel(painelAdmin, 110);
            ElevarPainel(admin.SubmodalAberto, 120);
            if (admin.AvisoAberto == null && admin.SubmodalAberto != null && XRSettings.isDeviceActive)
            {
                var teclado = ModoJogoManager.Instance != null
                    ? ModoJogoManager.Instance.TecladoPara(admin.SubmodalAberto.transform) : null;
                if (teclado != null && teclado.activeInHierarchy)
                {
                    var painel = painelSuperior;
                    var canvas = frente;
                    ElevarPainel(teclado, 130);
                    tecladoSuperior = teclado.transform;
                    painelSuperior = painel;
                    frente = canvas;
                }
            }
            ElevarPainel(admin.AvisoAberto, 140);
        }

        int camada = SortingLayer.NameToID("ConfiguracoesFrente");
        bool bloquearDesktop = frente != null && !XRSettings.isDeviceActive;
        if (bloquearDesktop && bloqueadorDesktop == null)
        {
            bloqueadorDesktop = new GameObject("Bloqueio de ponteiro das configurações",
                typeof(RectTransform), typeof(Canvas), typeof(GraphicRaycaster), typeof(Image));
            if (!bloqueadorDesktop.TryGetComponent<RectTransform>(out var rect) || rect == null
                || !bloqueadorDesktop.TryGetComponent<Canvas>(out var canvas) || canvas == null
                || !bloqueadorDesktop.TryGetComponent<GraphicRaycaster>(out var raycaster) || raycaster == null
                || !bloqueadorDesktop.TryGetComponent<Image>(out var imagem) || imagem == null)
            {
                bloqueadorDesktop.SetActive(false);
                Destroy(bloqueadorDesktop);
                bloqueadorDesktop = null;
                throw new InvalidOperationException("Não foi possível preparar o bloqueio do ponteiro.");
            }
            imagem.color = Color.clear;
        }
        if (bloquearDesktop)
        {
            if (!bloqueadorDesktop.TryGetComponent<Canvas>(out var canvas) || canvas == null
                || bloqueadorDesktop.transform is not RectTransform rect)
                throw new MissingComponentException("O bloqueio das configurações está incompleto.");
            var raiz = frente.rootCanvas;
            if (rect.parent != raiz.transform)
            {
                rect.SetParent(raiz.transform, false);
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = rect.offsetMax = Vector2.zero;
            }
            canvas.overrideSorting = true;
            canvas.sortingLayerID = camada;
            canvas.sortingOrder = ordemAtual[frente] - 1;
        }
        if (bloqueadorDesktop != null && bloqueadorDesktop.activeSelf != bloquearDesktop)
            bloqueadorDesktop.SetActive(bloquearDesktop);

        foreach (var item in ordemOriginal)
            if (!ordemAtual.ContainsKey(item.Key))
            {
                RestaurarCanvas(item.Key, item.Value);
                canvasesRetirados.Add(item.Key);
            }
        foreach (var canvas in canvasesRetirados)
            ordemOriginal.Remove(canvas);
        canvasesRetirados.Clear();

        foreach (var item in ordemAtual)
        {
            Canvas canvas = item.Key;
            if (!ordemOriginal.ContainsKey(canvas))
                ordemOriginal.Add(canvas, (canvas.overrideSorting, canvas.sortingLayerID, canvas.sortingOrder));
            if (!canvas.isRootCanvas && !canvas.overrideSorting) canvas.overrideSorting = true;
            if (canvas.sortingLayerID != camada) canvas.sortingLayerID = camada;
            if (canvas.sortingOrder != item.Value) canvas.sortingOrder = item.Value;
        }

        var eventos = EventSystem.current;
        if (painelSuperior != null && eventos != null && eventos.currentSelectedGameObject != null
            && !PermitePonteiro(eventos.currentSelectedGameObject.transform))
            eventos.SetSelectedGameObject(null);
    }

    private static void RestaurarCanvas(Canvas canvas, (bool sobrescrevia, int camada, int ordem) estado)
    {
        if (canvas == null) return;
        canvas.sortingLayerID = estado.camada;
        canvas.sortingOrder = estado.ordem;
        canvas.overrideSorting = estado.sobrescrevia;
    }

    private void RestaurarPrimeiroPlano()
    {
        painelSuperior = tecladoSuperior = null;
        if (bloqueadorDesktop != null) bloqueadorDesktop.SetActive(false);
        foreach (var item in ordemOriginal)
            RestaurarCanvas(item.Key, item.Value);
        ordemOriginal.Clear();
    }

    private Canvas PrepararCanvas(GameObject painel)
    {
        if (!painel.TryGetComponent<Canvas>(out var canvas) || canvas == null)
            canvas = painel.AddComponent<Canvas>();
        if (canvas == null)
            throw new InvalidOperationException("Não foi possível obter o Canvas das configurações.");

        var gerenciador = ModoJogoManager.Instance;
        if (XRSettings.isDeviceActive)
        {
            if (gerenciador == null || !gerenciador.GarantirSuperficieConfiguracoesRV(canvas))
                throw new InvalidOperationException("Não foi possível vincular o ponteiro de RV às configurações.");
        }
        if (!painel.TryGetComponent<GraphicRaycaster>(out var raycaster) || raycaster == null)
        {
            raycaster = painel.AddComponent<GraphicRaycaster>();
            if (raycaster == null)
                throw new InvalidOperationException("Não foi possível obter o raycaster das configurações.");
        }
        return canvas;
    }

    private void AbrirConfiguracoes()
    {
        if (aberto || modalConfiguracoes == null)
            return;

        visitaAoAbrir = SessaoCenario.Atual.Versao;
        emJogoAnterior = SessaoCenario.Atual.EmJogo;
        emDialogoAnterior = GameProgress.EstaEmDialogo;
        controleAnterior = firstPlayerControllerScript != null && firstPlayerControllerScript.ControleAtivo;
        modoInterfaceAnterior = ModoJogoManager.Instance != null && ModoJogoManager.Instance.uiMode;
        cursorAnterior = Cursor.lockState;
        cursorEstavaVisivel = Cursor.visible;

        try
        {
            if (!modalConfiguracoes.TryGetComponent<ModalConfiguracoes>(out var modal) || modal == null)
                throw new InvalidOperationException("O painel está sem o controlador ModalConfiguracoes.");
            controladorModal = modal;
            canvasModal = PrepararCanvas(modalConfiguracoes);
            modal.VincularAbertura(this);

            aberto = true;
            donoDaPausa = this;
            ++versaoDaPausa;
            if (botaoConfiguracoes != null) botaoConfiguracoes.gameObject.SetActive(false);
            if (firstPlayerControllerScript != null) firstPlayerControllerScript.DesativarControle();
            if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
            modalConfiguracoes.SetActive(true);
            AtualizarPrimeiroPlano();
            if (ModoJogoManager.Instance != null) ModoJogoManager.Instance.DefinirModoUI(true);
        }
        catch (Exception erro)
        {
            CancelarAbertura(erro);
        }
    }

    internal void AlternarPeloControleRV()
    {
        if (aberto)
        {
            // As telas secundárias mantêm seus próprios fluxos de retorno.
            if (modalConfiguracoes != null && modalConfiguracoes.activeInHierarchy
                && painelSuperior == modalConfiguracoes.transform)
                FecharConfiguracoes();
        }
        else if (botaoConfiguracoes != null && botaoConfiguracoes.isActiveAndEnabled
            && botaoConfiguracoes.IsInteractable())
        {
            AbrirConfiguracoes();
        }
    }

    private void CancelarAbertura(Exception erro)
    {
        Debug.LogException(erro, this);
        if (aberto)
            FecharConfiguracoes();
    }

    public void FecharConfiguracoes()
    {
        bool restaurarContexto = aberto;
        aberto = false;
        if (donoDaPausa == this) donoDaPausa = null;
        RestaurarPrimeiroPlano();
        if (modalConfiguracoes != null) modalConfiguracoes.SetActive(false);
        if (!restaurarContexto)
            return;

        bool mesmaVisita = visitaAoAbrir == SessaoCenario.Atual.Versao && CheckpointJogo.PodeAvancar;
        if (firstPlayerControllerScript != null)
        {
            if (mesmaVisita && controleAnterior && SessaoCenario.Atual.EmJogo && !GameProgress.EstaEmDialogo)
                firstPlayerControllerScript.AtivarControle();
            else
                firstPlayerControllerScript.DesativarControle();
        }

        bool mesmoContexto = mesmaVisita && emJogoAnterior == SessaoCenario.Atual.EmJogo
            && emDialogoAnterior == GameProgress.EstaEmDialogo;
        if (mesmoContexto && ModoJogoManager.Instance != null)
            ModoJogoManager.Instance.DefinirModoUI(modoInterfaceAnterior);
        if (mesmoContexto && !XRSettings.isDeviceActive)
        {
            Cursor.lockState = cursorAnterior;
            Cursor.visible = cursorEstavaVisivel;
        }
    }

    private void OnDestroy()
    {
        if (donoDaPausa == this)
        {
            donoDaPausa = null;
            RestaurarPrimeiroPlano();
        }
        if (bloqueadorDesktop != null) Destroy(bloqueadorDesktop);
    }
}
