using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AlternativasUI : MonoBehaviour
{
    public TextMeshProUGUI enunciado;
    public Button[] botoes;

    private int respostaSelecionada = -1;
    private ExercicioAlternativas exercicio;

    // ✨ NOVAS VARIÁVEIS PARA CONTROLE DE CHANCES
    private int tentativasRestantes = 3; 

    public void Setup(ExercicioAlternativas ex)
    {
        exercicio = ex;
        enunciado.text = ex.enunciado;
        respostaSelecionada = -1;

        // ✨ Reseta as chances toda vez que o exercício abre
        tentativasRestantes = 3; 

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
            botoes[i].image.color = Color.white;

        botoes[index].image.color = new Color32(56, 142, 60, 255);
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