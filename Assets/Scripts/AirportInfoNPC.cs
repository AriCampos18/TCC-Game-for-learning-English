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
                "Just go up the stairs and go straight ahead."
            };
        }
        else if(nivelAtual=="A2")
        {
            dialogoAtual = new List<string>()
            {
                //VOU COLOCAR ALGUMA COISA
            };
        }
        else if(nivelAtual=="B1")
        {
            dialogoAtual = new List<string>()
            {
                //VOU COLOCAR ALGUMA COISA
            };
        }
    }
}
