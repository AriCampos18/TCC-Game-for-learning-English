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
        if (nivelAtual == "A1")
        {
            string dataCheckout = ObterDataCheckout();
            dialogoAtual = new List<string>()
            {
                "Hello. How can I help you?",
                "What is the name on the reservation?",
                "How would you like to pay?",
                "Your room is 103 on the fifth floor",
                "Breakfast is available between 7:00 a.m. and 9:00 a.m.",
                "Your checkout is at "+dataCheckout+".",
                "Call us if you have any questions."
            };
        }
        else if(nivelAtual=="A2")
        {
            dialogoAtual = new List<string>()
            {
                cumprimento+". Welcome to the hotel. How can I help you?",
                "Sure. Could I have your name, please?",
                "Thank you. How many nights will you be staying with us?",
                "How would you like to pay for your stay?",
                "Your room is 205, on the second floor. Here is your room card.",
                "Breakfast is served from 7:00 a.m. to 10:00 a.m. in the restaurant on the first floor.",
                "Is there anything else you would like to know?",
                "Checkout is at 12:00 p.m. on October 13th.",
                "Is there anything else I can help you with?"
            };
        }
        else if(nivelAtual=="B1")
        {
            dialogoAtual = new List<string>()
            {
                cumprimento+". Welcome to the hotel. How can I assist you today?",
                "Certainly. Could you tell me the name under the reservation?",
                "I found your reservation. You booked a double room for seven nights. Is that correct?",
                "Would you like to pay by card or in cash?",
                "Your room is 308, on the third floor. Here is your room card.",
                "It’s just around the corner, next to the restaurant.",
                "Breakfast is served from 6:30 a.m. to 10:00 a.m. in the restaurant on the first floor.",
                "Do you need any other information about your stay?",
                "Checkout is at 12:00 p.m. on October 13th. If you need a late checkout, please let us know in advance.",
                "We can offer a late checkout until 2:00 p.m., depending on availability.",
                "Would you like anything else?"
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
