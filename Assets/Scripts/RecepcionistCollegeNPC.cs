// using System.Collections.Generic;
// using System.Threading.Tasks;
// using UnityEngine;
// using UnityEngine.Networking;
// using System.IO;
// using TMPro;
// using System;

// public class RecepcionistCollegeNPC: InteracaoNPC
// {
//     string nivelAtual;
//     string cumprimento;
//     int horaAtual = DateTime.Now.Hour;
//     List<string> dialogoAtual;
//     // private AudioSource audioSource;
//     // private BackendManager backendManager;
//     public ModalExercicio modalExercicio;
//     public GameObject modalLegenda;
//     public TextMeshProUGUI textoPularDialogo;
//     public ExercicioBase exAtual;

//     [System.NonSerialized] public List<ExercicioBase> exerciciosCompletarFrase;
//     [System.NonSerialized] public List<ExercicioBase> exerciciosCriarFrases;
//     [System.NonSerialized] public List<ExercicioBase> exerciciosSpeaking;
//     [System.NonSerialized] public List<ExercicioBase> exerciciosAlternativas;
//     [System.NonSerialized] public List<ExercicioBase> exerciciosWriting;
//     [System.NonSerialized] public List<ExercicioBase> exerciciosAssociacao;

//     public string idNpcParaVoz = "recepcionistCollege";

//     protected override void Start()
//     {
//         base.Start();
//         nivelAtual = DadosJogador.nivelUsuario;
//         nomeExibicaoLegenda = "College Recepcionist";
//         exerciciosCompletarFrase = GarantirListaExercicios(exerciciosCompletarFrase);
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
//         if (nivelAtual == "A1")
//         {
//             dialogoAtual = new List<string>()
//             {
//                 "Hello. Can I help you?",
//                 "The secretary's office is on the first floor",
//                 "Go straight and turn right."
//             };

//             exerciciosCriarFrases.Add(
//                 new ExercicioBlocos()
//                 {
//                     enunciado= "Arrange the words to form the correct sentence in english: Onde fica o escritório da secretária?",
//                     blocosPalavras = new List<string>
//                     {
//                         "Where",
//                         "located",
//                         "manager",
//                         "is",
//                         "secretary's",
//                         "meeting",
//                         "office",
//                         "How",
//                         "floor",
//                         "the",
//                         "reception",
//                         "does",
//                         "find",
//                         "When"
//                     },
//                     respostaCorreta = "Where is the secretary's office located?\n"
//                 }
//             );

//             exerciciosSpeaking.Add(
//                     new ExercicioSpeaking()
//                     {
//                         enunciado = "How do you ask for directions for secretary's office?",
//                         opcoesFala = new List<string>()
//                         {
//                             "How do I get there?\n",
//                             "Where can I find the student cafeteria?\n",
//                             "Is the library open on Sundays?\n"
//                         },
//                         respostaCorreta = 0
//                     }
//             );

//             exerciciosAlternativas.Add(
//                 new ExercicioAlternativas()
//                 {
//                     enunciado = "Choose the correct option to express thanks",
//                     alternativas = new List<string>(){ 
//                         "I am a student\r\n", 
//                         "It is a library\r\n", 
//                         "Thank you\r\n", 
//                         "My class is at eight\r\n" },
//                     alternativaCorreta = 2
//                 }
//             );
//         }
//         else if(nivelAtual=="A2")
//         {
//             dialogoAtual = new List<string>()
//             {
//                 cumprimento+". Welcome to the university. How can I help you?",
//                 "The secretary's office is on the first floor. Go straight and take the first door on your right.",
//                 "You're welcome. Have a nice day!" 
//             };

//             exerciciosCriarFrases.Add(
//                 new ExercicioBlocos()
//                 {
//                     enunciado= "Arrange the words to form the correct sentence in english: Eu gostaria de encontrar o escritório da secretária.",
//                     blocosPalavras = new List<string>
//                     {
//                         "I",
//                         "library",
//                         "secretary's",
//                         "to",
//                         "find",
//                         "the",
//                         "would",
//                         "office",
//                         "classroom",
//                         "teacher",
//                         "Where",
//                         "student",
//                         "is",
//                         "like",
//                     },
//                     respostaCorreta = "I would like to find the secretary's office.\n"
//                 }
//             );

//             exerciciosSpeaking.Add(
//                     new ExercicioSpeaking()
//                     {
//                         enunciado = "You did not understand the directions. Ask the receptionist to repeat them.",
//                         opcoesFala = new List<string>()
//                         {
//                             "Where can I buy a notebook?\n",
//                             "What time does the library close?\n",
//                             "Could you repeat the directions, please?\n"
//                         },
//                         respostaCorreta = 2
//                     }
//             );

//             exerciciosSpeaking.Add(
//                     new ExercicioSpeaking()
//                     {
//                         enunciado = "The secretary helped you. Thank her and say goodbye"
//                     }
//             );
//         }
//         else if(nivelAtual=="B1")
//         {
//             dialogoAtual = new List<string>()
//             {
//                 cumprimento+". Welcome to the university. How may I help you today?",
//                 "Are you a new student?",
//                 "The secretary's office is on the first floor. Go straight past the reception desk and you'll see a door on your right. The secretary can help you with enrollment inside.",
//                 "If you get lost, just come back and I'll help you."
//             };

//             exerciciosSpeaking.Add(
//                     new ExercicioSpeaking()
//                     {
//                         enunciado = "The receptionist asks how she can help you. Explain that you need to find the secretary’s office and say why you are there.",
//                         opcoesFala = new List<string>()
//                         {
//                             " I need to eat the secretary's office because I enroll yesterday.\n",
//                             "I need to go to the secretary's office because I have to enroll in a course. \n",
//                             "I am a course at the second floor.\n"
//                         },
//                         respostaCorreta = 1
//                     }
//             );

//             exerciciosSpeaking.Add(
//                     new ExercicioSpeaking()
//                     {
//                         enunciado = "This is your first visit to the university. Answer her question.",
//                         opcoesFala = new List<string>()
//                         {
//                             "Yes, this is my first time here. \n",
//                             "I need to enroll in Computer Science.\n",
//                             "The library closes at six o’clock.\n"
//                         },
//                         respostaCorreta = 0
//                     }
//             );

//             exerciciosSpeaking.Add(
//                     new ExercicioSpeaking()
//                     {
//                         enunciado = "You are not sure which door is the secretary's office. Ask the receptionist for clarification.",
//                         opcoesFala = new List<string>()
//                         {
//                             "Excuse me, could you tell me what time the secretary’s office opens?\n",
//                             "Excuse me, I don’t know where is the secretary’s office door\n",
//                             "Sorry, could you repeat which door is the secretary’s office?\n"
//                         },
//                         respostaCorreta = 2
//                     }
//             );

//             exerciciosSpeaking.Add(
//                     new ExercicioSpeaking()
//                     {
//                         enunciado = "The secretary has helped you with everything you needed. Thank her for her help and end the conversation politely."
//                     }
//             );
//         }
//     }

//     private void IniciarMissao(string id, string texto, string traducao)
//     {
//         MissionManager.AdicionarMissaoNPC(id, texto, traducao);
//     }

//     protected override async Task IniciarInteracao()
//     {
//         GameProgress.EstaEmDialogo = true;
//         nivelAtual = DadosJogador.nivelUsuario;
//         InicializarConteudosPorNivel();

//         if (nivelAtual == "A1")
//         {
//             IniciarMissao("universidade_a1_recepcao",
//                 "Converse com o recepcionista e descubra como chegar ao escritório da secretaria",
//                 "Talk to the receptionist and find out how to get to the secretary's office");
//             await interacaoA1();
//             MissionManager.ConcluirMissaoNPC("universidade_a1_recepcao");
//         }
//         else if (nivelAtual == "A2")
//         {
//             IniciarMissao("universidade_a2_recepcao",
//                 "Converse com a recepcionista",
//                 "Talk to the receptionist");
//             await interacaoA2();
//             MissionManager.ConcluirMissaoNPC("universidade_a2_recepcao");

//             IniciarMissao("universidade_a2_encontrar_secretaria",
//                 "Encontre o escritório da secretaria",
//                 "Find the secretary's office");
//         }
//         else
//         {
//             IniciarMissao("universidade_b1_orientacoes",
//                 "Peça orientações detalhadas ao recepcionista e esclareça dúvidas sobre a localização do escritório",
//                 "Ask the receptionist for detailed directions and clarify the location of the office");
//             await interacaoB1();
//             MissionManager.ConcluirMissaoNPC("universidade_b1_orientacoes");
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

//     private async Task interacaoA1()
//     {
//         int i=0;

//         await PlayAudioETexto(i++);

//         await AbrirExercicio(ObterTipoExercicio(exerciciosCriarFrases[0]), exerciciosCriarFrases[0]);
        
//         await PlayAudioETexto(i++);

//         await AbrirExercicio(ObterTipoExercicio(exerciciosSpeaking[0]), exerciciosSpeaking[0]);

//         await PlayAudioETexto(i++);

//         await AbrirExercicio(ObterTipoExercicio(exerciciosAlternativas[0]), exerciciosAlternativas[0]);
//     }

//     private async Task interacaoA2()
//     {
//         int i = 0;

//         await PlayAudioETexto(i++);

//         await AbrirExercicio(ObterTipoExercicio(exerciciosCriarFrases[0]), exerciciosCriarFrases[0]);

//         await PlayAudioETexto(i++);

//         await AbrirExercicio(ObterTipoExercicio(exerciciosSpeaking[0]), exerciciosSpeaking[0]);

//         // A recepcionista repete as direções após o pedido do usuário.
//         await PlayAudioETexto(1);

//         await AbrirExercicio(ObterTipoExercicio(exerciciosSpeaking[1]), exerciciosSpeaking[1]);

//         await PlayAudioETexto(i++);
//     }

//     private async Task interacaoB1()
//     {
//         int i = 0;

//         await PlayAudioETexto(i++);

//         await AbrirExercicio(ObterTipoExercicio(exerciciosSpeaking[0]), exerciciosSpeaking[0]);

//         await PlayAudioETexto(i++);

//         await AbrirExercicio(ObterTipoExercicio(exerciciosSpeaking[1]), exerciciosSpeaking[1]);

//         await PlayAudioETexto(i++);

//         await AbrirExercicio(ObterTipoExercicio(exerciciosSpeaking[2]), exerciciosSpeaking[2]);

//         // A recepcionista confirma a informação repetindo as direções detalhadas.
//         await PlayAudioETexto(2);

//         await PlayAudioETexto(i++);

//         await AbrirExercicio(ObterTipoExercicio(exerciciosSpeaking[3]), exerciciosSpeaking[3]);
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

//     private async Task PlayAudioETexto(int indiceDialogo, bool mostrarLegenda = true)
//     {
//         ultimaFraseDita = dialogoAtual[indiceDialogo];
//         byte[] audioBytes = await backendManagerComum.GerarAudio(dialogoAtual[indiceDialogo], idNpcParaVoz);

//         if (audioBytes != null && audioBytes.Length > 0)
//         {
//             Debug.Log($"\u00c1udio recebido! Tamanho: {audioBytes.Length} bytes");
//             string caminho = Path.Combine(Application.persistentDataPath, "audio_temp.wav");
//             File.WriteAllBytes(caminho, audioBytes);

//             UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + caminho, AudioType.WAV);
//             var operation = www.SendWebRequest();

//             while (!operation.isDone)
//                 await Task.Yield();

//             AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
//             audioSourceComum.clip = clip;

//             if (mostrarLegenda)
//             {
//                 textoPularDialogoComum.gameObject.SetActive(false);
//                 modalLegendaComum.GetComponent<ModalLegenda>().MostrarLegenda(nomeExibicaoLegenda, dialogoAtual[indiceDialogo]);
//             }

//             audioSourceComum.Play();
//             while (audioSourceComum.isPlaying)
//             {
//                 await Task.Yield();
//             }

//             if (textoPularDialogoComum != null)
//             {
//                 textoPularDialogoComum.gameObject.SetActive(true);
//                 Cursor.lockState = CursorLockMode.None;
//                 Cursor.visible = true;
//             }

//             await Task.Delay(150);

//             while (Input.GetMouseButton(0))
//             {
//                 await Task.Yield();
//             }

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

//             if (textoPularDialogoComum != null)
//             {
//                 textoPularDialogoComum.gameObject.SetActive(false);
//             }

//             if (modalLegendaComum != null)
//             {
//                 modalLegendaComum.SetActive(false);
//             }
//         }
//         else
//         {
//             Debug.LogError("O servidor retornou um array de bytes vazio.");
//             if (mostrarLegenda)
//             {
//                 modalLegendaComum.GetComponent<ModalLegenda>().MostrarLegenda(nomeExibicaoLegenda, dialogoAtual[indiceDialogo]);
//             }
//         }
//     }

//     public override async Task FalarFraseCustomizada(string textoParaFalar)
//     {
//         if (string.IsNullOrEmpty(textoParaFalar)) return;

//         byte[] audioBytes = await backendManagerComum.GerarAudio(textoParaFalar, idNpcParaVoz);

//         if (audioBytes != null && audioBytes.Length > 0)
//         {
//             string caminho = Path.Combine(Application.persistentDataPath, "audio_temp_custom.wav");
//             File.WriteAllBytes(caminho, audioBytes);

//             UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + caminho, AudioType.WAV);
//             var operation = www.SendWebRequest();

//             while (!operation.isDone)
//                 await Task.Yield();

//             AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
//             audioSourceComum.clip = clip;
//             if (modalLegendaComum != null && modalLegendaComum.activeSelf)
//             {
//                 if (textoPularDialogoComum != null) textoPularDialogoComum.gameObject.SetActive(false);
//                 modalLegendaComum.GetComponent<ModalLegenda>().MostrarLegenda(nomeExibicaoLegenda, textoParaFalar);
//             }

//             audioSourceComum.Play();

//             while (audioSourceComum.isPlaying)
//             {
//                 await Task.Yield();
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

public class RecepcionistCollegeNPC: InteracaoNPC, IRoteiroNpcCheckpoint
{
    string nivelAtual;
    string cumprimento;
    int horaAtual = DateTime.Now.Hour;
    List<string> dialogoAtual;
    private bool instrucoesJaApresentadas;
    private const string NpcKey = MissoesCollege.Recepcao;
    private int ultimaVisitaCollege;
    private ExerciseLoadResult catalogoExercicios;
    // private AudioSource audioSource;
    // private BackendManager backendManager;
    public ModalExercicio modalExercicio;
    public GameObject modalLegenda;
    public TextMeshProUGUI textoPularDialogo;
    public ExercicioBase exAtual;

    [System.NonSerialized] public List<ExercicioBase> exerciciosCompletarFrase;
    [System.NonSerialized] public List<ExercicioBase> exerciciosCriarFrases;
    [System.NonSerialized] public List<ExercicioBase> exerciciosSpeaking;
    [System.NonSerialized] public List<ExercicioBase> exerciciosAlternativas;
    [System.NonSerialized] public List<ExercicioBase> exerciciosWriting;
    [System.NonSerialized] public List<ExercicioBase> exerciciosAssociacao;

    public string idNpcParaVoz = "recepcionistCollege";

    protected override void Start()
    {
        base.Start();
        nivelAtual = DadosJogador.nivelUsuario;
        nomeExibicaoLegenda = "College Recepcionist";
        exerciciosCompletarFrase = GarantirListaExercicios(exerciciosCompletarFrase);
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
        var exerciciosCriarFrases = descricao?.exerciciosCriarFrases ?? this.exerciciosCriarFrases;
        var exerciciosSpeaking = descricao?.exerciciosSpeaking ?? this.exerciciosSpeaking;
        var exerciciosAlternativas = descricao?.exerciciosAlternativas ?? this.exerciciosAlternativas;
        var exerciciosWriting = descricao?.exerciciosWriting ?? this.exerciciosWriting;
        var cumprimento = descricao == null ? this.cumprimento : "{{greeting}}";

        if (nivelAtual == "A1")
        {
            dialogoAtual = new List<string>()
            {
                "Hello. Can I help you?",
                "The secretary's office is on the first floor",
                "Go straight and turn right."
            };

            exerciciosCriarFrases.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Arrange the words to form the correct sentence in english: Onde fica o escritório da secretária?",
                    blocosPalavras = new List<string>
                    {
                        "Where",
                        "located",
                        "manager",
                        "is",
                        "secretary's",
                        "meeting",
                        "office",
                        "How",
                        "floor",
                        "the",
                        "reception",
                        "does",
                        "find",
                        "When"
                    },
                    respostaCorreta = "Where is the secretary's office located?\n"
                }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "How do you ask for directions for secretary's office?",
                        opcoesFala = new List<string>()
                        {
                            "How do I get there?\n",
                            "Where can I find the student cafeteria?\n",
                            "Is the library open on Sundays?\n"
                        },
                        respostaCorreta = 0
                    }
            );

            exerciciosAlternativas.Add(
                new ExercicioAlternativas()
                {
                    enunciado = "Choose the correct option to express thanks",
                    alternativas = new List<string>(){ 
                        "I am a student\r\n", 
                        "It is a library\r\n", 
                        "Thank you\r\n", 
                        "My class is at eight\r\n" },
                    alternativaCorreta = 2
                }
            );
        }
        else if(nivelAtual=="A2")
        {
            dialogoAtual = new List<string>()
            {
                cumprimento+". Welcome to the university. How can I help you?",
                "The secretary's office is on the first floor. Go straight and take the first door on your right.",
                "You're welcome. Have a nice day!" 
            };

            exerciciosCriarFrases.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Arrange the words to form the correct sentence in english: Eu gostaria de encontrar o escritório da secretária.",
                    blocosPalavras = new List<string>
                    {
                        "I",
                        "library",
                        "secretary's",
                        "to",
                        "find",
                        "the",
                        "would",
                        "office",
                        "classroom",
                        "teacher",
                        "Where",
                        "student",
                        "is",
                        "like",
                    },
                    respostaCorreta = "I would like to find the secretary's office.\n"
                }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "You did not understand the directions. Ask the receptionist to repeat them.",
                        opcoesFala = new List<string>()
                        {
                            "Where can I buy a notebook?\n",
                            "What time does the library close?\n",
                            "Could you repeat the directions, please?\n"
                        },
                        respostaCorreta = 2
                    }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        modoLivre = true,
                        enunciado = "The secretary helped you. Thank her and say goodbye"
                    }
            );
        }
        else if(nivelAtual=="B1")
        {
            dialogoAtual = new List<string>()
            {
                cumprimento+". Welcome to the university. How may I help you today?",
                "Are you a new student?",
                "The secretary's office is on the first floor. Go straight past the reception desk and you'll see a door on your right. The secretary can help you with enrollment inside.",
                "If you get lost, just come back and I'll help you."
            };

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "The receptionist asks how she can help you. Explain that you need to find the secretary’s office and say why you are there.",
                        opcoesFala = new List<string>()
                        {
                            " I need to eat the secretary's office because I enroll yesterday.\n",
                            "I need to go to the secretary's office because I have to enroll in a course. \n",
                            "I am a course at the second floor.\n"
                        },
                        respostaCorreta = 1
                    }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "This is your first visit to the university. Answer her question.",
                        opcoesFala = new List<string>()
                        {
                            "Yes, this is my first time here. \n",
                            "I need to enroll in Computer Science.\n",
                            "The library closes at six o’clock.\n"
                        },
                        respostaCorreta = 0
                    }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "You are not sure which door is the secretary's office. Ask the receptionist for clarification.",
                        opcoesFala = new List<string>()
                        {
                            "Excuse me, could you tell me what time the secretary’s office opens?\n",
                            "Excuse me, I don’t know where is the secretary’s office door\n",
                            "Sorry, could you repeat which door is the secretary’s office?\n"
                        },
                        respostaCorreta = 2
                    }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        modoLivre = true,
                        enunciado = "The secretary has helped you with everything you needed. Thank her for her help and end the conversation politely."
                    }
            );
        }
        else if (nivelAtual == "A1-A2")
{
    dialogoAtual = new List<string>()
    {
        cumprimento + ". Welcome to the college. Are you looking for a specific office?",
        "The secretary's office is on the first floor. Go straight and turn right after the reception desk.",
        "You will see the secretary's office at the end of the hallway. She can help you with your enrollment.",
        "If you have any questions, you can come back here. Have a nice day!"
    };

    exerciciosSpeaking.Add(
        new ExercicioSpeaking()
        {
            enunciado = "You need to enroll in a course. Explain what you are looking for.",
            opcoesFala = new List<string>()
            {
                "I'm looking for the library because I want to pay my teacher.\n",
                "I'm looking for the secretary's office because I want to enroll in a course.\n",
                "I'm looking for the cafeteria because I want to enroll in a classroom.\n"
            },
            respostaCorreta = 1
        }
    );

    exerciciosCriarFrases.Add(
        new ExercicioBlocos()
        {
            enunciado = "Arrange the words to form the correct sentence in English: Onde fica o escritório da secretaria?",
            blocosPalavras = new List<string>()
            {
                "Where",
                "the",
                "secretary's",
                "office",
                "is",
                "located",
                "classroom",
                "floor",
                "How",
                "library"
            },
            respostaCorreta = "Where is the secretary's office located?\n"
        }
    );

    exerciciosSpeaking.Add(
        new ExercicioSpeaking()
        {
            enunciado = "The receptionist gives you directions. Ask her to explain the route again.",
            opcoesFala = new List<string>()
            {
                "Could you enroll me in a course, please?\n",
                "Could you tell me the tuition fee, please?\n",
                "Could you repeat the directions, please?\n"
            },
            respostaCorreta = 2
        }
    );

    exerciciosAlternativas.Add(
        new ExercicioAlternativas()
        {
            enunciado = "According to the receptionist, where is the secretary's office?",
            alternativas = new List<string>()
            {
                "On the ground floor, behind the cafeteria\r\n",
                "On the third floor, near the classrooms\r\n",
                "On the first floor, at the end of the hallway\r\n",
                "On the second floor, next to the library\r\n"
            },
            alternativaCorreta = 2
        }
    );

    exerciciosSpeaking.Add(
        new ExercicioSpeaking()
        {
            enunciado = "You understood the directions. Thank the receptionist before leaving.",
            opcoesFala = new List<string>()
            {
                "I would like to change my course yesterday.\n",
                "My classroom is helping the reception desk.\n",
                "Thank you for your help. I'll go there now.\n"
            },
            respostaCorreta = 2
        }
    );
}
else if (nivelAtual == "A2-B1")
{
    dialogoAtual = new List<string>()
    {
        cumprimento + ". Welcome to the college. Are you here to speak with the secretary?",
        "Her office is on the first floor. Walk past the reception desk and take the hallway on your right.",
        "Continue to the end of the hallway. You will find her office on your left, next to the administrative office.",
        "She can assist you with enrollment and answer questions about the courses.",
        "If you have trouble finding the office, feel free to come back and ask me for help."
    };

    exerciciosSpeaking.Add(
        new ExercicioSpeaking()
        {
            enunciado = "Explain to the receptionist why you need to speak with the secretary.",
            opcoesFala = new List<string>()
            {
                "I need to speak with the library because I'm interested in paying the reception.\n",
                "I need to speak with the secretary because I'm interested in enrolling in a course.\n",
                "I need to speak with the cafeteria because I'm interested in the hallway.\n"
            },
            respostaCorreta = 1
        }
    );

    exerciciosWriting.Add(
        new ExercicioWriting()
        {
            enunciado = "The receptionist asks which office you need to visit. Write a complete sentence explaining that you need to go to the secretary's office to complete your enrollment."
        }
    );

    exerciciosSpeaking.Add(
        new ExercicioSpeaking()
        {
            enunciado = "You want to make sure you understood the route correctly. Ask the receptionist to clarify where you should turn.",
            opcoesFala = new List<string>()
            {
                "Could you clarify how much the course costs per month?\n",
                "Could you clarify when the library closes?\n",
                "Could you clarify which hallway I should take?\n"
            },
            respostaCorreta = 2
        }
    );

    exerciciosCriarFrases.Add(
        new ExercicioBlocos()
        {
            enunciado = "Arrange the words to form the correct sentence in English: Continue até o final do corredor e vire à esquerda.",
            blocosPalavras = new List<string>()
            {
                "Continue",
                "to",
                "the",
                "end",
                "of",
                "the",
                "hallway",
                "and",
                "turn",
                "left",
                "right",
                "before"
            },
            respostaCorreta = "Continue to the end of the hallway and turn left.\n"
        }
    );

    exerciciosAlternativas.Add(
        new ExercicioAlternativas()
        {
            enunciado = "According to the receptionist, what can the secretary help you with?",
            alternativas = new List<string>()
            {
                "Library opening hours and book returns\r\n",
                "Cafeteria payments and meal schedules\r\n",
                "Classroom cleaning and building maintenance\r\n",
                "Enrollment and course-related questions\r\n"
            },
            alternativaCorreta = 3
        }
    );

    exerciciosSpeaking.Add(
        new ExercicioSpeaking()
        {
            enunciado = "You have understood the directions. Respond politely before going to the secretary's office.",
            opcoesFala = new List<string>()
            {
                "Thank you for enrolling me in the course yesterday.\n",
                "Thank you for confirming my classroom schedule.\n",
                "Thank you for explaining the route. I'll head there now.\n"
            },
            respostaCorreta = 2
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

    protected override bool PermitirReinteracaoAposConclusao => true;

    protected override Task IniciarInteracao() => ExecutarAtendimento(0);

    private async Task ExecutarAtendimento(int inicio)
    {
        int versao = SessaoCenario.Atual.Versao;
        GameProgress.EstaEmDialogo = true;
        int visita = MissionManager.Instance?.VisitaCollege ?? 0;
        if (visita != 0 && visita != ultimaVisitaCollege)
        {
            instrucoesJaApresentadas = false;
            ultimaVisitaCollege = visita;
        }

        try
        {
            if (instrucoesJaApresentadas)
            {
                await RepetirInstrucoesAsync();
                return;
            }

            nivelAtual = DadosJogador.nivelUsuario;
            InicializarConteudosPorNivel();
            catalogoExercicios = await ExerciseManager.Instance.LoadForNpcAsync(NpcKey, nivelAtual);
            if (!catalogoExercicios.Success)
                Debug.LogWarning("RecepcionistCollegeNPC: usando exercícios locais. " + catalogoExercicios.Error);

            ValidarVisita(versao);
            await CriarRoteiro(versao).Executar(inicio);

            instrucoesJaApresentadas = true;
            MissionManager.Instance?.RegistrarEventoCollege(MissoesCollege.FimConversa, NpcKey);
            CheckpointJogo.Instance?.CapturarCaminhoCollege(false);
        }
        finally
        {
            GameProgress.EstaEmDialogo = false;
            if (modalLegenda != null)
                modalLegenda.SetActive(false);
            if (textoPularDialogo != null)
                textoPularDialogo.gameObject.SetActive(false);

            LiberarControlePlayer();
        }
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
        if (cursor.Situacao.Length != 1)
            throw new System.IO.InvalidDataException("Estado do roteiro da recepção inválido.");
        await RoteiroNpcCheckpoint.ExigirCatalogo(this, versao, token);
        return async () =>
        {
            instrucoesJaApresentadas = false;
            TravarControlePlayer();
            await ExecutarAtendimento(cursor.Indice);
        };
    }

    internal void RetomarAposAtendimentoCheckpoint(int visita)
    {
        ValidarVisita(visita);
        AtualizarVisita();
        ultimaVisitaCollege = MissionManager.Instance?.VisitaCollege ?? 0;
        nivelAtual = SessaoCenario.Atual.Nivel;
        InicializarConteudosPorNivel();
        instrucoesJaApresentadas = true;
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
        var exerciciosCriarFrases = descricao?.exerciciosCriarFrases ?? this.exerciciosCriarFrases;
        var exerciciosSpeaking = descricao?.exerciciosSpeaking ?? this.exerciciosSpeaking;
        var exerciciosAlternativas = descricao?.exerciciosAlternativas ?? this.exerciciosAlternativas;
        var exerciciosWriting = descricao?.exerciciosWriting ?? this.exerciciosWriting;

        var roteiro = new RoteiroNpcCheckpoint(this, visita, () => new[] { RoteiroNpcCheckpoint.ChaveSaudacao(cumprimento) });
        void Fala(int indice, bool mostrarLegenda = true) => roteiro.Fala(dialogoAtual[indice], () => PlayAudioETexto(indice, mostrarLegenda));
        void Exercicio(string ponto, int ordem, ExercicioBase fallback) =>
            roteiro.Exercicio(ponto, ordem, () => AbrirExercicioDoPonto(ponto, ordem, fallback), modelo: fallback);

        switch (nivelAtual)
        {
            case "A1":
                Fala(0);
                Exercicio("office_location", 1, exerciciosCriarFrases[0]);
                Fala(1);
                Exercicio("directions_request", 1, exerciciosSpeaking[0]);
                Fala(2);
                Exercicio("closing", 1, exerciciosAlternativas[0]);
                break;
            case "A2":
                Fala(0);
                Exercicio("office_location", 1, exerciciosCriarFrases[0]);
                Fala(1);
                Exercicio("directions_clarification", 1, exerciciosSpeaking[0]);
                Fala(1);
                Exercicio("closing", 1, exerciciosSpeaking[1]);
                Fala(2);
                break;
            case "B1":
                Fala(0);
                Exercicio("visit_purpose", 1, exerciciosSpeaking[0]);
                Fala(1);
                Exercicio("first_visit", 1, exerciciosSpeaking[1]);
                Fala(2);
                Exercicio("directions_clarification", 1, exerciciosSpeaking[2]);
                Fala(2);
                Fala(3);
                Exercicio("closing", 1, exerciciosSpeaking[3]);
                break;
            case "A1-A2":
                Fala(0);
                Exercicio("visit_purpose", 1, exerciciosSpeaking[0]);
                Fala(1);
                Exercicio("office_location", 1, exerciciosCriarFrases[0]);
                Fala(2);
                Exercicio("directions_clarification", 1, exerciciosSpeaking[1]);
                Exercicio("office_location", 2, exerciciosAlternativas[0]);
                Fala(3);
                Exercicio("closing", 1, exerciciosSpeaking[2]);
                break;
            case "A2-B1":
                Fala(0);
                Exercicio("visit_purpose", 1, exerciciosSpeaking[0]);
                Fala(1);
                Exercicio("visit_purpose", 2, exerciciosWriting[0]);
                Fala(2);
                Exercicio("directions_clarification", 1, exerciciosSpeaking[1]);
                Fala(3);
                Exercicio("route_confirmation", 1, exerciciosCriarFrases[0]);
                Fala(4);
                Exercicio("secretary_services", 1, exerciciosAlternativas[0]);
                Exercicio("closing", 1, exerciciosSpeaking[2]);
                break;
        }
        return roteiro;
    }

    private Task AbrirExercicioDoPonto(string pointKey, int ordem, ExercicioBase fallback)
    {
        CheckpointJogo.IdentificarFallback(fallback, pointKey, ordem, nivelAtual);
        TipoExercicio tipo = ObterTipoExercicio(fallback);
        bool? speakingLivre = (fallback as ExercicioSpeaking)?.modoLivre;
        ExerciseCatalogItem item = catalogoExercicios?.Find(pointKey, nivelAtual, ordem, tipo, speakingLivre);
        if (item != null)
            return AbrirExercicio(item.Tipo, item.Exercise);

        Debug.LogWarning($"RecepcionistCollegeNPC: usando exerc?cio local em {nivelAtual}/{pointKey}/{ordem}.");
        return AbrirExercicio(tipo, fallback);
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

    private async Task PlayAudioETexto(int indiceDialogo, bool mostrarLegenda = true)
    {
        ultimaFraseDita = dialogoAtual[indiceDialogo];
        byte[] audioBytes = await backendManagerComum.GerarAudio(dialogoAtual[indiceDialogo], idNpcParaVoz);

        if (audioBytes != null && audioBytes.Length > 0)
        {
            Debug.Log($"\u00c1udio recebido! Tamanho: {audioBytes.Length} bytes");
            string caminho = Path.Combine(Application.persistentDataPath, "audio_temp.wav");
            File.WriteAllBytes(caminho, audioBytes);

            UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + caminho, AudioType.WAV);
            var operation = www.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();

            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
            audioSourceComum.clip = clip;

            if (mostrarLegenda)
            {
                textoPularDialogoComum.gameObject.SetActive(false);
                modalLegendaComum.GetComponent<ModalLegenda>().MostrarLegenda(nomeExibicaoLegenda, dialogoAtual[indiceDialogo]);
            }

            audioSourceComum.Play();
            while (audioSourceComum.isPlaying)
            {
                await Task.Yield();
            }

            if (textoPularDialogoComum != null)
            {
                textoPularDialogoComum.gameObject.SetActive(true);
                DefinirCursorDesktop(true);
            }

            await Task.Delay(150);

            while (Input.GetMouseButton(0))
            {
                await Task.Yield();
            }

            await AguardarAvancoDialogo();

            if (textoPularDialogoComum != null)
            {
                textoPularDialogoComum.gameObject.SetActive(false);
            }

            if (modalLegendaComum != null)
            {
                modalLegendaComum.SetActive(false);
            }
        }
        else
        {
            Debug.LogError("O servidor retornou um array de bytes vazio.");
            if (mostrarLegenda)
            {
                modalLegendaComum.GetComponent<ModalLegenda>().MostrarLegenda(nomeExibicaoLegenda, dialogoAtual[indiceDialogo]);
            }
        }
    }

    public override async Task FalarFraseCustomizada(string textoParaFalar)
    {
        if (string.IsNullOrEmpty(textoParaFalar)) return;
        int visita = SessaoCenario.Atual.Versao;
        ValidarVisita(visita);

        byte[] audioBytes = await backendManagerComum.GerarAudio(textoParaFalar, idNpcParaVoz);
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
            audioSourceComum.clip = clip;
            if (modalLegendaComum != null && modalLegendaComum.activeSelf)
            {
                if (textoPularDialogoComum != null) textoPularDialogoComum.gameObject.SetActive(false);
                modalLegendaComum.GetComponent<ModalLegenda>().MostrarLegenda(nomeExibicaoLegenda, textoParaFalar);
            }

            audioSourceComum.Play();

            while (audioSourceComum.isPlaying)
            {
                await Task.Yield();
            }
            ValidarVisita(visita);
        }
    }

    private Task RepetirInstrucoesAsync()
    {
        if (nivelAtual == "A1" || nivelAtual == "B1")
            return PlayAudioETexto(2);

        if (nivelAtual == "A2")
            return PlayAudioETexto(1);
        if (nivelAtual == "A1-A2")
            return PlayAudioETexto(2);

        if (nivelAtual == "A2-B1")
            return PlayAudioETexto(2);

        return Task.CompletedTask;
    }

    public void RepetirInstrucoes()
    {
        //O '_' descarta o valor retornado prlo RepetirInstrucoesAsync, pois não será usado depois. Isso é útil quando queremos chamar um método assíncrono sem aguardar sua conclusão.
        _ = RepetirInstrucoesAsync();
    }
}
