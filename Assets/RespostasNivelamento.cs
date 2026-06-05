using System.Collections.Generic;

public class RespostaNivelamento
{
    public string nivel;
    public string tipo;
    public string pergunta;

    // discursiva
    public string respostaUsuario;

    // objetiva
    public List<string> alternativas;
    public int alternativaCorreta;
    public int respostaSelecionada;
}