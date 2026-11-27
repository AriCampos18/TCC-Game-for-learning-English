// using System.Collections.Generic;
// using System.Threading.Tasks;
// using UnityEngine;
// using UnityEngine.Networking;
// using System.IO;
// using TMPro;
// using System;
// using System.Globalization;

// public class RecepcionistHotelNPC: InteracaoNPC
// {
//     string nivelAtual;
//     string cumprimento, periodo, periodoIng;
//     int horaAtual = DateTime.Now.Hour;
//     List<string> dialogoAtual;
//     private AudioSource audioSource;
//     private BackendManager backendManager;
//     public ModalExercicio modalExercicio;
//     public GameObject modalLegenda;
//     public TextMeshProUGUI textoPularDialogo;
//     public ExercicioBase exAtual;

//     public string idNpcParaVoz = "recepcionistHotel";

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
//         nomeExibicaoLegenda = "Hotel Recepcionist";
//         exerciciosCompletarFrase = GarantirListaExercicios(exerciciosCompletarFrase);
//         exerciciosCriarFrases = GarantirListaExercicios(exerciciosCriarFrases);
//         exerciciosSpeaking = GarantirListaExercicios(exerciciosSpeaking);
//         exerciciosAlternativas = GarantirListaExercicios(exerciciosAlternativas);
//         exerciciosWriting = GarantirListaExercicios(exerciciosWriting);
//         exerciciosAssociacao = GarantirListaExercicios(exerciciosAssociacao);

//         if (horaAtual < 12)
//         {
//             cumprimento = "Good morning!";
//             periodo = "de manhã";
//             periodoIng = "this morning";

//         }
//         else if (horaAtual < 18)
//         {
//             cumprimento = "Good afternoon!";
//             periodo = "a tarde";
//             periodoIng = "this afternoon";
//         }
//         else
//         {
//             cumprimento = "Good evening!";
//             periodo = "a noite";
//             periodoIng = "tonight";
//         }
//     }

//     private void InicializarConteudosPorNivel()
//     {
//         // Evita duplicar exercícios caso o jogador interaja com o NPC novamente.
//         exerciciosCompletarFrase.Clear();
//         exerciciosCriarFrases.Clear();
//         exerciciosSpeaking.Clear();
//         exerciciosAlternativas.Clear();
//         exerciciosWriting.Clear();
//         exerciciosAssociacao.Clear();

//         string dataCheckout = ObterDataCheckout();
//         if (nivelAtual == "A1")
//         {
//             dialogoAtual = new List<string>()
//             {
//                 "Hello. Can I help you?",
//                 "What is yout name?",
//                 "Would you like to pay by cash or card?",
//                 "Your room is 103. It is on the first floor",
//                 "Breakfast is from 7:00 a.m. to 9:00 a.m.",
//                 "Checkout is on "+dataCheckout+".",
//                 "Call me if you have a question."
//             };

//             exerciciosSpeaking.Add(
//                 new ExercicioSpeaking()
//                 {
//                     enunciado = "You are at the hotel. Say the correct sentence",
//                     opcoesFala = new List<string>()
//                     {
//                         "I have a reservation\n",
//                         "I am the breakfast yesterday\n",
//                         "Where are the classroom do?\n"
//                     },
//                     respostaCorreta = 0
//                 }
//             );

//             exerciciosWriting.Add(
//                 new ExercicioWriting()
//                 {
//                     enunciado = "What is your name? Write your answer."
//                 }
//             );

//             exerciciosAlternativas.Add(
//                 new ExercicioAlternativas()
//                 {
//                     enunciado = "The receptionist asked how you would like to pay. Choose the correct answer:",
//                     alternativas = new List<string>() { 
//                         "I would like to pay by room\r\n", 
//                         "I would like to pay by breakfast\r\n", 
//                         "I did pay by card\r\n", 
//                         "I would like to pay by card\r\n" },
//                     alternativaCorreta = 3
//                 }
//             );

//             exerciciosCriarFrases.Add(
//                 new ExercicioBlocos()
//                 {
//                     enunciado= "Arrange the words to form the correct sentence in english: Qual o horário do café da manhã?",
//                     blocosPalavras = new List<string>
//                     {
//                         "dinner",
//                         "served",
//                         "What",
//                         "Which",
//                         "breakfast",
//                         "when",
//                         "time",
//                         "is",
//                         "the",
//                         "lunch",
//                         "are"
//                     },
//                     respostaCorreta = "What time is breakfast served?\n"
//                 }
//             );

//                 exerciciosSpeaking.Add(
//                     new ExercicioSpeaking()
//                     {
//                         enunciado = "Speak the corret answaer to ask about the checkout",
//                         opcoesFala = new List<string>()
//                         {
//                             "Where is my breakfast yesterday?\n",
//                             "When is my checkout?\n",
//                             "When my checkout are?\n"
//                         },
//                         respostaCorreta = 1
//                     }
//                 );   

//                 exerciciosAlternativas.Add(
//                     new ExercicioAlternativas()
//                     {
//                         enunciado = "How long will you be staying at the hotel?",
//                         alternativas = new List<string>() { 
//                         "Two days\r\n", 
//                         "One week\r\n", 
//                         "Five days\r\n", 
//                         "Two weeks\r\n" },
//                         alternativaCorreta = 1
//                     }
//                 );     

//                 exerciciosSpeaking.Add(
//                     new ExercicioSpeaking()
//                     {
//                         enunciado = "Thank the receptionist",
//                         opcoesFala = new List<string>() { "Thank you"+"\n" },
//                         respostaCorreta = 0
//                     }
//                 );    

//                 exerciciosAssociacao.Add(
//                     new ExercicioAssociacao()
//                     {
//                         enunciado = "Match the Portuguese word with the English word.",
//                         opcoesPort = new List<string>
//                         {
//                             "reserva",
//                             "nome",
//                             "cartão",
//                             "quarto",
//                             "andar",
//                             "café da manhã",
//                             "saída",
//                             "semana"
//                         },
//                         opcoesIngles = new List<string>
//                         {
//                             "room",
//                             "week",
//                             "name",
//                             "card",
//                             "checkout",
//                             "reservation",
//                             "floor",
//                             "breakfast",
//                         },
//                         associacaoCorreta = new List<ParAssociacao>
//                         {
//                         new ParAssociacao { palavraPortugues = "reserva", palavraIngles = "reservation" },
//                         new ParAssociacao { palavraPortugues = "nome", palavraIngles = "name" },
//                         new ParAssociacao { palavraPortugues = "cartão", palavraIngles = "card" },
//                         new ParAssociacao { palavraPortugues = "quarto", palavraIngles = "room" },
//                         new ParAssociacao { palavraPortugues = "andar", palavraIngles = "floor" },
//                         new ParAssociacao { palavraPortugues = "café da manhã", palavraIngles = "breakfast" },
//                         new ParAssociacao { palavraPortugues = "saída", palavraIngles = "checkout" },
//                         new ParAssociacao { palavraPortugues = "semana", palavraIngles = "week" }
//                         }
//                     }
//                 );
//         }
//         else if(nivelAtual=="A2")
//         {
//             dialogoAtual = new List<string>()
//             {
//                 cumprimento+". Welcome to the hotel. Do you have a reservation?",
//                 "Great. Could I have your name, please?",
//                 "Thank you. How many nights are you staying with us?",
//                 "Would you like to pay by card or cash?",
//                 "Your room is 205, on the second floor. Here is your key card.",
//                 "Breakfast is from 7:00 a.m. to 10:00 a.m. in the restaurant. It's on the first floor.",
//                 "Is there anything else you would like to know?",
//                 "Checkout is at 12:00 p.m. on " + dataCheckout + ".",
//                 "Is there anything else I can help you with?",
//                 "Enjoy your stay!"
//             };

//             exerciciosCriarFrases.Add(
//                 new ExercicioBlocos()
//                 {
//                     enunciado= "Arrange the words to form the correct sentence in english: Eu tenho uma reserva para hoje "+periodo,
//                     blocosPalavras = new List<string>
//                     {
//                         "room",
//                         "served",
//                         "have",
//                         "Which",
//                         "tonight",
//                         "afternoon",
//                         "morning",
//                         "when",
//                         "reservation",
//                         "is",
//                         "for",
//                         "lunch",
//                         "I",
//                         "this"
//                     },
//                     respostaCorreta = "I have a reservation for "+periodoIng+"\n"
//                 }
//             );

//             exerciciosWriting.Add(
//                 new ExercicioWriting()
//                 {
//                     enunciado = "What is your name? Write your answer."
//                 }
//             );

//             exerciciosSpeaking.Add(
//                     new ExercicioSpeaking()
//                     {
//                         enunciado = "How long will you be staying at the hotel?",
//                         opcoesFala = new List<string>()
//                         {
//                             "I staying one week ago\n",
//                             "I'll be paying on the fifth floor\n",
//                             "I'll be staying for one week\n"
//                         },
//                         respostaCorreta = 2
//                     }
//             ); 

//             exerciciosAlternativas.Add(
//                 new ExercicioAlternativas()
//                 {
//                     enunciado = "The receptionist asked how you would like to pay. Choose the correct answer:",
//                     alternativas = new List<string>() { 
//                         "I'd like to pay on the fifth floor\r\n", 
//                         "I'd like to pay by card\r\n", 
//                         "I'd like to pay at seven o'clock\r\n", 
//                         "I'd like to pay with my room\r\n" },
//                     alternativaCorreta = 1
//                 }
//             );

//             exerciciosWriting.Add(
//                 new ExercicioWriting()
//                 {
//                     enunciado = "You want to know where you can have breakfast and what time it is. Ask these informations for the receptionist"
//                 }
//             );

//             exerciciosAlternativas.Add(
//                 new ExercicioAlternativas()
//                 {
//                     enunciado = "Where can you have breakfast?",
//                     alternativas = new List<string>() { 
//                         "At the reception desk\r\n", 
//                         "On the second floor\r\n", 
//                         "In room 205\r\n", 
//                         "On the first floor\r\n" },
//                     alternativaCorreta = 3
//                 }
//             );

//             exerciciosSpeaking.Add(
//                     new ExercicioSpeaking()
//                     {
//                         enunciado = "You want to know the checkout time. What do you ask the receptionist?",
//                         opcoesFala = new List<string>()
//                         {
//                             "What time is the checkout?\n",
//                             "Where can I leave my luggage?\n",
//                             "Does checkout include breakfast?\n"
//                         },
//                         respostaCorreta = 0
//                     }
//             ); 

//             exerciciosCompletarFrase.Add(
//                 new ExercicioCompletarFrase()
//                 {
//                     enunciado = "Complete the sentence with the correct word:",
//                     fraseAntes = "I need to leave at ",
//                     fraseDepois = " p.m.",
//                     respostaCorreta = "12:00",
//                     opcoes = new List<string>
//                     {
//                         "10:00",
//                         "served",
//                         "have",
//                         "Which",
//                         "tonight",
//                         "11:00",
//                         "morning",
//                         "12:00",
//                         "is",
//                         "for",
//                         "20:00",
//                         "I",
//                         "1:00"
//                     }
//                 }
//             );

//             exerciciosSpeaking.Add(
//                     new ExercicioSpeaking()
//                     {
//                         enunciado = "You don’t need anything else. What do you say?",
//                         opcoesFala = new List<string>()
//                         {
//                             "No, I am in the first floor\n",
//                             "No, thank you. That's all\n",
//                             "Thank you, I want the breakfast in\n"
//                         },
//                         respostaCorreta = 1
//                     }
//             ); 

//             exerciciosAssociacao.Add(
//                     new ExercicioAssociacao()
//                     {
//                         enunciado = "Match the Portuguese word with the English word.",
//                         opcoesPort = new List<string>
//                         {
//                             "noite",
//                             "hospedagem",
//                             "pagar",
//                             "cartão do quarto",
//                             "perguntar",
//                             "sair",
//                             "bagagem",
//                             "recepção"
//                         },
//                         opcoesIngles = new List<string>
//                         {
//                             "leave",
//                             "pay",
//                             "night",
//                             "luggage",
//                             "reception desk",
//                             "stay",
//                             "room card",
//                             "ask"
//                         },
//                         associacaoCorreta = new List<ParAssociacao>
//                         {
//                         new ParAssociacao { palavraPortugues = "noite", palavraIngles = "night" },
//                         new ParAssociacao { palavraPortugues = "hospedagem", palavraIngles = "stay" },
//                         new ParAssociacao { palavraPortugues = "pagar", palavraIngles = "pay" },
//                         new ParAssociacao { palavraPortugues = "cartão do quarto", palavraIngles = "room card" },
//                         new ParAssociacao { palavraPortugues = "perguntar", palavraIngles = "ask" },
//                         new ParAssociacao { palavraPortugues = "bagagem", palavraIngles = "luggage" },
//                         new ParAssociacao { palavraPortugues = "sair", palavraIngles = "leave" },
//                         new ParAssociacao { palavraPortugues = "recepção", palavraIngles = "reception desk" }
//                         }
//                     }
//             );
//         }
//         else if(nivelAtual=="B1")
//         {
//             dialogoAtual = new List<string>()
//             {
//                 cumprimento+". Welcome to the hotel. How can I help you today?",
//                 "Of course. Could you tell me the name the reservation is under?",
//                 "I found it. You booked a double room for seven nights, with breakfast included. Is that right?",
//                 "Perfect. Would you like to pay now by card or cash, or when you check out?",
//                 "Your room is 308, on the third floor. Here is your key card. The Wi-Fi is free, and the password is on the card.",
//                 "The elevator is just around the corner, next to the restaurant.",
//                 "Breakfast is served from 6:30 a.m. to 10:00 a.m. If you prefer, we can also bring it to your room.",
//                 "Is there anything else you'd like to know about your stay?",
//                 "Checkout is at 12:00 p.m. on " + dataCheckout + ". If you need a late checkout, please let us know in advance.",
//                 "We can offer a late checkout until 2:00 p.m., depending on availability.",
//                 "I hope you enjoy your stay. Just call the front desk if you need anything."
//             };

//             exerciciosSpeaking.Add(
//                     new ExercicioSpeaking()
//                     {
//                         enunciado = "Tell her you have a reservation, and say how many people it is for.",
//                         opcoesFala = new List<string>()
//                         {
//                             "I have a reservation for one person\n",
//                             "I have a breakfast for two people\n",
//                             "I reservation for one person\n"
//                         },
//                         respostaCorreta = 0
//                     }
//             ); 

//             exerciciosWriting.Add(
//                 new ExercicioWriting()
//                 {
//                     enunciado = "The receptionist asks for the name on the reservation. Write your answer in a complete sentence using -under the name-."
//                 }
//             );

//             exerciciosSpeaking.Add(
//                     new ExercicioSpeaking()
//                     {
//                         enunciado = "Confirm the question made by the recepcionist",
//                         opcoesFala = new List<string>()
//                         {
//                             "Yes, that's correct\n",
//                             "No, I am a double room\n",
//                             "Yes, that are correct\n"
//                         },
//                         respostaCorreta = 0
//                     }
//             ); 

//             exerciciosAlternativas.Add(
//                 new ExercicioAlternativas()
//                 {
//                     enunciado = "Select the correct answer to say how would you like to pay",
//                     alternativas = new List<string>() { 
//                         "I'd like to pay at breakfast\r\n", 
//                         "I'd like to pay on the second floor\r\n", 
//                         "I'd like to pay by card, please\r\n", 
//                         "I'd like to pay for seven nights ago\r\n" },
//                     alternativaCorreta = 2
//                 }
//             );

//             exerciciosCriarFrases.Add(
//                 new ExercicioBlocos()
//                 {
//                     enunciado= "Arrange the words to form the correct sentence in english: Onde fica o elevador?"+periodo,
//                     blocosPalavras = new List<string>
//                     {
//                         "Where",
//                         "served",
//                         "elevator",
//                         "Which",
//                         "how",
//                         "elevator",
//                         "perfect",
//                         "when",
//                         "Who",
//                         "is",
//                         "for",
//                         "lunch",
//                         "the",
//                         "are"
//                     },
//                     respostaCorreta = "Where is the elevator? \n"
//                 }
//             );

//             exerciciosSpeaking.Add(
//                     new ExercicioSpeaking()
//                     {
//                         enunciado = "Speak the correct answer to ask about the place and time of the breakfast",
//                         opcoesFala = new List<string>()
//                         {
//                             "Where breakfast are the time?\n",
//                             "What time is breakfast and where is it?\n",
//                             "What time is breakfast yesterday?\n"
//                         },
//                         respostaCorreta = 1
//                     }
//             );

//             exerciciosAlternativas.Add(
//                 new ExercicioAlternativas()
//                 {
//                     enunciado = "What time does the breakfast end?",
//                     alternativas = new List<string>(){ 
//                         "6:30 p.m\r\n", 
//                         "9:00 a.m\r\n", 
//                         "10:00 a.m\r\n", 
//                         "12:00 p.m\r\n" },
//                     alternativaCorreta = 3
//                 }
//             );

//             exerciciosWriting.Add(
//                 new ExercicioWriting()
//                 {
//                     enunciado = "Write a sentece asking about the time of the checkout."
//                 }
//             );

//             exerciciosSpeaking.Add(
//                     new ExercicioSpeaking()
//                     {
//                         enunciado = "The receptionist says you can ask for a late checkout. Your flight is in the evening and you need more time in the room. What do you say?",
//                         opcoesFala = new List<string>()
//                         {
//                             "I am the menu yesterday\n",
//                             "Yes, please. Could you help me?\n",
//                             "The classroom is amazing\n"
//                         },
//                         respostaCorreta = 1
//                     }
//             );

//             exerciciosSpeaking.Add(
//                     new ExercicioSpeaking()
//                     {
//                         enunciado = "You have a flight at 5:00 p.m. Ask the recepcionist if you can checkout later. ",
//                     }
//             );

//             exerciciosCompletarFrase.Add(
//                 new ExercicioCompletarFrase()
//                 {
//                     enunciado = "Complete the sentence with the correct word:",
//                     fraseAntes = "The late checkout is available until ",
//                     fraseDepois = " p.m.",
//                     respostaCorreta = "2:00",
//                     opcoes = new List<string>
//                     {
//                         "10:00",
//                         "served",
//                         "2:00",
//                         "Which",
//                         "tonight",
//                         "1:00",
//                         "morning",
//                         "12:00",                        "5:00",
//                         "I",
//                     }
//                 }
//             );

//             exerciciosSpeaking.Add(
//                     new ExercicioSpeaking()
//                     {
//                         enunciado = "Choose the correct sentence to say you don't need anything else.",
//                         opcoesFala = new List<string>()
//                         {
//                             "No, I need the room 308\n",
//                             "Thank you, all I need are today\n",
//                             "No, thank you. That's all I need\n"
//                         },
//                         respostaCorreta = 2
//                     }
//             );

//             exerciciosAssociacao.Add(
//                     new ExercicioAssociacao()
//                     {
//                         enunciado = "Match the Portuguese word with the English word.",
//                         opcoesPort = new List<string>
//                         {
//                             "reservar",
//                             "quarto de casal",
//                             "ao lado de",
//                             "servido",
//                             "com antecedência",
//                             "voo",
//                             "sair",
//                             "disponibilidade",
//                         },
//                         opcoesIngles = new List<string>
//                         {
//                             "double room",
//                             "next to",
//                             "availability",
//                             "served",
//                             "book",
//                             "in advance",
//                             "flight",
//                             "leave",
//                         },
//                         associacaoCorreta = new List<ParAssociacao>
//                         {
//                         new ParAssociacao { palavraPortugues = "reservar", palavraIngles = "book" },
//                         new ParAssociacao { palavraPortugues = "quarto de casal", palavraIngles = "double room" },
//                         new ParAssociacao { palavraPortugues = "ao lado de", palavraIngles = "next to" },
//                         new ParAssociacao { palavraPortugues = "servido", palavraIngles = "served" },
//                         new ParAssociacao { palavraPortugues = "com antecedência", palavraIngles = "in advance" },
//                         new ParAssociacao { palavraPortugues = "voo", palavraIngles = "flight" },
//                         new ParAssociacao { palavraPortugues = "sair", palavraIngles = "leave" },
//                         new ParAssociacao { palavraPortugues = "disponibilidade", palavraIngles = "availability" }
//                         }
//                     }
//             );
//         }
//     }

//     private string ObterDataCheckout()
//     {
//         DateTime dataCheckout = DateTime.Now.Date.AddDays(7);

//         int dia = dataCheckout.Day;
//         string sufixo;

//         if (dia % 100 >= 11 && dia % 100 <= 13)
//         {
//             sufixo = "th";
//         }
//         else
//         {
//             switch (dia % 10)
//             {
//                 case 1:
//                     sufixo = "st";
//                     break;
//                 case 2:
//                     sufixo = "nd";
//                     break;
//                 case 3:
//                     sufixo = "rd";
//                     break;
//                 default:
//                     sufixo = "th";
//                     break;
//             }
//         }

//         string mes = dataCheckout.ToString(
//             "MMMM",
//             CultureInfo.GetCultureInfo("en-US")
//         );

//         return $"{mes} {dia}{sufixo}";
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

//     protected override async Task IniciarInteracao()
//     {
//         GameProgress.EstaEmDialogo = true;
//         nivelAtual = DadosJogador.nivelUsuario;
//         InicializarConteudosPorNivel();

//         if (nivelAtual == "A1")
//         {
//             IniciarMissao(
//                 "hotel_a1_conversar",
//                 "Fale com o recepcionista do hotel",
//                 "Talk to the hotel receptionist"
//             );

//             await interacaoA1();
//         }
//         else if (nivelAtual == "A2")
//         {
//             IniciarMissao(
//                 "hotel_a2_checkin",
//                 "Fale com o recepcionista do hotel e realize seu check-in",
//                 "Talk to the hotel receptionist and check in"
//             );

//             await interacaoA2();
//         }
//         else
//         {
//             IniciarMissao(
//                 "hotel_b1_checkin",
//                 "Informe seus dados e confirme as informações da reserva",
//                 "Provide your details and confirm your reservation information"
//             );

//             await interacaoB1();
//         }

//         GameProgress.EstaEmDialogo = false;

//         if (modalLegenda != null)
//             modalLegenda.SetActive(false);

//         if (textoPularDialogo != null)
//             textoPularDialogo.gameObject.SetActive(false);

//         GameProgress.EstaEmDialogo = false;
//         LiberarControlePlayer();

//         if (RevisaoManager.Instance != null)
//             await RevisaoManager.Instance.ExecutarRevisaoEExibirModalFinal(this);
//         else
//             Debug.LogError("RevisaoManager não está disponível para finalizar Hotel.");
//     }

//     private async Task interacaoA1()
//     {
//         int i = 0;

//         // Conversa inicial e identificação do hóspede.
//         await PlayAudioETexto(i++);
//         exAtual = exerciciosSpeaking[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         await PlayAudioETexto(i++);
//         exAtual = exerciciosWriting[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         // Pagamento: finaliza a missão de conversar com a recepção.
//         await PlayAudioETexto(i++);
//         exAtual = exerciciosAlternativas[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         MissionManager.ConcluirMissaoNPC("hotel_a1_conversar");

//         IniciarMissao(
//             "hotel_a1_servicos",
//             "Conheça os serviços do hotel durante a conversa",
//             "Learn about the hotel's services during the conversation"
//         );

//         // Informações do quarto, café da manhã e checkout.
//         await PlayAudioETexto(i++);
//         await AbrirExercicio(
//             ObterTipoExercicio(exerciciosCriarFrases[0]),
//             exerciciosCriarFrases[0]
//         );

//         await PlayAudioETexto(i++);
//         exAtual = exerciciosSpeaking[1];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         await PlayAudioETexto(i++);
//         exAtual = exerciciosAlternativas[1];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         // A recepcionista encerra oferecendo ajuda; o usuário agradece em seguida.
//         await PlayAudioETexto(i++);
//         exAtual = exerciciosSpeaking[2];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         MissionManager.ConcluirMissaoNPC("hotel_a1_servicos");

//         await AbrirExercicio(
//             ObterTipoExercicio(exerciciosAssociacao[0]),
//             exerciciosAssociacao[0]
//         );
//     }

//     private async Task interacaoA2()
//     {
//         int i = 0;

//         await PlayAudioETexto(i++);
//         exAtual = exerciciosCriarFrases[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         await PlayAudioETexto(i++);
//         exAtual = exerciciosWriting[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         await PlayAudioETexto(i++);
//         await AbrirExercicio(
//             ObterTipoExercicio(exerciciosSpeaking[0]),
//             exerciciosSpeaking[0]
//         );

//         await PlayAudioETexto(i++);
//         exAtual = exerciciosAlternativas[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         // Check-in concluído após a escolha da forma de pagamento.
//         MissionManager.ConcluirMissaoNPC("hotel_a2_checkin");

//         IniciarMissao(
//             "hotel_a2_cartao_quarto",
//             "Pegue o cartão do quarto",
//             "Collect your room card"
//         );

//         await PlayAudioETexto(i++);
//         MissionManager.ConcluirMissaoNPC("hotel_a2_cartao_quarto");

//         // Informações sobre café da manhã e checkout.
//         exAtual = exerciciosWriting[1];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         await PlayAudioETexto(i++);
//         await AbrirExercicio(
//             ObterTipoExercicio(exerciciosAlternativas[1]),
//             exerciciosAlternativas[1]
//         );

//         await PlayAudioETexto(i++);
//         await AbrirExercicio(
//             ObterTipoExercicio(exerciciosSpeaking[1]),
//             exerciciosSpeaking[1]
//         );

//         await PlayAudioETexto(i++);
//         await AbrirExercicio(
//             ObterTipoExercicio(exerciciosCompletarFrase[0]),
//             exerciciosCompletarFrase[0]
//         );

//         await PlayAudioETexto(i++);
//         await AbrirExercicio(
//             ObterTipoExercicio(exerciciosSpeaking[2]),
//             exerciciosSpeaking[2]
//         );

//         await PlayAudioETexto(i++);

//         await AbrirExercicio(
//             ObterTipoExercicio(exerciciosAssociacao[0]),
//             exerciciosAssociacao[0]
//         );
//     }

//     private async Task interacaoB1()
//     {
//         int i = 0;

//         // Reserva, identificação e confirmação dos dados.
//         await PlayAudioETexto(i++);
//         exAtual = exerciciosSpeaking[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         await PlayAudioETexto(i++);
//         await AbrirExercicio(
//             ObterTipoExercicio(exerciciosWriting[0]),
//             exerciciosWriting[0]
//         );

//         await PlayAudioETexto(i++);
//         exAtual = exerciciosSpeaking[1];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         await PlayAudioETexto(i++);
//         exAtual = exerciciosAlternativas[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         MissionManager.ConcluirMissaoNPC("hotel_b1_checkin");

//         IniciarMissao(
//             "hotel_b1_servicos",
//             "Conheça os serviços do hotel e obtenha informações sobre o quarto e o café da manhã",
//             "Learn about the hotel services and get information about your room and breakfast"
//         );

//         // Cartão do quarto, elevador e café da manhã.
//         await PlayAudioETexto(i++);
//         exAtual = exerciciosCriarFrases[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         await PlayAudioETexto(i++);
//         await AbrirExercicio(
//             ObterTipoExercicio(exerciciosSpeaking[2]),
//             exerciciosSpeaking[2]
//         );

//         await PlayAudioETexto(i++);
//         exAtual = exerciciosAlternativas[1];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         MissionManager.ConcluirMissaoNPC("hotel_b1_servicos");

//         IniciarMissao(
//             "hotel_b1_late_checkout",
//             "Consulte o horário do checkout e solicite um checkout tardio, se necessário",
//             "Check the checkout time and request a late checkout if needed"
//         );

//         // Checkout: primeiro pergunta o horário; depois solicita a saída tardia.
//         await PlayAudioETexto(i++);
//         await AbrirExercicio(
//             ObterTipoExercicio(exerciciosWriting[1]),
//             exerciciosWriting[1]
//         );

//         await PlayAudioETexto(i++);
//         await AbrirExercicio(
//             ObterTipoExercicio(exerciciosSpeaking[3]),
//             exerciciosSpeaking[3]
//         );

//         // O pedido livre vem antes da resposta da recepcionista sobre disponibilidade.
//         await AbrirExercicio(
//             ObterTipoExercicio(exerciciosSpeaking[4]),
//             exerciciosSpeaking[4]
//         );

//         await PlayAudioETexto(i++);
//         await AbrirExercicio(
//             ObterTipoExercicio(exerciciosCompletarFrase[0]),
//             exerciciosCompletarFrase[0]
//         );

//         await PlayAudioETexto(i++);

//         MissionManager.ConcluirMissaoNPC("hotel_b1_late_checkout");

//         await AbrirExercicio(
//             ObterTipoExercicio(exerciciosAssociacao[0]),
//             exerciciosAssociacao[0]
//         );
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
using System.Globalization;

public class RecepcionistHotelNPC: InteracaoNPC, IRoteiroNpcCheckpoint
{
    string nivelAtual;
    string cumprimento, periodo, periodoIng;
    [Header("Objeto do Hotel")]
    public GameObject cartaoQuarto;
    List<string> dialogoAtual;
    private const string NpcKey = MissoesHotel.Recepcao;
    private int ultimaVisitaHotel;
    private int VisitaAtual => MissionManager.Instance?.VisitaHotel ?? 0;
    protected override bool PermitirReinteracaoAposConclusao =>
        VisitaAtual != 0 && VisitaAtual != ultimaVisitaHotel;
    private const string PeriodoToken = "{{periodo}}";
    private const string PeriodoIngToken = "{{periodo_ing}}";
    private ExerciseLoadResult catalogoExercicios;
    public ModalExercicio modalExercicio;
    public GameObject modalLegenda;
    public TextMeshProUGUI textoPularDialogo;
    public ExercicioBase exAtual;

    public string idNpcParaVoz = "recepcionistHotel";

    [System.NonSerialized] public List<ExercicioBase> exerciciosCompletarFrase;
    [System.NonSerialized] public List<ExercicioBase> exerciciosCriarFrases;
    [System.NonSerialized] public List<ExercicioBase> exerciciosSpeaking;
    [System.NonSerialized] public List<ExercicioBase> exerciciosAlternativas;
    [System.NonSerialized] public List<ExercicioBase> exerciciosWriting;

    [System.NonSerialized] public List<ExercicioBase> exerciciosAssociacao;

    protected override void Start()
    {
        base.Start();
        if (audioSourceComum == null)
        {
            audioSourceComum = gameObject.AddComponent<AudioSource>();
            audioSourceComum.playOnAwake = false;
        }

        if (cartaoQuarto != null)
            cartaoQuarto.SetActive(false);

        nivelAtual = DadosJogador.nivelUsuario;
        nomeExibicaoLegenda = "Hotel Recepcionist";
        exerciciosCompletarFrase = GarantirListaExercicios(exerciciosCompletarFrase);
        exerciciosCriarFrases = GarantirListaExercicios(exerciciosCriarFrases);
        exerciciosSpeaking = GarantirListaExercicios(exerciciosSpeaking);
        exerciciosAlternativas = GarantirListaExercicios(exerciciosAlternativas);
        exerciciosWriting = GarantirListaExercicios(exerciciosWriting);
        exerciciosAssociacao = GarantirListaExercicios(exerciciosAssociacao);
        AtualizarPeriodoDoDia();
    }

    private void AtualizarPeriodoDoDia()
    {
        int horaAtual = DateTime.Now.Hour;
        if (horaAtual < 12)
        {
            cumprimento = "Good morning!";
            periodo = "de manhã";
            periodoIng = "this morning";

        }
        else if (horaAtual < 18)
        {
            cumprimento = "Good afternoon!";
            periodo = "a tarde";
            periodoIng = "this afternoon";
        }
        else
        {
            cumprimento = "Good evening!";
            periodo = "a noite";
            periodoIng = "tonight";
        }
    }

    private void InicializarConteudosPorNivel()
    {
        var conteudo = MontarConteudo(nivelAtual, cumprimento, periodo, periodoIng, ObterDataCheckout());
        dialogoAtual = conteudo.Falas;
        Preencher(exerciciosCompletarFrase, conteudo.exerciciosCompletarFrase);
        Preencher(exerciciosCriarFrases, conteudo.exerciciosCriarFrases);
        Preencher(exerciciosSpeaking, conteudo.exerciciosSpeaking);
        Preencher(exerciciosAlternativas, conteudo.exerciciosAlternativas);
        Preencher(exerciciosWriting, conteudo.exerciciosWriting);
        Preencher(exerciciosAssociacao, conteudo.exerciciosAssociacao);
    }

    private static void Preencher(List<ExercicioBase> destino, List<ExercicioBase> origem)
    {
        destino.Clear();
        destino.AddRange(origem);
    }

    private static ConteudoRoteiro MontarConteudo(string nivelAtual, string cumprimento, string periodo,
        string periodoIng, string dataCheckout)
    {
        var descricao = new ConteudoRoteiro();
        List<string> dialogoAtual = null;
        var exerciciosCompletarFrase = descricao.exerciciosCompletarFrase;
        var exerciciosCriarFrases = descricao.exerciciosCriarFrases;
        var exerciciosSpeaking = descricao.exerciciosSpeaking;
        var exerciciosAlternativas = descricao.exerciciosAlternativas;
        var exerciciosWriting = descricao.exerciciosWriting;
        var exerciciosAssociacao = descricao.exerciciosAssociacao;

        if (nivelAtual == "A1")
        {
            dialogoAtual = new List<string>()
            {
                "Hello. Can I help you?",
                "What is yout name?",
                "Would you like to pay by cash or card?",
                "Your room is 103. It is on the first floor",
                "Breakfast is from 7:00 a.m. to 9:00 a.m.",
                "Checkout is on "+dataCheckout+".",
                "Call me if you have a question."
            };

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "You are at the hotel. Say the correct sentence",
                    opcoesFala = new List<string>()
                    {
                        "I have a reservation\n",
                        "I am the breakfast yesterday\n",
                        "Where are the classroom do?\n"
                    },
                    respostaCorreta = 0
                }
            );

            exerciciosWriting.Add(
                new ExercicioWriting()
                {
                    enunciado = "What is your name? Write your answer."
                }
            );

            exerciciosAlternativas.Add(
                new ExercicioAlternativas()
                {
                    enunciado = "The receptionist asked how you would like to pay. Choose the correct answer:",
                    alternativas = new List<string>() { 
                        "I would like to pay by room\r\n", 
                        "I would like to pay by breakfast\r\n", 
                        "I did pay by card\r\n", 
                        "I would like to pay by card\r\n" },
                    alternativaCorreta = 3
                }
            );

            exerciciosCriarFrases.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Arrange the words to form the correct sentence in english: Qual o horário do café da manhã?",
                    blocosPalavras = new List<string>
                    {
                        "dinner",
                        "served",
                        "What",
                        "Which",
                        "breakfast",
                        "when",
                        "time",
                        "is",
                        "the",
                        "lunch",
                        "are"
                    },
                    respostaCorreta = "What time is breakfast served?\n"
                }
            );

                exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Speak the corret answaer to ask about the checkout",
                        opcoesFala = new List<string>()
                        {
                            "Where is my breakfast yesterday?\n",
                            "When is my checkout?\n",
                            "When my checkout are?\n"
                        },
                        respostaCorreta = 1
                    }
                );   

                exerciciosAlternativas.Add(
                    new ExercicioAlternativas()
                    {
                        enunciado = "How long will you be staying at the hotel?",
                        alternativas = new List<string>() { 
                        "Two days\r\n", 
                        "One week\r\n", 
                        "Five days\r\n", 
                        "Two weeks\r\n" },
                        alternativaCorreta = 1
                    }
                );     

                exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Thank the receptionist",
                        opcoesFala = new List<string>() { "Thank you"+"\n" },
                        respostaCorreta = 0
                    }
                );    

                exerciciosAssociacao.Add(
                    new ExercicioAssociacao()
                    {
                        enunciado = "Match the Portuguese word with the English word.",
                        opcoesPort = new List<string>
                        {
                            "reserva",
                            "nome",
                            "cartão",
                            "quarto",
                            "andar",
                            "café da manhã",
                            "saída",
                            "semana"
                        },
                        opcoesIngles = new List<string>
                        {
                            "room",
                            "week",
                            "name",
                            "card",
                            "checkout",
                            "reservation",
                            "floor",
                            "breakfast",
                        },
                        associacaoCorreta = new List<ParAssociacao>
                        {
                        new ParAssociacao { palavraPortugues = "reserva", palavraIngles = "reservation" },
                        new ParAssociacao { palavraPortugues = "nome", palavraIngles = "name" },
                        new ParAssociacao { palavraPortugues = "cartão", palavraIngles = "card" },
                        new ParAssociacao { palavraPortugues = "quarto", palavraIngles = "room" },
                        new ParAssociacao { palavraPortugues = "andar", palavraIngles = "floor" },
                        new ParAssociacao { palavraPortugues = "café da manhã", palavraIngles = "breakfast" },
                        new ParAssociacao { palavraPortugues = "saída", palavraIngles = "checkout" },
                        new ParAssociacao { palavraPortugues = "semana", palavraIngles = "week" }
                        }
                    }
                );
        }
        else if(nivelAtual=="A2")
        {
            dialogoAtual = new List<string>()
            {
                cumprimento+". Welcome to the hotel. Do you have a reservation?",
                "Great. Could I have your name, please?",
                "Thank you. How many nights are you staying with us?",
                "Would you like to pay by card or cash?",
                "Your room is 205, on the second floor. Here is your key card.",
                "Breakfast is from 7:00 a.m. to 10:00 a.m. in the restaurant. It's on the first floor.",
                "Is there anything else you would like to know?",
                "Checkout is at 12:00 p.m. on " + dataCheckout + ".",
                "Is there anything else I can help you with?",
                "Enjoy your stay!"
            };

            exerciciosCriarFrases.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Arrange the words to form the correct sentence in english: Eu tenho uma reserva para hoje "+periodo,
                    blocosPalavras = new List<string>
                    {
                        "room",
                        "served",
                        "have",
                        "Which",
                        "tonight",
                        "afternoon",
                        "morning",
                        "when",
                        "reservation",
                        "a",
                        "is",
                        "for",
                        "lunch",
                        "I",
                        "this"
                    },
                    respostaCorreta = "I have a reservation for "+periodoIng+"\n"
                }
            );

            exerciciosWriting.Add(
                new ExercicioWriting()
                {
                    enunciado = "What is your name? Write your answer."
                }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "How long will you be staying at the hotel?",
                        opcoesFala = new List<string>()
                        {
                            "I staying one week ago\n",
                            "I'll be paying on the fifth floor\n",
                            "I'll be staying for one week\n"
                        },
                        respostaCorreta = 2
                    }
            ); 

            exerciciosAlternativas.Add(
                new ExercicioAlternativas()
                {
                    enunciado = "The receptionist asked how you would like to pay. Choose the correct answer:",
                    alternativas = new List<string>() { 
                        "I'd like to pay on the fifth floor\r\n", 
                        "I'd like to pay by card\r\n", 
                        "I'd like to pay at seven o'clock\r\n", 
                        "I'd like to pay with my room\r\n" },
                    alternativaCorreta = 1
                }
            );

            exerciciosWriting.Add(
                new ExercicioWriting()
                {
                    enunciado = "You want to know where you can have breakfast and what time it is. Ask these informations for the receptionist"
                }
            );

            exerciciosAlternativas.Add(
                new ExercicioAlternativas()
                {
                    enunciado = "Where can you have breakfast?",
                    alternativas = new List<string>() { 
                        "At the reception desk\r\n", 
                        "On the second floor\r\n", 
                        "In room 205\r\n", 
                        "On the first floor\r\n" },
                    alternativaCorreta = 3
                }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "You want to know the checkout time. What do you ask the receptionist?",
                        opcoesFala = new List<string>()
                        {
                            "What time is the checkout?\n",
                            "Where can I leave my luggage?\n",
                            "Does checkout include breakfast?\n"
                        },
                        respostaCorreta = 0
                    }
            ); 

            exerciciosCompletarFrase.Add(
                new ExercicioCompletarFrase()
                {
                    enunciado = "Complete the sentence with the correct word:",
                    fraseAntes = "I need to leave at ",
                    fraseDepois = " p.m.",
                    respostaCorreta = "12:00",
                    opcoes = new List<string>
                    {
                        "10:00",
                        "served",
                        "have",
                        "Which",
                        "tonight",
                        "11:00",
                        "morning",
                        "12:00",
                        "is",
                        "for",
                        "20:00",
                        "I",
                        "1:00"
                    }
                }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "You don’t need anything else. What do you say?",
                        opcoesFala = new List<string>()
                        {
                            "No, I am in the first floor\n",
                            "No, thank you. That's all\n",
                            "Thank you, I want the breakfast in\n"
                        },
                        respostaCorreta = 1
                    }
            ); 

            exerciciosAssociacao.Add(
                    new ExercicioAssociacao()
                    {
                        enunciado = "Match the Portuguese word with the English word.",
                        opcoesPort = new List<string>
                        {
                            "noite",
                            "hospedagem",
                            "pagar",
                            "cartão do quarto",
                            "perguntar",
                            "sair",
                            "bagagem",
                            "recepção"
                        },
                        opcoesIngles = new List<string>
                        {
                            "leave",
                            "pay",
                            "night",
                            "luggage",
                            "reception desk",
                            "stay",
                            "room card",
                            "ask"
                        },
                        associacaoCorreta = new List<ParAssociacao>
                        {
                        new ParAssociacao { palavraPortugues = "noite", palavraIngles = "night" },
                        new ParAssociacao { palavraPortugues = "hospedagem", palavraIngles = "stay" },
                        new ParAssociacao { palavraPortugues = "pagar", palavraIngles = "pay" },
                        new ParAssociacao { palavraPortugues = "cartão do quarto", palavraIngles = "room card" },
                        new ParAssociacao { palavraPortugues = "perguntar", palavraIngles = "ask" },
                        new ParAssociacao { palavraPortugues = "bagagem", palavraIngles = "luggage" },
                        new ParAssociacao { palavraPortugues = "sair", palavraIngles = "leave" },
                        new ParAssociacao { palavraPortugues = "recepção", palavraIngles = "reception desk" }
                        }
                    }
            );
        }
        else if(nivelAtual=="B1")
        {
            dialogoAtual = new List<string>()
            {
                cumprimento+". Welcome to the hotel. How can I help you today?",
                "Of course. Could you tell me the name the reservation is under?",
                "I found it. You booked a double room for seven nights, with breakfast included. Is that right?",
                "Perfect. Would you like to pay now by card or cash, or when you check out?",
                "Your room is 308, on the third floor. Here is your key card. The Wi-Fi is free, and the password is on the card.",
                "The elevator is just around the corner, next to the restaurant.",
                "Breakfast is served from 6:30 a.m. to 10:00 a.m. If you prefer, we can also bring it to your room.",
                "Is there anything else you'd like to know about your stay?",
                "Checkout is at 12:00 p.m. on " + dataCheckout + ". If you need a late checkout, please let us know in advance.",
                "We can offer a late checkout until 2:00 p.m., depending on availability.",
                "I hope you enjoy your stay. Just call the front desk if you need anything."
            };

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Tell her you have a reservation, and say how many people it is for.",
                        opcoesFala = new List<string>()
                        {
                            "I have a reservation for one person\n",
                            "I have a breakfast for two people\n",
                            "I reservation for one person\n"
                        },
                        respostaCorreta = 0
                    }
            ); 

            exerciciosWriting.Add(
                new ExercicioWriting()
                {
                    enunciado = "The receptionist asks for the name on the reservation. Write your answer in a complete sentence using -under the name-."
                }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Confirm the question made by the recepcionist",
                        opcoesFala = new List<string>()
                        {
                            "Yes, that's correct\n",
                            "No, I am a double room\n",
                            "Yes, that are correct\n"
                        },
                        respostaCorreta = 0
                    }
            ); 

            exerciciosAlternativas.Add(
                new ExercicioAlternativas()
                {
                    enunciado = "Select the correct answer to say how would you like to pay",
                    alternativas = new List<string>() { 
                        "I'd like to pay at breakfast\r\n", 
                        "I'd like to pay on the second floor\r\n", 
                        "I'd like to pay by card, please\r\n", 
                        "I'd like to pay for seven nights ago\r\n" },
                    alternativaCorreta = 2
                }
            );

            exerciciosCriarFrases.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Arrange the words to form the correct sentence in english: Onde fica o elevador?",
                    blocosPalavras = new List<string>
                    {
                        "Where",
                        "served",
                        "elevator",
                        "Which",
                        "how",
                        "elevator",
                        "perfect",
                        "when",
                        "Who",
                        "is",
                        "for",
                        "lunch",
                        "the",
                        "are"
                    },
                    respostaCorreta = "Where is the elevator? \n"
                }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Speak the correct answer to ask about the place and time of the breakfast",
                        opcoesFala = new List<string>()
                        {
                            "Where breakfast are the time?\n",
                            "What time is breakfast and where is it?\n",
                            "What time is breakfast yesterday?\n"
                        },
                        respostaCorreta = 1
                    }
            );

            exerciciosAlternativas.Add(
                new ExercicioAlternativas()
                {
                    enunciado = "What time does the breakfast end?",
                    alternativas = new List<string>(){ 
                        "6:30 p.m\r\n", 
                        "9:00 a.m\r\n", 
                        "10:00 a.m.\r\n",
                        "12:00 p.m\r\n" },
                    alternativaCorreta = 2
                }
            );

            exerciciosWriting.Add(
                new ExercicioWriting()
                {
                    enunciado = "Write a sentece asking about the time of the checkout."
                }
            );

            /*exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "The receptionist says you can ask for a late checkout. Your flight is in the evening and you need more time in the room. What do you say?",
                        opcoesFala = new List<string>()
                        {
                            "I am the menu yesterday\n",
                            "Yes, please. Could you help me?\n",
                            "The classroom is amazing\n"
                        },
                        respostaCorreta = 1
                    }
            );*/

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        modoLivre = true,
                        enunciado = "You have a flight at 5:00 p.m. Ask the recepcionist if you can checkout later. ",
                    }
            );

            exerciciosCompletarFrase.Add(
                new ExercicioCompletarFrase()
                {
                    enunciado = "Complete the sentence with the correct word:",
                    fraseAntes = "The late checkout is available until ",
                    fraseDepois = " p.m.",
                    respostaCorreta = "2:00",
                    opcoes = new List<string>
                    {
                        "10:00",
                        "served",
                        "2:00",
                        "Which",
                        "tonight",
                        "1:00",
                        "morning",
                        "12:00",                        "5:00",
                        "I",
                    }
                }
            );

            /*exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Choose the correct sentence to say you don't need anything else.",
                        opcoesFala = new List<string>()
                        {
                            "No, I need the room 308\n",
                            "Thank you, all I need are today\n",
                            "No, thank you. That's all I need\n"
                        },
                        respostaCorreta = 2
                    }
            );*/

            exerciciosAssociacao.Add(
                    new ExercicioAssociacao()
                    {
                        enunciado = "Match the Portuguese word with the English word.",
                        opcoesPort = new List<string>
                        {
                            "reservar",
                            "quarto de casal",
                            "ao lado de",
                            "servido",
                            "com antecedência",
                            "voo",
                            "sair",
                            "disponibilidade",
                        },
                        opcoesIngles = new List<string>
                        {
                            "double room",
                            "next to",
                            "availability",
                            "served",
                            "book",
                            "in advance",
                            "flight",
                            "leave",
                        },
                        associacaoCorreta = new List<ParAssociacao>
                        {
                        new ParAssociacao { palavraPortugues = "reservar", palavraIngles = "book" },
                        new ParAssociacao { palavraPortugues = "quarto de casal", palavraIngles = "double room" },
                        new ParAssociacao { palavraPortugues = "ao lado de", palavraIngles = "next to" },
                        new ParAssociacao { palavraPortugues = "servido", palavraIngles = "served" },
                        new ParAssociacao { palavraPortugues = "com antecedência", palavraIngles = "in advance" },
                        new ParAssociacao { palavraPortugues = "voo", palavraIngles = "flight" },
                        new ParAssociacao { palavraPortugues = "sair", palavraIngles = "leave" },
                        new ParAssociacao { palavraPortugues = "disponibilidade", palavraIngles = "availability" }
                        }
                    }
            );
        }
else if (nivelAtual == "A1-A2")
{
    dialogoAtual = new List<string>()
    {
        cumprimento + ". Welcome to the hotel. Do you have a reservation?",
        "Great. Could I have your name, please?",
        "Thank you. How many nights will you be staying?",
        "Would you like to pay by card or cash?",
        "Your room is 206, on the second floor. Here is your room key card. Please take it from the counter.",
        "Breakfast is served from 7:00 a.m. to 10:00 a.m. in the restaurant on the first floor.",
        "Your checkout is at 12:00 p.m. on " + dataCheckout + ". Enjoy your stay!"
    };

    exerciciosSpeaking.Add(
        new ExercicioSpeaking()
        {
            enunciado = "You arrive at the hotel. Tell the receptionist that you have a reservation.",
            opcoesFala = new List<string>()
            {
                "I have the breakfast room\n",
                "I have a reservation, please\n",
                "I am a reservation yesterday\n"
            },
            respostaCorreta = 1
        }
    );

    exerciciosWriting.Add(
        new ExercicioWriting()
        {
            enunciado = "The receptionist asks for your name. Write a complete sentence to introduce yourself."
        }
    );

    exerciciosSpeaking.Add(
        new ExercicioSpeaking()
        {
            enunciado = "The receptionist asks how long you will stay. Choose the correct answer.",
            opcoesFala = new List<string>()
            {
                "I'll pay on the second floor\n",
                "My room is at seven o'clock\n",
                "I'll stay for five nights\n"
            },
            respostaCorreta = 2
        }
    );

    exerciciosAlternativas.Add(
        new ExercicioAlternativas()
        {
            enunciado = "The receptionist asks how you would like to pay. What do you say?",
            alternativas = new List<string>()
            {
                "I'd like to pay at 10:00 a.m.\r\n",
                "I'd like to pay by card, please\r\n",
                "I'd like to pay on the second floor\r\n",
                "I'd like to pay for breakfast yesterday\r\n"
            },
            alternativaCorreta = 1
        }
    );

    exerciciosSpeaking.Add(
        new ExercicioSpeaking()
        {
            enunciado = "You need your room key. What do you say to the receptionist?",
            opcoesFala = new List<string>()
            {
                "Could I have breakfast yesterday?\n",
                "Where is my reservation number floor?\n",
                "Could I have my room key, please?\n"
            },
            respostaCorreta = 2
        }
    );

    exerciciosCriarFrases.Add(
        new ExercicioBlocos()
        {
            enunciado = "Arrange the words to form the correct sentence in English: A que horas o café da manhã é servido?",
            blocosPalavras = new List<string>()
            {
                "breakfast",
                "What",
                "served",
                "time",
                "is",
                "the",
                "lunch",
                "when",
                "are"
            },
            respostaCorreta = "What time is breakfast served?\n"
        }
    );

    exerciciosAlternativas.Add(
        new ExercicioAlternativas()
        {
            enunciado = "According to the receptionist, where can you have breakfast?",
            alternativas = new List<string>()
            {
                "At the reception desk\r\n",
                "Inside the elevator\r\n",
                "On the second floor\r\n",
                "In the restaurant on the first floor\r\n"
            },
            alternativaCorreta = 3
        }
    );

    exerciciosSpeaking.Add(
        new ExercicioSpeaking()
        {
            enunciado = "The receptionist tells you to enjoy your stay. How do you respond?",
            opcoesFala = new List<string>()
            {
                "I would like to pay yesterday\n",
                "My checkout is on the restaurant\n",
                "Thank you. I appreciate it\n"
            },
            respostaCorreta = 2
        }
    );

    exerciciosAssociacao.Add(
        new ExercicioAssociacao()
        {
            enunciado = "Match the Portuguese word with the English word.",
            opcoesPort = new List<string>()
            {
                "reserva",
                "hóspede",
                "estadia",
                "cartão do quarto",
                "balcão",
                "andar",
                "servido",
                "saída"
            },
            opcoesIngles = new List<string>()
            {
                "checkout",
                "guest",
                "room key card",
                "floor",
                "reservation",
                "served",
                "counter",
                "stay"
            },
            associacaoCorreta = new List<ParAssociacao>()
            {
                new ParAssociacao { palavraPortugues = "reserva", palavraIngles = "reservation" },
                new ParAssociacao { palavraPortugues = "hóspede", palavraIngles = "guest" },
                new ParAssociacao { palavraPortugues = "estadia", palavraIngles = "stay" },
                new ParAssociacao { palavraPortugues = "cartão do quarto", palavraIngles = "room key card" },
                new ParAssociacao { palavraPortugues = "balcão", palavraIngles = "counter" },
                new ParAssociacao { palavraPortugues = "andar", palavraIngles = "floor" },
                new ParAssociacao { palavraPortugues = "servido", palavraIngles = "served" },
                new ParAssociacao { palavraPortugues = "saída", palavraIngles = "checkout" }
            }
        }
    );
}
else if (nivelAtual == "A2-B1")
{
    dialogoAtual = new List<string>()
    {
        cumprimento + ". Welcome to the hotel. How can I help you today?",
        "Certainly. Could you tell me the name the reservation is under?",
        "I found your booking. You reserved a double room for seven nights, including breakfast. Is everything correct?",
        "Great. Would you prefer to pay for your stay now or when you check out?",
        "Your room is 308, on the third floor. Here is your room key card. You can pick it up from the counter. The Wi-Fi password is written on it.",
        "The elevator is around the corner, next to the restaurant.",
        "Breakfast is served from 6:30 a.m. to 10:00 a.m. You can also request room service if you prefer to eat in your room.",
        "Is there anything else you would like to know about your stay?",
        "Checkout is at 12:00 p.m. on " + dataCheckout + ". If you need more time, you can request a late checkout.",
        "Late checkout is available until 2:00 p.m., depending on availability. Please contact the front desk in advance."
    };

    exerciciosSpeaking.Add(
        new ExercicioSpeaking()
        {
            enunciado = "You arrive at the hotel. Explain that you have a reservation and mention how long you will be staying.",
            opcoesFala = new List<string>()
            {
                "I would like seven breakfasts for my room\n",
                "I have a reservation for seven nights\n",
                "I have a reservation on the third floor yesterday\n"
            },
            respostaCorreta = 1
        }
    );

    exerciciosWriting.Add(
        new ExercicioWriting()
        {
            enunciado = "The receptionist asks for the name on the reservation. Write a complete answer using the expression 'under the name'."
        }
    );

    exerciciosSpeaking.Add(
        new ExercicioSpeaking()
        {
            enunciado = "The receptionist confirms your reservation details. Confirm that the information is correct.",
            opcoesFala = new List<string>()
            {
                "No, I would like to reserve the elevator\n",
                "Yes, I stayed for breakfast yesterday\n",
                "Yes, that's all correct\n"
            },
            respostaCorreta = 2
        }
    );

    exerciciosAlternativas.Add(
        new ExercicioAlternativas()
        {
            enunciado = "The receptionist asks when you would like to pay. Choose the most appropriate answer.",
            alternativas = new List<string>()
            {
                "I'd prefer to pay on the third floor\r\n",
                "I'd prefer to pay when I check out, please\r\n",
                "I'd prefer to pay next to the elevator\r\n",
                "I'd prefer to pay for breakfast yesterday\r\n"
            },
            alternativaCorreta = 1
        }
    );

    exerciciosSpeaking.Add(
        new ExercicioSpeaking()
        {
            enunciado = "The receptionist gives you information about your room. Ask for your room key card.",
            opcoesFala = new List<string>()
            {
                "Could you tell me the breakfast password, please?\n",
                "Could you reserve the elevator for seven nights?\n",
                "Could you give me the room key card, please?\n"
            },
            respostaCorreta = 2
        }
    );

    exerciciosCriarFrases.Add(
        new ExercicioBlocos()
        {
            enunciado = "Arrange the words to form the correct sentence in English: A senha do Wi-Fi está escrita no cartão.",
            blocosPalavras = new List<string>()
            {
                "written",
                "password",
                "the",
                "on",
                "Wi-Fi",
                "is",
                "card",
                "room",
                "the",
                "breakfast"
            },
            respostaCorreta = "The Wi-Fi password is written on the card.\n"
        }
    );

    exerciciosSpeaking.Add(
        new ExercicioSpeaking()
        {
            enunciado = "You want to find the elevator. Ask the receptionist for directions.",
            opcoesFala = new List<string>()
            {
                "Could you tell me how much the breakfast walks?\n",
                "Could you tell me where the elevator is?\n",
                "Could you tell me when the room is served?\n"
            },
            respostaCorreta = 1
        }
    );

    exerciciosWriting.Add(
        new ExercicioWriting()
        {
            enunciado = "You would like to know whether breakfast can be served in your room. Write a question to the receptionist."
        }
    );

    exerciciosAlternativas.Add(
        new ExercicioAlternativas()
        {
            enunciado = "According to the receptionist, what time does breakfast service end?",
            alternativas = new List<string>()
            {
                "2:00 p.m.\r\n",
                "6:30 a.m.\r\n",
                "10:00 a.m.\r\n",
                "12:00 p.m.\r\n"
            },
            alternativaCorreta = 2
        }
    );

    exerciciosWriting.Add(
        new ExercicioWriting()
        {
            enunciado = "You have plans in the afternoon and would like to leave the hotel later. Write a polite question asking whether late checkout is available."
        }
    );

    exerciciosSpeaking.Add(
        new ExercicioSpeaking()
        {
            enunciado = "The receptionist explains that late checkout depends on availability. Ask what time you could leave if your request is approved.",
            opcoesFala = new List<string>()
            {
                "How many floors does my reservation have?\n",
                "What time would I need to check out?\n",
                "Where would I have my breakfast yesterday?\n"
            },
            respostaCorreta = 1
        }
    );

    exerciciosCompletarFrase.Add(
        new ExercicioCompletarFrase()
        {
            enunciado = "Complete the sentence with the correct time:",
            fraseAntes = "Late checkout is available until ",
            fraseDepois = " p.m.",
            respostaCorreta = "2:00",
            opcoes = new List<string>()
            {
                "6:30",
                "10:00",
                "12:00",
                "1:00",
                "2:00"
            }
        }
    );

    exerciciosAssociacao.Add(
        new ExercicioAssociacao()
        {
            enunciado = "Match the Portuguese expression with the English expression.",
            opcoesPort = new List<string>()
            {
                "reserva",
                "quarto de casal",
                "confirmar",
                "senha",
                "ao virar a esquina",
                "serviço de quarto",
                "com antecedência",
                "disponibilidade"
            },
            opcoesIngles = new List<string>()
            {
                "in advance",
                "double room",
                "availability",
                "reservation",
                "room service",
                "around the corner",
                "password",
                "confirm"
            },
            associacaoCorreta = new List<ParAssociacao>()
            {
                new ParAssociacao { palavraPortugues = "reserva", palavraIngles = "reservation" },
                new ParAssociacao { palavraPortugues = "quarto de casal", palavraIngles = "double room" },
                new ParAssociacao { palavraPortugues = "confirmar", palavraIngles = "confirm" },
                new ParAssociacao { palavraPortugues = "senha", palavraIngles = "password" },
                new ParAssociacao { palavraPortugues = "ao virar a esquina", palavraIngles = "around the corner" },
                new ParAssociacao { palavraPortugues = "serviço de quarto", palavraIngles = "room service" },
                new ParAssociacao { palavraPortugues = "com antecedência", palavraIngles = "in advance" },
                new ParAssociacao { palavraPortugues = "disponibilidade", palavraIngles = "availability" }
            }
        }
    );
}

        descricao.Falas = dialogoAtual;
        return descricao;
    }

    private string ObterDataCheckout()
    {
        int noites = nivelAtual == "A1-A2" ? 5 : 7;
        DateTime dataCheckout = DateTime.Now.Date.AddDays(noites);

        int dia = dataCheckout.Day;
        string sufixo;

        if (dia % 100 >= 11 && dia % 100 <= 13)
        {
            sufixo = "th";
        }
        else
        {
            switch (dia % 10)
            {
                case 1:
                    sufixo = "st";
                    break;
                case 2:
                    sufixo = "nd";
                    break;
                case 3:
                    sufixo = "rd";
                    break;
                default:
                    sufixo = "th";
                    break;
            }
        }

        string mes = dataCheckout.ToString(
            "MMMM",
            CultureInfo.GetCultureInfo("en-US")
        );

        return $"{mes} {dia}{sufixo}";
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

    protected override void AoIniciarInteracao()
    {
        ultimaVisitaHotel = VisitaAtual;
        MissionManager.Instance?.RegistrarEventoHotel(MissoesHotel.InicioConversa, NpcKey);
    }

    protected override Task IniciarInteracao() => ExecutarAtendimento(0);

    private async Task ExecutarAtendimento(int inicio, string saudacao = null)
    {
        int versao = SessaoCenario.Atual.Versao;
        GameProgress.EstaEmDialogo = true;
        nivelAtual = DadosJogador.nivelUsuario;
        CartaoQuartoHotel cartao = cartaoQuarto != null ? cartaoQuarto.GetComponent<CartaoQuartoHotel>() : null;
        try
        {
            if (cartao != null)
                cartao.Resetar();
            if (saudacao == null)
                AtualizarPeriodoDoDia();
            else
                RestaurarSaudacaoCheckpoint(saudacao);
            InicializarConteudosPorNivel();
            catalogoExercicios = await ExerciseManager.Instance.LoadForNpcAsync(NpcKey, nivelAtual);
            if (!catalogoExercicios.Success)
                Debug.LogWarning("RecepcionistHotelNPC: usando exercícios locais. " + catalogoExercicios.Error);

            ValidarVisita(versao);
            await CriarRoteiro(versao).Executar(inicio);

            MissionManager.Instance?.RegistrarEventoHotel(MissoesHotel.FimConversa, NpcKey);
            if (MissoesHotel.UsaCartao(nivelAtual)
                && !await ContinuarAposConversaCheckpoint(cartao, CheckpointJogo.HotelCartao))
                return;

            if (!MissoesHotel.UsaCartao(nivelAtual))
                CheckpointJogo.Instance?.CapturarMarcoHotel(CheckpointJogo.HotelRevisao);
        }
        finally
        {
            if (cartao != null)
                cartao.Resetar();
            GameProgress.EstaEmDialogo = false;
            OcultarInterfaceDaConversa();
            LiberarControlePlayer();
        }

        if (RevisaoManager.Instance != null)
            await RevisaoManager.Instance.ExecutarRevisaoEExibirModalFinal(this);
        else
            Debug.LogError("RevisaoManager não está disponível para finalizar Hotel.");
    }

    async Task<System.Func<Task>> IRoteiroNpcCheckpoint.PrepararRetomada(Newtonsoft.Json.Linq.JObject visita,
        int versao, System.Threading.CancellationToken token)
    {
        ValidarVisita(versao);
        AtualizarVisita();
        nivelAtual = SessaoCenario.Atual.Nivel;
        string saudacao = RoteiroNpcCheckpoint.SaudacaoSalva(visita);
        RestaurarSaudacaoCheckpoint(saudacao);
        InicializarConteudosPorNivel();
        var cursor = CriarRoteiro(versao).Validar(visita);
        if (cursor.Situacao.Length != 1)
            throw new System.IO.InvalidDataException("Estado do roteiro do Hotel inválido.");
        await RoteiroNpcCheckpoint.ExigirCatalogo(this, versao, token);
        return async () =>
        {
            ultimaVisitaHotel = VisitaAtual;
            TravarControlePlayer();
            await ExecutarAtendimento(cursor.Indice, saudacao);
        };
    }

    private void RestaurarSaudacaoCheckpoint(string saudacao)
    {
        cumprimento = saudacao;
        bool manha = saudacao == "Good morning!";
        bool tarde = saudacao == "Good afternoon!";
        periodo = manha ? "de manhã" : tarde ? "a tarde" : "a noite";
        periodoIng = manha ? "this morning" : tarde ? "this afternoon" : "tonight";
    }

    private async Task<bool> ContinuarAposConversaCheckpoint(CartaoQuartoHotel cartao, string passo)
    {
        if (passo == CheckpointJogo.HotelRevisao)
            return true;

        if (passo == CheckpointJogo.HotelCartao)
        {
            if (!await AguardarRecolherCartao(cartao))
                return false;
            CheckpointJogo.Instance?.CapturarMarcoHotel(CheckpointJogo.HotelAssociacao);
        }

        await AbrirExercicioDoPonto(VocabularioHotel, 1, exerciciosAssociacao[0]);
        CheckpointJogo.Instance?.CapturarMarcoHotel(CheckpointJogo.HotelRevisao);
        return true;
    }

    internal async Task RetomarMarcoCheckpoint(string passo, int versao)
    {
        if (!SessaoCenario.Atual.Permite(MissoesHotel.Cenario, versao))
            return;

        AtualizarVisita();
        ultimaVisitaHotel = VisitaAtual;
        interacaoConcluida = true;
        nivelAtual = SessaoCenario.Atual.Nivel;
        GameProgress.EstaEmDialogo = true;
        TravarControlePlayer();
        var cartao = cartaoQuarto != null ? cartaoQuarto.GetComponent<CartaoQuartoHotel>() : null;
        try
        {
            cartao?.Resetar();
            if (passo != CheckpointJogo.HotelRevisao)
            {
                AtualizarPeriodoDoDia();
                InicializarConteudosPorNivel();
                catalogoExercicios = await ExerciseManager.Instance.LoadForNpcAsync(NpcKey, nivelAtual);
                if (this == null || !SessaoCenario.Atual.Permite(MissoesHotel.Cenario, versao))
                    return;
            }

            if (!await ContinuarAposConversaCheckpoint(cartao, passo))
                return;
        }
        finally
        {
            if (this != null && SessaoCenario.Atual.Permite(MissoesHotel.Cenario, versao))
            {
                cartao?.Resetar();
                OcultarInterfaceDaConversa();
                GameProgress.EstaEmDialogo = false;
                LiberarControlePlayer();
            }
        }

        if (this != null && SessaoCenario.Atual.Permite(MissoesHotel.Cenario, versao)
            && RevisaoManager.Instance != null)
            await RevisaoManager.Instance.ExecutarRevisaoEExibirModalFinal(this);
    }

    private void OcultarInterfaceDaConversa()
    {
        if (modalLegenda != null)
            modalLegenda.SetActive(false);
        if (textoPularDialogo != null)
            textoPularDialogo.gameObject.SetActive(false);
    }

    private async Task<bool> AguardarRecolherCartao(CartaoQuartoHotel cartao)
    {
        OcultarInterfaceDaConversa();
        if (cartao == null || !cartao.Disponibilizar())
        {
            Debug.LogError("RecepcionistHotelNPC: vincule um cartão com CartaoQuartoHotel e Collider ativo.", this);
            return false;
        }

        // O raio aceita somente o cartão durante esta etapa, com o movimento liberado para alcançá-lo.
        LiberarControlePlayer();
        int visita = SessaoCenario.Atual.Versao;
        CheckpointJogo.Instance?.CapturarMarcoHotel(CheckpointJogo.HotelCartao);
        while (cartao != null && CartaoQuartoHotel.Disponivel == cartao
            && SessaoCenario.Atual.Permite(MissoesHotel.Cenario, visita))
            await Task.Yield();

        if (!SessaoCenario.Atual.Permite(MissoesHotel.Cenario, visita) || cartao == null || !cartao.Recolhido)
            return false;

        MissionManager.Instance?.RegistrarEventoHotel(MissoesHotel.Marco, MissoesHotel.CartaoRecolhido);
        TravarControlePlayer();
        return true;
    }


    internal const string VocabularioHotel = "hotel_vocabulary";

    internal static RoteiroNpcCheckpoint DescreverRoteiro(string nivel)
    {
        var conteudo = MontarConteudo(nivel, "{{greeting}}", "{{periodo}}", "{{periodo_ing}}", "{{checkout_date}}");
        return MontarRoteiro(0, nivel, null, conteudo);
    }

    private RoteiroNpcCheckpoint CriarRoteiro(int visita) => MontarRoteiro(visita, nivelAtual, this);

    private static RoteiroNpcCheckpoint MontarRoteiro(int visita, string nivelAtual, RecepcionistHotelNPC npc,
        ConteudoRoteiro descricao = null)
    {
        var dialogoAtual = descricao?.Falas ?? npc.dialogoAtual;
        var exerciciosCompletarFrase = descricao?.exerciciosCompletarFrase ?? npc.exerciciosCompletarFrase;
        var exerciciosCriarFrases = descricao?.exerciciosCriarFrases ?? npc.exerciciosCriarFrases;
        var exerciciosSpeaking = descricao?.exerciciosSpeaking ?? npc.exerciciosSpeaking;
        var exerciciosAlternativas = descricao?.exerciciosAlternativas ?? npc.exerciciosAlternativas;
        var exerciciosWriting = descricao?.exerciciosWriting ?? npc.exerciciosWriting;
        var exerciciosAssociacao = descricao?.exerciciosAssociacao ?? npc.exerciciosAssociacao;

        var roteiro = npc == null ? new RoteiroNpcCheckpoint(NpcKey)
            : new RoteiroNpcCheckpoint(npc, visita, () => new[] { RoteiroNpcCheckpoint.ChaveSaudacao(npc.cumprimento) });
        void Fala(int indice) => roteiro.Fala(dialogoAtual[indice], () => npc.PlayAudioETexto(indice));
        void Exercicio(string ponto, int ordem, ExercicioBase fallback) =>
            roteiro.Exercicio(ponto, ordem, () => npc.AbrirExercicioDoPonto(ponto, ordem, fallback), modelo: fallback);

        switch (nivelAtual)
        {
            case "A1":
                Fala(0);
                Exercicio("reservation", 1, exerciciosSpeaking[0]);
                Fala(1);
                Exercicio("guest_name", 1, exerciciosWriting[0]);
                Fala(2);
                Exercicio("payment", 1, exerciciosAlternativas[0]);
                Fala(3);
                Exercicio("breakfast", 1, exerciciosCriarFrases[0]);
                Fala(4);
                Exercicio("checkout", 1, exerciciosSpeaking[1]);
                Fala(5);
                Exercicio("stay_duration", 1, exerciciosAlternativas[1]);
                Fala(6);
                Exercicio("farewell", 1, exerciciosSpeaking[2]);
                Exercicio("hotel_vocabulary", 1, exerciciosAssociacao[0]);
                break;
            case "A2":
                Fala(0);
                Exercicio("reservation", 1, exerciciosCriarFrases[0]);
                Fala(1);
                Exercicio("guest_name", 1, exerciciosWriting[0]);
                Fala(2);
                Exercicio("stay_duration", 1, exerciciosSpeaking[0]);
                Fala(3);
                Exercicio("payment", 1, exerciciosAlternativas[0]);
                Fala(4);
                Exercicio("breakfast", 1, exerciciosWriting[1]);
                Fala(5);
                Exercicio("breakfast", 2, exerciciosAlternativas[1]);
                Fala(6);
                Exercicio("checkout", 1, exerciciosSpeaking[1]);
                Fala(7);
                Exercicio("checkout", 2, exerciciosCompletarFrase[0]);
                Fala(8);
                Exercicio("farewell", 1, exerciciosSpeaking[2]);
                Fala(9);
                break;
            case "B1":
                Fala(0);
                Exercicio("reservation", 1, exerciciosSpeaking[0]);
                Fala(1);
                Exercicio("guest_name", 1, exerciciosWriting[0]);
                Fala(2);
                Exercicio("reservation", 2, exerciciosSpeaking[1]);
                Fala(3);
                Exercicio("payment", 1, exerciciosAlternativas[0]);
                Fala(4);
                Exercicio("elevator", 1, exerciciosCriarFrases[0]);
                Fala(5);
                Exercicio("breakfast", 1, exerciciosSpeaking[2]);
                Fala(6);
                Exercicio("breakfast", 2, exerciciosAlternativas[1]);
                Fala(7);
                Exercicio("checkout", 1, exerciciosWriting[1]);
                Fala(8);
                Exercicio("checkout", 2, exerciciosSpeaking[3]);
                Fala(9);
                Exercicio("checkout", 3, exerciciosCompletarFrase[0]);
                Fala(10);
                Exercicio("hotel_vocabulary", 1, exerciciosAssociacao[0]);
                break;
            case "A1-A2":
                Fala(0);
                Exercicio("reservation", 1, exerciciosSpeaking[0]);
                Fala(1);
                Exercicio("guest_name", 1, exerciciosWriting[0]);
                Fala(2);
                Exercicio("stay_duration", 1, exerciciosSpeaking[1]);
                Fala(3);
                Exercicio("payment", 1, exerciciosAlternativas[0]);
                Fala(4);
                Exercicio("room_key", 1, exerciciosSpeaking[2]);
                Exercicio("breakfast", 1, exerciciosCriarFrases[0]);
                Fala(5);
                Exercicio("breakfast", 2, exerciciosAlternativas[1]);
                Fala(6);
                Exercicio("farewell", 1, exerciciosSpeaking[3]);
                break;
            case "A2-B1":
                Fala(0);
                Exercicio("reservation", 1, exerciciosSpeaking[0]);
                Fala(1);
                Exercicio("guest_name", 1, exerciciosWriting[0]);
                Fala(2);
                Exercicio("reservation", 2, exerciciosSpeaking[1]);
                Fala(3);
                Exercicio("payment", 1, exerciciosAlternativas[0]);
                Fala(4);
                Exercicio("room_key", 1, exerciciosSpeaking[2]);
                Fala(5);
                Exercicio("wifi", 1, exerciciosCriarFrases[0]);
                Exercicio("elevator", 1, exerciciosSpeaking[3]);
                Fala(6);
                Exercicio("breakfast", 1, exerciciosWriting[1]);
                Fala(7);
                Exercicio("breakfast", 2, exerciciosAlternativas[1]);
                Fala(8);
                Exercicio("checkout", 1, exerciciosWriting[2]);
                Fala(9);
                Exercicio("checkout", 2, exerciciosSpeaking[4]);
                Exercicio("checkout", 3, exerciciosCompletarFrase[0]);
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
        {
            ExercicioBase exercicio = item.Exercise;
            if (item.Exercise is ExercicioBlocos blocos
                && (blocos.enunciado?.Contains(PeriodoToken) == true
                    || blocos.respostaCorreta?.Contains(PeriodoIngToken) == true))
            {
                exercicio = PreencherPeriodo(blocos);
            }

            await AbrirExercicio(item.Tipo, exercicio);
        }
        else
        {
            Debug.LogWarning($"RecepcionistHotelNPC: usando exercício local em {nivelAtual}/{pointKey}/{ordem}.");
            await AbrirExercicio(tipo, fallback);
        }

        if (nivelAtual == "B1")
        {
            string alvo = MissoesHotel.AlvoExercicio(item?.Exercise.pointKey ?? pointKey, item?.Exercise.ordemBanco ?? ordem);
            MissionManager.Instance?.RegistrarEventoHotel(MissoesHotel.ExercicioEncerrado, alvo);
        }
    }

    private ExercicioBlocos PreencherPeriodo(ExercicioBlocos blocos)
    {
        return PreencherPeriodo(blocos, periodoIng == "this morning" ? "morning"
            : periodoIng == "this afternoon" ? "afternoon" : "night");
    }

    internal static ExercicioBlocos PreencherPeriodo(ExercicioBlocos blocos, string periodoCheckpoint)
    {
        string periodo = periodoCheckpoint == "morning" ? "de manhã"
            : periodoCheckpoint == "afternoon" ? "a tarde" : "a noite";
        string periodoIng = periodoCheckpoint == "morning" ? "this morning"
            : periodoCheckpoint == "afternoon" ? "this afternoon" : "tonight";
        string Preencher(string texto) => texto?.Replace(PeriodoToken, periodo).Replace(PeriodoIngToken, periodoIng);

        // O catálogo é compartilhado pelo cache. Os textos preenchidos são locais à interação.
        return new ExercicioBlocos
        {
            reviewPeriod = periodoCheckpoint,
            idExercise = blocos.idExercise,
            idTipoBanco = blocos.idTipoBanco,
            nomeTipoBanco = blocos.nomeTipoBanco,
            nivelBanco = blocos.nivelBanco,
            pointKey = blocos.pointKey,
            ordemBanco = blocos.ordemBanco,
            enunciado = Preencher(blocos.enunciado),
            feedback = blocos.feedback,
            respostasEsperadas = blocos.respostasEsperadas?.ConvertAll(Preencher),
            blocosPalavras = blocos.blocosPalavras,
            respostaCorreta = Preencher(blocos.respostaCorreta)
        };
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
        await ApresentarFalaDoRoteiro(backendManagerComum, audioSourceComum, idNpcParaVoz,
            nomeExibicaoLegenda, ultimaFraseDita, modalLegenda, textoPularDialogo);
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
