using System;
using System.Linq;
using Newtonsoft.Json.Linq;
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

    internal bool RestaurarMissoesHotel(JArray missoes, bool todasConcluidas) =>
        UsaCatalogoHotel && catalogoMissoes != null
        && catalogoMissoes.RestaurarMarco(missoes, todasConcluidas, AdicionarDoCatalogo, ConcluirMissao);

    internal bool RestaurarMissoesMarco(string cenario, JArray missoes, string missaoAtiva) =>
        TemCatalogo(cenario) && catalogoMissoes != null
        && catalogoMissoes.RestaurarMarco(missoes, missaoAtiva == null, AdicionarDoCatalogo, ConcluirMissao, missaoAtiva);

    internal bool InicioRegistradoCheckpoint(string tipo, string alvo) =>
        catalogoMissoes?.InicioRegistradoCheckpoint(tipo, alvo) == true;

    internal JArray CapturarIniciosCheckpoint() => catalogoMissoes?.CapturarIniciosCheckpoint() ?? new JArray();

    internal bool RestaurarMissoesRoteiro(string cenario, JArray missoes, JArray inicios) =>
        cenarioCatalogo == cenario && SessaoCenario.Atual.Nome == cenario && catalogoMissoes != null
        && catalogoMissoes.RestaurarRoteiro(missoes, inicios, AdicionarDoCatalogo,
            id => MarcarConcluida(id), LimparMissoes);

    internal bool PodeCapturarMissoes(string cenario) => TemCatalogo(cenario);

    internal JArray CapturarMissoesCheckpoint() => new JArray(missoes.Select(m => new JObject
    {
        ["mission_key"] = m.id,
        ["status"] = m.concluida ? "completed" : "active",
        ["satisfied_events"] = catalogoMissoes?.CapturarEventosCheckpoint(m.id) ?? new JArray()
    }));

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance != this)
            return;

        Instance = null;
        SessaoCenario.Atual.Encerrar();
    }

    internal void EncerrarCenario()
    {
        LimparMissoes();
        catalogoMissoes = null;
        cenarioCatalogo = null;
    }

    private static readonly HttpClient catalogClient = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
    private CatalogoMissoes catalogoMissoes;

    private string cenarioCatalogo;

    private int visitasVisitorCenter;
    internal bool UsaCatalogoVisitorCenter => TemCatalogo(MissoesVisitorCenter.Cenario);
    internal int VisitaVisitorCenter => UsaCatalogoVisitorCenter ? visitasVisitorCenter : 0;

    internal void RegistrarEventoVisitorCenter(string tipo, string alvo)
    {
        if (UsaCatalogoVisitorCenter)
            AvaliarEvento(tipo, alvo);
    }

    private int visitasRestaurant;
    internal bool UsaCatalogoRestaurant => TemCatalogo(MissoesRestaurant.Cenario);
    internal int VisitaRestaurant => UsaCatalogoRestaurant ? visitasRestaurant : 0;

    internal void RegistrarEventoRestaurant(string tipo, string alvo)
    {
        if (UsaCatalogoRestaurant)
            AvaliarEvento(tipo, alvo);
    }

    private int visitasHome;
    internal bool UsaCatalogoHome => TemCatalogo(MissoesHome.Cenario);
    internal int VisitaHome => UsaCatalogoHome ? visitasHome : 0;

    internal void RegistrarEventoHome(string tipo, string alvo)
    {
        if (UsaCatalogoHome)
            AvaliarEvento(tipo, alvo);
    }

    private int visitasHotel;
    internal bool UsaCatalogoHotel => TemCatalogo(MissoesHotel.Cenario);
    internal int VisitaHotel => UsaCatalogoHotel ? visitasHotel : 0;

    internal void RegistrarEventoHotel(string tipo, string alvo)
    {
        if (UsaCatalogoHotel)
            AvaliarEvento(tipo, alvo);
    }

    private int visitasCollege;
    internal bool UsaCatalogoCollege => TemCatalogo(MissoesCollege.Cenario);
    internal int VisitaCollege => UsaCatalogoCollege ? visitasCollege : 0;
    internal bool PodeAtenderSecretariaCollege => UsaCatalogoCollege && catalogoMissoes != null
        && MissaoConcluida(catalogoMissoes.Definicoes[0].mission_key);

    internal void RegistrarEventoCollege(string tipo, string alvo)
    {
        if (UsaCatalogoCollege)
            AvaliarEvento(tipo, alvo);
    }

    private int visitasClothingStore;
    internal bool UsaCatalogoClothingStore => TemCatalogo(MissoesClothingStore.Cenario);
    internal int VisitaClothingStore => UsaCatalogoClothingStore ? visitasClothingStore : 0;

    internal void RegistrarEventoClothingStore(string tipo, string alvo)
    {
        if (UsaCatalogoClothingStore)
            AvaliarEvento(tipo, alvo);
    }

    private bool TemCatalogo(string cenario) =>
        cenarioCatalogo == cenario && SessaoCenario.Atual.Permite(cenario);

    public bool UsaCatalogoAirport => TemCatalogo("Airport");
    public bool UsaCatalogoMarket => TemCatalogo("Market");

    public async Task PrepararCatalogoAsync(string cenario)
    {
        var sessao = SessaoCenario.Atual;
        int visita = sessao.Versao;
        if (sessao.Nome != cenario || sessao.EmJogo || sessao.EmResultado)
            return;

        EncerrarCenario();
        string nivel = sessao.Nivel;
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

        if (this == null || Instance != this || !sessao.Corresponde(visita))
            return;

        if (novoCatalogo == null)
        {
            ConfigurarCenario(cenario);
            return;
        }

        LimparMissoes();
        catalogoMissoes = novoCatalogo;
        cenarioCatalogo = cenario;
        if (cenario == MissoesClothingStore.Cenario)
            visitasClothingStore++;
        if (cenario == MissoesCollege.Cenario)
            visitasCollege++;
        if (cenario == MissoesHotel.Cenario)
            visitasHotel++;
        if (cenario == MissoesHome.Cenario)
            visitasHome++;
        if (cenario == MissoesRestaurant.Cenario)
            visitasRestaurant++;
        if (cenario == MissoesVisitorCenter.Cenario)
            visitasVisitorCenter++;
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
        if (!SessaoCenario.Atual.Permite("Market"))
            return;

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
        if (!SessaoCenario.Atual.Permite("Market"))
            return;

        if (UsaCatalogoMarket)
            RegistrarEventoMarket(tipo, alvo);
        else
            ConcluirMissao(id);
    }

    private void AvaliarEvento(string tipo, string alvo)
    {
        var sessao = SessaoCenario.Atual;
        if (sessao.Nome != cenarioCatalogo || (!sessao.EmJogo && tipo != "cenario_iniciado"))
            return;

        if (catalogoMissoes == null)
            return;

        catalogoMissoes.Registrar(tipo, alvo, AdicionarDoCatalogo, ConcluirMissao);
        if (sessao.EmJogo)
            CheckpointJogo.Instance?.AgendarCheckpointMissoes(sessao.Versao);
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
    private void ConfigurarCenario(string cenario)
    {
        LimparMissoes();
        catalogoMissoes = null;
        cenarioCatalogo = null;

        if (cenario == MissoesVisitorCenter.Cenario)
        {
            visitasVisitorCenter++;
            catalogoMissoes = MissoesVisitorCenter.Criar(DadosJogador.nivelUsuario);
            cenarioCatalogo = cenario;
            AvaliarEvento("cenario_iniciado", cenario);
            return;
        }

        if (cenario == MissoesRestaurant.Cenario)
        {
            visitasRestaurant++;
            catalogoMissoes = MissoesRestaurant.Criar(DadosJogador.nivelUsuario);
            cenarioCatalogo = cenario;
            AvaliarEvento("cenario_iniciado", cenario);
            return;
        }

        if (cenario == MissoesHome.Cenario)
        {
            visitasHome++;
            catalogoMissoes = MissoesHome.Criar(DadosJogador.nivelUsuario);
            cenarioCatalogo = cenario;
            AvaliarEvento("cenario_iniciado", cenario);
            return;
        }

        if (cenario == MissoesHotel.Cenario)
        {
            visitasHotel++;
            catalogoMissoes = MissoesHotel.Criar(DadosJogador.nivelUsuario);
            cenarioCatalogo = cenario;
            AvaliarEvento("cenario_iniciado", cenario);
            return;
        }

        if (cenario == MissoesCollege.Cenario)
        {
            visitasCollege++;
            catalogoMissoes = MissoesCollege.Criar(DadosJogador.nivelUsuario);
            cenarioCatalogo = cenario;
            AvaliarEvento("cenario_iniciado", cenario);
            return;
        }

        if (cenario == MissoesClothingStore.Cenario)
        {
            visitasClothingStore++;
            catalogoMissoes = MissoesClothingStore.Criar(DadosJogador.nivelUsuario);
            cenarioCatalogo = cenario;
            AvaliarEvento("cenario_iniciado", cenario);
            return;
        }

        if (cenario == "Airport" || cenario == "Market")
        {
            catalogoMissoes = MissoesLocaisMarketAirport.Criar(cenario, SessaoCenario.Atual.Nivel);
            cenarioCatalogo = cenario;
            Debug.Log($"Checkpoint diagnóstico: catálogo local de {cenario}/{SessaoCenario.Atual.Nivel}.", this);
            AvaliarEvento("cenario_iniciado", cenario);
        }
    }

    public static bool NivelMarketIntermediario(string nivel)
    {
        return nivel == "A1-A2" || nivel == "A2-B1";
    }

    public void RegistrarCestaMarketIntermediario()
    {
        if (!SessaoCenario.Atual.Permite("Market") || !MissaoAtiva("pegar_cesta"))
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
        if (!SessaoCenario.Atual.Permite("Market") || UsaCatalogoMarket || !MissaoConcluida("falar_padaria") || !MissaoConcluida("falar_atendente")
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
            {
                missao.itemUI.gameObject.SetActive(false);
                Destroy(missao.itemUI.gameObject);
            }
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
        var sessao = SessaoCenario.Atual;
        bool chaveValida = cenarioCatalogo == sessao.Nome && catalogoMissoes != null
            ? catalogoMissoes.Definicoes.Any(d => d.mission_key == id)
            : CatalogoMissoes.ChaveDoCenario(sessao.Nome, sessao.Nivel, id);
        if (!chaveValida || sessao.EmResultado || missoes.Exists(m => m.id == id))
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
        if (!SessaoCenario.Atual.Permite("Airport"))
            return;

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
        var sessao = SessaoCenario.Atual;
        if (sessao.EmJogo && MarcarConcluida(id))
            CheckpointJogo.Instance?.AgendarCheckpointMissoes(sessao.Versao);
    }

    private bool MarcarConcluida(string id)
    {
        var missao = missoes.Find(m => m.id == id);
        if (missao == null || missao.concluida)
            return false;

        missao.concluida = true;
        if (missao.itemUI != null)
            missao.itemUI.AtualizarVisual();
        return true;
    }

    public static void ConcluirMissaoNPC(string id)
    {
        if (!SessaoCenario.Atual.Permite("Airport"))
            return;

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
