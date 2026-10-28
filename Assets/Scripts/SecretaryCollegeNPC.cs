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

    public string idNpcParaVoz = "secretaryCollege";

    public List<ExercicioBase> exerciciosCompletarFrase;
    public List<ExercicioBase> exerciciosCriarFrases;
    public List<ExercicioBase> exerciciosSpeaking;
    public List<ExercicioBase> exerciciosAlternativas;
    public List<ExercicioBase> exerciciosWriting;

    public List<ExercicioBase> exerciciosAssociacao;

    public void Start(){
        nivelAtual = DadosJogador.nivelUsuario;
        nomeExibicaoLegenda = "College Secretary";

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
                "Hi. Can I help you?",
                "What is your name?",
                "What course do you want?",
                "What is your date of birth?",
                "Your registration is done.",
                "It is R$500 per month.",
                "Classes start on February 10th.",
                "Your classroom is room 204. It is on the second floor."
            };

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "You are at the college. You want to sign up for a course. What do you say?",
                        opcoesFala = new List<string>()
                        {
                            "I enroll yesterday at the library\n",
                            "I want to enroll\n",
                            "I want to eat my enrollment\n"
                        },
                        respostaCorreta = 1
                    }
            );

            exerciciosWriting.Add(
                new ExercicioWriting()
                {
                    enunciado = "The secretary asks your name. Write your answer."
                }
            );

            exerciciosWriting.Add(
                new ExercicioWriting()
                {
                    enunciado = "The secretary asks what course you want. Write your answer."
                }
            );

            exerciciosWriting.Add(
                new ExercicioWriting()
                {
                    enunciado = "The secretary asks your date of birth. Write your answer."
                }
            );

            exerciciosCriarFrases.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Arrange the words to form the correct sentence in english: Qual é o valor da mensalidade?",
                    blocosPlavras = new List<string>
                    {
                        "What",
                        "tuition",
                        "expensive",
                        "is",
                        "monthly",
                        "cost",
                        "fee",
                        "much",
                        "course",
                        "How",
                        "semester",
                        "pay",
                        "the",
                        "does"
                    },
                    respostaCorreta = "What is the monthly fee?\n"
                }
            );

            exerciciosCriarFrases.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Arrange the words to form the correct sentence in english: Quando as aulas começam?",
                    blocosPlavras = new List<string>
                    {
                        "do",
                        "classes",
                        "exam",
                        "enrollment",
                        "schedule",
                        "where",
                        "begin",
                        "the",
                        "When",
                        "Does",
                        "finish",
                        "students",
                        "start",
                        "semester"
                    },
                    respostaCorreta = "When do classes start?\n"
                }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "You can’t find your class. Ask the secretary.",
                        opcoesFala = new List<string>()
                        {
                            "Where yesterday the stairs?\n",
                            "How much does the hallway cost?\n",
                            "Where is my classroom?\n"
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
                        "escritório",
                        "andar",
                        "nome",
                        "curso",
                        "estudante",
                        "sala de aula",
                        "mensalidade",
                        "aula"
                    },

                    opcoesIngles = new List<string>
                    {
                        "class",
                        "name",
                        "office",
                        "student",
                        "floor",
                        "classroom",
                        "tuition",
                        "course"
                    },

                    associacaoCorreta = new List<ParAssociacao>
                    {
                        new ParAssociacao { palavraPortugues = "escritório", palavraIngles = "office" },
                        new ParAssociacao { palavraPortugues = "andar", palavraIngles = "floor" },
                        new ParAssociacao { palavraPortugues = "nome", palavraIngles = "name" },
                        new ParAssociacao { palavraPortugues = "curso", palavraIngles = "course" },
                        new ParAssociacao { palavraPortugues = "estudante", palavraIngles = "student" },
                        new ParAssociacao { palavraPortugues = "sala de aula", palavraIngles = "classroom" },
                        new ParAssociacao { palavraPortugues = "mensalidade", palavraIngles = "tuition" },
                        new ParAssociacao { palavraPortugues = "aula", palavraIngles = "class" }
                    }
                }
            );
        }
        else if(nivelAtual=="A2")
        {
            dialogoAtual = new List<string>()
            {
                cumprimento+". Do you need help with something?",
                "Could I have your name, please?",
                "Which course would you like to study?",
                "What is your date of birth?",
                "Thank you. Your registration is complete. You are now enrolled in Computer Science.",
                "The tuition is R$500 per month.",
                "Classes start on February fifth.",
                "Your classroom is room two hundred four, on the second floor."
            };

            exerciciosCriarFrases.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Arrange the words to form the correct sentence in english: Gostaria de me matricular na universidade.",
                    blocosPlavras = new List<string>
                    {
                        "university",
                        "study",
                        "would",
                        "school",
                        "at",
                        "enroll",
                        "student",
                        "I",
                        "course",
                        "the",
                        "want",
                        "like",
                        "register",
                        "to",
                        "class"
                    },
                    respostaCorreta = "I would like to enroll at the university.\n"
                }
            );

            exerciciosWriting.Add(
                new ExercicioWriting()
                {
                    enunciado = "The secretary asks your name. Write your answer in a complete sentence."
                }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "You want to join the Computer Science course. What do you tell the secretary?",
                        opcoesFala = new List<string>()
                        {
                            "I would like to study a hamburger.\n",
                            "I would like to study Computer Science.\n",
                            "I liked study Computer Science.\n"
                        },
                        respostaCorreta = 1
                    }
            );

            exerciciosWriting.Add(
                new ExercicioWriting()
                {
                    enunciado = "The secretary asks your date of birth. Write your answer in a complete sentence, with the month written out (for example, -May 10th-). "
                }
            );

            exerciciosWriting.Add(
                new ExercicioWriting()
                {
                    enunciado = "You want to know how much the course costs each month. Write a question for the secretary. "
                }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "You want to know the date when your classes begin. What do you ask the secretary?",
                        opcoesFala = new List<string>()
                        {
                            "Which course should I choose?\n",
                            "Where can I pay the tuition?\n",
                            "When do classes start?\n"
                        },
                        respostaCorreta = 2
                    }
            );

            exerciciosAlternativas.Add(
                new ExercicioAlternativas()
                {
                    enunciado = "What is the start date for classes?",
                    alternativas = new List<string>(){ 
                        "February 1st\r\n", 
                        "February 5th\r\n", 
                        "February 10th\r\n", 
                        "February 20th\r\n" },
                    alternativaCorreta = 2
                }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "You need to find your classroom, but you don’t know where it is. Ask the secretary where it is.",
                        opcoesFala = new List<string>()
                        {
                            "Where is my classroom?\n",
                            "When does the library open?\n",
                            "How much is the tuition?\n"
                        },
                        respostaCorreta = 0
                    }
            );

            exerciciosAlternativas.Add(
                new ExercicioAlternativas()
                {
                    enunciado = "Select the correct option for your classroom number, as stated by the secretary.",
                    alternativas = new List<string>(){ 
                        "Room 102\r\n", 
                        "Room 204\r\n", 
                        "Room 301\r\n", 
                        "Room 405\r\n" },
                    alternativaCorreta = 1
                }
            );

            exerciciosAssociacao.Add(
                new ExercicioAssociacao()
                {
                    enunciado = "Match the Portuguese word with the English word.",

                    opcoesPort = new List<string>
                    {
                        "matricular-se",
                        "matrícula",
                        "direções",
                        "repetir",
                        "escadas",
                        "biblioteca",
                        "data de nascimento",
                        "cronograma"
                    },

                    opcoesIngles = new List<string>
                    {
                        "library",
                        "enroll",
                        "schedule",
                        "directions",
                        "date of birth",
                        "stairs",
                        "registration",
                        "repeat"
                    },

                    associacaoCorreta = new List<ParAssociacao>
                    {
                        new ParAssociacao { palavraPortugues = "matricular-se", palavraIngles = "enroll" },
                        new ParAssociacao { palavraPortugues = "matrícula", palavraIngles = "registration" },
                        new ParAssociacao { palavraPortugues = "direções", palavraIngles = "directions" },
                        new ParAssociacao { palavraPortugues = "repetir", palavraIngles = "repeat" },
                        new ParAssociacao { palavraPortugues = "escadas", palavraIngles = "stairs" },
                        new ParAssociacao { palavraPortugues = "biblioteca", palavraIngles = "library" },
                        new ParAssociacao { palavraPortugues = "data de nascimento", palavraIngles = "date of birth" },
                        new ParAssociacao { palavraPortugues = "cronograma", palavraIngles = "schedule" }
                    }
                }
            );
        }
        else if(nivelAtual=="B1")
        {
            dialogoAtual = new List<string>()
            {
                cumprimento+". Are you here to enroll in a course?",
                "Could you give me your full name, please?",
                "Could you also give me your date of birth?",
                "Thank you. I just need to confirm a few details. You are enrolling in Computer Science, correct?",
                "Your enrollment is complete. Your tuition will be 500 dollars per month.",
                "Classes start on February fifteenth in room 325.",
                "Room 325 is on the third floor, just go up the stairs. Good luck with your classes!"
            };

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "The secretary asks if you are here to enroll in a course. You are, and you need her help. What do you say?",
                        opcoesFala = new List<string>()
                        {
                            "I enrolled in Computer Science last year, and I’m here to renew my hotel reservation.\n",
                            "Yes, please. Could you help me? I’m here to enroll in Computer Science\n",
                            "I’m looking for the cafeteria because I need to check in for my flight\n"
                        },
                        respostaCorreta = 1
                    }
            );

            exerciciosWriting.Add(
                new ExercicioWriting()
                {
                    enunciado = "Write your full name, including your first and last names, in a complete sentence."
                }
            );

            exerciciosWriting.Add(
                new ExercicioWriting()
                {
                    enunciado = "Write your date of birth in a complete sentence, with the month written out."
                }
            );

            exerciciosCompletarFrase.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Complete the sentence with the correct word: Yes, ____ __ corret",
                    blocosPlavras = new List<string>
                    {
                        "that",
                        "served",
                        "alright",
                        "Which",
                        "tonight",
                        "don't",
                        "morning",
                        "no",                        
                        "this",
                        "is",
                    },
                    respostaCorreta = "that, is\n"
                }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Your enrollment is complete, but you don't know when your classes begin. Ask the secretary politely.",
                        opcoesFala = new List<string>()
                        {
                            "Could you tell me when the classes start?\n",
                            "Could you tell me when do the classes start?\n",
                            "Could you tell me when the classes started?\n"
                        },
                        respostaCorreta = 0
                    }
            );

            exerciciosAlternativas.Add(
                new ExercicioAlternativas()
                {
                    enunciado = "Which room is your first class in? Choose the correct answer:",
                    alternativas = new List<string>() { 
                        "Room 102\r\n", 
                        "Room 204\r\n", 
                        "Room 301\r\n", 
                        "Room 305\r\n" },
                    alternativaCorreta = 3
                }
            );

            exerciciosWriting.Add(
                new ExercicioWriting()
                {
                    enunciado = "You received your schedule, and your first class is in room 305. You don't know where it is. Write a question to ask the secretary."
                }
            );

            exerciciosAssociacao.Add(
                new ExercicioAssociacao()
                {
                    enunciado = "Match the Portuguese word with the English word.",

                    opcoesPort = new List<string>
                    {
                        "matrícula (efetivada)",
                        "mensalidade",
                        "corredor",
                        "balcão da recepção",
                        "esclarecimento",
                        "confirmar",
                        "detalhes",
                        "nome completo"
                    },

                    opcoesIngles = new List<string>
                    {
                        "hallway",
                        "confirm",
                        "full name",
                        "tuition",
                        "clarification",
                        "details",
                        "enrollment",
                        "reception desk"
                    },

                    associacaoCorreta = new List<ParAssociacao>
                    {
                        new ParAssociacao { palavraPortugues = "matrícula (efetivada)", palavraIngles = "enrollment" },
                        new ParAssociacao { palavraPortugues = "mensalidade", palavraIngles = "tuition" },
                        new ParAssociacao { palavraPortugues = "corredor", palavraIngles = "hallway" },
                        new ParAssociacao { palavraPortugues = "balcão da recepção", palavraIngles = "reception desk" },
                        new ParAssociacao { palavraPortugues = "esclarecimento", palavraIngles = "clarification" },
                        new ParAssociacao { palavraPortugues = "confirmar", palavraIngles = "confirm" },
                        new ParAssociacao { palavraPortugues = "detalhes", palavraIngles = "details" },
                        new ParAssociacao { palavraPortugues = "nome completo", palavraIngles = "full name" }
                    }
                }
            );
        }
    }
}
