using System.Collections.Generic;

[System.Serializable]
public class ExercicioCompletarFrase : ExercicioBase
{
    public string fraseAntes;
    public string fraseDepois;
    public List<string> partesFrase;
    public string respostaCorreta;
    public List<string> opcoes;
}