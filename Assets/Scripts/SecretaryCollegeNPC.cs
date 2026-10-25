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

    public string idNpcParaVoz = "secretaryCollege";

    public void Start(){
        nivelAtual = DadosJogador.nivelUsuario;
        nomeExibicaoLegenda = "College Secretary";

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
                "Hi. How can I help you?",
                "What is your name?",
                "What course do you want to study?",
                "What is your date of birth?",
                "Your registration is complete.",
                "The tuition is R$500 per month.",
                "Classes start on February 10th.",
                "Your classroom is room 204 straight."
            };
        }
        else if(nivelAtual=="A2")
        {
            dialogoAtual = new List<string>()
            {
                cumprimento+". Do you need help with something?",
                "Could I have your name, please?",
                "Which course would you like to study?",
                "What is your date of birth?",
                "Thank you. Your registration is complete. You are now enrolled in Computer Science.",
                "The tuition is R$500 per month.",
                "Classes start on February tenth.",
                "Your classroom is room two hundred four, on the second floor."
            };
        }
        else if(nivelAtual=="B1")
        {
            dialogoAtual = new List<string>()
            {
                cumprimento+". Are you here to enroll in a course?",
                "Could you give me your full name, please?",
                "Could you also give me your date of birth?",
                "Thank you. I just need to confirm a few details. You are enrolling in Computer Science, correct?",
                "Your enrollment is complete. Your tuition will be 500 dollars per month.",
                "Classes start on February 31st in the 325 room.",
                "Room 325 is on the second floor, at the end of the hallway. Good luck with your classes!"
            };
        }
    }
}
