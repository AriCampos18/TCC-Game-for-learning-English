using UnityEngine;

public class GameProgress : MonoBehaviour
{
    public static GameProgress Instance;

    public bool falouAtendente = false;
    public bool falouPadaria = false;

    public bool seguindoAtendente = false;

    public static bool EstaEmDialogo { get; set; } = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
            EstaEmDialogo = false;
        }
    }

    internal void ReiniciarMarket()
    {
        falouAtendente = falouPadaria = seguindoAtendente = false;
        ProdutoManager.Instance?.ReiniciarMarket();
        foreach (var atendente in FindObjectsByType<ShelfNPC>(FindObjectsSortMode.None))
            atendente.AtualizarVisita();
        foreach (var caixa in FindObjectsByType<CashierNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            caixa.ReiniciarMarket();
        foreach (var porta in FindObjectsByType<PortaAutomatica>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            porta.ReiniciarMarket();
        foreach (var raio in FindObjectsByType<RaioCentralInteracao>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            raio.RestaurarCestas();
    }

    public bool PodeFalarComCaixa()
    {
        return SessaoCenario.Atual.Permite("Market") && falouAtendente && falouPadaria;
    }
}