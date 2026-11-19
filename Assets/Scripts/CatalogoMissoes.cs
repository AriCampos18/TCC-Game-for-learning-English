using System;
using System.Collections.Generic;
using System.Linq;

internal sealed class CatalogoMissoes
{
    internal sealed class Resposta
    {
        public List<Definicao> data = null;
    }

    internal sealed class Definicao
    {
        public string mission_key = null, nivel = null, texto = null, traducao = null;
        public int ordem = 0, id_cenario = 0;
        public Regra inicio = null, conclusao = null;
    }

    internal sealed class Regra
    {
        public string operador = null;
        public List<Evento> eventos = null;
    }

    internal sealed class Evento
    {
        public string tipo = null, alvo = null;
        public (string, string) Chave => (tipo, alvo);
    }

    private readonly HashSet<string> ativas = new HashSet<string>();
    private readonly HashSet<string> concluidas = new HashSet<string>();
    private readonly HashSet<(string, string)> eventosInicio = new HashSet<(string, string)>();
    private readonly Dictionary<string, HashSet<(string, string)>> eventosConclusao =
        new Dictionary<string, HashSet<(string, string)>>();

    public IReadOnlyList<Definicao> Definicoes { get; }

    public CatalogoMissoes(List<Definicao> definicoes, string nivel)
    {
        string identificadores;
        switch (nivel)
        {
            case "A1": identificadores = "1 2 3 4 5"; break;
            case "A1-A2": identificadores = "1 2 3 4 5"; break;
            case "A2": identificadores = "1 2 3 4 5 6"; break;
            case "A2-B1": identificadores = "1 2 3 destino 4 5 6 7"; break;
            case "B1": identificadores = "1 2 3 4 5 6 8"; break;
            default: throw new InvalidOperationException("Nível sem catálogo Airport.");
        }

        string prefixo = "aeroporto_" + nivel.ToLowerInvariant().Replace("-", "") + "_";
        var chaves = new HashSet<string>(identificadores.Split(' ').Select(id => prefixo + id));
        if (definicoes == null || definicoes.Count != chaves.Count
            || !definicoes.All(d => d != null && d.nivel == nivel && d.id_cenario > 0
                && !string.IsNullOrWhiteSpace(d.texto) && !string.IsNullOrWhiteSpace(d.traducao)
                && chaves.Remove(d.mission_key)))
            throw new InvalidOperationException("Catálogo Airport incompleto ou inválido.");

        Definicoes = definicoes.OrderBy(d => d.ordem).ToList();
        int cenario = Definicoes[0].id_cenario;
        var anteriores = new HashSet<string>();
        for (int i = 0; i < Definicoes.Count; i++)
        {
            Definicao d = Definicoes[i];
            if (d.ordem != i + 1 || d.id_cenario != cenario
                || !Valida(d.inicio, anteriores) || !Valida(d.conclusao, anteriores))
                throw new InvalidOperationException("Regra ou ordem inválida em " + d.mission_key);
            anteriores.Add(d.mission_key);
        }
    }

    private static bool Valida(Regra regra, HashSet<string> anteriores)
    {
        if (regra?.eventos == null || (regra.operador != "EVENTO" && regra.operador != "AND")
            || (regra.operador == "EVENTO" ? regra.eventos.Count != 1 : regra.eventos.Count < 2))
            return false;

        var chaves = new HashSet<(string, string)>();
        foreach (Evento e in regra.eventos)
        {
            if (e == null || string.IsNullOrWhiteSpace(e.alvo) || !chaves.Add(e.Chave))
                return false;
            switch (e.tipo)
            {
                case "cenario_iniciado": if (e.alvo != "Airport") return false; break;
                case "missao_concluida": if (!anteriores.Contains(e.alvo)) return false; break;
                case "npc_interagido":
                case "conversa_encerrada": if (e.alvo != "airport_info_npc" && e.alvo != "checkin_npc") return false; break;
                case "cartao_embarque_consultado": if (e.alvo != "cartao_embarque") return false; break;
                case "permanencia_confirmada": if (e.alvo != "portao_12" && e.alvo != "portao_18" && e.alvo != "portao_24") return false; break;
                case "marco_fluxo": if (e.alvo != "documentos_solicitados" && e.alvo != "portao_liberado") return false; break;
                case "exercicio_encerrado":
                    string[] partes = e.alvo.Split('/');
                    if (partes.Length != 3 || (partes[0] != "airport_info_npc" && partes[0] != "checkin_npc")
                        || string.IsNullOrWhiteSpace(partes[1]) || !int.TryParse(partes[2], out int ordem) || ordem < 1)
                        return false;
                    break;
                default: return false;
            }
        }

        return true;
    }

    public void Registrar(string tipo, string alvo, Action<Definicao> iniciar, Action<string> concluir)
    {
        var pendentes = new Queue<(string, string)>();
        pendentes.Enqueue((tipo, alvo));
        while (pendentes.Count > 0)
        {
            var evento = pendentes.Dequeue();
            eventosInicio.Add(evento);

            foreach (Definicao d in Definicoes)
            {
                string id = d.mission_key;
                if (!ativas.Contains(id) || !d.conclusao.eventos.Any(e => e.Chave == evento))
                    continue;

                var satisfeitos = eventosConclusao[id];
                satisfeitos.Add(evento);
                if (!d.conclusao.eventos.All(e => satisfeitos.Contains(e.Chave)))
                    continue;

                ativas.Remove(id);
                concluidas.Add(id);
                eventosConclusao.Remove(id);
                concluir(id);
                pendentes.Enqueue(("missao_concluida", id));
            }

            foreach (Definicao d in Definicoes)
            {
                string id = d.mission_key;
                if (ativas.Contains(id) || concluidas.Contains(id)
                    || !d.inicio.eventos.All(e => eventosInicio.Contains(e.Chave)))
                    continue;

                ativas.Add(id);
                eventosConclusao[id] = new HashSet<(string, string)>();
                iniciar(d);
            }
        }
    }
}
