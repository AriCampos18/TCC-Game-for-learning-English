using System.Collections.Generic;
using System.Linq;

internal static class MissoesHome
{
    internal const string Cenario = "Home";

    internal static HashSet<string> ChavesEsperadas(string nivel) =>
        new HashSet<string>(Definicoes(nivel).Select(d => d.mission_key));

    internal static string UltimaMissao(string nivel) =>
        Definicoes(nivel).LastOrDefault()?.mission_key;

    internal static CatalogoMissoes Criar(string nivel)
    {
        var definicoes = Definicoes(nivel);
        return definicoes.Count == 0 ? null : new CatalogoMissoes(definicoes, nivel, Cenario, catalogoLocal: true);
    }

    internal static bool ValidaEvento(CatalogoMissoes.Evento evento)
    {
        switch (evento.tipo)
        {
            case "conversa_encerrada": return evento.alvo == "brother_npc";
            case "permanencia_confirmada":
                return evento.alvo == "banheiro" || evento.alvo == "quarto" || evento.alvo == "cozinha";
            case "marco_fluxo":
                return evento.alvo == "mae_fala_3" || evento.alvo == "pai_fala_0" || evento.alvo == "pai_fala_1"
                    || evento.alvo == "irmao_fala_2" || evento.alvo == "jantar_concluido"
                    || evento.alvo == "dialogos_concluidos" || evento.alvo == "saida_home";
            case "exercicio_encerrado":
                string[] partes = evento.alvo.Split('/');
                return partes.Length == 3 && (partes[0] == "mother_npc" || partes[0] == "father_npc" || partes[0] == "brother_npc")
                    && !string.IsNullOrWhiteSpace(partes[1]) && int.TryParse(partes[2], out int ordem) && ordem > 0;
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

    private static CatalogoMissoes.Regra Regra(string tipo, string alvo) => Regra(Evento(tipo, alvo));
    private static CatalogoMissoes.Regra Marco(string alvo) => Regra("marco_fluxo", alvo);
    private static CatalogoMissoes.Regra Exercicio(string npc, string ponto, int ordem) =>
        Regra("exercicio_encerrado", $"{npc}/{ponto}/{ordem}");
    private static CatalogoMissoes.Regra Despedida() =>
        Regra(Evento("marco_fluxo", "dialogos_concluidos"), Evento("marco_fluxo", "saida_home"));

    private static List<CatalogoMissoes.Definicao> Definicoes(string nivel)
    {
        var missoes = new List<CatalogoMissoes.Definicao>();
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
                Adicionar("casa_a1_chegada", "Greet your family.",
                    "Cumprimente sua família.", Exercicio("mother_npc", "greeting", 1));
                Adicionar("casa_a1_dia_mae", "Tell your mother about your day.",
                    "Conte à sua mãe sobre seu dia.", Exercicio("mother_npc", "day_conversation", 2));
                Adicionar("casa_a1_banheiro", "Visit the renovated bathroom.",
                    "Visite o banheiro reformado.", Regra("permanencia_confirmada", "banheiro"));
                Adicionar("casa_a1_trabalho_mae", "Talk to your mother about her work.",
                    "Converse com sua mãe sobre o trabalho dela.", Exercicio("mother_npc", "work_conversation", 4));
                Adicionar("casa_a1_irmao", "Talk to your brother about school and the family dog.",
                    "Converse com seu irmão sobre a escola e o cachorro da família.", Regra("conversa_encerrada", "brother_npc"));
                Adicionar("casa_a1_jantar", "Sit down at the dinner table.",
                    "Sente-se à mesa para jantar.", Marco("jantar_concluido"));
                Adicionar("casa_a1_despedida", "Say goodbye to your family.",
                    "Despeça-se da sua família.", Despedida());
                break;
            case "A1-A2":
                Adicionar("casa_a1a2_chegada", "Say hello to your family when you arrive home.",
                    "Cumprimente seus familiares ao chegar em casa.", Exercicio("mother_npc", "greeting", 1));
                Adicionar("casa_a1a2_dia", "Share what you did today with your mother.",
                    "Compartilhe com sua mãe o que você fez hoje.", Exercicio("mother_npc", "day_conversation", 2));
                Adicionar("casa_a1a2_banheiro", "Take a look at the newly renovated bathroom.",
                    "Dê uma olhada no banheiro recém-reformado.", Regra("permanencia_confirmada", "banheiro"));
                Adicionar("casa_a1a2_trabalho_mae", "Ask your mother about her work.",
                    "Pergunte à sua mãe sobre o trabalho dela.", Marco("mae_fala_3"));
                Adicionar("casa_a1a2_pai", "Find out what your father does at work.",
                    "Descubra o que seu pai faz no trabalho.", Marco("pai_fala_1"));
                Adicionar("casa_a1a2_irmao", "Talk to your brother about his school day and the dog.",
                    "Converse com seu irmão sobre o dia na escola e o cachorro.", Regra("conversa_encerrada", "brother_npc"));
                Adicionar("casa_a1a2_jantar", "Join your family at the table for dinner.",
                    "Junte-se à sua família à mesa para jantar.", Marco("jantar_concluido"));
                Adicionar("casa_a1a2_despedida", "Say goodbye to everyone before leaving.",
                    "Despeça-se de todos antes de sair.", Despedida());
                break;
            case "A2":
                Adicionar("casa_a2_chegada", "Welcome your family and start a conversation.",
                    "Cumprimente sua família e inicie uma conversa.", Exercicio("mother_npc", "greeting", 1));
                Adicionar("casa_a2_dia_mae", "Describe how your day went to your mother.",
                    "Conte à sua mãe como foi seu dia.", Exercicio("mother_npc", "day_conversation", 2));
                Adicionar("casa_a2_comodo", "Explore the changes made to the bedroom.",
                    "Explore as mudanças feitas no quarto.", Regra("permanencia_confirmada", "quarto"));
                Adicionar("casa_a2_pai_trabalho", "Ask your father about his job.",
                    "Pergunte ao seu pai sobre seu trabalho.", Marco("pai_fala_0"));
                Adicionar("casa_a2_irmao", "Catch up with your brother about school and the dog.",
                    "Converse com seu irmão sobre a escola e o cachorro.", Marco("irmao_fala_2"));
                Adicionar("casa_a2_animais", "Share your experiences with pets.",
                    "Compartilhe suas experiências com animais de estimação.", Exercicio("brother_npc", "pets", 8));
                Adicionar("casa_a2_jantar", "Have dinner with your family.",
                    "Jante com sua família.", Marco("jantar_concluido"));
                Adicionar("casa_a2_despedida", "Thank your family and say goodbye.",
                    "Agradeça à sua família e despeça-se.", Despedida());
                break;
            case "A2-B1":
                Adicionar("casa_a2b1_chegada", "Greet your family and catch up with them.",
                    "Cumprimente sua família e coloque a conversa em dia.", Exercicio("mother_npc", "routine", 3));
                Adicionar("casa_a2b1_rotina_pai", "Talk to your father about your recent routine.",
                    "Converse com seu pai sobre sua rotina recente.", Exercicio("father_npc", "greeting", 2));
                Adicionar("casa_a2b1_banheiro", "Discuss the bathroom renovation with your mother.",
                    "Converse com sua mãe sobre a reforma do banheiro.", Regra(Evento("permanencia_confirmada", "banheiro"), Evento("exercicio_encerrado", "mother_npc/home_changes/5")));
                Adicionar("casa_a2b1_trabalho_pai", "Ask your father about his current project.",
                    "Pergunte ao seu pai sobre o projeto atual.", Marco("pai_fala_1"));
                Adicionar("casa_a2b1_trabalho_mae", "Find out how things are going at your mother's job.",
                    "Descubra como estão as coisas no trabalho da sua mãe.", Exercicio("mother_npc", "work_conversation", 7));
                Adicionar("casa_a2b1_irmao", "Ask your brother how school has been going.",
                    "Pergunte ao seu irmão como estão indo as coisas na escola.", Exercicio("brother_npc", "pets", 10));
                Adicionar("casa_a2b1_animais", "Talk with your brother about activities involving pets.",
                    "Converse com seu irmão sobre atividades com animais de estimação.", Regra("conversa_encerrada", "brother_npc"));
                Adicionar("casa_a2b1_jantar", "Join your family for dinner after your conversations.",
                    "Junte-se à família para jantar depois das conversas.", Marco("jantar_concluido"));
                Adicionar("casa_a2b1_despedida", "Thank everyone for dinner and say goodbye before leaving.",
                    "Agradeça a todos pelo jantar e despeça-se antes de sair.", Despedida());
                break;
            case "B1":
                Adicionar("casa_b1_rotina_pai", "Talk to your father about your recent routine.",
                    "Converse com seu pai sobre sua rotina recente.", Exercicio("father_npc", "greeting", 1));
                Adicionar("casa_b1_trabalho_pai", "Discuss your father's work responsibilities.",
                    "Converse sobre as responsabilidades profissionais do seu pai.", Marco("pai_fala_1"));
                Adicionar("casa_b1_cozinha", "Explore the recent changes in the kitchen.",
                    "Explore as mudanças recentes na cozinha.", Regra("permanencia_confirmada", "cozinha"));
                Adicionar("casa_b1_trabalho_mae", "Ask your mother about her professional life.",
                    "Pergunte à sua mãe sobre sua vida profissional.", Exercicio("mother_npc", "work_conversation", 4));
                Adicionar("casa_b1_irmao", "Catch up with your brother about school and the dog.",
                    "Converse com seu irmão sobre a escola e o cachorro.", Marco("irmao_fala_2"));
                Adicionar("casa_b1_animais", "Share your experiences with animals.",
                    "Compartilhe suas experiências com animais.", Exercicio("brother_npc", "pets", 7));
                Adicionar("casa_b1_jantar_planos", "Have dinner and discuss your plans for the evening.",
                    "Jante e converse sobre seus planos para a noite.", Regra(Evento("marco_fluxo", "jantar_concluido"), Evento("exercicio_encerrado", "father_npc/dinner_plans/8")));
                Adicionar("casa_b1_despedida", "Thank your family for the evening and say goodbye.",
                    "Agradeça à sua família pela noite e despeça-se.", Exercicio("mother_npc", "farewell", 9));
                break;
        }

        return missoes;
    }
}
