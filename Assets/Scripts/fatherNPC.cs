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

    public string idNpcParaVoz = "father";

    public void Start(){
        nivelAtual = DadosJogador.nivelUsuario;
        nomeExibicaoLegenda = "Father";
    }

    private void InicializarConteudosPorNivel()
    {
        if (nivelAtual == "A1")
        {
            dialogoAtual = new List<string>()
            {
                "Hi! Nice to see you.",
                "It was nice to see you."
            };
        }
        else if(nivelAtual=="A2")
        {
            dialogoAtual = new List<string>()
            { 
                "Work has been busy lately, but I enjoy what I do.",
                "Are you staying here tonight?",
                "It was great to see you. Come back soon!"     
            };

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Ask your father how things are going at work",
                        opcoesFala = new List<string>()
                        {
                            "How are things going at work, dad?\n",
                            "Where is the bathroom in this house?\n",
                            "What would you like to eat for dinner?\n"
                        },
                        respostaCorreta = 0
                    }
            );
        }
        else if(nivelAtual=="B1")
        {
            dialogoAtual = new List<string>()
            {
                "So, how have things been going lately?",
                "Work has been going well. I've been busy with a new project, but I enjoy it.",
                "Are you staying for the night?",
            };

            exerciciosWriting.Add(
                new ExercicioWriting()
                {
                    enunciado = "Write a short paragraph about your daily routine, including your usual activities and any recent changes"
                }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Ask your father about his recent experiences at work",
                        opcoesFala = new List<string>()
                        {
                            "How have things been going at work lately?\n",
                            "How many projects are you working tomorrow?\n",
                            "How did you enjoy your work next week?\n"
                        },
                        respostaCorreta = 0
                    }
            );
        }
    }
}
