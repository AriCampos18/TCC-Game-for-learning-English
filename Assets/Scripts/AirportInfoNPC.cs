using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using TMPro;
using System;
using System.Runtime.CompilerServices;

public class SecretaryCollegeNPC: InteracaoNPC
{

    string nivelAtual;
    string cumrpimento;
    int horaAtual = DateTime.Now.Hour;

    public string idNpcParaVoz = "airportInfo";

    public void Start(){
        nivelAtual = DadosJogador.nivelUsuario;
        nomeExibicaoLegenda = "Airport Employee";

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
                cumrpimento+". Do you need help?",
                "Go up the stairs. Then go straight ahead."
            };
        }
        else if(nivelAtual=="A2")
        {
            dialogoAtual = new List<string>()
            {
                cumprimento + ". Can I help you?",
                "Check-in for your airline is on the second floor.",
                "Take the escalator up, then walk straight. You will see the check-in desks on your left.",
                "It takes about five minutes. You're welcome. Have a nice trip!"
            };
        }
        else if(nivelAtual=="B1")
        {
            dialogoAtual = new List<string>()
            {
                 cumprimento + ". Welcome to the airport. How can I help you?",
                "Sure. The check-in desks for your airline are on the second floor.",
                "Take the escalator behind me, and when you get to the top, turn right. Walk past the cafés and you'll see the desks on your left, just before security.",
                "It should take you about five minutes, but I'd go now because the lines get long in the morning.",
                "If you need anything else, you can find me here or at any information desk. Have a good flight!"
            };
        }
    }
}
