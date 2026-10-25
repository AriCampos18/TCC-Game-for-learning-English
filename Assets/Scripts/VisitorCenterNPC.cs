using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using TMPro;
using System;

public class VisitorCenterNPC: InteracaoNPC
{

    string nivelAtual;
    string cumrpimento;
    int horaAtual = DateTime.Now.Hour;

    public string idNpcParaVoz = "tourGuide";

    public void Start(){
        nivelAtual = DadosJogador.nivelUsuario;
        nomeExibicaoLegenda = "Tour Guide";

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
                "Is this your first time here?",
                "Do you want to know how to get there?",
                "EXPLICA A DIREÇÃO DE FORMA SIMPLES"
            };
        }
        else if(nivelAtual=="A2")
        {
            dialogoAtual = new List<string>()
            {
                cumprimento+". How can I help you?",
                "Have you visited our city before?",
                "Here are some tourist attractions. The city garden is very popular.",
                "Would you like to know how to get there?",
                "DIREÇÕES UM POUCO MAIS COMPLEXAS QUE NO A1.",
                "No, it isn't far. It's about a ten-minute walk."
            };
        }
        else if(nivelAtual=="B1")
        {
            dialogoAtual = new List<string>()
            {
                cumprimento+". Welcome to the tourist information center. How can I help you today?",
                "Have you been here before?",
                "If it's your first visit, I'd recommend starting with the city park. It's one of the most famous places in the city.",
                "DIREÇÕES MAIS COMPLEXAS QUE NO A2",
                "If you don't feel like walking, you can also take bus 42. It stops right in front of the park.",
                "I'd go early, though, because it gets crowded in the afternoon."
            };
        }
    }
}
