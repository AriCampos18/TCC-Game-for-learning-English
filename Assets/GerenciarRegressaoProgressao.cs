using System.Collections.Generic;
using UnityEngine;

public class GerenciadorProgressao : MonoBehaviour
{
    public static GerenciadorProgressao Instance;
    public ModalBoasVindasGerenciador modalBoasVindasGerenciador;
    private int totalExercicios;
    private int exerciciosAcertados;
    private string nivelAtual;

    // Evita registrar duas vezes a mesma ocorrência de exercício.
    private HashSet<ExercicioBase> exerciciosRegistrados =
        new HashSet<ExercicioBase>();

    private bool cenarioFinalizado;

    private void Awake()
    {
        Instance = this;
    }

    public void IniciarCenario()
    {
        nivelAtual = DadosUsuario.nivelUsuario;
        totalExercicios = 0;
        exerciciosAcertados = 0;

        exerciciosRegistrados.Clear();
        cenarioFinalizado = false;

        if (!NivelValido(nivelAtual))
        {
            Debug.LogError(
                $"Não é possível iniciar a avaliação: nível global inválido ('{nivelAtual}')."
            );
        }
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
            $"Exercícios concluídos: {totalExercicios} | " +
            $"Acertos: {exerciciosAcertados}"
        );
    }

    public float CalcularAcuracia()
    {
        if (totalExercicios == 0)
            return 0f;

        return exerciciosAcertados /
               (float)totalExercicios * 100f;
    }

    public string AvaliarProgressao()
    {
        if (cenarioFinalizado)
            return "CenarioJaFinalizado";

        cenarioFinalizado = true;

        nivelAtual = DadosUsuario.nivelUsuario;
        if (!NivelValido(nivelAtual))
        {
            Debug.LogError(
                $"Não é possível atualizar o nível: nível global inválido ('{nivelAtual}')."
            );

            return "NivelInvalido";
        }

        if (totalExercicios == 0)
        {
            Debug.LogWarning(
                "Não é possível avaliar um cenário sem exercícios."
            );

            return "SemExercicios";
        }

        float acuracia = CalcularAcuracia();

        Debug.Log($"Acurácia do cenário: {acuracia:F1}%");

        if (acuracia >= 75f){
            return AvancarNivel();
        }

        if (acuracia >= 50f){
            AbrirModalEscolhaCenario("Manter",
                nivelAtual,
                nivelAtual);
            return "Manter";
        }

        return RegredirNivel();
    }

    private string AvancarNivel()
    {
        string nivelAnterior = nivelAtual;

        if (nivelAtual == "A1")
            AtualizarNivelGlobal("A2");
        else if (nivelAtual == "A2")
            AtualizarNivelGlobal("B1");
        else
            return "Manter";

        AbrirModalAvisoProximoNivel(
            "Avancar",
            nivelAnterior,
            nivelAtual
        );

        return "Avancar";
    }

    private string RegredirNivel()
    {
        string nivelAnterior = nivelAtual;

        if (nivelAtual == "B1")
            AtualizarNivelGlobal("A2");
        else if (nivelAtual == "A2")
            AtualizarNivelGlobal("A1");
        else
            return "Manter";
        
        AbrirModalAvisoProximoNivel("Regredir",
            nivelAnterior,
            nivelAtual);

        return "Regredir";
    }

    private void AtualizarNivelGlobal(string novoNivel)
    {
        nivelAtual = novoNivel;
        DadosUsuario.nivelUsuario = novoNivel;

        if (ProgressoNivelManager.Instance != null)
            ProgressoNivelManager.Instance.InicializarBarra();
    }

    private void AbrirModalAvisoProximoNivel(string resultado, string nivelAnterior, string nivelNovo)
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
    }

    private bool NivelValido(string nivel)
    {
        return nivel =="A1" || nivel == "A2" || nivel == "B1";
    }
}