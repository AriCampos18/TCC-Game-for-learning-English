using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance;

    public Transform containerMissoes;
    public GameObject prefabMissao;

    private List<Missao> missoes = new List<Missao>();

    void Awake()
    {
        Instance = this;
    }

    private static readonly HttpClient catalogClient = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
    private CatalogoMissoes catalogoAirport;

    public bool UsaCatalogoAirport => catalogoAirport != null;

    public async Task PrepararAirportAsync()
    {
        string nivel = DadosJogador.nivelUsuario;
        CatalogoMissoes novoCatalogo = null;
        try
        {
            string url = AuthenticationUIController.Instance?.BackendBaseUrl;
            if (string.IsNullOrWhiteSpace(url))
                throw new InvalidOperationException("URL do backend indisponível.");
            string body = await catalogClient.GetStringAsync(url.TrimEnd('/')
                + "/api/missions?cenario=Airport&nivel=" + Uri.EscapeDataString(nivel));
            var resposta = JsonConvert.DeserializeObject<CatalogoMissoes.Resposta>(body);
            novoCatalogo = new CatalogoMissoes(resposta?.data, nivel);
        }
        catch (Exception e) when (e is HttpRequestException || e is TaskCanceledException
            || e is JsonException || e is InvalidOperationException || e is UriFormatException)
        {
            Debug.LogWarning("Airport: usando missões locais nesta visita. " + e.Message);
        }

        if (novoCatalogo == null)
        {
            ConfigurarCenario("Airport");
            return;
        }

        LimparMissoes();
        catalogoAirport = novoCatalogo;
        RegistrarEvento("cenario_iniciado", "Airport");
    }

    public void RegistrarEvento(string tipo, string alvo)
    {
        catalogoAirport?.Registrar(tipo, alvo, AdicionarDoCatalogo, ConcluirMissao);
    }

    private void AdicionarDoCatalogo(CatalogoMissoes.Definicao definicao)
    {
        AdicionarMissao(definicao.mission_key, definicao.texto, definicao.traducao);
        // Uma etapa pode começar mais tarde que outra com ordem maior.
        int posicao = catalogoAirport.Definicoes.Count(d =>
            d.ordem < definicao.ordem && missoes.Exists(m => m.id == d.mission_key));
        Missao missao = missoes.Find(m => m.id == definicao.mission_key);
        if (missao?.itemUI != null)
            missao.itemUI.transform.SetSiblingIndex(posicao);
    }

    // Chamado quando o jogador seleciona um cenário.
    public void ConfigurarCenario(string cenario)
    {
        LimparMissoes();
        catalogoAirport = null;

        if (cenario == "Airport")
        {
            switch (DadosJogador.nivelUsuario)
            {
                case "A1":
                    AdicionarMissao("aeroporto_a1_1", "Chegue ao aeroporto e procure o balcão de informações", "Arrive at the airport and find the information desk");
                    break;
                case "A1-A2":
                    AdicionarMissao("aeroporto_a1a2_1", "Chegue ao aeroporto e procure o balcão de informações", "Arrive at the airport and find the information desk");
                    break;
                case "A2":
                    AdicionarMissao("aeroporto_a2_1", "Chegue ao aeroporto e procure o balcão de check-in", "Arrive at the airport and find the check-in desk");
                    break;
                case "A2-B1":
                    AdicionarMissao("aeroporto_a2b1_1", "Dirija-se ao balcão de informações e explique que precisa encontrar o check-in", "Go to the information desk and explain that you need to find the check-in desks");
                    break;
                case "B1":
                    AdicionarMissao("aeroporto_b1_1", "Chegue ao aeroporto e procure o balcão de informações", "Arrive at the airport and find the information desk");
                    break;
            }
        }

        // A missão inicial da cesta existe somente no Market.
        if (cenario == "Market")
        {
            AdicionarMissao(
                "pegar_cesta",
                "Enter the market and get a shopping basket",
                "Entre no mercado e pegue uma cesta de compras"
            );
        }
    }

    private void LimparMissoes()
    {
        foreach (Missao missao in missoes)
        {
            if (missao.itemUI != null)
                Destroy(missao.itemUI.gameObject);
        }

        missoes.Clear();
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

        GameObject obj = Instantiate(
            prefabMissao,
            containerMissoes
        );

        obj.SetActive(true);

        MissaoItemUI item = obj.GetComponent<MissaoItemUI>();

        item.Configurar(nova);

        nova.itemUI = item;

        missoes.Add(nova);
    }

    public static void AdicionarMissaoNPC(
        string id,
        string texto,
        string traducao
    )
    {
        if (Instance == null)
        {
            Debug.LogError(
                "MissionManager não está disponível para adicionar uma missão de NPC."
            );
            return;
        }

        Instance.AdicionarMissao(id, texto, traducao);
    }

    public void ConcluirMissao(string id)
    {
        Missao m = missoes.Find(x => x.id == id);

        if (m != null && !m.concluida)
        {
            m.concluida = true;

            if (m.itemUI != null)
                m.itemUI.AtualizarVisual();
        }
    }

    public static void ConcluirMissaoNPC(string id)
    {
        if (Instance == null)
        {
            Debug.LogError(
                "MissionManager não está disponível para concluir uma missão de NPC."
            );
            return;
        }

        Instance.ConcluirMissao(id);
    }

    public void RemoverMissao(string id)
    {
        Missao m = missoes.Find(x => x.id == id);

        if (m != null)
        {
            if (m.itemUI != null)
                Destroy(m.itemUI.gameObject);

            missoes.Remove(m);
        }
    }

    public bool MissaoAtiva(string id)
    {
        return missoes.Exists(m => m.id == id && !m.concluida);
    }

    public bool MissaoConcluida(string id)
    {
        Missao m = missoes.Find(x => x.id == id);

        return m != null && m.concluida;
    }
}