using UnityEngine;

public class ModalSelecaoCenario : MonoBehaviour
{
    public Transform player;
    public Transform pontoHome;
    public Transform pontoMarket;
    public Transform pontoCollege;
    public Transform pontoHotel;
    public Transform pontoAirport;
    public Transform pontoRestaurant;
    public Transform pontoInfoCenter;

    private CardCenarioManager[] cardsCenarios;

    public ModalFinalManager modalFinalManager;

    void Awake()
    {
        cardsCenarios = GetComponentsInChildren<CardCenarioManager>(true);
    }

    private void OnEnable()
    {
        AtualizarCards();
    }

    // Mostra somente os cenários permitidos.
    public void AtualizarCards()
    {
        if (cardsCenarios == null)
        {
            cardsCenarios =
                GetComponentsInChildren<CardCenarioManager>(true);
        }

        Debug.Log(
            "MODAL EXECUTOU. INSTANCE: " +
            (GerenciadorProgressao.Instance != null
                ? GerenciadorProgressao.Instance.gameObject.name
                : "NULL")
        );
        GerenciadorProgressao gerenciador =
            GerenciadorProgressao.Instance;

        if (gerenciador == null)
        {
            Debug.LogError(
                "GerenciadorProgressao não encontrado."
            );
            return;
        }

        foreach (CardCenarioManager card in cardsCenarios)
        {
            if (card == null)
                continue;

            bool permitido =
                gerenciador.PodeSelecionarCenario(
                    card.nomeCenario
                );

            card.gameObject.SetActive(permitido);
        }
    }

    public void SelecionarCenario(string cenario)
    {
        GerenciadorProgressao gerenciador =
            GerenciadorProgressao.Instance;

        if (gerenciador == null)
        {
            Debug.LogError(
                "GerenciadorProgressao não encontrado."
            );
            return;
        }

        // Valida novamente antes de iniciar o cenário.
        if (!gerenciador.PodeSelecionarCenario(cenario))
        {
            Debug.LogWarning(
                "Não é permitido escolher este cenário: " +
                cenario
            );

            AtualizarCards();
            return;
        }

        if (player == null)
        {
            Debug.LogError(
                "O Transform do jogador não foi associado."
            );
            return;
        }

        //logica para transportar o usuário para o cenario selecionado
        if(cenario == "Home")
            player.position = pontoHome.position;
        else if(cenario == "Market")
            player.position = pontoMarket.position;
        else if(cenario == "College")
            player.position = pontoCollege.position;
        else if(cenario == "Hotel")
            player.position = pontoHotel.position;
         else if(cenario == "Airport")
            player.position = pontoAirport.position;
        else if(cenario == "Restaurant")
            player.position = pontoRestaurant.position;
        else if(cenario == "Tourist Information Center")
            player.position = pontoInfoCenter.position;
        
        // Registra somente quando a escolha é válida e o destino está configurado.
        gerenciador.RegistrarCenarioEscolhido(cenario);
        modalFinalManager.MostrarInicioCenario(cenario);
    }
}
