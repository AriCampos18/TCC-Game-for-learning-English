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

    public string idNpcParaVoz = "recepcionistHotel";

    public void Start(){
        nivelAtual = DadosJogador.nivelUsuario;
        nomeExibicaoLegenda = "Recepcionist Hotel";

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
                "Hello. How can I help you?",
                "Is this your first time here?",
                "Would you like to know how to get there?",
                "EXPLICA A DIREÇÃO DE FORMA SIMPLES"
            };
        }
        else if(nivelAtual=="A2")
        {
            dialogoAtual = new List<string>()
            {
                cumprimento+". How can I help you?",
                "Have you visited our city before?",
                "Here are some tourist attractions.",
                "Would you like to know how to get there?",
                "DIREÇÕES UM POUCO MAIS COMPLEXAS QUE NO A1.",
                "No, it is about a ten-minute walk."
            };
        }
        else if(nivelAtual=="B1")
        {
            dialogoAtual = new List<string>()
            {
                cumprimento+". Welcome to the tourist information center. How can I help you today?",
                "Have you been here before?",
                "DIREÇÕES MAIS COMPLEXAS QUE NO A2",
            };
        }
    }
}
