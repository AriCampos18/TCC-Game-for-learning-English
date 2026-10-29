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
        nomeExibicaoLegenda = "Restaurant Client";
    }

    private void InicializarConteudosPorNivel()
    {
            dialogoAtual = new List<string>()
            {
                "Hi. Is this your first time here?",
                "Have you tried the food here before?",
                "I come here almost every week. I really like the burgers, and the portions are generous, too.",
                "If you like burgers, I'd recommend the house special.",
                "Yes, it’s delicious. The fries are great, too."
            };

        exerciciosSpeaking.Add(
            new ExercicioSpeaking()
            {
                enunciado = "Respond to the customer’s question by sharing your experience and how you feel about the place",
                opcoesFala = new List<string>()
                {
                    "Yes, I ordered a hamburger and it’s already arriving \n",
                    "No, I’d like to pay by card\n",
                    "Yes, it is. I’m enjoying the atmosphere\n"
                },
                respostaCorreta = 2
            }
        ); 

        exerciciosSpeaking.Add(
            new ExercicioSpeaking()
            {
                enunciado = "Respond to the customer’s question by explaining whether you have tried the restaurant’s food before",
                opcoesFala = new List<string>()
                {
                    "No, I haven't, but I'm looking forward to it\n",
                    "Yes, I’d like to book a table for two, please\n",
                    "Could I have the bill, please? I'd like to pay before leaving\n"
                },
                respostaCorreta = 0
            }
        ); 

        exerciciosCriarFrases.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Arrange the words to form the correct sentence in english: Não, obrigado. Isso é tudo o que vou pedir por enquanto.",
                    blocosPlavras = new List<string>
                    {
                        "trying",
                        "hamburger",
                        "you",
                        "worth",
                        "Would",
                        "delicious",
                        "the",
                        "say",
                        "is",
                        "it",
                        "a",
                        "be",
                        "the",
                        "enjoy",
                        "try"
                    },
                    respostaCorreta = "Would you say the hamburger is worth trying?\n"
                }
            );
    }
}
