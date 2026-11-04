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
//         else if(cenario == "Tourist Information Center")
//             player.position = pontoInfoCenter.position;
        
//         // Registra somente quando a escolha é válida e o destino está configurado.
//         gerenciador.RegistrarCenarioEscolhido(cenario);
//         modalFinalManager.MostrarInicioCenario(cenario);

//         modalSelecaoCenario.SetActive(false);
//     }
// }

using UnityEngine;
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

    private CardCenarioManager[] cardsCenarios;

    [Header("Modais")]
    public ModalFinalManager modalFinalManager;
    public GameObject modalSelecaoCenario;

    private bool ModoVR => XRSettings.isDeviceActive;

    void Awake()
    {
        cardsCenarios = GetComponentsInChildren<CardCenarioManager>(true);
    }

    private void OnEnable()
    {
        if (GerenciadorProgressao.Instance == null)
        {
            Debug.LogWarning(
                "GerenciadorProgressao ainda não está disponível. " +
                "Os cards serão atualizados quando o modal for aberto novamente."
            );

            return;
        }

        AtualizarCards();
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

    public void SelecionarCenario(string cenario)
    {
        GerenciadorProgressao gerenciador = GerenciadorProgressao.Instance;

        if (gerenciador == null)
        {
            Debug.LogError("GerenciadorProgressao não encontrado.");
            return;
        }

        // Valida novamente antes de iniciar o cenário.
        if (!gerenciador.PodeSelecionarCenario(cenario))
        {
            Debug.LogWarning("Não é permitido escolher este cenário: " + cenario);
            AtualizarCards();
            return;
        }

        if (player == null)
        {
            Debug.LogError("O Transform do jogador não foi associado.");
            return;
        }

        Transform destino = ObterPontoDestino(cenario);

        if (destino == null)
        {
            Debug.LogError(
                "O ponto de destino do cenário '" + cenario +
                "' não foi associado ou o cenário não possui um destino configurado."
            );
            return;
        }

        // No VR, player deve ser a raiz do XR Origin.
        // Assim, o rig inteiro é deslocado e a posição local da câmera
        // (altura e deslocamento do headset) é preservada.
        TransportarJogador(destino.position);

        // Registra somente depois de validar e transportar para o destino.
        gerenciador.RegistrarCenarioEscolhido(cenario);

        if (modalFinalManager == null)
        {
            Debug.LogError("ModalFinalManager não foi associado.");
            return;
        }

        modalFinalManager.MostrarInicioCenario(cenario);

        if (modalSelecaoCenario != null)
            modalSelecaoCenario.SetActive(false);
        else
            Debug.LogWarning("O objeto modalSelecaoCenario não foi associado.");
    }

    private Transform ObterPontoDestino(string cenario)
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
            case "Tourist Information Center":
                return pontoInfoCenter;
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
