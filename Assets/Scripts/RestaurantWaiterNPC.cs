using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using TMPro;
using System;

public class SecretaryCollegeNPC: InteracaoNPC
{

    string nivelAtual;
    string cumrpimento;
    int horaAtual = DateTime.Now.Hour;

    public string idNpcParaVoz = "recepcionistHotel";

    public void Start(){
        nivelAtual = DadosJogador.nivelUsuario;
        nomeExibicaoLegenda = "Restaurant Waiter";

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
                    blocosPlavras = new List<string>
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
                "ould you like something to drink with that?",
                "Your order will be ready in about twenty minutes.",
                "Is everything okay with your meal?",
                "Of course. I'll bring it right away.",
                "Sure. Your bill is $43. WWould you like to pay by card or cash?",
                "Thank you. I hope you enjoyed your meal. Come back soon!"
            };

            exerciciosCriarFrases.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Arrange the words to form the correct sentence in english: Eu gostaria de um hambúrguer e um suco de laranja, por favor",
                    blocosPlavras = new List<string>
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

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "You don't need anything more. What do you say?",
                        opcoesFala = new List<string>()
                        {
                            "No, that’s all for now, thank you\n",
                            "I would like to cancel the order I haven't placed yet and leave without paying.\n",
                            " I haven't decided what to order. Could I see the menu?\n"
                        },
                        respostaCorreta = 0
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

            exerciciosCompletarFrases.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Complete the sentence with the correct word: Could you ____ up the ___ of my burger, please?",
                    blocosPlavras = new List<string>
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
                        "it",
                    },
                    respostaCorreta = "wrap, rest\n"
                }
            );

            exerciciosCompletarFrases.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Complete the sentence with the correct word: _____ I have the ____, please?",
                    blocosPlavras = new List<string>
                    {
                        "bill",
                        "price",
                        "heat",
                        "menu",
                        "pack",
                        "cut",
                        "order",
                        "Could"
                    },
                    respostaCorreta = "Could, bill\n"
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
                "That's our best-seller. Most customers love it.",
                "Would you like anything to drink, or any side dishes with that?",
                "Your order should be ready in about ten minutes. I'll bring it to you as soon as it's ready.",
                "How was everything? Did you enjoy your meal?",
                "Certainly. I'll be right back.",
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
                    enunciado= "Arrange the words to form the correct sentence in english: Eu gostaria de pedir um hambúrguer acompanhado de um suco de laranja, por favor.",
                    blocosPlavras = new List<string>
                    {
                        "orange",
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
                    respostaCorreta = "I would like to order a hamburger, accompanied by an orange juice, please.\n"
                }
            );

            exerciciosCriarFrases.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Arrange the words to form the correct sentence in english: Não, obrigado. Isso é tudo o que vou pedir por enquanto.",
                    blocosPlavras = new List<string>
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

            exerciciosCompletarFrases.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Complete the sentence with the correct word: Your order should be ready in about __ _______, please?",
                    blocosPlavras = new List<string>
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
                    },
                    respostaCorreta = "20, minutes\n"
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
}
