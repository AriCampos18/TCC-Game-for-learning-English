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

    public string idNpcParaVoz = "mom";

    public List<ExercicioBase> exerciciosCompletarFrase;
    public List<ExercicioBase> exerciciosCriarFrases;
    public List<ExercicioBase> exerciciosSpeaking;
    public List<ExercicioBase> exerciciosAlternativas;
    public List<ExercicioBase> exerciciosWriting;

    public List<ExercicioBase> exerciciosAssociacao;

    public void Start(){
        nivelAtual = DadosJogador.nivelUsuario;
        nomeExibicaoLegenda = "Mom";
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
                "How is work going?",
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
                    blocosPlavras = new List<string>
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
                "Hey! It's been a while. How have you been?",
                "We made a few changes to the kitchen while you were away. What do you think?",
                "It's been good. My team is working on an interesting project, so the days go by quickly.",
                "Dinner is ready, everyone. Come to the table, please!",
                "It was really nice having you here."
            };

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Tell your family how you have been feeling lately and mention something about your recent routine",
                        opcoesFala = new List<string>()
                        {
                            "I would like to order a hamburger and a soda\n",
                            "Could you tell me where the airport check-in desk is?\n",
                            "I've been doing well, and I've been enjoying my new routine\n"
                        },
                        respostaCorreta = 2
                    }
            );

            exerciciosWriting.Add(
                new ExercicioWriting()
                {
                    enunciado = "Write a short message complimenting your family on the new kitchen and describing what you like about it"
                }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Ask your mother about his recent experiences at work",
                        opcoesFala = new List<string>()
                        {
                            "How have things been going at work, Mom?\n",
                            "Have you had a chance to buy any new clothes lately?\n",
                            "What kind of work does Mom usually do?\n"
                        },
                        respostaCorreta = 0
                    }
            );
        }
    }
}
