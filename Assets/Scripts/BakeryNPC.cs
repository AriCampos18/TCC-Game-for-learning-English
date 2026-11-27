// using System.Collections.Generic;
// using System.Threading.Tasks;
// using UnityEngine;
// using UnityEngine.Networking;
// using System.IO;
// using TMPro;
// using System;

// public enum TipoExercicio
// {
//     Speaking,
//     Alternativas,
//     Blocos,
//     Writing,
//     Associacao,
//     CompletarFrase
// }

// public class BakeryNPC : InteracaoNPC
// {
//     List<string> dialogoAtual;

//     public GameObject breadLoaf;
//     public GameObject cake;
//     public GameObject sandwich;
//     public GameObject sourdough;

//     string nivelAtual;
//     private AudioSource audioSource;
//     public ModalExercicio modalExercicio;
//     public string idNpcParaVoz = "bakery"; // Altere no Inspector para "homem_caixa" ou "mulher_padaria"

//     public List<Transform> pontosBakeryA1;
//     public List<Transform> pontosBakeryA2;
//     public List<Transform> pontosBakeryB1;
//     private MissionManager missaoManager;

//     public TextMeshProUGUI textoPularDialogo;
//     [System.NonSerialized] public List<ExercicioBase> exerciciosCompletarFrase;
//     [System.NonSerialized] public List<ExercicioBase> exerciciosCriarFrases;
//     [System.NonSerialized] public List<ExercicioBase> exerciciosSpeaking;
//     [System.NonSerialized] public List<ExercicioBase> exerciciosAlternativas;
//     [System.NonSerialized] public List<ExercicioBase> exerciciosWriting;
//     [System.NonSerialized] public List<ExercicioBase> exerciciosAssociacao;
//     private ExercicioBase exAtual;
//     private BackendManager backendManager;

//     public GameObject modalLegenda;

//     protected override void Start()
//     {
//         base.Start();

//         nomeExibicaoLegenda = "Bakery Attendant";

//         backendManager = new BackendManager();
//         missaoManager = MissionManager.Instance;

//         audioSource = GetComponent<AudioSource>();

//         exerciciosSpeaking = new List<ExercicioBase>();
//         exerciciosAlternativas = new List<ExercicioBase>();
//         exerciciosCriarFrases = new List<ExercicioBase>();
//         nivelAtual = DadosJogador.nivelUsuario;
//     }

//     private void InicializarConteudosPorNivel()
//     {
//         if (nivelAtual == "A1")
//         {
//             dialogoAtual = new List<string>()
//             {
//                 "Hello. What are you looking for?",
//                 "Do you want anything to drink?",
//                 "Do you want anything else?",
//                 "We have chicken sandwiches and chocolate cake.",
//                 "Here is your order",
//                 "You're welcome"
//             };

//             exerciciosSpeaking.Add(
//                 new ExercicioSpeaking()
//                 {
//                     enunciado = "Say the right sentence:",
//                     opcoesFala = new List<string>() {
//                         "There is a table.\n",
//                         "I'm looking for some bread.\n",
//                         "He goes to school.\r\n"
//                     },
//                     respostaCorreta = 1,
//                 } 
//             );

//             exerciciosSpeaking.Add(
//                 new ExercicioSpeaking()
//                 {
//                     enunciado = "Say the right sentence:",
//                     opcoesFala = new List<string>()
//                     {
//                         "I'm at the bakery.\n",
//                         "There are three chairs.\n",
//                         "Yes, I want some juice.\n",
//                     },
//                     respostaCorreta = 2,
//                 });

//             exerciciosSpeaking.Add(
//                 new ExercicioSpeaking()
//                 {
//                     enunciado = "Say the right sentence:",
//                     opcoesFala = new List<string>()
//                     {
//                         "She has a small dog.\n",
//                         "Yes, I want a cake and a sandwich.\n",
//                         "My favorite color is blue.\n"
//                     },
//                     respostaCorreta = 1,
//                 });
            
//             exerciciosCriarFrases.Add(
//                 new ExercicioBlocos()
//                 { 
//                     enunciado = "Arrange the words to form the correct sentence in english: Você pode me ajudar?",
//                     blocosPalavras = new List<string>() { 
//                         "No", "me", "thank", "you", "that", "Can", "was",
//                         "help", "all", "hello", "thank", "I", "maybe"
//                     },
//                     respostaCorreta = "Can you help me?",
//                 }
//             );

//             exerciciosSpeaking.Add(
//                 new ExercicioSpeaking()
//                 {
//                     enunciado = "Say a phrase to express gratitude."
//                 });

//             exerciciosAlternativas.Add(
//                 new ExercicioAlternativas()
//                 {
//                     enunciado = "What kinds of sandwich and cake do they have?",
//                     alternativas = new List<string>()
//                     {
//                         "Apple pie and orange juice\r\n",
//                         "Chicken sandwich and chocolate cake\r\n",
//                         "Cheese sandwich and strawberry cake\r\n",
//                         "Cheese pizza and ice cream\r\n"
//                     },
//                     alternativaCorreta = 1
//                 });
//         }
//         else if (nivelAtual == "A2")
//         {
//             dialogoAtual = new List<string>()
//             {
//                 "Hello! What are you looking for in the bakery section?",
//                 "Yes, we do. Which sandwich would you like, and would you like some juice too?",
//                 "Can I help you with anything else, or do you already have everything you need?",
//                 "Here is your order. You are welcome."
//             };

//             exerciciosSpeaking.Add(
//                 new ExercicioSpeaking()
//                 {
//                     enunciado = "Say the right sentence from the list:",
//                     opcoesFala = new List<string>() {
//                         "She can play the guitar.\r\n",
//                         "Are there any fresh croissants and sandwiches today?\r\n",
//                         "I usually wake up at 7 a.m.\r\n",
//                     },
//                     respostaCorreta = 1,
//                 }
//             );

//             exerciciosSpeaking.Add(
//                 new ExercicioSpeaking()
//                 {
//                     enunciado = "Say the right sentence from the list:",
//                     opcoesFala = new List<string>()
//                     {
//                         "He watches TV every night.",
//                         "I'd like the chicken sandwich and some orange juice.",
//                         "There is a woman near the window."
//                     },
//                     respostaCorreta = 1
//                 }
//             );

//              exerciciosSpeaking.Add(
//                 new ExercicioSpeaking()
//                 {
//                     enunciado = "Say a phrase to express gratitude."
//                 }
//             );

//             exerciciosCriarFrases.Add(
//                 new ExercicioBlocos() 
//                 { 
//                     enunciado = "Arrange the words to form the correct sentence in english: Não, isso é tudo que eu precisava, obrigado/a.",
//                     blocosPalavras = new List<string>() { 
//                         "No", "milk", "thank", "you", "that", "needed", "was",
//                         "same", "all", "hello", "thank", "I", "maybe", "ok"
//                     },
//                     respostaCorreta = "No, that was all I needed, thank you.",
//                 }
//             );

//             // exerciciosCompletarFrase.Add(
//             //     new ExercicioCompletarFrase() 
//             //     { 
//             //         enunciado = "Complete the sentence with the correct word:",
//             //         fraseAntes =  "I'm looking for something sweet for ",
//             //         fraseDepois = ".",
//             //         respostaCorreta = "breakfast",
//             //         blocosPalavras = new List<string>() { 
//             //             "sweet", "thank", "you", "that", "something", "was", "for",
//             //             "same", "for", "hello", "thank", "I'm", "maybe", "ok", "looking", "breakfast"
//             //         },
//             //         respostaCorreta = "I'm looking for something sweet for breakfast.",
//             //     }
//             // );
//         }
//         else
//         {
//             dialogoAtual = new List<string>()
//             {
//                 "Hello! What are you looking for today, and would you like me to recommend something popular as well?",
//                 "Yes, there are still a few chocolate croissants and some cheese sandwiches available.",
//                 "Here is your order. Enjoy your food.",
//                 "You are welcome. Have a nice day."
//             };

//             exerciciosSpeaking.Add(
//                 new ExercicioSpeaking()
//                 {
//                     enunciado = "How would you respond? Say the right answer.",
//                     opcoesFala = new List<string>() {
//                         "There is a supermarket across the street from my house. \r\n \r\n",
//                         "Are there any freshly baked pastries left on the shelf near the window, or have they all been sold already? \r\n",
//                         "He doesn’t enjoy crowded places either.\r\n"
//                     },
//                     respostaCorreta = 1
//                 }
//             );

//             exerciciosWriting.Add(
//                 new ExercicioWriting()
//                 {
//                     enunciado = "Express your gratitude in a sentence.",
//                 }
//             );

//             exerciciosCriarFrases.Add(
//                 new ExercicioBlocos() 
//                 { 
//                     enunciado = "Arrange the words to form the correct sentence in english: Ótimo, parece incrível. Eu vou querer o croissant e o sanduíche.",
//                     blocosPalavras = new List<string>() { 
//                         "No", "milk", "Great", "the", "you", "sandwich", "the",
//                         "needed", "is", "nice", "amazing", "same", "I'll", "hello",
//                         "have", "bread", "I", "that", "croissant", "ok", "looks", "and"
//                     },
//                     respostaCorreta = "Great, that looks amazing. I'll have the croissant and the sandwich.",
//                 }
//             );

//             exerciciosAlternativas.Add(
//                 new ExercicioAlternativas()
//                 {
//                     enunciado = "Which details are correct about today’s bakery products?",
//                     alternativas = new List<string>()
//                     {
//                         "The sandwiches are made with chicken.\r\n",
//                         "The bakery only has apple pies today.\r\n",
//                         "The bread was sold out this morning.\r\n",
//                         "The chocolate croissants are still available.\r\n"
//                     },
//                     alternativaCorreta = 3
//                 });
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
//         // Ativa o estado de diálogo logo no começo
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
//         GameProgress.Instance.falouPadaria = true;

//         // Desativa o estado quando a conversa acabar completamente!
//         GameProgress.EstaEmDialogo = false;
//         MissionManager.Instance.ConcluirMissao("falar_padaria");
//         bool podeLiberarCaixa =
//             MissionManager.Instance.MissaoConcluida("falar_atendente")
//             && ProdutoManager.Instance != null
//             && ProdutoManager.Instance.ProdutosObrigatoriosPegos();

//         if (podeLiberarCaixa)
//         {
//             MissionManager.Instance.AdicionarMissao(
//                 "falar_caixa",
//                 "Put the products on the counter to start the conversation with the cashier",
//                 "Coloque os produtos no caixa para iniciar a conversa com o caixa"
//             );
//         }

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

//         // Toca áudio e mostra a legenda ao mesmo tempo de forma síncrona
//         await PlayAudioETexto(i++);

//         exAtual = exerciciosSpeaking[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

//         await PlayAudioETexto(i++);

//         exAtual = exerciciosSpeaking[1];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

//         await PegarProduto(breadLoaf);

//         await PlayAudioETexto(i++);

//         exAtual = exerciciosSpeaking[2];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

//         await AbrirExercicio(ObterTipoExercicio(exerciciosCriarFrases[0]), exerciciosCriarFrases[0]); //

//         await PlayAudioETexto(i++);

//         await PegarProduto(cake, falar: true);
//         await PegarProduto(sandwich, falar: false);

//         exAtual = exerciciosAlternativas[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

//         await PlayAudioETexto(i++);

//         exAtual = exerciciosSpeaking[3];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         await PlayAudioETexto(i++);
//     }

//     private async Task interacaoA2()
//     {
//         int i = 0;
        
//         //Hello! What are you looking for in the bakery section?
//         await PlayAudioETexto(i++);
        
//         exAtual = exerciciosSpeaking[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

//         //Yes, we do. Which sandwich would you like, and would you like some juice too?
//         await PlayAudioETexto(i++);

//         exAtual = exerciciosSpeaking[1];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

//         await PlayAudioETexto(i++);

//         await AbrirExercicio(ObterTipoExercicio(exerciciosCriarFrases[0]), exerciciosCriarFrases[0]); //

//         await PegarProduto(sandwich, falar: true);
//         await PegarProduto(sourdough, falar: false);

//         await PlayAudioETexto(i++);
        
//         exAtual = exerciciosSpeaking[2];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);
//     }

//     private async Task interacaoB1()
//     {
//         int i = 0;

//         await PlayAudioETexto(i++);

//         exAtual = exerciciosSpeaking[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

//         await PlayAudioETexto(i++);

//         exAtual = exerciciosCriarFrases[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

//         await PegarProduto(sandwich, falar: true);
//         await PegarProduto(sourdough, falar: false);

//         exAtual = exerciciosAlternativas[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

//         await PlayAudioETexto(i++);

//         exAtual = exerciciosSpeaking[1];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         await PlayAudioETexto(i++);
//     }

//    private async Task PegarProduto(GameObject produto, bool falar = true)
//     {
//         if (produto != null)
//         {
//             if (falar)
//             {
//                 await FalarFraseCustomizada("Let me get that for you.");
//             }

//             await Task.Delay(1500);
//             produto.SetActive(false);
//         }
//     }

//     public async Task AbrirExercicio(TipoExercicio tipo, ExercicioBase ex)
//     {
//         Debug.Log("Abrindo exercício: " + tipo);

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
//             Debug.Log($"Áudio recebido! Tamanho: {audioBytes.Length} bytes");
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
//                     Debug.Log("Clique detectado!");
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

public enum TipoExercicio
{
    Speaking,
    Alternativas,
    Blocos,
    Writing,
    Associacao,
    CompletarFrase
}

public class BakeryNPC : InteracaoNPC, IRoteiroNpcCheckpoint
{
    List<string> dialogoAtual;

    public GameObject breadLoaf;
    public GameObject cake;
    public GameObject sandwich;
    public GameObject sourdough;

    string nivelAtual;
    private AudioSource audioSource;
    public ModalExercicio modalExercicio;
    public string idNpcParaVoz = "bakery"; // Altere no Inspector para "homem_caixa" ou "mulher_padaria"

    public List<Transform> pontosBakeryA1;
    public List<Transform> pontosBakeryA2;
    public List<Transform> pontosBakeryB1;
    private MissionManager missaoManager;

    public TextMeshProUGUI textoPularDialogo;
    [System.NonSerialized] public List<ExercicioBase> exerciciosCompletarFrase;
    [System.NonSerialized] public List<ExercicioBase> exerciciosCriarFrases;
    [System.NonSerialized] public List<ExercicioBase> exerciciosSpeaking;
    [System.NonSerialized] public List<ExercicioBase> exerciciosAlternativas;
    [System.NonSerialized] public List<ExercicioBase> exerciciosWriting;
    [System.NonSerialized] public List<ExercicioBase> exerciciosAssociacao;
    private ExercicioBase exAtual;
    private BackendManager backendManager;
    private ExerciseLoadResult exerciciosBancoBakery;
    private string estadoCarregamentoExerciciosBakery;

    public GameObject modalLegenda;

    protected override void Start()
    {
        base.Start();

        nomeExibicaoLegenda = "Bakery Attendant";

        backendManager = new BackendManager();
        missaoManager = MissionManager.Instance;

        audioSource = GetComponent<AudioSource>();

        exerciciosSpeaking = new List<ExercicioBase>();
        exerciciosAlternativas = new List<ExercicioBase>();
        exerciciosCriarFrases = new List<ExercicioBase>();
        exerciciosWriting = new List<ExercicioBase>();
        nivelAtual = DadosJogador.nivelUsuario;
    }

    private void InicializarConteudosPorNivel(ConteudoRoteiro descricao = null, string nivelDescricao = null)
    {
        string nivelAtual = nivelDescricao ?? this.nivelAtual;
        var dialogoAtual = this.dialogoAtual;
        var exerciciosCriarFrases = descricao?.exerciciosCriarFrases ?? this.exerciciosCriarFrases;
        var exerciciosSpeaking = descricao?.exerciciosSpeaking ?? this.exerciciosSpeaking;
        var exerciciosAlternativas = descricao?.exerciciosAlternativas ?? this.exerciciosAlternativas;
        var exerciciosWriting = descricao?.exerciciosWriting ?? this.exerciciosWriting;

        if (nivelAtual == "A1")
        {
            dialogoAtual = new List<string>()
            {
                "Hello. What are you looking for?",
                "Do you want anything to drink?",
                "Do you want anything else?",
                "We have chicken sandwiches and chocolate cake.",
                "Here is your order",
                "You're welcome"
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
                        "I'm at the bakery.\n",
                        "There are three chairs.\n",
                        "Yes, I want some juice.\n",
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
            
            exerciciosCriarFrases.Add(
                new ExercicioBlocos()
                { 
                    enunciado = "Arrange the words to form the correct sentence in english: Você pode me ajudar?",
                    blocosPalavras = new List<string>() { 
                        "No", "me", "thank", "you", "that", "Can", "was",
                        "help", "all", "hello", "thank", "I", "maybe"
                    },
                    respostaCorreta = "Can you help me?",
                }
            );

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    modoLivre = true,
                    enunciado = "Say a phrase to express gratitude."
                });

            exerciciosAlternativas.Add(
                new ExercicioAlternativas()
                {
                    enunciado = "What kinds of sandwich and cake do they have?",
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
                "Yes, we do. Which sandwich would you like, and would you like some juice too?",
                "Can I help you with anything else, or do you already have everything you need?",
                "Here is your order."
            };

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "Say the right sentence from the list:",
                    opcoesFala = new List<string>() {
                        "She can play the guitar.\r\n",
                        "Are there any fresh croissants and sandwiches today?\r\n",
                        "I usually wake up at 7 a.m.\r\n",
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
                        "I'd like the chicken sandwich and some orange juice.",
                        "There is a woman near the window."
                    },
                    respostaCorreta = 1
                }
            );

             exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    modoLivre = true,
                    enunciado = "Say a phrase to express gratitude."
                }
            );

            exerciciosCriarFrases.Add(
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
        else if(nivelAtual=="B1")
        {
            dialogoAtual = new List<string>()
            {
                "Hello! What are you looking for today, and would you like me to recommend something popular as well?",
                "Yes, there are still a few chocolate croissants and some cheese sandwiches available.",
                "Here is your order. Enjoy your food.",
                "You are welcome. Have a nice day."
            };

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "How would you respond? Say the right answer.",
                    opcoesFala = new List<string>() {
                        "There is a supermarket across the street from my house. \r\n \r\n",
                        "Are there any freshly baked pastries left on the shelf near the window, or have they all been sold already? \r\n",
                        "He doesn’t enjoy crowded places either.\r\n"
                    },
                    respostaCorreta = 1
                }
            );

            exerciciosWriting.Add(
                new ExercicioWriting()
                {
                    enunciado = "Express your gratitude in a sentence.",
                }
            );

            exerciciosCriarFrases.Add(
                new ExercicioBlocos() 
                { 
                    enunciado = "Arrange the words to form the correct sentence in english: Ótimo, parece incrível. Eu vou querer o croissant e o sanduíche.",
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
        else if (nivelAtual == "A1-A2")
        {
            dialogoAtual = new List<string>()
    {
        "Hello! Welcome to the bakery. What would you like today?",
        "Of course! We have fresh bread, chicken sandwiches and chocolate cake.",
        "Would you like something to drink with your order?",
        "We have orange juice and water. Which one would you prefer?",
        "Great choice! Would you like anything else?",
        "Certainly! I'll get the cake and sandwich for you.",
        "Here is your order. Is there anything else you need?",
        "You're welcome! Enjoy your food!"
    };

    exerciciosSpeaking.Add(new ExercicioSpeaking()
    {
        enunciado = "Tell the bakery attendant that you would like to buy a chicken sandwich.",
        opcoesFala = new List<string>()
        {
            "I'd like to book a hotel room.\r\n",
            "I need to find the train station.\r\n",
            "I'd like a chicken sandwich, please.\r\n"
        },
        respostaCorreta = 2
    });

    exerciciosAlternativas.Add(new ExercicioAlternativas()
    {
        enunciado = "Which products does the attendant say are available?",
        alternativas = new List<string>()
        {
            "Coffee, tea and apple pie only.",
            "Fresh bread, chicken sandwiches and chocolate cake.",
            "Pizza, hamburgers and ice cream.",
            "Rice, pasta and frozen vegetables."
        },
        alternativaCorreta = 1
    });

    exerciciosSpeaking.Add(new ExercicioSpeaking()
    {
        enunciado = "The attendant asks whether you would like something to drink. Ask for orange juice.",
        opcoesFala = new List<string>()
        {
            "Could I have another room, please?\r\n",
            "Where can I buy a train ticket?\r\n",
            "Could I have some orange juice, please?\r\n"
        },
        respostaCorreta = 2
    });

    exerciciosCriarFrases.Add(new ExercicioBlocos()
    {
        enunciado = "Arrange the words to form the correct sentence in English: Eu gostaria de um sanduíche e um suco de laranja.",
        blocosPalavras = new List<string>()
        {
            "I'd", "like", "a", "sandwich", "and",
            "some", "orange", "juice", "please", "thank"
        },
        respostaCorreta = "I'd like a sandwich and some orange juice."
    });

    exerciciosSpeaking.Add(new ExercicioSpeaking()
    {
        enunciado = "The attendant asks if you would like anything else. Tell them that you would also like a piece of chocolate cake.",
        opcoesFala = new List<string>()
        {
            "I need a larger pair of shoes.\r\n",
            "I'd also like a piece of chocolate cake, please.\r\n",
            "I would like to check out of the hotel.\r\n"
        },
        respostaCorreta = 1
    });

            exerciciosAlternativas.Add(new ExercicioAlternativas()
            {
                enunciado = "The attendant asks if you need anything else. Choose the appropriate response.",
                alternativas = new List<string>()
                {
                    "I'd like to change my reservation.",
                    "No, that's everything. Thank you.",
                    "I usually wake up at seven.",
                    "The airport is near the city center."
                },
                alternativaCorreta = 1
            });
        }
        else if(nivelAtual== "A2-B1")
        {
            dialogoAtual = new List<string>()
    {
        "Hello! Are you looking for something to eat, or would you like me to recommend some of our products?",
        "We have freshly baked chocolate croissants, cheese sandwiches and sourdough bread today.",
        "The croissants are especially popular in the morning. Would you like to know more about the sandwiches as well?",
        "The chicken sandwich comes with lettuce and tomato. We also have a cheese sandwich if you prefer a vegetarian option.",
        "Would you like to have a drink with your order?",
        "We have fresh orange juice and bottled water. The juice is made here every morning.",
        "That sounds good! Would you like anything else?",
        "Certainly. I'll prepare your order now.",
        "Here you are. I hope you enjoy everything!"
    };

    exerciciosSpeaking.Add(new ExercicioSpeaking()
    {
        modoLivre = true,
        enunciado = "Explain what you are looking for and ask the attendant to recommend something freshly baked."
    });

    exerciciosAlternativas.Add(new ExercicioAlternativas()
    {
        enunciado = "Which statement correctly describes the products available today?",
        alternativas = new List<string>()
        {
            "The bakery has no sandwiches available.",
            "All the products are sold out.",
            "The bakery has chocolate croissants, cheese sandwiches and sourdough bread.",
            "The bakery only sells frozen meals."
        },
        alternativaCorreta = 2
    });

    exerciciosSpeaking.Add(new ExercicioSpeaking()
    {
        modoLivre = true,
        enunciado = "You are interested in the chicken sandwich. Ask what ingredients it contains."
    });

    exerciciosCriarFrases.Add(new ExercicioBlocos()
    {
        enunciado = "Arrange the words to form the correct sentence in English: Eu gostaria de saber se o sanduíche contém algum ingrediente picante.",
        blocosPalavras = new List<string>()
        {
            "I'd", "like", "to", "know", "if", "the",
            "sandwich", "contains", "any", "spicy", "ingredients",
            "because", "today", "please"
        },
        respostaCorreta = "I'd like to know if the sandwich contains any spicy ingredients."
    });

    exerciciosSpeaking.Add(new ExercicioSpeaking()
    {
        modoLivre = true,
        enunciado = "You prefer a vegetarian option. Explain this preference and ask whether the cheese sandwich would be suitable."
    });

    exerciciosAlternativas.Add(new ExercicioAlternativas()
    {
        enunciado = "The attendant says the orange juice is made fresh every morning. What does this mean?",
        alternativas = new List<string>()
        {
            "It is always served frozen.",
            "It is made only once a month.",
            "It contains no fruit.",
            "It is prepared fresh each morning."
        },
        alternativaCorreta = 3
    });

    exerciciosSpeaking.Add(new ExercicioSpeaking()
    {
        modoLivre = true,
        enunciado = "You have chosen a sandwich and a drink. Tell the attendant that you would also like a chocolate croissant."
    });

    exerciciosCriarFrases.Add(new ExercicioBlocos()
    {
        enunciado = "Arrange the words to form the correct sentence in English: Embora eu já tenha escolhido um sanduíche, gostaria de experimentar o croissant também.",
        blocosPalavras = new List<string>()
        {
            "Although", "I've", "already", "chosen", "a", "sandwich,",
            "I'd", "also", "like", "to", "try", "the", "croissant",
            "because", "it", "looks", "delicious"
        },
        respostaCorreta = "Although I've already chosen a sandwich, I'd also like to try the croissant."
    });

    exerciciosSpeaking.Add(new ExercicioSpeaking()
    {
        modoLivre = true,
        enunciado = "The attendant asks whether you need anything else. Confirm that your order is complete and thank them politely."
    });
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

    protected override Task IniciarInteracao() => ExecutarAtendimento(0, false);

    private async Task ExecutarAtendimento(int inicio, bool retomada)
    {
        int versao = SessaoCenario.Atual.Versao;
        // Ativa o estado de diálogo logo no começo
        GameProgress.EstaEmDialogo = true;
        nivelAtual = DadosJogador.nivelUsuario;
        bool intermediario = MissionManager.NivelMarketIntermediario(nivelAtual);
        if (!retomada && intermediario)
            MissionManager.Instance?.ConcluirMissaoMarket("falar_padaria", "npc_interagido", "bakery_npc");

        InicializarConteudosPorNivel();
        await CarregarExerciciosDoBanco();

        ValidarVisita(versao);
        await CriarRoteiro(versao).Executar(inicio);
        GameProgress.Instance.falouPadaria = true;

        // Desativa o estado quando a conversa acabar completamente!
        GameProgress.EstaEmDialogo = false;
        var missoes = MissionManager.Instance;
        if (!intermediario)
            missoes.ConcluirMissaoMarket("falar_padaria", "conversa_encerrada", "bakery_npc");
        else
            missoes?.RegistrarEventoMarket("conversa_encerrada", "bakery_npc");

        missoes?.TentarLiberarCaixaMarket();

        if (modalLegenda != null)
            modalLegenda.SetActive(false);

        if (textoPularDialogo != null)
            textoPularDialogo.gameObject.SetActive(false);

        GameProgress.EstaEmDialogo = false;

        LiberarControlePlayer();
    }

    private async Task CarregarExerciciosDoBanco()
    {
        exerciciosBancoBakery = null;
        estadoCarregamentoExerciciosBakery = null;
        ExerciseManager manager = ExerciseManager.Instance;
        ExerciseLoadResult result = await manager.LoadForNpcAsync("bakery_npc", nivelAtual);
        if (!result.Success)
        {
            estadoCarregamentoExerciciosBakery = "falha completa de carregamento da API: " + result.Error;
            Debug.LogWarning("BakeryNPC: exercícios do NeonDB indisponíveis; usando conteúdo local. " + result.Error);
            return;
        }
        if (result.Count == 0)
        {
            estadoCarregamentoExerciciosBakery = "API respondeu sem exercícios para o nível " + nivelAtual;
            Debug.LogWarning("BakeryNPC: a API não retornou exercícios para o nível " + nivelAtual + ". Usando conteúdo local.");
            return;
        }
        exerciciosBancoBakery = result;
        Debug.Log("BakeryNPC: " + result.Count + " exercícios carregados do backend para " + nivelAtual + ".");
    }

    private async Task AbrirExercicioDoPonto(string pointKey, int ordem, ExercicioBase fallback)
    {
        CheckpointJogo.IdentificarFallback(fallback, pointKey, ordem, nivelAtual);
        TipoExercicio expectedType = fallback == null
            ? (TipoExercicio)(-1)
            : ObterTipoExercicio(fallback);
        bool? expectedSpeakingFreeMode = null;
        ExercicioSpeaking expectedSpeaking = fallback as ExercicioSpeaking;
        if (expectedSpeaking != null)
            expectedSpeakingFreeMode = expectedSpeaking.modoLivre;

        ExerciseCatalogItem databaseItem = exerciciosBancoBakery == null
            ? null
            : exerciciosBancoBakery.Find(
                pointKey, nivelAtual, ordem, expectedType, expectedSpeakingFreeMode);
        if (databaseItem != null)
        {
            Debug.Log("BakeryNPC: exercício carregado da API; id_exercise="
                + databaseItem.Exercise.idExercise
                + ", point_key=" + pointKey
                + ", nível=" + nivelAtual
                + ", modalidade=" + DescreverModalidade(databaseItem.Tipo, databaseItem.Exercise)
                + ", ordem=" + ordem + ".");
            await AbrirExercicio(databaseItem.Tipo, databaseItem.Exercise);
            return;
        }

        if (fallback == null)
            throw new InvalidOperationException("Exercício ausente no banco e no fallback local: " + pointKey);

        string fallbackReason = exerciciosBancoBakery == null
            ? (estadoCarregamentoExerciciosBakery ?? "catálogo da API indisponível")
            : "não foi encontrado exercício correspondente no catálogo da API";
        string expectedModality = fallback == null
            ? "indisponível"
            : DescreverModalidade(expectedType, fallback);
        Debug.LogWarning("BakeryNPC: usando fallback local por exercício; motivo="
            + fallbackReason
            + ", point_key=" + (string.IsNullOrWhiteSpace(pointKey) ? "indisponível" : pointKey)
            + ", nível=" + (string.IsNullOrWhiteSpace(nivelAtual) ? "indisponível" : nivelAtual)
            + ", modalidade esperada=" + expectedModality
            + ", ordem esperada=" + ordem + ".");
        await AbrirExercicio(ObterTipoExercicio(fallback), fallback);
    }

    private string DescreverModalidade(TipoExercicio tipo, ExercicioBase exercicio)
    {
        if (tipo == TipoExercicio.Speaking)
        {
            ExercicioSpeaking speaking = exercicio as ExercicioSpeaking;
            return speaking != null && speaking.modoLivre ? "Speaking Livre" : "Speaking";
        }

        switch (tipo)
        {
            case TipoExercicio.Alternativas: return "Alternativas";
            case TipoExercicio.Blocos: return "Criar Frase (Word Blocks)";
            case TipoExercicio.Writing: return "Writing";
            case TipoExercicio.Associacao: return "Associação";
            case TipoExercicio.CompletarFrase: return "Completar Frase";
            default: return tipo.ToString();
        }
    }


    async Task<System.Func<Task>> IRoteiroNpcCheckpoint.PrepararRetomada(Newtonsoft.Json.Linq.JObject visita,
        int versao, System.Threading.CancellationToken token)
    {
        ValidarVisita(versao);
        AtualizarVisita();
        nivelAtual = SessaoCenario.Atual.Nivel;
        InicializarConteudosPorNivel();
        var cursor = CriarRoteiro(versao).Validar(visita);
        var produtos = new[] { breadLoaf, cake, sandwich, sourdough };
        var chaves = new[] { "bread", "cake", "sandwich", "sourdough" };
        if (cursor.Situacao.Length != produtos.Length)
            throw new System.IO.InvalidDataException("Estado da vitrine inválido.");
        for (int i = 0; i < produtos.Length; i++)
            if (cursor.Situacao[i] != chaves[i] + ".hidden" && cursor.Situacao[i] != chaves[i] + ".visible")
                throw new System.IO.InvalidDataException("Produto da vitrine inválido.");
        await RoteiroNpcCheckpoint.ExigirCatalogo(this, versao, token);
        return async () =>
        {
            ValidarVisita(versao);
            TravarControlePlayer();
            for (int i = 0; i < produtos.Length; i++)
                if (produtos[i] != null && cursor.Situacao[i] == chaves[i] + ".hidden")
                {
                    produtosExibidos.Add(produtos[i]);
                    produtos[i].SetActive(false);
                }
            await ExecutarAtendimento(cursor.Indice, true);
        };
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

        var roteiro = new RoteiroNpcCheckpoint(this, visita, () => new[] {
            produtosExibidos.Contains(breadLoaf) ? "bread.hidden" : "bread.visible",
            produtosExibidos.Contains(cake) ? "cake.hidden" : "cake.visible",
            produtosExibidos.Contains(sandwich) ? "sandwich.hidden" : "sandwich.visible",
            produtosExibidos.Contains(sourdough) ? "sourdough.hidden" : "sourdough.visible"
        });
        void Fala(int indice) => roteiro.Fala(dialogoAtual[indice], () => PlayAudioETexto(indice));
        void Exercicio(string ponto, int ordem, ExercicioBase fallback) =>
            roteiro.Exercicio(ponto, ordem, () => AbrirExercicioDoPonto(ponto, ordem, fallback), modelo: fallback);

        switch (nivelAtual)
        {
            case "A1":
                Fala(0);
                Exercicio("initial_order", 1, exerciciosSpeaking[0]);
                Fala(1);
                Exercicio("product_selection", 1, exerciciosSpeaking[1]);
                roteiro.Acao("PegarProduto.breadLoaf", () => PegarProduto(breadLoaf));
                Fala(2);
                Exercicio("anything_else_response", 1, exerciciosSpeaking[2]);
                Exercicio("anything_else_response", 2, exerciciosCriarFrases[0]);
                Fala(3);
                roteiro.Acao("PegarProduto.cake", () => PegarProduto(cake, falar: true));
                roteiro.Acao("PegarProduto.sandwich", () => PegarProduto(sandwich, falar: false));
                Exercicio("product_comprehension", 1, exerciciosAlternativas[0]);
                Fala(4);
                Exercicio("order_completion", 1, exerciciosSpeaking[3]);
                Fala(5);
                break;
            case "A2":
                Fala(0);
                Exercicio("initial_order", 1, exerciciosSpeaking[0]);
                Fala(1);
                Exercicio("product_selection", 1, exerciciosSpeaking[1]);
                Fala(2);
                Exercicio("anything_else_response", 1, exerciciosCriarFrases[0]);
                roteiro.Acao("PegarProduto.sandwich", () => PegarProduto(sandwich, falar: true));
                roteiro.Acao("PegarProduto.sourdough", () => PegarProduto(sourdough, falar: false));
                Fala(3);
                Exercicio("order_completion", 1, exerciciosSpeaking[2]);
                break;
            case "B1":
                Fala(0);
                Exercicio("initial_order", 1, exerciciosSpeaking[0]);
                Fala(1);
                Exercicio("product_selection", 1, exerciciosCriarFrases[0]);
                roteiro.Acao("PegarProduto.sandwich", () => PegarProduto(sandwich, falar: true));
                roteiro.Acao("PegarProduto.sourdough", () => PegarProduto(sourdough, falar: false));
                Exercicio("product_comprehension", 1, exerciciosAlternativas[0]);
                Fala(2);
                Exercicio("order_completion", 1, exerciciosWriting[0]);
                Fala(3);
                break;
            case "A1-A2":
                Fala(0);
                Exercicio("initial_order", 1, exerciciosSpeaking[0]);
                Fala(1);
                Exercicio("product_comprehension", 1, exerciciosAlternativas[0]);
                Fala(2);
                Exercicio("product_selection", 1, exerciciosSpeaking[1]);
                Exercicio("product_selection", 2, exerciciosCriarFrases[0]);
                Fala(3);
                Exercicio("additional_item_request", 1, exerciciosSpeaking[2]);
                roteiro.Acao("PegarProduto.cake", () => PegarProduto(cake, falar: true));
                roteiro.Acao("PegarProduto.sandwich", () => PegarProduto(sandwich, falar: false));
                Fala(4);
                Exercicio("anything_else_response", 1, exerciciosAlternativas[1]);
                Fala(5);
                break;
            case "A2-B1":
                Fala(0);
                Exercicio("initial_order", 1, exerciciosSpeaking[0]);
                Fala(1);
                Exercicio("product_comprehension", 1, exerciciosAlternativas[0]);
                Fala(2);
                Exercicio("sandwich_details", 1, exerciciosSpeaking[1]);
                Exercicio("sandwich_details", 2, exerciciosCriarFrases[0]);
                Fala(3);
                Exercicio("dietary_preference", 1, exerciciosSpeaking[2]);
                Fala(4);
                Exercicio("drink_details", 1, exerciciosAlternativas[1]);
                Fala(5);
                Exercicio("additional_item_request", 1, exerciciosSpeaking[3]);
                roteiro.Acao("PegarProduto.sandwich", () => PegarProduto(sandwich, falar: true));
                roteiro.Acao("PegarProduto.sourdough", () => PegarProduto(sourdough, falar: false));
                Fala(6);
                Exercicio("anything_else_response", 1, exerciciosCriarFrases[1]);
                Fala(7);
                Exercicio("order_completion", 1, exerciciosSpeaking[4]);
                break;
        }
        return roteiro;
    }

    private readonly HashSet<GameObject> produtosExibidos = new HashSet<GameObject>();

    protected override void ReiniciarVisita()
    {
        foreach (GameObject produto in produtosExibidos)
            if (produto != null)
                produto.SetActive(true);
        produtosExibidos.Clear();
    }

    private async Task PegarProduto(GameObject produto, bool falar = true)
    {
        int visita = SessaoCenario.Atual.Versao;
        if (produto != null)
        {
            if (falar)
            {
                await FalarFraseCustomizada("Let me get that for you.");
            }

            await Task.Delay(1500);
            ValidarVisita(visita);
            if (produto.activeSelf)
                produtosExibidos.Add(produto);
            produto.SetActive(false);
        }
    }

    public async Task AbrirExercicio(TipoExercicio tipo, ExercicioBase ex)
    {
        Debug.Log("Abrindo exercício: " + tipo);

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
            
            audioSource.Play();
            
            while (audioSource.isPlaying)
            {
                await Task.Yield();
            }
            ValidarVisita(visita);

            if (modalLegenda != null)
            {
                modalLegenda.SetActive(false);
            }
        }
    }
}
