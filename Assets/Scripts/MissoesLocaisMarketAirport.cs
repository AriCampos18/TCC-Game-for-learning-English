using System.Collections.Generic;

internal static class MissoesLocaisMarketAirport
{
    internal static CatalogoMissoes Criar(string cenario, string nivel)
    {
        var missoes = new List<CatalogoMissoes.Definicao>();
        void Adicionar(string chave, string texto, string traducao, CatalogoMissoes.Regra inicio, CatalogoMissoes.Regra conclusao)
        {
            missoes.Add(new CatalogoMissoes.Definicao
            {
                mission_key = chave, nivel = nivel, ordem = missoes.Count + 1, texto = texto,
                traducao = traducao, inicio = inicio, conclusao = conclusao
            });
        }

        // As mesmas regras dos cat?logos do servidor, incluindo condi??es parciais AND.
        switch ((cenario, nivel))
        {
            case ("Market", "A1"):
                Adicionar("pegar_cesta", "Enter the market and get a shopping basket",
                    "Entre no mercado e pegue uma cesta de compras",
                    Regra(("cenario_iniciado", "Market")),
                    Regra(("marco_fluxo", "cesta_coletada")));
                Adicionar("falar_atendente", "Talk to the market attendant",
                    "Fale com o atendente do mercado",
                    Regra(("missao_concluida", "pegar_cesta")),
                    Regra(("conversa_encerrada", "shelf_npc")));
                Adicionar("falar_padaria", "Talk to the bakery attendant",
                    "Fale com o atendente da padaria",
                    Regra(("missao_concluida", "pegar_cesta")),
                    Regra(("conversa_encerrada", "bakery_npc")));
                Adicionar("seguir_atendente", "Follow the attendant to get the products",
                    "Siga o atendente para pegar os produtos",
                    Regra(("marco_fluxo", "rota_prateleiras_iniciada")),
                    Regra(("marco_fluxo", "rota_prateleiras_concluida")));
                Adicionar("interagir_produtos", "Get the milk and the fruit from the shelves",
                    "Pegue o leite e a fruta das prateleiras",
                    Regra(("marco_fluxo", "produtos_liberados")),
                    Regra(("marco_fluxo", "produtos_obrigatorios_coletados")));
                Adicionar("falar_caixa", "Put the products on the counter to start the conversation with the cashier.",
                    "Coloque os produtos no balcão do caixa para iniciar a conversa com o caixa.",
                    Regra(("conversa_encerrada", "shelf_npc"), ("conversa_encerrada", "bakery_npc"), ("missao_concluida", "interagir_produtos")),
                    Regra(("conversa_encerrada", "cashier_npc")));
                Adicionar("pegar_sacola", "Pick up your bag at the checkout and leave the market",
                    "Pegue a sacola no caixa e saia do mercado",
                    Regra(("marco_fluxo", "sacola_liberada")),
                    Regra(("marco_fluxo", "sacola_coletada"), ("marco_fluxo", "saida_market")));
                break;
            case ("Market", "A1-A2"):
                Adicionar("pegar_cesta", "Enter the market and pick up a shopping basket.",
                    "Entre no mercado e pegue uma cesta de compras.",
                    Regra(("cenario_iniciado", "Market")),
                    Regra(("marco_fluxo", "cesta_coletada")));
                Adicionar("falar_padaria", "Talk to the bakery attendant.",
                    "Fale com o atendente da padaria.",
                    Regra(("missao_concluida", "pegar_cesta")),
                    Regra(("npc_interagido", "bakery_npc")));
                Adicionar("falar_atendente", "Ask the market attendant for help.",
                    "Peça ajuda ao atendente do mercado.",
                    Regra(("missao_concluida", "pegar_cesta")),
                    Regra(("npc_interagido", "shelf_npc")));
                Adicionar("seguir_atendente", "Follow the attendant to the correct shelves.",
                    "Siga o atendente até as prateleiras corretas.",
                    Regra(("marco_fluxo", "rota_prateleiras_iniciada")),
                    Regra(("marco_fluxo", "rota_prateleiras_concluida")));
                Adicionar("interagir_produtos", "Pick up milk, cereal, and snacks from the shelves.",
                    "Pegue leite, cereal e salgadinhos nas prateleiras.",
                    Regra(("marco_fluxo", "produtos_liberados")),
                    Regra(("marco_fluxo", "produtos_obrigatorios_coletados")));
                Adicionar("falar_caixa", "Place your products on the checkout counter and talk to the cashier.",
                    "Coloque seus produtos no balcão do caixa e fale com o caixa.",
                    Regra(("conversa_encerrada", "shelf_npc"), ("conversa_encerrada", "bakery_npc"), ("missao_concluida", "interagir_produtos")),
                    Regra(("npc_interagido", "cashier_npc")));
                Adicionar("pegar_sacola", "Collect your shopping bag and leave the market.",
                    "Pegue sua sacola de compras e saia do mercado.",
                    Regra(("marco_fluxo", "sacola_liberada")),
                    Regra(("marco_fluxo", "sacola_coletada"), ("marco_fluxo", "saida_market")));
                break;
            case ("Market", "A2"):
                Adicionar("pegar_cesta", "Enter the market and get a shopping basket",
                    "Entre no mercado e pegue uma cesta de compras",
                    Regra(("cenario_iniciado", "Market")),
                    Regra(("marco_fluxo", "cesta_coletada")));
                Adicionar("falar_atendente", "Talk to the market attendant",
                    "Fale com o atendente do mercado",
                    Regra(("missao_concluida", "pegar_cesta")),
                    Regra(("conversa_encerrada", "shelf_npc")));
                Adicionar("falar_padaria", "Talk to the bakery attendant",
                    "Fale com o atendente da padaria",
                    Regra(("missao_concluida", "pegar_cesta")),
                    Regra(("conversa_encerrada", "bakery_npc")));
                Adicionar("seguir_atendente", "Follow the attendant to get the products",
                    "Siga o atendente para pegar os produtos",
                    Regra(("marco_fluxo", "rota_prateleiras_iniciada")),
                    Regra(("marco_fluxo", "rota_prateleiras_concluida")));
                Adicionar("interagir_produtos", "Get the milk, the cereal and the snack from the shelves",
                    "Pegue o leite, o cereal e o salgadinho das prateleiras",
                    Regra(("marco_fluxo", "produtos_liberados")),
                    Regra(("marco_fluxo", "produtos_obrigatorios_coletados")));
                Adicionar("falar_caixa", "Put the products on the counter to start the conversation with the cashier.",
                    "Coloque os produtos no balcão do caixa para iniciar a conversa com o caixa.",
                    Regra(("conversa_encerrada", "shelf_npc"), ("conversa_encerrada", "bakery_npc"), ("missao_concluida", "interagir_produtos")),
                    Regra(("conversa_encerrada", "cashier_npc")));
                Adicionar("pegar_sacola", "Pick up your bag at the checkout and leave the market",
                    "Pegue a sacola no caixa e saia do mercado",
                    Regra(("marco_fluxo", "sacola_liberada")),
                    Regra(("marco_fluxo", "sacola_coletada"), ("marco_fluxo", "saida_market")));
                break;
            case ("Market", "A2-B1"):
                Adicionar("pegar_cesta", "Enter the market and collect a basket for your shopping.",
                    "Entre no mercado e pegue uma cesta para suas compras.",
                    Regra(("cenario_iniciado", "Market")),
                    Regra(("marco_fluxo", "cesta_coletada")));
                Adicionar("falar_atendente", "Ask the market attendant for help finding the products.",
                    "Peça ao atendente do mercado ajuda para encontrar os produtos.",
                    Regra(("missao_concluida", "pegar_cesta")),
                    Regra(("npc_interagido", "shelf_npc")));
                Adicionar("falar_padaria", "Speak with the bakery attendant.",
                    "Converse com o atendente da padaria.",
                    Regra(("missao_concluida", "pegar_cesta")),
                    Regra(("npc_interagido", "bakery_npc")));
                Adicionar("seguir_atendente", "Follow the attendant to the shelves where the products are located.",
                    "Siga o atendente até as prateleiras onde estão os produtos.",
                    Regra(("marco_fluxo", "rota_prateleiras_iniciada")),
                    Regra(("marco_fluxo", "rota_prateleiras_concluida")));
                Adicionar("interagir_produtos", "Collect milk, cereal, and snacks from their respective shelves.",
                    "Recolha leite, cereal e salgadinhos nas respectivas prateleiras.",
                    Regra(("marco_fluxo", "produtos_liberados")),
                    Regra(("marco_fluxo", "produtos_obrigatorios_coletados")));
                Adicionar("falar_caixa", "Take your selected products to the checkout counter and speak with the cashier.",
                    "Leve os produtos escolhidos ao balcão do caixa e fale com o caixa.",
                    Regra(("conversa_encerrada", "shelf_npc"), ("conversa_encerrada", "bakery_npc"), ("missao_concluida", "interagir_produtos")),
                    Regra(("npc_interagido", "cashier_npc")));
                Adicionar("pegar_sacola", "Pick up your shopping bag at checkout and exit the market.",
                    "Pegue sua sacola de compras no caixa e saia do mercado.",
                    Regra(("marco_fluxo", "sacola_liberada")),
                    Regra(("marco_fluxo", "sacola_coletada"), ("marco_fluxo", "saida_market")));
                break;
            case ("Market", "B1"):
                Adicionar("pegar_cesta", "Enter the market and get a shopping basket",
                    "Entre no mercado e pegue uma cesta de compras",
                    Regra(("cenario_iniciado", "Market")),
                    Regra(("marco_fluxo", "cesta_coletada")));
                Adicionar("falar_atendente", "Talk to the market attendant",
                    "Fale com o atendente do mercado",
                    Regra(("missao_concluida", "pegar_cesta")),
                    Regra(("conversa_encerrada", "shelf_npc")));
                Adicionar("falar_padaria", "Talk to the bakery attendant",
                    "Fale com o atendente da padaria",
                    Regra(("missao_concluida", "pegar_cesta")),
                    Regra(("conversa_encerrada", "bakery_npc")));
                Adicionar("seguir_atendente", "Follow the attendant to get the products",
                    "Siga o atendente para pegar os produtos",
                    Regra(("marco_fluxo", "rota_prateleiras_iniciada")),
                    Regra(("marco_fluxo", "rota_prateleiras_concluida")));
                Adicionar("interagir_produtos", "Get the vegetable and the soda from the shelves",
                    "Pegue o legumes e o refrigerante das prateleiras",
                    Regra(("marco_fluxo", "produtos_liberados")),
                    Regra(("marco_fluxo", "produtos_obrigatorios_coletados")));
                Adicionar("falar_caixa", "Put the products on the counter to start the conversation with the cashier.",
                    "Coloque os produtos no balcão do caixa para iniciar a conversa com o caixa.",
                    Regra(("conversa_encerrada", "shelf_npc"), ("conversa_encerrada", "bakery_npc"), ("missao_concluida", "interagir_produtos")),
                    Regra(("conversa_encerrada", "cashier_npc")));
                Adicionar("pegar_sacola", "Pick up your bag at the checkout and leave the market",
                    "Pegue a sacola no caixa e saia do mercado",
                    Regra(("marco_fluxo", "sacola_liberada")),
                    Regra(("marco_fluxo", "sacola_coletada"), ("marco_fluxo", "saida_market")));
                break;
            case ("Airport", "A1"):
                Adicionar("aeroporto_a1_1", "Arrive at the airport and find the information desk",
                    "Chegue ao aeroporto e procure o balcão de informações",
                    Regra(("cenario_iniciado", "Airport")),
                    Regra(("npc_interagido", "airport_info_npc")));
                Adicionar("aeroporto_a1_2", "Ask for directions to find the check-in desk",
                    "Peça orientações para encontrar o balcão de check-in",
                    Regra(("missao_concluida", "aeroporto_a1_1")),
                    Regra(("conversa_encerrada", "airport_info_npc")));
                Adicionar("aeroporto_a1_3", "Go to the check-in desk and present your passport",
                    "Dirija-se ao balcão de check-in e apresente seu passaporte",
                    Regra(("missao_concluida", "aeroporto_a1_2")),
                    Regra(("npc_interagido", "checkin_npc")));
                Adicionar("aeroporto_a1_4", "Check your boarding pass",
                    "Consulte seu cartão de embarque",
                    Regra(("conversa_encerrada", "checkin_npc")),
                    Regra(("cartao_embarque_consultado", "cartao_embarque")));
                Adicionar("aeroporto_a1_5", "Find gate 12",
                    "Encontre o portão 12",
                    Regra(("missao_concluida", "aeroporto_a1_4")),
                    Regra(("permanencia_confirmada", "portao_12")));
                break;
            case ("Airport", "A1-A2"):
                Adicionar("aeroporto_a1a2_1", "Arrive at the airport and find the information desk",
                    "Chegue ao aeroporto e procure o balcão de informações",
                    Regra(("cenario_iniciado", "Airport")),
                    Regra(("npc_interagido", "airport_info_npc")));
                Adicionar("aeroporto_a1a2_2", "Ask for directions to find the check-in desk",
                    "Peça orientações para encontrar o balcão de check-in",
                    Regra(("missao_concluida", "aeroporto_a1a2_1")),
                    Regra(("conversa_encerrada", "airport_info_npc")));
                Adicionar("aeroporto_a1a2_3", "Go to the check-in desk and present your passport",
                    "Dirija-se ao balcão de check-in e apresente seu passaporte",
                    Regra(("missao_concluida", "aeroporto_a1a2_2")),
                    Regra(("npc_interagido", "checkin_npc")));
                Adicionar("aeroporto_a1a2_4", "Present your passport and booking confirmation",
                    "Apresente seu passaporte e confirmação da reserva",
                    Regra(("marco_fluxo", "documentos_solicitados")),
                    Regra(("exercicio_encerrado", "checkin_npc/travel_documents/1")));
                Adicionar("aeroporto_a1a2_5", "Check your boarding pass and find gate 18",
                    "Consulte seu cartão de embarque e encontre o portão 18",
                    Regra(("missao_concluida", "aeroporto_a1a2_4")),
                    Regra(("cartao_embarque_consultado", "cartao_embarque"), ("permanencia_confirmada", "portao_18")));
                break;
            case ("Airport", "A2"):
                Adicionar("aeroporto_a2_1", "Arrive at the airport and find the check-in desk",
                    "Chegue ao aeroporto e procure o balcão de check-in",
                    Regra(("cenario_iniciado", "Airport")),
                    Regra(("npc_interagido", "checkin_npc")));
                Adicionar("aeroporto_a2_2", "Tell the airport employee your destination",
                    "Informe seu destino ao funcionário",
                    Regra(("missao_concluida", "aeroporto_a2_1")),
                    Regra(("exercicio_encerrado", "checkin_npc/destination/1")));
                Adicionar("aeroporto_a2_3", "Present your passport and booking confirmation",
                    "Apresente seu passaporte e confirmação da reserva",
                    Regra(("missao_concluida", "aeroporto_a2_2")),
                    Regra(("exercicio_encerrado", "checkin_npc/travel_documents/1")));
                Adicionar("aeroporto_a2_4", "Tell the employee about the luggage you need to check in",
                    "Informe sua bagagem para despacho",
                    Regra(("missao_concluida", "aeroporto_a2_3")),
                    Regra(("exercicio_encerrado", "checkin_npc/checked_baggage/1")));
                Adicionar("aeroporto_a2_5", "Find out the boarding time",
                    "Descubra o horário de embarque",
                    Regra(("missao_concluida", "aeroporto_a2_4")),
                    Regra(("exercicio_encerrado", "checkin_npc/boarding_time/1")));
                Adicionar("aeroporto_a2_6", "Find gate 18",
                    "Encontre o portão 18",
                    Regra(("marco_fluxo", "portao_liberado")),
                    Regra(("permanencia_confirmada", "portao_18")));
                break;
            case ("Airport", "A2-B1"):
                Adicionar("aeroporto_a2b1_1", "Go to the information desk and explain that you need to find the check-in desks",
                    "Dirija-se ao balcão de informações e explique que precisa encontrar o check-in",
                    Regra(("cenario_iniciado", "Airport")),
                    Regra(("exercicio_encerrado", "airport_info_npc/checkin_location/1")));
                Adicionar("aeroporto_a2b1_2", "Find out how to reach the second floor",
                    "Descubra o caminho para chegar ao segundo andar",
                    Regra(("missao_concluida", "aeroporto_a2b1_1")),
                    Regra(("exercicio_encerrado", "airport_info_npc/checkin_directions/1")));
                Adicionar("aeroporto_a2b1_3", "Find out how long it will take to reach the check-in desks",
                    "Confirme quanto tempo levará para chegar ao check-in",
                    Regra(("missao_concluida", "aeroporto_a2b1_2")),
                    Regra(("exercicio_encerrado", "airport_info_npc/walking_time/1")));
                Adicionar("aeroporto_a2b1_destino", "Provide your destination and connecting-flight details",
                    "Informe seu destino e os detalhes do voo de conexão",
                    Regra(("npc_interagido", "checkin_npc")),
                    Regra(("exercicio_encerrado", "checkin_npc/destination/1")));
                Adicionar("aeroporto_a2b1_4", "Present your travel documents",
                    "Apresente seus documentos de viagem",
                    Regra(("missao_concluida", "aeroporto_a2b1_destino")),
                    Regra(("exercicio_encerrado", "checkin_npc/travel_documents/1")));
                Adicionar("aeroporto_a2b1_5", "Confirm the information about your checked baggage",
                    "Confirme as informações sobre sua bagagem despachada",
                    Regra(("missao_concluida", "aeroporto_a2b1_4")),
                    Regra(("exercicio_encerrado", "checkin_npc/baggage_transfer/1")));
                Adicionar("aeroporto_a2b1_6", "Check the boarding information and connecting gate",
                    "Confira as informações sobre o embarque e o portão de conexão",
                    Regra(("missao_concluida", "aeroporto_a2b1_5")),
                    Regra(("cartao_embarque_consultado", "cartao_embarque")));
                Adicionar("aeroporto_a2b1_7", "Find gate 24 for your flight",
                    "Encontre o portão 24 para seu voo",
                    Regra(("missao_concluida", "aeroporto_a2b1_6")),
                    Regra(("permanencia_confirmada", "portao_24")));
                break;
            case ("Airport", "B1"):
                Adicionar("aeroporto_b1_1", "Arrive at the airport and find the information desk",
                    "Chegue ao aeroporto e procure o balcão de informações",
                    Regra(("cenario_iniciado", "Airport")),
                    Regra(("npc_interagido", "airport_info_npc")));
                Adicionar("aeroporto_b1_2", "Ask for directions to find the check-in desk",
                    "Peça orientações para encontrar o balcão de check-in",
                    Regra(("missao_concluida", "aeroporto_b1_1")),
                    Regra(("exercicio_encerrado", "airport_info_npc/checkin_location/1")));
                Adicionar("aeroporto_b1_3", "Go to the check-in desk",
                    "Dirija-se ao balcão de check-in",
                    Regra(("conversa_encerrada", "airport_info_npc")),
                    Regra(("npc_interagido", "checkin_npc")));
                Adicionar("aeroporto_b1_4", "Provide your destination and connecting-flight details",
                    "Informe seu destino e os detalhes do voo de conexão",
                    Regra(("missao_concluida", "aeroporto_b1_3")),
                    Regra(("exercicio_encerrado", "checkin_npc/destination/1")));
                Adicionar("aeroporto_b1_5", "Present your travel documents",
                    "Apresente seus documentos de viagem",
                    Regra(("missao_concluida", "aeroporto_b1_4")),
                    Regra(("exercicio_encerrado", "checkin_npc/travel_documents/1")));
                Adicionar("aeroporto_b1_6", "Confirm the checked-baggage information",
                    "Confirme as informações sobre a bagagem despachada",
                    Regra(("missao_concluida", "aeroporto_b1_5")),
                    Regra(("exercicio_encerrado", "checkin_npc/baggage_transfer/1")));
                Adicionar("aeroporto_b1_8", "Find gate 24 for your flight",
                    "Encontre o portão 24 para seu voo",
                    Regra(("marco_fluxo", "portao_liberado")),
                    Regra(("permanencia_confirmada", "portao_24")));
                break;
        }

        return new CatalogoMissoes(missoes, nivel, cenario, catalogoLocal: true);
    }

    private static CatalogoMissoes.Regra Regra(params (string tipo, string alvo)[] eventos)
    {
        var regra = new CatalogoMissoes.Regra
        {
            operador = eventos.Length == 1 ? "EVENTO" : "AND",
            eventos = new List<CatalogoMissoes.Evento>()
        };
        foreach (var evento in eventos)
            regra.eventos.Add(new CatalogoMissoes.Evento { tipo = evento.tipo, alvo = evento.alvo });
        return regra;
    }
}
