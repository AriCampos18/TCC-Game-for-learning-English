using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance;

    public Transform containerMissoes;
    public GameObject prefabMissao;

    private List<Missao> missoes = new List<Missao>();

    void Start()
    {
        AdicionarMissao(
            "pegar_cesta",
            "Enter the market and get a shopping basket",
            "Entre no mercado e pegue uma cesta de compras"
        );
    }

    void Awake()
    {
        Instance = this;
    }

    public void ResetarMissao(string id)
    {
        Missao m = missoes.Find(x => x.id == id);

        if (m != null)
        {
            m.concluida = false;

            if (m.itemUI != null)
                m.itemUI.AtualizarVisual();
        }
    }

    public void AdicionarMissao(string id, string texto, string traducao)
    {
        if (missoes.Exists(m => m.id == id))
            return;

        Missao nova = new Missao()
        {
            id = id,
            texto = texto,
            traducao = traducao,
            concluida = false
        };

        GameObject obj =
            Instantiate(
                prefabMissao,
                containerMissoes
            );
            
        obj.SetActive(true);

        MissaoItemUI item =
            obj.GetComponent<MissaoItemUI>();

        item.Configurar(nova);

        nova.itemUI = item;

        missoes.Add(nova);
    }

    public void ConcluirMissao(string id)
    {
        Missao m =
            missoes.Find(x => x.id == id);

        if (m != null)
        {
            m.concluida = true;

            if (m.itemUI != null)
                m.itemUI.AtualizarVisual();
        }
    }

    public void RemoverMissao(string id)
    {
        Missao m =
            missoes.Find(x => x.id == id);

        if (m != null)
        {
            if (m.itemUI != null)
                Destroy(m.itemUI.gameObject);

            missoes.Remove(m);
        }
    }

    public bool MissaoConcluida(string id)
    {
        Missao m = missoes.Find(x => x.id == id);

        return m != null && m.concluida;
    }
}