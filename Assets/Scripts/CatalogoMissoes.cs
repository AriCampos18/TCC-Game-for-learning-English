using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

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

    internal JArray CapturarEventosCheckpoint(string chave)
    {
        return eventosConclusao.TryGetValue(chave, out var eventos)
            ? new JArray(eventos.OrderBy(e => e.Item1).ThenBy(e => e.Item2)
                .Select(e => new JObject { ["tipo"] = e.Item1, ["alvo"] = e.Item2 }))
            : new JArray();
    }

    public CatalogoMissoes(List<Definicao> definicoes, string nivel, string nomeCenario = "Airport",
        bool catalogoLocal = false)
    {
        var chaves = ChavesEsperadas(nomeCenario, nivel);
        if (definicoes == null || definicoes.Count == 0 || definicoes.Count != chaves.Count
            || !definicoes.All(d => d != null && d.nivel == nivel && (catalogoLocal || d.id_cenario == SessaoCenario.Id(nomeCenario))
                && !string.IsNullOrWhiteSpace(d.texto) && !string.IsNullOrWhiteSpace(d.traducao)
                && chaves.Remove(d.mission_key)))
            throw new InvalidOperationException("Catálogo incompleto ou inválido.");

        Definicoes = definicoes.OrderBy(d => d.ordem).ToList();
        int cenario = Definicoes[0].id_cenario;
        var anteriores = new HashSet<string>();
        for (int i = 0; i < Definicoes.Count; i++)
        {
            Definicao d = Definicoes[i];
            if (d.ordem != i + 1 || d.id_cenario != cenario
                || !Valida(d.inicio, anteriores, nomeCenario) || !Valida(d.conclusao, anteriores, nomeCenario))
                throw new InvalidOperationException("Regra ou ordem inválida em " + d.mission_key);
            anteriores.Add(d.mission_key);
        }
    }

    internal static bool ChaveDoCenario(string nome, string nivel, string chave) =>
        SessaoCenario.Conhecido(nome) && ChavesEsperadas(nome, nivel).Contains(chave);

    internal static HashSet<string> ChavesEsperadas(string nomeCenario, string nivel)
    {
        if (nomeCenario == MissoesVisitorCenter.Cenario)
            return MissoesVisitorCenter.ChavesEsperadas(nivel);

        if (nomeCenario == MissoesRestaurant.Cenario)
            return MissoesRestaurant.ChavesEsperadas(nivel);

        if (nomeCenario == MissoesHome.Cenario)
            return MissoesHome.ChavesEsperadas(nivel);

        if (nomeCenario == MissoesHotel.Cenario)
            return MissoesHotel.ChavesEsperadas(nivel);

        if (nomeCenario == MissoesCollege.Cenario)
            return MissoesCollege.ChavesEsperadas(nivel);

        if (nomeCenario == MissoesClothingStore.Cenario)
            return MissoesClothingStore.ChavesEsperadas(nivel);

        if (nomeCenario == "Market")
            return new HashSet<string> { "pegar_cesta", "falar_padaria", "falar_atendente",
                "seguir_atendente", "interagir_produtos", "falar_caixa", "pegar_sacola" };

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
        return new HashSet<string>(identificadores.Split(' ').Select(id => prefixo + id));
    }

    private static bool ValidaAlvoMarket(Evento evento)
    {
        switch (evento.tipo)
        {
            case "npc_interagido":
            case "conversa_encerrada":
                return evento.alvo == "bakery_npc" || evento.alvo == "shelf_npc" || evento.alvo == "cashier_npc";
            case "marco_fluxo":
                return evento.alvo == "cesta_coletada" || evento.alvo == "rota_prateleiras_iniciada"
                    || evento.alvo == "rota_prateleiras_concluida" || evento.alvo == "produtos_liberados"
                    || evento.alvo == "produtos_obrigatorios_coletados" || evento.alvo == "sacola_liberada"
                    || evento.alvo == "sacola_coletada" || evento.alvo == "saida_market";
            default:
                return false;
        }
    }

    private static bool Valida(Regra regra, HashSet<string> anteriores, string nomeCenario)
    {
        if (regra?.eventos == null || (regra.operador != "EVENTO" && regra.operador != "AND")
            || (regra.operador == "EVENTO" ? regra.eventos.Count != 1 : regra.eventos.Count < 2))
            return false;

        var chaves = new HashSet<(string, string)>();
        foreach (Evento e in regra.eventos)
        {
            if (e == null || string.IsNullOrWhiteSpace(e.alvo) || !chaves.Add(e.Chave))
                return false;
            if (nomeCenario == "Market" && e.tipo != "cenario_iniciado" && e.tipo != "missao_concluida")
            {
                if (!ValidaAlvoMarket(e))
                    return false;
                continue;
            }
            if (nomeCenario == MissoesClothingStore.Cenario &&
                e.tipo != "cenario_iniciado" && e.tipo != "missao_concluida")
            {
                if (!MissoesClothingStore.ValidaEvento(e))
                    return false;
                continue;
            }
            if (nomeCenario == MissoesCollege.Cenario &&
                e.tipo != "cenario_iniciado" && e.tipo != "missao_concluida")
            {
                if (!MissoesCollege.ValidaEvento(e))
                    return false;
                continue;
            }
            if (nomeCenario == MissoesHotel.Cenario &&
                e.tipo != "cenario_iniciado" && e.tipo != "missao_concluida")
            {
                if (!MissoesHotel.ValidaEvento(e))
                    return false;
                continue;
            }
            if (nomeCenario == MissoesHome.Cenario &&
                e.tipo != "cenario_iniciado" && e.tipo != "missao_concluida")
            {
                if (!MissoesHome.ValidaEvento(e))
                    return false;
                continue;
            }
            if (nomeCenario == MissoesRestaurant.Cenario &&
                e.tipo != "cenario_iniciado" && e.tipo != "missao_concluida")
            {
                if (!MissoesRestaurant.ValidaEvento(e))
                    return false;
                continue;
            }
            if (nomeCenario == MissoesVisitorCenter.Cenario &&
                e.tipo != "cenario_iniciado" && e.tipo != "missao_concluida")
            {
                if (!MissoesVisitorCenter.ValidaEvento(e))
                    return false;
                continue;
            }
            switch (e.tipo)
            {
                case "cenario_iniciado": if (e.alvo != nomeCenario) return false; break;
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

    private static string ChaveInicioCheckpoint((string, string) evento) =>
        "start." + RoteiroNpcCheckpoint.Hash(evento.Item1 + "|" + evento.Item2);

    internal bool InicioRegistradoCheckpoint(string tipo, string alvo) => eventosInicio.Contains((tipo, alvo));

    internal JArray CapturarIniciosCheckpoint() => new JArray(Definicoes.SelectMany(d => d.inicio.eventos)
        .Select(e => e.Chave).Distinct().Where(eventosInicio.Contains).Select(ChaveInicioCheckpoint));

    internal bool RestaurarRoteiro(JArray missoes, JArray inicios, Action<Definicao> iniciar, Action<string> concluir, Action limpar = null)
    {
        var definicoes = Definicoes.ToDictionary(d => d.mission_key);
        var inicioPermitido = Definicoes.SelectMany(d => d.inicio.eventos).Select(e => e.Chave).Distinct()
            .ToDictionary(ChaveInicioCheckpoint);
        var eventosRestaurados = new HashSet<(string, string)>();
        foreach (var token in inicios)
        {
            if (!inicioPermitido.TryGetValue((string)token ?? "", out var evento) || !eventosRestaurados.Add(evento))
                return false;
        }

        var estados = new Dictionary<string, (bool concluida, HashSet<(string, string)> eventos)>();
        foreach (var token in missoes)
        {
            string chave = (string)token["mission_key"] ?? "";
            string status = (string)token["status"];
            if (!definicoes.TryGetValue(chave, out var d) || estados.ContainsKey(chave)
                || (status != "completed" && status != "active") || !(token["satisfied_events"] is JArray eventos)
                || !d.inicio.eventos.All(e => eventosRestaurados.Contains(e.Chave)))
                return false;

            var parciais = new HashSet<(string, string)>();
            foreach (var item in eventos)
            {
                var evento = ((string)item["tipo"], (string)item["alvo"]);
                if (!d.conclusao.eventos.Any(e => e.Chave == evento) || !parciais.Add(evento))
                    return false;
            }
            bool concluida = status == "completed";
            if (concluida ? parciais.Count != 0 : d.conclusao.eventos.All(e => parciais.Contains(e.Chave)))
                return false;
            estados.Add(chave, (concluida, parciais));
        }

        limpar?.Invoke();
        ativas.Clear();
        concluidas.Clear();
        eventosInicio.Clear();
        eventosInicio.UnionWith(eventosRestaurados);
        eventosConclusao.Clear();
        foreach (var token in missoes)
        {
            string chave = (string)token["mission_key"];
            var estado = estados[chave];
            iniciar(definicoes[chave]);
            if (estado.concluida)
            {
                concluidas.Add(chave);
                concluir(chave);
            }
            else
            {
                ativas.Add(chave);
                eventosConclusao[chave] = estado.eventos;
            }
        }
        return true;
    }

    internal bool RestaurarMarco(JArray missoes, bool todasConcluidas, Action<Definicao> iniciar, Action<string> concluir,
        string missaoAtiva = null)
    {
        if (Definicoes.Count == 0 || (!todasConcluidas && missaoAtiva == null && Definicoes.Count != 2)
            || (todasConcluidas && missaoAtiva != null))
            return false;
        string chaveAtiva = todasConcluidas ? null : missaoAtiva ?? Definicoes.Last().mission_key;
        int quantidade = todasConcluidas ? Definicoes.Count
            : Definicoes.TakeWhile(d => d.mission_key != chaveAtiva).Count() + 1;
        if (quantidade > Definicoes.Count || missoes.Count != quantidade)
            return false;
        var eventosParciais = new HashSet<(string, string)>();
        for (int i = 0; i < quantidade; i++)
        {
            bool ativa = Definicoes[i].mission_key == chaveAtiva;
            string estado = ativa ? "active" : "completed";
            var entrada = missoes.OfType<JObject>().FirstOrDefault(m =>
                (string)m["mission_key"] == Definicoes[i].mission_key);
            if (entrada == null || (string)entrada["status"] != estado
                || !(entrada["satisfied_events"] is JArray eventos) || (!ativa && eventos.Count != 0))
                return false;
            foreach (var evento in eventos)
            {
                if (!(evento is JObject objeto) || objeto.Count != 2)
                    return false;
                var chaveEvento = ((string)objeto["tipo"], (string)objeto["alvo"]);
                if (!Definicoes[i].conclusao.eventos.Any(e => e.Chave == chaveEvento) || !eventosParciais.Add(chaveEvento))
                    return false;
            }
            if (ativa && Definicoes[i].conclusao.eventos.All(e => eventosParciais.Contains(e.Chave)))
                return false;
        }

        ativas.Clear();
        concluidas.Clear();
        eventosInicio.Clear();
        eventosConclusao.Clear();
        foreach (var definicao in Definicoes.Take(quantidade))
        {
            iniciar(definicao);
            foreach (var evento in definicao.inicio.eventos)
                eventosInicio.Add(evento.Chave);
            if (definicao.mission_key != chaveAtiva)
            {
                concluidas.Add(definicao.mission_key);
                concluir(definicao.mission_key);
                eventosInicio.Add(("missao_concluida", definicao.mission_key));
            }
            else
            {
                ativas.Add(definicao.mission_key);
                eventosConclusao[definicao.mission_key] = eventosParciais;
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
