using System.Collections.Generic;
using UnityEngine;

public class ExercicioRevisao
{
    public TipoExercicio tipo;
    public ExercicioBase exercicio;
    public string contextoNpc;
    public bool aguardandoAssociacao;
}

public class RevisaoManager : MonoBehaviour
{
    public static RevisaoManager Instance;

    private List<ExercicioRevisao> exerciciosErrados = new List<ExercicioRevisao>();

    void Awake()
    {
        Instance = this;
    }

    public void RegistrarErro(TipoExercicio tipo, ExercicioBase exercicio, string contextoNpc)
    {
        exerciciosErrados.Add(new ExercicioRevisao()
        {
            tipo = tipo,
            exercicio = exercicio,
            contextoNpc = contextoNpc,
            aguardandoAssociacao = false
        });
    }

    public void RegistrarAssociacao(ExercicioAssociacao exercicio)
    {
        if (exercicio == null)
        {
            Debug.LogError("Não é possível registrar um exercício de associação nulo.");
            return;
        }

        bool jaRegistrado = exerciciosErrados.Exists(item =>
            item.aguardandoAssociacao && ReferenceEquals(item.exercicio, exercicio));
        if (jaRegistrado)
            return;

        exerciciosErrados.Add(new ExercicioRevisao()
        {
            tipo = TipoExercicio.Associacao,
            exercicio = exercicio,
            aguardandoAssociacao = true
        });
    }

    public List<ExercicioRevisao> GetAssociacoesPendentes()
    {
        return exerciciosErrados.FindAll(item => item.aguardandoAssociacao);
    }

    public void ConcluirAssociacao(ExercicioBase exercicio)
    {
        exerciciosErrados.RemoveAll(item =>
            item.aguardandoAssociacao && ReferenceEquals(item.exercicio, exercicio));
    }

    public List<ExercicioRevisao> GetExerciciosErrados()
    {
        return exerciciosErrados.FindAll(item => !item.aguardandoAssociacao);
    }

    public bool TemRevisao()
    {
        return exerciciosErrados.Exists(item => !item.aguardandoAssociacao);
    }

    public void Limpar()
    {
        exerciciosErrados.Clear();
    }
}