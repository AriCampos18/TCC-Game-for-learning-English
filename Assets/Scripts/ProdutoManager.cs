using System.Collections.Generic;
using UnityEngine;

public class ProdutoManager : MonoBehaviour
{
    public static ProdutoManager Instance;

    [Header("Grupos de produtos")]
    public Transform grupoLeites;
    public Transform grupoFrutas;
    public Transform grupoCereais;
    public Transform grupoSalgadinhos;
    public Transform grupoLegumes;
    public Transform grupoRefrigerantes;

    [Header("Prefabs para aparecer no caixa")]
    public GameObject prefabLeite;
    public GameObject prefabFruta;
    public GameObject prefabCereal;
    public GameObject prefabSalgadinho;
    public GameObject prefabLegume;
    public GameObject prefabRefrigerante;
    private readonly HashSet<ProdutoInterativo> produtosOcultados = new HashSet<ProdutoInterativo>();

    private string nivelAtual;
    private bool marketIntermediario;

    private List<TipoProdutoMercado> produtosPegos = new List<TipoProdutoMercado>();

    void Awake()
    {
        Instance = this;
        DesativarTodos();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public List<TipoProdutoMercado> GetProdutosPegos()
    {
        return new List<TipoProdutoMercado>(produtosPegos);
    }

    public GameObject GetPrefabProduto(TipoProdutoMercado tipo)
    {
        switch (tipo)
        {
            case TipoProdutoMercado.Milk:
                return prefabLeite;

            case TipoProdutoMercado.Fruit:
                return prefabFruta;

            case TipoProdutoMercado.Cereal:
                return prefabCereal;

            case TipoProdutoMercado.Snack:
                return prefabSalgadinho;

            case TipoProdutoMercado.Vegetable:
                return prefabLegume;

            case TipoProdutoMercado.Soda:
                return prefabRefrigerante;
        }

        return null;
    }

    internal void ReiniciarMarket()
    {
        produtosPegos.Clear();
        nivelAtual = null;
        foreach (var produto in produtosOcultados)
            if (produto != null)
                produto.gameObject.SetActive(true);
        produtosOcultados.Clear();
        DesativarTodos();
    }

    public void LiberarProdutos(string nivel)
    {
        if (!SessaoCenario.Atual.Permite("Market"))
            return;
        marketIntermediario = MissionManager.NivelMarketIntermediario(nivel);
        nivelAtual = marketIntermediario ? "A2" : nivel;
        produtosPegos.Clear();

        DesativarTodos();

        if (nivelAtual == "A1")
        {
            AtivarGrupo(grupoLeites);
            AtivarGrupo(grupoFrutas);
        }
        else if (nivelAtual == "A2")
        {
            AtivarGrupo(grupoLeites);
            AtivarGrupo(grupoCereais);
            AtivarGrupo(grupoSalgadinhos);
        }
        else
        {
            AtivarGrupo(grupoLegumes);
            AtivarGrupo(grupoRefrigerantes);
        }
    }

    public void RegistrarProduto(ProdutoInterativo produto)
    {
        if (!SessaoCenario.Atual.Permite("Market") || produto == null || !produto.podePegar) return;

        if (produto.gameObject.activeSelf)
            produtosOcultados.Add(produto);

        if (marketIntermediario)
        {
            produto.DefinirInterativo(false);
            produto.gameObject.SetActive(false);
        }

        if (!produtosPegos.Contains(produto.tipo))
            produtosPegos.Add(produto.tipo);

        switch (produto.tipo)
        {
            case TipoProdutoMercado.Milk:
                DesativarGrupo(grupoLeites);
                break;

            case TipoProdutoMercado.Fruit:
                DesativarGrupo(grupoFrutas);
                break;

            case TipoProdutoMercado.Cereal:
                DesativarGrupo(grupoCereais);
                break;

            case TipoProdutoMercado.Snack:
                DesativarGrupo(grupoSalgadinhos);
                break;

            case TipoProdutoMercado.Vegetable:
                DesativarGrupo(grupoLegumes);
                break;

            case TipoProdutoMercado.Soda:
                DesativarGrupo(grupoRefrigerantes);
                break;
        }

        if (MissaoProdutosConcluida())
        {
            MissionManager.Instance.ConcluirMissaoMarket("interagir_produtos", "marco_fluxo", "produtos_obrigatorios_coletados");
            MissionManager.Instance.TentarLiberarCaixaMarket();
        }
    }

    public bool MissaoProdutosConcluida()
    {
        if (nivelAtual == "A1")
            return produtosPegos.Contains(TipoProdutoMercado.Milk)
                && produtosPegos.Contains(TipoProdutoMercado.Fruit);

        if (nivelAtual == "A2")
            return produtosPegos.Contains(TipoProdutoMercado.Milk)
                && produtosPegos.Contains(TipoProdutoMercado.Cereal)
                && produtosPegos.Contains(TipoProdutoMercado.Snack);

        return produtosPegos.Contains(TipoProdutoMercado.Vegetable)
            && produtosPegos.Contains(TipoProdutoMercado.Soda);
    }

    private void AtivarGrupo(Transform grupo)
    {
        if (grupo == null) return;

        ProdutoInterativo[] produtos = grupo.GetComponentsInChildren<ProdutoInterativo>(true);

        foreach (ProdutoInterativo p in produtos)
        {
            if (p != null)
                p.DefinirInterativo(true);
        }
    }

    private void DesativarGrupo(Transform grupo)
    {
        if (grupo == null) return;

        ProdutoInterativo[] produtos = grupo.GetComponentsInChildren<ProdutoInterativo>(true);

        foreach (ProdutoInterativo p in produtos)
        {
            if (p != null)
                p.DefinirInterativo(false);
        }
    }

    private void DesativarTodos()
    {
        DesativarGrupo(grupoLeites);
        DesativarGrupo(grupoFrutas);
        DesativarGrupo(grupoCereais);
        DesativarGrupo(grupoSalgadinhos);
        DesativarGrupo(grupoLegumes);
        DesativarGrupo(grupoRefrigerantes);
    }

    public bool ProdutosObrigatoriosPegos()
    {
        return MissaoProdutosConcluida();
    }
}