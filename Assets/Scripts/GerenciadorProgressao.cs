using System.Collections.Generic;
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

    private void Awake()
    {
        Debug.Log("Awake do GerenciadorProgressao foi chamado.");

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Debug.Log("Instância do GerenciadorProgressao definida.");
    }

    // Inicia um novo ciclo de avaliação.
    public void IniciarCiclo()
    {
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
        if (!cicloInicializado)
            IniciarCiclo();

        if (!cicloInicializado)
            return;

        totalExercicios = 0;
        exerciciosAcertados = 0;

        exerciciosRegistrados.Clear();
        cenarioFinalizado = false;

        nivelAtual = DadosJogador.nivelUsuario;
    }

    public void RegistrarResultado(
        ExercicioBase exercicio,
        bool acertou)
    {
        if (cenarioFinalizado || exercicio == null)
            return;

        if (!exerciciosRegistrados.Add(exercicio))
            return;

        totalExercicios++;

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
        if (cenarioFinalizado)
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

        Debug.Log(
            $"Cenários iniciais concluídos: {cenariosConcluidos} | " +
            $"Acurácia inicial: {CalcularAcuraciaCiclo():F1}%"
        );

        // Ainda faltam cenários iniciais.
        if (cenariosConcluidos < quantidadeCenariosIniciais)
        {
            MostrarResultadoNoModal("ContinuarCenarios",nivelAtual,nivelAtual);

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
        return !ultimosCenarios.Contains(cenario);
    }

    // Mantém compatibilidade com chamadas antigas.
    public string AvaliarProgressao()
    {
        return FinalizarCenario();
    }
}