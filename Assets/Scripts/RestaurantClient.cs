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
                "I really like the burgers here. The portions are generous, too.",
                "Yes, it’s delicious. The fries are great, too."
            };
    }
}
