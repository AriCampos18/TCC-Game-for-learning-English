using System.Collections.Generic;
using System.Linq;

internal static class MissoesRestaurant
{
    internal const string Cenario = "Restaurant";
    internal const string Garcom = "restaurant_waiter_npc";
    internal const string Cliente = "restaurant_client";

    internal static HashSet<string> ChavesEsperadas(string nivel) =>
        new HashSet<string>(Definicoes(nivel).Select(d => d.mission_key));

    internal static CatalogoMissoes Criar(string nivel)
    {
        var definicoes = Definicoes(nivel);
        return definicoes.Count == 0 ? null : new CatalogoMissoes(definicoes, nivel, Cenario, catalogoLocal: true);
    }

    internal static bool ValidaEvento(CatalogoMissoes.Evento e)
    {
        switch (e.tipo)
        {
            case "conversa_encerrada": return e.alvo == Cliente;
            case "marco_fluxo":
                return e.alvo == "mesa_sentou" || e.alvo == "mesa_levantou"
                    || e.alvo == "mesa_retornou" || e.alvo == "refeicao_concluida";
            case "exercicio_encerrado":
                return e.alvo == Garcom + "/drink_order/1" || e.alvo == Garcom + "/meal_order/1"
                    || e.alvo == Garcom + "/menu_details/1" || e.alvo == Garcom + "/menu_details/2"
                    || e.alvo == Garcom + "/takeaway/1" || e.alvo == Garcom + "/payment/1";
            default: return false;
        }
    }

    private static CatalogoMissoes.Evento Evento(string tipo, string alvo) =>
        new CatalogoMissoes.Evento { tipo = tipo, alvo = alvo };

    private static CatalogoMissoes.Regra Regra(params CatalogoMissoes.Evento[] eventos) =>
        new CatalogoMissoes.Regra
        {
            operador = eventos.Length == 1 ? "EVENTO" : "AND",
            eventos = new List<CatalogoMissoes.Evento>(eventos)
        };

    private static List<CatalogoMissoes.Definicao> Definicoes(string nivel)
    {
        var missoes = new List<CatalogoMissoes.Definicao>();
        void Adicionar(string chave, string texto, string traducao, params CatalogoMissoes.Evento[] eventos)
        {
            int i = missoes.Count;
            missoes.Add(new CatalogoMissoes.Definicao
            {
                mission_key = chave, nivel = nivel, ordem = i + 1, texto = texto, traducao = traducao,
                inicio = i == 0 ? Regra(Evento("cenario_iniciado", Cenario))
                    : Regra(Evento("missao_concluida", missoes[i - 1].mission_key)),
                conclusao = Regra(eventos)
            });
        }

        switch (nivel)
        {
            case "A1":
                Adicionar("rest_a1_entrar", "Take a seat at a restaurant table.",
                    "Sente-se em uma mesa do restaurante.", Evento("marco_fluxo", "mesa_sentou"));
                Adicionar("rest_a1_pedido", "Place your food order.",
                    "Faça seu pedido de comida.", Evento("exercicio_encerrado", Garcom + "/drink_order/1"));
                Adicionar("rest_a1_comer", "Enjoy your food.",
                    "Aproveite sua comida.", Evento("marco_fluxo", "refeicao_concluida"));
                Adicionar("rest_a1_viagem", "Ask for your meal to go.",
                    "Peça sua refeição para viagem.", Evento("exercicio_encerrado", Garcom + "/takeaway/1"));
                Adicionar("rest_a1_pagamento", "Pay for your meal.",
                    "Pague pela sua refeição.", Evento("exercicio_encerrado", Garcom + "/payment/1"));
                break;
            case "A1-A2":
                Adicionar("rest_a1a2_entrar", "Find a table and sit down for your meal.",
                    "Encontre uma mesa e sente-se para sua refeição.", Evento("marco_fluxo", "mesa_sentou"));
                Adicionar("rest_a1a2_pedido", "Tell the waiter what you would like to order.",
                    "Diga ao garçom o que você gostaria de pedir.", Evento("exercicio_encerrado", Garcom + "/drink_order/1"));
                Adicionar("rest_a1a2_refeicao", "Enjoy the meal you ordered.",
                    "Aproveite a refeição que você pediu.", Evento("marco_fluxo", "refeicao_concluida"));
                Adicionar("rest_a1a2_viagem", "Ask to take your leftover food home.",
                    "Peça para levar para casa a comida que sobrou.", Evento("exercicio_encerrado", Garcom + "/takeaway/1"));
                Adicionar("rest_a1a2_pagamento", "Ask for the bill and pay for your meal.",
                    "Peça a conta e pague pela refeição.", Evento("exercicio_encerrado", Garcom + "/payment/1"));
                break;
            case "A2":
                Adicionar("rest_a2_entrar", "Choose a table and get ready to order.",
                    "Escolha uma mesa e prepare-se para fazer seu pedido.", Evento("marco_fluxo", "mesa_sentou"));
                Adicionar("rest_a2_pedido", "Order your meal from the waiter.",
                    "Faça seu pedido de refeição ao garçom.", Evento("exercicio_encerrado", Garcom + "/meal_order/1"));
                Adicionar("rest_a2_comer", "Have your meal at the restaurant.",
                    "Faça sua refeição no restaurante.", Evento("marco_fluxo", "refeicao_concluida"));
                Adicionar("rest_a2_embalagem", "Request a container for your leftovers.",
                    "Peça uma embalagem para levar a comida que sobrou.", Evento("exercicio_encerrado", Garcom + "/takeaway/1"));
                Adicionar("rest_a2_pagamento", "Settle the bill before leaving.",
                    "Pague a conta antes de sair.", Evento("exercicio_encerrado", Garcom + "/payment/1"));
                break;
            case "A2-B1":
                Adicionar("rest_a2b1_entrar", "Take a seat and get ready for your dining experience.",
                    "Sente-se à mesa e prepare-se para sua experiência no restaurante.", Evento("marco_fluxo", "mesa_sentou"));
                Adicionar("rest_a2b1_pratos", "Ask the waiter about the dishes on the menu.",
                    "Pergunte ao garçom sobre os pratos do cardápio.", Evento("exercicio_encerrado", Garcom + "/menu_details/1"));
                Adicionar("rest_a2b1_pedido", "Choose your meal and place your order with the waiter.",
                    "Escolha sua refeição e faça o pedido ao garçom.", Evento("exercicio_encerrado", Garcom + "/meal_order/1"));
                Adicionar("rest_a2b1_cliente", "Leave your table and have a conversation with another customer.",
                    "Saia da sua mesa e converse com outro cliente.", Evento("marco_fluxo", "mesa_levantou"), Evento("conversa_encerrada", Cliente));
                Adicionar("rest_a2b1_refeicao", "Return to your table and enjoy your meal.",
                    "Volte à sua mesa e aproveite sua refeição.", Evento("marco_fluxo", "mesa_retornou"), Evento("marco_fluxo", "refeicao_concluida"));
                Adicionar("rest_a2b1_viagem", "Ask the waiter to pack up your remaining food.",
                    "Peça ao garçom para embalar a comida que restou.", Evento("exercicio_encerrado", Garcom + "/takeaway/1"));
                Adicionar("rest_a2b1_pagamento", "Pay the bill and request a receipt.",
                    "Pague a conta e solicite um recibo.", Evento("exercicio_encerrado", Garcom + "/payment/1"));
                break;
            case "B1":
                Adicionar("rest_b1_entrar", "Find a suitable table and settle in for your meal.",
                    "Encontre uma mesa adequada e acomode-se para sua refeição.", Evento("marco_fluxo", "mesa_sentou"));
                Adicionar("rest_b1_perguntar_pratos", "Ask about the available dishes and request a recommendation.",
                    "Pergunte sobre os pratos disponíveis e peça uma recomendação.", Evento("exercicio_encerrado", Garcom + "/menu_details/2"));
                Adicionar("rest_b1_pedido", "Decide what you would like to eat and place your order.",
                    "Decida o que gostaria de comer e faça seu pedido.", Evento("exercicio_encerrado", Garcom + "/meal_order/1"));
                Adicionar("rest_b1_cliente", "Step away from your table to chat with a fellow customer.",
                    "Afaste-se da mesa para conversar com outro cliente.", Evento("marco_fluxo", "mesa_levantou"), Evento("conversa_encerrada", Cliente));
                Adicionar("rest_b1_comer", "Return to your seat and enjoy the food you ordered.",
                    "Retorne ao seu lugar e aproveite a comida que pediu.", Evento("marco_fluxo", "mesa_retornou"), Evento("marco_fluxo", "refeicao_concluida"));
                Adicionar("rest_b1_viagem", "Arrange to take the remaining food home.",
                    "Solicite que a comida restante seja preparada para viagem.", Evento("exercicio_encerrado", Garcom + "/takeaway/1"));
                Adicionar("rest_b1_pagamento", "Settle your restaurant bill.",
                    "Quite a conta do restaurante.", Evento("exercicio_encerrado", Garcom + "/payment/1"));
                break;
        }

        return missoes;
    }
}
