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
                "OK. Your order will be ready in about fifteen minutes.",
                "How can I help you?",
                "Sure. The bill is 30 dollars. Cash or card?"
            };
        }
        else if(nivelAtual=="A2")
        {
            dialogoAtual = new List<string>()
            {
                cumprimento+". Welcome to our restaurant.",
                "Here is the menu. Take your time.",
                "Are you ready to order?",
                "ould you like something to drink with that?",
                "Your order will be ready in about twenty minutes.",
                "Is everything okay with your meal?",
                "Of course. I'll bring it right away.",
                "Sure. Your bill is $43. WWould you like to pay by card or cash?",
                "Thank you. I hope you enjoyed your meal. Come back soon!"
            };
        }
        else if(nivelAtual=="B1")
        {
            dialogoAtual = new List<string>()
            {
                cumprimento+". Welcome. Here is the menu. Let me know when you're ready to order.",
                "It comes with a beef patty, cheese, lettuce, tomato, and onion. You can also add bacon for 5 dollars.",
                "That's our best-seller. Most customers love it.",
                "Would you like anything to drink, or any side dishes with that?",
                "Your order should be ready in about ten minutes. I'll bring it to you as soon as it's ready.",
                "How was everything? Did you enjoy your meal?",
                "Certainly. I'll be right back.",
                "Your bill is R$42. Would you like to pay by card or cash?",
                "Thank you very much. Would you like a receipt?"
            };
        }
    }
}
