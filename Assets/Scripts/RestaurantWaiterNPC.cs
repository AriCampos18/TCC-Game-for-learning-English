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
using System.Linq;

public class RestaurantWaiterNPC : InteracaoNPC, IRoteiroNpcCheckpoint
{
    string nivelAtual;
    string cumprimento;
    int horaAtual = DateTime.Now.Hour;
    List<string> dialogoAtual;
    private const string NpcKey = MissoesRestaurant.Garcom;
    private InteracaoRestaurant mesaAtual;
    private int ultimaVisita;
    private int VisitaAtual => MissionManager.Instance?.VisitaRestaurant ?? 0;

    protected override bool PermitirReinteracaoAposConclusao => mesaAtual != null;

    protected override bool PodeIniciarComDialogo => mesaAtual != null;

    public override bool PodeInteragir() => mesaAtual != null && VisitaAtual == ultimaVisita && base.PodeInteragir();

    internal bool PodeIniciarAtendimento() =>
        isActiveAndEnabled && PertenceAoCenarioAtivo && !GameProgress.EstaEmDialogo && VisitaAtual != 0 && ultimaVisita != VisitaAtual && mesaAtual == null;

    internal async void IniciarAtendimento(InteracaoRestaurant mesa)
    {
        if (!PodeIniciarAtendimento() || mesa == null)
            return;

        bool atendimentoConcluido = false;
        try
        {
            mesa.Sentar();
            mesaAtual = mesa;
            if (IdentificarMesa(mesa, MesasCheckpoint()) == null)
                Debug.LogWarning("Restaurant: configure chaves de checkpoint únicas nas mesas do garçom.", mesa);
            ultimaVisita = VisitaAtual;
            Emitir("marco_fluxo", "mesa_sentou");
            GameProgress.EstaEmDialogo = true;
            await Interagir();
            atendimentoConcluido = true;
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception e)
        {
            Debug.LogError($"Restaurant: {e.Message}", this);
        }
        finally
        {
            if (mesaAtual != null)
            {
                mesaAtual.Encerrar();
                mesaAtual = null;
                if (!atendimentoConcluido)
                {
                    GameProgress.EstaEmDialogo = false;
                    LiberarControlePlayer();
                }
            }
        }
    }
    private ExerciseLoadResult catalogoExercicios;
    public ModalExercicio modalExercicio;
    public GameObject modalLegenda;
    public TextMeshProUGUI textoPularDialogo;
    public ExercicioBase exAtual;

    private bool aguardandoConversaComCliente;
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
        if (audioSourceComum == null)
        {
            audioSourceComum = gameObject.AddComponent<AudioSource>();
            audioSourceComum.playOnAwake = false;
        }

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
                        modoLivre = true,
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
                        "20 minutes\r\n", 
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
                    respostasCorretas = new List<string> { "wrap", "rest" },
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
                    respostasCorretas = new List<string> { "Could", "bill" },
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
                "The hamburger is our best-seller. Most customers love it.",
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
                        modoLivre = true,
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
                    respostaCorreta = "No, thank you. That's all I'll order for now.\n"
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
                        "."
                    },
                    respostasCorretas = new List<string> { "10", "minutes" },
                    opcoes = new List<string>
                    {
                        "minutes",
                        "price",
                        "heat",
                        "menu",
                        "pack",
                        "10",
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
                    modoLivre = true,
                    enunciado = "You couldn't finish your hamburger. Ask the waiter to wrap the rest for you."
                }
            );  

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    modoLivre = true,
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
        else if (nivelAtual == "A1-A2")
{
    dialogoAtual = new List<string>()
    {
        cumprimento + " Welcome to our restaurant!",
        "Here is the menu. You can take your time.",
        "Are you ready to order?",
        "Would you like something to drink?",
        "Your hamburger and orange juice will be ready in about fifteen minutes.",
        "How is your meal? Is everything okay?",
        "Of course. I can bring you a container.",
        "Here is your food to go.",
        "Your bill is 35 dollars. Would you like to pay by card or cash?",
        "Thank you for coming. Have a great day!"
    };

    exerciciosSpeaking.Add(new ExercicioSpeaking()
    {
        enunciado = "You would like to order a hamburger. Tell the waiter what you want.",
        opcoesFala = new List<string>()
        {
            "I would like the bill, please.\n",
            "I would like to book a hotel room.\n",
            "I would like a hamburger, please.\n"
        },
        respostaCorreta = 2
    });

    exerciciosAlternativas.Add(new ExercicioAlternativas()
    {
        enunciado = "The waiter asks if you would like something to drink. Choose the best answer.",
        alternativas = new List<string>()
        {
            "I'd like a table, please.\r\n",
            "I'd like an orange juice, please.\r\n",
            "I'd like another menu, please.\r\n",
            "I'd like the restaurant, please.\r\n"
        },
        alternativaCorreta = 1
    });

    exerciciosCriarFrases.Add(new ExercicioBlocos()
    {
        enunciado = "Arrange the words to form the correct sentence in English: Você poderia trazer uma embalagem para levar o restante, por favor?",
        blocosPalavras = new List<string>()
        {
            "container",
            "Could",
            "the",
            "bring",
            "please",
            "you",
            "me",
            "rest",
            "for",
            "a",
            "bill",
            "my"
        },
        respostaCorreta = "Could you bring me a container for the rest, please?\n"
    });

    exerciciosSpeaking.Add(new ExercicioSpeaking()
    {
        enunciado = "You have finished eating. Ask the waiter for the bill.",
        opcoesFala = new List<string>()
        {
            "Could I have the menu, please?\n",
            "Could I have another hamburger, please?\n",
            "Could I have the bill, please?\n"
        },
        respostaCorreta = 2
    });

    exerciciosAlternativas.Add(new ExercicioAlternativas()
    {
        enunciado = "The waiter asks how you would like to pay. Choose the correct answer.",
        alternativas = new List<string>()
        {
            "I'd like to pay by menu, please.\r\n",
            "I'd like to pay by hamburger, please.\r\n",
            "I'd like to pay by table, please.\r\n",
            "I'd like to pay by card, please.\r\n"
        },
        alternativaCorreta = 3
    });

    exerciciosSpeaking.Add(new ExercicioSpeaking()
    {
        modoLivre = true,
        enunciado = "Thank the waiter politely."
    });

    exerciciosAssociacao.Add(new ExercicioAssociacao()
    {
        enunciado = "Match the Portuguese word with its English equivalent.",
        opcoesPort = new List<string>()
        {
            "garçom",
            "cardápio",
            "pedido",
            "bebida",
            "embalagem",
            "conta",
            "dinheiro"
        },
        opcoesIngles = new List<string>()
        {
            "bill",
            "waiter",
            "drink",
            "cash",
            "menu",
            "container",
            "order"
        },
        associacaoCorreta = new List<ParAssociacao>()
        {
            new ParAssociacao { palavraPortugues = "garçom", palavraIngles = "waiter" },
            new ParAssociacao { palavraPortugues = "cardápio", palavraIngles = "menu" },
            new ParAssociacao { palavraPortugues = "pedido", palavraIngles = "order" },
            new ParAssociacao { palavraPortugues = "bebida", palavraIngles = "drink" },
            new ParAssociacao { palavraPortugues = "embalagem", palavraIngles = "container" },
            new ParAssociacao { palavraPortugues = "conta", palavraIngles = "bill" },
            new ParAssociacao { palavraPortugues = "dinheiro", palavraIngles = "cash" }
        }
    });
}
else if (nivelAtual == "A2-B1")
{
    dialogoAtual = new List<string>()
    {
        cumprimento + " Welcome to our restaurant. Here is the menu.",
        "Our hamburger is very popular. It comes with cheese, lettuce, tomato, and fries.",
        "Is there anything else you would like to order?",
        "Your order should be ready in about twenty minutes.",
        "How was your meal? Was everything to your liking?",
        "Certainly. I can pack the remaining food for you.",
        "Here is your order. I hope you enjoy it later!",
        "Your bill is R$42. Would you prefer to pay by card or in cash?",
        "Here is your receipt. Thank you for visiting us!"
    };

    exerciciosSpeaking.Add(new ExercicioSpeaking()
    {
        enunciado = "You want to know what comes with the restaurant's hamburger. Ask the waiter.",
        opcoesFala = new List<string>()
        {
            "Could you tell me where the airport is?\n",
            "Could you tell me when the restaurant closes yesterday?\n",
            "Could you tell me what comes with the hamburger?\n"
        },
        respostaCorreta = 2
    });

    exerciciosCriarFrases.Add(new ExercicioBlocos()
    {
        enunciado = "Arrange the words to form the correct sentence in English: Eu gostaria de pedir um hambúrguer com queijo e batatas fritas, por favor.",
        blocosPalavras = new List<string>()
        {
            "fries",
            "would",
            "cheese",
            "please",
            "hamburger",
            "I",
            "with",
            "like",
            "and",
            "to",
            "order",
            "a",
            "salad",
            "some"
        },
        respostaCorreta = "I would like to order a hamburger with cheese and fries, please.\n"
    });

    exerciciosCriarFrases.Add(new ExercicioBlocos()
    {
        enunciado = "Arrange the words to form the correct sentence in English: Isso é tudo por enquanto, obrigado.",
        blocosPalavras = new List<string>()
        {
            "That's",
            "for",
            "thank",
            "all",
            "now",
            "you",
            "everything",
            "much",
            "order",
            "No",
            "please"
        },
        respostaCorreta = "That's all for now, thank you.\n"
    });

    exerciciosCompletarFrase.Add(new ExercicioCompletarFrase()
    {
        enunciado = "Complete the sentence with the correct words:",
        partesFrase = new List<string>()
        {
            "Could you ",
            " the rest of my meal, please?"
        },
        respostaCorreta = "pack",
        opcoes = new List<string>()
        {
            "order",
            "pay",
            "serve",
            "choose",
            "pack",
            "recommend"
        }
    });

    exerciciosWriting.Add(new ExercicioWriting()
    {
        enunciado = "You enjoyed your meal. Write a short message to compliment the waiter on the food and service."
    });

    exerciciosSpeaking.Add(new ExercicioSpeaking()
    {
        modoLivre = true,
        enunciado = "Ask the waiter for the bill and explain that you are ready to pay."
    });

    exerciciosAlternativas.Add(new ExercicioAlternativas()
    {
        enunciado = "The waiter asks whether you would prefer to pay by card or in cash. Choose the most appropriate answer.",
        alternativas = new List<string>()
        {
            "I'd prefer to order another table, if that's possible.\r\n",
            "I'd prefer to take the menu home, if that's possible.\r\n",
            "I'd prefer to change my meal yesterday, if that's possible.\r\n",
            "I'd prefer to pay by card, if that's possible.\r\n"
        },
        alternativaCorreta = 3
    });

    exerciciosAssociacao.Add(new ExercicioAssociacao()
    {
        enunciado = "Match the Portuguese word or expression with its English equivalent.",
        opcoesPort = new List<string>()
        {
            "ingredientes",
            "acompanhamento",
            "recomendar",
            "sobras",
            "embalar",
            "atendimento",
            "recibo",
            "dinheiro em espécie"
        },
        opcoesIngles = new List<string>()
        {
            "receipt",
            "leftovers",
            "service",
            "ingredients",
            "cash",
            "side dish",
            "pack",
            "recommend"
        },
        associacaoCorreta = new List<ParAssociacao>()
        {
            new ParAssociacao { palavraPortugues = "ingredientes", palavraIngles = "ingredients" },
            new ParAssociacao { palavraPortugues = "acompanhamento", palavraIngles = "side dish" },
            new ParAssociacao { palavraPortugues = "recomendar", palavraIngles = "recommend" },
            new ParAssociacao { palavraPortugues = "sobras", palavraIngles = "leftovers" },
            new ParAssociacao { palavraPortugues = "embalar", palavraIngles = "pack" },
            new ParAssociacao { palavraPortugues = "atendimento", palavraIngles = "service" },
            new ParAssociacao { palavraPortugues = "recibo", palavraIngles = "receipt" },
            new ParAssociacao { palavraPortugues = "dinheiro em espécie", palavraIngles = "cash" }
        }
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

    private static void Emitir(string tipo, string alvo) =>
        MissionManager.Instance?.RegistrarEventoRestaurant(tipo, alvo);

    protected override async Task IniciarInteracao()
    {
        nivelAtual = DadosJogador.nivelUsuario;
        try
        {
            InicializarConteudosPorNivel();
            catalogoExercicios = await ExerciseManager.Instance.LoadForNpcAsync(NpcKey, nivelAtual);
            if (!catalogoExercicios.Success)
                Debug.LogWarning("RestaurantWaiterNPC: usando exercícios locais. " + catalogoExercicios.Error);

            await CriarRoteiro(SessaoCenario.Atual.Versao).Executar();
        }
        finally
        {
            aguardandoConversaComCliente = false;
            if (modalLegenda != null)
                modalLegenda.SetActive(false);
            if (textoPularDialogo != null)
                textoPularDialogo.gameObject.SetActive(false);
        }

        mesaAtual.Encerrar();
        GameProgress.EstaEmDialogo = false;
        LiberarControlePlayer();
        if (RevisaoManager.Instance != null)
            await RevisaoManager.Instance.ExecutarRevisaoEExibirModalFinal(this);
        else
            Debug.LogError("RevisaoManager não está disponível para finalizar Restaurant.");
    }

    private async Task LevantarParaCliente()
    {
        await AguardarSentado(InteracaoRestaurant.Etapa.Levantar);
        Emitir("marco_fluxo", "mesa_levantou");
    }

    private async Task ConversarComCliente()
    {
        await AguardarConversaComCliente();
        Emitir("conversa_encerrada", MissoesRestaurant.Cliente);
    }

    private async Task RetornarParaMesa()
    {
        LiberarControlePlayer();
        try
        {
            await mesaAtual.Aguardar(InteracaoRestaurant.Etapa.Retornar);
        }
        finally
        {
            TravarControlePlayer();
        }
        Emitir("marco_fluxo", "mesa_retornou");
    }

    private async Task AguardarSentado(InteracaoRestaurant.Etapa etapa)
    {
        DefinirCursorDesktop(false);
        if (movimentoPlayer != null)
            movimentoPlayer.PermitirOlharSemMover(true);
        try
        {
            await mesaAtual.Aguardar(etapa);
        }
        finally
        {
            if (movimentoPlayer != null)
                movimentoPlayer.PermitirOlharSemMover(false);
        }
    }

    private async Task Comer()
    {
        await AguardarSentado(InteracaoRestaurant.Etapa.Comer);
        await mesaAtual.SimularRefeicao();
        Emitir("marco_fluxo", "refeicao_concluida");
    }

    private InteracaoRestaurant[] MesasCheckpoint() =>
        FindObjectsByType<InteracaoRestaurant>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .Where(mesa => mesa.PertenceAoGarcom(this)).ToArray();

    private static string IdentificarMesa(InteracaoRestaurant mesa, InteracaoRestaurant[] mesas)
    {
        if (mesa == null || !mesas.Contains(mesa))
            return null;
        string chave = mesa.ChaveCheckpoint;
        if (chave != null)
            return mesas.Count(m => m.ChaveCheckpoint == chave) == 1 ? "table.key." + chave : null;
        return mesas.Length == 1 ? "table.single" : null;
    }

    Task<Func<Task>> IRoteiroNpcCheckpoint.PrepararRetomada(Newtonsoft.Json.Linq.JObject visita,
        int versao, System.Threading.CancellationToken token) => PrepararRoteiroRestaurado(visita, versao, token);

    private async Task<Func<Task>> PrepararRoteiroRestaurado(Newtonsoft.Json.Linq.JObject visita,
        int versao, System.Threading.CancellationToken token)
    {
        ValidarVisita(versao);
        AtualizarVisita();
        nivelAtual = SessaoCenario.Atual.Nivel;
        cumprimento = RoteiroNpcCheckpoint.SaudacaoSalva(visita);
        InicializarConteudosPorNivel();
        var roteiro = CriarRoteiro(versao);
        var cursor = roteiro.Validar(visita);
        if (cursor.Situacao.Length != 3 || (cursor.Situacao[2] != "table.seated" && cursor.Situacao[2] != "table.standing"))
            throw new System.IO.InvalidDataException("Estado da mesa inválido.");
        string etapa = (string)visita["safe_steps"][1];
        bool estavaEmPe = etapa == "action.table.customer" || etapa == "action.table.return";
        if (estavaEmPe != (cursor.Situacao[2] == "table.standing"))
            throw new System.IO.InvalidDataException("A posição da mesa diverge da etapa do atendimento.");
        var mesas = MesasCheckpoint();
        var mesa = mesas.SingleOrDefault(m => IdentificarMesa(m, mesas) == cursor.Situacao[1]);
        if (mesa == null || !mesa.isActiveAndEnabled)
            throw new System.IO.InvalidDataException("A mesa do checkpoint está indisponível ou tem chave duplicada.");

        catalogoExercicios = await ExerciseManager.Instance.LoadForNpcAsync(NpcKey, nivelAtual, token);
        token.ThrowIfCancellationRequested();
        ValidarVisita(versao);
        if (!catalogoExercicios.Success)
            throw new System.IO.InvalidDataException("Catálogo de Restaurant indisponível para retomar o atendimento.");

        return async () =>
        {
            ValidarVisita(versao);
            ultimaVisita = VisitaAtual;
            interacaoConcluida = true;
            mesaAtual = mesa;
            GameProgress.EstaEmDialogo = true;
            TravarControlePlayer();
            try
            {
                mesa.RetomarPosicao(cursor.Situacao[2] == "table.seated");
                await roteiro.Executar(cursor.Indice);
            }
            finally
            {
                if (SessaoCenario.Atual.Permite(Cenario, versao))
                {
                    aguardandoConversaComCliente = false;
                    mesa.Encerrar();
                    mesaAtual = null;
                    GameProgress.EstaEmDialogo = false;
                    if (modalLegenda != null) modalLegenda.SetActive(false);
                    if (textoPularDialogo != null) textoPularDialogo.gameObject.SetActive(false);
                    LiberarControlePlayer();
                }
            }
            if (SessaoCenario.Atual.Permite(Cenario, versao) && RevisaoManager.Instance != null)
                await RevisaoManager.Instance.ExecutarRevisaoEExibirModalFinal(this);
        };
    }

    internal string[] EstadoMesaDuranteCliente()
    {
        string chave = IdentificarMesa(mesaAtual, MesasCheckpoint());
        string saudacao = RoteiroNpcCheckpoint.ChaveSaudacao(cumprimento);
        return aguardandoConversaComCliente && chave != null && saudacao != null && !mesaAtual.Sentado
            ? new[] { saudacao, chave, "table.standing", "parent." + CriarRoteiro(SessaoCenario.Atual.Versao).Identidade }
            : null;
    }

    internal async Task<(Action Posicionar, Func<Task> Continuar)> PrepararRetomadaDoCliente(
        Newtonsoft.Json.Linq.JObject visita, string[] estado, int versao, System.Threading.CancellationToken token)
    {
        if (estado.Length != 4 || estado[2] != "table.standing")
            throw new System.IO.InvalidDataException("Estado da mesa durante a conversa com o cliente inválido.");
        AtualizarVisita();
        nivelAtual = SessaoCenario.Atual.Nivel;
        var pai = (Newtonsoft.Json.Linq.JObject)visita.DeepClone();
        pai.Remove("line_key");
        pai.Remove("next_exercise");
        pai["safe_steps"] = new Newtonsoft.Json.Linq.JArray("script", "step", estado[0]);
        cumprimento = RoteiroNpcCheckpoint.SaudacaoSalva(pai);
        InicializarConteudosPorNivel();
        var roteiro = CriarRoteiro(versao);
        if (estado[3] != "parent." + roteiro.Identidade)
            throw new System.IO.InvalidDataException("O roteiro do garçom mudou desde a conversa com o cliente.");
        pai["safe_steps"] = roteiro.CursorDaAcao("table.return", estado.Take(3).ToArray());
        var mesas = MesasCheckpoint();
        var mesa = mesas.SingleOrDefault(m => IdentificarMesa(m, mesas) == estado[1]);
        var retorno = await PrepararRoteiroRestaurado(pai, versao, token);
        return (() =>
        {
            ValidarVisita(versao);
            mesa.RetomarPosicao(false);
        }, async () =>
        {
            ValidarVisita(versao);
            Emitir("conversa_encerrada", MissoesRestaurant.Cliente);
            await ExecutarRoteiroRetomado(versao, retorno);
        });
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
        var exerciciosWriting = descricao?.exerciciosWriting ?? this.exerciciosWriting;
        var exerciciosAssociacao = descricao?.exerciciosAssociacao ?? this.exerciciosAssociacao;

        var mesas = MesasCheckpoint();
        var roteiro = new RoteiroNpcCheckpoint(this, visita, () =>
        {
            string mesa = IdentificarMesa(mesaAtual, mesas);
            string saudacao = RoteiroNpcCheckpoint.ChaveSaudacao(cumprimento);
            return mesa == null || saudacao == null ? null
                : new[] { saudacao, mesa, mesaAtual.Sentado ? "table.seated" : "table.standing" };
        });
        void Fala(int indice) => roteiro.Fala(dialogoAtual[indice], () => PlayAudioETexto(indice));
        void Exercicio(string ponto, int ordem, ExercicioBase fallback) =>
            roteiro.Exercicio(ponto, ordem, () => AbrirExercicioDoPonto(ponto, ordem, fallback), modelo: fallback);
        void Cliente()
        {
            roteiro.Acao("table.stand", LevantarParaCliente);
            roteiro.Acao("table.customer", ConversarComCliente);
            roteiro.Acao("table.return", RetornarParaMesa);
        }
        void Refeicao() => roteiro.Acao("table.meal", Comer);

        switch (nivelAtual)
        {
            case "A1":
                Fala(0);
                Fala(1);
                Fala(2);
                Exercicio("meal_order", 1, exerciciosSpeaking[0]);
                Fala(3);
                Exercicio("drink_order", 1, exerciciosAlternativas[0]);
                Fala(4);
                Refeicao();
                Fala(5);
                Exercicio("takeaway", 1, exerciciosCriarFrases[0]);
                Fala(6);
                Fala(7);
                Exercicio("bill_request", 1, exerciciosSpeaking[1]);
                Fala(8);
                Exercicio("payment", 1, exerciciosAlternativas[1]);
                Exercicio("farewell", 1, exerciciosSpeaking[2]);
                Exercicio("restaurant_vocabulary", 1, exerciciosAssociacao[0]);
                break;
            case "A2":
                Fala(0);
                Fala(1);
                Fala(2);
                Exercicio("meal_order", 1, exerciciosCriarFrases[0]);
                Fala(3);
                Exercicio("preparation_time", 1, exerciciosAlternativas[0]);
                Fala(4);
                Refeicao();
                Fala(5);
                Exercicio("takeaway", 1, exerciciosCompletarFrase[0]);
                Fala(6);
                Exercicio("bill_request", 1, exerciciosCompletarFrase[1]);
                Fala(7);
                Exercicio("payment", 1, exerciciosAlternativas[1]);
                Fala(8);
                Exercicio("restaurant_vocabulary", 1, exerciciosAssociacao[0]);
                break;
            case "B1":
                Fala(0);
                Exercicio("menu_details", 1, exerciciosSpeaking[0]);
                Fala(1);
                Exercicio("menu_details", 2, exerciciosSpeaking[1]);
                Fala(2);
                Exercicio("meal_order", 1, exerciciosCriarFrases[0]);
                Fala(3);
                Exercicio("order_closing", 1, exerciciosCriarFrases[1]);
                Fala(4);
                Exercicio("preparation_time", 1, exerciciosCompletarFrase[0]);
                Cliente();
                Refeicao();
                Fala(5);
                Exercicio("meal_feedback", 1, exerciciosWriting[0]);
                Exercicio("takeaway", 1, exerciciosSpeaking[2]);
                Fala(6);
                Fala(7);
                Exercicio("bill_request", 1, exerciciosSpeaking[3]);
                Fala(8);
                Exercicio("payment", 1, exerciciosSpeaking[4]);
                Fala(9);
                Exercicio("farewell", 1, exerciciosAlternativas[0]);
                Exercicio("restaurant_vocabulary", 1, exerciciosAssociacao[0]);
                break;
            case "A1-A2":
                Fala(0);
                Fala(1);
                Fala(2);
                Exercicio("meal_order", 1, exerciciosSpeaking[0]);
                Fala(3);
                Exercicio("drink_order", 1, exerciciosAlternativas[0]);
                Fala(4);
                Refeicao();
                Fala(5);
                Exercicio("takeaway", 1, exerciciosCriarFrases[0]);
                Fala(6);
                Fala(7);
                Exercicio("bill_request", 1, exerciciosSpeaking[1]);
                Fala(8);
                Exercicio("payment", 1, exerciciosAlternativas[1]);
                Fala(9);
                Exercicio("farewell", 1, exerciciosSpeaking[2]);
                Exercicio("restaurant_vocabulary", 1, exerciciosAssociacao[0]);
                break;
            case "A2-B1":
                Fala(0);
                Exercicio("menu_details", 1, exerciciosSpeaking[0]);
                Fala(1);
                Exercicio("meal_order", 1, exerciciosCriarFrases[0]);
                Fala(2);
                Exercicio("order_closing", 1, exerciciosCriarFrases[1]);
                Fala(3);
                Cliente();
                Refeicao();
                Fala(4);
                Exercicio("takeaway", 1, exerciciosCompletarFrase[0]);
                Fala(5);
                Fala(6);
                Exercicio("meal_feedback", 1, exerciciosWriting[0]);
                Exercicio("bill_request", 1, exerciciosSpeaking[1]);
                Fala(7);
                Exercicio("payment", 1, exerciciosAlternativas[0]);
                Fala(8);
                Exercicio("restaurant_vocabulary", 1, exerciciosAssociacao[0]);
                break;
        }
        return roteiro;
    }

    private async Task AbrirExercicioDoPonto(string pointKey, int ordem, ExercicioBase fallback)
    {
        int visita = SessaoCenario.Atual.Versao;
        if (pointKey == "restaurant_vocabulary")
            CheckpointJogo.Instance?.CapturarAssociacaoFinal(this, pointKey, ordem);
        CheckpointJogo.IdentificarFallback(fallback, pointKey, ordem, nivelAtual);
        TipoExercicio tipo = ObterTipoExercicio(fallback);
        bool? speakingLivre = (fallback as ExercicioSpeaking)?.modoLivre;
        ExerciseCatalogItem item = catalogoExercicios?.Find(pointKey, nivelAtual, ordem, tipo, speakingLivre);
        if (item != null)
            await AbrirExercicio(item.Tipo, item.Exercise);
        else
        {
            Debug.LogWarning($"RestaurantWaiterNPC: usando exercício local em {nivelAtual}/{pointKey}/{ordem}.");
            await AbrirExercicio(tipo, fallback);
        }

        ValidarVisita(visita);
        Emitir("exercicio_encerrado", $"{NpcKey}/{pointKey}/{ordem}");
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

    public bool PodeReceberConversaDoCliente(string nivel)
    {
        return aguardandoConversaComCliente && !conversaClienteConcluida
            && nivel == nivelAtual && (nivel == "A2-B1" || nivel == "B1");
    }

    private async Task AguardarConversaComCliente()
    {
        conversaClienteConcluida = false;
        aguardandoConversaComCliente = true;
        GameProgress.EstaEmDialogo = false;
        LiberarControlePlayer();

        while (!conversaClienteConcluida)
        {
            await Task.Yield();
            if (this == null || !isActiveAndEnabled || VisitaAtual != ultimaVisita)
                throw new OperationCanceledException();
        }

        aguardandoConversaComCliente = false;
        GameProgress.EstaEmDialogo = true;
        TravarControlePlayer();
    }

    public void FinalizarConversaComCliente()
    {
        if (PertenceAoCenarioAtivo && VisitaAtual == ultimaVisita && aguardandoConversaComCliente)
            conversaClienteConcluida = true;
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
