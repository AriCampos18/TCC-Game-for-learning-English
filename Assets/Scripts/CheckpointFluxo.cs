using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

internal sealed partial class CheckpointJogo
{
    private static readonly string[] Cenarios = {
        "Market", "Airport", "Clothing Store", "Hotel", "Home", "Visitor Center", "Restaurant", "College"
    };
    private JObject visitaConcluida;
    private JObject decisao;
    private readonly JArray processadas = new JArray();
    private string modoGiro = "snap";
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

    private void Capturar(string fase, JObject visita)
    {
        if (restaurando || encerrando || semPosse || descarregado || dispositivo == null || GerenciadorProgressao.Instance == null)
            return;
        var novo = new JObject {
            ["phase"] = fase, ["account_level"] = DadosJogador.nivelUsuario,
            ["preferences"] = new JObject { ["turn_mode"] = modoGiro },
            ["visit"] = visita == null ? JValue.CreateNull() : (JToken)visita.DeepClone(),
            ["progression"] = GerenciadorProgressao.Instance.CapturarProgressaoCheckpoint(
                fase == "scenario_result" || fase == "progression_result", decisao, processadas)
        };
        if (JToken.DeepEquals(novo, dispositivo.Atual))
            return;
        if (pendenteRejeitado)
        {
            dispositivo.Pendente = null; // O servidor confirmou a rejeição da tentativa.
            pendenteRejeitado = false;
        }
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
        if (!NivelValido(nivel) || progresso == null
            || (fase != "scenario_selection" && fase != "scenario_intro" && fase != "scenario_result" && fase != "progression_result")
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

        DadosJogador.nivelUsuario = nivel;
        modoGiro = (string)estado["preferences"]?["turn_mode"] ?? "snap";
        decisao = progresso["decision"] as JObject;
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
            if (fase == "scenario_intro")
            {
                await selecao.RetomarEntradaCheckpoint(cenario, versao, vida.Token);
                ValidarConta();
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
        else if (fase != "scenario_intro")
            final.RetomarResultadoCheckpoint(versao, fase, decisao);
    }

    private static bool NivelValido(string nivel) =>
        nivel == "A1" || nivel == "A1-A2" || nivel == "A2" || nivel == "A2-B1" || nivel == "B1";
}
