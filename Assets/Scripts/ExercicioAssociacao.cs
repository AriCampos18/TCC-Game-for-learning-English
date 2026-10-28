using System.Collections.Generic;

[System.Serializable]
public class ExercicioAssociacao: ExercicioBase
{
    public List<string> opcoesPort;
    public List<string> opcoesIngles;

    public List<ParAssociacao> associacaoCorreta;
}