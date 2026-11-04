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

public class AirportInfoNPC : InteracaoNPC
{
    string nivelAtual;
    string cumprimento;
    int horaAtual = DateTime.Now.Hour;
    List<string> dialogoAtual;
    private AudioSource audioSource;
    private Animator animator;

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

    private void InicializarConteudosPorNivel()
    {
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

    private void IniciarMissao(string id, string texto, string traducao)
    {
        MissionManager.AdicionarMissaoNPC(id, texto, traducao);
    }

    private void ConcluirMissao(string id)
    {
        MissionManager.ConcluirMissaoNPC(id);
    }

    protected override async Task IniciarInteracao()
    {
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

    private async Task interacaoA1()
    {
        int i = 0;

        IniciarMissao("aeroporto_a1_1", "Chegue ao aeroporto e procure o balcão de informações", "Arrive at the airport and find the information desk");

        await PlayAudioETexto(i++);

        exAtual = exerciciosSpeaking[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        ConcluirMissao("aeroporto_a1_1");
        IniciarMissao("aeroporto_a1_2", "Peça orientações para encontrar o balcão de check-in", "Ask for directions to find the check-in desk");

        await PlayAudioETexto(i++);

        exAtual = exerciciosAlternativas[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        ConcluirMissao("aeroporto_a1_2");
    }

    private async Task interacaoA2()
    {
        int i = 0;

        IniciarMissao("aeroporto_a2_1", "Chegue ao aeroporto e procure o balcão de check-in", "Arrive at the airport and find the check-in desk");

        await PlayAudioETexto(i++);

        exAtual = exerciciosSpeaking[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++);

        exAtual = exerciciosSpeaking[1];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await AbrirExercicio(ObterTipoExercicio(exerciciosWriting[0]), exerciciosWriting[0]);

        await PlayAudioETexto(i++);

        ConcluirMissao("aeroporto_a2_1");
    }

    private async Task interacaoB1()
    {
        int i = 0;

        IniciarMissao("aeroporto_b1_1", "Chegue ao aeroporto e dirija-se ao balcão de informações", "Arrive at the airport and go to the information desk");

        await PlayAudioETexto(i++);

        exAtual = exerciciosSpeaking[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        ConcluirMissao("aeroporto_b1_1");
        IniciarMissao("aeroporto_b1_2", "Peça orientações para encontrar o balcão de check-in", "Ask for directions to find the check-in desk");

        await PlayAudioETexto(i++);

        await AbrirExercicio(ObterTipoExercicio(exerciciosAlternativas[0]), exerciciosAlternativas[0]);

        await PlayAudioETexto(i++);

        exAtual = exerciciosSpeaking[1];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++);

        await PlayAudioETexto(i++);

        exAtual = exerciciosSpeaking[2];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        ConcluirMissao("aeroporto_b1_2");
    }

    public async Task AbrirExercicio(TipoExercicio tipo, ExercicioBase ex)
    {
        //Debug.Log("Abrindo exercício: " + tipo);

        ModalLegenda legenda = modalLegenda.GetComponent<ModalLegenda>();

        if (modalLegenda != null)
        {
            modalLegenda.SetActive(false);
        }

        await EntrarModoExercicio();

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

        await SairModoExercicio();

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
        // 1. PASSANDO O ID DO NPC JUNTO COM O TEXTO PARA O BACKEND
        byte[] audioBytes = await backendManager.GerarAudio(dialogoAtual[i], idNpcParaVoz);
        
        if (audioBytes != null && audioBytes.Length > 0)
        {
            //Debug.Log($"Áudio recebido! Tamanho: {audioBytes.Length} bytes");
            string caminho = Path.Combine(Application.persistentDataPath, "audio_temp.wav");
            File.WriteAllBytes(caminho, audioBytes);
            
            UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + caminho, AudioType.WAV);
            var operation = www.SendWebRequest();
            
            while (!operation.isDone)
                await Task.Yield();

            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
            audioSource.clip = clip;

            if (textoPularDialogo != null)
                textoPularDialogo.gameObject.SetActive(false);

            modalLegenda.GetComponent<ModalLegenda>().MostrarLegenda(nomeExibicaoLegenda, dialogoAtual[i]);
            audioSource.Play();
            while (audioSource.isPlaying)
            {
                await Task.Yield();
            }

            if (textoPularDialogo != null)
            {
                textoPularDialogo.gameObject.SetActive(true);

                DefinirCursorDesktop(true);
            }

            await Task.Yield();

            await AguardarAvancoDialogo();


            if (textoPularDialogo != null)
            {
                textoPularDialogo.gameObject.SetActive(false);
            }

            if (modalLegenda != null)
            {
                modalLegenda.SetActive(false);
            }
            
        }
        else
        {
            Debug.LogError("O servidor retornou um array de bytes vazio.");
            modalLegenda.GetComponent<ModalLegenda>().MostrarLegenda(nomeExibicaoLegenda, dialogoAtual[i]);
        }
    }

    public override async Task FalarFraseCustomizada(string textoParaFalar)
    {
        if (string.IsNullOrEmpty(textoParaFalar)) return;

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

                if (textoPularDialogo != null)
                    textoPularDialogo.gameObject.SetActive(false);

                modalLegenda.GetComponent<ModalLegenda>()
                    .MostrarLegenda(nomeExibicaoLegenda, textoParaFalar);
            }
            
            audioSource.Play();
            
            while (audioSource.isPlaying)
            {
                await Task.Yield();
            }

            if (modalLegenda != null)
            {
                modalLegenda.SetActive(false);
            }
        }
    }
}
