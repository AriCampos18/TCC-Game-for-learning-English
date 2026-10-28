using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using TMPro;
using System;

public class RecepcionistHotelNPC: InteracaoNPC
{

    string nivelAtual;
    string cumrpimento, periodo, periodoIng;
    int horaAtual = DateTime.Now.Hour;

    public string idNpcParaVoz = "recepcionistHotel";

    public List<ExercicioBase> exerciciosCompletarFrase;
    public List<ExercicioBase> exerciciosCriarFrases;
    public List<ExercicioBase> exerciciosSpeaking;
    public List<ExercicioBase> exerciciosAlternativas;
    public List<ExercicioBase> exerciciosWriting;

    public List<ExercicioBase> exerciciosAssociacao;

    public void Start(){
        nivelAtual = DadosJogador.nivelUsuario;
        nomeExibicaoLegenda = "Hotel Recepcionist";

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
        string dataCheckout = ObterDataCheckout();
        if (nivelAtual == "A1")
        {
            dialogoAtual = new List<string>()
            {
                "Hello. Can I help you?",
                "What is yout name?",
                "Cash or card?",
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
                    blocosPlavras = new List<string>
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
                        associacaoCorreta = new ParAssociacao { palavraPortugues = "reserva", palavraIngles = "reservation" },
                        new ParAssociacao { palavraPortugues = "nome", palavraIngles = "name" },
                        new ParAssociacao { palavraPortugues = "cartão", palavraIngles = "card" },
                        new ParAssociacao { palavraPortugues = "quarto", palavraIngles = "room" },
                        new ParAssociacao { palavraPortugues = "andar", palavraIngles = "floor" },
                        new ParAssociacao { palavraPortugues = "café da manhã", palavraIngles = "breakfast" },
                        new ParAssociacao { palavraPortugues = "saída", palavraIngles = "checkout" },
                        new ParAssociacao { palavraPortugues = "semana", palavraIngles = "week" }
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
                    blocosPlavras = new List<string>
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
                    enunciado = "Say how is your name"
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
                    enunciado = "The receptionist asks for the name on the reservation. Write your answer in a complete sentence."
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
                new ExercicioBlocos()
                {
                    enunciado= "Complete the sentence with the correct word: I need to leave at ____ p.m.",
                    blocosPlavras = new List<string>
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
                    },
                    respostaCorreta = "12:00\n"
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
                        associacaoCorreta = new ParAssociacao { palavraPortugues = "noite", palavraIngles = "night" },
                        new ParAssociacao { palavraPortugues = "hospedagem", palavraIngles = "stay" },
                        new ParAssociacao { palavraPortugues = "pagar", palavraIngles = "pay" },
                        new ParAssociacao { palavraPortugues = "cartão do quarto", palavraIngles = "room card" },
                        new ParAssociacao { palavraPortugues = "perguntar", palavraIngles = "ask" },
                        new ParAssociacao { palavraPortugues = "bagagem", palavraIngles = "luggage" },
                        new ParAssociacao { palavraPortugues = "sair", palavraIngles = "leave" },
                        new ParAssociacao { palavraPortugues = "recepção", palavraIngles = "reception desk" }
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
                "The elevators is just around the corner, next to the restaurant.",
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
                    enunciado= "Arrange the words to form the correct sentence in english: Onde fica o elevador?"+periodo,
                    blocosPlavras = new List<string>
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
                        "10:00 a.m\r\n", 
                        "12:00 p.m\r\n" },
                    alternativaCorreta = 3
                }
            );

            exerciciosWriting.Add(
                new ExercicioWriting()
                {
                    enunciado = "Write a sentece asking about the time of the checkout."
                }
            );

            exerciciosSpeaking.Add(
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
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "You have a flight at 5:00 p.m. Ask the recepcionist if you can checkout later. ",
                    }
            );

            exerciciosCompletarFrase.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Complete the sentence with the correct word: The late chekout is available until _____ p.m.",
                    blocosPlavras = new List<string>
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
                    },
                    respostaCorreta = "2:00\n"
                }
            );

            exerciciosSpeaking.Add(
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
            );

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
                        },
                        associacaoCorreta = new ParAssociacao { palavraPortugues = "reservar", palavraIngles = "book" },
                        new ParAssociacao { palavraPortugues = "quarto de casal", palavraIngles = "double room" },
                        new ParAssociacao { palavraPortugues = "ao lado de", palavraIngles = "next to" },
                        new ParAssociacao { palavraPortugues = "servido", palavraIngles = "served" },
                        new ParAssociacao { palavraPortugues = "com antecedência", palavraIngles = "in advance" },
                        new ParAssociacao { palavraPortugues = "voo", palavraIngles = "flight" },
                        new ParAssociacao { palavraPortugues = "sair", palavraIngles = "leave" },
                        new ParAssociacao { palavraPortugues = "disponibilidade", palavraIngles = "availability" }
                    }
            );
        }
    }

    private string ObterDataCheckout()
    {
        DateTime dataCheckout = DateTime.Now.Date.AddDays(7);

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
}
