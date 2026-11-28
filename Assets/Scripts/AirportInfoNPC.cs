// using System.Collections.Generic;
// using System.Threading.Tasks;
// using UnityEngine;
// using UnityEngine.Networking;
// using System.IO;
// using TMPro;
// using System;
// using System.Runtime.CompilerServices;

// public class AirportInfoNPC : InteracaoNPC
// {
//     string nivelAtual;
//     string cumprimento;
//     int horaAtual = DateTime.Now.Hour;
//     List<string> dialogoAtual;
//     private AudioSource audioSource;
//     private Animator animator;

//     private BackendManager backendManager;
//     private MissionManager missaoManager;
//     public ModalExercicio modalExercicio;
//     public GameObject modalLegenda;
//     public ExercicioBase exAtual;
//     public TextMeshProUGUI textoPularDialogo;

//     public List<ExercicioCompletarFrase> exerciciosCompletarFrase = new List<ExercicioCompletarFrase>();
//     [System.NonSerialized] public List<ExercicioBase> exerciciosCriarFrases;
//     [System.NonSerialized] public List<ExercicioBase> exerciciosSpeaking;
//     [System.NonSerialized] public List<ExercicioBase> exerciciosAlternativas;
//     [System.NonSerialized] public List<ExercicioBase> exerciciosWriting;
//     [System.NonSerialized] public List<ExercicioBase> exerciciosAssociacao;

//     public string idNpcParaVoz = "airportInfo";

//     protected override void Start()
//     {
//         base.Start();
//         nivelAtual = DadosJogador.nivelUsuario;
//         nomeExibicaoLegenda = "Airport Employee";
//         backendManager = new BackendManager();
//         missaoManager = MissionManager.Instance; 
//         audioSource = GetComponent<AudioSource>();
//         animator = GetComponent<Animator>();
//         if (exerciciosCompletarFrase == null)
//             exerciciosCompletarFrase = new List<ExercicioCompletarFrase>();

//         exerciciosCompletarFrase = exerciciosCompletarFrase ?? new List<ExercicioCompletarFrase>();
//         exerciciosCriarFrases = GarantirListaExercicios(exerciciosCriarFrases);
//         exerciciosSpeaking = GarantirListaExercicios(exerciciosSpeaking);
//         exerciciosAlternativas = GarantirListaExercicios(exerciciosAlternativas);
//         exerciciosWriting = GarantirListaExercicios(exerciciosWriting);
//         exerciciosAssociacao = GarantirListaExercicios(exerciciosAssociacao);

//         if (horaAtual < 12)
//         {
//             cumprimento = "Good morning!";
//         }
//         else if (horaAtual < 18)
//         {
//             cumprimento = "Good afternoon!";
//         }
//         else
//         {
//             cumprimento = "Good evening!";
//         }
//     }

//     private void InicializarConteudosPorNivel()
//     {
//         exerciciosCompletarFrase.Clear();
//         exerciciosCriarFrases.Clear();
//         exerciciosSpeaking.Clear();
//         exerciciosAlternativas.Clear();
//         exerciciosWriting.Clear();
//         exerciciosAssociacao.Clear();

//         if (nivelAtual == "A1")
//         {
//             dialogoAtual = new List<string>()
//             {
//                 cumprimento+". Do you need help?",
//                 "Go up the stairs. Then go straight ahead."
//             };

//             exerciciosSpeaking.Add(
//                     new ExercicioSpeaking()
//                     {
//                         enunciado = "Ask the airport staff where you can find the check-in",
//                         opcoesFala = new List<string>()
//                         {
//                             "Hello. Could you tell me where the baggage drop is?\n",
//                             "Hello. I need to find the check-in desk\n",
//                             "Excuse me, where can I find the departure screens?\n"
//                         },
//                         respostaCorreta = 1
//                     }
//             );

//             exerciciosAlternativas.Add(
//                 new ExercicioAlternativas()
//                 {
//                     enunciado = "What document do you need for an international flight?",
//                     alternativas = new List<string>() { 
//                         "A restaurant menu\r\n", 
//                         "A library card\r\n", 
//                         "A hotel room key\r\n", 
//                         "A passport\r\n" },
//                     alternativaCorreta = 3
//                 }
//             );
//         }
//         else if(nivelAtual=="A2")
//         {
//             dialogoAtual = new List<string>()
//             {
//                 cumprimento + ". Can I help you?",
//                 "Check-in for your airline is on the second floor.",
//                 "Take the escalator up, then walk straight. You will see the check-in desks on your left.",
//                 "It takes about five minutes. Have a nice trip!"
//             };

//             exerciciosSpeaking.Add(
//                 new ExercicioSpeaking()
//                 {
//                     enunciado = "Ask the airport employee where the check-in desk are.",
//                     opcoesFala = new List<string>()
//                     {
//                         "Could you tell me which floor the check-in desks are on?\n",
//                         "Could you tell me what time my flight arrived yesterday?\n",
//                         "Could you tell me where I can buy a meal on the plane?\n"
//                     },
//                     respostaCorreta = 0
//                 }
//             );

//             exerciciosSpeaking.Add(
//                 new ExercicioSpeaking()
//                 {
//                     enunciado = "Ask the employee how to get there.",
//                     opcoesFala = new List<string>()
//                     {
//                         "How long is my flight to London?\n",
//                         "Could you explain how I can get to the second floor?\n",
//                         "Where can I collect my luggage after landing?\n"
//                     },
//                     respostaCorreta = 1
//                 }
//             );

//             exerciciosWriting.Add(
//                 new ExercicioWriting()
//                 {
//                     enunciado = "Write a question asking the airport employee how long it takes to get to the check-in desks."
//                 }
//             );
//         }
//         else if(nivelAtual=="B1")
//         {
//             dialogoAtual = new List<string>()
//             {
//                  cumprimento + ". Welcome to the airport. How can I help you?",
//                 "Sure. The check-in desks for your airline are on the second floor.",
//                 "Take the escalator behind me, and when you get to the top, turn right. Walk past the cafés and you'll see the desks on your left, just before security.",
//                 "It should take you about five minutes, but I'd go now because the lines get long in the morning.",
//                 "If you need anything else, you can find me here or at any information desk. Have a good flight!"
//             };

//             exerciciosSpeaking.Add(
//                 new ExercicioSpeaking()
//                 {
//                     enunciado = "You need to check in for your flight. Ask the employee where your airline's check-in desks are located.",
//                     opcoesFala = new List<string>()
//                     {
//                         "Could you confirm whether my luggage has arrived at the destination?\n",
//                         "Would you mind telling me if my connecting flight was delayed yesterday?\n",
//                         "Could you let me know which floor the check-in desks are on?\n"
//                     },
//                     respostaCorreta = 2
//                 }
//             );

//             // 2. Solicitar instruções detalhadas
//             exerciciosAlternativas.Add(
//                 new ExercicioAlternativas()
//                 {
//                     enunciado = "The employee says the check-in desks are on the second floor. What should you ask to find them?",
//                     alternativas = new List<string>()
//                     {
//                         "Could you explain how I can get there from here?\r\n",
//                         "Could you tell me whether my passport is still valid?\r\n",
//                         "Would you mind checking if my flight ticket has been paid for?\r\n",
//                         "Could you tell me where I can collect my luggage after landing?\r\n"
//                     },
//                     alternativaCorreta = 0
//                 }
//             );

//             // 3. Perguntar sobre o tempo e as filas
//             exerciciosSpeaking.Add(
//                 new ExercicioSpeaking()
//                 {
//                     enunciado = "You are concerned about arriving at check-in on time. Ask how long the walk takes and whether you should leave now.",
//                     opcoesFala = new List<string>()
//                     {
//                         "Could you explain how to request a refund for my ticket?\n",
//                         "Do you know how long it takes to get there, and whether I should leave now?\n",
//                         "Would you mind telling me where I can collect my luggage after the flight?\n"
//                     },
//                     respostaCorreta = 1
//                 }
//             );

//             // 4. Perguntar onde conseguir mais ajuda
//             exerciciosSpeaking.Add(
//                 new ExercicioSpeaking()
//                 {
//                     enunciado = "After receiving directions, you want to know where to go if you need further assistance. Ask the employee politely.",
//                     opcoesFala = new List<string>()
//                     {
//                         "Could you tell me where I can get further assistance if I need it?\n",
//                         "Could you tell me how to change my seat after the flight has landed?\n",
//                         "Would you mind explaining why the cafés are closed this morning?\n"
//                     },
//                     respostaCorreta = 0
//                 }
//             );
//         }
//     }

//     private TipoExercicio ObterTipoExercicio(ExercicioBase exercicio)
//     {
//         if (exercicio is ExercicioSpeaking)
//             return TipoExercicio.Speaking;
//         if (exercicio is ExercicioAlternativas)
//             return TipoExercicio.Alternativas;
//         if (exercicio is ExercicioBlocos)
//             return TipoExercicio.Blocos;
//         if (exercicio is ExercicioCompletarFrase)
//             return TipoExercicio.CompletarFrase;
//         if (exercicio is ExercicioWriting)
//             return TipoExercicio.Writing;
//         if (exercicio is ExercicioAssociacao)
//             return TipoExercicio.Associacao;

//         throw new InvalidOperationException("Tipo de exercício não suportado: " + exercicio.GetType().Name);
//     }

//     private void IniciarMissao(string id, string texto, string traducao)
//     {
//         MissionManager.AdicionarMissaoNPC(id, texto, traducao);
//     }

//     private void ConcluirMissao(string id)
//     {
//         MissionManager.ConcluirMissaoNPC(id);
//     }

//     protected override async Task IniciarInteracao()
//     {
//         GameProgress.EstaEmDialogo = true;
//         nivelAtual = DadosJogador.nivelUsuario;
//         InicializarConteudosPorNivel();

//         if (nivelAtual == "A1")
//         {
//             await interacaoA1();
//         }
//         else if (nivelAtual == "A2")
//         {
//             await interacaoA2();
//         }
//         else
//         {
//             await interacaoB1();
//         }

//         // Desativa o estado quando a conversa acabar completamente!
//         GameProgress.EstaEmDialogo = false;
//         //MissionManager.Instance.ConcluirMissao("finalizar_compra");

//         if (modalLegenda != null)
//             modalLegenda.SetActive(false);

//         if (textoPularDialogo != null)
//             textoPularDialogo.gameObject.SetActive(false);

//         GameProgress.EstaEmDialogo = false;

//         LiberarControlePlayer();
//     }

//     private async Task interacaoA1()
//     {
//         int i = 0;

//         IniciarMissao("aeroporto_a1_1", "Chegue ao aeroporto e procure o balcão de informações", "Arrive at the airport and find the information desk");

//         await PlayAudioETexto(i++);

//         exAtual = exerciciosSpeaking[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         ConcluirMissao("aeroporto_a1_1");
//         IniciarMissao("aeroporto_a1_2", "Peça orientações para encontrar o balcão de check-in", "Ask for directions to find the check-in desk");

//         await PlayAudioETexto(i++);

//         exAtual = exerciciosAlternativas[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         ConcluirMissao("aeroporto_a1_2");
//     }

//     private async Task interacaoA2()
//     {
//         int i = 0;

//         IniciarMissao("aeroporto_a2_1", "Chegue ao aeroporto e procure o balcão de check-in", "Arrive at the airport and find the check-in desk");

//         await PlayAudioETexto(i++);

//         exAtual = exerciciosSpeaking[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         await PlayAudioETexto(i++);

//         exAtual = exerciciosSpeaking[1];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         await AbrirExercicio(ObterTipoExercicio(exerciciosWriting[0]), exerciciosWriting[0]);

//         await PlayAudioETexto(i++);

//         ConcluirMissao("aeroporto_a2_1");
//     }

//     private async Task interacaoB1()
//     {
//         int i = 0;

//         IniciarMissao("aeroporto_b1_1", "Chegue ao aeroporto e dirija-se ao balcão de informações", "Arrive at the airport and go to the information desk");

//         await PlayAudioETexto(i++);

//         exAtual = exerciciosSpeaking[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         ConcluirMissao("aeroporto_b1_1");
//         IniciarMissao("aeroporto_b1_2", "Peça orientações para encontrar o balcão de check-in", "Ask for directions to find the check-in desk");

//         await PlayAudioETexto(i++);

//         await AbrirExercicio(ObterTipoExercicio(exerciciosAlternativas[0]), exerciciosAlternativas[0]);

//         await PlayAudioETexto(i++);

//         exAtual = exerciciosSpeaking[1];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         await PlayAudioETexto(i++);

//         await PlayAudioETexto(i++);

//         exAtual = exerciciosSpeaking[2];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         ConcluirMissao("aeroporto_b1_2");
//     }

//     public async Task AbrirExercicio(TipoExercicio tipo, ExercicioBase ex)
//     {
//         //Debug.Log("Abrindo exercício: " + tipo);

//         ModalLegenda legenda = modalLegenda.GetComponent<ModalLegenda>();

//         if (modalLegenda != null)
//         {
//             modalLegenda.SetActive(false);
//         }

//         await EntrarModoExercicio();

//         Cursor.lockState = CursorLockMode.None;
//         Cursor.visible = true;

//         modalExercicio.Abrir(tipo, ex, this);

//         if (legenda != null) 
//         {
//             legenda.AjustarPosicaoPeloEstadoDoJogo(); 
//         }

//         while (!modalExercicio.exercicioFinalizado)
//         {
//             await Task.Yield();
//         }

//         if (modalLegenda != null)
//         {
//             modalLegenda.SetActive(false);
//         }

//         await SairModoExercicio();

//         // ✨ Volta a prender o mouse após terminar o exercício (se o diálogo geral não tiver acabado)
//         Cursor.lockState = CursorLockMode.Locked;
//         Cursor.visible = false;

//         if (legenda != null)
//         {
//             legenda.AjustarPosicaoPeloEstadoDoJogo();
//         }
//     }

//     private async Task PlayAudioETexto(int i)
//     {
//         ultimaFraseDita = dialogoAtual[i];
//         // 1. PASSANDO O ID DO NPC JUNTO COM O TEXTO PARA O BACKEND
//         byte[] audioBytes = await backendManager.GerarAudio(dialogoAtual[i], idNpcParaVoz);
        
//         if (audioBytes != null && audioBytes.Length > 0)
//         {
//             //Debug.Log($"Áudio recebido! Tamanho: {audioBytes.Length} bytes");
//             string caminho = Path.Combine(Application.persistentDataPath, "audio_temp.wav");
//             File.WriteAllBytes(caminho, audioBytes);
            
//             UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + caminho, AudioType.WAV);
//             var operation = www.SendWebRequest();
            
//             while (!operation.isDone)
//                 await Task.Yield();

//             AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
//             audioSource.clip = clip;

//             if (textoPularDialogo != null)
//                 textoPularDialogo.gameObject.SetActive(false);

//             modalLegenda.GetComponent<ModalLegenda>().MostrarLegenda(nomeExibicaoLegenda, dialogoAtual[i]);
//             audioSource.Play();
//             while (audioSource.isPlaying)
//             {
//                 await Task.Yield();
//             }

//             if (textoPularDialogo != null)
//             {
//                 textoPularDialogo.gameObject.SetActive(true);

//                 Cursor.lockState = CursorLockMode.None;
//                 Cursor.visible = true;
//             }

//             await Task.Yield();

//             bool clicou = false;
//             while (!clicou)
//             {
//                 if (Input.GetMouseButtonDown(0)) 
//                 {
//                     clicou = true; 
//                 }
//                 else
//                 {
//                     await Task.Yield(); 
//                 }
//             }


//             if (textoPularDialogo != null)
//             {
//                 textoPularDialogo.gameObject.SetActive(false);
//             }

//             if (modalLegenda != null)
//             {
//                 modalLegenda.SetActive(false);
//             }
            
//             while (Input.GetMouseButton(0))
//             {
//                 await Task.Yield();
//             }
//         }
//         else
//         {
//             Debug.LogError("O servidor retornou um array de bytes vazio.");
//             modalLegenda.GetComponent<ModalLegenda>().MostrarLegenda(nomeExibicaoLegenda, dialogoAtual[i]);
//         }
//     }

//     public override async Task FalarFraseCustomizada(string textoParaFalar)
//     {
//         if (string.IsNullOrEmpty(textoParaFalar)) return;

//         byte[] audioBytes = await backendManager.GerarAudio(textoParaFalar, idNpcParaVoz);
        
//         if (audioBytes != null && audioBytes.Length > 0)
//         {
//             string caminho = Path.Combine(Application.persistentDataPath, "audio_temp_custom.wav");
//             File.WriteAllBytes(caminho, audioBytes);
            
//             UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + caminho, AudioType.WAV);
//             var operation = www.SendWebRequest();
            
//             while (!operation.isDone)
//                 await Task.Yield();

//             AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
//             audioSource.clip = clip;
//             if (modalLegenda != null)
//             {
//                 modalLegenda.SetActive(true);

//                 if (textoPularDialogo != null)
//                     textoPularDialogo.gameObject.SetActive(false);

//                 modalLegenda.GetComponent<ModalLegenda>()
//                     .MostrarLegenda(nomeExibicaoLegenda, textoParaFalar);
//             }
            
//             audioSource.Play();
            
//             while (audioSource.isPlaying)
//             {
//                 await Task.Yield();
//             }

//             if (modalLegenda != null)
//             {
//                 modalLegenda.SetActive(false);
//             }
//         }
//     }
// }

using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using TMPro;
using System;
using System.Runtime.CompilerServices;

public class AirportInfoNPC : InteracaoNPC, IRoteiroNpcCheckpoint
{
    string nivelAtual;
    string cumprimento;
    int horaAtual = DateTime.Now.Hour;
    List<string> dialogoAtual;
    private AudioSource audioSource;
    private Animator animator;

    private bool instrucoesJaApresentadas;
    internal bool OrientacoesConcluidasNaVisita
    {
        get
        {
            AtualizarVisita();
            return PertenceAoCenarioAtivo && instrucoesJaApresentadas;
        }
    }

    protected override void ReiniciarVisita() => instrucoesJaApresentadas = false;
    private const string NpcKey = "airport_info_npc";
    private ExerciseLoadResult catalogoExercicios;

    private BackendManager backendManager;
    private MissionManager missaoManager;
    public ModalExercicio modalExercicio;
    public GameObject modalLegenda;
    public ExercicioBase exAtual;
    public TextMeshProUGUI textoPularDialogo;

    public List<ExercicioCompletarFrase> exerciciosCompletarFrase = new List<ExercicioCompletarFrase>();
    [System.NonSerialized] public List<ExercicioBase> exerciciosCriarFrases;
    [System.NonSerialized] public List<ExercicioBase> exerciciosSpeaking;
    [System.NonSerialized] public List<ExercicioBase> exerciciosAlternativas;
    [System.NonSerialized] public List<ExercicioBase> exerciciosWriting;
    [System.NonSerialized] public List<ExercicioBase> exerciciosAssociacao;

    public string idNpcParaVoz = "airportInfo";

    protected override void Start()
    {
        base.Start();
        nivelAtual = DadosJogador.nivelUsuario;
        nomeExibicaoLegenda = "Airport Employee";
        backendManager = new BackendManager();
        missaoManager = MissionManager.Instance; 
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        if (exerciciosCompletarFrase == null)
            exerciciosCompletarFrase = new List<ExercicioCompletarFrase>();

        exerciciosCompletarFrase = exerciciosCompletarFrase ?? new List<ExercicioCompletarFrase>();
        exerciciosCriarFrases = GarantirListaExercicios(exerciciosCriarFrases);
        exerciciosSpeaking = GarantirListaExercicios(exerciciosSpeaking);
        exerciciosAlternativas = GarantirListaExercicios(exerciciosAlternativas);
        exerciciosWriting = GarantirListaExercicios(exerciciosWriting);
        exerciciosAssociacao = GarantirListaExercicios(exerciciosAssociacao);

        if (horaAtual < 12)
        {
            cumprimento = "Good morning!";
        }
        else if (horaAtual < 18)
        {
            cumprimento = "Good afternoon!";
        }
        else
        {
            cumprimento = "Good evening!";
        }
    }

    private void InicializarConteudosPorNivel(ConteudoRoteiro descricao = null, string nivelDescricao = null)
    {
        string nivelAtual = nivelDescricao ?? this.nivelAtual;
        var dialogoAtual = this.dialogoAtual;
        var exerciciosCompletarFrase = descricao?.FrasesTipadas ?? this.exerciciosCompletarFrase;
        var exerciciosCriarFrases = descricao?.exerciciosCriarFrases ?? this.exerciciosCriarFrases;
        var exerciciosSpeaking = descricao?.exerciciosSpeaking ?? this.exerciciosSpeaking;
        var exerciciosAlternativas = descricao?.exerciciosAlternativas ?? this.exerciciosAlternativas;
        var exerciciosWriting = descricao?.exerciciosWriting ?? this.exerciciosWriting;
        var exerciciosAssociacao = descricao?.exerciciosAssociacao ?? this.exerciciosAssociacao;
        var cumprimento = descricao == null ? this.cumprimento : "{{greeting}}";

        exerciciosCompletarFrase.Clear();
        exerciciosCriarFrases.Clear();
        exerciciosSpeaking.Clear();
        exerciciosAlternativas.Clear();
        exerciciosWriting.Clear();
        exerciciosAssociacao.Clear();

        if (nivelAtual == "A1")
        {
            dialogoAtual = new List<string>()
            {
                cumprimento+". Do you need help?",
                "Go up the stairs. Then go straight ahead."
            };

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Ask the airport staff where you can find the check-in",
                        opcoesFala = new List<string>()
                        {
                            "Hello. Could you tell me where the baggage drop is?\n",
                            "Hello. I need to find the check-in desk\n",
                            "Excuse me, where can I find the departure screens?\n"
                        },
                        respostaCorreta = 1
                    }
            );

            exerciciosAlternativas.Add(
                new ExercicioAlternativas()
                {
                    enunciado = "What document do you need for an international flight?",
                    alternativas = new List<string>() { 
                        "A restaurant menu\r\n", 
                        "A library card\r\n", 
                        "A hotel room key\r\n", 
                        "A passport\r\n" },
                    alternativaCorreta = 3
                }
            );
        }
        else if(nivelAtual=="A2")
        {
            dialogoAtual = new List<string>()
            {
                cumprimento + ". Can I help you?",
                "Check-in for your airline is on the second floor.",
                "Take the escalator up, then walk straight. You will see the check-in desks on your left.",
                "It takes about five minutes. Have a nice trip!"
            };

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "Ask the airport employee where the check-in desk are.",
                    opcoesFala = new List<string>()
                    {
                        "Could you tell me which floor the check-in desks are on?\n",
                        "Could you tell me what time my flight arrived yesterday?\n",
                        "Could you tell me where I can buy a meal on the plane?\n"
                    },
                    respostaCorreta = 0
                }
            );

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "Ask the employee how to get there.",
                    opcoesFala = new List<string>()
                    {
                        "How long is my flight to London?\n",
                        "Could you explain how I can get to the second floor?\n",
                        "Where can I collect my luggage after landing?\n"
                    },
                    respostaCorreta = 1
                }
            );

            exerciciosWriting.Add(
                new ExercicioWriting()
                {
                    enunciado = "Write a question asking the airport employee how long it takes to get to the check-in desks."
                }
            );
        }
        else if(nivelAtual=="B1")
        {
            dialogoAtual = new List<string>()
            {
                 cumprimento + ". Welcome to the airport. How can I help you?",
                "Sure. The check-in desks for your airline are on the second floor.",
                "Take the escalator behind me, and when you get to the top, turn right. Walk past the cafés and you'll see the desks on your left, just before security.",
                "It should take you about five minutes, but I'd go now because the lines get long in the morning.",
                "If you need anything else, you can find me here or at any information desk. Have a good flight!"
            };

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "You need to check in for your flight. Ask the employee where your airline's check-in desks are located.",
                    opcoesFala = new List<string>()
                    {
                        "Could you confirm whether my luggage has arrived at the destination?\n",
                        "Would you mind telling me if my connecting flight was delayed yesterday?\n",
                        "Could you let me know which floor the check-in desks are on?\n"
                    },
                    respostaCorreta = 2
                }
            );

            // 2. Solicitar instruções detalhadas
            exerciciosAlternativas.Add(
                new ExercicioAlternativas()
                {
                    enunciado = "The employee says the check-in desks are on the second floor. What should you ask to find them?",
                    alternativas = new List<string>()
                    {
                        "Could you explain how I can get there from here?\r\n",
                        "Could you tell me whether my passport is still valid?\r\n",
                        "Would you mind checking if my flight ticket has been paid for?\r\n",
                        "Could you tell me where I can collect my luggage after landing?\r\n"
                    },
                    alternativaCorreta = 0
                }
            );

            // 3. Perguntar sobre o tempo e as filas
            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "You are concerned about arriving at check-in on time. Ask how long the walk takes and whether you should leave now.",
                    opcoesFala = new List<string>()
                    {
                        "Could you explain how to request a refund for my ticket?\n",
                        "Do you know how long it takes to get there, and whether I should leave now?\n",
                        "Would you mind telling me where I can collect my luggage after the flight?\n"
                    },
                    respostaCorreta = 1
                }
            );

            // 4. Perguntar onde conseguir mais ajuda
            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "After receiving directions, you want to know where to go if you need further assistance. Ask the employee politely.",
                    opcoesFala = new List<string>()
                    {
                        "Could you tell me where I can get further assistance if I need it?\n",
                        "Could you tell me how to change my seat after the flight has landed?\n",
                        "Would you mind explaining why the cafés are closed this morning?\n"
                    },
                    respostaCorreta = 0
                }
            );
        }

        else if (nivelAtual == "A1-A2")
{
    dialogoAtual = new List<string>()
    {
        cumprimento + ". Can I help you?",
        "The check-in desks are on the second floor.",
        "Take the escalator upstairs and walk straight ahead. You will find the desks on your left."
    };

    exerciciosSpeaking.Add(
        new ExercicioSpeaking()
        {
            enunciado = "Ask the airport employee which floor the check-in desks are on.",
            opcoesFala = new List<string>()
            {
                "Could you tell me when my flight will land?\n",
                "Could you tell me which floor the check-in desks are on?\n",
                "Could you tell me where I can collect my luggage?\n"
            },
            respostaCorreta = 1
        }
    );

    exerciciosAlternativas.Add(
        new ExercicioAlternativas()
        {
            enunciado = "The employee says the check-in desks are on the second floor. How should you get there?",
            alternativas = new List<string>()
            {
                "Walk outside and look for a taxi.\r\n",
                "Go to the baggage claim area.\r\n",
                "Take the escalator upstairs and walk straight ahead.\r\n",
                "Go downstairs and turn left.\r\n"
            },
            alternativaCorreta = 2
        }
    );
}
else if (nivelAtual == "A2-B1")
{
    dialogoAtual = new List<string>()
    {
        cumprimento + ". Welcome to the airport. How can I assist you?",
        "Your airline's check-in desks are located on the second floor.",
        "Take the escalator behind me, turn right at the top, and walk past the cafés. The desks will be on your left.",
        "It should take around five minutes. I recommend heading there now, as the queues can get quite long."
    };

    exerciciosSpeaking.Add(
        new ExercicioSpeaking()
        {
            enunciado = "You need to find your airline's check-in desks. Ask the employee where they are located.",
            opcoesFala = new List<string>()
            {
                "Do you know when the boarding process will finish?\n",
                "Would you mind telling me which floor the check-in desks are on?\n",
                "Could you confirm whether my luggage has been transferred?\n"
            },
            respostaCorreta = 1
        }
    );

    exerciciosAlternativas.Add(
        new ExercicioAlternativas()
        {
            enunciado = "The employee explains how to reach the check-in desks. What should you do after taking the escalator?",
            alternativas = new List<string>()
            {
                "Leave the terminal and enter the parking area.\r\n",
                "Turn right and walk past the cafés.\r\n",
                "Turn left and go downstairs again.\r\n",
                "Walk straight to the baggage claim area.\r\n"
            },
            alternativaCorreta = 1
        }
    );

    exerciciosSpeaking.Add(
        new ExercicioSpeaking()
        {
            enunciado = "You want to know how long it will take to reach the check-in desks. Ask the employee politely.",
            opcoesFala = new List<string>()
            {
                "Do you know which gate my connecting flight leaves from?\n",
                "Would you mind telling me how long it takes to get there?\n",
                "Could you tell me how much my checked baggage weighs?\n"
            },
            respostaCorreta = 1
        }
    );
}

        if (descricao != null)
            descricao.Falas = dialogoAtual;
        else
        {
            this.dialogoAtual = dialogoAtual;
        }
    }

    private TipoExercicio ObterTipoExercicio(ExercicioBase exercicio)
    {
        if (exercicio is ExercicioSpeaking)
            return TipoExercicio.Speaking;
        if (exercicio is ExercicioAlternativas)
            return TipoExercicio.Alternativas;
        if (exercicio is ExercicioBlocos)
            return TipoExercicio.Blocos;
        if (exercicio is ExercicioCompletarFrase)
            return TipoExercicio.CompletarFrase;
        if (exercicio is ExercicioWriting)
            return TipoExercicio.Writing;
        if (exercicio is ExercicioAssociacao)
            return TipoExercicio.Associacao;

        throw new InvalidOperationException("Tipo de exercício não suportado: " + exercicio.GetType().Name);
    }

    private static void EmitirEventoMissao(string tipo, string alvo)
    {
        MissionManager.Instance?.RegistrarEvento(tipo, alvo);
    }

    private void IniciarMissao(string id, string texto, string traducao)
    {
        if (MissionManager.Instance?.UsaCatalogoAirport != true)
            MissionManager.AdicionarMissaoNPC(id, texto, traducao);
    }

    private void ConcluirMissao(string id)
    {
        if (MissionManager.Instance?.UsaCatalogoAirport != true)
            MissionManager.ConcluirMissaoNPC(id);
    }

    protected override Task IniciarInteracao() => ExecutarAtendimento(0, false);

    private async Task ExecutarAtendimento(int inicio, bool retomada)
    {
        int versao = SessaoCenario.Atual.Versao;
        GameProgress.EstaEmDialogo = true;

        if (instrucoesJaApresentadas)
        {
            await RepetirInstrucoesAsync();
        }
        else
        {
            nivelAtual = DadosJogador.nivelUsuario;
            if (!retomada) switch (nivelAtual)
            {
                case "A1":
                    EmitirEventoMissao("npc_interagido", "airport_info_npc");
                    ConcluirMissao("aeroporto_a1_1");
                    IniciarMissao("aeroporto_a1_2", "Ask for directions to find the check-in desk", "Peça orientações para encontrar o balcão de check-in");
                    break;
                case "A1-A2":
                    EmitirEventoMissao("npc_interagido", "airport_info_npc");
                    ConcluirMissao("aeroporto_a1a2_1");
                    IniciarMissao("aeroporto_a1a2_2", "Ask for directions to find the check-in desk", "Peça orientações para encontrar o balcão de check-in");
                    break;
                case "B1":
                    EmitirEventoMissao("npc_interagido", "airport_info_npc");
                    ConcluirMissao("aeroporto_b1_1");
                    IniciarMissao("aeroporto_b1_2", "Ask for directions to find the check-in desk", "Peça orientações para encontrar o balcão de check-in");
                    break;
            }

            InicializarConteudosPorNivel();
            catalogoExercicios = await ExerciseManager.Instance.LoadForNpcAsync(NpcKey, nivelAtual);
            if (!catalogoExercicios.Success)
                Debug.LogWarning("AirportInfoNPC: usando exerc?cios locais. " + catalogoExercicios.Error);

            ValidarVisita(versao);
            await CriarRoteiro(versao).Executar(inicio);

            switch (nivelAtual)
            {
                case "A1":
                    IniciarMissao("aeroporto_a1_3", "Go to the check-in desk and present your passport", "Dirija-se ao balcão de check-in e apresente seu passaporte");
                    break;
                case "A1-A2":
                    IniciarMissao("aeroporto_a1a2_3", "Go to the check-in desk and present your passport", "Dirija-se ao balcão de check-in e apresente seu passaporte");
                    break;
                case "B1":
                    EmitirEventoMissao("conversa_encerrada", "airport_info_npc");
                    IniciarMissao("aeroporto_b1_3", "Go to the check-in desk", "Dirija-se ao balcão de check-in");
                    break;
            }

            instrucoesJaApresentadas = true;
        }

        // Desativa o estado quando a conversa acabar completamente!
        GameProgress.EstaEmDialogo = false;
            //MissionManager.Instance.ConcluirMissao("finalizar_compra");

        if (modalLegenda != null)
            modalLegenda.SetActive(false);
        if (textoPularDialogo != null)
            textoPularDialogo.gameObject.SetActive(false);

        GameProgress.EstaEmDialogo = false;

        LiberarControlePlayer();
    }


    async Task<System.Func<Task>> IRoteiroNpcCheckpoint.PrepararRetomada(Newtonsoft.Json.Linq.JObject visita,
        int versao, System.Threading.CancellationToken token)
    {
        ValidarVisita(versao);
        AtualizarVisita();
        nivelAtual = SessaoCenario.Atual.Nivel;
        cumprimento = RoteiroNpcCheckpoint.SaudacaoSalva(visita);
        InicializarConteudosPorNivel();
        var cursor = CriarRoteiro(versao).Validar(visita);
        await RoteiroNpcCheckpoint.ExigirCatalogo(this, versao, token);
        return async () =>
        {
            ValidarVisita(versao);
            instrucoesJaApresentadas = false;
            TravarControlePlayer();
            await ExecutarAtendimento(cursor.Indice, true);
        };
    }

    internal void RetomarAposOrientacoesCheckpoint(int versao)
    {
        MarcarAtendimentoRestaurado(versao);
        instrucoesJaApresentadas = true;
        nivelAtual = SessaoCenario.Atual.Nivel;
        InicializarConteudosPorNivel();
    }

    internal RoteiroNpcCheckpoint DescreverRoteiro(string nivel)
    {
        var conteudo = new ConteudoRoteiro();
        InicializarConteudosPorNivel(conteudo, nivel);
        return CriarRoteiro(0, conteudo, nivel);
    }

    private RoteiroNpcCheckpoint CriarRoteiro(int visita, ConteudoRoteiro descricao = null, string nivelDescricao = null)
    {
        string nivelAtual = nivelDescricao ?? this.nivelAtual;
        var dialogoAtual = descricao?.Falas ?? this.dialogoAtual;
        var exerciciosSpeaking = descricao?.exerciciosSpeaking ?? this.exerciciosSpeaking;
        var exerciciosAlternativas = descricao?.exerciciosAlternativas ?? this.exerciciosAlternativas;
        var exerciciosWriting = descricao?.exerciciosWriting ?? this.exerciciosWriting;

        var roteiro = new RoteiroNpcCheckpoint(this, visita, () => new[] { RoteiroNpcCheckpoint.ChaveSaudacao(cumprimento) });
        void Fala(int indice) => roteiro.Fala(dialogoAtual[indice], () => PlayAudioETexto(indice));
        void Exercicio(string ponto, int ordem, ExercicioBase fallback) =>
            roteiro.Exercicio(ponto, ordem, () => AbrirExercicioDoPonto(ponto, ordem, fallback), modelo: fallback);

        switch (nivelAtual)
        {
            case "A1":
                Fala(0);
                Exercicio("checkin_location", 1, exerciciosSpeaking[0]);
                Fala(1);
                Exercicio("travel_document", 1, exerciciosAlternativas[0]);
                roteiro.Acao("EmitirEventoMissao.conversa_encerrada.airport_info_npc", () => { EmitirEventoMissao("conversa_encerrada", "airport_info_npc"); return Task.CompletedTask; });
                roteiro.Acao("ConcluirMissao.aeroporto_a1_2", () => { ConcluirMissao("aeroporto_a1_2"); return Task.CompletedTask; });
                break;
            case "A2":
                Fala(0);
                Exercicio("checkin_location", 1, exerciciosSpeaking[0]);
                Fala(1);
                Exercicio("checkin_directions", 1, exerciciosSpeaking[1]);
                Exercicio("walking_time", 1, exerciciosWriting[0]);
                Fala(2);
                break;
            case "B1":
                Fala(0);
                Exercicio("checkin_location", 1, exerciciosSpeaking[0]);
                roteiro.Acao("ConcluirMissao.aeroporto_b1_2", () => { ConcluirMissao("aeroporto_b1_2"); return Task.CompletedTask; });
                Fala(1);
                Exercicio("checkin_directions", 1, exerciciosAlternativas[0]);
                Fala(2);
                Exercicio("walking_time", 1, exerciciosSpeaking[1]);
                Fala(3);
                Fala(4);
                Exercicio("further_assistance", 1, exerciciosSpeaking[2]);
                break;
            case "A1-A2":
                Fala(0);
                Exercicio("checkin_location", 1, exerciciosSpeaking[0]);
                Fala(1);
                Exercicio("checkin_directions", 1, exerciciosAlternativas[0]);
                Fala(2);
                roteiro.Acao("EmitirEventoMissao.conversa_encerrada.airport_info_npc", () => { EmitirEventoMissao("conversa_encerrada", "airport_info_npc"); return Task.CompletedTask; });
                roteiro.Acao("ConcluirMissao.aeroporto_a1a2_2", () => { ConcluirMissao("aeroporto_a1a2_2"); return Task.CompletedTask; });
                break;
            case "A2-B1":
                Fala(0);
                Exercicio("checkin_location", 1, exerciciosSpeaking[0]);
                roteiro.Acao("ConcluirMissao.aeroporto_a2b1_1", () => { ConcluirMissao("aeroporto_a2b1_1"); return Task.CompletedTask; });
                roteiro.Acao("IniciarMissao.aeroporto_a2b1_2", () => { IniciarMissao( "aeroporto_a2b1_2", "Find out how to reach the second floor", "Descubra o caminho para chegar ao segundo andar" ); return Task.CompletedTask; });
                Fala(1);
                Exercicio("checkin_directions", 1, exerciciosAlternativas[0]);
                roteiro.Acao("ConcluirMissao.aeroporto_a2b1_2", () => { ConcluirMissao("aeroporto_a2b1_2"); return Task.CompletedTask; });
                roteiro.Acao("IniciarMissao.aeroporto_a2b1_3", () => { IniciarMissao( "aeroporto_a2b1_3", "Find out how long it will take to reach the check-in desks", "Confirme quanto tempo levará para chegar ao check-in" ); return Task.CompletedTask; });
                Fala(2);
                Exercicio("walking_time", 1, exerciciosSpeaking[1]);
                roteiro.Acao("ConcluirMissao.aeroporto_a2b1_3", () => { ConcluirMissao("aeroporto_a2b1_3"); return Task.CompletedTask; });
                Fala(3);
                break;
        }
        return roteiro;
    }

    private async Task AbrirExercicioDoPonto(string pointKey, int ordem, ExercicioBase fallback)
    {
        CheckpointJogo.IdentificarFallback(fallback, pointKey, ordem, nivelAtual);
        TipoExercicio tipo = ObterTipoExercicio(fallback);
        bool? speakingLivre = (fallback as ExercicioSpeaking)?.modoLivre;
        ExerciseCatalogItem item = catalogoExercicios?.Find(pointKey, nivelAtual, ordem, tipo, speakingLivre);
        if (item != null)
            await AbrirExercicio(item.Tipo, item.Exercise);
        else
        {

            Debug.LogWarning($"AirportInfoNPC: usando exerc?cio local em {nivelAtual}/{pointKey}/{ordem}.");
            await AbrirExercicio(tipo, fallback);
        }

        EmitirEventoMissao("exercicio_encerrado", $"{NpcKey}/{pointKey}/{ordem}");
    }

    public async Task AbrirExercicio(TipoExercicio tipo, ExercicioBase ex)
    {
        //Debug.Log("Abrindo exercício: " + tipo);

        ModalLegenda legenda = modalLegenda.GetComponent<ModalLegenda>();

        if (modalLegenda != null)
        {
            modalLegenda.SetActive(false);
        }

        await EntrarModoExercicio(tipo);

        DefinirCursorDesktop(true);

        modalExercicio.Abrir(tipo, ex, this);

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

        await SairModoExercicio(tipo);

        // ✨ Volta a prender o mouse após terminar o exercício (se o diálogo geral não tiver acabado)
        DefinirCursorDesktop(false);

        if (legenda != null)
        {
            legenda.AjustarPosicaoPeloEstadoDoJogo();
        }
    }

    private async Task PlayAudioETexto(int i)
    {
        ultimaFraseDita = dialogoAtual[i];
        await ApresentarFalaDoRoteiro(backendManager, audioSource, idNpcParaVoz,
            nomeExibicaoLegenda, ultimaFraseDita, modalLegenda, textoPularDialogo);
    }

    public override async Task FalarFraseCustomizada(string textoParaFalar)
    {
        if (string.IsNullOrEmpty(textoParaFalar)) return;
        int visita = SessaoCenario.Atual.Versao;
        ValidarVisita(visita);

        byte[] audioBytes = await backendManager.GerarAudio(textoParaFalar, idNpcParaVoz);
        ValidarVisita(visita);
        
        if (audioBytes != null && audioBytes.Length > 0)
        {
            string caminho = Path.Combine(Application.persistentDataPath, "audio_temp_custom.wav");
            File.WriteAllBytes(caminho, audioBytes);
            
            UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + caminho, AudioType.WAV);
            var operation = www.SendWebRequest();
            
            while (!operation.isDone)
                await Task.Yield();

            ValidarVisita(visita);
            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
            audioSource.clip = clip;
            if (modalLegenda != null)
            {
                modalLegenda.SetActive(true);

                if (textoPularDialogo != null)
                    textoPularDialogo.gameObject.SetActive(false);

                modalLegenda.GetComponent<ModalLegenda>()
                    .MostrarLegenda(nomeExibicaoLegenda, textoParaFalar);
            }
            
            await ReproduzirVoz(audioSource, visita);
            ValidarVisita(visita);

            if (modalLegenda != null)
            {
                modalLegenda.SetActive(false);
            }
        }
    }

    private Task RepetirInstrucoesAsync()
    {
        if(nivelAtual == "A1")
        {
            return PlayAudioETexto(1);
        }
        else if(nivelAtual == "A2" || nivelAtual == "B1")
        {
            return PlayAudioETexto(2);
        }
        return Task.CompletedTask;
    }

    public void RepetirInstrucoes()
    {
        _ = RepetirInstrucoesAsync();
    }
}
