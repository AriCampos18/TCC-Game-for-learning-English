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

    public string idNpcParaVoz = "recepcionistHotel";

    public void Start(){
        nivelAtual = DadosJogador.nivelUsuario;
        nomeExibicaoLegenda = "Restaurant Waiter";

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
                "Welcome!",
                "Here is the menu.",
                "Are you ready to order?",
                "Anything to drink?",
                "Sure. Your order will be ready in about 15 minutes.",
                "How can I help you?",
                "Of course. The bill is 30 dollars. How would you like to pay?"
            };
        }
        else if(nivelAtual=="A2")
        {
            dialogoAtual = new List<string>()
            {
                cumprimento+". Welcome to our restaurant.",
                "Here is the menu. Take your time.",
                "Are you ready to order?",
                "Would you like anything else?",
                "Your order will be ready in about twenty minutes.",
                "Is everything okay?",
                "Of course.",
                "Sure. Your bill is $43. Would you like to pay by card or cash?",
                "Thank you and I hope you enjoyed it."
            };
        }
        else if(nivelAtual=="B1")
        {
            dialogoAtual = new List<string>()
            {
                cumprimento+". Welcome. Here is the menu. Let me know when you're ready to order.",
                "It comes with a burger patty, cheese, salad, and onion.",
                "This burger is the restaurant's best-seller.",
                "Would you like anything else?",
                "Your order should be ready in about thirteen minutes.",
                "How was everything?",
                "Certainly.",
                "Your bill is R$42. Would you like to pay by card or cash?",
                "Would you like anything else?"
            };
        }
    }
}
