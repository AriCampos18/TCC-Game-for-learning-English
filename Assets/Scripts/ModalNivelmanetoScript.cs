using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using System.Threading.Tasks;

public class ModalNivelmanetoScript : MonoBehaviour
{
    private const string RespostaNaoSoube = "Nao soube responder";

    private enum TipoPerguntaNivelamento
    {
        Discursiva,
        Speaking,
        Objetiva
    }

    public Button botaoNext, botaoPular, botaoNaoConsigoFalarAgora;
    public List<string> perguntasDiscursivas;
    public List<ExercicioBase> perguntasObjetivas;
    public List<string> perguntasSpeaking;
    private int perguntaAtual = 0;
    public TextMeshProUGUI textoPergunta, tituloPergunta;
    public TMP_InputField campoResposta;
    public GameObject modalRespostaNivelamento;
    private BackendManager backendManager;
    public GameObject nivelamentoObjetivo;
    public GameObject nivelamentoDiscursivo;
    public GameObject nivelamentoSpeaking;
    public SpeakingUI speakingUI;
    public List<RespostaNivelamento> respostasNivelamento;
    public GameObject modalCarregando;

    public GameObject modalAvisoCampoVazio;
    public ModalErroSpeaking modalErroSpeaking;

    private int indiceDiscursiva = 0;
    private int indiceObjetiva = 0;
    private int indiceSpeaking = 0;
    private int totalPerguntas;
    private List<TipoPerguntaNivelamento> ordemPerguntas;
    private NivelamentoObjetivoScript nivelamentoObjetivoScript;
    private ModalRespostaNivelamentoScript modalRespostaScript;
    private readonly List<byte[]> audiosSpeaking = new List<byte[]>();
    private bool finalizando;

    // Start is called before the first frame update
    void Start()
    {
        backendManager = new BackendManager();
        nivelamentoObjetivoScript = nivelamentoObjetivo.GetComponent<NivelamentoObjetivoScript>();
        modalRespostaScript =
        modalRespostaNivelamento.GetComponent<ModalRespostaNivelamentoScript>();
        perguntasObjetivas = new List<ExercicioBase>();
        respostasNivelamento = new List<RespostaNivelamento>();
        audiosSpeaking.Clear();
        nivelamentoDiscursivo.SetActive(true);
        nivelamentoObjetivo.SetActive(false);

        if (speakingUI == null)
        {
            SpeakingUI[] componentesSpeaking = FindObjectsOfType<SpeakingUI>(true);
            if (componentesSpeaking.Length == 1)
            {
                speakingUI = componentesSpeaking[0];
            }
            else if (componentesSpeaking.Length > 1)
            {
                Debug.LogError("Há mais de um SpeakingUI na cena. Atribua o SpeakingUI correto ao modal de nivelamento.");
            }
        }

        if (nivelamentoSpeaking == null && speakingUI != null)
        {
            nivelamentoSpeaking = speakingUI.gameObject;
        }

        perguntasSpeaking = new List<string>
        {
            "Hello. What is your name?",
            "What will you eat this evening?",
            "What do you usually do on weekends?",
            "Describe your daily routine."
        };

        perguntasDiscursivas = new List<string>
        {
            "Which do you prefer: to go to the mall or to go to the park?",
            "Why do you want to learn English?",
            "Which is more interesting for you: studying English online or in a classroom? Why?"
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
                alternativas = new List<string>() {
                "Yes, we are going to the park tomorrow.",
                "We went to the park yesterday.",
                "We usually stay home on weekends." },
                alternativaCorreta = 1
            }
        );

        perguntasObjetivas.Add(
            new ExercicioAlternativas()
            {
                enunciado = "Have you ever visited a famous place or eaten a different type of food? Select the right answer.",
                alternativas = new List<string>() { "Yes, I usually eat different food on weekends.",
                    "Yes, I went to a restaurant with my family last month.",
                    "Yes, I've tried Mexican food and really liked it." },
                alternativaCorreta = 2
            }
        );

        FirstPlayerController player = FindObjectOfType<FirstPlayerController>();
        if (player != null)
        {
            player.DesativarControle();
        }

        ordemPerguntas = CriarOrdemPerguntas();
        totalPerguntas = ordemPerguntas.Count;

        MostrarPerguntaAtual();

        if (botaoNext != null)
            botaoNext.onClick.AddListener(ClickProxima);

        if (botaoPular != null)
            botaoPular.onClick.AddListener(ClickPular);

        if (botaoNaoConsigoFalarAgora != null)
            botaoNaoConsigoFalarAgora.onClick.AddListener(ClickNaoConsigoFalarAgora);
        else
            Debug.LogWarning("Atribua o botão 'Não consigo falar agora' ao ModalNivelmanetoScript no Inspector.");
        
        if (speakingUI != null && modalErroSpeaking != null)
    {
        speakingUI.modalErroSpeaking = modalErroSpeaking;
        modalErroSpeaking.speakingUI = speakingUI;
        modalErroSpeaking.nivelamentoScript = this;
    }
    }

    private List<TipoPerguntaNivelamento> CriarOrdemPerguntas()
    {
        List<TipoPerguntaNivelamento> ordem = new List<TipoPerguntaNivelamento>();
        int maiorQuantidade = Mathf.Max(
            perguntasDiscursivas.Count,
            Mathf.Max(perguntasSpeaking.Count, perguntasObjetivas.Count)
        );

        for (int i = 0; i < maiorQuantidade; i++)
        {
            if (i < perguntasDiscursivas.Count)
                ordem.Add(TipoPerguntaNivelamento.Discursiva);

            if (i < perguntasSpeaking.Count)
                ordem.Add(TipoPerguntaNivelamento.Speaking);

            if (i < perguntasObjetivas.Count)
                ordem.Add(TipoPerguntaNivelamento.Objetiva);
        }

        return ordem;
    }

    public void MostrarPerguntaAtual()
    {
        if (perguntaAtual < totalPerguntas)
        {
            switch (ordemPerguntas[perguntaAtual])
            {
                case TipoPerguntaNivelamento.Discursiva:
                    MostrarPerguntaDiscursivaAtual();
                    break;
                case TipoPerguntaNivelamento.Speaking:
                    MostrarPerguntaSpeakingAtual();
                    break;
                case TipoPerguntaNivelamento.Objetiva:
                    MostrarPerguntaObjetivaAtual();
                    break;
            }
        }
        else
        {
            FinalizarComIA();
        }
    }

    private void MostrarPerguntaDiscursivaAtual()
    {
        if (indiceDiscursiva >= perguntasDiscursivas.Count)
            return;

        nivelamentoDiscursivo.SetActive(true);
        nivelamentoObjetivo.SetActive(false);
        if (nivelamentoSpeaking != null)
            nivelamentoSpeaking.SetActive(false);

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
        if (nivelamentoSpeaking != null)
            nivelamentoSpeaking.SetActive(false);

        ExercicioAlternativas perguntaObj =
            (ExercicioAlternativas)perguntasObjetivas[indiceObjetiva];

        tituloPergunta.text = $"Question {perguntaAtual + 1}";
        textoPergunta.text = perguntaObj.enunciado;

        nivelamentoObjetivoScript.ConfigurarAlternativas(perguntaObj);

        indiceObjetiva++;
    }

    private void MostrarPerguntaSpeakingAtual()
    {
        if (indiceSpeaking >= perguntasSpeaking.Count || speakingUI == null || nivelamentoSpeaking == null)
        {
            Debug.LogError("Não foi possível exibir a pergunta de speaking: configure o SpeakingUI e o painel de speaking.");
            return;
        }

        nivelamentoObjetivo.SetActive(false);
        nivelamentoDiscursivo.SetActive(false);
        nivelamentoSpeaking.SetActive(true);
        speakingUI.PrepararParaNivelamento();

        tituloPergunta.text = $"Question {perguntaAtual + 1}";
        textoPergunta.text = perguntasSpeaking[indiceSpeaking];

        indiceSpeaking++;
    }

    public void ClickProxima()
    {
        if (finalizando || !ValidarRespostaAtual() || !RegistrarRespostaAtual())
            return;

        AvancarPergunta();
    }

    public void ClickNaoConsigoFalarAgora()
    {
        if (finalizando || nivelamentoSpeaking == null || !nivelamentoSpeaking.activeSelf)
            return;

        if (speakingUI != null)
            speakingUI.CancelarGravacao();

        AvancarPergunta();
    }

    private void AvancarPergunta()
    {
        perguntaAtual++;

        if (perguntaAtual >= totalPerguntas)
        {
            FinalizarComIA();
            return;
        }

        MostrarPerguntaAtual();
    }

    private bool RegistrarPerguntaPulada()
    {
        RespostaNivelamento resposta = new RespostaNivelamento
        {
            nivel = ObterNivelAtual(),
            respostaUsuario = RespostaNaoSoube
        };

        if (nivelamentoDiscursivo.activeSelf)
        {
            resposta.tipo = "discursiva";
            resposta.pergunta = perguntasDiscursivas[indiceDiscursiva - 1];
        }
        else if (nivelamentoObjetivo.activeSelf)
        {
            ExercicioAlternativas exercicio =
                (ExercicioAlternativas)perguntasObjetivas[indiceObjetiva - 1];
            resposta.tipo = "objetiva";
            resposta.pergunta = exercicio.enunciado;
            resposta.alternativas = exercicio.alternativas;
            resposta.alternativaCorreta = exercicio.alternativaCorreta;
            resposta.respostaSelecionada = -1;
        }
        else if (nivelamentoSpeaking != null && nivelamentoSpeaking.activeSelf)
        {
            resposta.tipo = "speaking";
            resposta.pergunta = perguntasSpeaking[indiceSpeaking - 1];
            audiosSpeaking.Add(null);
        }
        else
        {
            Debug.LogError("Não foi possível registrar a pergunta pulada: nenhum painel está ativo.");
            return false;
        }

        respostasNivelamento.Add(resposta);
        return true;
    }

    private bool ValidarRespostaAtual()
    {
        bool respostaValida = true;

        if (nivelamentoDiscursivo.activeSelf)
        {
            respostaValida = campoResposta != null &&
                !string.IsNullOrWhiteSpace(campoResposta.text);
        }
        else if (nivelamentoObjetivo.activeSelf)
        {
            respostaValida = nivelamentoObjetivoScript != null &&
                nivelamentoObjetivoScript.GetRespostaSelecionada() != -1;
        }
        else if (nivelamentoSpeaking != null && nivelamentoSpeaking.activeSelf)
        {
            byte[] audio = speakingUI != null ? speakingUI.ObterAudioWav() : null;
            respostaValida = audio != null && audio.Length > 0;
        }
        else
        {
            Debug.LogError("Nenhum painel de pergunta está ativo no nivelamento.");
            return false;
        }

        if (!respostaValida && modalAvisoCampoVazio != null)
            modalAvisoCampoVazio.SetActive(true);

        return respostaValida;
    }

    private bool RegistrarRespostaAtual()
    {
        RespostaNivelamento resposta = new RespostaNivelamento
        {
            nivel = ObterNivelAtual()
        };

        if (nivelamentoDiscursivo.activeSelf)
        {
            resposta.tipo = "discursiva";
            resposta.pergunta = perguntasDiscursivas[indiceDiscursiva - 1];
            resposta.respostaUsuario = campoResposta.text.Trim();
            campoResposta.text = "";
        }
        else if (nivelamentoObjetivo.activeSelf)
        {
            ExercicioAlternativas exercicio =
                (ExercicioAlternativas)perguntasObjetivas[indiceObjetiva - 1];
            resposta.tipo = "objetiva";
            resposta.pergunta = exercicio.enunciado;
            resposta.alternativas = exercicio.alternativas;
            resposta.alternativaCorreta = exercicio.alternativaCorreta;
            resposta.respostaSelecionada =
                nivelamentoObjetivoScript.GetRespostaSelecionada();
        }
        else
        {
            byte[] audio = speakingUI.ObterAudioWav();
            if (audio == null || audio.Length == 0)
            {
                Debug.LogError("O áudio da resposta de speaking não está disponível.");
                return false;
            }

            resposta.tipo = "speaking";
            resposta.pergunta = perguntasSpeaking[indiceSpeaking - 1];
            audiosSpeaking.Add(audio);
        }

        respostasNivelamento.Add(resposta);
        return true;
    }

    public void ClickPular()
    {
        if (finalizando)
            return;

        if (nivelamentoSpeaking != null && nivelamentoSpeaking.activeSelf && speakingUI != null)
            speakingUI.CancelarGravacao();

        if (!RegistrarPerguntaPulada())
            return;

        string nivelAtual = ObterNivelAtual();

        Debug.Log("Pergunta pulada no nível: " + nivelAtual);

        if (nivelAtual == "A1")
        {
            FinalizarA1Direto();
            return;
        }

        FinalizarComIA();
    }

    public void Fechar()
    {
        if (nivelamentoSpeaking != null)
            nivelamentoSpeaking.SetActive(false);

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
        if (finalizando)
            return;

        finalizando = true;
        Debug.Log("Nivelamento finalizado com IA!");

        if (modalCarregando != null)
            modalCarregando.SetActive(true);

        if (botaoNext != null)
            botaoNext.interactable = false;
        if (botaoPular != null)
            botaoPular.interactable = false;

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

            if (!await TranscreverRespostasSpeaking())
                return;

            PacoteNivelamento pacote = new PacoteNivelamento();
            pacote.respostas = respostasNivelamento;

            string json =
                JsonConvert.SerializeObject(
                    pacote,
                    Formatting.Indented
                );

            RetornoIANivelamento resultado = await backendManager.ProcessarRespostasNivelamento(json);

            if (resultado == null)
            {
                Debug.LogError("Resultado da IA veio NULL. Verifique se o backend/LM Studio está rodando e se retornou JSON válido.");
                return;
            }

            if (string.IsNullOrEmpty(resultado.nivel))
            {
                Debug.LogError("Resultado da IA veio sem nível.");
                return;
            }

            DadosJogador.nivelUsuario = resultado.nivel;
            if (ProgressoNivelManager.Instance != null)
            {
                ProgressoNivelManager.Instance.InicializarBarra();
            }

            modalRespostaScript.MostrarResultado(
                resultado.nivel,
                resultado.feedback
            );

            if (nivelamentoSpeaking != null)
                nivelamentoSpeaking.SetActive(false);

            this.gameObject.SetActive(false);
            modalRespostaNivelamento.SetActive(true);
        }
        finally
        {
            if (modalCarregando != null)
                modalCarregando.SetActive(false);

            finalizando = false;
            if (gameObject.activeInHierarchy)
            {
                if (botaoNext != null)
                    botaoNext.interactable = true;
                if (botaoPular != null)
                    botaoPular.interactable = true;
            }
        }
    }

    private async Task<bool> TranscreverRespostasSpeaking()
    {
        int indiceAudio = 0;
        int totalRespostasSpeaking = 0;
        foreach (RespostaNivelamento resposta in respostasNivelamento)
        {
            if (resposta.tipo == "speaking")
                totalRespostasSpeaking++;
        }

        if (totalRespostasSpeaking != audiosSpeaking.Count)
        {
            Debug.LogError("A quantidade de respostas de speaking não corresponde à quantidade de áudios gravados.");
            return false;
        }

        for (int i = 0; i < respostasNivelamento.Count; i++)
        {
            RespostaNivelamento resposta = respostasNivelamento[i];
            if (resposta.tipo != "speaking")
                continue;

            if (string.IsNullOrWhiteSpace(resposta.respostaUsuario))
            {
                if (audiosSpeaking[indiceAudio] == null)
                {
                    Debug.LogError($"Não há áudio gravado para a pergunta de speaking: {resposta.pergunta}");
                    return false;
                }

                string jsonTranscricao = await backendManager.EnviarAudioWhisperFast(
                    audiosSpeaking[indiceAudio],
                    JsonConvert.SerializeObject(new List<string>())
                );

                if (string.IsNullOrWhiteSpace(jsonTranscricao))
                {
                    Debug.LogError($"Não foi possível transcrever o áudio da pergunta: {resposta.pergunta}");
                    return false;
                }

                try
                {
                    SpeakingResult resultado = JsonConvert.DeserializeObject<SpeakingResult>(jsonTranscricao);
                    if (resultado == null || string.IsNullOrWhiteSpace(resultado.transcricao))
                    {
                        Debug.LogError($"O backend não retornou uma transcrição válida para a pergunta: {resposta.pergunta}");
                        return false;
                    }

                    resposta.respostaUsuario = resultado.transcricao.Trim();
                }
                catch (JsonException ex)
                {
                    Debug.LogError($"Resposta inválida do backend de transcrição: {ex.Message}");
                    return false;
                }
            }

            indiceAudio++;
        }

        return true;
    }

    private void FinalizarA1Direto()
    {
        Debug.Log("Usuário ficou no nível A1.");
        DadosJogador.nivelUsuario = "A1";

        if (ProgressoNivelManager.Instance != null)
            ProgressoNivelManager.Instance.InicializarBarra();

        modalRespostaScript.MostrarResultado(
            "A1",
            "Você ficou no nível A1 de inglês. Você está no início da aprendizagem de inglês. As próximas atividades irão ajudar no desenvolvimento do vocabulário, compreensão e conversação."
        );

        if (nivelamentoSpeaking != null)
            nivelamentoSpeaking.SetActive(false);

        gameObject.SetActive(false);
        modalRespostaNivelamento.SetActive(true);
    }

    public void DescartarSpeakingAtual()
    {
        if (finalizando ||
            nivelamentoSpeaking == null ||
            !nivelamentoSpeaking.activeSelf)
        {
            return;
        }

        if (speakingUI != null)
            speakingUI.CancelarGravacao();

        if (modalAvisoCampoVazio != null)
            modalAvisoCampoVazio.SetActive(false);

        // Não registra a pergunta nem o áudio descartado.
        // Apenas avança para a próxima pergunta do nivelamento.
        AvancarPergunta();
    }
}
