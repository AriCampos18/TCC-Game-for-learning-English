using System.Collections.Generic;
using UnityEngine;

public class ExercicioRevisao
{
    public TipoExercicio tipo;
    public ExercicioBase exercicio;
    public string contextoNpc;
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
            contextoNpc = contextoNpc
        });
    }

    public List<ExercicioRevisao> GetExerciciosErrados()
    {
        return new List<ExercicioRevisao>(exerciciosErrados);
    }

    public bool TemRevisao()
    {
        return exerciciosErrados.Count > 0;
    }

    public void Limpar()
    {
        exerciciosErrados.Clear();
    }
}