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
                "May I see your passport, please?",
                "Here is your boarding pass. Your gate is twelve."
            };
        }
        else if(nivelAtual=="A2")
        {
            dialogoAtual = new List<string>()
            {
                cumrpimento+". Where are you flying today?",
                "May I see your passport and booking confirmation, please?",
                "Do you have any bags to check in?",
                "Your gate is eighteen, and boarding starts at two thirdy p.m."
            };
        }
        else if(nivelAtual=="B1")
        {
            dialogoAtual = new List<string>()
            {
                cumrpimento+". Where are you travelling today?",
                "Could I see your passport and booking confirmation, please?",
                "Your connecting flight leaves from gate twenty four. Your checked bag will be transferred automatically.",
                "Please check the departure screens in case the gate changes."
            };
        }
    }
}
