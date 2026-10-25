using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using TMPro;
using System;

public class RecepcionistHotelNPC: InteracaoNPC
{

    string nivelAtual;
    string cumrpimento;
    int horaAtual = DateTime.Now.Hour;

    public string idNpcParaVoz = "recepcionistHotel";

    public void Start(){
        nivelAtual = DadosJogador.nivelUsuario;
        nomeExibicaoLegenda = "Hotel Recepcionist";

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
        string dataCheckout = ObterDataCheckout();
        if (nivelAtual == "A1")
        {
            dialogoAtual = new List<string>()
            {
                "Hello. Can I help you?",
                "What is yout name?",
                "Cash or card?",
                "Your room is 103. It is on the first floor",
                "Breakfast is from 7:00 a.m. to 9:00 a.m.",
                "Checkout is on "+dataCheckout+".",
                "Call me if you have a question."
            };
        }
        else if(nivelAtual=="A2")
        {
            dialogoAtual = new List<string>()
            {
                cumprimento+". Welcome to the hotel. Do you have a reservation?",
                "Great. Could I have your name, please?",
                "Thank you. How many nights are you staying with us?",
                "Would you like to pay by card or cash?",
                "Your room is 205, on the second floor. Here is your key card.",
                "Breakfast is from 7:00 a.m. to 10:00 a.m. in the restaurant. It's on the first floor.",
                "Is there anything else you would like to know?",
                "Checkout is at 12:00 p.m. on " + dataCheckout + ".",
                "Is there anything else I can help you with?",
                "Enjoy your stay!"
            };
        }
        else if(nivelAtual=="B1")
        {
            dialogoAtual = new List<string>()
            {
                cumprimento+". Welcome to the hotel. How can I help you today?",
                "Of course. Could you tell me the name the reservation is under?",
                "I found it. You booked a double room for seven nights, with breakfast included. Is that right?",
                "Perfect. Would you like to pay now by card or cash, or when you check out?",
                "Your room is 308, on the third floor. Here is your key card. The Wi-Fi is free, and the password is on the card.",
                "The elevators is just around the corner, next to the restaurant.",
                "Breakfast is served from 6:30 a.m. to 10:00 a.m. If you prefer, we can also bring it to your room.",
                "Is there anything else you'd like to know about your stay?",
                "Checkout is at 12:00 p.m. on " + dataCheckout + ". If you need a late checkout, please let us know in advance.",
                "We can offer a late checkout until 2:00 p.m., depending on availability.",
                "I hope you enjoy your stay. Just call the front desk if you need anything."
            };
        }
    }

    private string ObterDataCheckout()
    {
        DateTime dataCheckout = DateTime.Now.Date.AddDays(7);

        int dia = dataCheckout.Day;
        string sufixo;

        if (dia % 100 >= 11 && dia % 100 <= 13)
        {
            sufixo = "th";
        }
        else
        {
            switch (dia % 10)
            {
                case 1:
                    sufixo = "st";
                    break;
                case 2:
                    sufixo = "nd";
                    break;
                case 3:
                    sufixo = "rd";
                    break;
                default:
                    sufixo = "th";
                    break;
            }
        }

        string mes = dataCheckout.ToString(
            "MMMM",
            CultureInfo.GetCultureInfo("en-US")
        );

        return $"{mes} {dia}{sufixo}";
    }
}
