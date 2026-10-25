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
        nomeExibicaoLegenda = "Restaurant Client";
    }

    private void InicializarConteudosPorNivel()
    {
            dialogoAtual = new List<string>()
            {
                "Hi. Is this your first time here?",
                "Have you tried the food here before?",
                "I come here almost every week. I really like the burgers, and the portions are generous, too.",
                "If you like burgers, I'd recommend the house special.",
                "Yes, it’s delicious. The fries are great, too."
            };
    }
}
