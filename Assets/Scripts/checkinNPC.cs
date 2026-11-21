// using System.Collections.Generic;
// using System.Threading.Tasks;
// using UnityEngine;
// using UnityEngine.Networking;
// using System.IO;
// using TMPro;
// using System;

// public class checkinNPC : InteracaoNPC
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

//     [System.NonSerialized] public List<ExercicioBase> exerciciosCompletarFrase;
//     [System.NonSerialized] public List<ExercicioBase> exerciciosCriarFrases;
//     [System.NonSerialized] public List<ExercicioBase> exerciciosSpeaking;
//     [System.NonSerialized] public List<ExercicioBase> exerciciosAlternativas;
//     [System.NonSerialized] public List<ExercicioBase> exerciciosWriting;
//     [System.NonSerialized] public List<ExercicioBase> exerciciosAssociacao;

//     public string idNpcParaVoz = "airportInfo";
//     public Transform pontoChegadaPortao12;
//     public Transform pontoChegadaPortao18;
//     public Transform pontoChegadaPortao24;
//     public GameObject marcadorPortao12;
//     public GameObject marcadorPortao18;
//     public GameObject marcadorPortao24;

//     private bool aguardandoChegadaPortao;
//     private bool fluxoCheckinConcluido;
//     private bool finalizacaoIniciada;
//     private string idMissaoChegadaPortao;
//     private Transform pontoChegadaPortaoAtual;
//     private GameObject marcadorPortaoAtual;
//     private float tempoParadoPortao;
//     private Vector3 ultimaPosicaoPlayer;

//     private const float DistanciaChegadaPortao = 2f;
//     private const float TempoNecessarioPortao = 3f;
//     private const float VelocidadeMaximaParado = 0.15f;

//     protected override void Start()
//     {
//         base.Start();
//         DesativarMarcadoresPortoes();
//         nivelAtual = DadosJogador.nivelUsuario;
//         nomeExibicaoLegenda = "Airport Employee";
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

//     private void Update()
//     {
//         if (!aguardandoChegadaPortao || !fluxoCheckinConcluido)
//             return;

//         if (pontoChegadaPortaoAtual == null || movimentoPlayer == null)
//             return;

//         Vector3 posicaoAtual = movimentoPlayer.transform.position;
//         Vector3 deslocamento = posicaoAtual - pontoChegadaPortaoAtual.position;
//         deslocamento.y = 0f;

//         bool dentroDaArea =
//             deslocamento.sqrMagnitude
//             <= DistanciaChegadaPortao * DistanciaChegadaPortao;

//         if (dentroDaArea)
//         {
//             Vector3 movimento = posicaoAtual - ultimaPosicaoPlayer;
//             movimento.y = 0f;

//             float velocidade = movimento.magnitude
//                 / Mathf.Max(Time.deltaTime, 0.0001f);

//             if (velocidade <= VelocidadeMaximaParado)
//                 tempoParadoPortao += Time.deltaTime;
//             else
//                 tempoParadoPortao = 0f;

//             if (tempoParadoPortao >= TempoNecessarioPortao)
//                 ConcluirChegadaAoPortao();
//         }
//         else
//         {
//             tempoParadoPortao = 0f;
//         }

//         ultimaPosicaoPlayer = posicaoAtual;
//     }

//     private void DesativarMarcadoresPortoes()
//     {
//         if (marcadorPortao12 != null)
//             marcadorPortao12.SetActive(false);
//         if (marcadorPortao18 != null)
//             marcadorPortao18.SetActive(false);
//         if (marcadorPortao24 != null)
//             marcadorPortao24.SetActive(false);
//     }

//     private void IniciarMissaoChegadaPortao(
//         string id,
//         string texto,
//         string traducao,
//         Transform pontoChegada,
//         GameObject marcador)
//     {
//         IniciarMissao(id, texto, traducao);

//         idMissaoChegadaPortao = id;
//         pontoChegadaPortaoAtual = pontoChegada;
//         marcadorPortaoAtual = marcador;
//         aguardandoChegadaPortao = true;
//         tempoParadoPortao = 0f;

//         if (movimentoPlayer != null)
//             ultimaPosicaoPlayer = movimentoPlayer.transform.position;

//         if (marcadorPortaoAtual != null)
//             marcadorPortaoAtual.SetActive(true);
//         else
//             Debug.LogWarning("checkinNPC: atribua o marcador do portão no Inspector.");

//         if (pontoChegadaPortaoAtual == null)
//             Debug.LogWarning("checkinNPC: atribua o ponto de chegada do portão no Inspector.");
//     }

//     private void ConcluirChegadaAoPortao()
//     {
//         MissionManager.ConcluirMissaoNPC(idMissaoChegadaPortao);

//         if (MissionManager.Instance == null
//             || !MissionManager.Instance.MissaoConcluida(idMissaoChegadaPortao))
//         {
//             Debug.LogError(
//                 "checkinNPC: a missão do portão não foi concluída no MissionManager."
//             );
//             tempoParadoPortao = 0f;
//             return;
//         }

//         aguardandoChegadaPortao = false;
//         if (marcadorPortaoAtual != null)
//             marcadorPortaoAtual.SetActive(false);

//         if (!finalizacaoIniciada)
//         {
//             finalizacaoIniciada = true;
//             FinalizarCenarioAposChegadaPortao();
//         }
//     }

//     private async void FinalizarCenarioAposChegadaPortao()
//     {
//         bool finalizacaoConcluida = false;

//         if (RevisaoManager.Instance != null)
//             finalizacaoConcluida =
//                 await RevisaoManager.Instance.ExecutarRevisaoEExibirModalFinal(this);
//         else
//             Debug.LogError("RevisaoManager não está disponível para finalizar Airport.");

//         if (!finalizacaoConcluida)
//         {
//             finalizacaoIniciada = false;
//             aguardandoChegadaPortao = true;
//             tempoParadoPortao = 0f;

//             if (marcadorPortaoAtual != null)
//                 marcadorPortaoAtual.SetActive(true);
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
//                 "Hello. Where are you going?", 
//                 "Can I see your passport, please?",
//                 "Here is your boarding pass. Your gate is twelve."
//             };

//             exerciciosSpeaking.Add(
//                     new ExercicioSpeaking()
//                     {
//                         enunciado = "Show your passport to the airport staff.",
//                         opcoesFala = new List<string>()
//                         {
//                             "Here is my passport\n",
//                             "Here is my breakfast\n",
//                             "My passport is a boarding gate\n"
//                         },
//                         respostaCorreta = 0
//                     }
//             );

//             exerciciosCompletarFrase.Add(
//                 new ExercicioCompletarFrase()
//                 {
//                     enunciado = "Complete the sentence with the correct word:",
//                     fraseAntes = "Your boarding gate is ",
//                     fraseDepois = ".",
//                     respostaCorreta = "12",
//                     opcoes = new List<string>
//                     {
//                         "20",
//                         "12",
//                         "passport",
//                         "18",
//                         "boarding",
//                         "21",
//                         "airport",
//                         "2",
//                         "24",
//                         "ticket"
//                     }
//                 }
//             );

//             exerciciosAssociacao.Add(
//                 new ExercicioAssociacao()
//                 {
//                     enunciado = "Match the Portuguese word with the English word.",

//                     opcoesPort = new List<string>
//                     {
//                         "passaporte",
//                         "aeroporto",
//                         "portão de embarque",
//                         "cartão de embarque",
//                         "voo",
//                         "bagagem",
//                         "balcão de check-in",
//                         "escadas"
//                     },

//                     opcoesIngles = new List<string>
//                     {
//                         "flight",
//                         "stairs",
//                         "passport",
//                         "check-in desk",
//                         "luggage",
//                         "boarding pass",
//                         "airport",
//                         "boarding gate"
//                     },

//                     associacaoCorreta = new List<ParAssociacao>
//                     {
//                         new ParAssociacao { palavraPortugues = "passaporte", palavraIngles = "passport" },
//                         new ParAssociacao { palavraPortugues = "aeroporto", palavraIngles = "airport" },
//                         new ParAssociacao { palavraPortugues = "portão de embarque", palavraIngles = "boarding gate" },
//                         new ParAssociacao { palavraPortugues = "cartão de embarque", palavraIngles = "boarding pass" },
//                         new ParAssociacao { palavraPortugues = "voo", palavraIngles = "flight" },
//                         new ParAssociacao { palavraPortugues = "bagagem", palavraIngles = "luggage" },
//                         new ParAssociacao { palavraPortugues = "balcão de check-in", palavraIngles = "check-in desk" },
//                         new ParAssociacao { palavraPortugues = "escadas", palavraIngles = "stairs" }
//                     }
//                 }
//             );
//         }
//         else if(nivelAtual=="A2")
//         {
//             dialogoAtual = new List<string>()
//             {
//                 cumprimento+". Where are you flying today?",
//                 "May I see your passport and booking confirmation, please?",
//                 "Do you have any bags to check in?",
//                 "Your gate is eighteen, and boarding starts at 2:30 p.m. Have a nice flight!"
//             };

//             exerciciosCriarFrases.Add(
//                 new ExercicioBlocos()
//                 {
//                     enunciado= "Arrange the words to form the correct sentence in english: Eu estou indo para Londres",
//                     blocosPalavras = new List<string>
//                     {
//                         "London",
//                         "going",
//                         "to",
//                         "I",
//                         "Paris",
//                         "am",
//                         "traveling",
//                         "from",
//                         "is",
//                         "fly",
//                         "the"
//                     },
//                     respostaCorreta = "I am going to London\n"
//                 }
//             );

//             exerciciosSpeaking.Add(
//                     new ExercicioSpeaking()
//                     {
//                         enunciado = "The airport staff asks to see your passport and booking confirmation. Choose the most appropriate response.",
//                         opcoesFala = new List<string>()
//                         {
//                             "Here is my passport and booking confirmation\n",
//                             "I left my booking confirmation at the hotel\n",
//                             "I showed my passport to the hotel receptionist last week\n"
//                         },
//                         respostaCorreta = 0
//                     }
//             );

//             exerciciosSpeaking.Add(
//                     new ExercicioSpeaking()
//                     {
//                         enunciado = "The airport staff asks if you have any luggage to check in. Choose the most appropriate response.",
//                         opcoesFala = new List<string>()
//                         {
//                             "Yes, I need to check in at gate 18\n",
//                             "Yes, I have one suitcase to check in\n",
//                             "Yes, I have already arrived at my destination\n"
//                         },
//                         respostaCorreta = 1
//                     }
//             );

//             exerciciosAlternativas.Add(
//                     new ExercicioAlternativas()
//                     {
//                         enunciado = "What time does your boarding start?",
//                         alternativas = new List<string>() { 
//                         "Two p.m.\r\n", 
//                         "Two thirty p.m.\r\n", 
//                         "Three thirty p.m.\r\n", 
//                         "Eleven thirty p.m.\r\n" },
//                         alternativaCorreta = 1
//                     }
//             );     

//             exerciciosCompletarFrase.Add(
//                 new ExercicioCompletarFrase()
//                 {
//                     enunciado = "Complete the sentence with the correct word:",
//                     fraseAntes = "The boarding gate is ",
//                     fraseDepois = ".",
//                     respostaCorreta = "18",
//                     opcoes = new List<string>
//                     {
//                         "passport",
//                         "24",
//                         "18",
//                         "luggage",
//                         "12",
//                         "ticket",
//                         "8",
//                         "28",
//                         "airport",
//                         "21",
//                         "boarding",
//                         "80"
//                     }
//                 }
//             );

//             exerciciosAssociacao.Add(
//                 new ExercicioAssociacao()
//                 {
//                     enunciado = "Match the Portuguese word or expression with the English equivalent.",

//                     opcoesPort = new List<string>
//                     {
//                         "mala de viagem",
//                         "confirmação da reserva",
//                         "despachar bagagem",
//                         "embarque",
//                         "destino",
//                         "horário",
//                         "portão",
//                         "seguir para"
//                     },

//                     opcoesIngles = new List<string>
//                     {
//                         "boarding",
//                         "head to",
//                         "suitcase",
//                         "gate",
//                         "departure time",
//                         "destination",
//                         "booking confirmation",
//                         "check in a bag"
//                     },

//                     associacaoCorreta = new List<ParAssociacao>
//                     {
//                         new ParAssociacao { palavraPortugues = "mala de viagem", palavraIngles = "suitcase" },
//                         new ParAssociacao { palavraPortugues = "confirmação da reserva", palavraIngles = "booking confirmation" },
//                         new ParAssociacao { palavraPortugues = "despachar bagagem", palavraIngles = "check in a bag" },
//                         new ParAssociacao { palavraPortugues = "embarque", palavraIngles = "boarding" },
//                         new ParAssociacao { palavraPortugues = "destino", palavraIngles = "destination" },
//                         new ParAssociacao { palavraPortugues = "horário", palavraIngles = "departure time" },
//                         new ParAssociacao { palavraPortugues = "portão", palavraIngles = "gate" },
//                         new ParAssociacao { palavraPortugues = "seguir para", palavraIngles = "head to" }
//                     }
//                 }
//             );
//         }
//         else if(nivelAtual=="B1")
//         {
//             dialogoAtual = new List<string>()
//             {
//                 cumprimento+". Where are you travelling today?",
//                 "Could I see your passport and booking confirmation, please?",
//                 "Are you checking any bags today?",
//                 "You have a connecting flight, so your checked bag will be transferred automatically. Your second flight leaves from gate 24.",
//                 "Please check the departure screens in case the gate changes. Boarding usually starts 40 minutes before departure."
//             };

//             exerciciosSpeaking.Add(
//                     new ExercicioSpeaking()
//                     {
//                         enunciado = "Explain your travel plans to the airport staff, mentioning where is your final destination",
//                         opcoesFala = new List<string>()
//                         {
//                             " I’m travelling to Toronto, and I have a connecting flight\n",
//                             "I have a stop in Toronto before my final destination\n",
//                             "My connecting flight depart from different terminal\n"
//                         },
//                         respostaCorreta = 0
//                     }
//             ); 

//             exerciciosSpeaking.Add(
//                 new ExercicioSpeaking()
//                 {
//                     enunciado = "The airport staff asks for your passport and booking confirmation. You have both documents ready. Choose the most appropriate response.",
//                     opcoesFala = new List<string>()
//                     {
//                         "Certainly. I have both documents with me, and I can show them to you now.\n",
//                         "I need to collect my suitcase before I can show you my passport.",
//                         "My connecting flight leaves from another terminal, so I booked a hotel nearby."
//                     },
//                     respostaCorreta = 0
//                 }
//             );

//             exerciciosAlternativas.Add(
//                     new ExercicioAlternativas()
//                     {
//                         enunciado = "What will happen to the checked bag?",
//                         alternativas = new List<string>() { 
//                         "The user must collect it before boarding the first flight\r\n", 
//                         "It will be transferred automatically\r\n", 
//                         " It will be sent to the information desk\r\n", 
//                         "It must be carried as hand luggage\r\n" },
//                         alternativaCorreta = 1
//                     }
//             );   

//             exerciciosSpeaking.Add(
//                 new ExercicioSpeaking()
//                 {
//                     enunciado = "The airport staff advises you to check the departure screens for updates about your connecting flight. Choose the most appropriate response.",
//                     opcoesFala = new List<string>()
//                     {
//                         "hank you. I checked the gate tomorrow\n",
//                         "Thank you. I’ll check the restaurant menu",
//                         "Thank you. I’ll check the departure screens"
//                     },
//                     respostaCorreta = 2
//                 }
//             );

//             exerciciosAssociacao.Add(
//                 new ExercicioAssociacao()
//                 {
//                     enunciado = "Match the Portuguese word or expression with the English equivalent.",

//                     opcoesPort = new List<string>
//                     {
//                         "voo de conexão",
//                         "destino final",
//                         "bagagem despachada",
//                         "ser transferido automaticamente",
//                         "telas de partidas",
//                         "atrasado",
//                         "confirmação da reserva",
//                         "mudança de portão",
//                         "balcão de informações",
//                         "terminal"
//                     },

//                     opcoesIngles = new List<string>
//                     {
//                         "departure screens",
//                         "checked baggage",
//                         "terminal",
//                         "booking confirmation",
//                         "connecting flight",
//                         "gate change",
//                         "information desk",
//                         "delayed",
//                         "final destination",
//                         "be transferred automatically"
//                     },

//                     associacaoCorreta = new List<ParAssociacao>
//                     {
//                         new ParAssociacao { palavraPortugues = "voo de conexão", palavraIngles = "connecting flight" },
//                         new ParAssociacao { palavraPortugues = "destino final", palavraIngles = "final destination" },
//                         new ParAssociacao { palavraPortugues = "bagagem despachada", palavraIngles = "checked baggage" },
//                         new ParAssociacao { palavraPortugues = "ser transferido automaticamente", palavraIngles = "be transferred automatically" },
//                         new ParAssociacao { palavraPortugues = "telas de partidas", palavraIngles = "departure screens" },
//                         new ParAssociacao { palavraPortugues = "atrasado", palavraIngles = "delayed" },
//                         new ParAssociacao { palavraPortugues = "confirmação da reserva", palavraIngles = "booking confirmation" },
//                         new ParAssociacao { palavraPortugues = "mudança de portão", palavraIngles = "gate change" },
//                         new ParAssociacao { palavraPortugues = "balcão de informações", palavraIngles = "information desk" },
//                         new ParAssociacao { palavraPortugues = "terminal", palavraIngles = "terminal" }
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
//         fluxoCheckinConcluido = true;
//     }

//     private async Task interacaoA1()
//     {
//         int i = 0;

//         IniciarMissao("aeroporto_a1_3", "Dirija-se ao balcão de check-in e apresente seu passaporte", "Go to the check-in desk and present your passport");

//         await PlayAudioETexto(i++); // Where are you going?
//         await PlayAudioETexto(i++); // Can I see your passport, please?

//         exAtual = exerciciosSpeaking[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         ConcluirMissao("aeroporto_a1_3");
//         IniciarMissao("aeroporto_a1_4", "Consulte seu cartão de embarque", "Check your boarding pass");

//         await PlayAudioETexto(i++); // Boarding pass and gate number

//         exAtual = exerciciosCompletarFrase[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         ConcluirMissao("aeroporto_a1_4");
//         IniciarMissaoChegadaPortao(
//             "aeroporto_a1_5",
//             "Encontre o portão 12",
//             "Find gate 12",
//             pontoChegadaPortao12,
//             marcadorPortao12
//         );

//         await AbrirExercicio(ObterTipoExercicio(exerciciosAssociacao[0]), exerciciosAssociacao[0]);
//     }

//     private async Task interacaoA2()
//     {
//         int i = 0;

//         IniciarMissao("aeroporto_a2_2", "Informe seu destino ao funcionário", "Tell the airport employee your destination");

//         await PlayAudioETexto(i++); // Where are you flying today?

//         exAtual = exerciciosCriarFrases[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         ConcluirMissao("aeroporto_a2_2");
//         IniciarMissao("aeroporto_a2_3", "Apresente seu passaporte e confirmação da reserva", "Present your passport and booking confirmation");

//         await PlayAudioETexto(i++); // Passport and booking confirmation

//         exAtual = exerciciosSpeaking[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         ConcluirMissao("aeroporto_a2_3");
//         IniciarMissao("aeroporto_a2_4", "Informe sua bagagem para despacho", "Tell the employee about the luggage you need to check in");

//         await PlayAudioETexto(i++); // Bags to check in

//         exAtual = exerciciosSpeaking[1];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         ConcluirMissao("aeroporto_a2_4");
//         IniciarMissao("aeroporto_a2_5", "Descubra o horário de embarque", "Find out the boarding time");

//         await PlayAudioETexto(i++); // Gate and boarding time

//         exAtual = exerciciosAlternativas[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         ConcluirMissao("aeroporto_a2_5");

//         exAtual = exerciciosCompletarFrase[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         IniciarMissaoChegadaPortao(
//             "aeroporto_a2_6",
//             "Encontre o portão 18",
//             "Find gate 18",
//             pontoChegadaPortao18,
//             marcadorPortao18
//         );

//         await AbrirExercicio(ObterTipoExercicio(exerciciosAssociacao[0]), exerciciosAssociacao[0]);
//     }

//     private async Task interacaoB1()
//     {
//         int i = 0;

//         IniciarMissao("aeroporto_b1_3", "Dirija-se ao balcão de check-in", "Go to the check-in desk");

//         await PlayAudioETexto(i++); // Destination and connecting flight

//         ConcluirMissao("aeroporto_b1_3");
//         IniciarMissao("aeroporto_b1_4", "Informe seu destino e os detalhes do voo de conexão", "Provide your destination and connecting-flight details");

//         exAtual = exerciciosSpeaking[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         ConcluirMissao("aeroporto_b1_4");
//         IniciarMissao("aeroporto_b1_5", "Apresente seus documentos de viagem", "Present your travel documents");

//         await PlayAudioETexto(i++); // Passport and booking confirmation

//         await AbrirExercicio(ObterTipoExercicio(exerciciosSpeaking[1]), exerciciosSpeaking[1]);

//         ConcluirMissao("aeroporto_b1_5");
//         IniciarMissao("aeroporto_b1_6", "Confirme as informações sobre a bagagem despachada", "Confirm the checked-baggage information");

//         await PlayAudioETexto(i++); // Are you checking any bags?

//         // O funcionário informa a transferência automática da bagagem nesta fala.
//         await PlayAudioETexto(i++); // Checked bag transfer and gate 24

//         exAtual = exerciciosAlternativas[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         ConcluirMissao("aeroporto_b1_6");
//         IniciarMissao("aeroporto_b1_7", "Consulte os painéis de partida", "Check the departure screens");

//         await PlayAudioETexto(i++); // Check departure screens

//         exAtual = exerciciosSpeaking[2];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         ConcluirMissao("aeroporto_b1_7");
//         IniciarMissaoChegadaPortao(
//             "aeroporto_b1_8",
//             "Encontre o portão 24 para seu voo",
//             "Find gate 24 for your flight",
//             pontoChegadaPortao24,
//             marcadorPortao24
//         );

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

public class checkinNPC : InteracaoNPC, IRoteiroNpcCheckpoint
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

    [System.NonSerialized] public List<ExercicioBase> exerciciosCompletarFrase;
    [System.NonSerialized] public List<ExercicioBase> exerciciosCriarFrases;
    [System.NonSerialized] public List<ExercicioBase> exerciciosSpeaking;
    [System.NonSerialized] public List<ExercicioBase> exerciciosAlternativas;
    [System.NonSerialized] public List<ExercicioBase> exerciciosWriting;
    [System.NonSerialized] public List<ExercicioBase> exerciciosAssociacao;

    public string idNpcParaVoz = "airportInfo";
    public Transform pontoChegadaPortao12;
    public Transform pontoChegadaPortao18;
    public Transform pontoChegadaPortao24;
    public GameObject marcadorPortao12;
    public GameObject marcadorPortao18;
    public GameObject marcadorPortao24;

    [Header("Consulta ao cartão de embarque")]
    [SerializeField] private GameObject painelCartaoEmbarque;
    [SerializeField] private TMP_Text textoCartaoEmbarque;
    [SerializeField] private TMP_Text instrucoesCartaoEmbarque;

    private bool botaoCartaoVRPressionado;
    private bool cartaoEmbarqueAberto;

    private bool aguardandoChegadaPortao;
    private bool fluxoCheckinConcluido;
    private const string NpcKey = "checkin_npc";
    private ExerciseLoadResult catalogoExercicios;
    private string idMissaoChegadaPortao;
    private Transform pontoChegadaPortaoAtual;
    private GameObject marcadorPortaoAtual;
    private float tempoParadoPortao;
    private bool chegadaPortaoConfirmada;
    private bool cartaoEmbarqueConsultado;
    private Vector3 ultimaPosicaoPlayer;

    private const float DistanciaChegadaPortao = 2f;
    private const float TempoNecessarioPortao = 3f;
    private const float VelocidadeMaximaParado = 0.15f;

    protected override void Start()
    {
        base.Start();
        if (painelCartaoEmbarque != null && !transform.IsChildOf(painelCartaoEmbarque.transform))
            painelCartaoEmbarque.SetActive(false);
        if (instrucoesCartaoEmbarque != null)
            instrucoesCartaoEmbarque.enabled = false;
        DesativarMarcadoresPortoes();
        nivelAtual = DadosJogador.nivelUsuario;
        nomeExibicaoLegenda = "Airport Employee";
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

    private void Update()
    {
        if (!PertenceAoCenarioAtivo)
            return;

        AtualizarVisita();
        AtualizarConsultaCartaoEmbarque();
        if (cartaoEmbarqueAberto || !fluxoCheckinConcluido || !aguardandoChegadaPortao || chegadaPortaoConfirmada)
            return;

        if (MissionManager.Instance?.MissaoAtiva(idMissaoChegadaPortao) != true
            || pontoChegadaPortaoAtual == null || movimentoPlayer == null)
        {
            tempoParadoPortao = 0f;
            return;
        }

        Vector3 posicaoAtual = movimentoPlayer.transform.position;
        Vector3 deslocamento = posicaoAtual - pontoChegadaPortaoAtual.position;
        deslocamento.y = 0f;
        Vector3 movimento = posicaoAtual - ultimaPosicaoPlayer;
        movimento.y = 0f;
        ultimaPosicaoPlayer = posicaoAtual;

        float tempoFrame = Time.deltaTime;
        bool dentroDaArea = deslocamento.sqrMagnitude <= DistanciaChegadaPortao * DistanciaChegadaPortao;
        bool parado = movimento.magnitude / Mathf.Max(tempoFrame, 0.0001f) <= VelocidadeMaximaParado;
        tempoParadoPortao = dentroDaArea && parado ? tempoParadoPortao + tempoFrame : 0f;

        if (tempoParadoPortao >= TempoNecessarioPortao)
        {
            chegadaPortaoConfirmada = true;
            string ponto = nivelAtual == "A1" ? "portao_12"
                : nivelAtual == "A1-A2" || nivelAtual == "A2" ? "portao_18" : "portao_24";
            EmitirEventoMissao("permanencia_confirmada", ponto);
            CapturarCaminhoCheckpoint();
            TentarConcluirMissaoPortao();
        }
    }

    protected override void ReiniciarVisita()
    {
        aguardandoChegadaPortao = fluxoCheckinConcluido = chegadaPortaoConfirmada = false;
        cartaoEmbarqueConsultado = cartaoEmbarqueAberto = botaoCartaoVRPressionado = false;
        tempoParadoPortao = 0f;
        DesativarMarcadoresPortoes();
        if (CartaoEmbarqueConfigurado())
            painelCartaoEmbarque.SetActive(false);
        if (instrucoesCartaoEmbarque != null)
            instrucoesCartaoEmbarque.enabled = false;
    }

    private string MissaoConsultaCartao()
    {
        switch (nivelAtual)
        {
            case "A1": return "aeroporto_a1_4";
            case "A1-A2": return "aeroporto_a1a2_5";
            case "A2-B1": return "aeroporto_a2b1_6";
            default: return null;
        }
    }

    private void AtualizarConsultaCartaoEmbarque()
    {
        bool acionado = !ModoVR && Input.GetKeyDown(KeyCode.B);
        if (ModoVR)
        {
            var controle = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.RightHand);
            bool pressionado = controle.TryGetFeatureValue(UnityEngine.XR.CommonUsages.secondaryButton, out bool valor) && valor;
            acionado = pressionado && !botaoCartaoVRPressionado;
            botaoCartaoVRPressionado = pressionado;
        }

        if (!acionado || !fluxoCheckinConcluido)
            return;

        if (cartaoEmbarqueAberto)
        {
            if (painelCartaoEmbarque != null)
                painelCartaoEmbarque.SetActive(false);
            cartaoEmbarqueAberto = false;
            GameProgress.EstaEmDialogo = false;
            LiberarControlePlayer();
            CapturarCaminhoCheckpoint();
            TentarConcluirMissaoPortao();
            return;
        }

        string missao = MissaoConsultaCartao();
        if (cartaoEmbarqueConsultado || GameProgress.EstaEmDialogo || missao == null
            || MissionManager.Instance?.MissaoAtiva(missao) != true)
            return;

        if (!CartaoEmbarqueConfigurado())
        {
            Debug.LogError("Airport: vincule um painel de cartão de embarque e um texto TMP dentro dele ao checkinNPC.", this);
            return;
        }

        string dadosEmbarque = nivelAtual == "A1" ? "Gate: 12"
            : nivelAtual == "A1-A2" ? "Gate: 18\nBoarding: 2:30 p.m."
            : "Connecting flight — Gate: 24\nBoarding: 2:30 p.m.";
        textoCartaoEmbarque.text = $"Boarding pass\n{dadosEmbarque}\n\n{(ModoVR ? "Secondary button" : "B")}: Close";
        painelCartaoEmbarque.SetActive(true);
        if (!textoCartaoEmbarque.isActiveAndEnabled)
        {
            painelCartaoEmbarque.SetActive(false);
            Debug.LogError("Airport: o texto do cartão permanece inativo quando o painel é aberto.", this);
            return;
        }

        cartaoEmbarqueAberto = true;
        GameProgress.EstaEmDialogo = true;
        TravarControlePlayer();
        RegistrarConsultaCartaoEmbarque();
        if (instrucoesCartaoEmbarque != null)
            instrucoesCartaoEmbarque.enabled = false;
    }

    private bool CartaoEmbarqueConfigurado()
    {
        return painelCartaoEmbarque != null && textoCartaoEmbarque != null
            && !transform.IsChildOf(painelCartaoEmbarque.transform)
            && textoCartaoEmbarque.transform.IsChildOf(painelCartaoEmbarque.transform)
            && (painelCartaoEmbarque.transform.parent == null
                || painelCartaoEmbarque.transform.parent.gameObject.activeInHierarchy);
    }

    private void RegistrarConsultaCartaoEmbarque()
    {
        if (!fluxoCheckinConcluido || !cartaoEmbarqueAberto || cartaoEmbarqueConsultado)
            return;

        switch (nivelAtual)
        {
            case "A1":
                if (MissionManager.Instance?.MissaoAtiva("aeroporto_a1_4") != true)
                    return;
                cartaoEmbarqueConsultado = true;
                EmitirEventoMissao("cartao_embarque_consultado", "cartao_embarque");
                ConcluirMissao("aeroporto_a1_4");
                IniciarMissaoChegadaPortao("aeroporto_a1_5", "Find gate 12", "Encontre o portão 12", pontoChegadaPortao12, marcadorPortao12);
                break;
            case "A1-A2":
                if (MissionManager.Instance?.MissaoAtiva("aeroporto_a1a2_5") != true)
                    return;
                cartaoEmbarqueConsultado = true;
                EmitirEventoMissao("cartao_embarque_consultado", "cartao_embarque");
                TentarConcluirMissaoPortao();
                break;
            case "A2-B1":
                if (MissionManager.Instance?.MissaoAtiva("aeroporto_a2b1_6") != true)
                    return;
                cartaoEmbarqueConsultado = true;
                EmitirEventoMissao("cartao_embarque_consultado", "cartao_embarque");
                ConcluirMissao("aeroporto_a2b1_6");
                if (MissionManager.Instance?.MissaoConcluida("aeroporto_a2b1_6") == true)
                    IniciarMissaoChegadaPortao("aeroporto_a2b1_7", "Find gate 24 for your flight", "Encontre o portão 24 para seu voo", pontoChegadaPortao24, marcadorPortao24);
                break;
        }
    }

    private void DesativarMarcadoresPortoes()
    {
        if (marcadorPortao12 != null)
            marcadorPortao12.SetActive(false);
        if (marcadorPortao18 != null)
            marcadorPortao18.SetActive(false);
        if (marcadorPortao24 != null)
            marcadorPortao24.SetActive(false);
    }

    private void IniciarMissaoChegadaPortao(
        string id,
        string texto,
        string traducao,
        Transform pontoChegada,
        GameObject marcador)
    {
        IniciarMissao(id, texto, traducao);
        AguardarChegadaAoPortao(id, pontoChegada, marcador);
    }

    private void AguardarChegadaAoPortao(string id, Transform pontoChegada, GameObject marcador)
    {
        idMissaoChegadaPortao = id;
        pontoChegadaPortaoAtual = pontoChegada;
        marcadorPortaoAtual = marcador;
        aguardandoChegadaPortao = true;
        chegadaPortaoConfirmada = false;
        tempoParadoPortao = 0f;

        if (movimentoPlayer != null)
            ultimaPosicaoPlayer = movimentoPlayer.transform.position;

        if (marcadorPortaoAtual != null)
            marcadorPortaoAtual.SetActive(true);
        else
            Debug.LogWarning("checkinNPC: atribua o marcador do portão no Inspector.");

        if (pontoChegadaPortaoAtual == null)
            Debug.LogWarning("checkinNPC: atribua o ponto de chegada do portão no Inspector.");
    }

    private static string MissaoPortaoCheckpoint(string nivel) => nivel switch
    {
        "A1" => "aeroporto_a1_5",
        "A1-A2" => "aeroporto_a1a2_5",
        "A2" => "aeroporto_a2_6",
        "A2-B1" => "aeroporto_a2b1_7",
        "B1" => "aeroporto_b1_8",
        _ => null
    };

    internal static bool MissaoRetomavelCheckpoint(string nivel, string missao) =>
        !string.IsNullOrEmpty(missao) && (missao == MissaoPortaoCheckpoint(nivel)
            || (nivel == "A1" && missao == "aeroporto_a1_4")
            || (nivel == "A2-B1" && missao == "aeroporto_a2b1_6"));

    private void CapturarCaminhoCheckpoint()
    {
        if (!fluxoCheckinConcluido || cartaoEmbarqueAberto || MissionManager.Instance?.UsaCatalogoAirport != true)
            return;
        string missao = MissaoConsultaCartao();
        if (missao == null || MissionManager.Instance.MissaoAtiva(missao) == false)
            missao = MissaoPortaoCheckpoint(nivelAtual);
        if (MissionManager.Instance.MissaoAtiva(missao))
            CheckpointJogo.Instance?.CapturarCaminhoAirport(missao);
    }

    internal void RetomarCaminhoCheckpoint(int visita, Newtonsoft.Json.Linq.JObject missao)
    {
        ValidarVisita(visita);
        AtualizarVisita();
        nivelAtual = SessaoCenario.Atual.Nivel;
        interacaoConcluida = true;
        fluxoCheckinConcluido = true;
        string chave = (string)missao["mission_key"];
        string portao = MissaoPortaoCheckpoint(nivelAtual);
        var eventos = (Newtonsoft.Json.Linq.JArray)missao["satisfied_events"];
        cartaoEmbarqueConsultado = (nivelAtual == "A1" || nivelAtual == "A2-B1") && chave == portao;
        bool chegou = false;
        foreach (var evento in eventos)
        {
            cartaoEmbarqueConsultado |= (string)evento["tipo"] == "cartao_embarque_consultado";
            chegou |= (string)evento["tipo"] == "permanencia_confirmada";
        }
        if (chave == portao)
        {
            if (nivelAtual == "A1")
                AguardarChegadaAoPortao(portao, pontoChegadaPortao12, marcadorPortao12);
            else if (nivelAtual == "A1-A2" || nivelAtual == "A2")
                AguardarChegadaAoPortao(portao, pontoChegadaPortao18, marcadorPortao18);
            else
                AguardarChegadaAoPortao(portao, pontoChegadaPortao24, marcadorPortao24);
            chegadaPortaoConfirmada = chegou;
        }
        if (!cartaoEmbarqueConsultado && MissaoConsultaCartao() != null && instrucoesCartaoEmbarque != null
            && CartaoEmbarqueConfigurado())
        {
            instrucoesCartaoEmbarque.text = ModoVR ? "Secondary button: View boarding pass" : "B: View boarding pass";
            instrucoesCartaoEmbarque.enabled = true;
        }
        LiberarControlePlayer();
    }

    private void TentarConcluirMissaoPortao()
    {
        if (cartaoEmbarqueAberto || !aguardandoChegadaPortao || !chegadaPortaoConfirmada)
            return;

        MissionManager missoes = MissionManager.Instance;
        if (missoes == null)
            return;
        if (!missoes.UsaCatalogoAirport)
        {
            if (nivelAtual == "A1-A2" && !cartaoEmbarqueConsultado)
                return;
            if (missoes.MissaoAtiva(idMissaoChegadaPortao))
                missoes.ConcluirMissao(idMissaoChegadaPortao);
        }
        if (!missoes.MissaoConcluida(idMissaoChegadaPortao))
            return;
        aguardandoChegadaPortao = false;
        if (marcadorPortaoAtual != null)
            marcadorPortaoAtual.SetActive(false);

        FinalizarCenarioAposChegadaPortao();
    }

    private async void FinalizarCenarioAposChegadaPortao()
    {
        if (RevisaoManager.Instance != null)
            await RevisaoManager.Instance.ExecutarRevisaoEExibirModalFinal(this);
        else
            Debug.LogError("RevisaoManager não está disponível para finalizar Airport.");
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
                "Hello. Where are you going?", 
                "Can I see your passport, please?",
                "Here is your boarding pass. Your gate is twelve."
            };

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Show your passport to the airport staff.",
                        opcoesFala = new List<string>()
                        {
                            "Here is my passport\n",
                            "Here is my breakfast\n",
                            "My passport is a boarding gate\n"
                        },
                        respostaCorreta = 0
                    }
            );

            exerciciosCompletarFrase.Add(
                new ExercicioCompletarFrase()
                {
                    enunciado = "Complete the sentence with the correct word:",
                    fraseAntes = "Your boarding gate is ",
                    fraseDepois = ".",
                    respostaCorreta = "12",
                    opcoes = new List<string>
                    {
                        "20",
                        "12",
                        "passport",
                        "18",
                        "boarding",
                        "21",
                        "airport",
                        "2",
                        "24",
                        "ticket"
                    }
                }
            );

            exerciciosAssociacao.Add(
                new ExercicioAssociacao()
                {
                    enunciado = "Match the Portuguese word with the English word.",

                    opcoesPort = new List<string>
                    {
                        "passaporte",
                        "aeroporto",
                        "portão de embarque",
                        "cartão de embarque",
                        "voo",
                        "bagagem",
                        "balcão de check-in",
                        "escadas"
                    },

                    opcoesIngles = new List<string>
                    {
                        "flight",
                        "stairs",
                        "passport",
                        "check-in desk",
                        "luggage",
                        "boarding pass",
                        "airport",
                        "boarding gate"
                    },

                    associacaoCorreta = new List<ParAssociacao>
                    {
                        new ParAssociacao { palavraPortugues = "passaporte", palavraIngles = "passport" },
                        new ParAssociacao { palavraPortugues = "aeroporto", palavraIngles = "airport" },
                        new ParAssociacao { palavraPortugues = "portão de embarque", palavraIngles = "boarding gate" },
                        new ParAssociacao { palavraPortugues = "cartão de embarque", palavraIngles = "boarding pass" },
                        new ParAssociacao { palavraPortugues = "voo", palavraIngles = "flight" },
                        new ParAssociacao { palavraPortugues = "bagagem", palavraIngles = "luggage" },
                        new ParAssociacao { palavraPortugues = "balcão de check-in", palavraIngles = "check-in desk" },
                        new ParAssociacao { palavraPortugues = "escadas", palavraIngles = "stairs" }
                    }
                }
            );
        }
        else if(nivelAtual=="A2")
        {
            dialogoAtual = new List<string>()
            {
                cumprimento+". Where are you flying today?",
                "May I see your passport and booking confirmation, please?",
                "Do you have any bags to check in?",
                "Your gate is eighteen, and boarding starts at 2:30 p.m. Have a nice flight!"
            };

            exerciciosCriarFrases.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Arrange the words to form the correct sentence in english: Eu estou indo para Londres",
                    blocosPalavras = new List<string>
                    {
                        "London",
                        "going",
                        "to",
                        "I",
                        "Paris",
                        "am",
                        "traveling",
                        "from",
                        "is",
                        "fly",
                        "the"
                    },
                    respostaCorreta = "I am going to London\n"
                }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "The airport staff asks to see your passport and booking confirmation. Choose the most appropriate response.",
                        opcoesFala = new List<string>()
                        {
                            "Here is my passport and booking confirmation\n",
                            "I left my booking confirmation at the hotel\n",
                            "I showed my passport to the hotel receptionist last week\n"
                        },
                        respostaCorreta = 0
                    }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "The airport staff asks if you have any luggage to check in. Choose the most appropriate response.",
                        opcoesFala = new List<string>()
                        {
                            "Yes, I need to check in at gate 18\n",
                            "Yes, I have one suitcase to check in\n",
                            "Yes, I have already arrived at my destination\n"
                        },
                        respostaCorreta = 1
                    }
            );

            exerciciosAlternativas.Add(
                    new ExercicioAlternativas()
                    {
                        enunciado = "What time does your boarding start?",
                        alternativas = new List<string>() { 
                        "Two p.m.\r\n", 
                        "Two thirty p.m.\r\n", 
                        "Three thirty p.m.\r\n", 
                        "Eleven thirty p.m.\r\n" },
                        alternativaCorreta = 1
                    }
            );     

            exerciciosCompletarFrase.Add(
                new ExercicioCompletarFrase()
                {
                    enunciado = "Complete the sentence with the correct word:",
                    fraseAntes = "The boarding gate is ",
                    fraseDepois = ".",
                    respostaCorreta = "18",
                    opcoes = new List<string>
                    {
                        "passport",
                        "24",
                        "18",
                        "luggage",
                        "12",
                        "ticket",
                        "8",
                        "28",
                        "airport",
                        "21",
                        "boarding",
                        "80"
                    }
                }
            );

            exerciciosAssociacao.Add(
                new ExercicioAssociacao()
                {
                    enunciado = "Match the Portuguese word or expression with the English equivalent.",

                    opcoesPort = new List<string>
                    {
                        "mala de viagem",
                        "confirmação da reserva",
                        "despachar bagagem",
                        "embarque",
                        "destino",
                        "horário",
                        "portão",
                        "seguir para"
                    },

                    opcoesIngles = new List<string>
                    {
                        "boarding",
                        "head to",
                        "suitcase",
                        "gate",
                        "departure time",
                        "destination",
                        "booking confirmation",
                        "check in a bag"
                    },

                    associacaoCorreta = new List<ParAssociacao>
                    {
                        new ParAssociacao { palavraPortugues = "mala de viagem", palavraIngles = "suitcase" },
                        new ParAssociacao { palavraPortugues = "confirmação da reserva", palavraIngles = "booking confirmation" },
                        new ParAssociacao { palavraPortugues = "despachar bagagem", palavraIngles = "check in a bag" },
                        new ParAssociacao { palavraPortugues = "embarque", palavraIngles = "boarding" },
                        new ParAssociacao { palavraPortugues = "destino", palavraIngles = "destination" },
                        new ParAssociacao { palavraPortugues = "horário", palavraIngles = "departure time" },
                        new ParAssociacao { palavraPortugues = "portão", palavraIngles = "gate" },
                        new ParAssociacao { palavraPortugues = "seguir para", palavraIngles = "head to" }
                    }
                }
            );
        }
        else if(nivelAtual=="B1")
        {
            dialogoAtual = new List<string>()
            {
                cumprimento+". Where are you travelling today?",
                "Could I see your passport and booking confirmation, please?",
                "Are you checking any bags today?",
                "You have a connecting flight, so your checked bag will be transferred automatically. Your second flight leaves from gate 24.",
                "Please check the departure screens in case the gate changes. Boarding usually starts 40 minutes before departure."
            };

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Explain your travel plans to the airport staff, mentioning where is your final destination",
                        opcoesFala = new List<string>()
                        {
                            " I’m travelling to Toronto, and I have a connecting flight\n",
                            "I have a stop in Toronto before my final destination\n",
                            "My connecting flight depart from different terminal\n"
                        },
                        respostaCorreta = 0
                    }
            ); 

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "The airport staff asks for your passport and booking confirmation. You have both documents ready. Choose the most appropriate response.",
                    opcoesFala = new List<string>()
                    {
                        "Certainly. I have both documents with me, and I can show them to you now.\n",
                        "I need to collect my suitcase before I can show you my passport.",
                        "My connecting flight leaves from another terminal, so I booked a hotel nearby."
                    },
                    respostaCorreta = 0
                }
            );

            exerciciosAlternativas.Add(
                    new ExercicioAlternativas()
                    {
                        enunciado = "What will happen to the checked bag?",
                        alternativas = new List<string>() { 
                        "The user must collect it before boarding the first flight\r\n", 
                        "It will be transferred automatically\r\n", 
                        " It will be sent to the information desk\r\n", 
                        "It must be carried as hand luggage\r\n" },
                        alternativaCorreta = 1
                    }
            );   

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "The airport staff advises you to check the departure screens for updates about your connecting flight. Choose the most appropriate response.",
                    opcoesFala = new List<string>()
                    {
                        "hank you. I checked the gate tomorrow\n",
                        "Thank you. I’ll check the restaurant menu",
                        "Thank you. I’ll check the departure screens"
                    },
                    respostaCorreta = 2
                }
            );

            exerciciosAssociacao.Add(
                new ExercicioAssociacao()
                {
                    enunciado = "Match the Portuguese word or expression with the English equivalent.",

                    opcoesPort = new List<string>
                    {
                        "voo de conexão",
                        "destino final",
                        "bagagem despachada",
                        "ser transferido automaticamente",
                        "telas de partidas",
                        "atrasado",
                        "confirmação da reserva",
                        "mudança de portão",
                        "balcão de informações",
                        "terminal"
                    },

                    opcoesIngles = new List<string>
                    {
                        "departure screens",
                        "checked baggage",
                        "terminal",
                        "booking confirmation",
                        "connecting flight",
                        "gate change",
                        "information desk",
                        "delayed",
                        "final destination",
                        "be transferred automatically"
                    },

                    associacaoCorreta = new List<ParAssociacao>
                    {
                        new ParAssociacao { palavraPortugues = "voo de conexão", palavraIngles = "connecting flight" },
                        new ParAssociacao { palavraPortugues = "destino final", palavraIngles = "final destination" },
                        new ParAssociacao { palavraPortugues = "bagagem despachada", palavraIngles = "checked baggage" },
                        new ParAssociacao { palavraPortugues = "ser transferido automaticamente", palavraIngles = "be transferred automatically" },
                        new ParAssociacao { palavraPortugues = "telas de partidas", palavraIngles = "departure screens" },
                        new ParAssociacao { palavraPortugues = "atrasado", palavraIngles = "delayed" },
                        new ParAssociacao { palavraPortugues = "confirmação da reserva", palavraIngles = "booking confirmation" },
                        new ParAssociacao { palavraPortugues = "mudança de portão", palavraIngles = "gate change" },
                        new ParAssociacao { palavraPortugues = "balcão de informações", palavraIngles = "information desk" },
                        new ParAssociacao { palavraPortugues = "terminal", palavraIngles = "terminal" }
                    }
                }
            );
        }
        else if (nivelAtual == "A1-A2")
{
    dialogoAtual = new List<string>()
    {
        cumprimento + ". Where are you flying today?",
        "May I see your passport and booking confirmation, please?",
        "Here is your boarding pass. Your gate is eighteen, and boarding starts at 2:30 p.m."
    };

    exerciciosCriarFrases.Add(
        new ExercicioBlocos()
        {
            enunciado = "Arrange the words to form the correct sentence in English: Estou indo para Londres.",
            blocosPalavras = new List<string>()
            {
                "London",
                "going",
                "I",
                "to",
                "am",
                "Paris",
                "traveling",
                "from",
                "the",
                "is"
            },
            respostaCorreta = "I am going to London\n"
        }
    );

    exerciciosSpeaking.Add(
        new ExercicioSpeaking()
        {
            enunciado = "The airport employee asks to see your passport and booking confirmation. Choose the most appropriate response.",
            opcoesFala = new List<string>()
            {
                "I would like to change my boarding gate.\n",
                "My suitcase is already at the destination.\n",
                "Here are my passport and booking confirmation.\n"
            },
            respostaCorreta = 2
        }
    );

    exerciciosCompletarFrase.Add(
        new ExercicioCompletarFrase()
        {
            enunciado = "Complete the sentence with the correct number:",
            fraseAntes = "Your boarding gate is ",
            fraseDepois = ".",
            respostaCorreta = "18",
            opcoes = new List<string>()
            {
                "passport",
                "airport",
                "boarding",
                "20",
                "ticket",
                "12",
                "18",
                "24"
            }
        }
    );

    exerciciosAssociacao.Add(
        new ExercicioAssociacao()
        {
            enunciado = "Match the Portuguese word or expression with the English equivalent.",

            opcoesPort = new List<string>()
            {
                "passaporte",
                "confirmação da reserva",
                "cartão de embarque",
                "portão de embarque",
                "mala de viagem",
                "despachar bagagem",
                "destino",
                "horário de embarque"
            },

            opcoesIngles = new List<string>()
            {
                "boarding pass",
                "destination",
                "suitcase",
                "boarding time",
                "passport",
                "check in a bag",
                "booking confirmation",
                "boarding gate"
            },

            associacaoCorreta = new List<ParAssociacao>()
            {
                new ParAssociacao { palavraPortugues = "passaporte", palavraIngles = "passport" },
                new ParAssociacao { palavraPortugues = "confirmação da reserva", palavraIngles = "booking confirmation" },
                new ParAssociacao { palavraPortugues = "cartão de embarque", palavraIngles = "boarding pass" },
                new ParAssociacao { palavraPortugues = "portão de embarque", palavraIngles = "boarding gate" },
                new ParAssociacao { palavraPortugues = "mala de viagem", palavraIngles = "suitcase" },
                new ParAssociacao { palavraPortugues = "despachar bagagem", palavraIngles = "check in a bag" },
                new ParAssociacao { palavraPortugues = "destino", palavraIngles = "destination" },
                new ParAssociacao { palavraPortugues = "horário de embarque", palavraIngles = "boarding time" }
            }
        }
    );
}
else if (nivelAtual == "A2-B1")
{
    dialogoAtual = new List<string>()
    {
        cumprimento + ". Where are you travelling today?",
        "Could I see your passport and booking confirmation, please?",
        "Are you checking any bags today? Your checked luggage will be transferred automatically to your connecting flight.",
        "Your next flight leaves from gate 24. Boarding starts at 2:30 p.m. Please check the departure screens for any updates."
    };

    exerciciosSpeaking.Add(
        new ExercicioSpeaking()
        {
            enunciado = "Explain your travel plans to the airport employee, mentioning your destination and connecting flight.",
            opcoesFala = new List<string>()
            {
                "My boarding pass is at the hotel reception.\n",
                "I'm travelling to Toronto, and I have a connecting flight.\n",
                "I'm collecting my luggage at the information desk.\n"
            },
            respostaCorreta = 1
        }
    );

    exerciciosSpeaking.Add(
        new ExercicioSpeaking()
        {
            enunciado = "The employee asks for your passport and booking confirmation. Choose the most appropriate response.",
            opcoesFala = new List<string>()
            {
                "I need to check the departure screens first.\n",
                "My connecting flight leaves from gate 24.\n",
                "Certainly. I have both documents with me.\n"
            },
            respostaCorreta = 2
        }
    );

    exerciciosAlternativas.Add(
        new ExercicioAlternativas()
        {
            enunciado = "You have a connecting flight. What will happen to your checked luggage?",
            alternativas = new List<string>()
            {
                "You must carry it to the boarding gate yourself.\r\n",
                "It will be sent to the information desk.\r\n",
                "You must collect it before checking in.\r\n",
                "It will be transferred automatically to the connecting flight.\r\n"
            },
            alternativaCorreta = 3
        }
    );

    exerciciosSpeaking.Add(
        new ExercicioSpeaking()
        {
            enunciado = "The employee advises you to check the departure screens for updates. Choose the most appropriate response.",
            opcoesFala = new List<string>()
            {
                "Thank you. I'll collect my luggage now.\n",
                "Thank you. I'll ask for a hotel room.\n",
                "Thank you. I'll check the departure screens.\n"
            },
            respostaCorreta = 2
        }
    );

    exerciciosAssociacao.Add(
        new ExercicioAssociacao()
        {
            enunciado = "Match the Portuguese word or expression with the English equivalent.",

            opcoesPort = new List<string>()
            {
                "voo de conexão",
                "destino final",
                "bagagem despachada",
                "ser transferido automaticamente",
                "telas de partidas",
                "mudança de portão",
                "confirmação da reserva",
                "horário de embarque",
                "balcão de informações",
                "terminal"
            },

            opcoesIngles = new List<string>()
            {
                "departure screens",
                "checked baggage",
                "terminal",
                "booking confirmation",
                "connecting flight",
                "gate change",
                "information desk",
                "final destination",
                "boarding time",
                "be transferred automatically"
            },

            associacaoCorreta = new List<ParAssociacao>()
            {
                new ParAssociacao { palavraPortugues = "voo de conexão", palavraIngles = "connecting flight" },
                new ParAssociacao { palavraPortugues = "destino final", palavraIngles = "final destination" },
                new ParAssociacao { palavraPortugues = "bagagem despachada", palavraIngles = "checked baggage" },
                new ParAssociacao { palavraPortugues = "ser transferido automaticamente", palavraIngles = "be transferred automatically" },
                new ParAssociacao { palavraPortugues = "telas de partidas", palavraIngles = "departure screens" },
                new ParAssociacao { palavraPortugues = "mudança de portão", palavraIngles = "gate change" },
                new ParAssociacao { palavraPortugues = "confirmação da reserva", palavraIngles = "booking confirmation" },
                new ParAssociacao { palavraPortugues = "horário de embarque", palavraIngles = "boarding time" },
                new ParAssociacao { palavraPortugues = "balcão de informações", palavraIngles = "information desk" },
                new ParAssociacao { palavraPortugues = "terminal", palavraIngles = "terminal" }
            }
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

    public override bool PodeInteragir()
    {
        if (!base.PodeInteragir())
            return false;

        MissionManager missoes = MissionManager.Instance;
        if (missoes == null)
            return true;

        return DadosJogador.nivelUsuario switch
        {
            "A1" => missoes.MissaoAtiva("aeroporto_a1_3"),
            "A1-A2" => missoes.MissaoAtiva("aeroporto_a1a2_3"),
            "A2" => missoes.MissaoAtiva("aeroporto_a2_1"),
            "A2-B1" => missoes.MissaoConcluida("aeroporto_a2b1_3"),
            "B1" => missoes.MissaoAtiva("aeroporto_b1_3"),
            _ => true
        };
    }

    protected override Task IniciarInteracao() => ExecutarAtendimento(0, false);

    private async Task ExecutarAtendimento(int inicio, bool retomada)
    {
        int versao = SessaoCenario.Atual.Versao;
        GameProgress.EstaEmDialogo = true;
        nivelAtual = DadosJogador.nivelUsuario;
        if (!retomada) switch (nivelAtual)
        {
            case "A1":
                EmitirEventoMissao("npc_interagido", "checkin_npc");
                ConcluirMissao("aeroporto_a1_3");
                break;
            case "A1-A2":
                EmitirEventoMissao("npc_interagido", "checkin_npc");
                ConcluirMissao("aeroporto_a1a2_3");
                break;
            case "A2":
                EmitirEventoMissao("npc_interagido", "checkin_npc");
                ConcluirMissao("aeroporto_a2_1");
                break;
            case "B1":
                EmitirEventoMissao("npc_interagido", "checkin_npc");
                ConcluirMissao("aeroporto_b1_3");
                break;
        }

        InicializarConteudosPorNivel();
        catalogoExercicios = await ExerciseManager.Instance.LoadForNpcAsync(NpcKey, nivelAtual);
        if (!catalogoExercicios.Success)
            Debug.LogWarning("checkinNPC: usando exerc?cios locais. " + catalogoExercicios.Error);

        ValidarVisita(versao);
        await CriarRoteiro(versao).Executar(inicio);

        // Desativa o estado quando a conversa acabar completamente!
        GameProgress.EstaEmDialogo = false;
        //MissionManager.Instance.ConcluirMissao("finalizar_compra");

        if (modalLegenda != null)
            modalLegenda.SetActive(false);

        if (textoPularDialogo != null)
            textoPularDialogo.gameObject.SetActive(false);

        GameProgress.EstaEmDialogo = false;

        LiberarControlePlayer();
        fluxoCheckinConcluido = true;
        if (nivelAtual == "A1")
        {
            EmitirEventoMissao("conversa_encerrada", "checkin_npc");
            IniciarMissao("aeroporto_a1_4", "Check your boarding pass", "Consulte seu cartão de embarque");
        }

        if (MissaoConsultaCartao() != null)
        {
            if (!CartaoEmbarqueConfigurado())
                Debug.LogError("Airport: configure Painel Cartao Embarque e Texto Cartao Embarque para permitir a consulta.", this);
            else if (instrucoesCartaoEmbarque != null)
            {
                instrucoesCartaoEmbarque.text = ModoVR ? "Secondary button: View boarding pass" : "B: View boarding pass";
                instrucoesCartaoEmbarque.enabled = true;
            }
        }
        CapturarCaminhoCheckpoint();
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
        var orientacoes = FindAnyObjectByType<AirportInfoNPC>();
        if (orientacoes != null && nivelAtual != "A2")
            orientacoes.RetomarAposOrientacoesCheckpoint(versao);
        await RoteiroNpcCheckpoint.ExigirCatalogo(this, versao, token);
        return async () =>
        {
            ValidarVisita(versao);
            TravarControlePlayer();
            if (cursor.Situacao.Length == 2 && cursor.Situacao[1] == "gate.ready")
            {
                if (nivelAtual == "A2" || nivelAtual == "A1-A2")
                    AguardarChegadaAoPortao(MissaoPortaoCheckpoint(nivelAtual), pontoChegadaPortao18, marcadorPortao18);
                else if (nivelAtual == "B1")
                    AguardarChegadaAoPortao(MissaoPortaoCheckpoint(nivelAtual), pontoChegadaPortao24, marcadorPortao24);
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
        var exerciciosCompletarFrase = descricao?.exerciciosCompletarFrase ?? this.exerciciosCompletarFrase;
        var exerciciosCriarFrases = descricao?.exerciciosCriarFrases ?? this.exerciciosCriarFrases;
        var exerciciosSpeaking = descricao?.exerciciosSpeaking ?? this.exerciciosSpeaking;
        var exerciciosAlternativas = descricao?.exerciciosAlternativas ?? this.exerciciosAlternativas;
        var exerciciosAssociacao = descricao?.exerciciosAssociacao ?? this.exerciciosAssociacao;

        var roteiro = new RoteiroNpcCheckpoint(this, visita, () => aguardandoChegadaPortao
            ? new[] { RoteiroNpcCheckpoint.ChaveSaudacao(cumprimento), "gate.ready" }
            : new[] { RoteiroNpcCheckpoint.ChaveSaudacao(cumprimento) });
        void Fala(int indice) => roteiro.Fala(dialogoAtual[indice], () => PlayAudioETexto(indice));
        void Exercicio(string ponto, int ordem, ExercicioBase fallback) =>
            roteiro.Exercicio(ponto, ordem, () => AbrirExercicioDoPonto(ponto, ordem, fallback), modelo: fallback);

        switch (nivelAtual)
        {
            case "A1":
                Fala(0);
                Fala(1);
                Exercicio("travel_documents", 1, exerciciosSpeaking[0]);
                Fala(2);
                Exercicio("gate_number", 1, exerciciosCompletarFrase[0]);
                Exercicio("checkin_vocabulary", 1, exerciciosAssociacao[0]);
                break;
            case "A2":
                roteiro.Acao("IniciarMissao.aeroporto_a2_2", () => { IniciarMissao("aeroporto_a2_2", "Tell the airport employee your destination", "Informe seu destino ao funcionário"); return Task.CompletedTask; });
                Fala(0);
                Exercicio("destination", 1, exerciciosCriarFrases[0]);
                roteiro.Acao("ConcluirMissao.aeroporto_a2_2", () => { ConcluirMissao("aeroporto_a2_2"); return Task.CompletedTask; });
                roteiro.Acao("IniciarMissao.aeroporto_a2_3", () => { IniciarMissao("aeroporto_a2_3", "Present your passport and booking confirmation", "Apresente seu passaporte e confirmação da reserva"); return Task.CompletedTask; });
                Fala(1);
                Exercicio("travel_documents", 1, exerciciosSpeaking[0]);
                roteiro.Acao("ConcluirMissao.aeroporto_a2_3", () => { ConcluirMissao("aeroporto_a2_3"); return Task.CompletedTask; });
                roteiro.Acao("IniciarMissao.aeroporto_a2_4", () => { IniciarMissao("aeroporto_a2_4", "Tell the employee about the luggage you need to check in", "Informe sua bagagem para despacho"); return Task.CompletedTask; });
                Fala(2);
                Exercicio("checked_baggage", 1, exerciciosSpeaking[1]);
                roteiro.Acao("ConcluirMissao.aeroporto_a2_4", () => { ConcluirMissao("aeroporto_a2_4"); return Task.CompletedTask; });
                roteiro.Acao("IniciarMissao.aeroporto_a2_5", () => { IniciarMissao("aeroporto_a2_5", "Find out the boarding time", "Descubra o horário de embarque"); return Task.CompletedTask; });
                Fala(3);
                Exercicio("boarding_time", 1, exerciciosAlternativas[0]);
                roteiro.Acao("ConcluirMissao.aeroporto_a2_5", () => { ConcluirMissao("aeroporto_a2_5"); return Task.CompletedTask; });
                Exercicio("gate_number", 1, exerciciosCompletarFrase[0]);
                roteiro.Acao("EmitirEventoMissao.marco_fluxo.portao_liberado", () => { EmitirEventoMissao("marco_fluxo", "portao_liberado"); return Task.CompletedTask; });
                roteiro.Acao("IniciarMissaoChegadaPortao.aeroporto_a2_6", () => { IniciarMissaoChegadaPortao("aeroporto_a2_6", "Find gate 18", "Encontre o portão 18", pontoChegadaPortao18, marcadorPortao18); return Task.CompletedTask; });
                Exercicio("checkin_vocabulary", 1, exerciciosAssociacao[0]);
                break;
            case "B1":
                Fala(0);
                roteiro.Acao("IniciarMissao.aeroporto_b1_4", () => { IniciarMissao("aeroporto_b1_4", "Provide your destination and connecting-flight details", "Informe seu destino e os detalhes do voo de conexão"); return Task.CompletedTask; });
                Exercicio("destination", 1, exerciciosSpeaking[0]);
                roteiro.Acao("ConcluirMissao.aeroporto_b1_4", () => { ConcluirMissao("aeroporto_b1_4"); return Task.CompletedTask; });
                roteiro.Acao("IniciarMissao.aeroporto_b1_5", () => { IniciarMissao("aeroporto_b1_5", "Present your travel documents", "Apresente seus documentos de viagem"); return Task.CompletedTask; });
                Fala(1);
                Exercicio("travel_documents", 1, exerciciosSpeaking[1]);
                roteiro.Acao("ConcluirMissao.aeroporto_b1_5", () => { ConcluirMissao("aeroporto_b1_5"); return Task.CompletedTask; });
                roteiro.Acao("IniciarMissao.aeroporto_b1_6", () => { IniciarMissao("aeroporto_b1_6", "Confirm the checked-baggage information", "Confirme as informações sobre a bagagem despachada"); return Task.CompletedTask; });
                Fala(2);
                Fala(3);
                Exercicio("baggage_transfer", 1, exerciciosAlternativas[0]);
                roteiro.Acao("ConcluirMissao.aeroporto_b1_6", () => { ConcluirMissao("aeroporto_b1_6"); return Task.CompletedTask; });
                Fala(4);
                Exercicio("departure_screens", 1, exerciciosSpeaking[2]);
                roteiro.Acao("EmitirEventoMissao.marco_fluxo.portao_liberado", () => { EmitirEventoMissao("marco_fluxo", "portao_liberado"); return Task.CompletedTask; });
                roteiro.Acao("IniciarMissaoChegadaPortao.aeroporto_b1_8", () => { IniciarMissaoChegadaPortao("aeroporto_b1_8", "Find gate 24 for your flight", "Encontre o portão 24 para seu voo", pontoChegadaPortao24, marcadorPortao24); return Task.CompletedTask; });
                Exercicio("checkin_vocabulary", 1, exerciciosAssociacao[0]);
                break;
            case "A1-A2":
                Fala(0);
                Exercicio("destination", 1, exerciciosCriarFrases[0]);
                Fala(1);
                roteiro.Acao("EmitirEventoMissao.marco_fluxo.documentos_solicitados", () => { EmitirEventoMissao("marco_fluxo", "documentos_solicitados"); return Task.CompletedTask; });
                roteiro.Acao("IniciarMissao.aeroporto_a1a2_4", () => { IniciarMissao( "aeroporto_a1a2_4", "Present your passport and booking confirmation", "Apresente seu passaporte e confirmação da reserva" ); return Task.CompletedTask; });
                Exercicio("travel_documents", 1, exerciciosSpeaking[0]);
                roteiro.Acao("ConcluirMissao.aeroporto_a1a2_4", () => { ConcluirMissao("aeroporto_a1a2_4"); return Task.CompletedTask; });
                roteiro.Acao("IniciarMissao.aeroporto_a1a2_5", () => { IniciarMissao( "aeroporto_a1a2_5", "Check your boarding pass and find gate 18", "Consulte seu cartão de embarque e encontre o portão 18" ); return Task.CompletedTask; });
                Fala(2);
                Exercicio("gate_number", 1, exerciciosCompletarFrase[0]);
                Exercicio("checkin_vocabulary", 1, exerciciosAssociacao[0]);
                roteiro.Acao("AguardarChegadaAoPortao.aeroporto_a1a2_5", () => { AguardarChegadaAoPortao("aeroporto_a1a2_5", pontoChegadaPortao18, marcadorPortao18); return Task.CompletedTask; });
                break;
            case "A2-B1":
                roteiro.Acao("EmitirEventoMissao.npc_interagido.checkin_npc", () => { EmitirEventoMissao("npc_interagido", "checkin_npc"); return Task.CompletedTask; });
                roteiro.Acao("IniciarMissao.aeroporto_a2b1_destino", () => { IniciarMissao( "aeroporto_a2b1_destino", "Provide your destination and connecting-flight details", "Informe seu destino e os detalhes do voo de conexão" ); return Task.CompletedTask; });
                Fala(0);
                Exercicio("destination", 1, exerciciosSpeaking[0]);
                roteiro.Acao("ConcluirMissao.aeroporto_a2b1_destino", () => { ConcluirMissao("aeroporto_a2b1_destino"); return Task.CompletedTask; });
                roteiro.Acao("IniciarMissao.aeroporto_a2b1_4", () => { IniciarMissao( "aeroporto_a2b1_4", "Present your travel documents", "Apresente seus documentos de viagem" ); return Task.CompletedTask; });
                Fala(1);
                Exercicio("travel_documents", 1, exerciciosSpeaking[1]);
                roteiro.Acao("ConcluirMissao.aeroporto_a2b1_4", () => { ConcluirMissao("aeroporto_a2b1_4"); return Task.CompletedTask; });
                roteiro.Acao("IniciarMissao.aeroporto_a2b1_5", () => { IniciarMissao( "aeroporto_a2b1_5", "Confirm the information about your checked baggage", "Confirme as informações sobre sua bagagem despachada" ); return Task.CompletedTask; });
                Fala(2);
                Exercicio("baggage_transfer", 1, exerciciosAlternativas[0]);
                roteiro.Acao("ConcluirMissao.aeroporto_a2b1_5", () => { ConcluirMissao("aeroporto_a2b1_5"); return Task.CompletedTask; });
                roteiro.Acao("IniciarMissao.aeroporto_a2b1_6", () => { IniciarMissao( "aeroporto_a2b1_6", "Check the boarding information and connecting gate", "Confira as informações sobre o embarque e o portão de conexão" ); return Task.CompletedTask; });
                Fala(3);
                Exercicio("departure_screens", 1, exerciciosSpeaking[2]);
                Exercicio("checkin_vocabulary", 1, exerciciosAssociacao[0]);
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

            Debug.LogWarning($"checkinNPC: usando exerc?cio local em {nivelAtual}/{pointKey}/{ordem}.");
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
