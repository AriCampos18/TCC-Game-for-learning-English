using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AlternativasUI : MonoBehaviour
{
    public Button[] botoes;

    private int respostaSelecionada = -1;

    public TextMeshProUGUI textoFeedback;
    private ExercicioAlternativas exercicio;

    private Color32 corNormal = new Color32(255, 255, 255, 255);
    private Color32 corSelecionada = new Color32(220, 235, 255, 255);
    private Color32 corCorreta = new Color32(200, 230, 201, 255);
    private Color32 corErrada = new Color32(255, 205, 210, 255);

    // ✨ NOVAS VARIÁVEIS PARA CONTROLE DE CHANCES
    private int tentativasRestantes = 3; 

    public void Setup(ExercicioAlternativas ex)
    {
        exercicio = ex;
        respostaSelecionada = -1;

        // ✨ Reseta as chances toda vez que o exercício abre
        tentativasRestantes = 3; 

        if (textoFeedback != null) textoFeedback.text = "";

        for (int i = 0; i < botoes.Length; i++)
        {
            int index = i;
            botoes[i].onClick.RemoveAllListeners();
            botoes[i].GetComponentInChildren<TextMeshProUGUI>().text = ex.alternativas[i];

            botoes[i].onClick.AddListener(() =>
            {
                Selecionar(index);
            });
        }
    }

    void Selecionar(int index)
    {
        respostaSelecionada = index;

        for (int i = 0; i < botoes.Length; i++)
            botoes[i].image.color = corNormal;

        botoes[index].image.color = corSelecionada;
    }

    public void MostrarResultadoVisual()
    {
        if (respostaSelecionada == -1) return;

        if (EstaCorreto())
            botoes[respostaSelecionada].image.color = corCorreta;
        else
            botoes[respostaSelecionada].image.color = corErrada;
    }
    
    public bool Respondeu()
    {
        return respostaSelecionada != -1;
    }

    public bool EstaCorreto()
    {
        return respostaSelecionada == exercicio.alternativaCorreta;
    }

    // ✨ MÉTODOS AUXILIARES PARA O MODAL CONSULTAR AS CHANCES
    public void ReduzirTentativa()
    {
        tentativasRestantes--;
    }

    public int ObterTentativasRestantes()
    {
        return tentativasRestantes;
    }
}