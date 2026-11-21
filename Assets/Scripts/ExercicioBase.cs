using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class ExercicioBase 
{
    // Persisted exercise identity and location; local legacy exercises leave these at defaults.
    internal string reviewGreeting;
    internal string reviewTotal;
    internal string reviewPeriod;

    internal ExercicioBase CopiarParaRevisaoCheckpoint() => (ExercicioBase)MemberwiseClone();

    public int idExercise;
    public int idTipoBanco;
    public string nomeTipoBanco;
    public string nivelBanco;
    public string pointKey;
    public int ordemBanco;
    public string enunciado;
    public string feedback;
    public List<string> respostasEsperadas = new List<string>();
}
