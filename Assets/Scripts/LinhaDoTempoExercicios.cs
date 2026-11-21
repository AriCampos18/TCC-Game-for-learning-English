using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

internal sealed class ConteudoRoteiro
{
    internal List<string> Falas;
    internal readonly List<ExercicioCompletarFrase> FrasesTipadas = new();
    internal readonly List<ExercicioBase> exerciciosCompletarFrase = new();
    internal readonly List<ExercicioBase> exerciciosCriarFrases = new();
    internal readonly List<ExercicioBase> exerciciosSpeaking = new();
    internal readonly List<ExercicioBase> exerciciosAlternativas = new();
    internal readonly List<ExercicioBase> exerciciosWriting = new();
    internal readonly List<ExercicioBase> exerciciosAssociacao = new();
}

internal enum TipoEtapaRoteiro { Fala, Exercicio, Marco, Secao, Lacuna }

internal sealed class DescricaoEtapaRoteiro
{
    internal readonly TipoEtapaRoteiro Tipo;
    internal readonly string Chave, Npc, Texto, Ponto;
    internal readonly int Ordem;
    internal readonly bool AncoraDeInsercao;
    internal readonly Type ModeloEsperado;
    internal readonly bool? SpeakingLivre;

    internal DescricaoEtapaRoteiro(TipoEtapaRoteiro tipo, string chave, string npc, string texto,
        string ponto = null, int ordem = 0, ExercicioBase modelo = null)
    {
        Tipo = tipo;
        Chave = chave;
        Npc = npc;
        Texto = texto;
        Ponto = ponto;
        Ordem = ordem;
        ModeloEsperado = modelo?.GetType();
        SpeakingLivre = (modelo as ExercicioSpeaking)?.modoLivre;
        AncoraDeInsercao = tipo == TipoEtapaRoteiro.Exercicio && modelo != null && modelo is not ExercicioAssociacao;
    }
}

internal sealed partial class RoteiroNpcCheckpoint
{
    internal IEnumerable<DescricaoEtapaRoteiro> DescreverEtapas()
    {
        foreach (var etapa in etapas)
        {
            var tipo = etapa.Ponto != null ? TipoEtapaRoteiro.Exercicio
                : etapa.Fala != null ? TipoEtapaRoteiro.Fala : TipoEtapaRoteiro.Marco;
            yield return new DescricaoEtapaRoteiro(tipo, etapa.Chave, etapa.Ator?.CheckpointNpcKey ?? npcDescricao, etapa.Fala,
                etapa.Ponto, etapa.Ordem, etapa.Modelo);
            if (etapa.Anotacao != null)
                yield return new DescricaoEtapaRoteiro(TipoEtapaRoteiro.Marco, etapa.Chave + ".annotation",
                    etapa.Ator?.CheckpointNpcKey ?? npcDescricao, etapa.Anotacao);
        }
    }
}

internal sealed class LinhaDoTempoExercicios
{
    internal static readonly string[] Niveis = { "A1", "A1-A2", "A2", "A2-B1", "B1" };
    internal readonly List<DescricaoEtapaRoteiro> Etapas = new();

    internal static LinhaDoTempoExercicios Criar(string cenario, string nivel)
    {
        var linha = new LinhaDoTempoExercicios();
        if (!SessaoCenario.Conhecido(cenario) || !Niveis.Contains(nivel))
            return linha;

        var npcs = UnityEngine.Object.FindObjectsByType<InteracaoNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .Where(npc => npc.Cenario == cenario).ToArray();
        T Unico<T>() where T : InteracaoNPC
        {
            var itens = npcs.OfType<T>().ToArray();
            return itens.Length == 1 ? itens[0] : null;
        }

        void Bloco<T>(string titulo, Func<T, RoteiroNpcCheckpoint> descrever) where T : InteracaoNPC
        {
            linha.Adicionar(TipoEtapaRoteiro.Secao, titulo);
            var roteiros = npcs.OfType<T>().Select(npc => descrever(npc).DescreverEtapas().ToArray()).ToArray();
            if (roteiros.Length == 0)
            {
                linha.Adicionar(TipoEtapaRoteiro.Lacuna, "O NPC de " + titulo + " não está disponível nas cenas carregadas.");
                return;
            }
            var etapas = roteiros[0];
            var assinaturas = etapas.Select(IdentidadeEtapa);
            if (roteiros.Skip(1).Any(r => !assinaturas.SequenceEqual(r.Select(IdentidadeEtapa))))
            {
                linha.Adicionar(TipoEtapaRoteiro.Lacuna, "Há roteiros diferentes para " + titulo + ". A ordem não foi inferida.");
                return;
            }

            foreach (var etapa in etapas)
            {
                linha.Etapas.Add(etapa);
                if (etapa.Chave == "action.table.customer")
                    Bloco<RestaurantClient>("Conversa com o cliente", cliente => cliente.DescreverRoteiro(nivel));
            }
        }

        void Marco(string texto) => linha.Adicionar(TipoEtapaRoteiro.Marco, texto);
        void Nota(string texto) => linha.Adicionar(TipoEtapaRoteiro.Lacuna, texto);
        void Associacao(string npc)
        {
            var associacao = CheckpointJogo.AssociacaoFinal(cenario, nivel);
            if (associacao.Ponto != null)
                linha.AdicionarAssociacao(npc, associacao.Ponto, associacao.Ordem);
        }
        switch (cenario)
        {
            case "Airport":
                if (nivel == "A2")
                    Nota("Atendimento de informações exibido separadamente. O check-in A2 tem liberação própria.");
                Bloco<AirportInfoNPC>("Informações do aeroporto", npc => npc.DescreverRoteiro(nivel));
                Bloco<checkinNPC>("Check-in", npc => npc.DescreverRoteiro(nivel));
                Marco(nivel == "A1" || nivel == "A1-A2" || nivel == "A2-B1"
                    ? "Consulta ao cartão e chegada ao portão, conforme as condições do nível."
                    : "Chegada ao portão.");
                break;
            case "Market":
                Marco("Entrada e coleta da cesta.");
                Nota("Padaria e prateleiras são exibidas em blocos. A ordem entre os atendimentos depende das condições da visita.");
                Bloco<BakeryNPC>("Padaria", npc => npc.DescreverRoteiro(nivel));
                Bloco<ShelfNPC>("Prateleiras", npc => npc.DescreverRoteiro(nivel));
                Marco("Coleta dos produtos e liberação do caixa.");
                Bloco<CashierNPC>("Caixa", npc => npc.DescreverRoteiro(nivel));
                Marco("Coleta da sacola e travessia da saída.");
                Associacao("cashier_npc");
                break;
            case "Clothing Store":
                Bloco<ClothingSellerNPC>("Atendimento na loja", npc => npc.DescreverRoteiro(nivel));
                Nota("A prova pode ocorrer durante as falas do provador ou na espera indicada.");
                break;
            case "Hotel":
                linha.Adicionar(TipoEtapaRoteiro.Secao, "Recepção do hotel");
                linha.Etapas.AddRange(RecepcionistHotelNPC.DescreverRoteiro(nivel).DescreverEtapas());
                if (MissoesHotel.UsaCartao(nivel))
                {
                    Marco("Recolher o cartão do quarto após o atendimento.");
                    linha.AdicionarAssociacao(MissoesHotel.Recepcao, RecepcionistHotelNPC.VocabularioHotel, 1);
                }
                break;
            case "Home":
                var mae = Unico<motherNPC>();
                var pai = Unico<fatherNPC>();
                var irmao = Unico<brotherNPC>();
                if (mae == null || pai == null || irmao == null)
                    Nota("O roteiro da família exige uma instância inequívoca de cada NPC.");
                else
                    linha.Etapas.AddRange(FamiliaManager.DescreverRoteiro(nivel, mae, pai, irmao).DescreverEtapas());
                if (nivel != "B1")
                {
                    Marco("Atravessar a saída após a despedida.");
                    Associacao("mother_npc");
                }
                break;
            case "Visitor Center":
                Bloco<VisitorCenterNPC>("Atendimento", npc => npc.DescreverRoteiro(nivel));
                Marco("Permanência no ponto do City Park após o atendimento.");
                Associacao(MissoesVisitorCenter.NpcKey);
                break;
            case "Restaurant":
                Marco("Sentar à mesa antes do atendimento.");
                Bloco<RestaurantWaiterNPC>("Atendimento do garçom", npc => npc.DescreverRoteiro(nivel));
                break;
            case "College":
                Bloco<RecepcionistCollegeNPC>("Recepção", npc => npc.DescreverRoteiro(nivel));
                Bloco<SecretaryCollegeNPC>("Secretaria", npc => npc.DescreverRoteiro(nivel));
                if (nivel == "A1-A2" || nivel == "A2-B1")
                    Nota("O documento é liberado durante o atendimento. Sua consulta pode ocorrer antes ou após o fim da conversa.");
                break;
        }

        Marco("Revisão conforme os erros da visita.");
        return linha;
    }

    private static (TipoEtapaRoteiro, string, string, string, string, int, Type, bool?) IdentidadeEtapa(DescricaoEtapaRoteiro e) =>
        (e.Tipo, e.Chave, e.Npc, e.Texto, e.Ponto, e.Ordem, e.ModeloEsperado, e.SpeakingLivre);

    private void Adicionar(TipoEtapaRoteiro tipo, string texto) =>
        Etapas.Add(new DescricaoEtapaRoteiro(tipo, null, null, texto));

    private void AdicionarAssociacao(string npc, string ponto, int ordem) =>
        Etapas.Add(new DescricaoEtapaRoteiro(TipoEtapaRoteiro.Exercicio, $"exercise.{npc}.{ponto}.{ordem}", npc,
            null, ponto, ordem, new ExercicioAssociacao()));
}
