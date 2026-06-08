using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

public class ProdutoColetaManager : MonoBehaviour
{
    public static ProdutoColetaManager Instance;

    public List<ProdutoInterativo> leites;
    public List<ProdutoInterativo> frutas;
    public List<ProdutoInterativo> cereais;
    public List<ProdutoInterativo> salgadinhos;
    public List<ProdutoInterativo> legumes;
    public List<ProdutoInterativo> refrigerantes;

    public GameObject painelAviso;
    public TextMeshProUGUI textoAviso;
    public TextMeshProUGUI textoTraducao;

    private string nivelAtual;

    private bool pegouLeite;
    private bool pegouFruta;
    private bool pegouCereal;
    private bool pegouSalgadinho;
    private bool pegouLegume;
    private bool pegouRefrigerante;

    private int qtdFrutas = 0;
    private int qtdLegumes = 0;

    void Awake()
    {
        Instance = this;
        DesativarTodos();
    }

    public void LiberarProdutos(string nivel)
    {
        nivelAtual = nivel;

        DesativarTodos();

        if (nivel == "A1")
        {
            AtivarGrupo(leites);
            AtivarGrupo(frutas);
        }
        else if (nivel == "A2")
        {
            AtivarGrupo(leites);
            AtivarGrupo(cereais);
            AtivarGrupo(salgadinhos);
        }
        else
        {
            AtivarGrupo(legumes);
            AtivarGrupo(refrigerantes);
        }
    }

    public void RegistrarProduto(ProdutoInterativo produto)
    {
        if (produto == null || !produto.podePegar) return;

        switch (produto.tipo)
        {
            case TipoProdutoMercado.Milk:
                pegouLeite = true;
                DesativarGrupo(leites);
                break;

            case TipoProdutoMercado.Fruit:
                pegouFruta = true;
                qtdFrutas++;
                if (qtdFrutas >= 2) DesativarGrupo(frutas);
                break;

            case TipoProdutoMercado.Cereal:
                pegouCereal = true;
                DesativarGrupo(cereais);
                break;

            case TipoProdutoMercado.Snack:
                pegouSalgadinho = true;
                DesativarGrupo(salgadinhos);
                break;

            case TipoProdutoMercado.Vegetable:
                pegouLegume = true;
                qtdLegumes++;
                if (qtdLegumes >= 2) DesativarGrupo(legumes);
                break;

            case TipoProdutoMercado.Soda:
                pegouRefrigerante = true;
                DesativarGrupo(refrigerantes);
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

    public string ProdutosFaltandoEN()
    {
        if (nivelAtual == "A1")
        {
            if (!pegouLeite) return "You still need to get milk.";
            if (!pegouFruta) return "You still need to get fruit.";
        }
        else if (nivelAtual == "A2")
        {
            if (!pegouLeite) return "You still need to get milk.";
            if (!pegouCereal) return "You still need to get cereal.";
            if (!pegouSalgadinho) return "You still need to get snacks.";
        }
        else
        {
            if (!pegouLegume) return "You still need to get vegetables.";
            if (!pegouRefrigerante) return "You still need to get soda.";
        }

        return "";
    }

    public string ProdutosFaltandoPT()
    {
        if (nivelAtual == "A1")
        {
            if (!pegouLeite) return "Você ainda precisa pegar o leite.";
            if (!pegouFruta) return "Você ainda precisa pegar uma fruta.";
        }
        else if (nivelAtual == "A2")
        {
            if (!pegouLeite) return "Você ainda precisa pegar o leite.";
            if (!pegouCereal) return "Você ainda precisa pegar o cereal.";
            if (!pegouSalgadinho) return "Você ainda precisa pegar o salgadinho.";
        }
        else
        {
            if (!pegouLegume) return "Você ainda precisa pegar os legumes.";
            if (!pegouRefrigerante) return "Você ainda precisa pegar o refrigerante.";
        }

        return "";
    }
    public async void MostrarAvisoProdutosFaltando()
    {
        if (painelAviso == null) return;

        textoAviso.text = ProdutosFaltandoEN();
        textoTraducao.text = ProdutosFaltandoPT();

        painelAviso.SetActive(true);

        await Task.Delay(5000);

        painelAviso.SetActive(false);
    }
    private void AtivarGrupo(List<ProdutoInterativo> grupo)
    {
        foreach (ProdutoInterativo p in grupo)
        {
            if (p != null)
                p.DefinirInterativo(true);
        }
    }

    private void DesativarGrupo(List<ProdutoInterativo> grupo)
    {
        foreach (ProdutoInterativo p in grupo)
        {
            if (p != null)
                p.DefinirInterativo(false);
        }
    }

    private void DesativarTodos()
    {
        AtivarOuDesativarTudo(false);
    }

    private void AtivarOuDesativarTudo(bool ativo)
    {
        foreach (var p in leites) if (p != null) p.DefinirInterativo(ativo);
        foreach (var p in frutas) if (p != null) p.DefinirInterativo(ativo);
        foreach (var p in cereais) if (p != null) p.DefinirInterativo(ativo);
        foreach (var p in salgadinhos) if (p != null) p.DefinirInterativo(ativo);
        foreach (var p in legumes) if (p != null) p.DefinirInterativo(ativo);
        foreach (var p in refrigerantes) if (p != null) p.DefinirInterativo(ativo);
    }
}