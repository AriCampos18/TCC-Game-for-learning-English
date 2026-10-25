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
                "It was nice to see you."
            };
        }
        else if(nivelAtual=="A2")
        {
            dialogoAtual = new List<string>()
            {
                "Work has been busy lately, but I enjoy what I do."
            };
        }
        else if(nivelAtual=="B1")
        {
            dialogoAtual = new List<string>()
            {
                "So, how have things been going lately?",
                "Work has been going well. I've been busy with a new project, but I enjoy it.",
                "Are you staying for the night?",
            };
        }
    }
}
