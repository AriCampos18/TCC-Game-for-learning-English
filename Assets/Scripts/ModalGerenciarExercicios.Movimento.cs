using System;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;

public sealed partial class ModalGerenciarExercicios
{
    private Linha movendo;
    private bool publicadoVerificado;
    private CancellationTokenSource validacaoMovimento;
    private string textoVoltarAnterior;
    private bool cenarioHabilitado, nivelHabilitado, salvarHabilitado;

    private async void Mover(Linha row)
    {
        bool fala = row.Etapa.Tipo == TipoEtapaRoteiro.Fala;
        if (!MesmaConta || movendo != null || (fala ? !FalaEditavel(row) : row.Exercicio == null || row.Exercicio is ExercicioAssociacao))
        {
            if (MesmaConta && row.Exercicio is ExercicioAssociacao)
                Avisar("Etapa protegida", "A Associação precisa permanecer na posição definida pelo roteiro.");
            return;
        }

        if (row.Original != null && !PublicadoRevisado(row))
        {
            Avisar("Exercício publicado protegido", "A independência pedagógica e os gatilhos deste exercício ainda não foram validados.");
            return;
        }

        movendo = row;
        cenarioHabilitado = seletorCenario.interactable;
        nivelHabilitado = seletorNivel.interactable;
        seletorCenario.interactable = seletorNivel.interactable = false;
        if (botaoSalvar != null)
        {
            salvarHabilitado = botaoSalvar.interactable;
            botaoSalvar.interactable = false;
        }
        var voltar = botaoVoltar.GetComponentInChildren<TMP_Text>(true);
        if (voltar != null)
        {
            textoVoltarAnterior = voltar.text;
            voltar.text = "Cancelar movimento";
        }
        AtualizarModoMovimento(row.Original != null ? "Validando as missões do cenário…" : null);
        if (row.Original == null)
            return;

        var token = new CancellationTokenSource();
        validacaoMovimento = token;
        try
        {
            var gerente = ExerciseManager.Instance;
            var missoes = gerente == null ? null
                : await gerente.LerMissoesAdministrativasAsync(filtro.cenario, filtro.nivel, token.Token);
            if (validacaoMovimento != token || !MesmaConta || movendo != row)
                return;
            var catalogo = MissoesCompativeis(missoes)
                ? await gerente.LerCatalogoAdministrativoAsync(filtro.cenario, row.Etapa.Npc, filtro.nivel, token.Token)
                : null;
            if (validacaoMovimento != token || !MesmaConta || movendo != row)
                return;
            publicadoVerificado = ParPublicadoCompativel(catalogo);
            AtualizarModoMovimento(publicadoVerificado ? null : "Movimento bloqueado: missões ausentes ou incompatíveis.");
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested) { }
        catch (Exception e) when (e is System.Net.Http.HttpRequestException || e is Newtonsoft.Json.JsonException
            || e is ArgumentException || e is InvalidOperationException || e is OperationCanceledException)
        {
            if (validacaoMovimento == token && MesmaConta)
                AtualizarModoMovimento("Movimento bloqueado: não foi possível confirmar as missões.");
        }
        finally
        {
            if (validacaoMovimento == token)
                validacaoMovimento = null;
            token.Dispose();
        }
    }

    private void AtualizarModoMovimento(string mensagem)
    {
        float posicao = rolagem.verticalNormalizedPosition;
        Renderizar();
        Canvas.ForceUpdateCanvases();
        rolagem.verticalNormalizedPosition = posicao;
        status.text = mensagem ?? (roteiro.Any(row => DestinoValido(movendo, row))
            ? "Escolha uma âncora destacada ou cancele o movimento."
            : "Nenhuma âncora compatível. Cancele para voltar.");
    }

    private void EncerrarMovimento(bool atualizar)
    {
        if (movendo == null)
            return;

        validacaoMovimento?.Cancel();
        validacaoMovimento = null;
        publicadoVerificado = false;
        movendo = null;
        var voltar = botaoVoltar.GetComponentInChildren<TMP_Text>(true);
        if (voltar != null) voltar.text = textoVoltarAnterior;
        seletorCenario.interactable = cenarioHabilitado;
        seletorNivel.interactable = nivelHabilitado;
        if (botaoSalvar != null) botaoSalvar.interactable = salvarHabilitado;
        if (atualizar)
        {
            status.text = "Movimento cancelado.";
            Renderizar();
        }
    }

    private bool DestinoValido(Linha origem, Linha destino)
    {
        if (origem?.Etapa.Tipo == TipoEtapaRoteiro.Fala)
            return DestinoFalaValido(origem, destino);
        if (origem == null || destino == origem || !destino.Ancora || destino.Exercicio == null
            || destino.Exercicio is ExercicioAssociacao)
            return false;

        int de = roteiro.IndexOf(origem), para = roteiro.IndexOf(destino);
        if (de < 0 || para < 0 || de + 1 == para)
            return false;
        for (int i = Math.Min(de, para); i < Math.Max(de, para); i++)
        {
            var entre = roteiro[i].Etapa.Tipo;
            if (entre is TipoEtapaRoteiro.Marco or TipoEtapaRoteiro.Secao or TipoEtapaRoteiro.Lacuna
                || roteiro[i].Exercicio is ExercicioAssociacao)
                return false;
        }

        if (origem.Original != null)
            return publicadoVerificado && PublicadoRevisado(origem) && PublicadoRevisado(destino) && de == para + 1;

        bool mesmoContexto = origem.Posicao.Npc == destino.Etapa.Npc && origem.Posicao.Ponto == destino.Etapa.Ponto;
        if (mesmoContexto)
            return true;

        // Fora do ponto de origem, só exercícios estruturados equivalentes e sem marcadores dinâmicos.
        return origem.Exercicio.GetType() == destino.Exercicio.GetType()
            && (origem.Exercicio is ExercicioAlternativas opt && SemContexto(opt.alternativas, opt.enunciado)
                || origem.Exercicio is ExercicioBlocos blocos && SemContexto(blocos.blocosPalavras, blocos.enunciado, blocos.respostaCorreta)
                || origem.Exercicio is ExercicioCompletarFrase frase && SemContexto(frase.opcoes,
                    frase.enunciado, frase.fraseAntes, frase.fraseDepois)
                    && SemContexto(frase.partesFrase) && SemContexto(frase.respostasCorretas, frase.respostaCorreta));
    }

    private static bool SemContexto(System.Collections.Generic.IEnumerable<string> valores, params string[] textos) =>
        (valores ?? Array.Empty<string>()).Concat(textos).All(v => v == null || !v.Contains("{{"));

    private bool PublicadoRevisado(Linha row)
    {
        if (filtro.cenario != SessaoCenario.Id(MissoesVisitorCenter.Cenario) || filtro.nivel != "A2-B1"
            || row.Original == null || !ReferenceEquals(row.Original, row.Exercicio)
            || !row.Ancora || row.Etapa.Npc != MissoesVisitorCenter.NpcKey || row.Etapa.Ponto != "directions_confirmation")
            return false;
        // Cada hash identifica o enunciado revisado no seed 017. Mudanças exigem nova revisão.
        string hash = RoteiroNpcCheckpoint.Hash(row.Original.enunciado ?? "");
        return (row.Etapa.Ordem == 1 && row.Exercicio is ExercicioWriting
            && (row.Original.respostasEsperadas?.Count ?? 0) == 0 && hash == "B90D02CC0035C9C4AA51A102744C9AF77A1FC330FAC4E8151FB9CA0EF86F41A8")
            || (row.Etapa.Ordem == 2 && row.Exercicio is ExercicioAlternativas alternativas
                && alternativas.alternativaCorreta == 1 && alternativas.alternativas != null
                && RoteiroNpcCheckpoint.Hash(string.Join("\n", alternativas.alternativas.Select(a => a?.Trim() ?? ""))) == "CAF7795AD032EEB774D528F5BE9F3BBDBB75179BF1630585EE774F53E672EFA8"
                && hash == "90DC8EA7101F6740637093537BC715CD1AA5BDE03A65977625B5F548FDD8A7E2");
    }

    private bool ParPublicadoCompativel(ExerciseLoadResult catalogo)
    {
        if (catalogo?.Success != true || !catalogo.ByPoint.TryGetValue("directions_confirmation", out var itens))
            return false;
        var par = roteiro.Where(PublicadoRevisado).ToArray();
        return par.Length == 2 && par.All(row => itens.Any(item =>
            item.Exercise.idExercise == row.Original.idExercise && item.Exercise.ordemBanco == row.Etapa.Ordem
            && PublicadoRevisado(new Linha { Etapa = row.Etapa, Ancora = true, Original = item.Exercise, Exercicio = item.Exercise })));
    }

    private static bool MissoesCompativeis(CatalogoMissoes.Resposta resposta)
    {
        var dados = resposta?.data;
        var chaves = MissoesVisitorCenter.ChavesEsperadas("A2-B1");
        if (dados == null || dados.Count != chaves.Count
            || dados.Any(m => m == null || m.id_cenario != SessaoCenario.Id(MissoesVisitorCenter.Cenario)
                || m.nivel != "A2-B1" || m.inicio?.eventos == null || m.conclusao?.eventos == null)
            || !chaves.SetEquals(dados.Select(m => m.mission_key)))
            return false;
        var eventos = dados.SelectMany(m => m.inicio.eventos.Concat(m.conclusao.eventos)).ToArray();
        if (eventos.Any(e => e == null || string.IsNullOrEmpty(e.tipo) || string.IsNullOrEmpty(e.alvo)))
            return false;
        var exercicios = eventos.Where(e => e.tipo == "exercicio_encerrado").Select(e => e.alvo).ToArray();
        return exercicios.Length == 2 && exercicios.Contains("visitor_center_npc/first_visit/1")
            && exercicios.Contains("visitor_center_npc/transportation/1");
    }

    private void AplicarMovimento(Linha destino)
    {
        var origem = movendo;
        if (!MesmaConta || !DestinoValido(origem, destino))
            return;
        roteiro.Remove(origem);
        roteiro.Insert(roteiro.IndexOf(destino), origem);
        if (origem.Etapa.Tipo == TipoEtapaRoteiro.Fala)
            origem.FalaMovida = true;
        else
            origem.Destino = destino.Etapa;
        EncerrarMovimento(false);
        RegistrarMudanca();
    }
}
