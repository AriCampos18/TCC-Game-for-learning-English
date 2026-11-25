// using UnityEngine;

// public class ModalSelecaoCenario : MonoBehaviour
// {
//     public Transform player;
//     public Transform pontoHome;
//     public Transform pontoMarket;
//     public Transform pontoCollege;
//     public Transform pontoHotel;
//     public Transform pontoAirport;
//     public Transform pontoRestaurant;
//     public Transform pontoInfoCenter;

//     private CardCenarioManager[] cardsCenarios;

//     public ModalFinalManager modalFinalManager;

//     public GameObject modalSelecaoCenario;

//     void Awake()
//     {
//         cardsCenarios = GetComponentsInChildren<CardCenarioManager>(true);
//     }

//     private void OnEnable()
//     {
//         if (GerenciadorProgressao.Instance == null)
//         {
//             Debug.LogWarning(
//                 "GerenciadorProgressao ainda não está disponível. " +
//                 "Os cards serão atualizados quando o modal for aberto novamente."
//             );

//             return;
//         }

//         AtualizarCards();
//     }

//     // Mostra somente os cenários permitidos.
//     public void AtualizarCards()
//     {
//         if (cardsCenarios == null)
//         {
//             cardsCenarios =
//                 GetComponentsInChildren<CardCenarioManager>(true);
//         }

//         Debug.Log(
//             "MODAL EXECUTOU. INSTANCE: " +
//             (GerenciadorProgressao.Instance != null
//                 ? GerenciadorProgressao.Instance.gameObject.name
//                 : "NULL")
//         );
//         GerenciadorProgressao gerenciador =
//             GerenciadorProgressao.Instance;

//         if (gerenciador == null)
//         {
//             Debug.LogError(
//                 "GerenciadorProgressao não encontrado."
//             );
//             return;
//         }

//         foreach (CardCenarioManager card in cardsCenarios)
//         {
//             if (card == null)
//                 continue;

//             bool permitido =
//                 gerenciador.PodeSelecionarCenario(
//                     card.nomeCenario
//                 );

//             card.gameObject.SetActive(permitido);
//         }
//     }

//     public void SelecionarCenario(string cenario)
//     {
//         GerenciadorProgressao gerenciador =
//             GerenciadorProgressao.Instance;

//         if (gerenciador == null)
//         {
//             Debug.LogError(
//                 "GerenciadorProgressao não encontrado."
//             );
//             return;
//         }

//         // Valida novamente antes de iniciar o cenário.
//         if (!gerenciador.PodeSelecionarCenario(cenario))
//         {
//             Debug.LogWarning(
//                 "Não é permitido escolher este cenário: " +
//                 cenario
//             );

//             AtualizarCards();
//             return;
//         }

//         if (player == null)
//         {
//             Debug.LogError(
//                 "O Transform do jogador não foi associado."
//             );
//             return;
//         }

//         //logica para transportar o usuário para o cenario selecionado
//         if(cenario == "Home")
//             player.position = pontoHome.position;
//         else if(cenario == "Market")
//             player.position = pontoMarket.position;
//         else if(cenario == "College")
//             player.position = pontoCollege.position;
//         else if(cenario == "Hotel")
//             player.position = pontoHotel.position;
//          else if(cenario == "Airport")
//             player.position = pontoAirport.position;
//         else if(cenario == "Restaurant")
//             player.position = pontoRestaurant.position;
//         else if(cenario == "Visitor Center")
//             player.position = pontoInfoCenter.position;
        
//         // Registra somente quando a escolha é válida e o destino está configurado.
//         gerenciador.RegistrarCenarioEscolhido(cenario);
//         modalFinalManager.MostrarInicioCenario(cenario);

//         modalSelecaoCenario.SetActive(false);
//     }
// }

using System;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class ModalSelecaoCenario : MonoBehaviour
{
    [Header("Jogador")]
    [Tooltip("No modo VR, associe este campo ao objeto raiz do XR Origin.")]
    public Transform player;

    [Header("Pontos de destino")]
    public Transform pontoHome;
    public Transform pontoMarket;
    public Transform pontoCollege;
    public Transform pontoHotel;
    public Transform pontoAirport;
    public Transform pontoRestaurant;
    public Transform pontoInfoCenter;
    public Transform pontoClothingStore;

    private CardCenarioManager[] cardsCenarios;

    [Header("Modais")]
    public ModalFinalManager modalFinalManager;
    public GameObject modalSelecaoCenario;

    internal bool EntradaAberta => isActiveAndEnabled
        || (modalFinalManager != null && modalFinalManager.IntroducaoAberta);

    private bool preparando;
    private bool selecaoInterrompida;
    private ScrollRect rolagem;
    private TMP_Text titulo;
    private string tituloOriginal;
    private Vector2 ancoraMinContentVR, ancoraMaxContentVR, pivoContentVR;
    private Vector2 posicaoContentVR, tamanhoContentVR, tamanhoScrollVR;
    private bool layoutDesktop;

    private bool ModoVR => XRSettings.isDeviceActive;

    void Awake()
    {
        cardsCenarios = GetComponentsInChildren<CardCenarioManager>(true);
        titulo = transform.Find("Text (TMP)")?.GetComponent<TMP_Text>();
        tituloOriginal = titulo != null ? titulo.text : null;
    }

    private void OnEnable()
    {
        if (player != null && player.TryGetComponent<FirstPlayerController>(out var controlador))
        {
            controlador.PermitirOlharSemMover(false);
            controlador.DesativarControle();
        }

        if (GerenciadorProgressao.Instance == null)
        {
            Debug.LogWarning(
                "GerenciadorProgressao ainda não está disponível. " +
                "Os cards serão atualizados quando o modal for aberto novamente."
            );

            return;
        }

        AtualizarCards();
        MostrarStatus(tituloOriginal);
        if (layoutDesktop)
            VoltarAoTopo();
        CheckpointJogo.Instance?.Selecionando();
    }

    internal void DefinirLayoutDesktop(bool desktop)
    {
        rolagem ??= GetComponentInChildren<ScrollRect>(true);
        if (rolagem == null || rolagem.content == null || layoutDesktop == desktop)
            return;

        var painel = (RectTransform)rolagem.transform;
        var content = rolagem.content;
        if (desktop)
        {
            ancoraMinContentVR = content.anchorMin;
            ancoraMaxContentVR = content.anchorMax;
            pivoContentVR = content.pivot;
            posicaoContentVR = content.anchoredPosition;
            tamanhoContentVR = content.sizeDelta;
            tamanhoScrollVR = painel.sizeDelta;
            painel.sizeDelta = new Vector2(1008f, 460f);
            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = Vector2.one;
            content.pivot = new Vector2(0.5f, 1f);
            content.sizeDelta = new Vector2(0f, tamanhoContentVR.y);
            content.anchoredPosition = Vector2.zero;
            if (isActiveAndEnabled)
                VoltarAoTopo();
        }
        else
        {
            painel.sizeDelta = tamanhoScrollVR;
            content.anchorMin = ancoraMinContentVR;
            content.anchorMax = ancoraMaxContentVR;
            content.pivot = pivoContentVR;
            content.sizeDelta = tamanhoContentVR;
            content.anchoredPosition = posicaoContentVR;
        }
        layoutDesktop = desktop;
    }

    private void VoltarAoTopo()
    {
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rolagem.content);
        rolagem.StopMovement();
        rolagem.verticalNormalizedPosition = 1f;
    }

    private void OnDisable()
    {
        // An old download must not open an introduction after leaving selection.
        if (preparando)
            selecaoInterrompida = true;
    }

    private void MostrarStatus(string mensagem)
    {
        if (titulo != null)
            titulo.text = mensagem;
    }

    // Mostra somente os cenários permitidos.
    public void AtualizarCards()
    {
        if (cardsCenarios == null)
        {
            cardsCenarios = GetComponentsInChildren<CardCenarioManager>(true);
        }

        Debug.Log(
            "MODAL EXECUTOU. INSTANCE: " +
            (GerenciadorProgressao.Instance != null
                ? GerenciadorProgressao.Instance.gameObject.name
                : "NULL")
        );

        GerenciadorProgressao gerenciador = GerenciadorProgressao.Instance;

        if (gerenciador == null)
        {
            Debug.LogError("GerenciadorProgressao não encontrado.");
            return;
        }

        foreach (CardCenarioManager card in cardsCenarios)
        {
            if (card == null)
                continue;

            bool permitido = gerenciador.PodeSelecionarCenario(card.nomeCenario);
            card.gameObject.SetActive(permitido);
        }
    }

    public async void SelecionarCenario(string cenario)
    {
        if (preparando || !isActiveAndEnabled || !CheckpointJogo.PodeAvancar)
            return;
        GerenciadorProgressao gerenciador = GerenciadorProgressao.Instance;

        if (gerenciador == null)
        {
            Debug.LogError("GerenciadorProgressao não encontrado.");
            MostrarStatus("Could not prepare the scenario. Try again.");
            return;
        }

        // Valida novamente antes de iniciar o cenário.
        if (!gerenciador.PodeSelecionarCenario(cenario))
        {
            Debug.LogWarning("Não é permitido escolher este cenário: " + cenario);
            AtualizarCards();
            MostrarStatus("Scenario unavailable. Choose another scenario.");
            return;
        }

        if (player == null)
        {
            Debug.LogError("O Transform do jogador não foi associado.");
            MostrarStatus("Scenario unavailable. Choose another scenario.");
            return;
        }

        Transform destino = ObterPontoDestino(cenario);

        if (destino == null)
        {
            Debug.LogError(
                "O ponto de destino do cenário '" + cenario +
                "' não foi associado ou o cenário não possui um destino configurado."
            );
            MostrarStatus("Scenario unavailable. Choose another scenario.");
            return;
        }

        // No VR, player deve ser a raiz do XR Origin.
        // Assim, o rig inteiro é deslocado e a posição local da câmera
        // (altura e deslocamento do headset) é preservada.
        if (modalFinalManager == null || MissionManager.Instance == null || modalSelecaoCenario == null)
        {
            Debug.LogError("A seleção requer ModalFinalManager, MissionManager e o painel de seleção.");
            MostrarStatus("Could not open the scenario. Try again.");
            return;
        }

        var sessao = SessaoCenario.Atual;
        int visita = sessao.Preparar(cenario, DadosJogador.nivelUsuario);
        if (visita == 0)
        {
            MostrarStatus("Could not prepare the scenario. Try again.");
            return;
        }

        preparando = true;
        selecaoInterrompida = false;
        MissionManager missoes = MissionManager.Instance;
        bool entradaConfirmada = false;
        Vector3? posicaoAnterior = null;
        try
        {
            MostrarStatus("Preparing scenario…");
            await missoes.PrepararCatalogoAsync(cenario);
            if (this == null || selecaoInterrompida || !isActiveAndEnabled || !CheckpointJogo.PodeAvancar
                || !sessao.Corresponde(visita))
                return;

            if (player == null || destino == null || modalFinalManager == null
                || modalSelecaoCenario == null || MissionManager.Instance != missoes)
                throw new InvalidOperationException("Uma referência da entrada deixou de estar disponível.");

            posicaoAnterior = player.position;
            TransportarJogador(destino.position);
            if (!modalFinalManager.MostrarInicioCenario(cenario))
                throw new InvalidOperationException("A introdução do cenário não está ativa na hierarquia.");

            CheckpointJogo.Instance?.Entrada(cenario);
            entradaConfirmada = true;
            modalSelecaoCenario.SetActive(false);
        }
        catch (Exception erro)
        {
            Debug.LogException(erro, this);
        }
        finally
        {
            preparando = false;
            if (!entradaConfirmada && sessao.Corresponde(visita))
            {
                sessao.Encerrar();
                if (missoes != null && MissionManager.Instance == missoes)
                    missoes.EncerrarCenario();
                if (this != null)
                {
                    if (modalFinalManager != null)
                        modalFinalManager.LimparVisita();
                    if (posicaoAnterior.HasValue && player != null)
                        TransportarJogador(posicaoAnterior.Value);
                    MostrarStatus("Could not open the scenario. Try again.");
                }
            }
        }
    }

    internal void PosicionarEntradaCheckpoint(string cenario)
    {
        Transform destino = ObterPontoDestino(cenario);
        if (player == null || destino == null)
            throw new InvalidOperationException("Entrada do cenário não está configurada.");
        TransportarJogador(destino.position);
    }

    internal async Task PrepararEntradaCheckpoint(string cenario, int visita, CancellationToken ct)
    {
        var missoes = MissionManager.Instance;
        if (missoes == null)
            throw new InvalidOperationException("Gerenciador de missões indisponível.");

        await missoes.PrepararCatalogoAsync(cenario);
        ct.ThrowIfCancellationRequested();
        if (this == null || !SessaoCenario.Atual.Corresponde(visita))
            throw new OperationCanceledException();

        if (modalSelecaoCenario != null)
            modalSelecaoCenario.SetActive(false);
    }

    internal Transform ObterPontoDestino(string cenario)
    {
        switch (cenario)
        {
            case "Home":
                return pontoHome;
            case "Market":
                return pontoMarket;
            case "College":
                return pontoCollege;
            case "Hotel":
                return pontoHotel;
            case "Airport":
                return pontoAirport;
            case "Restaurant":
                return pontoRestaurant;
            case MissoesVisitorCenter.Cenario:
                return pontoInfoCenter;
            case MissoesClothingStore.Cenario:
                return pontoClothingStore;
            default:
                Debug.LogWarning("Cenário sem ponto de destino configurado: " + cenario);
                return null;
        }
    }

    private void TransportarJogador(Vector3 posicaoDestino)
    {
        CharacterController characterController =
            player.GetComponent<CharacterController>();

        bool controladorEstavaAtivo =
            characterController != null && characterController.enabled;

        // Evita que o CharacterController bloqueie ou corrija
        // o reposicionamento instantâneo do jogador.
        if (controladorEstavaAtivo)
            characterController.enabled = false;

        player.position = posicaoDestino;

        if (controladorEstavaAtivo)
            characterController.enabled = true;

        if (ModoVR)
        {
            Debug.Log("Jogador transportado no modo VR: " + posicaoDestino);
        }
        else
        {
            Debug.Log("Jogador transportado no modo desktop: " + posicaoDestino);
        }
    }
}
