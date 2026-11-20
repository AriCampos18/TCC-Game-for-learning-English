using System.Collections.Generic;
using System.Linq;

internal static class MissoesHotel
{
    internal const string Cenario = "Hotel";
    internal const string Recepcao = "recepcionist_hotel_npc";
    internal const string InicioConversa = "npc_interagido";
    internal const string FimConversa = "conversa_encerrada";
    internal const string ExercicioEncerrado = "exercicio_encerrado";
    internal const string Marco = "marco_fluxo";
    internal const string CartaoRecolhido = "cartao_quarto_recolhido";

    internal static bool UsaCartao(string nivel) => nivel == "A1-A2" || nivel == "A2" || nivel == "A2-B1";
    internal static string AlvoExercicio(string ponto, int ordem) => $"{Recepcao}/{ponto}/{ordem}";

    internal static bool ValidaEvento(CatalogoMissoes.Evento evento)
    {
        switch (evento.tipo)
        {
            case InicioConversa:
            case FimConversa: return evento.alvo == Recepcao;
            case Marco: return evento.alvo == CartaoRecolhido;
            case ExercicioEncerrado:
                return evento.alvo == AlvoExercicio("payment", 1) || evento.alvo == AlvoExercicio("breakfast", 2)
                    || evento.alvo == AlvoExercicio("checkout", 3);
            default: return false;
        }
    }

    private static CatalogoMissoes.Regra Regra(string tipo, string alvo) =>
        new CatalogoMissoes.Regra
        {
            operador = "EVENTO",
            eventos = new List<CatalogoMissoes.Evento> { new CatalogoMissoes.Evento { tipo = tipo, alvo = alvo } }
        };

    internal static HashSet<string> ChavesEsperadas(string nivel) =>
        new HashSet<string>(Definicoes(nivel).Select(d => d.mission_key));

    internal static CatalogoMissoes Criar(string nivel)
    {
        var definicoes = Definicoes(nivel);
        return definicoes.Count == 0 ? null : new CatalogoMissoes(definicoes, nivel, Cenario, catalogoLocal: true);
    }

    private static List<CatalogoMissoes.Definicao> Definicoes(string nivel)
    {
        var missoes = new List<CatalogoMissoes.Definicao>();
        var inicio = Regra(InicioConversa, Recepcao);
        var fim = Regra(FimConversa, Recepcao);
        var cartao = Regra(Marco, CartaoRecolhido);

        void Adicionar(string chave, string texto, string traducao, CatalogoMissoes.Regra conclusao)
        {
            int i = missoes.Count;
            missoes.Add(new CatalogoMissoes.Definicao
            {
                mission_key = chave, nivel = nivel, ordem = i + 1, texto = texto, traducao = traducao,
                inicio = i == 0 ? Regra("cenario_iniciado", Cenario)
                    : Regra("missao_concluida", missoes[i - 1].mission_key),
                conclusao = conclusao
            });
        }

        switch (nivel)
        {
            case "A1":
                Adicionar("hotel_a1_conversar", "Talk to the hotel receptionist.",
                    "Fale com o recepcionista do hotel.", inicio);
                Adicionar("hotel_a1_servicos", "Learn about the hotel services as you talk.",
                    "Conheça os serviços do hotel durante a conversa.", fim);
                break;
            case "A1-A2":
                Adicionar("hotel_a1a2_checkin", "Check in and confirm the details of your stay with the receptionist.",
                    "Realize seu check-in e confirme os dados da hospedagem com o recepcionista.", fim);
                Adicionar("hotel_a1a2_cartao", "Pick up your room key card.", "Pegue o cartão do quarto.", cartao);
                break;
            case "A2":
                Adicionar("hotel_a2_checkin", "Talk to the hotel receptionist and check in.",
                    "Fale com o recepcionista do hotel e realize seu check-in.", fim);
                Adicionar("hotel_a2_cartao_quarto", "Pick up your room key card from the counter.",
                    "Pegue o cartão do quarto no balcão.", cartao);
                break;
            case "A2-B1":
                Adicionar("hotel_a2b1_checkin", "Confirm your reservation and check in with the hotel receptionist.",
                    "Confirme sua reserva e realize o check-in com o recepcionista do hotel.", fim);
                Adicionar("hotel_a2b1_cartao",
                    "Collect your room key card from the reception desk and make sure you have everything you need to access your room.",
                    "Retire o cartão de acesso ao quarto no balcão da recepção e certifique-se de que tem tudo o que precisa para entrar no quarto.", cartao);
                break;
            case "B1":
                Adicionar("hotel_b1_conversar", "Enter the hotel and speak to the receptionist.",
                    "Entre no hotel e converse com o recepcionista.", inicio);
                Adicionar("hotel_b1_checkin", "Provide your details and confirm the information on your reservation.",
                    "Informe seus dados e confirme as informações da reserva.", Regra(ExercicioEncerrado, AlvoExercicio("payment", 1)));
                Adicionar("hotel_b1_servicos", "Find out about the hotel's services and get information about your room and breakfast.",
                    "Conheça os serviços do hotel e obtenha informações sobre o quarto e o café da manhã.", Regra(ExercicioEncerrado, AlvoExercicio("breakfast", 2)));
                Adicionar("hotel_b1_late_checkout", "Check the checkout time and request a late checkout.",
                    "Consulte o horário do checkout e solicite um checkout tardio.", Regra(ExercicioEncerrado, AlvoExercicio("checkout", 3)));
                break;
        }

        return missoes;
    }
}
