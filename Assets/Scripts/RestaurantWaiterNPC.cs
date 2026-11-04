// using System.Collections.Generic;
// using System.Threading.Tasks;
// using UnityEngine;
// using UnityEngine.Networking;
// using System.IO;
// using TMPro;
// using System;

// public class RestaurantWaiterNPC : InteracaoNPC
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

//     private bool conversaClienteConcluida;

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
//         nomeExibicaoLegenda = "Restaurant Waiter";
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
//                 "Welcome!",
//                 "Here is the menu.",
//                 "Are you ready to order?",
//                 "Anything to drink?",
//                 "OK. Your order will be ready in about fifteen minutes.",
//                 "How can I help you?",
//                 "Of course",
//                 "Here it is",
//                 "Sure. The bill is 30 dollars. Cash or card?"
//             };

//             exerciciosSpeaking.Add(
//                     new ExercicioSpeaking()
//                     {
//                         enunciado = "You want a hamburger. What do you say to the waiter?",
//                         opcoesFala = new List<string>()
//                         {
//                             "I need to check out of my hotel\n",
//                             "CI like order hamburger yesterday\n",
//                             "I would like a hamburger, please\n"
//                         },
//                         respostaCorreta = 2
//                     }
//             );  

//             exerciciosAlternativas.Add(
//                     new ExercicioAlternativas()
//                     {
//                         enunciado = "Ask for some drink to the waiter",
//                         alternativas = new List<string>() { 
//                         "I would like a juice, please\r\n", 
//                         "I would like a hamburger, please\r\n", 
//                         "I would like the menu, please\r\n", 
//                         "I would like the bill, please\r\n" },
//                         alternativaCorreta = 0
//                     }
//             );  

//             exerciciosCriarFrases.Add(
//                 new ExercicioBlocos()
//                 {
//                     enunciado= "Arrange the words to form the correct sentence in english: Posso levar isso para viagem?",
//                     blocosPalavras = new List<string>
//                     {
//                         "this",
//                         "Can",
//                         "order",
//                         "go",
//                         "I",
//                         "take",
//                         "menu",
//                         "to",
//                         "eat",
//                         "it",
//                         "please",
//                         "for",
//                         "table",
//                         "the"
//                     },
//                     respostaCorreta = "Can I take this to go?\n"
//                 }
//             );

//             exerciciosSpeaking.Add(
//                     new ExercicioSpeaking()
//                     {
//                         enunciado = "You want to pay. What do you say to the waiter?",
//                         opcoesFala = new List<string>()
//                         {
//                             "Could I have some extra napkins, please?\n",
//                             "Can I have the bill, please?\n",
//                             "Is the soup served hot?\n"
//                         },
//                         respostaCorreta = 1
//                     }
//             );  

//             exerciciosAlternativas.Add(
//                     new ExercicioAlternativas()
//                     {
//                         enunciado = "The waiter asks how you want to pay. Choose the correct answer.",
//                         alternativas = new List<string>() { 
//                         "By card\r\n", 
//                         "By table\r\n", 
//                         "By menu\r\n", 
//                         "By hamburger\r\n" },
//                         alternativaCorreta = 0
//                     }
//             );  

//             exerciciosSpeaking.Add(
//                     new ExercicioSpeaking()
//                     {
//                         enunciado = "Say thank you to the waiter."
//                     }
//             );  

//             exerciciosAssociacao.Add(
//                 new ExercicioAssociacao()
//                 {
//                     enunciado = "Match the Portuguese word with the English word.",

//                     opcoesPort = new List<string>
//                     {
//                         "cardápio",
//                         "pedido",
//                         "hambúrguer",
//                         "suco",
//                         "conta",
//                         "pagar",
//                         "água"
//                     },

//                     opcoesIngles = new List<string>
//                     {
//                         "water",
//                         "menu",
//                         "bill",
//                         "hamburger",
//                         "pay",
//                         "order",
//                         "juice"
//                     },

//                     associacaoCorreta = new List<ParAssociacao>
//                     {
//                         new ParAssociacao { palavraPortugues = "cardápio", palavraIngles = "menu" },
//                         new ParAssociacao { palavraPortugues = "pedido", palavraIngles = "order" },
//                         new ParAssociacao { palavraPortugues = "hambúrguer", palavraIngles = "hamburger" },
//                         new ParAssociacao { palavraPortugues = "suco", palavraIngles = "juice" },
//                         new ParAssociacao { palavraPortugues = "conta", palavraIngles = "bill" },
//                         new ParAssociacao { palavraPortugues = "pagar", palavraIngles = "pay" },
//                         new ParAssociacao { palavraPortugues = "água", palavraIngles = "water" }
//                     }
//                 }
//             );
//         }
//         else if(nivelAtual=="A2")
//         {
//             dialogoAtual = new List<string>()
//             {
//                 cumprimento+". Welcome to our restaurant.",
//                 "Here is the menu. Take your time.",
//                 "Are you ready to order?",
//                 "Your order will be ready in about twenty minutes.",
//                 "Here is your hamburger and orange juice",
//                 "Is everything okay with your meal?",
//                 "Of course. I'll bring it right away.",
//                 "Sure. Your bill is $43. WWould you like to pay by card or cash?",
//                 "Thank you. I hope you enjoyed your meal. Come back soon!"
//             };

//             exerciciosCriarFrases.Add(
//                 new ExercicioBlocos()
//                 {
//                     enunciado= "Arrange the words to form the correct sentence in english: Eu gostaria de um hambúrguer e um suco de laranja, por favor",
//                     blocosPalavras = new List<string>
//                     {
//                         "orange",
//                         "I",
//                         "please",
//                         "sandwich",
//                         "would",
//                         "juice",
//                         "like",
//                         "a",
//                         "hamburger",
//                         "and",
//                         "water",
//                         "some",
//                         "the",
//                         "pizza",
//                         "an"
//                     },
//                     respostaCorreta = "I would like a hamburger and an orange juice, please\n"
//                 }
//             );

//             exerciciosAlternativas.Add(
//                     new ExercicioAlternativas()
//                     {
//                         enunciado = "How long will the order take?",
//                         alternativas = new List<string>() { 
//                         "5 minutes\r\n", 
//                         "10 minutes\r\n", 
//                         "15 minutes\r\n", 
//                         "30 minutes\r\n" },
//                         alternativaCorreta = 2
//                     }
//             ); 

//             exerciciosCompletarFrase.Add(
//                 new ExercicioCompletarFrase()
//                 {
//                     enunciado = "Complete the sentence with the correct words:",
//                     partesFrase = new List<string>
//                     {
//                         "Could you ",
//                         " up the ",
//                         " of my burger, please?"
//                     },
//                     respostaCorreta = "wrap, rest",
//                     opcoes = new List<string>
//                     {
//                         "everything",
//                         "rest",
//                         "ready",
//                         "wrap",
//                         "perfect",
//                         "No",
//                         "are",
//                         "the",
//                         "food",
//                         "very",
//                         "it"
//                     }
//                 }
//             );

//             exerciciosCompletarFrase.Add(
//                 new ExercicioCompletarFrase()
//                 {
//                     enunciado = "Complete the sentence with the correct words:",
//                     partesFrase = new List<string>
//                     {
//                         "",
//                         " I have the ",
//                         ", please?"
//                     },
//                     respostaCorreta = "Could, bill",
//                     opcoes = new List<string>
//                     {
//                         "bill",
//                         "price",
//                         "heat",
//                         "menu",
//                         "pack",
//                         "cut",
//                         "order",
//                         "Could"
//                     }
//                 }
//             );

//             exerciciosAlternativas.Add(
//                     new ExercicioAlternativas()
//                     {
//                         enunciado = "Select the correct option to answer the waiter.",
//                         alternativas = new List<string>() { 
//                         "I'd like to pay by card, please\r\n", 
//                         "I'd like to pay by table\r\n", 
//                         "I'd like to pay by hamburger\r\n", 
//                         "I'd like to pay by menu\r\n" },
//                         alternativaCorreta = 0
//                     }
//             ); 

//             exerciciosAssociacao.Add(
//                 new ExercicioAssociacao()
//                 {
//                     enunciado = "Match the Portuguese word with the English word.",

//                     opcoesPort = new List<string>
//                     {
//                         "acompanhamento",
//                         "embalagem",
//                         "talheres",
//                         "pedido",
//                         "restante",
//                         "conta",
//                         "dinheiro em espécie",
//                         "troco"
//                     },

//                     opcoesIngles = new List<string>
//                     {
//                         "change",
//                         "order",
//                         "cash",
//                         "side dish",
//                         "package",
//                         "bill",
//                         "cutlery",
//                         "leftovers"
//                     },

//                     associacaoCorreta = new List<ParAssociacao>
//                     {
//                         new ParAssociacao { palavraPortugues = "acompanhamento", palavraIngles = "side dish" },
//                         new ParAssociacao { palavraPortugues = "embalagem", palavraIngles = "package" },
//                         new ParAssociacao { palavraPortugues = "talheres", palavraIngles = "cutlery" },
//                         new ParAssociacao { palavraPortugues = "pedido", palavraIngles = "order" },
//                         new ParAssociacao { palavraPortugues = "restante", palavraIngles = "leftovers" },
//                         new ParAssociacao { palavraPortugues = "conta", palavraIngles = "bill" },
//                         new ParAssociacao { palavraPortugues = "dinheiro em espécie", palavraIngles = "cash" },
//                         new ParAssociacao { palavraPortugues = "troco", palavraIngles = "change" }
//                     }
//                 }
//             );
//         }
//         else if(nivelAtual=="B1")
//         {
//             dialogoAtual = new List<string>()
//             {
//                 cumprimento+". Welcome. Here is the menu. Let me know when you're ready to order.",
//                 "It comes with a beef patty, cheese, lettuce, tomato, and onion. You can also add bacon for 5 dollars.",
//                 "The haburger is our best-seller. Most customers love it.",
//                 "Anything else?",
//                 "Your order should be ready in about ten minutes. I'll bring it to you as soon as it's ready.",
//                 "How was everything? Did you enjoy your meal?",
//                 "Certainly. I'll be right back.",
//                 "Here is the bag with your food.",
//                 "Your bill is R$42. Would you like to pay by card or cash?",
//                 "Thank you very much. Would you like a receipt?"
//             };

//             exerciciosSpeaking.Add(
//                     new ExercicioSpeaking()
//                     {
//                         enunciado = "You are interested in the hambuger. Ask what comes with it.",
//                     }
//             );

//             exerciciosSpeaking.Add(
//                     new ExercicioSpeaking()
//                     {
//                         enunciado = "Ask the restaurant employee for a recommendation to help you choose a dish.",
//                         opcoesFala = new List<string>()
//                         {
//                             "Why did you recommended this dish to me?\n",
//                             "Could you tell me when the airport check-in desk opens?\n",
//                             "Could you recommend a dish from the menu?\n"
//                         },
//                         respostaCorreta = 2
//                     }
//             );  

//             exerciciosCriarFrases.Add(
//                 new ExercicioBlocos()
//                 {
//                     enunciado= "Arrange the words to form the correct sentence in english: Eu gostaria de pedir um hambúrguer acompanhado de um suco de limão, por favor.",
//                     blocosPalavras = new List<string>
//                     {
//                         "lemon",
//                         "would",
//                         "meal",
//                         "please",
//                         "hamburger",
//                         "to",
//                         "some",
//                         "I",
//                         "by",
//                         "juice",
//                         "order",
//                         "the",
//                         "an",
//                         "accompanied",
//                         "drink",
//                         "a",
//                         "like",
//                         "Could",
//                         "with"
//                     },
//                     respostaCorreta = "I would like to order a hamburger, accompanied by an lemon juice, please.\n"
//                 }
//             );

//             exerciciosCriarFrases.Add(
//                 new ExercicioBlocos()
//                 {
//                     enunciado= "Arrange the words to form the correct sentence in english: Não, obrigado. Isso é tudo o que vou pedir por enquanto.",
//                     blocosPalavras = new List<string>
//                     {
//                         "order",
//                         "No",
//                         "everything",
//                         "for",
//                         "I'll",
//                         "thank",
//                         "now",
//                         "That's",
//                         "please",
//                         "you",
//                         "all",
//                         "the",
//                         "would",
//                         "too",
//                         "much"
//                     },
//                     respostaCorreta = "No, thank you. That’s all I’ll order for now.\n"
//                 }
//             );

//             exerciciosCompletarFrase.Add(
//                 new ExercicioCompletarFrase()
//                 {
//                     enunciado = "Complete the sentence with the correct words:",
//                     partesFrase = new List<string>
//                     {
//                         "Your order should be ready in about ",
//                         " ",
//                         ", please?"
//                     },
//                     respostaCorreta = "20, minutes",
//                     opcoes = new List<string>
//                     {
//                         "minutes",
//                         "price",
//                         "heat",
//                         "menu",
//                         "pack",
//                         "20",
//                         "cut",
//                         "30",
//                         "12",
//                         "half",
//                         "hour",
//                         "ready"
//                     }
//                 }
//             );

//             exerciciosWriting.Add(
//                 new ExercicioWriting()
//                 {
//                     enunciado = "Compliment the burger to the waiter."
//                 }
//             );

//             exerciciosSpeaking.Add(
//                 new ExercicioSpeaking()
//                 {
//                     enunciado = "You couldn't finish your hamburger. Ask the waiter to wrap the rest for you."
//                 }
//             );  

//             exerciciosSpeaking.Add(
//                 new ExercicioSpeaking()
//                 {
//                     enunciado = "Ask the waiter for the bill politely so you can pay for your meal."
//                 }
//             ); 

//             exerciciosSpeaking.Add(
//                     new ExercicioSpeaking()
//                     {
//                         enunciado = "You are ready to pay for your meal. Choose the most appropriate way to tell the waiter how you would like to pay.",
//                         opcoesFala = new List<string>()
//                         {
//                             "I’d prefer to pay by card, if that’s possible\n",
//                             "I’ve already paid for my meal, so I don’t need the bill\n",
//                             "Could I pay for the table next to the window, please?\n"
//                         },
//                         respostaCorreta = 0
//                     }
//             );

//             exerciciosAlternativas.Add(
//                     new ExercicioAlternativas()
//                     {
//                         enunciado = "Choose the most appropriate response to politely decline an additional offer after finishing your meal",
//                         alternativas = new List<string>() { 
//                         "I’d like to buy a coat that matches my shoes\r\n", 
//                         "I usually take the bus to school because it’s more convenient\r\n", 
//                         "She’s preparing dinner at home while her family sets the table\r\n", 
//                         "No, thank you. Everything was delicious, and I really enjoyed the meal.\r\n" },
//                         alternativaCorreta = 3
//                     }
//             );

//             exerciciosAssociacao.Add(
//                 new ExercicioAssociacao()
//                 {
//                     enunciado = "Match the Portuguese word or expression with its English equivalent.",

//                     opcoesPort = new List<string>
//                     {
//                         "ingredientes",
//                         "mais vendido",
//                         "porção",
//                         "saboroso",
//                         "sobras",
//                         "embrulhar para viagem",
//                         "conta",
//                         "pagar em dinheiro",
//                         "recomendar",
//                         "acompanhamento"
//                     },

//                     opcoesIngles = new List<string>
//                     {
//                         "bill",
//                         "side dish",
//                         "leftovers",
//                         "ingredients",
//                         "pay in cash",
//                         "best-seller",
//                         "wrap up",
//                         "portion",
//                         "recommend",
//                         "delicious"
//                     },

//                     associacaoCorreta = new List<ParAssociacao>
//                     {
//                         new ParAssociacao { palavraPortugues = "ingredientes", palavraIngles = "ingredients" },
//                         new ParAssociacao { palavraPortugues = "mais vendido", palavraIngles = "best-seller" },
//                         new ParAssociacao { palavraPortugues = "porção", palavraIngles = "portion" },
//                         new ParAssociacao { palavraPortugues = "saboroso", palavraIngles = "delicious" },
//                         new ParAssociacao { palavraPortugues = "sobras", palavraIngles = "leftovers" },
//                         new ParAssociacao { palavraPortugues = "embrulhar para viagem", palavraIngles = "wrap up" },
//                         new ParAssociacao { palavraPortugues = "conta", palavraIngles = "bill" },
//                         new ParAssociacao { palavraPortugues = "pagar em dinheiro", palavraIngles = "pay in cash" },
//                         new ParAssociacao { palavraPortugues = "recomendar", palavraIngles = "recommend" },
//                         new ParAssociacao { palavraPortugues = "acompanhamento", palavraIngles = "side dish" }
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

//         if (RevisaoManager.Instance != null)
//             await RevisaoManager.Instance.ExecutarRevisaoEExibirModalFinal(this);
//         else
//             Debug.LogError("RevisaoManager não está disponível para finalizar Restaurant.");
//     }

//     private async Task interacaoA1()
//     {
//         int i = 0;

//         IniciarMissao("rest_a1_entrar", "Entre no restaurante e sente-se em uma mesa", "Enter the restaurant and sit at a table");
//         await PlayAudioETexto(i++); // Welcome!
//         await PlayAudioETexto(i++); // Here is the menu.
//         ConcluirMissao("rest_a1_entrar");

//         IniciarMissao("rest_a1_pedido", "Olhe o menu e faça seu pedido", "Look at the menu and place your order");
//         await PlayAudioETexto(i++); // Are you ready to order?

//         exAtual = exerciciosSpeaking[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         await PlayAudioETexto(i++); // Anything to drink?

//         exAtual = exerciciosAlternativas[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         await PlayAudioETexto(i++); // Your order will be ready...
//         ConcluirMissao("rest_a1_pedido");

//         IniciarMissao("rest_a1_comer", "Coma a comida", "Eat your meal");
//         // O avanço para a próxima fala representa o retorno do usuário após a refeição.
//         await PlayAudioETexto(i++); // How can I help you?
//         ConcluirMissao("rest_a1_comer");

//         IniciarMissao("rest_a1_viagem", "Solicite a refeição para viagem", "Ask to have your meal packed to go");
//         exAtual = exerciciosCriarFrases[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         await PlayAudioETexto(i++); // Of course.
//         await PlayAudioETexto(i++); // Here it is.
//         ConcluirMissao("rest_a1_viagem");

//         IniciarMissao("rest_a1_pagamento", "Realize o pagamento", "Pay the bill");
//         exAtual = exerciciosSpeaking[1];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         await PlayAudioETexto(i++); // Bill amount and payment question.

//         await AbrirExercicio(ObterTipoExercicio(exerciciosAlternativas[1]), exerciciosAlternativas[1]);

//         exAtual = exerciciosSpeaking[2];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         ConcluirMissao("rest_a1_pagamento");
//         await AbrirExercicio(ObterTipoExercicio(exerciciosAssociacao[0]), exerciciosAssociacao[0]);
//     }

//     private async Task interacaoA2()
//     {
//         int i = 0;

//         IniciarMissao("rest_a2_entrar", "Entre no restaurante e sente-se em uma mesa", "Enter the restaurant and sit at a table");
//         await PlayAudioETexto(i++); // Welcome.
//         await PlayAudioETexto(i++); // Menu.
//         ConcluirMissao("rest_a2_entrar");

//         IniciarMissao("rest_a2_pedido", "Faça seu pedido ao garçom", "Place your order with the waiter");
//         await PlayAudioETexto(i++); // Are you ready to order?

//         exAtual = exerciciosCriarFrases[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         await PlayAudioETexto(i++); // Order ready in 15 minutes.
//         exAtual = exerciciosAlternativas[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);
//         ConcluirMissao("rest_a2_pedido");

//         IniciarMissao("rest_a2_comer", "Coma a comida", "Eat your meal");
//         await PlayAudioETexto(i++); // Food is served.
//         await PlayAudioETexto(i++); // Is everything okay?
//         ConcluirMissao("rest_a2_comer");

//         IniciarMissao("rest_a2_embalagem", "Peça uma embalagem para levar o resto para viagem", "Ask for a container to take the leftovers away");
//         await AbrirExercicio(ObterTipoExercicio(exerciciosCompletarFrase[0]), exerciciosCompletarFrase[0]);

//         await PlayAudioETexto(i++); // Of course. I'll bring it right away.
//         await AbrirExercicio(ObterTipoExercicio(exerciciosCompletarFrase[1]), exerciciosCompletarFrase[1]);
//         ConcluirMissao("rest_a2_embalagem");

//         IniciarMissao("rest_a2_pagamento", "Pague sua conta", "Pay your bill");
//         await PlayAudioETexto(i++); // Bill amount and payment question.

//         exAtual = exerciciosAlternativas[1];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         await PlayAudioETexto(i++); // Thank you.
//         ConcluirMissao("rest_a2_pagamento");

//         await AbrirExercicio(ObterTipoExercicio(exerciciosAssociacao[0]), exerciciosAssociacao[0]);
//     }

//     private async Task interacaoB1()
//     {
//         int i = 0;

//         IniciarMissao("rest_b1_entrar", "Entre no restaurante e sente-se em uma mesa", "Enter the restaurant and sit at a table");
//         await PlayAudioETexto(i++); // Welcome, menu and invitation to order.
//         ConcluirMissao("rest_b1_entrar");

//         IniciarMissao("rest_b1_perguntar_pratos", "Pergunte sobre os pratos disponíveis", "Ask about the dishes on the menu");
//         exAtual = exerciciosSpeaking[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         await PlayAudioETexto(i++); // Ingredients.
//         exAtual = exerciciosSpeaking[1];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         await PlayAudioETexto(i++); // Recommendation.
//         ConcluirMissao("rest_b1_perguntar_pratos");

//         IniciarMissao("rest_b1_pedido", "Faça seu pedido", "Place your order");
//         exAtual = exerciciosCriarFrases[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         await PlayAudioETexto(i++); // Anything else?
//         exAtual = exerciciosCriarFrases[1];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         await PlayAudioETexto(i++); // Order will be ready.
//         ConcluirMissao("rest_b1_pedido");

//         IniciarMissao("rest_b1_cliente", "Converse com o outro cliente", "Talk to the other customer");
//         await AguardarConversaComCliente();
//         // A conversa com o cliente conclui esta missão no RestaurantClient.

//         IniciarMissao("rest_b1_comer", "Coma sua comida", "Eat your meal");
//         await PlayAudioETexto(i++); // How was everything?
//         ConcluirMissao("rest_b1_comer");

//         IniciarMissao("rest_b1_viagem", "Solicite para levar o restante da refeição para viagem", "Ask to have the rest of your meal packed to go");
//         exAtual = exerciciosWriting[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         exAtual = exerciciosSpeaking[2];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         await PlayAudioETexto(i++); // Certainly. I'll be right back.
//         await PlayAudioETexto(i++); // Here is the bag.
//         ConcluirMissao("rest_b1_viagem");

//         IniciarMissao("rest_b1_pagamento", "Pague a conta", "Pay the bill");
//         exAtual = exerciciosSpeaking[3];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         await PlayAudioETexto(i++); // Bill amount and payment question.

//         exAtual = exerciciosSpeaking[4];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         await PlayAudioETexto(i++); // Thank you. Would you like a receipt?
//         await AbrirExercicio(ObterTipoExercicio(exerciciosAlternativas[0]), exerciciosAlternativas[0]);

//         ConcluirMissao("rest_b1_pagamento");
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

//     private async Task AguardarConversaComCliente()
//     {
//         conversaClienteConcluida = false;

//         // Aqui você pode colocar o garçom no balcão
//         // e iniciar a animação de preparação do pedido.

//         while (!conversaClienteConcluida)
//         {
//             await Task.Yield();
//         }

//         // Aqui você pode finalizar a animação
//         // e fazer o garçom voltar ao atendimento.
//     }

//     public void FinalizarConversaComCliente()
//     {
//         conversaClienteConcluida = true;
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

public class RestaurantWaiterNPC : InteracaoNPC
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

    private bool conversaClienteConcluida;

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
        nivelAtual = DadosJogador.nivelUsuario;
        nomeExibicaoLegenda = "Restaurant Waiter";
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
                "Welcome!",
                "Here is the menu.",
                "Are you ready to order?",
                "Anything to drink?",
                "OK. Your order will be ready in about fifteen minutes.",
                "How can I help you?",
                "Of course",
                "Here it is",
                "Sure. The bill is 30 dollars. Cash or card?"
            };

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "You want a hamburger. What do you say to the waiter?",
                        opcoesFala = new List<string>()
                        {
                            "I need to check out of my hotel\n",
                            "CI like order hamburger yesterday\n",
                            "I would like a hamburger, please\n"
                        },
                        respostaCorreta = 2
                    }
            );  

            exerciciosAlternativas.Add(
                    new ExercicioAlternativas()
                    {
                        enunciado = "Ask for some drink to the waiter",
                        alternativas = new List<string>() { 
                        "I would like a juice, please\r\n", 
                        "I would like a hamburger, please\r\n", 
                        "I would like the menu, please\r\n", 
                        "I would like the bill, please\r\n" },
                        alternativaCorreta = 0
                    }
            );  

            exerciciosCriarFrases.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Arrange the words to form the correct sentence in english: Posso levar isso para viagem?",
                    blocosPalavras = new List<string>
                    {
                        "this",
                        "Can",
                        "order",
                        "go",
                        "I",
                        "take",
                        "menu",
                        "to",
                        "eat",
                        "it",
                        "please",
                        "for",
                        "table",
                        "the"
                    },
                    respostaCorreta = "Can I take this to go?\n"
                }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "You want to pay. What do you say to the waiter?",
                        opcoesFala = new List<string>()
                        {
                            "Could I have some extra napkins, please?\n",
                            "Can I have the bill, please?\n",
                            "Is the soup served hot?\n"
                        },
                        respostaCorreta = 1
                    }
            );  

            exerciciosAlternativas.Add(
                    new ExercicioAlternativas()
                    {
                        enunciado = "The waiter asks how you want to pay. Choose the correct answer.",
                        alternativas = new List<string>() { 
                        "By card\r\n", 
                        "By table\r\n", 
                        "By menu\r\n", 
                        "By hamburger\r\n" },
                        alternativaCorreta = 0
                    }
            );  

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Say thank you to the waiter."
                    }
            );  

            exerciciosAssociacao.Add(
                new ExercicioAssociacao()
                {
                    enunciado = "Match the Portuguese word with the English word.",

                    opcoesPort = new List<string>
                    {
                        "cardápio",
                        "pedido",
                        "hambúrguer",
                        "suco",
                        "conta",
                        "pagar",
                        "água"
                    },

                    opcoesIngles = new List<string>
                    {
                        "water",
                        "menu",
                        "bill",
                        "hamburger",
                        "pay",
                        "order",
                        "juice"
                    },

                    associacaoCorreta = new List<ParAssociacao>
                    {
                        new ParAssociacao { palavraPortugues = "cardápio", palavraIngles = "menu" },
                        new ParAssociacao { palavraPortugues = "pedido", palavraIngles = "order" },
                        new ParAssociacao { palavraPortugues = "hambúrguer", palavraIngles = "hamburger" },
                        new ParAssociacao { palavraPortugues = "suco", palavraIngles = "juice" },
                        new ParAssociacao { palavraPortugues = "conta", palavraIngles = "bill" },
                        new ParAssociacao { palavraPortugues = "pagar", palavraIngles = "pay" },
                        new ParAssociacao { palavraPortugues = "água", palavraIngles = "water" }
                    }
                }
            );
        }
        else if(nivelAtual=="A2")
        {
            dialogoAtual = new List<string>()
            {
                cumprimento+". Welcome to our restaurant.",
                "Here is the menu. Take your time.",
                "Are you ready to order?",
                "Your order will be ready in about twenty minutes.",
                "Here is your hamburger and orange juice",
                "Is everything okay with your meal?",
                "Of course. I'll bring it right away.",
                "Sure. Your bill is $43. WWould you like to pay by card or cash?",
                "Thank you. I hope you enjoyed your meal. Come back soon!"
            };

            exerciciosCriarFrases.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Arrange the words to form the correct sentence in english: Eu gostaria de um hambúrguer e um suco de laranja, por favor",
                    blocosPalavras = new List<string>
                    {
                        "orange",
                        "I",
                        "please",
                        "sandwich",
                        "would",
                        "juice",
                        "like",
                        "a",
                        "hamburger",
                        "and",
                        "water",
                        "some",
                        "the",
                        "pizza",
                        "an"
                    },
                    respostaCorreta = "I would like a hamburger and an orange juice, please\n"
                }
            );

            exerciciosAlternativas.Add(
                    new ExercicioAlternativas()
                    {
                        enunciado = "How long will the order take?",
                        alternativas = new List<string>() { 
                        "5 minutes\r\n", 
                        "10 minutes\r\n", 
                        "15 minutes\r\n", 
                        "30 minutes\r\n" },
                        alternativaCorreta = 2
                    }
            ); 

            exerciciosCompletarFrase.Add(
                new ExercicioCompletarFrase()
                {
                    enunciado = "Complete the sentence with the correct words:",
                    partesFrase = new List<string>
                    {
                        "Could you ",
                        " up the ",
                        " of my burger, please?"
                    },
                    respostaCorreta = "wrap, rest",
                    opcoes = new List<string>
                    {
                        "everything",
                        "rest",
                        "ready",
                        "wrap",
                        "perfect",
                        "No",
                        "are",
                        "the",
                        "food",
                        "very",
                        "it"
                    }
                }
            );

            exerciciosCompletarFrase.Add(
                new ExercicioCompletarFrase()
                {
                    enunciado = "Complete the sentence with the correct words:",
                    partesFrase = new List<string>
                    {
                        "",
                        " I have the ",
                        ", please?"
                    },
                    respostaCorreta = "Could, bill",
                    opcoes = new List<string>
                    {
                        "bill",
                        "price",
                        "heat",
                        "menu",
                        "pack",
                        "cut",
                        "order",
                        "Could"
                    }
                }
            );

            exerciciosAlternativas.Add(
                    new ExercicioAlternativas()
                    {
                        enunciado = "Select the correct option to answer the waiter.",
                        alternativas = new List<string>() { 
                        "I'd like to pay by card, please\r\n", 
                        "I'd like to pay by table\r\n", 
                        "I'd like to pay by hamburger\r\n", 
                        "I'd like to pay by menu\r\n" },
                        alternativaCorreta = 0
                    }
            ); 

            exerciciosAssociacao.Add(
                new ExercicioAssociacao()
                {
                    enunciado = "Match the Portuguese word with the English word.",

                    opcoesPort = new List<string>
                    {
                        "acompanhamento",
                        "embalagem",
                        "talheres",
                        "pedido",
                        "restante",
                        "conta",
                        "dinheiro em espécie",
                        "troco"
                    },

                    opcoesIngles = new List<string>
                    {
                        "change",
                        "order",
                        "cash",
                        "side dish",
                        "package",
                        "bill",
                        "cutlery",
                        "leftovers"
                    },

                    associacaoCorreta = new List<ParAssociacao>
                    {
                        new ParAssociacao { palavraPortugues = "acompanhamento", palavraIngles = "side dish" },
                        new ParAssociacao { palavraPortugues = "embalagem", palavraIngles = "package" },
                        new ParAssociacao { palavraPortugues = "talheres", palavraIngles = "cutlery" },
                        new ParAssociacao { palavraPortugues = "pedido", palavraIngles = "order" },
                        new ParAssociacao { palavraPortugues = "restante", palavraIngles = "leftovers" },
                        new ParAssociacao { palavraPortugues = "conta", palavraIngles = "bill" },
                        new ParAssociacao { palavraPortugues = "dinheiro em espécie", palavraIngles = "cash" },
                        new ParAssociacao { palavraPortugues = "troco", palavraIngles = "change" }
                    }
                }
            );
        }
        else if(nivelAtual=="B1")
        {
            dialogoAtual = new List<string>()
            {
                cumprimento+". Welcome. Here is the menu. Let me know when you're ready to order.",
                "It comes with a beef patty, cheese, lettuce, tomato, and onion. You can also add bacon for 5 dollars.",
                "The haburger is our best-seller. Most customers love it.",
                "Anything else?",
                "Your order should be ready in about ten minutes. I'll bring it to you as soon as it's ready.",
                "How was everything? Did you enjoy your meal?",
                "Certainly. I'll be right back.",
                "Here is the bag with your food.",
                "Your bill is R$42. Would you like to pay by card or cash?",
                "Thank you very much. Would you like a receipt?"
            };

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "You are interested in the hambuger. Ask what comes with it.",
                    }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Ask the restaurant employee for a recommendation to help you choose a dish.",
                        opcoesFala = new List<string>()
                        {
                            "Why did you recommended this dish to me?\n",
                            "Could you tell me when the airport check-in desk opens?\n",
                            "Could you recommend a dish from the menu?\n"
                        },
                        respostaCorreta = 2
                    }
            );  

            exerciciosCriarFrases.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Arrange the words to form the correct sentence in english: Eu gostaria de pedir um hambúrguer acompanhado de um suco de limão, por favor.",
                    blocosPalavras = new List<string>
                    {
                        "lemon",
                        "would",
                        "meal",
                        "please",
                        "hamburger",
                        "to",
                        "some",
                        "I",
                        "by",
                        "juice",
                        "order",
                        "the",
                        "an",
                        "accompanied",
                        "drink",
                        "a",
                        "like",
                        "Could",
                        "with"
                    },
                    respostaCorreta = "I would like to order a hamburger, accompanied by an lemon juice, please.\n"
                }
            );

            exerciciosCriarFrases.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Arrange the words to form the correct sentence in english: Não, obrigado. Isso é tudo o que vou pedir por enquanto.",
                    blocosPalavras = new List<string>
                    {
                        "order",
                        "No",
                        "everything",
                        "for",
                        "I'll",
                        "thank",
                        "now",
                        "That's",
                        "please",
                        "you",
                        "all",
                        "the",
                        "would",
                        "too",
                        "much"
                    },
                    respostaCorreta = "No, thank you. That’s all I’ll order for now.\n"
                }
            );

            exerciciosCompletarFrase.Add(
                new ExercicioCompletarFrase()
                {
                    enunciado = "Complete the sentence with the correct words:",
                    partesFrase = new List<string>
                    {
                        "Your order should be ready in about ",
                        " ",
                        ", please?"
                    },
                    respostaCorreta = "20, minutes",
                    opcoes = new List<string>
                    {
                        "minutes",
                        "price",
                        "heat",
                        "menu",
                        "pack",
                        "20",
                        "cut",
                        "30",
                        "12",
                        "half",
                        "hour",
                        "ready"
                    }
                }
            );

            exerciciosWriting.Add(
                new ExercicioWriting()
                {
                    enunciado = "Compliment the burger to the waiter."
                }
            );

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "You couldn't finish your hamburger. Ask the waiter to wrap the rest for you."
                }
            );  

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "Ask the waiter for the bill politely so you can pay for your meal."
                }
            ); 

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "You are ready to pay for your meal. Choose the most appropriate way to tell the waiter how you would like to pay.",
                        opcoesFala = new List<string>()
                        {
                            "I’d prefer to pay by card, if that’s possible\n",
                            "I’ve already paid for my meal, so I don’t need the bill\n",
                            "Could I pay for the table next to the window, please?\n"
                        },
                        respostaCorreta = 0
                    }
            );

            exerciciosAlternativas.Add(
                    new ExercicioAlternativas()
                    {
                        enunciado = "Choose the most appropriate response to politely decline an additional offer after finishing your meal",
                        alternativas = new List<string>() { 
                        "I’d like to buy a coat that matches my shoes\r\n", 
                        "I usually take the bus to school because it’s more convenient\r\n", 
                        "She’s preparing dinner at home while her family sets the table\r\n", 
                        "No, thank you. Everything was delicious, and I really enjoyed the meal.\r\n" },
                        alternativaCorreta = 3
                    }
            );

            exerciciosAssociacao.Add(
                new ExercicioAssociacao()
                {
                    enunciado = "Match the Portuguese word or expression with its English equivalent.",

                    opcoesPort = new List<string>
                    {
                        "ingredientes",
                        "mais vendido",
                        "porção",
                        "saboroso",
                        "sobras",
                        "embrulhar para viagem",
                        "conta",
                        "pagar em dinheiro",
                        "recomendar",
                        "acompanhamento"
                    },

                    opcoesIngles = new List<string>
                    {
                        "bill",
                        "side dish",
                        "leftovers",
                        "ingredients",
                        "pay in cash",
                        "best-seller",
                        "wrap up",
                        "portion",
                        "recommend",
                        "delicious"
                    },

                    associacaoCorreta = new List<ParAssociacao>
                    {
                        new ParAssociacao { palavraPortugues = "ingredientes", palavraIngles = "ingredients" },
                        new ParAssociacao { palavraPortugues = "mais vendido", palavraIngles = "best-seller" },
                        new ParAssociacao { palavraPortugues = "porção", palavraIngles = "portion" },
                        new ParAssociacao { palavraPortugues = "saboroso", palavraIngles = "delicious" },
                        new ParAssociacao { palavraPortugues = "sobras", palavraIngles = "leftovers" },
                        new ParAssociacao { palavraPortugues = "embrulhar para viagem", palavraIngles = "wrap up" },
                        new ParAssociacao { palavraPortugues = "conta", palavraIngles = "bill" },
                        new ParAssociacao { palavraPortugues = "pagar em dinheiro", palavraIngles = "pay in cash" },
                        new ParAssociacao { palavraPortugues = "recomendar", palavraIngles = "recommend" },
                        new ParAssociacao { palavraPortugues = "acompanhamento", palavraIngles = "side dish" }
                    }
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

        if (RevisaoManager.Instance != null)
            await RevisaoManager.Instance.ExecutarRevisaoEExibirModalFinal(this);
        else
            Debug.LogError("RevisaoManager não está disponível para finalizar Restaurant.");
    }

    private async Task interacaoA1()
    {
        int i = 0;

        IniciarMissao("rest_a1_entrar", "Entre no restaurante e sente-se em uma mesa", "Enter the restaurant and sit at a table");
        await PlayAudioETexto(i++); // Welcome!
        await PlayAudioETexto(i++); // Here is the menu.
        ConcluirMissao("rest_a1_entrar");

        IniciarMissao("rest_a1_pedido", "Olhe o menu e faça seu pedido", "Look at the menu and place your order");
        await PlayAudioETexto(i++); // Are you ready to order?

        exAtual = exerciciosSpeaking[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++); // Anything to drink?

        exAtual = exerciciosAlternativas[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++); // Your order will be ready...
        ConcluirMissao("rest_a1_pedido");

        IniciarMissao("rest_a1_comer", "Coma a comida", "Eat your meal");
        // O avanço para a próxima fala representa o retorno do usuário após a refeição.
        await PlayAudioETexto(i++); // How can I help you?
        ConcluirMissao("rest_a1_comer");

        IniciarMissao("rest_a1_viagem", "Solicite a refeição para viagem", "Ask to have your meal packed to go");
        exAtual = exerciciosCriarFrases[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++); // Of course.
        await PlayAudioETexto(i++); // Here it is.
        ConcluirMissao("rest_a1_viagem");

        IniciarMissao("rest_a1_pagamento", "Realize o pagamento", "Pay the bill");
        exAtual = exerciciosSpeaking[1];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++); // Bill amount and payment question.

        await AbrirExercicio(ObterTipoExercicio(exerciciosAlternativas[1]), exerciciosAlternativas[1]);

        exAtual = exerciciosSpeaking[2];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        ConcluirMissao("rest_a1_pagamento");
        await AbrirExercicio(ObterTipoExercicio(exerciciosAssociacao[0]), exerciciosAssociacao[0]);
    }

    private async Task interacaoA2()
    {
        int i = 0;

        IniciarMissao("rest_a2_entrar", "Entre no restaurante e sente-se em uma mesa", "Enter the restaurant and sit at a table");
        await PlayAudioETexto(i++); // Welcome.
        await PlayAudioETexto(i++); // Menu.
        ConcluirMissao("rest_a2_entrar");

        IniciarMissao("rest_a2_pedido", "Faça seu pedido ao garçom", "Place your order with the waiter");
        await PlayAudioETexto(i++); // Are you ready to order?

        exAtual = exerciciosCriarFrases[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++); // Order ready in 15 minutes.
        exAtual = exerciciosAlternativas[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);
        ConcluirMissao("rest_a2_pedido");

        IniciarMissao("rest_a2_comer", "Coma a comida", "Eat your meal");
        await PlayAudioETexto(i++); // Food is served.
        await PlayAudioETexto(i++); // Is everything okay?
        ConcluirMissao("rest_a2_comer");

        IniciarMissao("rest_a2_embalagem", "Peça uma embalagem para levar o resto para viagem", "Ask for a container to take the leftovers away");
        await AbrirExercicio(ObterTipoExercicio(exerciciosCompletarFrase[0]), exerciciosCompletarFrase[0]);

        await PlayAudioETexto(i++); // Of course. I'll bring it right away.
        await AbrirExercicio(ObterTipoExercicio(exerciciosCompletarFrase[1]), exerciciosCompletarFrase[1]);
        ConcluirMissao("rest_a2_embalagem");

        IniciarMissao("rest_a2_pagamento", "Pague sua conta", "Pay your bill");
        await PlayAudioETexto(i++); // Bill amount and payment question.

        exAtual = exerciciosAlternativas[1];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++); // Thank you.
        ConcluirMissao("rest_a2_pagamento");

        await AbrirExercicio(ObterTipoExercicio(exerciciosAssociacao[0]), exerciciosAssociacao[0]);
    }

    private async Task interacaoB1()
    {
        int i = 0;

        IniciarMissao("rest_b1_entrar", "Entre no restaurante e sente-se em uma mesa", "Enter the restaurant and sit at a table");
        await PlayAudioETexto(i++); // Welcome, menu and invitation to order.
        ConcluirMissao("rest_b1_entrar");

        IniciarMissao("rest_b1_perguntar_pratos", "Pergunte sobre os pratos disponíveis", "Ask about the dishes on the menu");
        exAtual = exerciciosSpeaking[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++); // Ingredients.
        exAtual = exerciciosSpeaking[1];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++); // Recommendation.
        ConcluirMissao("rest_b1_perguntar_pratos");

        IniciarMissao("rest_b1_pedido", "Faça seu pedido", "Place your order");
        exAtual = exerciciosCriarFrases[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++); // Anything else?
        exAtual = exerciciosCriarFrases[1];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++); // Order will be ready.
        ConcluirMissao("rest_b1_pedido");

        IniciarMissao("rest_b1_cliente", "Converse com o outro cliente", "Talk to the other customer");
        await AguardarConversaComCliente();
        // A conversa com o cliente conclui esta missão no RestaurantClient.

        IniciarMissao("rest_b1_comer", "Coma sua comida", "Eat your meal");
        await PlayAudioETexto(i++); // How was everything?
        ConcluirMissao("rest_b1_comer");

        IniciarMissao("rest_b1_viagem", "Solicite para levar o restante da refeição para viagem", "Ask to have the rest of your meal packed to go");
        exAtual = exerciciosWriting[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        exAtual = exerciciosSpeaking[2];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++); // Certainly. I'll be right back.
        await PlayAudioETexto(i++); // Here is the bag.
        ConcluirMissao("rest_b1_viagem");

        IniciarMissao("rest_b1_pagamento", "Pague a conta", "Pay the bill");
        exAtual = exerciciosSpeaking[3];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++); // Bill amount and payment question.

        exAtual = exerciciosSpeaking[4];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++); // Thank you. Would you like a receipt?
        await AbrirExercicio(ObterTipoExercicio(exerciciosAlternativas[0]), exerciciosAlternativas[0]);

        ConcluirMissao("rest_b1_pagamento");
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

    private async Task AguardarConversaComCliente()
    {
        conversaClienteConcluida = false;

        // Aqui você pode colocar o garçom no balcão
        // e iniciar a animação de preparação do pedido.

        while (!conversaClienteConcluida)
        {
            await Task.Yield();
        }

        // Aqui você pode finalizar a animação
        // e fazer o garçom voltar ao atendimento.
    }

    public void FinalizarConversaComCliente()
    {
        conversaClienteConcluida = true;
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
