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
                "Look! We have a new bathroom.",
                "How is your work?",
                "Dinner is ready!"
            };
        }
        else if(nivelAtual=="A2")
        {
            dialogoAtual = new List<string>()
            {
                "Hi! It's so good to see you. How have you been?",
                "I’m great, thanks. How was your day?",
                "We changed the room while you were away. Do you like it?",
                "How is work going?",
                "Dinner is ready. Come to the table, please."
            };
        }
        else if(nivelAtual=="B1")
        {
            dialogoAtual = new List<string>()
            {
                "Hey! It's been a while. How have you been?",
                "We made a few changes to the kitchen while you were away. What do you think?",
                "It's been good. My team is working on an interesting project, so the days go by quickly.",
                "Dinner is ready, everyone. Come to the table, please!",
                "It was really nice having you here."
            };
        }
    }
}
