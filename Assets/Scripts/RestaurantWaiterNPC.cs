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

            exerciciosCompletarFrase.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Complete the sentence with the correct word: Could you ____ up the ___ of my burger, please?",
                    blocosPalavras = new List<string>
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

            exerciciosCompletarFrase.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Complete the sentence with the correct word: _____ I have the ____, please?",
                    blocosPalavras = new List<string>
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
                    blocosPalavras = new List<string>
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
                new ExercicioBlocos()
                {
                    enunciado= "Complete the sentence with the correct word: Your order should be ready in about __ _______, please?",
                    blocosPalavras = new List<string>
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

    protected override async Task IniciarInteracao()
    {
        GameProgress.EstaEmDialogo = true;
        try
        {
            nivelAtual = DadosJogador.nivelUsuario;
            InicializarConteudosPorNivel();

            if (dialogoAtual == null || dialogoAtual.Count == 0)
                throw new InvalidOperationException("O diálogo do NPC não foi inicializado para o nível atual.");

            string idPrimeiraMissao = "restaurant_" + nivelAtual.ToLowerInvariant() + "_01";
            MissionManager.AdicionarMissaoNPC(
                idPrimeiraMissao,
                "Enter the restaurant and sit at a table",
                "Entre no restaurante e sente-se em uma mesa"
            );

            if (nivelAtual == "A1")
                await interacaoA1();
            else if (nivelAtual == "A2")
                await interacaoA2();
            else
                await interacaoB1();
        }
        finally
        {
            GameProgress.EstaEmDialogo = false;
            if (modalLegendaComum != null)
                modalLegendaComum.SetActive(false);
            if (textoPularDialogoComum != null)
                textoPularDialogoComum.gameObject.SetActive(false);
            LiberarControlePlayer();
        }
    }



    private List<ExercicioBase> ObterFilaExercicios()
    {
        List<ExercicioBase> fila = new List<ExercicioBase>();
        fila.AddRange(exerciciosSpeaking);
        fila.AddRange(exerciciosAlternativas);
        fila.AddRange(exerciciosCompletarFrase);
        fila.AddRange(exerciciosCriarFrases);
        fila.AddRange(exerciciosWriting);
        fila.AddRange(exerciciosAssociacao);
        return fila;
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

    public async Task AbrirExercicio(TipoExercicio tipo, ExercicioBase exercicio)
    {
        Debug.Log("Abrindo exerc\u00edcio: " + tipo);
        string enunciado = exercicio.enunciado;
        bool pedidoA1 = nivelAtual == "A1" && enunciado.Contains("You want a hamburger");
        bool levarA1 = nivelAtual == "A1" && enunciado.Contains("Posso levar isso para viagem");
        bool pagamentoA1 = nivelAtual == "A1" && enunciado.Contains("how you want to pay");
        bool pedidoA2 = nivelAtual == "A2" && enunciado.Contains("Eu gostaria de um hambúrguer e um suco de laranja");
        bool levarA2 = nivelAtual == "A2" && enunciado.Contains("wrap up the");
        bool pagamentoA2 = nivelAtual == "A2" && enunciado.Contains("Select the correct option to answer the waiter");
        bool perguntaPratoB1 = nivelAtual == "B1" && enunciado.Contains("You are interested in the hambuger");
        bool recomendacaoB1 = nivelAtual == "B1" && enunciado.Contains("Ask the restaurant employee for a recommendation");
        bool pedidoB1 = nivelAtual == "B1" && enunciado.Contains("Eu gostaria de pedir um hambúrguer");
        bool comerB1 = nivelAtual == "B1" && enunciado.Contains("Compliment the burger");
        bool levarB1 = nivelAtual == "B1" && enunciado.Contains("wrap the rest for you");
        bool contaB1 = nivelAtual == "B1" && enunciado.Contains("Ask the waiter for the bill politely");
        bool pagamentoB1 = nivelAtual == "B1" && enunciado.Contains("how you would like to pay");

        if (pedidoA1)
        {
            MissionManager.ConcluirMissaoNPC("restaurant_a1_01");
            MissionManager.AdicionarMissaoNPC(
                "restaurant_a1_02",
                "Look at the menu and place your order",
                "Olhe o menu e faça seu pedido"
            );
        }

        if (pedidoA2)
        {
            MissionManager.ConcluirMissaoNPC("restaurant_a2_01");
            MissionManager.AdicionarMissaoNPC(
                "restaurant_a2_02",
                "Place your order with the waiter",
                "Faça seu pedido ao garçom"
            );
        }

        if (perguntaPratoB1)
        {
            MissionManager.ConcluirMissaoNPC("restaurant_b1_01");
            MissionManager.AdicionarMissaoNPC(
                "restaurant_b1_02",
                "Ask the waiter about the dishes",
                "Pergunte sobre pratos para o garçom"
            );
        }

        if (levarA1)
        {
            MissionManager.ConcluirMissaoNPC("restaurant_a1_03");
            MissionManager.AdicionarMissaoNPC(
                "restaurant_a1_04",
                "Ask to have the meal packed to take away",
                "Solicite a refeição para viagem"
            );
        }

        if (pagamentoA1)
        {
            MissionManager.AdicionarMissaoNPC(
                "restaurant_a1_05",
                "Pay for your meal",
                "Realize o pagamento"
            );
        }

        if (levarA2)
        {
            MissionManager.ConcluirMissaoNPC("restaurant_a2_03");
            MissionManager.AdicionarMissaoNPC(
                "restaurant_a2_04",
                "Ask for a container to take the leftovers away",
                "Peça uma embalagem para levar o resto para viagem"
            );
        }

        if (pagamentoA2)
        {
            MissionManager.AdicionarMissaoNPC(
                "restaurant_a2_05",
                "Pay your bill",
                "Pague sua conta"
            );
        }

        if (comerB1)
        {
            MissionManager.AdicionarMissaoNPC(
                "restaurant_b1_05",
                "Eat your food",
                "Coma sua comida"
            );
        }

        if (levarB1)
        {
            MissionManager.ConcluirMissaoNPC("restaurant_b1_05");
            MissionManager.AdicionarMissaoNPC(
                "restaurant_b1_06",
                "Ask to take your food away",
                "Solicite para levar para viagem"
            );
        }

        if (contaB1 || pagamentoB1)
        {
            MissionManager.AdicionarMissaoNPC(
                "restaurant_b1_07",
                "Pay the bill",
                "Pague a conta"
            );
        }

        ModalLegenda legenda = modalLegendaComum.GetComponent<ModalLegenda>();

        if (modalLegendaComum != null)
        {
            modalLegendaComum.SetActive(false);
        }

        await EntrarModoExercicio();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        modalExercicioComum.Abrir(tipo, exercicio, this);

        if (legenda != null)
        {
            legenda.AjustarPosicaoPeloEstadoDoJogo();
        }

        while (!modalExercicioComum.exercicioFinalizado)
        {
            await Task.Yield();
        }

        if (pedidoA1)
        {
            MissionManager.ConcluirMissaoNPC("restaurant_a1_02");
            MissionManager.AdicionarMissaoNPC(
                "restaurant_a1_03",
                "Eat your food",
                "Coma a comida"
            );
        }

        if (levarA1)
        {
            MissionManager.ConcluirMissaoNPC("restaurant_a1_04");
            MissionManager.AdicionarMissaoNPC(
                "restaurant_a1_05",
                "Pay for your meal",
                "Realize o pagamento"
            );
        }

        if (pagamentoA1)
            MissionManager.ConcluirMissaoNPC("restaurant_a1_05");

        if (pedidoA2)
        {
            MissionManager.ConcluirMissaoNPC("restaurant_a2_02");
            MissionManager.AdicionarMissaoNPC(
                "restaurant_a2_03",
                "Eat your food",
                "Coma a comida"
            );
        }

        if (levarA2)
        {
            MissionManager.ConcluirMissaoNPC("restaurant_a2_04");
            MissionManager.AdicionarMissaoNPC(
                "restaurant_a2_05",
                "Pay your bill",
                "Pague sua conta"
            );
        }

        if (pagamentoA2)
            MissionManager.ConcluirMissaoNPC("restaurant_a2_05");

        if (recomendacaoB1)
        {
            MissionManager.ConcluirMissaoNPC("restaurant_b1_02");
            MissionManager.AdicionarMissaoNPC(
                "restaurant_b1_03",
                "Place your order",
                "Faça seu pedido"
            );
        }

        if (pedidoB1)
        {
            MissionManager.ConcluirMissaoNPC("restaurant_b1_03");
            MissionManager.AdicionarMissaoNPC(
                "restaurant_b1_04",
                "Talk to the other customer",
                "Converse com o outro cliente"
            );
        }

        if (comerB1)
        {
            MissionManager.ConcluirMissaoNPC("restaurant_b1_05");
            MissionManager.AdicionarMissaoNPC(
                "restaurant_b1_06",
                "Ask to take your food away",
                "Solicite para levar para viagem"
            );
        }

        if (levarB1)
        {
            MissionManager.ConcluirMissaoNPC("restaurant_b1_06");
            MissionManager.AdicionarMissaoNPC(
                "restaurant_b1_07",
                "Pay the bill",
                "Pague a conta"
            );
        }

        if (pagamentoB1)
            MissionManager.ConcluirMissaoNPC("restaurant_b1_07");

        if (modalLegendaComum != null)
        {
            modalLegendaComum.SetActive(false);
        }

        await SairModoExercicio();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

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
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            await Task.Delay(150);

            while (Input.GetMouseButton(0))
            {
                await Task.Yield();
            }

            bool clicou = false;
            while (!clicou)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    clicou = true;
                }
                else
                {
                    await Task.Yield();
                }
            }

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

    private async Task interacaoA1()
    {
        List<ExercicioBase> filaExercicios = ObterFilaExercicios();
        int indiceExercicio = 0;

        for (int i = 0; i < dialogoAtual.Count; i++)
        {
            await PlayAudioETexto(i);

            int dialogosRestantes = dialogoAtual.Count - i;
            int exerciciosRestantes = filaExercicios.Count - indiceExercicio;
            int exerciciosNesteTrecho =
                (exerciciosRestantes + dialogosRestantes - 1) / dialogosRestantes;

            for (int j = 0; j < exerciciosNesteTrecho; j++)
            {
                ExercicioBase exercicio = filaExercicios[indiceExercicio++];
                await AbrirExercicio(ObterTipoExercicio(exercicio), exercicio);
            }
        }
    }

    private async Task interacaoA2()
    {
        List<ExercicioBase> filaExercicios = ObterFilaExercicios();
        int indiceExercicio = 0;

        for (int i = 0; i < dialogoAtual.Count; i++)
        {
            await PlayAudioETexto(i);

            int dialogosRestantes = dialogoAtual.Count - i;
            int exerciciosRestantes = filaExercicios.Count - indiceExercicio;
            int exerciciosNesteTrecho =
                (exerciciosRestantes + dialogosRestantes - 1) / dialogosRestantes;

            for (int j = 0; j < exerciciosNesteTrecho; j++)
            {
                ExercicioBase exercicio = filaExercicios[indiceExercicio++];
                await AbrirExercicio(ObterTipoExercicio(exercicio), exercicio);
            }
        }
    }

    private async Task interacaoB1()
    {
        List<ExercicioBase> filaExercicios = ObterFilaExercicios();
        int indiceExercicio = 0;

        for (int i = 0; i < dialogoAtual.Count; i++)
        {
            await PlayAudioETexto(i);

            int dialogosRestantes = dialogoAtual.Count - i;
            int exerciciosRestantes = filaExercicios.Count - indiceExercicio;
            int exerciciosNesteTrecho =
                (exerciciosRestantes + dialogosRestantes - 1) / dialogosRestantes;

            for (int j = 0; j < exerciciosNesteTrecho; j++)
            {
                ExercicioBase exercicio = filaExercicios[indiceExercicio++];
                await AbrirExercicio(ObterTipoExercicio(exercicio), exercicio);
            }
        }
    }

    public override async Task FalarFraseCustomizada(string textoParaFalar)
    {
        if (string.IsNullOrEmpty(textoParaFalar)) return;

        byte[] audioBytes = await backendManagerComum.GerarAudio(textoParaFalar, idNpcParaVoz);

        if (audioBytes != null && audioBytes.Length > 0)
        {
            string caminho = Path.Combine(Application.persistentDataPath, "audio_temp_custom.wav");
            File.WriteAllBytes(caminho, audioBytes);

            UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + caminho, AudioType.WAV);
            var operation = www.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();

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
        }
    }
}
