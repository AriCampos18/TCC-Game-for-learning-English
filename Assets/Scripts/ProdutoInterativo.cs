using UnityEngine;

public enum TipoProdutoMercado
{
    Milk,
    Fruit,
    Cereal,
    Snack,
    Vegetable,
    Soda
}

public class ProdutoInterativo : MonoBehaviour
{
    public TipoProdutoMercado tipo;
    public bool podePegar = false;

    private Collider col;
    private int layerOriginal;

    void Awake()
    {
        col = GetComponent<Collider>();
        layerOriginal = gameObject.layer;
        DefinirInterativo(false);
    }

    public void DefinirInterativo(bool ativo)
    {
        podePegar = ativo;

        if (col != null)
            col.enabled = ativo;

        gameObject.layer = ativo ? LayerMask.NameToLayer("Interativo") : LayerMask.NameToLayer("Default");
    }
}