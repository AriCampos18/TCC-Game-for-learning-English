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
    private CatalogoMissoes catalogoMissoes;

    private string cenarioCatalogo;

    public bool UsaCatalogoAirport => cenarioCatalogo == "Airport";
    public bool UsaCatalogoMarket => cenarioCatalogo == "Market";

    public async Task PrepararCatalogoAsync(string cenario)
    {
        string nivel = DadosJogador.nivelUsuario;
        CatalogoMissoes novoCatalogo = null;
        try
        {
            string url = AuthenticationUIController.Instance?.BackendBaseUrl;
            if (string.IsNullOrWhiteSpace(url))
                throw new InvalidOperationException("URL do backend indisponível.");
            string body = await catalogClient.GetStringAsync(url.TrimEnd('/')
                + "/api/missions?cenario=" + Uri.EscapeDataString(cenario) + "&nivel=" + Uri.EscapeDataString(nivel));
            var resposta = JsonConvert.DeserializeObject<CatalogoMissoes.Resposta>(body);
            novoCatalogo = new CatalogoMissoes(resposta?.data, nivel, cenario);
        }
        catch (Exception e) when (e is HttpRequestException || e is TaskCanceledException
            || e is JsonException || e is InvalidOperationException || e is UriFormatException)
        {
            Debug.LogWarning(cenario + ": usando missões locais nesta visita. " + e.Message);
        }

        if (novoCatalogo == null)
        {
            ConfigurarCenario(cenario);
            return;
        }

        LimparMissoes();
        catalogoMissoes = novoCatalogo;
        cenarioCatalogo = cenario;
        AvaliarEvento("cenario_iniciado", cenario);
    }

    public void RegistrarEvento(string tipo, string alvo)
    {
        if (UsaCatalogoAirport)
            AvaliarEvento(tipo, alvo);
    }

    public void RegistrarEventoMarket(string tipo, string alvo)
    {
        if (UsaCatalogoMarket)
            AvaliarEvento(tipo, alvo);
    }

    public void IniciarMissaoMarket(string id, string texto, string traducao)
    {
        if (!UsaCatalogoMarket)
        {
            AdicionarMissao(id, texto, traducao);
            return;
        }

        switch (id)
        {
            case "seguir_atendente": RegistrarEventoMarket("marco_fluxo", "rota_prateleiras_iniciada"); break;
            case "interagir_produtos": RegistrarEventoMarket("marco_fluxo", "produtos_liberados"); break;
            case "pegar_sacola": RegistrarEventoMarket("marco_fluxo", "sacola_liberada"); break;
        }
    }

    public void ConcluirMissaoMarket(string id, string tipo, string alvo)
    {
        if (UsaCatalogoMarket)
            RegistrarEventoMarket(tipo, alvo);
        else
            ConcluirMissao(id);
    }

    private void AvaliarEvento(string tipo, string alvo)
    {
        catalogoMissoes?.Registrar(tipo, alvo, AdicionarDoCatalogo, ConcluirMissao);
    }

    private void AdicionarDoCatalogo(CatalogoMissoes.Definicao definicao)
    {
        AdicionarMissao(definicao.mission_key, definicao.texto, definicao.traducao);
        // Uma etapa pode começar mais tarde que outra com ordem maior.
        int posicao = catalogoMissoes.Definicoes.Count(d =>
            d.ordem < definicao.ordem && missoes.Exists(m => m.id == d.mission_key));
        Missao missao = missoes.Find(m => m.id == definicao.mission_key);
        if (missao?.itemUI != null)
            missao.itemUI.transform.SetSiblingIndex(posicao);
    }

    // Chamado quando o jogador seleciona um cenário.
    public void ConfigurarCenario(string cenario)
    {
        LimparMissoes();
        catalogoMissoes = null;
        cenarioCatalogo = null;

        if (cenario == "Airport")
        {
            switch (DadosJogador.nivelUsuario)
            {
                case "A1":
                    AdicionarMissao("aeroporto_a1_1", "Arrive at the airport and find the information desk", "Chegue ao aeroporto e procure o balcão de informações");
                    break;
                case "A1-A2":
                    AdicionarMissao("aeroporto_a1a2_1", "Arrive at the airport and find the information desk", "Chegue ao aeroporto e procure o balcão de informações");
                    break;
                case "A2":
                    AdicionarMissao("aeroporto_a2_1", "Arrive at the airport and find the check-in desk", "Chegue ao aeroporto e procure o balcão de check-in");
                    break;
                case "A2-B1":
                    AdicionarMissao("aeroporto_a2b1_1", "Go to the information desk and explain that you need to find the check-in desks", "Dirija-se ao balcão de informações e explique que precisa encontrar o check-in");
                    break;
                case "B1":
                    AdicionarMissao("aeroporto_b1_1", "Arrive at the airport and find the information desk", "Chegue ao aeroporto e procure o balcão de informações");
                    break;
            }
        }

        // A missão inicial da cesta existe somente no Market.
        if (cenario == "Market" && NivelMarketIntermediario(DadosJogador.nivelUsuario))
        {
            bool iniciante = DadosJogador.nivelUsuario == "A1-A2";
            AdicionarMissao("pegar_cesta",
                iniciante ? "Enter the market and pick up a shopping basket."
                    : "Enter the market and collect a basket for your shopping.",
                iniciante ? "Entre no mercado e pegue uma cesta de compras."
                    : "Entre no mercado e pegue uma cesta para suas compras.");
            return;
        }

        if (cenario == "Market")
        {
            AdicionarMissao(
                "pegar_cesta",
                "Enter the market and get a shopping basket",
                "Entre no mercado e pegue uma cesta de compras"
            );
        }
    }

    public static bool NivelMarketIntermediario(string nivel)
    {
        return nivel == "A1-A2" || nivel == "A2-B1";
    }

    public void RegistrarCestaMarketIntermediario()
    {
        if (!MissaoAtiva("pegar_cesta"))
            return;

        ConcluirMissaoMarket("pegar_cesta", "marco_fluxo", "cesta_coletada");
        if (UsaCatalogoMarket)
            return;
        if (DadosJogador.nivelUsuario == "A1-A2")
        {
            AdicionarMissao("falar_padaria", "Talk to the bakery attendant.",
                "Fale com o atendente da padaria.");
            AdicionarMissao("falar_atendente", "Ask the market attendant for help.",
                "Peça ajuda ao atendente do mercado.");
        }
        else
        {
            AdicionarMissao("falar_atendente", "Ask the market attendant for help finding the products.",
                "Peça ao atendente do mercado ajuda para encontrar os produtos.");
            AdicionarMissao("falar_padaria", "Speak with the bakery attendant.",
                "Converse com o atendente da padaria.");
        }
    }

    public void TentarLiberarCaixaMarket()
    {
        if (UsaCatalogoMarket || !MissaoConcluida("falar_padaria") || !MissaoConcluida("falar_atendente")
            || !MissaoConcluida("interagir_produtos") || GameProgress.Instance?.PodeFalarComCaixa() != true)
            return;

        if (!NivelMarketIntermediario(DadosJogador.nivelUsuario))
        {
            AdicionarMissao("falar_caixa", "Put the products on the counter to start the conversation with the cashier.",
                "Coloque os produtos no balcão do caixa para iniciar a conversa com o caixa.");
            return;
        }

        bool iniciante = DadosJogador.nivelUsuario == "A1-A2";
        AdicionarMissao("falar_caixa",
            iniciante ? "Place your products on the checkout counter and talk to the cashier."
                : "Take your selected products to the checkout counter and speak with the cashier.",
            iniciante ? "Coloque seus produtos no balcão do caixa e fale com o caixa."
                : "Leve os produtos escolhidos ao balcão do caixa e fale com o caixa.");
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