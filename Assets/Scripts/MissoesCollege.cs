using System.Collections.Generic;

internal static class MissoesCollege
{
    internal const string Cenario = "College";
    internal const string Recepcao = "recepcionist_college_npc";
    internal const string Secretaria = "secretary_college_npc";
    internal const string InicioConversa = "npc_interagido";
    internal const string FimConversa = "conversa_encerrada";
    internal const string ExercicioEncerrado = "exercicio_encerrado";
    internal const string InicioAulas = Secretaria + "/class_start/1";
    internal const string Marco = "marco_fluxo";
    internal const string DocumentoConsultado = "documento_curso_consultado";

    private static string[] Chaves(string nivel)
    {
        switch (nivel)
        {
            case "A1": return new[] { "universidade_a1_recepcao", "universidade_a1_matricula" };
            case "A1-A2": return new[] { "universidade_a1a2_recepcao", "universidade_a1a2_matricula" };
            case "A2": return new[] { "universidade_a2_recepcao", "universidade_a2_encontrar_secretaria", "universidade_a2_matricula" };
            case "A2-B1": return new[] { "universidade_a2b1_orientacoes", "universidade_a2b1_matricula" };
            case "B1": return new[] { "universidade_b1_orientacoes", "universidade_b1_matricula", "universidade_b1_cronograma" };
            default: return new string[0];
        }
    }

    internal static HashSet<string> ChavesEsperadas(string nivel) => new HashSet<string>(Chaves(nivel));

    internal static bool ValidaEvento(CatalogoMissoes.Evento evento)
    {
        switch (evento.tipo)
        {
            case InicioConversa: return evento.alvo == Secretaria;
            case FimConversa: return evento.alvo == Recepcao || evento.alvo == Secretaria;
            case ExercicioEncerrado: return evento.alvo == InicioAulas;
            case Marco: return evento.alvo == DocumentoConsultado;
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

    internal static CatalogoMissoes Criar(string nivel)
    {
        string[] chaves = Chaves(nivel);
        if (chaves.Length == 0)
            return null;

        var missoes = new List<CatalogoMissoes.Definicao>();
        var recepcao = Evento(FimConversa, Recepcao);
        var secretaria = Evento(FimConversa, Secretaria);
        var documento = Evento(Marco, DocumentoConsultado);

        void Adicionar(string texto, string traducao, params CatalogoMissoes.Evento[] eventos)
        {
            int i = missoes.Count;
            missoes.Add(new CatalogoMissoes.Definicao
            {
                mission_key = chaves[i], nivel = nivel, ordem = i + 1, texto = texto, traducao = traducao,
                inicio = i == 0 ? Regra(Evento("cenario_iniciado", Cenario))
                    : Regra(Evento("missao_concluida", chaves[i - 1])),
                conclusao = Regra(eventos)
            });
        }

        switch (nivel)
        {
            case "A1":
                Adicionar("Talk to the receptionist and ask how to get to the secretary's office.",
                    "Converse com a recepcionista e pergunte como chegar ao escritório da secretaria.", recepcao);
                Adicionar("Enroll in your course.", "Faça sua matrícula no curso.", secretaria);
                break;
            case "A1-A2":
                Adicionar("Talk to the receptionist and find out how to get to the secretary's office.",
                    "Converse com a recepcionista e descubra como chegar ao escritório da secretaria.", recepcao);
                Adicionar("Complete your enrollment and check your course information.",
                    "Conclua sua matrícula e consulte as informações do curso.", secretaria, documento);
                break;
            case "A2":
                Adicionar("Talk to the receptionist.", "Converse com a recepcionista.", recepcao);
                Adicionar("Find the secretary's office.", "Encontre o escritório da secretaria.", Evento(InicioConversa, Secretaria));
                Adicionar("Complete your enrollment.", "Faça sua matrícula.", secretaria);
                break;
            case "A2-B1":
                Adicionar("Ask the receptionist for detailed directions to the secretary's office.",
                    "Peça orientações detalhadas à recepcionista para chegar ao escritório da secretaria.", recepcao);
                Adicionar("Confirm your details, complete your enrollment, and check the information about your first class.",
                    "Confirme seus dados, conclua a matrícula e consulte as informações da primeira aula.", secretaria, documento);
                break;
            case "B1":
                Adicionar("Ask the receptionist for detailed directions and clarify where the secretary's office is.",
                    "Peça orientações detalhadas à recepcionista e esclareça dúvidas sobre a localização do escritório da secretaria.", recepcao);
                Adicionar("Provide your personal details, confirm the course information, and complete the enrollment process.",
                    "Informe seus dados, confirme as informações do curso e conclua o processo de matrícula.", Evento(ExercicioEncerrado, InicioAulas));
                Adicionar("Check your schedule, identify the room for your first class, and ask how to get there.",
                    "Consulte o cronograma, identifique a sala da primeira aula e pergunte como chegar até ela.", secretaria);
                break;
        }

        return new CatalogoMissoes(missoes, nivel, Cenario, catalogoLocal: true);
    }
}
