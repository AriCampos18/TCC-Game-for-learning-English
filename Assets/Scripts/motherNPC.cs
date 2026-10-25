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

    public string idNpcParaVoz = "mom";

    public void Start(){
        nivelAtual = DadosJogador.nivelUsuario;
        nomeExibicaoLegenda = "Mom";
    }

    private void InicializarConteudosPorNivel()
    {
        if (nivelAtual == "A1")
        {
            dialogoAtual = new List<string>()
            {
                "Hey. How are you?",
                "How was your day?",
                "We changed the bathroom. Go there.",
                "How is your work?",
                "Dinner is ready!"
            };
        }
        else if(nivelAtual=="A2")
        {
            dialogoAtual = new List<string>()
            {
                "Hi! It's so good to see you. How have you been?",
                "I’m great. How was your day?",
                "Dinner is ready."
            };
        }
        else if(nivelAtual=="B1")
        {
            dialogoAtual = new List<string>()
            {
                "Hey! It's been a while. How have you been?",
                "We made a few changes to the kicthen while you were away.",
                "It's been good. My team is working on an interesting project, so the days go by quickly",
                "Dinner is ready, everyone. Come to the table, please!",
                "It was really nice having you here."
            };
        }
    }
}
