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

    public string idNpcParaVoz = "brother";

    public void Start(){
        nivelAtual = DadosJogador.nivelUsuario;
        nomeExibicaoLegenda = "Brother";
    }

    private void InicializarConteudosPorNivel()
    {
        if (nivelAtual == "A1")
        {
            dialogoAtual = new List<string>()
            {
                "Hi! I am happy to see you.",  
                "I usually do my homework and then play with the dog.",
                "He likes playing with his ball."
            };
        }
        else if(nivelAtual=="A2")
        {
            dialogoAtual = new List<string>()
            {
                "Hey! Long time no see.", 
                "I like school, but I prefer spending time playing with the dog.",
                "He really likes his ball. Yesterday we played in the backyard for an hour."
            };
        }
        else if(nivelAtual=="B1")
        {
            dialogoAtual = new List<string>()
            {
                "Hey! It's been a while. What have you been up to?",
                "I like school, but honestly I'd rather spend time with the dog after classes.",
                "He loves playing fetch with his ball and running around in the backyard."
            };
        }
    }
}
