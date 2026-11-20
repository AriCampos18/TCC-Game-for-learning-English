using System.Collections.Generic;
using System.Linq;

internal static class MissoesVisitorCenter
{
    internal const string Cenario = "Visitor Center";
    internal const string NpcKey = "visitor_center_npc";
    internal const string CityPark = "city_park";
    internal const string FalaB1 = "visitor_center_fala_1_encerrada";

    internal static HashSet<string> ChavesEsperadas(string nivel) =>
        new HashSet<string>(Definicoes(nivel).Select(d => d.mission_key));

    internal static string ChaveChegada(string nivel) => Definicoes(nivel).LastOrDefault()?.mission_key;

    internal static CatalogoMissoes Criar(string nivel)
    {
        var definicoes = Definicoes(nivel);
        return definicoes.Count == 0 ? null : new CatalogoMissoes(definicoes, nivel, Cenario, catalogoLocal: true);
    }

    internal static bool ValidaEvento(CatalogoMissoes.Evento evento)
    {
        switch (evento.tipo)
        {
            case "npc_interagido":
            case "conversa_encerrada": return evento.alvo == NpcKey;
            case "marco_fluxo": return evento.alvo == FalaB1;
            case "permanencia_confirmada": return evento.alvo == CityPark;
            case "exercicio_encerrado":
                return evento.alvo == AlvoExercicio("walking_time", 1) || evento.alvo == AlvoExercicio("walking_time", 2)
                    || evento.alvo == AlvoExercicio("destination_choice", 1) || evento.alvo == AlvoExercicio("first_visit", 1)
                    || evento.alvo == AlvoExercicio("transportation", 1);
            default: return false;
        }
    }

    internal static string AlvoExercicio(string ponto, int ordem) => $"{NpcKey}/{ponto}/{ordem}";

    private static CatalogoMissoes.Regra Regra(string tipo, string alvo) =>
        new CatalogoMissoes.Regra
        {
            operador = "EVENTO",
            eventos = new List<CatalogoMissoes.Evento> { new CatalogoMissoes.Evento { tipo = tipo, alvo = alvo } }
        };

    private static CatalogoMissoes.Regra Exercicio(string ponto, int ordem) =>
        Regra("exercicio_encerrado", AlvoExercicio(ponto, ordem));

    private static List<CatalogoMissoes.Definicao> Definicoes(string nivel)
    {
        var missoes = new List<CatalogoMissoes.Definicao>();
        var clique = Regra("npc_interagido", NpcKey);
        var parque = Regra("permanencia_confirmada", CityPark);

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
                Adicionar("visitor_center_a1_conversa", "Start a conversation with the visitor center employee.",
                    "Inicie uma conversa com o funcionário do centro de informações turísticas.", clique);
                Adicionar("visitor_center_a1_atracoes", "Learn about the city's tourist attractions.",
                    "Conheça as atrações turísticas da cidade.", Regra("conversa_encerrada", NpcKey));
                Adicionar("visitor_center_a1_city_park", "Find the City Park.",
                    "Encontre o parque da cidade.", parque);
                break;
            case "A1-A2":
                Adicionar("visitor_center_a1a2_conversa", "Talk to the tourist information employee.",
                    "Converse com o funcionário do centro de informações turísticas.", clique);
                Adicionar("visitor_center_a1a2_atracoes", "Talk about the City Park.",
                    "Converse sobre o parque da cidade.", Exercicio("walking_time", 1));
                Adicionar("visitor_center_a1a2_city_park", "Follow the directions and find the City Park.",
                    "Siga as orientações e encontre o parque da cidade.", parque);
                break;
            case "A2":
                Adicionar("visitor_center_a2_conversa", "Ask the visitor center employee for information.",
                    "Peça informações ao funcionário do centro de informações turísticas.", clique);
                Adicionar("visitor_center_a2_atracoes", "Choose the City Park as your destination.",
                    "Escolha o parque da cidade como destino.", Exercicio("destination_choice", 1));
                Adicionar("visitor_center_a2_informacoes_parque", "Find out more about the City Park.",
                    "Obtenha mais informações sobre o parque da cidade.", Exercicio("walking_time", 2));
                Adicionar("visitor_center_a2_destino", "Follow the directions to reach the City Park.",
                    "Siga as orientações para chegar ao parque da cidade.", parque);
                break;
            case "A2-B1":
                Adicionar("visitor_center_a2b1_conversa", "Ask the visitor center employee for sightseeing recommendations.",
                    "Peça recomendações de atrações turísticas ao funcionário do centro de informações.", Exercicio("first_visit", 1));
                Adicionar("visitor_center_a2b1_escolher_destino", "Choose the City Park and ask for directions to get there.",
                    "Escolha o parque da cidade e peça orientações para chegar até lá.", Exercicio("transportation", 1));
                Adicionar("visitor_center_a2b1_percorrer_trajeto", "Interpret the directions and make your way to the City Park.",
                    "Interprete as orientações e siga o trajeto até o parque da cidade.", parque);
                break;
            case "B1":
                Adicionar("visitor_center_b1_conversa", "Engage with the visitor center employee to discuss local attractions.",
                    "Converse com o funcionário do centro de informações sobre as atrações locais.", clique);
                Adicionar("visitor_center_b1_escolher_city_park", "Select the City Park as your preferred destination.",
                    "Selecione o parque da cidade como seu destino de preferência.", Regra("marco_fluxo", FalaB1));
                Adicionar("visitor_center_b1_percorrer_trajeto", "Understand the detailed directions and navigate to the City Park.",
                    "Compreenda as orientações detalhadas e siga o caminho até o parque da cidade.", parque);
                break;
        }

        return missoes;
    }
}
