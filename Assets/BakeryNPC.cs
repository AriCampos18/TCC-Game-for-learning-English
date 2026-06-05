using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using TMPro;

public enum TipoExercicio
{
    Speaking,
    Alternativas,
    Blocos
}

public class BakeryNPC : InteracaoNPC
{
    List<string> dialogoAtual;
    private Animator animator;


    string nivelAtual;
    private AudioSource audioSource;
    public ModalExercicio modalExercicio;
    public string idNpcParaVoz = "mulher"; // Altere no Inspector para "homem_caixa" ou "mulher_padaria"
    public string nomeExibicaoLegenda = "Bakery Attendant";
    private MissionManager missaoManager;
    public string ultimaFraseDita = "";
    public List<ExercicioBase> exerciciosSpeaking;

    public TextMeshProUGUI textoPularDialogo;
    public List<ExercicioBase> exerciciosAlternativas;

    public List<ExercicioBase> exerciciosBlocos;
    private ExercicioBase exAtual;
    private BackendManager backendManager;

    public GameObject modalLegenda;

    protected override void Start()
    {
        base.Start();
        backendManager = new BackendManager();
        audioSource = GetComponent<AudioSource>();
        exerciciosSpeaking = new List<ExercicioBase>();
        exerciciosAlternativas = new List<ExercicioBase>();
        animator = GetComponent<Animator>();
        exerciciosBlocos = new List<ExercicioBase>();
    }

    private void InicializarConteudosPorNivel()
    {
        if (nivelAtual == "A1")
        {
            dialogoAtual = new List<string>()
            {
                "Hello. What are you looking for?",
                "Do you want anything to drink?",
                "Do you want anything else?",
                "We have chicken sandwich and chocolate cake",
                "Here is your order",
                "You are welcome"
            };

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "Say the right sentence:",
                    opcoesFala = new List<string>() {
                        "There is a table.\n",
                        "I'm looking for some bread.\n",
                        "He goes to school.\r\n"
                    },
                    respostaCorreta = 1,
                } 
            );

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "Say the right sentence:",
                    opcoesFala = new List<string>()
                    {
                        "Iam at the bakery.\n",
                        "There are three chairs.\n",
                        "Yes, I want some juice.\n"
                    },
                    respostaCorreta = 2,
                });

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "Say the right sentence:",
                    opcoesFala = new List<string>()
                    {
                        "She has a small dog.\n",
                        "Yes, I want a cake and a sandwich.\n",
                        "My favorite color is blue.\n"
                    },
                    respostaCorreta = 1,
                });

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "Say the phrase to express gratitude.:",
                    opcoesFala = new List<string>()
                    {
                        "Thank you.\n"
                    },
                    respostaCorreta = 0,
                });

            exerciciosAlternativas.Add(
                new ExercicioAlternativas()
                {
                    enunciado = "What do they have at the bakery?",
                    alternativas = new List<string>()
                    {
                        "Apple pie and orange juice\r\n",
                        "Chicken sandwich and chocolate cake\r\n",
                        "Cheese sandwich and strawberry cake\r\n",
                        "Cheese pizza and ice cream\r\n"
                    },
                    alternativaCorreta = 1
                });
        }
        else if (nivelAtual == "A2")
        {
            dialogoAtual = new List<string>()
            {
                "Hello! What are you looking for in the bakery section?",
                "Yes, we have. Which sandwich can I prepare for you, and would you like some juice too?",
                "Can I help you with anything else, or do you already have everything you need?",
                "Here is your order. You are welcome."
            };

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "Say the right sentence from the list:",
                    opcoesFala = new List<string>() {
                        "She can play the guitar\r\n",
                        "Are there any fresh croissants and sandwiches today?\r\n",
                        "I usually wake up at 7 a.m\r\n",
                    },
                    respostaCorreta = 1,
                }
            );

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "Say the right sentence from the list:",
                    opcoesFala = new List<string>()
                    {
                        "He watches TV every night.",
                        "I’d like the chicken sandwich and some orange juice.",
                        "There is a woman near the window."
                    },
                    respostaCorreta = 1
                }
            );

             exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "Say the phrase to express gratitude:",
                    opcoesFala = new List<string>()
                    {
                        "Thanks a lot for your assistance."
                    },
                    respostaCorreta = 0
                }
            );

            exerciciosBlocos.Add(
                new ExercicioBlocos() 
                { 
                    enunciado = "Arrange the words to form the correct sentence in english: Não, isso é tudo que eu precisava, obrigado/a.",
                    blocosPalavras = new List<string>() { 
                        "No", "milk", "thank", "you", "that", "needed", "was",
                        "same", "all", "hello", "thank", "I", "maybe", "ok"
                    },
                    respostaCorreta = "No, that was all I needed, thank you.",
                }
            );
        }
        else
        {
            dialogoAtual = new List<string>()
            {
                "Hello! What are you looking for today, and would you like me to recommend something popular as well?",
                "Yes, there are still a few chocolate croissants available and cheese sandwiches.",
                "Here is your order. Enjoy your food.",
                "You are welcome. Have a nice day."
            };

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "How should you like to respond? Say the right answer.",
                    opcoesFala = new List<string>() {
                        "There is a supermarket across the street from my house. \r\n \r\n",
                        "Are there any freshly baked pastries left on the shelf near the window, or have they all been sold already? \r\n",
                        "He doesn’t enjoy crowded places either.\r\n"
                    },
                    respostaCorreta = 1
                }
            );

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "Say the phrase to express gratitude:",
                    opcoesFala = new List<string>() {
                        "You've been very helpful today. Really thank you. \r\n"
                    },
                    respostaCorreta = 0
                }
            );

            exerciciosBlocos.Add(
                new ExercicioBlocos() 
                { 
                    enunciado = "Arrange the words to form the correct sentence in english: Ótimo, parece incrível. Eu vou querer o croissant and the sandwich.",
                    blocosPalavras = new List<string>() { 
                        "No", "milk", "Great", "the", "you", "sandwich", "the",
                        "needed", "is", "nice", "amazing", "same", "I'll", "hello",
                        "have", "bread", "I", "that", "croissant", "ok", "looks", "and"
                    },
                    respostaCorreta = "Great, that looks amazing. I'll have the croissant and the sandwich.",
                }
            );

            exerciciosAlternativas.Add(
                new ExercicioAlternativas()
                {
                    enunciado = "Which details are correct about today’s bakery products?",
                    alternativas = new List<string>()
                    {
                        "The sandwiches are made with chicken.\r\n",
                        "The bakery only has apple pies today.\r\n",
                        "The bread was sold out this morning.\r\n",
                        "The chocolate croissants are still available.\r\n"
                    },
                    alternativaCorreta = 3
                });
        }
    }

    protected override async Task IniciarInteracao()
    {
        // Ativa o estado de diálogo logo no começo
        GameProgress.EstaEmDialogo = true;
        nivelAtual = DadosJogador.nivelUsuario;
        InicializarConteudosPorNivel();

        if (nivelAtual == "A1")
        {
            await interacaoA1();
        }
        else if (nivelAtual == "A2")
        {
            await interacaoA2();
        }
        else
        {
            await interacaoB1();
        }
        GameProgress.Instance.falouPadaria = true;

        // Desativa o estado quando a conversa acabar completamente!
        GameProgress.EstaEmDialogo = false;
        MissionManager.Instance.ConcluirMissao("falar_padaria");
        bool missoesConcluidas = missaoManager.MissaoConcluida("falar_atendente");
        if(missoesConcluidas)
        {
            MissionManager.Instance.AdicionarMissao(
                "falar_caixa",
                "Put the products on the counter to start the conversation with the cashier",
                "Coloque os produtos no caixa para iniciar a conversa com o caixa"
            );
        }
    }

    private async Task interacaoA1()
    {
        int i = 0;

        // Toca áudio e mostra a legenda ao mesmo tempo de forma síncrona
        await PlayAudioETexto(i++, mostrarLegenda: true);

        exAtual = exerciciosSpeaking[0];
        await AbrirExercicio(TipoExercicio.Speaking, exAtual);

        await PlayAudioETexto(i++, mostrarLegenda: true);

        exAtual = exerciciosSpeaking[1];
        await AbrirExercicio(TipoExercicio.Speaking, exAtual);

        await PlayAudioETexto(i++, mostrarLegenda: true);

        exAtual = exerciciosSpeaking[2];
        await AbrirExercicio(TipoExercicio.Speaking, exAtual);

        await PlayAudioETexto(i++, mostrarLegenda: true);

        exAtual = exerciciosAlternativas[0];
        await AbrirExercicio(TipoExercicio.Alternativas, exAtual);

        await PlayAudioETexto(i++, mostrarLegenda: true);

        exAtual = exerciciosSpeaking[3];
        await AbrirExercicio(TipoExercicio.Speaking, exAtual);

        await PlayAudioETexto(i++, mostrarLegenda: true);
    }

    private async Task interacaoA2()
    {
        int i = 0;
        
        await PlayAudioETexto(i++, mostrarLegenda: true);
        
        exAtual = exerciciosSpeaking[0];
        await AbrirExercicio(TipoExercicio.Speaking, exAtual);

        await PlayAudioETexto(i++, mostrarLegenda: true);
        
        exAtual = exerciciosSpeaking[1];
        await AbrirExercicio(TipoExercicio.Speaking, exAtual);

        await PlayAudioETexto(i++, mostrarLegenda: true);

        await AbrirExercicio(TipoExercicio.Blocos, exerciciosBlocos[0]);

        await PlayAudioETexto(i++, mostrarLegenda: true);

        exAtual = exerciciosSpeaking[2];
        await AbrirExercicio(TipoExercicio.Speaking, exAtual);

        await PlayAudioETexto(i++, mostrarLegenda: true);
 
    }

    private async Task interacaoB1()
    {
        int i = 0;

        await PlayAudioETexto(i++, mostrarLegenda: true);

        exAtual = exerciciosSpeaking[0];
        await AbrirExercicio(TipoExercicio.Speaking, exAtual);

        await PlayAudioETexto(i++, mostrarLegenda: true);

        exAtual = exerciciosBlocos[0];
        await AbrirExercicio(TipoExercicio.Blocos, exAtual);

        exAtual = exerciciosAlternativas[0];
        await AbrirExercicio(TipoExercicio.Alternativas, exAtual);

        await PlayAudioETexto(i++, mostrarLegenda: true);

        exAtual = exerciciosSpeaking[1];
        await AbrirExercicio(TipoExercicio.Speaking, exAtual);

        await PlayAudioETexto(i++, mostrarLegenda: true);
    }

    public async Task AbrirExercicio(TipoExercicio tipo, ExercicioBase ex)
    {
        Debug.Log("Abrindo exercício...");

        ModalLegenda legenda = modalLegenda.GetComponent<ModalLegenda>();

        // Removeu a linha antiga: legenda.MoverParaExercicio();

        if (modalLegenda != null)
        {
            modalLegenda.SetActive(false);
        }

        await EntrarModoExercicio();

        modalExercicio.Abrir(tipo, ex);

        // ✨ Força a legenda a recalcular a posição agora que o modal abriu de fato
        if (legenda != null) 
        {
            legenda.AjustarPosicaoPeloEstadoDoJogo(); 
        }

        while (!modalExercicio.exercicioFinalizado)
        {
            await Task.Yield();
        }

        if (modalLegenda != null)
        {
            modalLegenda.SetActive(false);
        }

        await SairModoExercicio();

        // ✨ Força a legenda a voltar para o centro agora que o modal fechou de fato
        if (legenda != null)
        {
            legenda.AjustarPosicaoPeloEstadoDoJogo();
        }
    }

    private async Task PlayAudioETexto(int i, bool mostrarLegenda = true)
{
    ultimaFraseDita = dialogoAtual[i];
    // 1. PASSANDO O ID DO NPC JUNTO COM O TEXTO PARA O BACKEND
    byte[] audioBytes = await backendManager.GerarAudio(dialogoAtual[i], idNpcParaVoz);
    
    if (audioBytes != null && audioBytes.Length > 0)
    {
        Debug.Log($"Áudio recebido! Tamanho: {audioBytes.Length} bytes");
        string caminho = Path.Combine(Application.persistentDataPath, "audio_temp.wav");
        File.WriteAllBytes(caminho, audioBytes);
        
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + caminho, AudioType.WAV);
        var operation = www.SendWebRequest();
        
        while (!operation.isDone)
            await Task.Yield();

        AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
        audioSource.clip = clip;

        if (mostrarLegenda)
        {
            textoPularDialogo.gameObject.SetActive(false);
            // Usa o nome dinâmico do NPC na legenda
            modalLegenda.GetComponent<ModalLegenda>().MostrarLegenda(nomeExibicaoLegenda, dialogoAtual[i]);
        }

        audioSource.Play();
        while (audioSource.isPlaying)
        {
            await Task.Yield();
        }

        if (textoPularDialogo != null)
        {
            textoPularDialogo.gameObject.SetActive(true);
        }

        await Task.Yield();

        bool clicou = false;
        while (!clicou)
        {
            if (Input.GetMouseButtonDown(0)) 
            {
                clicou = true; 
            }
            else
            {
                await Task.Yield(); 
            }
        }

        if (textoPularDialogo != null)
        {
            textoPularDialogo.gameObject.SetActive(false);
        }
    }
    else
    {
        Debug.LogError("O servidor retornou um array de bytes vazio.");
        if (mostrarLegenda)
        {
            modalLegenda.GetComponent<ModalLegenda>().MostrarLegenda(nomeExibicaoLegenda, dialogoAtual[i]);
        }
    }
}

public async Task FalarFraseCustomizada(string textoParaFalar)
{
    if (string.IsNullOrEmpty(textoParaFalar)) return;

    // 1. PASSANDO O ID DO NPC JUNTO COM O TEXTO PARA O BACKEND TAMBÉM NO FEEDBACK
    byte[] audioBytes = await backendManager.GerarAudio(textoParaFalar, idNpcParaVoz);
    
    if (audioBytes != null && audioBytes.Length > 0)
    {
        string caminho = Path.Combine(Application.persistentDataPath, "audio_temp_custom.wav");
        File.WriteAllBytes(caminho, audioBytes);
        
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + caminho, AudioType.WAV);
        var operation = www.SendWebRequest();
        
        while (!operation.isDone)
            await Task.Yield();

        AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
        audioSource.clip = clip;

        if (modalLegenda != null)
        {
            modalLegenda.SetActive(true); 
            if (textoPularDialogo != null) textoPularDialogo.gameObject.SetActive(false);
            
            // Usa o nome dinâmico do NPC na legenda
            modalLegenda.GetComponent<ModalLegenda>().MostrarLegenda(nomeExibicaoLegenda, textoParaFalar);
        }

        animator.SetBool("IsTalking", true);
        audioSource.Play();
        
        while (audioSource.isPlaying)
        {
            await Task.Yield();
        }
        animator.SetBool("IsTalking", false);
    }
}
}