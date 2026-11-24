using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed partial class ModalGerenciarExercicios
{
    private const string SeletorInsercao = "ModalSelecionarExFala";
    private const string FormularioFala = "ModalFormFalaNPC";
    private static readonly Regex TextoTecnico = new(@"[{}$£€0-9<>]|\b(gate|room|floor|price|total|checkout|deadline|receipt|platform|turn left|turn right)\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    // The live scripts do not describe the pedagogical meaning of a static line.
    // Only informationally neutral originals can be changed without a dependency declaration.
    private static readonly Regex FalaSocial = new(
        @"\A(?:of course|certainly|sure|okay|very well|all right|great|excellent|thank you|thanks|you are welcome|you're welcome|no problem)[.!]?\z",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private bool FalaEstatica(Linha linha) =>
        linha.Etapa.Tipo == TipoEtapaRoteiro.Fala && !linha.Excluida
        && !string.IsNullOrWhiteSpace(linha.FalaAtual) && !TextoTecnico.IsMatch(linha.FalaAtual)
        && RoteiroNpcCheckpoint.ChaveSaudacao(linha.Etapa.Texto) == null;

    private bool MesmoTrecho(Linha outra, Linha linha) =>
        FalaEstatica(outra) && outra.Etapa.Npc == linha.Etapa.Npc;

    private bool AncoraFala(Linha linha)
    {
        int i = roteiro.IndexOf(linha);
        if (i <= 0 || !FalaEstatica(linha) || linha.FalaNova
            || SessaoCenario.Id(SessaoCenario.CenarioDoNpc(linha.Etapa.Npc)) != filtro.cenario)
            return false;
        var anterior = roteiro.Take(i).LastOrDefault(r => !r.Excluida);
        return anterior != null && MesmoTrecho(anterior, linha);
    }

    private bool FalaEditavel(Linha linha)
    {
        if (linha.FalaNova) return !linha.Excluida && roteiro.Contains(linha);
        if (!AncoraFala(linha) || !FalaSocial.IsMatch(linha.Etapa.Texto)) return false;
        bool repetida = roteiro.Count(r => !r.FalaNova && r.Etapa.Npc == linha.Etapa.Npc
            && r.Etapa.Texto == linha.Etapa.Texto && r.Etapa.Tipo == TipoEtapaRoteiro.Fala) > 1;
        var proxima = roteiro.Skip(roteiro.IndexOf(linha) + 1).FirstOrDefault(r => !r.Excluida && !r.FalaNova);
        return !repetida && proxima != null && MesmoTrecho(proxima, linha)
            && !roteiro.Any(r => r.Etapa.Chave == linha.Etapa.Chave + ".annotation");
    }

    private bool DestinoFalaValido(Linha origem, Linha destino)
    {
        if (origem == destino || !FalaEditavel(origem) || !AncoraFala(destino)
            || origem.Etapa.Npc != destino.Etapa.Npc) return false;
        int de = roteiro.IndexOf(origem), para = roteiro.IndexOf(destino);
        return de >= 0 && para >= 0 && de + 1 != para
            && roteiro.Skip(Math.Min(de, para)).Take(Math.Abs(de - para)).All(r => r.Excluida || MesmoTrecho(r, origem));
    }

    private void RenderizarAncora(Linha linha)
    {
        var botao = Filho<Button>(Instanciar(itemAdicionar, linha), "AdicionarExercicio");
        if (botao == null) return;
        VinculosModaisTcc.Texto(botao.GetComponentInChildren<TMP_Text>(true), movendo != null ? "Mover para cá" : "+ Fala");
        VinculosModaisTcc.Ligar(botao, () =>
        {
            if (movendo != null) AplicarMovimento(linha);
            else EscolherInsercao(linha);
        });
    }

    private void EscolherInsercao(Linha ancora)
    {
        if (!MesmaConta || submodal != null || !roteiro.Contains(ancora)) return;
        var painel = VinculosModaisTcc.Painel(gameObject, SeletorInsercao);
        var fala = Filho<Button>(painel, "FalaNPC");
        var exercicio = Filho<Button>(painel, "Exercicio");
        var voltar = Filho<Button>(painel, "Voltar");
        if (fala == null || exercicio == null || voltar == null)
        {
            Avisar("Seletor incompleto", "Confira FalaNPC, Exercicio e Voltar no ModalSelecionarExFala.");
            return;
        }
        if (!AbrirSubmodal(painel)) return;
        var eventos = new List<Action>();
        limparSubmodal = () => eventos.ForEach(remover => remover());
        fala.interactable = AncoraFala(ancora);
        exercicio.interactable = ancora.Ancora;
        LigarTemporario(fala, () => AbrirFala(null, ancora), eventos);
        LigarTemporario(exercicio, () => EscolherTipo(ancora, true), eventos);
        LigarTemporario(voltar, FecharSubmodal, eventos);
    }

    private void ExcluirFala(Linha linha)
    {
        if (!FalaEditavel(linha)) return;
        Avisar("Excluir fala do rascunho?", linha.FalaAtual, () =>
        {
            if (!MesmaConta || !FalaEditavel(linha)) return;
            if (linha.FalaNova) roteiro.Remove(linha);
            else linha.Excluida = true;
            RegistrarMudanca();
        });
    }

    private void AbrirFala(Linha editada, Linha ancora)
    {
        if (!MesmaConta || aviso != null || !(editada == null ? AncoraFala(ancora) : FalaEditavel(editada))) return;
        var painel = VinculosModaisTcc.Painel(gameObject, FormularioFala);
        var entrada = Filho<TMP_InputField>(painel, "InputFalaNPC");
        var orientacao = Filho<TMP_InputField>(painel, "OrientacoesIA");
        var npc = Filho<TMP_Dropdown>(painel, "DropdownNPC");
        var salvar = Filho<Button>(painel, "BtnSalvar");
        var voltar = Filho<Button>(painel, "BtnVoltar");
        var sugerir = Filho<Button>(painel, "BtnSugerirFala");
        if (entrada == null || npc == null || salvar == null || voltar == null || sugerir == null)
        {
            Avisar("Formulário incompleto", "Confira os controles de ModalFormFalaNPC.");
            return;
        }
        if (!AbrirSubmodal(painel)) return;
        var alvo = editada ?? ancora;
        var eventos = new List<Action>();
        var selecao = filtro;
        var pedido = new CancellationTokenSource();
        limparSubmodal = () =>
        {
            pedido.Cancel();
            pedido.Dispose();
            eventos.ForEach(remover => remover());
            entrada.SetTextWithoutNotify("");
            orientacao?.SetTextWithoutNotify("");
        };
        // Uma posição interna tem um único ator demonstravelmente compatível.
        // Mudanças de ator são limites do roteiro, mesmo quando há vários NPCs cadastrados.
        npc.ClearOptions();
        npc.AddOptions(new List<string> { alvo.Etapa.Npc });
        npc.SetValueWithoutNotify(0);
        npc.interactable = false;
        entrada.SetTextWithoutNotify(editada?.FalaAtual ?? "");
        orientacao?.SetTextWithoutNotify("");
        Texto(painel, "Titulo", editada == null ? "Adicionar fala" : "Editar fala");
        Texto(painel, "TxtNomeCenario", CheckpointJogo.NomeCenario(filtro.cenario));
        Texto(painel, "TxtNivelEscolhido", filtro.nivel);
        int indice = roteiro.IndexOf(alvo);
        var antes = roteiro.Take(indice).Where(r => r.Etapa.Tipo == TipoEtapaRoteiro.Fala && !r.Excluida)
            .Reverse().Take(2).Reverse().ToArray();
        var depois = roteiro.Skip(indice + (editada == null ? 0 : 1))
            .Where(r => r.Etapa.Tipo == TipoEtapaRoteiro.Fala && !r.Excluida).Take(2).ToArray();
        string Referencias(IEnumerable<Linha> linhas) => string.Join("\n", linhas.Select(r => $"{r.Etapa.Npc}: {r.FalaAtual}"));
        Texto(painel, "FalaAnterior", Referencias(antes));
        Texto(painel, "ProximaFala", Referencias(depois));
        var rotulo = sugerir.GetComponentInChildren<TMP_Text>(true);
        var titulo = rotulo != null ? rotulo.text : "";
        sugerir.interactable = true;
        eventos.Add(() => VinculosModaisTcc.Texto(rotulo, titulo));
        LigarTemporario(voltar, () =>
        {
            FecharSubmodal();
            if (ancora != null) EscolherInsercao(ancora);
        }, eventos);
        LigarTemporario(Filho<Button>(painel, "BtnApagar"), () => entrada.text = "", eventos);
        LigarTemporario(Filho<Button>(painel, "BtnCopiar"), () => GUIUtility.systemCopyBuffer = entrada.text, eventos);
        LigarTemporario(salvar, () =>
        {
            if (!MesmaConta || submodal != painel || filtro != selecao || aviso != null) return;
            string texto = entrada.text.Trim();
            if (texto.Length == 0 || texto.Length > 500 || TextoTecnico.IsMatch(texto))
            {
                Avisar("Fala incompatível", "Digite até 500 caracteres sem marcadores, números ou dados operacionais protegidos.");
                return;
            }
            if (editada != null)
            {
                if (!editada.FalaNova && !FalaSocial.IsMatch(texto))
                {
                    Avisar("Fala protegida", "Falas originais só aceitam outra fórmula social neutra neste rascunho.");
                    return;
                }
                if (!FalaEditavel(editada)) return;
                editada.TextoFala = texto;
                editada.Mudou = !string.Equals(texto, editada.Etapa.Texto, StringComparison.Ordinal);
            }
            else
            {
                if (!AncoraFala(ancora)) return;
                string chave = "draft.line." + Guid.NewGuid().ToString("N");
                roteiro.Insert(roteiro.IndexOf(ancora), new Linha
                {
                    Chave = chave, FalaNova = true,
                    Etapa = new DescricaoEtapaRoteiro(TipoEtapaRoteiro.Fala, chave, ancora.Etapa.Npc, texto)
                });
            }
            FecharSubmodal();
            RegistrarMudanca();
        }, eventos);
        LigarTemporario(sugerir, async () =>
        {
            var cancelamento = pedido.Token;
            bool Vigente() => !cancelamento.IsCancellationRequested && this != null && MesmaConta && filtro == selecao
                && submodal == painel && npc.value == 0;
            if (!Vigente() || !sugerir.interactable || aviso != null) return;
            sugerir.interactable = false;
            VinculosModaisTcc.Texto(rotulo, "Gerando…");
            string textoOriginal = entrada.text;
            string orientacaoOriginal = orientacao?.text;
            try
            {
                var contexto = new
                {
                    id_cenario = selecao.cenario, nivel = selecao.nivel, npc = alvo.Etapa.Npc,
                    anchor = alvo.Chave, position = indice + 1,
                    before = antes.Select(r => new { npc = r.Etapa.Npc, text = r.FalaAtual }).ToArray(),
                    after = depois.Select(r => new { npc = r.Etapa.Npc, text = r.FalaAtual }).ToArray(),
                    exercises = ContextoProximo(indice, r => r.Exercicio != null, r => r.Exercicio.enunciado),
                    milestones = ContextoProximo(indice, r => r.Etapa.Tipo == TipoEtapaRoteiro.Marco,
                        r => r.Etapa.Texto ?? NomeMarco(r.Etapa.Chave)),
                    guidance = orientacaoOriginal ?? ""
                };
                var gerente = ExerciseManager.Instance ?? throw new InvalidOperationException();
                var resposta = await gerente.SugerirFalaAdministrativaAsync(contexto, cancelamento);
                if (!Vigente() || entrada.text != textoOriginal || orientacao?.text != orientacaoOriginal) return;
                string sugestao = resposta?["data"]?["speech"]?.ToString();
                if (string.IsNullOrWhiteSpace(sugestao) || TextoTecnico.IsMatch(sugestao)
                    || (string)resposta?["data"]?["anchor"] != alvo.Chave
                    || (string)resposta?["data"]?["npc"] != alvo.Etapa.Npc
                    || editada != null && !editada.FalaNova && !FalaSocial.IsMatch(sugestao))
                    Avisar("Sugestão indisponível", resposta?["error"]?["message"]?.ToString()
                        ?? "A sugestão não é compatível com a âncora selecionada.");
                else entrada.text = sugestao;
            }
            catch (Exception ex) when (ex is System.Net.Http.HttpRequestException || ex is Newtonsoft.Json.JsonException
                || ex is InvalidOperationException || ex is ArgumentException || ex is OperationCanceledException)
            {
                if (Vigente()) Avisar("Sugestão indisponível", "Não foi possível gerar uma fala. Tente novamente.");
            }
            finally
            {
                if (Vigente())
                {
                    sugerir.interactable = true;
                    VinculosModaisTcc.Texto(rotulo, titulo);
                }
            }
        }, eventos);
    }

    private string[] ContextoProximo(int indice, Func<Linha, bool> incluir, Func<Linha, string> descrever) => roteiro
        .Select((row, i) => (row, i)).Where(x => incluir(x.row) && !x.row.Excluida)
        .OrderBy(x => Math.Abs(x.i - indice)).Take(4).OrderBy(x => x.i)
        .Select(x => descrever(x.row) ?? "").Where(t => t.Length > 0)
        .Select(t => t.Substring(0, Math.Min(800, t.Length))).ToArray();
}
