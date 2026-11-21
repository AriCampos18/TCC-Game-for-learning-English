using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class GerenciadorProgressao : MonoBehaviour
{
    public static GerenciadorProgressao Instance;
    public ModalFinalManager modalFinalManager;

    public ModalBoasVindasGerenciador modalBoasVindasGerenciador;
    public GameObject modalSelecaoCenario;
    private List<string> ultimosCenarios = new List<string>();

    private int totalExercicios;
    private int exerciciosAcertados;
    private string nivelAtual;

    private int cenariosConcluidos;
    private int quantidadeCenariosIniciais;
    private bool cenarioExtraSolicitado;
    private bool cicloInicializado;

    private int totalExerciciosCiclo;
    private int exerciciosAcertadosCiclo;

    private HashSet<ExercicioBase> exerciciosRegistrados =
        new HashSet<ExercicioBase>();

    private bool cenarioFinalizado;
    private int visitaCenario;
    private string checkpointCiclo;
    private readonly List<string> visitasContabilizadas = new List<string>();
    private readonly JArray resultadosCheckpoint = new JArray();

    internal JObject CapturarProgressaoCheckpoint(bool incluirVisita, JObject decisao, JArray processadas)
    {
        if (!cicloInicializado)
            IniciarCiclo();
        return new JObject
        {
            ["cycle_id"] = checkpointCiclo,
            ["completed_visit_ids"] = new JArray(visitasContabilizadas),
            ["initial_scenario_count"] = quantidadeCenariosIniciais,
            ["cycle_totals"] = Totais(totalExerciciosCiclo, exerciciosAcertadosCiclo),
            ["visit_totals"] = incluirVisita ? Totais(totalExercicios, exerciciosAcertados) : Totais(0, 0),
            ["extra_pending"] = cenarioExtraSolicitado,
            ["recent_scenario_ids"] = new JArray(ultimosCenarios.Select(SessaoCenario.Id)),
            ["bar_correct_keys"] = ProgressoNivelManager.Instance?.CapturarCheckpoint() ?? new JArray(),
            ["decision"] = decisao == null ? JValue.CreateNull() : (JToken)decisao.DeepClone(),
            ["processed_decision_ids"] = processadas.DeepClone()
        };
    }

    private static JObject Totais(int total, int acertos) => new JObject { ["graded"] = total, ["correct"] = acertos };

    internal JArray ResultadosCheckpoint() => (JArray)resultadosCheckpoint.DeepClone();

    internal void RestaurarCheckpoint(JObject progresso, JArray resultados, bool jaFinalizado)
    {
        nivelAtual = DadosJogador.nivelUsuario;
        cicloInicializado = true;
        checkpointCiclo = (string)progresso["cycle_id"];
        visitasContabilizadas.Clear();
        visitasContabilizadas.AddRange(progresso["completed_visit_ids"].Values<string>());
        cenariosConcluidos = visitasContabilizadas.Count;
        quantidadeCenariosIniciais = (int)progresso["initial_scenario_count"];
        totalExerciciosCiclo = (int)progresso["cycle_totals"]["graded"];
        exerciciosAcertadosCiclo = (int)progresso["cycle_totals"]["correct"];
        cenarioExtraSolicitado = (bool)progresso["extra_pending"];
        totalExercicios = (int)progresso["visit_totals"]["graded"];
        exerciciosAcertados = (int)progresso["visit_totals"]["correct"];
        ultimosCenarios.Clear();
        ultimosCenarios.AddRange(progresso["recent_scenario_ids"].Values<int>().Select(CheckpointJogo.NomeCenario));
        visitaCenario = SessaoCenario.Atual.EmResultado || SessaoCenario.Atual.EmJogo ? SessaoCenario.Atual.Versao : 0;
        cenarioFinalizado = jaFinalizado;
        exerciciosRegistrados.Clear();
        resultadosCheckpoint.RemoveAll();
        foreach (var resultado in resultados)
            resultadosCheckpoint.Add(resultado.DeepClone());
    }

    private void Awake()
    {
        Debug.Log(
            $"[Progressao] Awake | Objeto: {gameObject.name} | " +
            $"Instance antes: {(Instance != null ? Instance.gameObject.name : "NULL")}"
        );

        if (Instance != null && Instance != this)
        {
            Debug.LogWarning(
                $"[Progressao] Duplicado destruído: {gameObject.name} | " +
                $"Instância mantida: {Instance.gameObject.name}"
            );

            Destroy(gameObject);
            return;
        }

        Instance = this;

        Debug.Log($"[Progressao] Instância definida: {gameObject.name}");
    }

    private void OnDestroy()
    {
        Debug.Log($"[Progressao] OnDestroy: {gameObject.name}");

        if (Instance == this)
            Instance = null;
    }

    // Inicia um novo ciclo de avaliação.
    public void IniciarCiclo()
    {
        checkpointCiclo = Guid.NewGuid().ToString();
        visitasContabilizadas.Clear();
        cenariosConcluidos = 0;
        totalExerciciosCiclo = 0;
        exerciciosAcertadosCiclo = 0;
        cenarioExtraSolicitado = false;

        nivelAtual = DadosJogador.nivelUsuario;

        if (!NivelValido(nivelAtual))
        {
            Debug.LogError("Nível inválido: " + nivelAtual);
            cicloInicializado = false;
            return;
        }

        if (nivelAtual == "A1" ||
            nivelAtual == "A2" ||
            nivelAtual == "B1")
        {
            quantidadeCenariosIniciais = 2;
        }
        else
        {
            quantidadeCenariosIniciais = 1;
        }

        cicloInicializado = true;

        Debug.Log(
            $"Novo ciclo: nível {nivelAtual} | " +
            $"Cenários iniciais: {quantidadeCenariosIniciais}"
        );
    }

    

    // Chamado quando um cenário começa.
    public void IniciarCenario()
    {
        if (!SessaoCenario.Atual.EmJogo || SessaoCenario.Atual.Corresponde(visitaCenario))
            return;
        visitaCenario = SessaoCenario.Atual.Versao;
        if (!cicloInicializado)
            IniciarCiclo();

        if (!cicloInicializado)
            return;

        totalExercicios = 0;
        exerciciosAcertados = 0;

        exerciciosRegistrados.Clear();
        resultadosCheckpoint.RemoveAll();
        cenarioFinalizado = false;

        nivelAtual = DadosJogador.nivelUsuario;
    }

    public void RegistrarResultado(
        ExercicioBase exercicio,
        bool acertou,
        InteracaoNPC origem = null)
    {
        if (!SessaoCenario.Atual.EmJogo || !SessaoCenario.Atual.Corresponde(visitaCenario)
            || cenarioFinalizado || exercicio == null)
            return;

        if (!exerciciosRegistrados.Add(exercicio))
            return;

        totalExercicios++;
        var chave = CheckpointJogo.ChaveExercicio(exercicio, origem, incluirCenario: false);
        if (chave != null)
        {
            var resultado = new JObject { ["key"] = chave, ["outcome"] = acertou ? "correct" : "failed" };
            if (!acertou)
                resultado["incorrect_attempts"] = 3;
            resultadosCheckpoint.Add(resultado);
        }

        if (acertou)
            exerciciosAcertados++;

        Debug.Log(
            $"Exercícios do cenário: {totalExercicios} | " +
            $"Acertos: {exerciciosAcertados}"
        );
    }

    // Acurácia somente do cenário atual.
    public float CalcularAcuracia()
    {
        if (totalExercicios == 0)
            return 0f;

        return exerciciosAcertados /
               (float)totalExercicios * 100f;
    }

    // Acurácia conjunta dos cenários iniciais.
    private float CalcularAcuraciaCiclo()
    {
        if (totalExerciciosCiclo == 0)
            return 0f;

        return exerciciosAcertadosCiclo /
               (float)totalExerciciosCiclo * 100f;
    }

    // Chamado ao fechar o modal final do cenário.
    public string FinalizarCenario()
    {
        if (!SessaoCenario.Atual.EmResultado || !SessaoCenario.Atual.Corresponde(visitaCenario) || cenarioFinalizado)
            return "CenarioJaFinalizado";

        cenarioFinalizado = true;

        if (totalExercicios == 0)
        {
            Debug.LogWarning(
                "Cenário sem exercícios registrados. " +
                "A avaliação não será realizada."
            );

            AbrirSelecaoCenario();
            return "SemExercicios";
        }

        // O cenário extra é avaliado separadamente.
        if (cenarioExtraSolicitado)
        {
            float acuraciaExtra = CalcularAcuracia();

            Debug.Log(
                $"Acurácia do cenário extra: {acuraciaExtra:F1}%"
            );

            if (acuraciaExtra >= 75f)
            {
                AvancarOuConcluir();
            }
            else
            {
                // Não atingiu 75% no cenário extra.
                // Permanece no nível atual e reinicia o ciclo.
                MostrarResultado(
                    "Manter",
                    nivelAtual,
                    nivelAtual
                );
            }

            return "AvaliacaoExtraFinalizada";
        }

        // Acumula os resultados dos cenários iniciais.
        totalExerciciosCiclo += totalExercicios;
        exerciciosAcertadosCiclo += exerciciosAcertados;
        cenariosConcluidos++;
        string visitaId = CheckpointJogo.Instance?.VisitaId;
        if (visitaId != null)
            visitasContabilizadas.Add(visitaId);

        Debug.Log(
            $"Cenários iniciais concluídos: {cenariosConcluidos} | " +
            $"Acurácia inicial: {CalcularAcuraciaCiclo():F1}%"
        );

        // Ainda faltam cenários iniciais.
        if (cenariosConcluidos < quantidadeCenariosIniciais)
        {
            AbrirSelecaoCenario();

            return "ContinuarCenarios";
        }

        // Todos os cenários iniciais foram concluídos.
        AvaliarCiclo();

        return "AvaliacaoProcessada";
    }

    // Avalia a acurácia dos cenários iniciais.
    private void AvaliarCiclo()
    {
        float acuracia = CalcularAcuraciaCiclo();

        nivelAtual = DadosJogador.nivelUsuario;

        Debug.Log(
            $"Acurácia inicial do nível {nivelAtual}: " +
            $"{acuracia:F1}%"
        );

        if (nivelAtual == "A1")
        {
            // A1: qualquer resultado abaixo de 75%
            // exige um cenário extra.
            if (acuracia >= 75f)
            {
                AvancarOuConcluir();
            }
            else
            {
                SolicitarCenarioExtra();
            }
        }
        else if (nivelAtual == "A2" ||
                 nivelAtual == "B1" ||
                 nivelAtual == "A1-A2" ||
                 nivelAtual == "A2-B1")
        {
            if (acuracia >= 75f)
            {
                AvancarOuConcluir();
            }
            else if (acuracia >= 50f)
            {
                SolicitarCenarioExtra();
            }
            else
            {
                RegredirNivel();
            }
        }
    }

    // Solicita um cenário extra e separa suas métricas.
    private void SolicitarCenarioExtra()
    {
        cenarioExtraSolicitado = true;
        visitasContabilizadas.Clear();

        // Zera os contadores da avaliação inicial.
        // A acurácia do extra será calculada individualmente.
        totalExerciciosCiclo = 0;
        exerciciosAcertadosCiclo = 0;
        cenariosConcluidos = 0;

        Debug.Log(
            "Será necessário concluir um cenário extra. " +
            "A avaliação será feita separadamente."
        );

        MostrarResultadoNoModal("CenarioExtra", nivelAtual, nivelAtual);
    }

    private void AvancarOuConcluir()
    {
        string nivelAnterior = nivelAtual;

        if (nivelAtual == "A1")
        {
            AtualizarNivelGlobal("A1-A2");
        }
        else if (nivelAtual == "A1-A2")
        {
            AtualizarNivelGlobal("A2");
        }
        else if (nivelAtual == "A2")
        {
            AtualizarNivelGlobal("A2-B1");
        }
        else if (nivelAtual == "A2-B1")
        {
            AtualizarNivelGlobal("B1");
        }
        else if (nivelAtual == "B1")
        {
            // O usuário concluiu o nível final.
            MostrarResultado(
                "Concluido",
                nivelAnterior,
                nivelAnterior
            );

            return;
        }

        MostrarResultado(
            "Avancar",
            nivelAnterior,
            DadosJogador.nivelUsuario
        );
    }

    private void RegredirNivel()
    {
        string nivelAnterior = nivelAtual;
        string nivelNovo;

        if (nivelAtual == "B1")
            nivelNovo = "A2-B1";
        else if (nivelAtual == "A2-B1")
            nivelNovo = "A2";
        else if (nivelAtual == "A2")
            nivelNovo = "A1-A2";
        else if (nivelAtual == "A1-A2")
            nivelNovo = "A1";
        else
        {
            MostrarResultado(
                "Manter",
                nivelAtual,
                nivelAtual
            );

            return;
        }

        AtualizarNivelGlobal(nivelNovo);

        MostrarResultado(
            "Regredir",
            nivelAnterior,
            nivelNovo
        );
    }

    private void AtualizarNivelGlobal(string novoNivel)
    {
        nivelAtual = novoNivel;
        DadosJogador.nivelUsuario = novoNivel;

        if (ProgressoNivelManager.Instance != null)
        {
            ProgressoNivelManager.Instance.InicializarBarra();
        }
    }

    private void MostrarResultado(string resultado, string nivelAnterior, string nivelNovo)
    {
        IniciarCiclo();

        MostrarResultadoNoModal(
            resultado,
            nivelAnterior,
            nivelNovo
        );
    }

    public void AbrirSelecaoCenario()
    {
        if (SessaoCenario.Atual.EmJogo)
            return;

        SessaoCenario.Atual.Encerrar();
        MissionManager.Instance?.EncerrarCenario();
        modalFinalManager?.LimparVisita();
        GameProgress.EstaEmDialogo = false;
        if (modalSelecaoCenario != null)
        {
            modalSelecaoCenario.SetActive(true);
        }
        else
        {
            Debug.LogError(
                "Modal de seleção de cenário não foi associado."
            );
        }
    }

    private void AbrirModalAvisoProximoNivel(
        string resultado,
        string nivelAnterior,
        string nivelNovo)
    {
        if (modalBoasVindasGerenciador != null)
        {
            modalBoasVindasGerenciador.AbrirModal(
                ModalBoasVindasGerenciador.TipoModal.AvisoProgressao,
                resultado,
                nivelAnterior,
                nivelNovo
            );
        }
        else
        {
            Debug.LogError(
                "ModalBoasVindasGerenciador não foi associado."
            );
        }
    }

    // Registra o cenário escolhido e mantém apenas os dois últimos cenários do histórico.
    public void RegistrarCenarioEscolhido(string cenario)
    {
        if (!PodeSelecionarCenario(cenario))
        {
            Debug.LogWarning(
                "Esse cenário está bloqueado: " + cenario
            );
            return;
        }

        ultimosCenarios.Add(cenario);

        if (ultimosCenarios.Count > 2)
        {
            ultimosCenarios.RemoveAt(0);
        }

        Debug.Log(
            "Últimos cenários: " +
            string.Join(", ", ultimosCenarios)
        );
    }

    public void MostrarResultadoNoModal(
        string resultado,
        string nivelAnterior,
        string nivelNovo)
    {
        if (modalFinalManager != null)
        {
            modalFinalManager.MostrarResultado(
                resultado,
                nivelAnterior,
                nivelNovo
            );
        }
        else
        {
            Debug.LogError(
                "ModalFinalManager não foi associado."
            );
        }
    }

    public void ContinuarParaSelecao()
    {
        AbrirSelecaoCenario();
    }

    private bool NivelValido(string nivel)
    {
        return nivel == "A1" ||
               nivel == "A2" ||
               nivel == "B1" ||
               nivel == "A1-A2" ||
               nivel == "A2-B1";
    }

    public bool PodeSelecionarCenario(string cenario)
    {
        return SessaoCenario.Conhecido(cenario) && !ultimosCenarios.Contains(cenario);
    }

    // Mantém compatibilidade com chamadas antigas.
    public string AvaliarProgressao()
    {
        return FinalizarCenario();
    }
}