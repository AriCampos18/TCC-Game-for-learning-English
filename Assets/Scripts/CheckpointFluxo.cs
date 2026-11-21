using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

internal interface IRoteiroNpcCheckpoint
{
    Task<Func<Task>> PrepararRetomada(JObject visita, int versao, System.Threading.CancellationToken token);
}

internal sealed partial class RoteiroNpcCheckpoint
{
    internal const string Passo = "npc.script";
    private const string Fim = "action.route.finished";
    private readonly System.Collections.Generic.List<Etapa> etapas = new();
    private readonly InteracaoNPC npc;
    private readonly string npcDescricao;

    internal RoteiroNpcCheckpoint(string npcDescricao) => this.npcDescricao = npcDescricao;
    private readonly int visita;
    private readonly Func<string[]> situacao;
    private int indiceEmExecucao = -1;
    private sealed class Etapa
    {
        internal readonly string Chave, Fala, Ponto;
        internal readonly int Ordem;
        internal readonly InteracaoNPC Ator;
        internal ExercicioBase Modelo;
        internal string Anotacao;
        internal readonly Func<Task> Executar;

        internal Etapa(string chave, Func<Task> executar, string fala, string ponto, int ordem, InteracaoNPC ator)
        {
            Chave = chave;
            Executar = executar;
            Fala = fala;
            Ponto = ponto;
            Ordem = ordem;
            Ator = ator;
        }
    }

    internal RoteiroNpcCheckpoint(InteracaoNPC npc, int visita, Func<string[]> situacao = null)
    {
        this.npc = npc;
        this.visita = visita;
        this.situacao = situacao;
    }

    internal static async Task ExigirCatalogo(InteracaoNPC npc, int versao, System.Threading.CancellationToken token)
    {
        var catalogo = await ExerciseManager.Instance.LoadForNpcAsync(npc.CheckpointNpcKey, SessaoCenario.Atual.Nivel, token);
        token.ThrowIfCancellationRequested();
        npc.ValidarRoteiroCheckpoint(versao);
        if (!catalogo.Success)
            throw new InvalidDataException("O catálogo do NPC está indisponível para retomar o roteiro.");
    }

    internal static string ChaveSaudacao(string texto) => texto switch
    {
        "Good morning!" => "greeting.morning",
        "Good afternoon!" => "greeting.afternoon",
        "Good evening!" => "greeting.evening",
        _ => null
    };

    internal static string SaudacaoSalva(JObject estado)
    {
        var dados = estado["safe_steps"] as JArray;
        return (dados?.Count > 2 ? (string)dados[2] : null) switch
        {
            "greeting.morning" => "Good morning!",
            "greeting.afternoon" => "Good afternoon!",
            "greeting.evening" => "Good evening!",
            _ => throw new InvalidDataException("Saudação do roteiro ausente ou inválida.")
        };
    }

    internal static string Hash(string valor)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        return BitConverter.ToString(sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(valor))).Replace("-", "");
    }

    internal void Fala(string texto, Func<Task> executar, InteracaoNPC ator = null, string anotacao = null)
    {
        ator ??= npc;
        string prefixo = "line." + Hash((ator?.CheckpointNpcKey ?? npcDescricao) + "|" + texto) + ".";
        string chave = prefixo + (etapas.Count(e => e.Chave.StartsWith(prefixo, StringComparison.Ordinal)) + 1);
        etapas.Add(new Etapa(chave, executar, texto, null, 0, ator) { Anotacao = anotacao });
    }

    internal void Exercicio(string ponto, int ordem, Func<Task> executar, InteracaoNPC ator = null, ExercicioBase modelo = null)
    {
        ator ??= npc;
        etapas.Add(new Etapa($"exercise.{ator?.CheckpointNpcKey ?? npcDescricao}.{ponto}.{ordem}", executar, null, ponto, ordem, ator) { Modelo = modelo });
    }

    internal void Acao(string chave, Func<Task> executar) =>
        etapas.Add(new Etapa("action." + chave, executar, null, null, 0, npc));

    internal string Identidade => Assinatura;

    private string Assinatura => "script." + Hash(string.Join("|", etapas.Select(e => e.Chave)));

    internal (int Indice, string[] Situacao) Validar(JObject estado)
    {
        var dados = (estado["safe_steps"] as JArray)?.Values<string>().ToArray();
        int indice = dados?.Length >= 2 && dados[0] == Assinatura
            ? dados[1] == Fim ? etapas.Count : etapas.FindIndex(e => e.Chave == dados[1]) : -1;
        if (indice < 0)
            throw new InvalidDataException("O roteiro salvo não corresponde ao roteiro atual.");

        var etapa = indice == etapas.Count ? new Etapa(Fim, null, null, null, 0, npc) : etapas[indice];
        var proximo = estado["next_exercise"] as JObject;
        if ((string)estado["line_key"] != (etapa.Fala == null ? null : etapa.Chave)
            || (etapa.Ponto == null ? proximo != null
                : proximo == null || (string)proximo["npc_key"] != etapa.Ator.CheckpointNpcKey
                    || (string)proximo["point_key"] != etapa.Ponto || (int?)proximo["ordem"] != etapa.Ordem))
            throw new InvalidDataException("A etapa do roteiro diverge da fala ou exercício salvo.");

        var exercicios = etapas.Where(e => e.Ponto != null)
            .Select(e => (e.Ator.CheckpointNpcKey, e.Ponto, e.Ordem)).ToHashSet();
        var esperados = etapas.Take(indice).Where(e => e.Ponto != null)
            .Select(e => (e.Ator.CheckpointNpcKey, e.Ponto, e.Ordem)).ToHashSet();
        var concluidos = ((JArray)estado["exercises"])
            .Select(e => ((string)e["key"]["npc_key"], (string)e["key"]["point_key"], (int)e["key"]["ordem"]))
            .Where(exercicios.Contains);
        if (!esperados.SetEquals(concluidos))
            throw new InvalidDataException("Os exercícios concluídos divergem da etapa do roteiro.");
        return (indice, dados.Skip(2).ToArray());
    }

    internal JArray CursorDaAcao(string acao, string[] marcos)
    {
        string chave = "action." + acao;
        if (!etapas.Any(e => e.Chave == chave))
            throw new InvalidDataException("A transição salva não existe no roteiro.");
        var cursor = new JArray(Assinatura, chave);
        foreach (string marco in marcos)
            cursor.Add(marco);
        return cursor;
    }

    internal void AtualizarCursor()
    {
        if (indiceEmExecucao < 0)
            return;
        npc.ValidarRoteiroCheckpoint(visita);
        var etapa = indiceEmExecucao == etapas.Count
            ? new Etapa(Fim, null, null, null, 0, npc) : etapas[indiceEmExecucao];
        var dados = new JArray(Assinatura, etapa.Chave);
        var marcos = situacao == null ? Array.Empty<string>() : situacao();
        if (marcos == null)
            return;
        foreach (var marco in marcos)
            dados.Add(marco);
        CheckpointJogo.Instance?.CapturarEtapaNpc(npc, visita, dados, etapa.Fala != null ? etapa.Chave : null,
            etapa.Ponto, etapa.Ordem, etapa.Ator.CheckpointNpcKey);
    }

    internal async Task Executar(int inicio = 0)
    {
        if (visita == 0)
            throw new InvalidOperationException("Uma descrição administrativa não pode executar o roteiro.");
        if (etapas.Select(e => e.Chave).Distinct().Count() != etapas.Count)
            throw new InvalidOperationException("O roteiro contém chaves de etapa duplicadas.");
        if (inicio > 0)
        {
            foreach (var ator in etapas.Select(e => e.Ator).Distinct())
                ator.ultimaFraseDita = etapas.Take(inicio).LastOrDefault(e => e.Ator == ator && e.Fala != null)?.Fala ?? "";
        }
        try
        {
            for (int indice = inicio; indice < etapas.Count; indice++)
            {
                indiceEmExecucao = indice;
                AtualizarCursor();
                await etapas[indice].Executar();
                npc.ValidarRoteiroCheckpoint(visita);
            }
            indiceEmExecucao = etapas.Count;
            AtualizarCursor();
        }
        finally { indiceEmExecucao = -1; }
    }
}

internal sealed partial class CheckpointJogo
{
    private static readonly string[] Cenarios = {
        "Market", "Airport", "Clothing Store", "Hotel", "Home", "Visitor Center", "Restaurant", "College"
    };
    internal const string HotelCartao = "hotel.room_card";
    internal const string HotelAssociacao = "hotel.association";
    internal const string HotelRevisao = "hotel.review";
    internal const string CollegeRevisao = "college.review";
    private static (string Prefixo, string Npc, string Marco) PerfilRevisao(string cenario) => cenario switch
    {
        "Market" => ("market", "cashier_npc", "market.exit"),
        "Airport" => ("airport", "checkin_npc", "airport.gate_reached"),
        "Clothing Store" => ("clothing", "clothing_seller_npc", "clothing.conversation_finished"),
        "Hotel" => ("hotel", "recepcionist_hotel_npc", "hotel.conversation_finished"),
        "Home" => ("home", "mother_npc", "home.family_finished"),
        "Visitor Center" => ("visitor_center", "visitor_center_npc", "visitor_center.park_reached"),
        "Restaurant" => ("restaurant", "restaurant_waiter_npc", "restaurant.service_finished"),
        "College" => ("college", "secretary_college_npc", "college.secretary_finished"),
        _ => default
    };
    private const string CollegeAguardandoSecretaria = "college.walk_to_secretary";
    private const string CollegeAguardandoDocumento = "college.read_document";
    private const string HomeAguardandoSaida = "home.walk_to_exit";
    private const string AirportAguardandoPortao = "airport.walk_to_gate";

    internal const string VisitorAguardandoParque = "visitor_center.walk_to_park";

    internal static (string Ponto, int Ordem) AssociacaoFinal(string cenario, string nivel) => cenario switch
    {
        "Market" => ("checkout_vocabulary", 1),
        "Clothing Store" => ("clothing_vocabulary", 1),
        "Home" => ("farewell", nivel == "A1" ? 9 : nivel == "A2" || nivel == "B1" ? 10 : 13),
        "Visitor Center" => ("arrival_vocabulary", 1),
        "Restaurant" => ("restaurant_vocabulary", 1),
        _ => default
    };

    private System.Func<Task> continuarMarco;
    internal bool RetomadaInterna => continuarMarco != null;
    internal bool TemMarcoInterno => (string)dispositivo?.Atual?["phase"] == "scenario_active"
        || (string)dispositivo?.Atual?["phase"] == "review";
    private JObject visitaConcluida;
    private JObject decisao;
    private readonly JArray processadas = new JArray();
    private string modoGiro = "snap";
    internal string ModoGiro => modoGiro;
    internal bool PodeAlterarGiro => preparado && !restaurando && !encerrando && !descarregado && !semPosse;
    internal bool PodeSalvarManual => PodeAlterarGiro && dispositivo?.Atual != null;

    internal bool AlterarModoGiro(string modo)
    {
        if (!PodeAlterarGiro || (modo != "snap" && modo != "smooth"))
            return false;
        modoGiro = modo;
        if (dispositivo.Atual is JObject atual && (string)atual["preferences"]?["turn_mode"] != modo)
        {
            var novo = (JObject)atual.DeepClone();
            novo["preferences"]["turn_mode"] = modo;
            dispositivo.Atual = novo;
            dispositivo.Alterado = true;
            _ = Persistir();
        }
        return true;
    }
    internal string VisitaId { get; private set; }

    internal static string NomeCenario(int id) => id >= 1 && id <= Cenarios.Length ? Cenarios[id - 1] : null;

    internal static void IdentificarFallback(ExercicioBase exercicio, string ponto, int ordem, string nivel)
    {
        if (exercicio == null || exercicio.idExercise > 0)
            return;
        exercicio.pointKey = ponto;
        exercicio.ordemBanco = ordem;
        exercicio.nivelBanco = nivel;
    }

    internal static JObject ChaveExercicio(ExercicioBase exercicio, InteracaoNPC npc, bool incluirCenario)
    {
        if (exercicio == null || npc == null || string.IsNullOrEmpty(npc.CheckpointNpcKey)
            || string.IsNullOrEmpty(exercicio.pointKey) || exercicio.ordemBanco <= 0)
            return null;
        var chave = new JObject { ["npc_key"] = npc.CheckpointNpcKey, ["point_key"] = exercicio.pointKey,
            ["ordem"] = exercicio.ordemBanco };
        if (exercicio.idExercise > 0)
            chave["exercise_id"] = exercicio.idExercise;
        if (incluirCenario)
        {
            chave["scenario_id"] = SessaoCenario.Id(npc.Cenario);
            chave["level"] = exercicio.nivelBanco;
        }
        return chave;
    }

    internal static string IdentidadeExercicio(JObject chave) =>
        $"{chave["scenario_id"]}/{chave["level"]}/{chave["npc_key"]}/{chave["point_key"]}/{chave["ordem"]}";

    internal void Selecionando()
    {
        if (restaurando || !NivelValido(DadosJogador.nivelUsuario))
            return;
        if (decisao != null)
        {
            if (processadas.Count == 512)
                processadas.RemoveAt(0);
            processadas.Add((string)decisao["decision_id"]);
            decisao = null;
        }
        VisitaId = null;
        visitaConcluida = null;
        Capturar("scenario_selection", null);
    }

    internal void Entrada(string cenario)
    {
        if (restaurando)
            return;
        VisitaId = Guid.NewGuid().ToString();
        visitaConcluida = null;
        Capturar("scenario_intro", NovaVisita(cenario, DadosJogador.nivelUsuario, "scenario.intro"));
    }

    internal void Inicio()
    {
        // A entrada permanece como último marco retomável durante a atividade.
        if (dispositivo?.Alterado == true)
            _ = Persistir();
    }

    internal void CapturarMarcoHotel(string passo) => CapturarMarcoInterno("Hotel", passo);
    internal void CapturarRevisaoCollege() => CapturarMarcoInterno("College", CollegeRevisao);

    internal void CapturarRevisao(string cenario, int visita)
    {
        var perfil = PerfilRevisao(cenario);
        if (perfil.Prefixo != null && SessaoCenario.Atual.Permite(cenario, visita))
            CapturarMarcoInterno(cenario, perfil.Prefixo + ".review");
    }

    internal void CapturarAssociacaoFinal(InteracaoNPC npc, string ponto, int ordem)
    {
        var contexto = SessaoCenario.Atual;
        string cenario = npc.Cenario;
        var perfil = PerfilRevisao(cenario);
        if (npc.CheckpointNpcKey == perfil.Npc && AssociacaoFinal(cenario, contexto.Nivel) == (ponto, ordem))
            CapturarMarcoInterno(cenario, perfil.Prefixo + ".association");
    }

    internal void CapturarCaminhoCityPark() => CapturarMarcoInterno("Visitor Center", VisitorAguardandoParque);

    internal void CapturarCaminhoCollege(bool documento)
    {
        string nivel = SessaoCenario.Atual.Nivel;
        CapturarMarcoInterno("College", documento ? CollegeAguardandoDocumento : CollegeAguardandoSecretaria,
            MissoesCollege.ChaveSegundaMissao(nivel));
    }

    internal void CapturarCaminhoHome() =>
        CapturarMarcoInterno("Home", HomeAguardandoSaida, MissoesHome.UltimaMissao(SessaoCenario.Atual.Nivel));

    internal void CapturarCaminhoAirport(string missaoAtiva) =>
        CapturarMarcoInterno("Airport", AirportAguardandoPortao, missaoAtiva);

    internal void CapturarEtapaNpc(InteracaoNPC npc, int versao, JArray cursor, string linha, string ponto, int ordem, string npcExercicio)
    {
        if (!SessaoCenario.Atual.Permite(npc.Cenario, versao))
            return;
        CapturarMarcoInterno(npc.Cenario, RoteiroNpcCheckpoint.Passo, npcRoteiro: npc.CheckpointNpcKey,
            cursor: cursor, linha: linha, ponto: ponto, ordem: ordem, npcExercicio: npcExercicio);
    }

    private void CapturarMarcoInterno(string cenario, string passo, string missaoAtiva = null,
        string npcRoteiro = null, JArray cursor = null, string linha = null, string ponto = null, int ordem = 0,
        string npcExercicio = null)
    {
        var contexto = SessaoCenario.Atual;
        var progresso = GerenciadorProgressao.Instance;
        var selecao = FindAnyObjectByType<ModalSelecaoCenario>(FindObjectsInactive.Include);
        var perfil = PerfilRevisao(cenario);
        bool revisao = passo == perfil.Prefixo + ".review";
        var associacao = AssociacaoFinal(cenario, contexto.Nivel);
        bool associacaoFinal = associacao.Ponto != null && passo == perfil.Prefixo + ".association";
        bool caminhoCityPark = passo == VisitorAguardandoParque;
        bool caminhoAirport = passo == AirportAguardandoPortao;
        bool caminhoCollege = passo == CollegeAguardandoSecretaria || passo == CollegeAguardandoDocumento;
        bool caminhoHome = passo == HomeAguardandoSaida;
        bool preservarFila = cursor != null || revisao || associacaoFinal || caminhoCityPark || caminhoAirport || caminhoCollege || caminhoHome
            || (cenario == "Hotel" && (passo == HotelCartao || passo == HotelAssociacao));
        var ancora = selecao?.ObterPontoDestino(cenario);
        if (!preservarFila || !contexto.Permite(cenario) || VisitaId == null || progresso == null || ancora == null
            || (cenario == "Market" && MissionManager.Instance?.UsaCatalogoMarket != true)
            || (cenario == "Airport" && MissionManager.Instance?.UsaCatalogoAirport != true)
            || RevisaoManager.Instance == null
            || RevisaoManager.Instance.GetAssociacoesPendentes(cenario).Count != 0)
            return;

        var resultados = progresso.ResultadosCheckpoint();
        var totais = progresso.CapturarProgressaoCheckpoint(true, null, processadas)["visit_totals"];
        if (resultados.Count != (int)totais["graded"]
            || resultados.Select(r => IdentidadeExercicio((JObject)r["key"])).Distinct().Count() != resultados.Count)
            return;

        if (associacaoFinal && resultados.Any(r => (string)r["key"]["npc_key"] == perfil.Npc
            && (string)r["key"]["point_key"] == associacao.Ponto && (int)r["key"]["ordem"] == associacao.Ordem))
            return;
        var fila = RevisaoManager.Instance.CapturarFilaCheckpoint(cenario);
        var erros = resultados.Where(r => (string)r["outcome"] == "failed").Select(r => (JObject)r["key"])
            .ToDictionary(IdentidadeExercicio);
        if (fila == null || fila.Cast<JObject>().Select(IdentidadeExercicio).Distinct().Count() != fila.Count)
            return;
        var revisados = RevisaoManager.Instance.CapturarRevisadosCheckpoint();
        foreach (var item in fila.Concat(revisados).Cast<JObject>())
            if (!erros.Remove(IdentidadeExercicio(item)))
                return;
        if (erros.Count != 0)
            return;

        var missoes = MissionManager.Instance?.CapturarMissoesCheckpoint();
        var chavesMissoes = CatalogoMissoes.ChavesEsperadas(cenario, contexto.Nivel);
        if (missoes == null || (cursor != null && !chavesMissoes.IsSupersetOf(missoes.Select(m => (string)m["mission_key"])))
            || (cursor == null && (missoes.Count == 0
            || (missaoAtiva != null ? (string)missoes.Last["mission_key"] != missaoAtiva
                || !chavesMissoes.IsSupersetOf(missoes.Select(m => (string)m["mission_key"]))
                : !chavesMissoes.SetEquals(missoes.Select(m => (string)m["mission_key"])))
            || missoes.Where((m, i) => (string)m["status"] !=
                ((passo == HotelCartao && i == 1)
                    || (caminhoCityPark && (string)m["mission_key"] == MissoesVisitorCenter.ChaveChegada(contexto.Nivel))
                    || (missaoAtiva != null && (string)m["mission_key"] == missaoAtiva)
                    ? "active" : "completed")).Any())))
            return;

        var visita = NovaVisita(contexto.Nome, contexto.Nivel, passo);
        visita["npc_key"] = npcRoteiro ?? perfil.Npc;
        visita["missions"] = missoes;
        visita["exercises"] = resultados;
        var marcos = new JArray(caminhoCityPark ? "visitor_center.conversation_finished"
            : caminhoAirport ? "airport.checkin_finished"
            : passo == CollegeAguardandoSecretaria ? "college.reception_finished"
            : caminhoHome ? "home.dialogue_finished" : perfil.Marco);
        if (cenario == "Hotel" && passo != HotelCartao && MissoesHotel.UsaCartao(contexto.Nivel))
            marcos.Add("hotel.card_collected");
        var inicios = cursor == null ? marcos : MissionManager.Instance.CapturarIniciosCheckpoint();
        if (cursor != null && cenario == "Market" && MissionManager.Instance.MissaoAtiva("interagir_produtos")
            && ProdutoManager.Instance != null)
            inicios.Add("market.inventory." + ProdutoManager.Instance.CapturarInventarioCheckpoint());
        visita["milestones"] = inicios;
        if (cursor != null)
        {
            visita["safe_steps"] = cursor;
            if (linha != null)
                visita["line_key"] = linha;
            if (ponto != null)
                visita["next_exercise"] = new JObject { ["npc_key"] = npcExercicio, ["point_key"] = ponto, ["ordem"] = ordem };
        }
        if (passo == HotelAssociacao)
            visita["next_exercise"] = new JObject { ["npc_key"] = MissoesHotel.Recepcao, ["point_key"] = "hotel_vocabulary", ["ordem"] = 1 };
        if (associacaoFinal)
            visita["next_exercise"] = new JObject { ["npc_key"] = perfil.Npc, ["point_key"] = associacao.Ponto,
                ["ordem"] = associacao.Ordem };
        visita["review"] = new JObject { ["phase"] = revisao ? "active" : "pending", ["queue"] = fila,
            ["completed"] = revisados };
        var p = ancora.position;
        var pose = new JObject {
            ["anchor_key"] = perfil.Prefixo + ".entrance",
            ["position"] = new JObject { ["x"] = p.x, ["y"] = p.y, ["z"] = p.z },
            ["yaw_deg"] = ancora.eulerAngles.y, ["camera_pitch_deg"] = 0
        };
        Capturar(revisao ? "review" : "scenario_active", visita, pose);
    }

    internal async Task ContinuarMarcoInterno()
    {
        var tarefa = continuarMarco;
        continuarMarco = null;
        if (tarefa == null)
            return;

        try { await tarefa(); }
        catch (Exception e) when (Prevista(e))
        {
            if (!descarregado)
                Debug.LogWarning("Checkpoint interno: " + e.Message, this);
        }
    }

    internal void Resultado()
    {
        if (restaurando || VisitaId == null || GerenciadorProgressao.Instance == null)
            return;
        var gerenciador = GerenciadorProgressao.Instance;
        var sessaoCenario = SessaoCenario.Atual;
        var visita = NovaVisita(sessaoCenario.Nome, sessaoCenario.Nivel, "scenario.result");
        var resultados = gerenciador.ResultadosCheckpoint();
        var totais = gerenciador.CapturarProgressaoCheckpoint(true, null, processadas)["visit_totals"];
        if (resultados.Count != (int)totais["graded"]
            || resultados.Select(r => IdentidadeExercicio((JObject)r["key"])).Distinct().Count() != resultados.Count)
        {
            Status = "Resultado sem chaves completas • entrada preservada";
            Debug.LogWarning("Checkpoint: resultado sem identidade estável para todos os exercícios.", this);
            return;
        }
        visita["missions"] = MissionManager.Instance?.CapturarMissoesCheckpoint() ?? new JArray();
        visita["exercises"] = resultados;
        visita["review"] = new JObject { ["phase"] = "completed", ["queue"] = new JArray(),
            ["completed"] = new JArray(resultados.Where(r => (string)r["outcome"] == "failed").Select(r => r["key"].DeepClone())) };
        visitaConcluida = visita;
        Capturar("scenario_result", visita);
    }

    internal void ResultadoProgressao(string resultado, string anterior, string novo)
    {
        if (restaurando || visitaConcluida == null)
            return;
        string tipo = resultado switch { "Avancar" => "advance", "Regredir" => "regress", "Manter" => "maintain",
            "CenarioExtra" => "extra", "Concluido" => "complete", _ => null };
        if (tipo == null)
            return;
        decisao = new JObject { ["decision_id"] = Guid.NewGuid().ToString(), ["kind"] = tipo,
            ["previous_level"] = anterior, ["new_level"] = novo, ["visit_id"] = VisitaId,
            ["status"] = "awaiting_confirmation" };
        Capturar("progression_result", visitaConcluida);
    }

    private JObject NovaVisita(string cenario, string nivel, string passo) => new JObject {
        ["visit_id"] = VisitaId, ["scenario_id"] = SessaoCenario.Id(cenario), ["level"] = nivel,
        ["flow_version"] = 1, ["step_key"] = passo, ["missions"] = new JArray(), ["exercises"] = new JArray(),
        ["milestones"] = new JArray(),
        ["review"] = new JObject { ["phase"] = "pending", ["queue"] = new JArray(), ["completed"] = new JArray() }
    };

    private void Capturar(string fase, JObject visita, JObject pose = null)
    {
        if (restaurando || encerrando || semPosse || descarregado || dispositivo == null || GerenciadorProgressao.Instance == null)
            return;
        var novo = new JObject {
            ["phase"] = fase, ["account_level"] = DadosJogador.nivelUsuario,
            ["preferences"] = new JObject { ["turn_mode"] = modoGiro },
            ["visit"] = visita == null ? JValue.CreateNull() : (JToken)visita.DeepClone(),
            ["progression"] = GerenciadorProgressao.Instance.CapturarProgressaoCheckpoint(
                fase != "scenario_selection" && fase != "scenario_intro", decisao, processadas)
        };
        if (pose != null)
            novo["pose"] = pose;
        if (JToken.DeepEquals(novo, dispositivo.Atual))
            return;
        if (pendenteRejeitado)
        {
            dispositivo.Pendente = null; // O servidor confirmou a rejeição da tentativa.
            pendenteRejeitado = false;
        }
        if ((string)visita?["step_key"] == RoteiroNpcCheckpoint.Passo)
        {
            var anterior = dispositivo.Atual;
            if ((string)anterior?["visit"]?["visit_id"] == (string)visita["visit_id"]
                && (string)anterior["visit"]["step_key"] != RoteiroNpcCheckpoint.Passo)
                dispositivo.MarcoAnterior = (JObject)anterior.DeepClone();
        }
        else
            dispositivo.MarcoAnterior = null;
        dispositivo.Atual = novo;
        dispositivo.Alterado = true;
        _ = Persistir();
    }

    private async Task Restaurar(JObject estado)
    {
        string fase = (string)estado["phase"];
        string nivel = (string)estado["account_level"];
        var visita = estado["visit"] as JObject;
        var progresso = estado["progression"] as JObject;
        string passo = (string)visita?["step_key"];
        string cenarioSalvo = NomeCenario((int?)visita?["scenario_id"] ?? 0);
        var perfil = PerfilRevisao(cenarioSalvo);
        bool perfilConfere = perfil.Prefixo != null && (string)visita?["npc_key"] == perfil.Npc
            && (string)estado["pose"]?["anchor_key"] == perfil.Prefixo + ".entrance";
        bool hotelInterno = perfilConfere && cenarioSalvo == "Hotel" && fase == "scenario_active"
            && MissoesHotel.UsaCartao((string)visita?["level"]) && (passo == HotelCartao || passo == HotelAssociacao);
        var associacao = AssociacaoFinal(cenarioSalvo, (string)visita?["level"]);
        var proximoExercicio = visita?["next_exercise"] as JObject;
        bool associacaoFinal = perfilConfere && fase == "scenario_active" && associacao.Ponto != null
            && passo == perfil.Prefixo + ".association" && (string)proximoExercicio?["npc_key"] == perfil.Npc
            && (string)proximoExercicio?["point_key"] == associacao.Ponto
            && (int?)proximoExercicio?["ordem"] == associacao.Ordem;
        bool caminhoCityPark = perfilConfere && cenarioSalvo == "Visitor Center" && fase == "scenario_active"
            && passo == VisitorAguardandoParque;
        bool caminhoAirport = perfilConfere && cenarioSalvo == "Airport" && fase == "scenario_active"
            && passo == AirportAguardandoPortao;
        bool caminhoCollege = perfilConfere && cenarioSalvo == "College" && fase == "scenario_active"
            && (passo == CollegeAguardandoSecretaria || (passo == CollegeAguardandoDocumento
                && ((string)visita?["level"] == "A1-A2" || (string)visita?["level"] == "A2-B1")));
        bool caminhoHome = perfilConfere && cenarioSalvo == "Home" && fase == "scenario_active"
            && passo == HomeAguardandoSaida && (string)visita?["level"] != "B1";
        string chaveNpc = (string)visita?["npc_key"];
        bool roteiro = fase == "scenario_active" && passo == RoteiroNpcCheckpoint.Passo
            && cenarioSalvo != null && SessaoCenario.CenarioDoNpc(chaveNpc) == cenarioSalvo
            && (string)estado["pose"]?["anchor_key"] == perfil.Prefixo + ".entrance";
        bool restaurarFila = roteiro || fase == "review" || associacaoFinal || caminhoCityPark || caminhoAirport
            || caminhoCollege || caminhoHome || hotelInterno;
        bool interno = roteiro || hotelInterno || associacaoFinal || caminhoCityPark || caminhoAirport || caminhoCollege || caminhoHome || (perfilConfere && fase == "review" && passo == perfil.Prefixo + ".review");
        if (!NivelValido(nivel) || progresso == null
            || (!interno && fase != "scenario_selection" && fase != "scenario_intro" && fase != "scenario_result" && fase != "progression_result")
            || ((fase == "scenario_selection") != (visita == null))
            || (visita != null && ((int?)visita["flow_version"] != 1 || NomeCenario((int)visita["scenario_id"]) == null
                || !NivelValido((string)visita["level"]) || !Guid.TryParse((string)visita["visit_id"], out _))))
            throw new InvalidDataException("A fase ou a versão do checkpoint não é retomável nesta etapa.");
        if (!(progresso["bar_correct_keys"] is JArray barra)
            || !(progresso["completed_visit_ids"] is JArray) || !(progresso["recent_scenario_ids"] is JArray)
            || !(progresso["cycle_totals"] is JObject) || !(progresso["visit_totals"] is JObject)
            || (visita != null && !(visita["exercises"] is JArray))
            || (fase == "progression_result" && !(progresso["decision"] is JObject)))
            throw new InvalidDataException("Checkpoint de progressão incompleto.");

        var gerenciador = GerenciadorProgressao.Instance;
        var selecao = FindAnyObjectByType<ModalSelecaoCenario>(FindObjectsInactive.Include);
        var final = gerenciador != null ? gerenciador.modalFinalManager : null;
        if (gerenciador == null || selecao == null || final == null)
            throw new InvalidOperationException("Faltam referências de seleção e resultado para retomar.");

        var apresentador = interno ? FindObjectsByType<InteracaoNPC>(FindObjectsSortMode.None)
            .FirstOrDefault(npc => npc.Cenario == cenarioSalvo && npc.CheckpointNpcKey == (roteiro ? chaveNpc : perfil.Npc)) : null;
        if (interno && (apresentador == null || (roteiro && apresentador is not IRoteiroNpcCheckpoint) || !(visita["missions"] is JArray)
            || !(visita["review"]?["queue"] is JArray)
            || !(visita["review"]?["completed"] is JArray)
            || RevisaoManager.Instance == null))
            throw new InvalidDataException("O marco interno não pode reconstruir este estado.");

        var ultimaMissaoAirport = caminhoAirport ? (visita["missions"] as JArray)?.Last as JObject : null;
        if (caminhoAirport && !checkinNPC.MissaoRetomavelCheckpoint((string)visita["level"],
            (string)ultimaMissaoAirport?["mission_key"]))
            throw new InvalidDataException("A missão de embarque não corresponde ao nível salvo.");

        string missaoAtivaRoteiro = roteiro
            ? (string)((JArray)visita["missions"]).OfType<JObject>().LastOrDefault(m => (string)m["status"] == "active")?["mission_key"] : null;
        if (interno && restaurarFila)
        {
            var falhas = ((JArray)visita["exercises"]).Where(e => (string)e["outcome"] == "failed")
                .Select(e => IdentidadeExercicio((JObject)e["key"])).ToHashSet();
            var revisao = visita["review"];
            foreach (var chave in ((JArray)revisao["queue"]).Concat((JArray)revisao["completed"]))
                if (!(chave is JObject exercicio) || !falhas.Remove(IdentidadeExercicio(exercicio)))
                    throw new InvalidDataException("A revisão contém um item duplicado ou sem três erros.");
            if (falhas.Count != 0)
                throw new InvalidDataException("A revisão não cobre todos os exercícios com três erros.");
        }

        DadosJogador.nivelUsuario = nivel;
        modoGiro = (string)estado["preferences"]?["turn_mode"] ?? "snap";
        decisao = progresso["decision"] as JObject;
        processadas.RemoveAll();
        foreach (var id in (progresso["processed_decision_ids"] as JArray ?? new JArray()))
            processadas.Add(id.DeepClone());
        var contexto = SessaoCenario.Atual;
        contexto.Encerrar();
        contexto.Bloqueada = false;
        MissionManager.Instance?.EncerrarCenario();
        final.LimparVisita();
        var boasVindas = gerenciador.modalBoasVindasGerenciador
            ?? FindAnyObjectByType<ModalBoasVindasGerenciador>(FindObjectsInactive.Include);
        boasVindas?.OcultarParaCheckpoint();
        FindAnyObjectByType<FirstPlayerController>()?.DesativarControle();

        int versao = 0;
        if (visita != null)
        {
            VisitaId = (string)visita["visit_id"];
            string cenario = NomeCenario((int)visita["scenario_id"]);
            versao = contexto.Preparar(cenario, (string)visita["level"]);
            if (fase == "scenario_intro" || interno)
            {
                await selecao.RetomarEntradaCheckpoint(cenario, versao, vida.Token, mostrarIntroducao: !interno);
                ValidarConta();
                if (interno)
                {
                    if (!contexto.Iniciar(versao)
                        || (hotelInterno ? MissionManager.Instance?.RestaurarMissoesHotel((JArray)visita["missions"], passo != HotelCartao)
                            : roteiro && ((JArray)visita["milestones"]).Count > 0
                                ? MissionManager.Instance?.RestaurarMissoesRoteiro(cenario, (JArray)visita["missions"],
                                    new JArray(((JArray)visita["milestones"]).Where(m => ((string)m).StartsWith("start.", StringComparison.Ordinal))))
                                : MissionManager.Instance?.RestaurarMissoesMarco(cenario, (JArray)visita["missions"],
                                caminhoCityPark ? MissoesVisitorCenter.ChaveChegada(contexto.Nivel)
                                    : caminhoCollege ? MissoesCollege.ChaveSegundaMissao(contexto.Nivel)
                                    : caminhoHome ? MissoesHome.UltimaMissao(contexto.Nivel)
                                    : roteiro ? missaoAtivaRoteiro : (string)ultimaMissaoAirport?["mission_key"])) != true)
                    {
                        contexto.Encerrar();
                        MissionManager.Instance?.EncerrarCenario();
                        throw new InvalidDataException("Missões salvas não correspondem ao marco interno.");
                    }
                    var revisao = RevisaoManager.Instance;
                    revisao.IniciarCenario(cenario);
                    if (restaurarFila && !await revisao.RestaurarFilaCheckpoint(
                        cenario, versao, (JArray)visita["review"]["queue"],
                        (JArray)visita["review"]["completed"], vida.Token))
                    {
                        if (contexto.Corresponde(versao))
                        {
                            contexto.Encerrar();
                            MissionManager.Instance?.EncerrarCenario();
                        }
                        throw new InvalidDataException("Não foi possível reconstruir os exercícios pendentes da revisão.");
                    }
                    ValidarConta();
                    if (associacaoFinal)
                    {
                        var banco = await ExerciseManager.Instance.LoadForPointAsync(perfil.Npc, associacao.Ponto,
                            contexto.Nivel, vida.Token);
                        ValidarConta();
                        if (!contexto.Permite(cenario, versao))
                            throw new OperationCanceledException();
                        var item = banco.Find(associacao.Ponto, contexto.Nivel, associacao.Ordem, TipoExercicio.Associacao);
                        if (item?.Exercise is not ExercicioAssociacao exercicio)
                        {
                            contexto.Encerrar();
                            MissionManager.Instance?.EncerrarCenario();
                            throw new InvalidDataException("A Associação do marco salvo não está disponível no catálogo.");
                        }
                        revisao.RegistrarAssociacao(exercicio, apresentador);
                    }
                    Func<Task> retomarRoteiro = null;
                    if (roteiro)
                    {
                        if (cenario == "Market")
                            RestaurarMarketDuranteRoteiro(visita, versao);
                        retomarRoteiro = await ((IRoteiroNpcCheckpoint)apresentador).PrepararRetomada(visita, versao, vida.Token);
                        ValidarConta();
                    }
                    continuarMarco = async () =>
                    {
                        if (!SessaoCenario.Atual.Permite(cenario, versao))
                            return;
                        if (retomarRoteiro != null)
                            await apresentador.ExecutarRoteiroRetomado(versao, retomarRoteiro);
                        else if (caminhoHome)
                        {
                            var familia = FindAnyObjectByType<FamiliaManager>();
                            if (familia == null)
                                throw new InvalidDataException("Família do Home indisponível.");
                            await familia.RetomarSaidaCheckpoint(versao);
                        }
                        else if (caminhoCollege && apresentador is SecretaryCollegeNPC secretaria)
                        {
                            var recepcao = FindAnyObjectByType<RecepcionistCollegeNPC>();
                            if (recepcao == null)
                                throw new InvalidDataException("Recepcionista do College indisponível.");
                            recepcao.RetomarAposAtendimentoCheckpoint(versao);
                            if (passo == CollegeAguardandoDocumento)
                                await secretaria.RetomarConsultaCheckpoint(versao);
                            else
                                recepcao.LiberarControlePlayer();
                        }
                        else if (caminhoAirport && apresentador is checkinNPC checkin)
                            checkin.RetomarCaminhoCheckpoint(versao, ultimaMissaoAirport);
                        else if (caminhoCityPark && apresentador is VisitorCenterNPC visitante)
                            await visitante.RetomarCaminhoCheckpoint(versao, vida.Token);
                        else if (apresentador is RecepcionistHotelNPC hotel)
                            await hotel.RetomarMarcoCheckpoint(passo, versao);
                        else if (apresentador is CashierNPC caixa)
                            await caixa.RetomarRevisaoCheckpoint();
                        else
                            await revisao.ExecutarRevisaoEExibirModalFinal(apresentador);
                    };
                    if (final.modalMissoes != null)
                        final.modalMissoes.SetActive(true);
                    if (final.modalBarraProgressao != null)
                        final.modalBarraProgressao.SetActive(true);
                }
            }
            else
            {
                if (!contexto.Iniciar(versao) || !contexto.MostrarResultado(versao))
                    throw new InvalidOperationException("Não foi possível montar a fase de resultado.");
                visitaConcluida = (JObject)visita.DeepClone();
            }
        }

        gerenciador.RestaurarCheckpoint(progresso, visita?["exercises"] as JArray ?? new JArray(),
            fase == "progression_result" || visita == null);
        ProgressoNivelManager.Instance?.RestaurarCheckpoint(barra);
        if (visita == null)
            gerenciador.AbrirSelecaoCenario();
        else if (!interno && fase != "scenario_intro")
            final.RetomarResultadoCheckpoint(versao, fase, decisao);
    }

    private void RestaurarMarketDuranteRoteiro(JObject visita, int versao)
    {
        var jogo = GameProgress.Instance;
        var missoes = MissionManager.Instance;
        if (jogo == null || missoes == null)
            throw new InvalidDataException("O estado de Market não está disponível.");

        jogo.ReiniciarMarket();
        jogo.falouAtendente = missoes.InicioRegistradoCheckpoint("conversa_encerrada", "shelf_npc");
        jogo.falouPadaria = missoes.InicioRegistradoCheckpoint("conversa_encerrada", "bakery_npc");
        if (jogo.falouAtendente)
            FindAnyObjectByType<ShelfNPC>()?.MarcarAtendimentoRestaurado(versao);
        if (jogo.falouPadaria)
            FindAnyObjectByType<BakeryNPC>()?.MarcarAtendimentoRestaurado(versao);

        if (!missoes.MissaoAtiva("interagir_produtos"))
            return;
        var inventarios = ((JArray)visita["milestones"]).Values<string>()
            .Where(m => m.StartsWith("market.inventory.", StringComparison.Ordinal)).ToArray();
        if (inventarios.Length != 1 || !int.TryParse(inventarios[0].Substring("market.inventory.".Length), out int mascara)
            || ProdutoManager.Instance?.RestaurarInventarioCheckpoint((string)visita["level"], mascara) != true)
            throw new InvalidDataException("O inventário do corredor não corresponde ao checkpoint.");
    }

    private static bool NivelValido(string nivel) =>
        nivel == "A1" || nivel == "A1-A2" || nivel == "A2" || nivel == "A2-B1" || nivel == "B1";
}
