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

    private string nivelAtual;

    private bool pegouLeite;
    private bool pegouFruta;
    private bool pegouCereal;
    private bool pegouSalgadinho;
    private bool pegouLegume;
    private bool pegouRefrigerante;

    void Awake()
    {
        Instance = this;
        DesativarTodos();
    }

    public void LiberarProdutos(string nivel)
    {
        nivelAtual = nivel;

        pegouLeite = false;
        pegouFruta = false;
        pegouCereal = false;
        pegouSalgadinho = false;
        pegouLegume = false;
        pegouRefrigerante = false;

        DesativarTodos();

        if (nivel == "A1")
        {
            AtivarGrupo(grupoLeites);
            AtivarGrupo(grupoFrutas);
        }
        else if (nivel == "A2")
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
        if (produto == null || !produto.podePegar) return;

        switch (produto.tipo)
        {
            case TipoProdutoMercado.Milk:
                pegouLeite = true;
                DesativarGrupo(grupoLeites);
                break;

            case TipoProdutoMercado.Fruit:
                pegouFruta = true;
                DesativarGrupo(grupoFrutas);
                break;

            case TipoProdutoMercado.Cereal:
                pegouCereal = true;
                DesativarGrupo(grupoCereais);
                break;

            case TipoProdutoMercado.Snack:
                pegouSalgadinho = true;
                DesativarGrupo(grupoSalgadinhos);
                break;

            case TipoProdutoMercado.Vegetable:
                pegouLegume = true;
                DesativarGrupo(grupoLegumes);
                break;

            case TipoProdutoMercado.Soda:
                pegouRefrigerante = true;
                DesativarGrupo(grupoRefrigerantes);
                break;
        }

        if (MissaoProdutosConcluida())
        {
            MissionManager.Instance.ConcluirMissao("interagir_produtos");
        }
    }

    public bool MissaoProdutosConcluida()
    {
        if (nivelAtual == "A1")
            return pegouLeite && pegouFruta;

        if (nivelAtual == "A2")
            return pegouLeite && pegouCereal && pegouSalgadinho;

        return pegouLegume && pegouRefrigerante;
    }

    private void AtivarGrupo(Transform grupo)
    {
        if (grupo == null) return;

        ProdutoInterativo[] produtos =
            grupo.GetComponentsInChildren<ProdutoInterativo>(true);

        foreach (ProdutoInterativo p in produtos)
        {
            if (p != null)
                p.DefinirInterativo(true);
        }
    }

    private void DesativarGrupo(Transform grupo)
    {
        if (grupo == null) return;

        ProdutoInterativo[] produtos =
            grupo.GetComponentsInChildren<ProdutoInterativo>(true);

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
}