using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;

public class ModalNivelmanetoScript : MonoBehaviour
{
    public Button botaoNext, botaoPular;
    public List<string> perguntasDiscursivas;
    public List<ExercicioBase> perguntasObjetivas;
    private int perguntaAtual = 0;
    public TextMeshProUGUI textoPergunta, tituloPergunta;
    public TMP_InputField campoResposta;
    public GameObject modalRespostaNivelamento;
    private BackendManager backendManager;
    public GameObject nivelamentoObjetivo;
    public GameObject nivelamentoDiscursivo;
    public List<RespostaNivelamento> respostasNivelamento;
    public GameObject modalCarregando;

    public GameObject modalAvisoCampoVazio;

    private int indiceDiscursiva = 0;
    private int indiceObjetiva = 0;
    private int totalPerguntas;
    private NivelamentoObjetivoScript nivelamentoObjetivoScript;
    private ModalRespostaNivelamentoScript modalRespostaScript;

    // Start is called before the first frame update
    void Start()
    {
        backendManager = new BackendManager();
        nivelamentoObjetivoScript = nivelamentoObjetivo.GetComponent<NivelamentoObjetivoScript>();
        modalRespostaScript =
        modalRespostaNivelamento.GetComponent<ModalRespostaNivelamentoScript>();
        perguntasObjetivas = new List<ExercicioBase>();
        respostasNivelamento = new List<RespostaNivelamento>();
        nivelamentoDiscursivo.SetActive(true);
        nivelamentoObjetivo.SetActive(false);
        perguntasDiscursivas = new List<string>
        {
            "Hello. What is your name?",
            "Which do you prefer: to go to the mall or to go to the park?",
            "What will you eat this evening?",
            "What do you usually do on weekends?",
            "Why do you want to learn English?",
            "Which is more interesting for you: studying English online or in a classroom? Why?",
            "Describe your daily routine."
        };

        perguntasObjetivas.Add(
            new ExercicioAlternativas()
            {
                enunciado = "Is there a supermarket near your house? Select the correct answer.",
                alternativas = new List<string>() { "Yes, there is one two blocks away.",
                    "Yes, I usually go there on Saturdays.",
                    "Yes, my house is very comfortable." },
                alternativaCorreta = 0
            }
        );

        perguntasObjetivas.Add(
            new ExercicioAlternativas()
            {
                enunciado = "Did you and your friends go to the park yesterday, or were you at home? Select the correct answer.",
                alternativas = new List<string>() { "We are going to the park after school today.",
                    "We stayed at home because it was raining.",
                    "We usually go to the park on weekends." },
                alternativaCorreta = 1
            }
        );

        perguntasObjetivas.Add(
            new ExercicioAlternativas()
            {
                enunciado = "Have you ever visited a famous place or eaten a different type of food? Select the right answer.",
                alternativas = new List<string>() { "Yes, I usually eat different food on weekends.",
                    "Yes, I went to a restaurant with my family last month.",
                    "Yes, I�ve tried Mexican food and really liked it." },
                alternativaCorreta = 2
            }
        );

        FirstPlayerController player = FindObjectOfType<FirstPlayerController>();
        if (player != null)
        {
            player.DesativarControle();
        }

        totalPerguntas = perguntasDiscursivas.Count + perguntasObjetivas.Count;

        MostrarPerguntaAtual();

        if(botaoNext != null && botaoPular != null)
        {
            botaoNext.onClick.AddListener(ClickProxima);
            botaoPular.onClick.AddListener(ClickPular);
        }
    }

    public void MostrarPerguntaAtual()
    {
        if(perguntaAtual < totalPerguntas)
        {
            bool mostrarObjetiva = (perguntaAtual > 0) && ((perguntaAtual + 1) % 3 == 0);
            if (mostrarObjetiva)
            {
                MostrarPerguntaObjetivaAtual(); 
            }
            else
            {
                MostrarPerguntaDiscursivaAtual();
            }
        }
        else
        {
            // Todas as perguntas foram respondidas ou puladas, aqui voc� pode processar as respostas
            Debug.Log("Nivelamento finalizado!");
            //backendManager.ProcessarRespostasNivelamento(respostas);
            this.Fechar();
        }
    }

    private void MostrarPerguntaDiscursivaAtual()
    {
        if (indiceDiscursiva >= perguntasDiscursivas.Count)
            return;

        nivelamentoDiscursivo.SetActive(true);
        nivelamentoObjetivo.SetActive(false);

        tituloPergunta.text = $"Question {perguntaAtual + 1}";
        textoPergunta.text = perguntasDiscursivas[indiceDiscursiva];

        campoResposta.text = "";

        indiceDiscursiva++;
    }

    private void MostrarPerguntaObjetivaAtual()
    {
        if (indiceObjetiva >= perguntasObjetivas.Count)
            return;

        nivelamentoObjetivo.SetActive(true);
        nivelamentoDiscursivo.SetActive(false);

        ExercicioAlternativas perguntaObj =
            (ExercicioAlternativas)perguntasObjetivas[indiceObjetiva];

        tituloPergunta.text = $"Question {perguntaAtual + 1}";
        textoPergunta.text = perguntaObj.enunciado;

        nivelamentoObjetivoScript.ConfigurarAlternativas(perguntaObj);

        indiceObjetiva++;
    }

    public void ClickProxima()
    {
        bool respostaValida = true;

        // VALIDA DISCURSIVA
        if (nivelamentoDiscursivo.activeSelf)
        {
            if (string.IsNullOrWhiteSpace(campoResposta.text))
            {
                modalAvisoCampoVazio.SetActive(true);
                respostaValida = false;
            }
        }

        // VALIDA OBJETIVA
        if (nivelamentoObjetivo.activeSelf)
        {
            if (nivelamentoObjetivoScript.GetRespostaSelecionada() == -1)
            {
                modalAvisoCampoVazio.SetActive(true);
                respostaValida = false;
            }
        }

        if (respostaValida)
        {
            if (perguntaAtual == totalPerguntas - 1)
            {
                if (nivelamentoDiscursivo.activeSelf &&
                    !string.IsNullOrEmpty(campoResposta.text))
                {
                    RespostaNivelamento r = new RespostaNivelamento();

                    r.nivel = ObterNivelAtual();
                    r.tipo = "discursiva";
                    r.pergunta = perguntasDiscursivas[indiceDiscursiva - 1];
                    r.respostaUsuario = campoResposta.text;

                    respostasNivelamento.Add(r);

                    campoResposta.text = "";
                }
                else if (nivelamentoObjetivo.activeSelf)
                {
                    ExercicioAlternativas ex =
                        (ExercicioAlternativas)perguntasObjetivas[indiceObjetiva - 1];

                    RespostaNivelamento r = new RespostaNivelamento();

                    r.nivel = ObterNivelAtual();
                    r.tipo = "objetiva";
                    r.pergunta = ex.enunciado;
                    r.alternativas = ex.alternativas;
                    r.alternativaCorreta = ex.alternativaCorreta;
                    r.respostaSelecionada =
                        nivelamentoObjetivoScript.GetRespostaSelecionada();

                    respostasNivelamento.Add(r);
                }

                FinalizarComIA();
            }
            else
            {
                if (nivelamentoDiscursivo.activeSelf &&
                    !string.IsNullOrEmpty(campoResposta.text))
                {
                    RespostaNivelamento r = new RespostaNivelamento();

                    r.nivel = ObterNivelAtual();
                    r.tipo = "discursiva";
                    r.pergunta = perguntasDiscursivas[indiceDiscursiva - 1];
                    r.respostaUsuario = campoResposta.text;

                    respostasNivelamento.Add(r);

                    campoResposta.text = "";
                }
                else if (nivelamentoObjetivo.activeSelf)
                {
                    ExercicioAlternativas ex =
                        (ExercicioAlternativas)perguntasObjetivas[indiceObjetiva - 1];

                    RespostaNivelamento r = new RespostaNivelamento();

                    r.nivel = ObterNivelAtual();
                    r.tipo = "objetiva";
                    r.pergunta = ex.enunciado;
                    r.alternativas = ex.alternativas;
                    r.alternativaCorreta = ex.alternativaCorreta;
                    r.respostaSelecionada =
                        nivelamentoObjetivoScript.GetRespostaSelecionada();

                    respostasNivelamento.Add(r);
                }

                perguntaAtual++;
                MostrarPerguntaAtual();
            }
        }
    }

    public void ClickPular()
    {
        string nivelAtual = ObterNivelAtual();

        Debug.Log("Pergunta pulada no nível: " + nivelAtual);

        // SE PULOU NO A1
        if (nivelAtual == "A1")
        {
            FinalizarA1Direto();
        }

        // SE PULOU NO A2 OU B1
        else
        {
            FinalizarComIA();
        }
    }

    public void Fechar()
    {
        this.gameObject.SetActive(false);
        modalRespostaNivelamento.SetActive(true);
    }

    private string ObterNivelAtual()
    {
        if (perguntaAtual <= 2)
            return "A1";

        if (perguntaAtual <= 5)
            return "A2";

        return "B1";
    }

    private async void FinalizarComIA()
    {
        Debug.Log("Nivelamento finalizado com IA!");

        modalCarregando.SetActive(true);

        try
        {
            if (backendManager == null)
            {
                Debug.LogError("backendManager está NULL");
                return;
            }

            if (modalRespostaScript == null)
            {
                Debug.LogError("modalRespostaScript está NULL");
                return;
            }

            PacoteNivelamento pacote = new PacoteNivelamento();
            pacote.respostas = respostasNivelamento;

            string json =
                JsonConvert.SerializeObject(
                    pacote,
                    Formatting.Indented
                );

            RetornoIANivelamento resultado =
                await backendManager.ProcessarRespostasNivelamento(json);

            DadosJogador.nivelUsuario = resultado.nivel;

            if (ProgressoNivelManager.Instance != null)
            {
                ProgressoNivelManager.Instance.InicializarBarra();
            }

            modalRespostaScript.MostrarResultado(
                resultado.nivel,
                resultado.feedback
            );

            this.gameObject.SetActive(false);
            modalRespostaNivelamento.SetActive(true);
        }
        finally
        {
            modalCarregando.SetActive(false);
        }
    }

    private void FinalizarA1Direto()
    {
        Debug.Log("Usuário ficou no nível A1.");

        // ADICIONE ESTA LINHA PARA SALVAR O NÍVEL CORRETAMENTE:
        DadosJogador.nivelUsuario = "A1"; 

        if (ProgressoNivelManager.Instance != null)
        {
            ProgressoNivelManager.Instance.InicializarBarra();
        }

        modalRespostaScript.MostrarResultado("A1", "Você ficou no nível A1 de inglês. Você está no início da aprendizagem de inglês. As próximas atividades irão ajudar no desenvolvimento do vocabulário, compreensão e conversação.");

        this.gameObject.SetActive(false);
        modalRespostaNivelamento.SetActive(true);
    }
}
