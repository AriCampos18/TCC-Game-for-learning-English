using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
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
    private bool barraInicializada;

    private readonly Dictionary<string, JObject> chavesCheckpoint = new Dictionary<string, JObject>();

    internal JArray CapturarCheckpoint() => new JArray(chavesCheckpoint.Values.Select(c => c.DeepClone()));

    internal void RestaurarCheckpoint(JArray chaves)
    {
        InicializarBarra();
        foreach (JObject chave in chaves)
        {
            chavesCheckpoint[CheckpointJogo.IdentidadeExercicio(chave)] = (JObject)chave.DeepClone();
            if ((int?)chave["exercise_id"] is int id)
                idsJaPontuados.Add(id);
        }
        exerciciosAcertados = Mathf.Min(chavesCheckpoint.Count, totalExercicios);
        AtualizarVisual();
    }

    private readonly HashSet<int> idsJaPontuados = new HashSet<int>();

    private HashSet<ExercicioBase> exerciciosJaPontuados = new HashSet<ExercicioBase>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (!barraInicializada)
            InicializarBarra();
    }

    public void InicializarBarra()
    {
        barraInicializada = true;
        nivelAtual = DadosJogador.nivelUsuario;
        Debug.Log("Nível atual da barra: " + DadosJogador.nivelUsuario);

        exerciciosAcertados = 0;
        exerciciosJaPontuados.Clear();
        idsJaPontuados.Clear();
        chavesCheckpoint.Clear();

        totalExercicios = nivelAtual switch
        {
            "A1" => 14,
            "A2" => 16,
            _ => 12
        };

        var (inicio, fim) = nivelAtual switch
        {
            "A1" => ("A1", "A1-A2"),
            "A1-A2" => ("A1-A2", "A2"),
            "A2" => ("A2", "A2-B1"),
            "A2-B1" => ("A2-B1", "B1"),
            "B1" => ("B1", ""),
            _ => ("", "")
        };
        ExibirRotulo(textoInicioNivel, inicio);
        ExibirRotulo(textoFimNivel, fim);
        AtualizarVisual();
    }

    private static void ExibirRotulo(TextMeshProUGUI rotulo, string nivel)
    {
        if (rotulo == null)
            return;

        if (!rotulo.enableAutoSizing)
        {
            rotulo.fontSizeMax = rotulo.fontSize;
            rotulo.enableAutoSizing = true;
        }
        rotulo.textWrappingMode = TextWrappingModes.NoWrap;
        rotulo.text = nivel;
    }

    public void RegistrarAcerto(ExercicioBase exercicio, InteracaoNPC origem = null)
    {
        var sessao = SessaoCenario.Atual;
        if (!sessao.EmJogo || exercicio == null)
            return;

        var chave = CheckpointJogo.ChaveExercicio(exercicio, origem, incluirCenario: true);
        string identidade = chave != null ? CheckpointJogo.IdentidadeExercicio(chave) : null;
        if (identidade != null && chavesCheckpoint.ContainsKey(identidade))
            return;

        // O catálogo executável é recriado a cada visita. Mantém a pontuação por exercício persistido.
        bool primeiroAcerto = exercicio.idExercise > 0
            ? idsJaPontuados.Add(exercicio.idExercise)
            : exerciciosJaPontuados.Add(exercicio);
        if (!primeiroAcerto)
            return;

        if (identidade != null)
            chavesCheckpoint.Add(identidade, chave);
        exerciciosAcertados = Mathf.Min(exerciciosAcertados + 1, totalExercicios);
        AtualizarVisual();
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
