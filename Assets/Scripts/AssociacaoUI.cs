using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AssociacaoUI : MonoBehaviour
{
    public TMP_Dropdown[] seletores;
    public TextMeshProUGUI[] rotulosPortugues;
    public TextMeshProUGUI textoFeedback;

    private ExercicioAssociacao exercicioAtual;
    private int tentativasRestantes;
    private bool configuracaoValida;

    public void InicializarExercicio(ExercicioAssociacao exercicio)
    {
        exercicioAtual = exercicio;
        tentativasRestantes = 3;
        configuracaoValida = false;

        if (exercicio == null || exercicio.opcoesPort == null ||
            exercicio.opcoesPort.Count == 0 || exercicio.opcoesIngles == null ||
            exercicio.opcoesIngles.Count == 0 ||
            exercicio.associacaoCorreta == null ||
            seletores == null || rotulosPortugues == null ||
            seletores.Length != exercicio.opcoesPort.Count ||
            rotulosPortugues.Length != exercicio.opcoesPort.Count)
        {
            Debug.LogError("Configure os rótulos e seletores de associação no Inspector para cada opção em português.");
            return;
        }

        foreach (string palavraPortugues in exercicio.opcoesPort)
        {
            bool encontrouPar = false;
            foreach (ParAssociacao par in exercicio.associacaoCorreta)
            {
                if (par != null && par.palavraPortugues == palavraPortugues &&
                    exercicio.opcoesIngles.Contains(par.palavraIngles))
                {
                    encontrouPar = true;
                    break;
                }
            }

            if (!encontrouPar)
            {
                Debug.LogError("O gabarito de associação não contém um par válido para: " + palavraPortugues);
                return;
            }
        }

        List<string> opcoes = new List<string> { "Choose a translation" };
        opcoes.AddRange(exercicio.opcoesIngles);

        for (int i = 0; i < exercicio.opcoesPort.Count; i++)
        {
            if (seletores[i] == null || rotulosPortugues[i] == null)
            {
                Debug.LogError("Todos os seletores e rótulos do exercício de associação precisam estar atribuídos.");
                return;
            }

            rotulosPortugues[i].text = exercicio.opcoesPort[i];
            seletores[i].ClearOptions();
            seletores[i].AddOptions(opcoes);
            seletores[i].value = 0;
            seletores[i].RefreshShownValue();
            seletores[i].interactable = true;
        }

        configuracaoValida = true;

        if (textoFeedback != null)
            textoFeedback.text = string.Empty;
    }

    public bool Respondeu()
    {
        if (!configuracaoValida || seletores == null)
            return false;

        foreach (TMP_Dropdown seletor in seletores)
        {
            if (seletor == null || seletor.value == 0)
                return false;
        }

        return true;
    }

    public bool EstaCorreto()
    {
        if (!configuracaoValida || exercicioAtual == null || seletores == null || !Respondeu())
            return false;

        for (int i = 0; i < exercicioAtual.opcoesPort.Count; i++)
        {
            string palavraInglesCorreta = null;
            foreach (ParAssociacao par in exercicioAtual.associacaoCorreta)
            {
                if (par.palavraPortugues == exercicioAtual.opcoesPort[i])
                {
                    palavraInglesCorreta = par.palavraIngles;
                    break;
                }
            }

            if (palavraInglesCorreta == null ||
                seletores[i].options[seletores[i].value].text != palavraInglesCorreta)
                return false;
        }

        return true;
    }

    public void ReduzirTentativa()
    {
        tentativasRestantes--;
    }

    public int ObterTentativasRestantes()
    {
        return tentativasRestantes;
    }

    public void MostrarFeedback(string mensagem)
    {
        if (textoFeedback != null)
            textoFeedback.text = mensagem;
    }

    public void BloquearSeletores()
    {
        if (seletores == null)
            return;

        foreach (TMP_Dropdown seletor in seletores)
        {
            if (seletor != null)
                seletor.interactable = false;
        }
    }
}
