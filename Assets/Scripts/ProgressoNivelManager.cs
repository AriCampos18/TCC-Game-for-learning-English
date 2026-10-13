using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressoNivelManager : MonoBehaviour
{
    public static ProgressoNivelManager Instance;
    public RectTransform fillBarra;
    public float larguraMaxima = 347.616f;
    public TextMeshProUGUI textoInicioNivel;
    public TextMeshProUGUI textoFimNivel;

    private string nivelAtual;
    private int totalExercicios;
    private int exerciciosAcertados;

    private HashSet<ExercicioBase> exerciciosJaPontuados = new HashSet<ExercicioBase>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        InicializarBarra();
    }

    public void InicializarBarra()
    {
        nivelAtual = DadosJogador.nivelUsuario;
        Debug.Log("Nível atual da barra: " + DadosJogador.nivelUsuario);

        exerciciosAcertados = 0;
        exerciciosJaPontuados.Clear();

        if (nivelAtual == "A1")
        {
            totalExercicios = 14;

            if (textoInicioNivel != null)
                textoInicioNivel.text = "A1";

            if (textoFimNivel != null)
                textoFimNivel.text = "A2";
        }
        else if (nivelAtual == "A2")
        {
            totalExercicios = 16;

            if (textoInicioNivel != null)
                textoInicioNivel.text = "A2";

            if (textoFimNivel != null)
                textoFimNivel.text = "B1";
        }
        else
        {
            totalExercicios = 12;

            if (textoInicioNivel != null)
                textoInicioNivel.text = "B1";

            if (textoFimNivel != null)
                textoFimNivel.text = "";
        }

        AtualizarVisual();
    }

    public void RegistrarAcerto(ExercicioBase exercicio)
    {
        if (exercicio != null)
        {
            if (!exerciciosJaPontuados.Contains(exercicio))
            {
                exerciciosJaPontuados.Add(exercicio);
                exerciciosAcertados++;

                if (exerciciosAcertados > totalExercicios)
                {
                    exerciciosAcertados = totalExercicios;
                }

                AtualizarVisual();
            }
        }
    }

    private void AtualizarVisual()
    {
        float progresso = 0f;

        if (totalExercicios > 0)
            progresso = exerciciosAcertados / (float)totalExercicios;

        if (fillBarra != null)
        {
            progresso = Mathf.Clamp01(progresso);

            fillBarra.anchorMin = new Vector2(0f, 0.5f);
            fillBarra.anchorMax = new Vector2(0f, 0.5f);
            fillBarra.pivot = new Vector2(0f, 0.5f);

            fillBarra.anchoredPosition = new Vector2(0f, fillBarra.anchoredPosition.y);

            fillBarra.sizeDelta = new Vector2(
                larguraMaxima * progresso,
                fillBarra.sizeDelta.y
            );
        }
    }
}