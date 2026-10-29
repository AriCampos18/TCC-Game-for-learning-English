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

    public string idNpcParaVoz = "brother";

    public void Start(){
        nivelAtual = DadosJogador.nivelUsuario;
        nomeExibicaoLegenda = "Brother";
    }

    private void InicializarConteudosPorNivel()
    {
        if (nivelAtual == "A1")
        {
            dialogoAtual = new List<string>()
            {
                "Hi! I am happy to see you.",  
                "I usually do my homework and then play with the dog.",
                "He likes playing with his ball."
            };

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Ask your brother about his daily routine after school",
                        opcoesFala = new List<string>()
                        {
                            "What do you usually do after school?\n",
                            "What is your favorite subject at school?\n",
                            "Do you have homework tonight?\n"
                        },
                        respostaCorreta = 0
                    }
            ); 

            exerciciosCompletarFrase.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Complete the sentence with the correct word: What does the dog ____ to play with?",
                    blocosPlavras = new List<string>
                    {
                        "want",
                        "fetch",
                        "like",
                        "eat",
                        "sleep",
                        "need",
                        "run",
                        "have",
                        "walk",
                        "play",
                        "likes"
                    },
                    respostaCorreta = "like\n"
                }
            );

            exerciciosAlternativas.Add(
                new ExercicioAlternativas()
                {
                        enunciado = "What does the dog play with?",
                        alternativas = new List<string>() { 
                        "A ball\r\n", 
                        "A book\r\n", 
                        "A chair\r\n", 
                        "A shoe\r\n" },
                        alternativaCorreta = 0
                }
            ); 
        }
        else if(nivelAtual=="A2")
        {
            dialogoAtual = new List<string>()
            {
                "Hey! Long time no see.", 
                "I like school, but I prefer spending time playing with the dog.",
                "He really likes his ball. Yesterday we played in the backyard for an hour."
            };

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Ask your brother how he is doing at school.",
                        opcoesFala = new List<string>()
                        {
                            "How much does your school lunch cost yesterday?\n",
                            "How have things been going at school lately?\n",
                            "Where is the school yesterday going?\n"
                        },
                        respostaCorreta = 1
                    }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Ask your brother what his dog likes to play with."
                    }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Talk about the pets you have or the animals you would like to have."
                    }
            );
        }
        else if(nivelAtual=="B1")
        {
            dialogoAtual = new List<string>()
            {
                "Hey! It's been a while. What have you been up to?",
                "I like school, but honestly I'd rather spend time with the dog after classes.",
                "He loves playing fetch with his ball and running around in the backyard."
            };

            exerciciosCriarFrases.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Arrange the words to form the correct sentence in english: Como as coisas têm ido na escola ultimamente?",
                    blocosPlavras = new List<string>
                    {
                        "lately",
                        "been",
                        "school",
                        "How",
                        "things",
                        "at",
                        "going",
                        "have",
                        "work",
                        "is",
                        "your",
                        "doing",
                        "the",
                        "well"
                    },
                    respostaCorreta = "How have things been going at school lately?\n"
                }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Ask your brother about the activities the dog enjoys doing lately.",
                        opcoesFala = new List<string>()
                        {
                            "How many classes does the dog have?\n",
                            "Where does the dog study?\n",
                            "What does the dog like doing these days?\n"
                        },
                        respostaCorreta = 2
                    }
            );

            exerciciosWriting.Add(
                new ExercicioWriting()
                {
                    enunciado = "Write a short paragraph about your relationship with animals, describing your experiences with pets or the animals you would like to have"
                }
            );
        }
    }
}
