using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public class NivelamentoObjetivoScript : MonoBehaviour
{
    public Button[] botoes;
    private int respostaSelecionada = -1;
    private NivelamentoObjetivoScript nivelamentoObjetivoScript;
    private ModalNivelmanetoScript modalNivelamento;
    private ExercicioAlternativas exercicioAtual;

    // Start is called before the first frame update
    void Start()
    {
        nivelamentoObjetivoScript = FindObjectOfType<NivelamentoObjetivoScript>();
        modalNivelamento = FindObjectOfType<ModalNivelmanetoScript>();

    }

   public void ConfigurarAlternativas(ExercicioAlternativas ex)
    {
        respostaSelecionada = -1;
        exercicioAtual = ex;

        for (int i = 0; i < botoes.Length; i++)
        {
            int index = i;

            botoes[i].onClick.RemoveAllListeners();

            // RESETAR COR
            botoes[i].image.color = Color.white;

            // REATIVAR BOTÃO
            botoes[i].interactable = true;

            botoes[i].GetComponentInChildren<TextMeshProUGUI>().text =
                ex.alternativas[i];

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
        {
            botoes[i].interactable = false;
        }

        int correta = exercicioAtual.alternativaCorreta;

        // ACERTOU
        if (index == correta)
        {
            botoes[index].image.color = Color.green;
        }

        // ERROU
        else
        {
            botoes[index].image.color = Color.red;

            // MOSTRA A CORRETA
            botoes[correta].image.color = Color.green;
        }
    }

    public int GetRespostaSelecionada()
    {
        return respostaSelecionada;
    }
}
