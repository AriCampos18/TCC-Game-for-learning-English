// using System.Collections.Generic;
// using System.Threading.Tasks;
// using UnityEngine;
// using UnityEngine.Networking;
// using System.IO;
// using TMPro;
// using System;

// public class motherNPC : InteracaoNPC
// {
//     string nivelAtual;
//     string cumprimento;
//     int horaAtual = DateTime.Now.Hour;
//     List<string> dialogoAtual;
//     private AudioSource audioSource;
//     private BackendManager backendManager;
//     public ModalExercicio modalExercicio;
//     public GameObject modalLegenda;
//     public TextMeshProUGUI textoPularDialogo;
//     public ExercicioBase exAtual;

//     public string idNpcParaVoz = "mom";

//     [SerializeField] private FamiliaManager familiaManager;

//     [System.NonSerialized] public List<ExercicioBase> exerciciosCompletarFrase;
//     [System.NonSerialized] public List<ExercicioBase> exerciciosCriarFrases;
//     [System.NonSerialized] public List<ExercicioBase> exerciciosSpeaking;
//     [System.NonSerialized] public List<ExercicioBase> exerciciosAlternativas;
//     [System.NonSerialized] public List<ExercicioBase> exerciciosWriting;

//     [System.NonSerialized] public List<ExercicioBase> exerciciosAssociacao;

//     protected override void Start()
//     {
//         base.Start();
//         nivelAtual = DadosJogador.nivelUsuario;
//         nomeExibicaoLegenda = "Mom";
//         exerciciosCompletarFrase = GarantirListaExercicios(exerciciosCompletarFrase);
//         exerciciosCriarFrases = GarantirListaExercicios(exerciciosCriarFrases);
//         exerciciosSpeaking = GarantirListaExercicios(exerciciosSpeaking);
//         exerciciosAlternativas = GarantirListaExercicios(exerciciosAlternativas);
//         exerciciosWriting = GarantirListaExercicios(exerciciosWriting);
//         exerciciosAssociacao = GarantirListaExercicios(exerciciosAssociacao);
//     }

//     private void InicializarConteudosPorNivel()
//     {
//         if (nivelAtual == "A1")
//         {
//             dialogoAtual = new List<string>()
//             {
//                 "Hey. How are you?",
//                 "How was your day?",
//                 "Look! We have a new bathroom.",
//                 "How is your work?",
//                 "Dinner is ready!"
//             };

//             exerciciosSpeaking.Add(
//                     new ExercicioSpeaking()
//                     {
//                         enunciado = "Choose the correct response to the question.",
//                         opcoesFala = new List<string>()
//                         {
//                             "The bathroom is next to the kitchen\n",
//                             "I would like to book a flight to London\n",
//                             "I’m fine, thank you\n"
//                         },
//                         respostaCorreta = 2
//                     }
//             ); 

//             exerciciosAlternativas.Add(
//                 new ExercicioAlternativas()
//                 {
//                         enunciado = "Choose the correct answer to the question.",
//                         alternativas = new List<string>() { 
//                         "This is my dog\r\n", 
//                         "I is good\r\n", 
//                         "It was good\r\n", 
//                         "It is a bathroom\r\n" },
//                         alternativaCorreta = 2
//                 }
//             );  

//             exerciciosSpeaking.Add(
//                     new ExercicioSpeaking()
//                     {
//                         enunciado = "Say something nice about the bathroom"
//                     }
//             ); 

//             exerciciosSpeaking.Add(
//                     new ExercicioSpeaking()
//                     {
//                         enunciado = "Tell about your day at work",
//                         opcoesFala = new List<string>()
//                         {
//                             "I’m looking for a new job?\n",
//                             "I had a busy day at the office\n",
//                             "Yes, please. Could you help me?\n"
//                         },
//                         respostaCorreta = 1
//                     }
//             );

//             exerciciosSpeaking.Add(
//                     new ExercicioSpeaking()
//                     {
//                         enunciado = "Say goodbye to your family"
//                     }
//             );

//             exerciciosAssociacao.Add(
//                 new ExercicioAssociacao()
//                 {
//                     enunciado = "Match the Portuguese word with the English word.",

//                     opcoesPort = new List<string>
//                     {
//                         "banheiro",
//                         "sala de estar",
//                         "mãe",
//                         "pai",
//                         "irmão",
//                         "cachorro",
//                         "bola",
//                         "mesa",
//                         "jantar"
//                     },

//                     opcoesIngles = new List<string>
//                     {
//                         "dog",
//                         "father",
//                         "dinner",
//                         "living room",
//                         "ball",
//                         "bathroom",
//                         "table",
//                         "brother",
//                         "mother"
//                     },

//                     associacaoCorreta = new List<ParAssociacao>
//                     {
//                         new ParAssociacao { palavraPortugues = "banheiro", palavraIngles = "bathroom" },
//                         new ParAssociacao { palavraPortugues = "sala de estar", palavraIngles = "living room" },
//                         new ParAssociacao { palavraPortugues = "mãe", palavraIngles = "mother" },
//                         new ParAssociacao { palavraPortugues = "pai", palavraIngles = "father" },
//                         new ParAssociacao { palavraPortugues = "irmão", palavraIngles = "brother" },
//                         new ParAssociacao { palavraPortugues = "cachorro", palavraIngles = "dog" },
//                         new ParAssociacao { palavraPortugues = "bola", palavraIngles = "ball" },
//                         new ParAssociacao { palavraPortugues = "mesa", palavraIngles = "table" },
//                         new ParAssociacao { palavraPortugues = "jantar", palavraIngles = "dinner" }
//                     }
//                 }
//             );
//         }
//         else if(nivelAtual=="A2")
//         {
//             dialogoAtual = new List<string>()
//             {
//                 "Hi! It's so good to see you. How have you been?",
//                 "I’m great, thanks. How was your day?",
//                 "We changed the room while you were away. Do you like it?",
//                 "Dinner is ready. Come to the table, please."
//             };

//             exerciciosSpeaking.Add(
//                     new ExercicioSpeaking()
//                     {
//                         enunciado = "Tell your family how you have been feeling and ask how they are doing",
//                         opcoesFala = new List<string>()
//                         {
//                             "The dog needs a new ball to play with\n",
//                             "I've been good. How about you?\n",
//                             "I would like to reserve a table for two\n"
//                         },
//                         respostaCorreta = 1
//                     }
//             );

//             exerciciosWriting.Add(
//                 new ExercicioWriting()
//                 {
//                     enunciado = "Talk about something you did during the day"
//                 }
//             );

//             exerciciosAlternativas.Add(
//                 new ExercicioAlternativas()
//                 {
//                     enunciado = "Which object can you find in the room?",
//                     alternativas = new List<string>() { 
//                         "Shower\r\n", 
//                         "Bedside table\r\n", 
//                         "Dining table\r\n", 
//                         "Refrigerator\r\n" },
//                     alternativaCorreta = 1
//                 }
//             );

//             exerciciosSpeaking.Add(
//                 new ExercicioSpeaking()
//                 {
//                     enunciado = "Describe what you like about the room using a complete sentence."
//                 }
//             );

//             exerciciosCriarFrases.Add(
//                 new ExercicioBlocos()
//                 {
//                     enunciado= "Arrange the words to form the correct sentence in english: Obrigado pelo jantar. Foi ótimo ver vocês.",
//                     blocosPalavras = new List<string>
//                     {
//                         "you",
//                         "great",
//                         "Thanks",
//                         "all",
//                         "for",
//                         "was",
//                         "lunch",
//                         "It",
//                         "see",
//                         "dinner",
//                         "nice",
//                         "to",
//                         "you",
//                         "meet",
//                         "everyone"
//                     },
//                     respostaCorreta = "Thanks for dinner. It was great to see you all.\n"
//                 }
//             );

//             exerciciosAssociacao.Add(
//                 new ExercicioAssociacao()
//                 {
//                     enunciado = "Match the Portuguese word or expression with the English equivalent.",

//                     opcoesPort = new List<string>
//                     {
//                         "mudanças",
//                         "ultimamente",
//                         "ocupado",
//                         "passar tempo",
//                         "preferir",
//                         "chuveiro",
//                         "mesa de cabeceira",
//                         "rotina",
//                         "animal de estimação",
//                         "durante o jantar"
//                     },

//                     opcoesIngles = new List<string>
//                     {
//                         "spend time",
//                         "bedside table",
//                         "lately",
//                         "pet",
//                         "shower",
//                         "prefer",
//                         "busy",
//                         "during dinner",
//                         "changes",
//                         "routine"
//                     },

//                     associacaoCorreta = new List<ParAssociacao>
//                     {
//                         new ParAssociacao { palavraPortugues = "mudanças", palavraIngles = "changes" },
//                         new ParAssociacao { palavraPortugues = "ultimamente", palavraIngles = "lately" },
//                         new ParAssociacao { palavraPortugues = "ocupado", palavraIngles = "busy" },
//                         new ParAssociacao { palavraPortugues = "passar tempo", palavraIngles = "spend time" },
//                         new ParAssociacao { palavraPortugues = "preferir", palavraIngles = "prefer" },
//                         new ParAssociacao { palavraPortugues = "chuveiro", palavraIngles = "shower" },
//                         new ParAssociacao { palavraPortugues = "mesa de cabeceira", palavraIngles = "bedside table" },
//                         new ParAssociacao { palavraPortugues = "rotina", palavraIngles = "routine" },
//                         new ParAssociacao { palavraPortugues = "animal de estimação", palavraIngles = "pet" },
//                         new ParAssociacao { palavraPortugues = "durante o jantar", palavraIngles = "during dinner" }
//                     }
//                 }
//             );
//         }
//         else if(nivelAtual=="B1")
//         {
//             dialogoAtual = new List<string>()
//             {
//                 "Hey! It's been a while.",
//                 "We made a few changes to the kitchen while you were away. What do you think?",
//                 "It's been good. My team is working on an interesting project, so the days go by quickly.",
//                 "Dinner is ready, everyone. Come to the table, please!",
//                 "It was really nice having you here."
//             };

//             exerciciosWriting.Add(
//                 new ExercicioWriting()
//                 {
//                     enunciado = "Write a short message complimenting your family on the new kitchen and describing what you like about it"
//                 }
//             );

//             exerciciosSpeaking.Add(
//                     new ExercicioSpeaking()
//                     {
//                         enunciado = "Ask your mother about her recent experiences at work",
//                         opcoesFala = new List<string>()
//                         {
//                             "How have things been going at work, Mom?\n",
//                             "Have you had a chance to buy any new clothes lately?\n",
//                             "What kind of work does Mom usually do?\n"
//                         },
//                         respostaCorreta = 0
//                     }
//             );

//             exerciciosSpeaking.Add(
//                     new ExercicioSpeaking()
//                     {
//                         enunciado = "Say goodbye to your family after dinner, expressing appreciation for the time you spent together.",
//                         opcoesFala = new List<string>()
//                         {
//                             "It was boring to see you all. I’ll call you soon\n",
//                             "It was great seeing everyone again. Thank you for having me\n",
//                             "Thanks for dinner. I’ll see you at work tomorrow\n"
//                         },
//                         respostaCorreta = 1
//                     }
//             );

//             exerciciosAssociacao.Add(
//                 new ExercicioAssociacao()
//                 {
//                     enunciado = "Match the Portuguese word or expression with the English equivalent.",

//                     opcoesPort = new List<string>
//                     {
//                         "saudade",
//                         "ultimamente",
//                         "equipe",
//                         "projeto",
//                         "conquistas",
//                         "quintal",
//                         "brincar de buscar",
//                         "ficar durante a noite",
//                         "voltar para casa",
//                         "colega"
//                     },

//                     opcoesIngles = new List<string>
//                     {
//                         "backyard",
//                         "spend the night",
//                         "team",
//                         "colleague",
//                         "lately",
//                         "archievements",
//                         "miss",
//                         "project",
//                         "play fetch",
//                         "go back home"
//                     },

//                     associacaoCorreta = new List<ParAssociacao>
//                     {
//                         new ParAssociacao { palavraPortugues = "saudade", palavraIngles = "miss" },
//                         new ParAssociacao { palavraPortugues = "ultimamente", palavraIngles = "lately" },
//                         new ParAssociacao { palavraPortugues = "equipe", palavraIngles = "team" },
//                         new ParAssociacao { palavraPortugues = "projeto", palavraIngles = "project" },
//                         new ParAssociacao { palavraPortugues = "conquistas", palavraIngles = "archievements" },
//                         new ParAssociacao { palavraPortugues = "quintal", palavraIngles = "backyard" },
//                         new ParAssociacao { palavraPortugues = "brincar de buscar", palavraIngles = "play fetch" },
//                         new ParAssociacao { palavraPortugues = "ficar durante a noite", palavraIngles = "spend the night" },
//                         new ParAssociacao { palavraPortugues = "voltar para casa", palavraIngles = "go back home" },
//                         new ParAssociacao { palavraPortugues = "colega", palavraIngles = "colleague" }
//                     }
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

//     protected override async Task IniciarInteracao()
//     {
//         if (familiaManager == null)
//         {
//             Debug.LogError("FamiliaManager não foi atribuído à mãe!");
//             LiberarControlePlayer();
//             return;
//         }

//         await familiaManager.IniciarVisita();
//     }

//     private async Task interacaoA1()
//     {
//         int i = 0;

//         await PlayAudioETexto(i++);

//         exAtual = exerciciosSpeaking[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

//         await PlayAudioETexto(i++);

//         exAtual = exerciciosAlternativas[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

//         await PlayAudioETexto(i++);

//         //misão de olhar o bathroom

//         exAtual = exerciciosWriting[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

//         await PlayAudioETexto(i++);

//         exAtual = exerciciosSpeaking[1];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

//         await PlayAudioETexto(i++); 

//         //jantar

//         exAtual = exerciciosSpeaking[2];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         await AbrirExercicio(ObterTipoExercicio(exerciciosAssociacao[0]), exerciciosAssociacao[0]);
//     }

//     private async Task interacaoA2()
//     {
//         int i = 0;

//         await PlayAudioETexto(i++);
        
//         exAtual = exerciciosSpeaking[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

//         await PlayAudioETexto(i++);
        
//         exAtual = exerciciosWriting[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

//         await PlayAudioETexto(i++);

//         //ir até o quarto

//         await AbrirExercicio(ObterTipoExercicio(exerciciosAlternativas[0]), exerciciosAlternativas[0]);

//         //volta para a sala

//         await AbrirExercicio(ObterTipoExercicio(exerciciosSpeaking[1]), exerciciosSpeaking[1]); //

//         await PlayAudioETexto(i++); 

//         //janta

//         exAtual = exerciciosCriarFrases[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

//         await AbrirExercicio(ObterTipoExercicio(exerciciosAssociacao[0]), exerciciosAssociacao[0]);
//     }

//     private async Task interacaoB1()
//     {
//         int i = 0;
        
//         await PlayAudioETexto(i++);

//         exAtual = exerciciosSpeaking[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

//         await PlayAudioETexto(i++);

//         //missão de ver a cozinha

//         await AbrirExercicio(ObterTipoExercicio(exerciciosWriting[0]), exerciciosWriting[0]); //

//         exAtual = exerciciosSpeaking[1];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

//         await PlayAudioETexto(i++);

//         await PlayAudioETexto(i++);

//         //jantar

//         await PlayAudioETexto(i++);

//         exAtual = exerciciosSpeaking[2];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

//         await AbrirExercicio(ObterTipoExercicio(exerciciosAssociacao[0]), exerciciosAssociacao[0]);
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

//     public void PrepararParaFamilia()
//     {
//         nivelAtual = DadosJogador.nivelUsuario;

//         exerciciosCompletarFrase = GarantirListaExercicios(exerciciosCompletarFrase);
//         exerciciosCriarFrases = GarantirListaExercicios(exerciciosCriarFrases);
//         exerciciosSpeaking = GarantirListaExercicios(exerciciosSpeaking);
//         exerciciosAlternativas = GarantirListaExercicios(exerciciosAlternativas);
//         exerciciosWriting = GarantirListaExercicios(exerciciosWriting);
//         exerciciosAssociacao = GarantirListaExercicios(exerciciosAssociacao);

//         exerciciosCompletarFrase.Clear();
//         exerciciosCriarFrases.Clear();
//         exerciciosSpeaking.Clear();
//         exerciciosAlternativas.Clear();
//         exerciciosWriting.Clear();
//         exerciciosAssociacao.Clear();

//         InicializarConteudosPorNivel();
//     }

//     public async Task ExecutarFala(int indice)
//     {
//         await PlayAudioETexto(indice);
//     }

//     public async Task ExecutarExercicio(ExercicioBase exercicio)
//     {
//         await AbrirExercicio(ObterTipoExercicio(exercicio), exercicio);
//     }

//     public void FinalizarInteracaoFamilia()
//     {
//         if (modalLegendaComum != null)
//             modalLegendaComum.SetActive(false);

//         if (textoPularDialogoComum != null)
//             textoPularDialogoComum.gameObject.SetActive(false);

//         LiberarControlePlayer();
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

public class motherNPC : InteracaoNPC
{
    string nivelAtual;
    string cumprimento;
    int horaAtual = DateTime.Now.Hour;
    List<string> dialogoAtual;
    private AudioSource audioSource;
    private BackendManager backendManager;
    public ModalExercicio modalExercicio;
    public GameObject modalLegenda;
    public TextMeshProUGUI textoPularDialogo;
    public ExercicioBase exAtual;

    public string idNpcParaVoz = "mom";

    [SerializeField] private FamiliaManager familiaManager;

    [System.NonSerialized] public List<ExercicioBase> exerciciosCompletarFrase;
    [System.NonSerialized] public List<ExercicioBase> exerciciosCriarFrases;
    [System.NonSerialized] public List<ExercicioBase> exerciciosSpeaking;
    [System.NonSerialized] public List<ExercicioBase> exerciciosAlternativas;
    [System.NonSerialized] public List<ExercicioBase> exerciciosWriting;

    [System.NonSerialized] public List<ExercicioBase> exerciciosAssociacao;

    protected override void Start()
    {
        base.Start();
        nivelAtual = DadosJogador.nivelUsuario;
        nomeExibicaoLegenda = "Mom";
        exerciciosCompletarFrase = GarantirListaExercicios(exerciciosCompletarFrase);
        exerciciosCriarFrases = GarantirListaExercicios(exerciciosCriarFrases);
        exerciciosSpeaking = GarantirListaExercicios(exerciciosSpeaking);
        exerciciosAlternativas = GarantirListaExercicios(exerciciosAlternativas);
        exerciciosWriting = GarantirListaExercicios(exerciciosWriting);
        exerciciosAssociacao = GarantirListaExercicios(exerciciosAssociacao);
    }

    private void InicializarConteudosPorNivel()
    {
        if (nivelAtual == "A1")
        {
            dialogoAtual = new List<string>()
            {
                "Hey. How are you?",
                "How was your day?",
                "Look! We have a new bathroom.",
                "How is your work?",
                "Dinner is ready!"
            };

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Choose the correct response to the question.",
                        opcoesFala = new List<string>()
                        {
                            "The bathroom is next to the kitchen\n",
                            "I would like to book a flight to London\n",
                            "I’m fine, thank you\n"
                        },
                        respostaCorreta = 2
                    }
            ); 

            exerciciosAlternativas.Add(
                new ExercicioAlternativas()
                {
                        enunciado = "Choose the correct answer to the question.",
                        alternativas = new List<string>() { 
                        "This is my dog\r\n", 
                        "I is good\r\n", 
                        "It was good\r\n", 
                        "It is a bathroom\r\n" },
                        alternativaCorreta = 2
                }
            );  

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Say something nice about the bathroom"
                    }
            ); 

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Tell about your day at work",
                        opcoesFala = new List<string>()
                        {
                            "I’m looking for a new job?\n",
                            "I had a busy day at the office\n",
                            "Yes, please. Could you help me?\n"
                        },
                        respostaCorreta = 1
                    }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Say goodbye to your family"
                    }
            );

            exerciciosAssociacao.Add(
                new ExercicioAssociacao()
                {
                    enunciado = "Match the Portuguese word with the English word.",

                    opcoesPort = new List<string>
                    {
                        "banheiro",
                        "sala de estar",
                        "mãe",
                        "pai",
                        "irmão",
                        "cachorro",
                        "bola",
                        "mesa",
                        "jantar"
                    },

                    opcoesIngles = new List<string>
                    {
                        "dog",
                        "father",
                        "dinner",
                        "living room",
                        "ball",
                        "bathroom",
                        "table",
                        "brother",
                        "mother"
                    },

                    associacaoCorreta = new List<ParAssociacao>
                    {
                        new ParAssociacao { palavraPortugues = "banheiro", palavraIngles = "bathroom" },
                        new ParAssociacao { palavraPortugues = "sala de estar", palavraIngles = "living room" },
                        new ParAssociacao { palavraPortugues = "mãe", palavraIngles = "mother" },
                        new ParAssociacao { palavraPortugues = "pai", palavraIngles = "father" },
                        new ParAssociacao { palavraPortugues = "irmão", palavraIngles = "brother" },
                        new ParAssociacao { palavraPortugues = "cachorro", palavraIngles = "dog" },
                        new ParAssociacao { palavraPortugues = "bola", palavraIngles = "ball" },
                        new ParAssociacao { palavraPortugues = "mesa", palavraIngles = "table" },
                        new ParAssociacao { palavraPortugues = "jantar", palavraIngles = "dinner" }
                    }
                }
            );
        }
        else if(nivelAtual=="A2")
        {
            dialogoAtual = new List<string>()
            {
                "Hi! It's so good to see you. How have you been?",
                "I’m great, thanks. How was your day?",
                "We changed the room while you were away. Do you like it?",
                "Dinner is ready. Come to the table, please."
            };

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Tell your family how you have been feeling and ask how they are doing",
                        opcoesFala = new List<string>()
                        {
                            "The dog needs a new ball to play with\n",
                            "I've been good. How about you?\n",
                            "I would like to reserve a table for two\n"
                        },
                        respostaCorreta = 1
                    }
            );

            exerciciosWriting.Add(
                new ExercicioWriting()
                {
                    enunciado = "Talk about something you did during the day"
                }
            );

            exerciciosAlternativas.Add(
                new ExercicioAlternativas()
                {
                    enunciado = "Which object can you find in the room?",
                    alternativas = new List<string>() { 
                        "Shower\r\n", 
                        "Bedside table\r\n", 
                        "Dining table\r\n", 
                        "Refrigerator\r\n" },
                    alternativaCorreta = 1
                }
            );

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "Describe what you like about the room using a complete sentence."
                }
            );

            exerciciosCriarFrases.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Arrange the words to form the correct sentence in english: Obrigado pelo jantar. Foi ótimo ver vocês.",
                    blocosPalavras = new List<string>
                    {
                        "you",
                        "great",
                        "Thanks",
                        "all",
                        "for",
                        "was",
                        "lunch",
                        "It",
                        "see",
                        "dinner",
                        "nice",
                        "to",
                        "you",
                        "meet",
                        "everyone"
                    },
                    respostaCorreta = "Thanks for dinner. It was great to see you all.\n"
                }
            );

            exerciciosAssociacao.Add(
                new ExercicioAssociacao()
                {
                    enunciado = "Match the Portuguese word or expression with the English equivalent.",

                    opcoesPort = new List<string>
                    {
                        "mudanças",
                        "ultimamente",
                        "ocupado",
                        "passar tempo",
                        "preferir",
                        "chuveiro",
                        "mesa de cabeceira",
                        "rotina",
                        "animal de estimação",
                        "durante o jantar"
                    },

                    opcoesIngles = new List<string>
                    {
                        "spend time",
                        "bedside table",
                        "lately",
                        "pet",
                        "shower",
                        "prefer",
                        "busy",
                        "during dinner",
                        "changes",
                        "routine"
                    },

                    associacaoCorreta = new List<ParAssociacao>
                    {
                        new ParAssociacao { palavraPortugues = "mudanças", palavraIngles = "changes" },
                        new ParAssociacao { palavraPortugues = "ultimamente", palavraIngles = "lately" },
                        new ParAssociacao { palavraPortugues = "ocupado", palavraIngles = "busy" },
                        new ParAssociacao { palavraPortugues = "passar tempo", palavraIngles = "spend time" },
                        new ParAssociacao { palavraPortugues = "preferir", palavraIngles = "prefer" },
                        new ParAssociacao { palavraPortugues = "chuveiro", palavraIngles = "shower" },
                        new ParAssociacao { palavraPortugues = "mesa de cabeceira", palavraIngles = "bedside table" },
                        new ParAssociacao { palavraPortugues = "rotina", palavraIngles = "routine" },
                        new ParAssociacao { palavraPortugues = "animal de estimação", palavraIngles = "pet" },
                        new ParAssociacao { palavraPortugues = "durante o jantar", palavraIngles = "during dinner" }
                    }
                }
            );
        }
        else if(nivelAtual=="B1")
        {
            dialogoAtual = new List<string>()
            {
                "Hey! It's been a while.",
                "We made a few changes to the kitchen while you were away. What do you think?",
                "It's been good. My team is working on an interesting project, so the days go by quickly.",
                "Dinner is ready, everyone. Come to the table, please!",
                "It was really nice having you here."
            };

            exerciciosWriting.Add(
                new ExercicioWriting()
                {
                    enunciado = "Write a short message complimenting your family on the new kitchen and describing what you like about it"
                }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Ask your mother about her recent experiences at work",
                        opcoesFala = new List<string>()
                        {
                            "How have things been going at work, Mom?\n",
                            "Have you had a chance to buy any new clothes lately?\n",
                            "What kind of work does Mom usually do?\n"
                        },
                        respostaCorreta = 0
                    }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Say goodbye to your family after dinner, expressing appreciation for the time you spent together.",
                        opcoesFala = new List<string>()
                        {
                            "It was boring to see you all. I’ll call you soon\n",
                            "It was great seeing everyone again. Thank you for having me\n",
                            "Thanks for dinner. I’ll see you at work tomorrow\n"
                        },
                        respostaCorreta = 1
                    }
            );

            exerciciosAssociacao.Add(
                new ExercicioAssociacao()
                {
                    enunciado = "Match the Portuguese word or expression with the English equivalent.",

                    opcoesPort = new List<string>
                    {
                        "saudade",
                        "ultimamente",
                        "equipe",
                        "projeto",
                        "conquistas",
                        "quintal",
                        "brincar de buscar",
                        "ficar durante a noite",
                        "voltar para casa",
                        "colega"
                    },

                    opcoesIngles = new List<string>
                    {
                        "backyard",
                        "spend the night",
                        "team",
                        "colleague",
                        "lately",
                        "archievements",
                        "miss",
                        "project",
                        "play fetch",
                        "go back home"
                    },

                    associacaoCorreta = new List<ParAssociacao>
                    {
                        new ParAssociacao { palavraPortugues = "saudade", palavraIngles = "miss" },
                        new ParAssociacao { palavraPortugues = "ultimamente", palavraIngles = "lately" },
                        new ParAssociacao { palavraPortugues = "equipe", palavraIngles = "team" },
                        new ParAssociacao { palavraPortugues = "projeto", palavraIngles = "project" },
                        new ParAssociacao { palavraPortugues = "conquistas", palavraIngles = "archievements" },
                        new ParAssociacao { palavraPortugues = "quintal", palavraIngles = "backyard" },
                        new ParAssociacao { palavraPortugues = "brincar de buscar", palavraIngles = "play fetch" },
                        new ParAssociacao { palavraPortugues = "ficar durante a noite", palavraIngles = "spend the night" },
                        new ParAssociacao { palavraPortugues = "voltar para casa", palavraIngles = "go back home" },
                        new ParAssociacao { palavraPortugues = "colega", palavraIngles = "colleague" }
                    }
                }
            );
        }
        else if (nivelAtual == "A1-A2")
{
    dialogoAtual = new List<string>()
    {
        "Hi! It's so nice to have you home. How are you doing?",
        "How was your day? Did anything interesting happen?",
        "Look! We have made some changes to the bathroom. What do you think?",
        "I've had a busy day at work, but everything is going well.",
        "Dinner is ready! Come and join us at the table."
    };

    exerciciosSpeaking.Add(new ExercicioSpeaking()
    {
        enunciado = "Greet your mother and tell her how you are feeling.",
        opcoesFala = new List<string>()
        {
            "I'm doing well, thank you. It's great to be home.\n",
            "I would like to book a room, please.\n",
            "Where can I find the train station?\n"
        },
        respostaCorreta = 0
    });

    exerciciosWriting.Add(new ExercicioWriting()
    {
        enunciado = "Write a short paragraph about something interesting that happened during your day."
    });

    exerciciosAlternativas.Add(new ExercicioAlternativas()
    {
        enunciado = "Your mother asks what you think about the renovated bathroom. Choose the most appropriate response.",
        alternativas = new List<string>()
        {
            "It looks really nice! I like the new design.\r\n",
            "I would like to order something to drink.\r\n",
            "I have to finish my homework yesterday.\r\n",
            "The dog is playing with a ball outside.\r\n"
        },
        alternativaCorreta = 0
    });

    exerciciosSpeaking.Add(new ExercicioSpeaking()
    {
        enunciado = "Describe one thing you like about the renovated bathroom."
    });

    exerciciosSpeaking.Add(new ExercicioSpeaking()
    {
        enunciado = "Ask your mother how her day at work was.",
        opcoesFala = new List<string>()
        {
            "How was your day at work, Mom?\n",
            "Where did you buy your new shoes yesterday?\n",
            "Would you like to visit the airport?\n"
        },
        respostaCorreta = 0
    });

    exerciciosSpeaking.Add(new ExercicioSpeaking()
    {
        enunciado = "Respond to your mother and say that you are ready for dinner."
    });

    exerciciosAssociacao.Add(new ExercicioAssociacao()
    {
        enunciado = "Match the Portuguese word or expression with its English equivalent.",
        opcoesPort = new List<string>()
        {
            "reforma",
            "banheiro",
            "dia ocupado",
            "trabalho",
            "jantar",
            "mesa",
            "estar em casa"
        },
        opcoesIngles = new List<string>()
        {
            "dinner",
            "busy day",
            "bathroom",
            "renovation",
            "at home",
            "work",
            "table"
        },
        associacaoCorreta = new List<ParAssociacao>()
        {
            new ParAssociacao { palavraPortugues = "reforma", palavraIngles = "renovation" },
            new ParAssociacao { palavraPortugues = "banheiro", palavraIngles = "bathroom" },
            new ParAssociacao { palavraPortugues = "dia ocupado", palavraIngles = "busy day" },
            new ParAssociacao { palavraPortugues = "trabalho", palavraIngles = "work" },
            new ParAssociacao { palavraPortugues = "jantar", palavraIngles = "dinner" },
            new ParAssociacao { palavraPortugues = "mesa", palavraIngles = "table" },
            new ParAssociacao { palavraPortugues = "estar em casa", palavraIngles = "at home" }
        }
    });
}
else if (nivelAtual == "A2-B1")
{
    dialogoAtual = new List<string>()
    {
        "It's wonderful to have you back! How have you been lately?",
        "We decided to renovate the bathroom while you were away. What do you think of the changes?",
        "Work has been quite demanding recently, but I'm enjoying the new responsibilities.",
        "Dinner is ready, everyone. I hope you're hungry!",
        "It was really nice having you home. I hope you can visit us again soon!"
    };

    exerciciosSpeaking.Add(new ExercicioSpeaking()
    {
        enunciado = "Tell your mother how things have been going lately and ask about her.",
        opcoesFala = new List<string>()
        {
            "I've been doing well, thanks. How about you?\n",
            "I would like to check out of the hotel.\n",
            "Could you show me the way to the station?\n"
        },
        respostaCorreta = 0
    });

    exerciciosWriting.Add(new ExercicioWriting()
    {
        enunciado = "Write a short message describing what you think about the bathroom renovation. Mention at least two details you liked."
    });

    exerciciosSpeaking.Add(new ExercicioSpeaking()
    {
        enunciado = "Ask your mother what motivated the family to renovate the bathroom."
    });

    exerciciosSpeaking.Add(new ExercicioSpeaking()
    {
        enunciado = "Ask your mother how her new responsibilities at work are going.",
        opcoesFala = new List<string>()
        {
            "How have your new responsibilities at work been going?\n",
            "How much does the bathroom renovation cost yesterday?\n",
            "Where do you usually buy your dinner?\n"
        },
        respostaCorreta = 0
    });

    exerciciosCriarFrases.Add(new ExercicioBlocos()
    {
        enunciado = "Arrange the words to form the correct sentence in English: Parece que vocês fizeram um ótimo trabalho na reforma.",
        blocosPalavras = new List<string>()
        {
            "a",
            "job",
            "you",
            "great",
            "renovation",
            "It",
            "did",
            "the",
            "looks",
            "like",
            "with"
        },
        respostaCorreta = "It looks like you did a great job with the renovation.\n"
    });

    exerciciosAssociacao.Add(new ExercicioAssociacao()
    {
        enunciado = "Match the Portuguese word or expression with its English equivalent.",
        opcoesPort = new List<string>()
        {
            "responsabilidades",
            "reforma",
            "exigente",
            "ultimamente",
            "mudanças",
            "aproveitar",
            "trabalho",
            "juntar-se"
        },
        opcoesIngles = new List<string>()
        {
            "demanding",
            "responsibilities",
            "renovation",
            "join",
            "lately",
            "changes",
            "work",
            "enjoy"
        },
        associacaoCorreta = new List<ParAssociacao>()
        {
            new ParAssociacao { palavraPortugues = "responsabilidades", palavraIngles = "responsibilities" },
            new ParAssociacao { palavraPortugues = "reforma", palavraIngles = "renovation" },
            new ParAssociacao { palavraPortugues = "exigente", palavraIngles = "demanding" },
            new ParAssociacao { palavraPortugues = "ultimamente", palavraIngles = "lately" },
            new ParAssociacao { palavraPortugues = "mudanças", palavraIngles = "changes" },
            new ParAssociacao { palavraPortugues = "aproveitar", palavraIngles = "enjoy" },
            new ParAssociacao { palavraPortugues = "trabalho", palavraIngles = "work" },
            new ParAssociacao { palavraPortugues = "juntar-se", palavraIngles = "join" }
        }
    });
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

    protected override async Task IniciarInteracao()
    {
        if (familiaManager == null)
        {
            Debug.LogError("FamiliaManager não foi atribuído à mãe!");
            LiberarControlePlayer();
            return;
        }

        await familiaManager.IniciarVisita();
    }

    private async Task interacaoA1()
    {
        int i = 0;

        await PlayAudioETexto(i++);

        exAtual = exerciciosSpeaking[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

        await PlayAudioETexto(i++);

        exAtual = exerciciosAlternativas[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

        await PlayAudioETexto(i++);

        //misão de olhar o bathroom

        exAtual = exerciciosWriting[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

        await PlayAudioETexto(i++);

        exAtual = exerciciosSpeaking[1];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

        await PlayAudioETexto(i++); 

        //jantar

        exAtual = exerciciosSpeaking[2];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await AbrirExercicio(ObterTipoExercicio(exerciciosAssociacao[0]), exerciciosAssociacao[0]);
    }

    private async Task interacaoA2()
    {
        int i = 0;

        await PlayAudioETexto(i++);
        
        exAtual = exerciciosSpeaking[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

        await PlayAudioETexto(i++);
        
        exAtual = exerciciosWriting[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

        await PlayAudioETexto(i++);

        //ir até o quarto

        await AbrirExercicio(ObterTipoExercicio(exerciciosAlternativas[0]), exerciciosAlternativas[0]);

        //volta para a sala

        await AbrirExercicio(ObterTipoExercicio(exerciciosSpeaking[1]), exerciciosSpeaking[1]); //

        await PlayAudioETexto(i++); 

        //janta

        exAtual = exerciciosCriarFrases[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

        await AbrirExercicio(ObterTipoExercicio(exerciciosAssociacao[0]), exerciciosAssociacao[0]);
    }

    private async Task interacaoB1()
    {
        int i = 0;
        
        await PlayAudioETexto(i++);

        exAtual = exerciciosSpeaking[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

        await PlayAudioETexto(i++);

        //missão de ver a cozinha

        await AbrirExercicio(ObterTipoExercicio(exerciciosWriting[0]), exerciciosWriting[0]); //

        exAtual = exerciciosSpeaking[1];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

        await PlayAudioETexto(i++);

        await PlayAudioETexto(i++);

        //jantar

        await PlayAudioETexto(i++);

        exAtual = exerciciosSpeaking[2];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

        await AbrirExercicio(ObterTipoExercicio(exerciciosAssociacao[0]), exerciciosAssociacao[0]);
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

    public void PrepararParaFamilia()
    {
        nivelAtual = DadosJogador.nivelUsuario;

        exerciciosCompletarFrase = GarantirListaExercicios(exerciciosCompletarFrase);
        exerciciosCriarFrases = GarantirListaExercicios(exerciciosCriarFrases);
        exerciciosSpeaking = GarantirListaExercicios(exerciciosSpeaking);
        exerciciosAlternativas = GarantirListaExercicios(exerciciosAlternativas);
        exerciciosWriting = GarantirListaExercicios(exerciciosWriting);
        exerciciosAssociacao = GarantirListaExercicios(exerciciosAssociacao);

        exerciciosCompletarFrase.Clear();
        exerciciosCriarFrases.Clear();
        exerciciosSpeaking.Clear();
        exerciciosAlternativas.Clear();
        exerciciosWriting.Clear();
        exerciciosAssociacao.Clear();

        InicializarConteudosPorNivel();
    }

    public async Task ExecutarFala(int indice)
    {
        await PlayAudioETexto(indice);
    }

    public async Task ExecutarExercicio(ExercicioBase exercicio)
    {
        await AbrirExercicio(ObterTipoExercicio(exercicio), exercicio);
    }

    public void FinalizarInteracaoFamilia()
    {
        if (modalLegendaComum != null)
            modalLegendaComum.SetActive(false);

        if (textoPularDialogoComum != null)
            textoPularDialogoComum.gameObject.SetActive(false);

        LiberarControlePlayer();
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
