using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class CartaoQuartoHotel : MonoBehaviour
{
    internal static CartaoQuartoHotel Disponivel { get; private set; }
    internal bool Recolhido { get; private set; }
    private int quadroDisponibilizacao;

    internal void Resetar()
    {
        Recolhido = false;
        gameObject.SetActive(false);
    }

    internal bool Disponibilizar()
    {
        if (Disponivel != null)
            return false;

        gameObject.SetActive(true);
        if (!isActiveAndEnabled || !GetComponentsInChildren<Collider>().Any(colisor => colisor.enabled))
        {
            gameObject.SetActive(false);
            return false;
        }

        quadroDisponibilizacao = Time.frameCount;
        Disponivel = this;
        return true;
    }

    internal bool TentarRecolher()
    {
        if (Disponivel != this || Recolhido || Time.frameCount <= quadroDisponibilizacao)
            return false;

        Recolhido = true;
        gameObject.SetActive(false);
        return true;
    }

    private void OnDisable()
    {
        if (Disponivel == this)
            Disponivel = null;
    }
}
