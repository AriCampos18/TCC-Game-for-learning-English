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
                "The secretary's office is on the first floor",
                "Go straight and turn right."
            };
        }
        else if(nivelAtual=="A2")
        {
            dialogoAtual = new List<string>()
            {
                cumprimento+". Welcome to the university. How can I help you?",
                "The secretary's office is on the first floor. Go straight and take the first door on your right.",
            };
        }
        else if(nivelAtual=="B1")
        {
            dialogoAtual = new List<string>()
            {
                cumprimento+". Welcome to the university. How may I help you today?",
                "Have you been here before?",
                "The secretary's office is on the first floor. Go straight past the reception desk and you will see a door on your right. It's there. You can talk with the secretary inside."
            };
        }
    }
}
