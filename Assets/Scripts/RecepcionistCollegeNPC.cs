using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using TMPro;
using System;

public class RecepcionistCollegeNPC: InteracaoNPC
{

    string nivelAtual;
    string cumrpimento;
    int horaAtual = DateTime.Now.Hour;

    public string idNpcParaVoz = "recepcionistCollege";

    public void Start(){
        nivelAtual = DadosJogador.nivelUsuario;
        nomeExibicaoLegenda = "College Recepcionist";

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
                "Hello. Can I help you?",
                "The secretary's office is on the first floor",
                "Go straight and turn right."
            };

            exerciciosCriarFrases.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Arrange the words to form the correct sentence in english: Onde fica o escritório da secretária?",
                    blocosPlavras = new List<string>
                    {
                        "Where",
                        "located",
                        "manager",
                        "is",
                        "secretary's",
                        "meeting",
                        "office",
                        "How",
                        "floor",
                        "the",
                        "reception",
                        "does",
                        "find",
                        "When"
                    },
                    respostaCorreta = "Where is the secretary's office located?\n"
                }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "How do you ask for directions for secretary's office?",
                        opcoesFala = new List<string>()
                        {
                            "How do I get there?\n",
                            "Where can I find the student cafeteria?\n",
                            "Is the library open on Sundays?\n"
                        },
                        respostaCorreta = 0
                    }
            );

            exerciciosAlternativas.Add(
                new ExercicioAlternativas()
                {
                    enunciado = "Choose the correct option to express thanks",
                    alternativas = new List<string>(){ 
                        "I am a student\r\n", 
                        "It is a library\r\n", 
                        "Thank you\r\n", 
                        "My class is at eight\r\n" },
                    alternativaCorreta = 2
                }
            );
        }
        else if(nivelAtual=="A2")
        {
            dialogoAtual = new List<string>()
            {
                cumprimento+". Welcome to the university. How can I help you?",
                "The secretary's office is on the first floor. Go straight and take the first door on your right.",
                "You're welcome. Have a nice day!" 
            };

            exerciciosCriarFrases.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Arrange the words to form the correct sentence in english: Eu gostaria de encontrar o escritório da secretária.",
                    blocosPlavras = new List<string>
                    {
                        "I",
                        "library",
                        "secretary's",
                        "to",
                        "find",
                        "the",
                        "would",
                        "office",
                        "classroom",
                        "teacher",
                        "Where",
                        "student",
                        "is",
                        "like",
                    },
                    respostaCorreta = "I would like to find the secretary's office.\n"
                }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "You did not understand the directions. Ask the receptionist to repeat them.",
                        opcoesFala = new List<string>()
                        {
                            "Where can I buy a notebook?\n",
                            "What time does the library close?\n",
                            "Could you repeat the directions, please?\n"
                        },
                        respostaCorreta = 2
                    }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "The secretary helped you. Thank her and say goodbye"
                    }
            );
        }
        else if(nivelAtual=="B1")
        {
            dialogoAtual = new List<string>()
            {
                cumprimento+". Welcome to the university. How may I help you today?",
                "Are you a new student?",
                "The secretary's office is on the first floor. Go straight past the reception desk and you'll see a door on your right. The secretary can help you with enrollment inside.",
                "If you get lost, just come back and I'll help you."
            };

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "The receptionist asks how she can help you. Explain that you need to find the secretary’s office and say why you are there.",
                        opcoesFala = new List<string>()
                        {
                            " I need to eat the secretary's office because I enroll yesterday.\n",
                            "I need to go to the secretary's office because I have to enroll in a course. \n",
                            "I am a course at the second floor.\n"
                        },
                        respostaCorreta = 1
                    }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "This is your first visit to the university. Answer her question.",
                        opcoesFala = new List<string>()
                        {
                            "No, this is my first time here. \n",
                            "I need to enroll in Computer Science.\n",
                            "The library closes at six o’clock.\n"
                        },
                        respostaCorreta = 0
                    }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "You are not sure which door is the secretary's office. Ask the receptionist for clarification.",
                        opcoesFala = new List<string>()
                        {
                            "Excuse me, could you tell me what time the secretary’s office opens?\n",
                            "Excuse me, I don’t know where is the secretary’s office door\n",
                            "Sorry, could you repeat which door is the secretary’s office?\n"
                        },
                        respostaCorreta = 2
                    }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "The secretary has helped you with everything you needed. Thank her for her help and end the conversation politely."
                    }
            );
        }
    }
}
