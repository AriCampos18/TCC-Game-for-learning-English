using UnityEngine;

public class SacolaInterativaScript : MonoBehaviour
{
    private CashierNPC caixa;

    public void Configurar(CashierNPC cashier)
    {
        caixa = cashier;
    }

    internal bool PodeRecolher => SessaoCenario.Atual.Permite("Market") && caixa != null
        && caixa.MissaoSacolaLiberada() && !caixa.JogadorPegouSacola();

    public void PegarSacola()
    {
        if (!PodeRecolher)
            return;

        caixa.SacolaFoiPega();

        gameObject.SetActive(false);
    }
}