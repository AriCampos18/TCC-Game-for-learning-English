using UnityEngine;

public class SacolaInterativaScript : MonoBehaviour
{
    private CashierNPC caixa;

    public void Configurar(CashierNPC cashier)
    {
        caixa = cashier;
    }

    public void PegarSacola()
    {
        if (caixa != null)
            caixa.SacolaFoiPega();

        gameObject.SetActive(false);
    }
}