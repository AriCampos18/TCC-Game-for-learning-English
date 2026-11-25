// using System.Collections.Generic;
// using System.Threading.Tasks;
// using UnityEngine;
// using UnityEngine.Networking;
// using System.IO;
// using TMPro;
// using System;

// public class RestaurantClient : InteracaoNPC
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
//     [SerializeField] private RestaurantWaiterNPC garcom; //ARRASTAR NO INSPECTOR O GARÇOM DPS

//     [System.NonSerialized] public List<ExercicioBase> exerciciosCompletarFrase;
//     [System.NonSerialized] public List<ExercicioBase> exerciciosCriarFrases;
//     [System.NonSerialized] public List<ExercicioBase> exerciciosSpeaking;
//     [System.NonSerialized] public List<ExercicioBase> exerciciosAlternativas;
//     [System.NonSerialized] public List<ExercicioBase> exerciciosWriting;
//     [System.NonSerialized] public List<ExercicioBase> exerciciosAssociacao;

//     public string idNpcParaVoz = "recepcionistHotel";

//     protected override void Start()
//     {
//         base.Start();
//         nivelAtual = DadosJogador.nivelUsuario;
//         nomeExibicaoLegenda = "Restaurant Client";
//         exerciciosCompletarFrase = GarantirListaExercicios(exerciciosCompletarFrase);
//         exerciciosCriarFrases = GarantirListaExercicios(exerciciosCriarFrases);
//         exerciciosSpeaking = GarantirListaExercicios(exerciciosSpeaking);
//         exerciciosAlternativas = GarantirListaExercicios(exerciciosAlternativas);
//         exerciciosWriting = GarantirListaExercicios(exerciciosWriting);
//         exerciciosAssociacao = GarantirListaExercicios(exerciciosAssociacao);
//     }

//     private void InicializarConteudosPorNivel()
//     {
//         exerciciosCompletarFrase.Clear();
//         exerciciosCriarFrases.Clear();
//         exerciciosSpeaking.Clear();
//         exerciciosAlternativas.Clear();
//         exerciciosWriting.Clear();
//         exerciciosAssociacao.Clear();

//             dialogoAtual = new List<string>()
//             {
//                 "Hi. Is this your first time here?",
//                 "Have you tried the food here before?",
//                 "I come here almost every week. I really like the burgers, and the portions are generous, too.",
//                 "Yes, it’s delicious. The fries are great, too."
//             };

//         exerciciosSpeaking.Add(
//             new ExercicioSpeaking()
//             {
//                 enunciado = "Respond to the customer’s question by sharing your experience and how you feel about the place",
//                 opcoesFala = new List<string>()
//                 {
//                     "Yes, I ordered a hamburger and it’s already arriving \n",
//                     "No, I’d like to pay by card\n",
//                     "Yes, it is. I’m enjoying the atmosphere\n"
//                 },
//                 respostaCorreta = 2
//             }
//         ); 

//         exerciciosSpeaking.Add(
//             new ExercicioSpeaking()
//             {
//                 enunciado = "Respond to the customer’s question by explaining whether you have tried the restaurant’s food before",
//                 opcoesFala = new List<string>()
//                 {
//                     "No, I haven't, but I'm looking forward to it\n",
//                     "Yes, I’d like to book a table for two, please\n",
//                     "Could I have the bill, please? I'd like to pay before leaving\n"
//                 },
//                 respostaCorreta = 0
//             }
//         ); 

//         exerciciosCriarFrases.Add(
//                 new ExercicioBlocos()
//                 {
//                     enunciado= "Arrange the words to form the correct sentence in english: Não, obrigado. Você diria que vale a pena experimentar o hambúrguer?",
//                     blocosPalavras = new List<string>
//                     {
//                         "trying",
//                         "hamburger",
//                         "you",
//                         "worth",
//                         "Would",
//                         "delicious",
//                         "the",
//                         "say",
//                         "is",
//                         "it",
//                         "a",
//                         "be",
//                         "the",
//                         "enjoy",
//                         "try"
//                     },
//                     respostaCorreta = "Would you say the hamburger is worth trying?\n"
//                 }
//             );
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
//         GameProgress.EstaEmDialogo = true;
//         nivelAtual = DadosJogador.nivelUsuario;
//         InicializarConteudosPorNivel();

//         await interacaoUnica();

//         // A missão é criada pelo garçom antes de aguardar esta conversa.
//         MissionManager.ConcluirMissaoNPC("rest_b1_cliente");

//         // Desativa o estado quando a conversa acabar completamente!
//         garcom.FinalizarConversaComCliente();
//         GameProgress.EstaEmDialogo = false;

//         //MissionManager.Instance.ConcluirMissao("finalizar_compra");

//         if (modalLegenda != null)
//             modalLegenda.SetActive(false);

//         if (textoPularDialogo != null)
//             textoPularDialogo.gameObject.SetActive(false);

//         GameProgress.EstaEmDialogo = false;

//         LiberarControlePlayer();
//     }

//     private async Task interacaoUnica()
//     {
//         int i = 0;

//         await PlayAudioETexto(i++);

//         exAtual = exerciciosSpeaking[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);//

//         await PlayAudioETexto(i++);

//         exAtual = exerciciosSpeaking[1];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

//         await PlayAudioETexto(i++);

//         exAtual = exerciciosCriarFrases[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

//         await PlayAudioETexto(i++);
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

public class RestaurantClient : InteracaoNPC, IRoteiroNpcCheckpoint
{
    string nivelAtual;
    string cumprimento;
    int horaAtual = DateTime.Now.Hour;
    List<string> dialogoAtual;
    private const string NpcKey = MissoesRestaurant.Cliente;
    private int ultimaVisita;
    private int VisitaAtual => MissionManager.Instance?.VisitaRestaurant ?? 0;

    protected override bool PermitirReinteracaoAposConclusao =>
        VisitaAtual != 0 && VisitaAtual != ultimaVisita;
    private ExerciseLoadResult catalogoExercicios;
    public ModalExercicio modalExercicio;
    public GameObject modalLegenda;
    public TextMeshProUGUI textoPularDialogo;
    public ExercicioBase exAtual;
    [SerializeField] private RestaurantWaiterNPC garcom; //ARRASTAR NO INSPECTOR O GARÇOM DPS

    [System.NonSerialized] public List<ExercicioBase> exerciciosCompletarFrase;
    [System.NonSerialized] public List<ExercicioBase> exerciciosCriarFrases;
    [System.NonSerialized] public List<ExercicioBase> exerciciosSpeaking;
    [System.NonSerialized] public List<ExercicioBase> exerciciosAlternativas;
    [System.NonSerialized] public List<ExercicioBase> exerciciosWriting;
    [System.NonSerialized] public List<ExercicioBase> exerciciosAssociacao;

    public string idNpcParaVoz = "recepcionistHotel";

    protected override void Start()
    {
        base.Start();
        if (audioSourceComum == null)
        {
            audioSourceComum = gameObject.AddComponent<AudioSource>();
            audioSourceComum.playOnAwake = false;
        }

        nivelAtual = DadosJogador.nivelUsuario;
        nomeExibicaoLegenda = "Restaurant Client";
        exerciciosCompletarFrase = GarantirListaExercicios(exerciciosCompletarFrase);
        exerciciosCriarFrases = GarantirListaExercicios(exerciciosCriarFrases);
        exerciciosSpeaking = GarantirListaExercicios(exerciciosSpeaking);
        exerciciosAlternativas = GarantirListaExercicios(exerciciosAlternativas);
        exerciciosWriting = GarantirListaExercicios(exerciciosWriting);
        exerciciosAssociacao = GarantirListaExercicios(exerciciosAssociacao);
    }

    private void InicializarConteudosPorNivel(ConteudoRoteiro descricao = null, string nivelDescricao = null)
    {
        string nivelAtual = nivelDescricao ?? this.nivelAtual;
        var dialogoAtual = this.dialogoAtual;
        var exerciciosCompletarFrase = descricao?.exerciciosCompletarFrase ?? this.exerciciosCompletarFrase;
        var exerciciosCriarFrases = descricao?.exerciciosCriarFrases ?? this.exerciciosCriarFrases;
        var exerciciosSpeaking = descricao?.exerciciosSpeaking ?? this.exerciciosSpeaking;
        var exerciciosAlternativas = descricao?.exerciciosAlternativas ?? this.exerciciosAlternativas;
        var exerciciosWriting = descricao?.exerciciosWriting ?? this.exerciciosWriting;
        var exerciciosAssociacao = descricao?.exerciciosAssociacao ?? this.exerciciosAssociacao;

        exerciciosCompletarFrase.Clear();
        exerciciosCriarFrases.Clear();
        exerciciosSpeaking.Clear();
        exerciciosAlternativas.Clear();
        exerciciosWriting.Clear();
        exerciciosAssociacao.Clear();

        if(nivelAtual== "A2-B1")
        {
            dialogoAtual = new List<string>()
            {
                "Hi! Is this your first time at this restaurant?",
                "Have you tried the hamburger here before?",
                "I come here quite often. The hamburgers are really good, and the portions are quite large.",
                "I agree! The fries are delicious, too."
            };

            exerciciosSpeaking.Add(new ExercicioSpeaking()
            {
                enunciado = "Respond to the customer and explain whether this is your first visit.",
                opcoesFala = new List<string>()
                {
                    "Yes, this is my first time here. I'm enjoying it so far.\n",
                    "Yes, I'd like to pay the bill now.\n",
                    "No, I need to book a hotel room.\n"
                },
                respostaCorreta = 0
            });

            exerciciosSpeaking.Add(new ExercicioSpeaking()
            {
                enunciado = "The customer asks if you have tried the hamburger before. Answer appropriately.",
                opcoesFala = new List<string>()
                {
                    "No, I haven't, but I'm looking forward to trying it.\n",
                    "Yes, could you bring me the bill?\n",
                    "No, I'd like to reserve a table for tomorrow.\n"
                },
                respostaCorreta = 0
            });

            exerciciosCriarFrases.Add(new ExercicioBlocos()
            {
                enunciado = "Arrange the words to form the correct sentence in English: Você recomendaria experimentar o hambúrguer daqui?",
                blocosPalavras = new List<string>()
                {
                    "recommend",
                    "Would",
                    "here",
                    "trying",
                    "the",
                    "you",
                    "hamburger",
                    "recommendation",
                    "try",
                    "a",
                    "to"
                },
                respostaCorreta = "Would you recommend trying the hamburger here?\n"
            });
        }
        else if(nivelAtual== "B1")
        {
            dialogoAtual = new List<string>()
            {
                "Hi. Is this your first time here?",
                "Have you tried the food here before?",
                "I come here almost every week. I really like the burgers, and the portions are generous, too.",
                "Yes, it’s delicious. The fries are great, too."
            };

        exerciciosSpeaking.Add(
            new ExercicioSpeaking()
            {
                enunciado = "Respond to the customer’s question by sharing your experience and how you feel about the place",
                opcoesFala = new List<string>()
                {
                    "Yes, I ordered a hamburger and it’s already arriving \n",
                    "No, I’d like to pay by card\n",
                    "Yes, it is. I’m enjoying the atmosphere\n"
                },
                respostaCorreta = 2
            }
        ); 

        exerciciosSpeaking.Add(
            new ExercicioSpeaking()
            {
                enunciado = "Respond to the customer’s question by explaining whether you have tried the restaurant’s food before",
                opcoesFala = new List<string>()
                {
                    "No, I haven't, but I'm looking forward to it\n",
                    "Yes, I’d like to book a table for two, please\n",
                    "Could I have the bill, please? I'd like to pay before leaving\n"
                },
                respostaCorreta = 0
            }
        ); 

        exerciciosCriarFrases.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Arrange the words to form the correct sentence in english: Não, obrigado. Você diria que vale a pena experimentar o hambúrguer?",
                    blocosPalavras = new List<string>
                    {
                        "trying",
                        "hamburger",
                        "you",
                        "worth",
                        "Would",
                        "delicious",
                        "the",
                        "say",
                        "is",
                        "it",
                        "a",
                        "be",
                        "the",
                        "enjoy",
                        "try"
                    },
                    respostaCorreta = "Would you say the hamburger is worth trying?\n"
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

    public override bool PodeInteragir()
    {
        return base.PodeInteragir() && garcom != null
            && garcom.PodeReceberConversaDoCliente(DadosJogador.nivelUsuario);
    }

    protected override Task IniciarInteracao() => ExecutarConversa(0, null, null);

    private async Task ExecutarConversa(int inicio, string[] estadoMesa, Func<Task> concluir)
    {
        int versao = SessaoCenario.Atual.Versao;
        nivelAtual = DadosJogador.nivelUsuario;
        if (garcom == null || (concluir == null && !garcom.PodeReceberConversaDoCliente(nivelAtual)))
        {
            LiberarControlePlayer();
            return;
        }

        ultimaVisita = VisitaAtual;
        GameProgress.EstaEmDialogo = true;
        try
        {
            InicializarConteudosPorNivel();
            catalogoExercicios = await ExerciseManager.Instance.LoadForNpcAsync(NpcKey, nivelAtual);
            if (!catalogoExercicios.Success)
                Debug.LogWarning("RestaurantClient: usando exercícios locais. " + catalogoExercicios.Error);

            await CriarRoteiro(SessaoCenario.Atual.Versao, estadoMesa).Executar(inicio);

        }
        finally
        {
            if (modalLegenda != null)
                modalLegenda.SetActive(false);

            if (textoPularDialogo != null)
                textoPularDialogo.gameObject.SetActive(false);

            GameProgress.EstaEmDialogo = false;
            LiberarControlePlayer();
        }

        ValidarVisita(versao);
        if (concluir == null)
            garcom.FinalizarConversaComCliente();
        else
            await concluir();
    }


    async Task<Func<Task>> IRoteiroNpcCheckpoint.PrepararRetomada(Newtonsoft.Json.Linq.JObject visita,
        int versao, System.Threading.CancellationToken token)
    {
        ValidarVisita(versao);
        AtualizarVisita();
        nivelAtual = SessaoCenario.Atual.Nivel;
        if (garcom == null || (nivelAtual != "A2-B1" && nivelAtual != "B1"))
            throw new System.IO.InvalidDataException("Garçom ou nível incompatível com a conversa do cliente.");
        InicializarConteudosPorNivel();
        var cursor = CriarRoteiro(versao, null).Validar(visita);
        await RoteiroNpcCheckpoint.ExigirCatalogo(this, versao, token);
        var retorno = await garcom.PrepararRetomadaDoCliente(visita, cursor.Situacao, versao, token);
        return async () =>
        {
            ValidarVisita(versao);
            retorno.Posicionar();
            TravarControlePlayer();
            await ExecutarConversa(cursor.Indice, cursor.Situacao, retorno.Continuar);
        };
    }

    internal RoteiroNpcCheckpoint DescreverRoteiro(string nivel)
    {
        var conteudo = new ConteudoRoteiro();
        InicializarConteudosPorNivel(conteudo, nivel);
        return CriarRoteiro(0, null, conteudo, nivel);
    }

    private RoteiroNpcCheckpoint CriarRoteiro(int visita, string[] estadoMesa, ConteudoRoteiro descricao = null, string nivelDescricao = null)
    {
        string nivelAtual = nivelDescricao ?? this.nivelAtual;
        var dialogoAtual = descricao?.Falas ?? this.dialogoAtual;
        var exerciciosCriarFrases = descricao?.exerciciosCriarFrases ?? this.exerciciosCriarFrases;
        var exerciciosSpeaking = descricao?.exerciciosSpeaking ?? this.exerciciosSpeaking;

        var roteiro = new RoteiroNpcCheckpoint(this, visita, () => estadoMesa ?? garcom?.EstadoMesaDuranteCliente());
        void Fala(int indice) => roteiro.Fala(dialogoAtual[indice], () => PlayAudioETexto(indice));
        void Exercicio(string ponto, int ordem, ExercicioBase fallback) =>
            roteiro.Exercicio(ponto, ordem, () => AbrirExercicioDoPonto(ponto, ordem, fallback), modelo: fallback);

        switch (nivelAtual)
        {
            case "B1":
                Fala(0);
                Exercicio("first_visit", 1, exerciciosSpeaking[0]);
                Fala(1);
                Exercicio("menu_experience", 1, exerciciosSpeaking[1]);
                Fala(2);
                Exercicio("meal_comment", 1, exerciciosCriarFrases[0]);
                Fala(3);
                break;
            case "A2-B1":
                Fala(0);
                Exercicio("first_visit", 1, exerciciosSpeaking[0]);
                Fala(1);
                Exercicio("menu_experience", 1, exerciciosSpeaking[1]);
                Fala(2);
                Exercicio("meal_comment", 1, exerciciosCriarFrases[0]);
                Fala(3);
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

        Debug.LogWarning($"RestaurantClient: usando exercício local em {nivelAtual}/{pointKey}/{ordem}.");
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
        // 1. PASSANDO O ID DO NPC JUNTO COM O TEXTO PARA O BACKEND
        byte[] audioBytes = await backendManagerComum.GerarAudio(dialogoAtual[i], idNpcParaVoz);
        
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
            audioSourceComum.clip = clip;

            if (textoPularDialogo != null)
                textoPularDialogo.gameObject.SetActive(false);

            modalLegenda.GetComponent<ModalLegenda>().MostrarLegenda(nomeExibicaoLegenda, dialogoAtual[i]);
            audioSourceComum.Play();
            while (audioSourceComum.isPlaying)
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
            if (modalLegenda != null)
            {
                modalLegenda.SetActive(true);

                if (textoPularDialogo != null)
                    textoPularDialogo.gameObject.SetActive(false);

                modalLegenda.GetComponent<ModalLegenda>()
                    .MostrarLegenda(nomeExibicaoLegenda, textoParaFalar);
            }
            
            audioSourceComum.Play();
            
            while (audioSourceComum.isPlaying)
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
