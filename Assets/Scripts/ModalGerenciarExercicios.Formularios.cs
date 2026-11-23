using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed partial class ModalGerenciarExercicios
{
    private static int Tipo(ExercicioBase ex) => ex switch
    {
        ExercicioWriting => 0, ExercicioSpeaking fala => fala.modoLivre ? 2 : 1,
        ExercicioAlternativas => 3, ExercicioBlocos => 4, ExercicioCompletarFrase => 5,
        ExercicioAssociacao => 6, _ => -1
    };

    private static string NomeTipo(ExercicioBase ex) => Tipo(ex) switch
    {
        0 => "Writing", 1 => "Speaking com opções", 2 => "Speaking livre", 3 => "Alternativas",
        4 => "Criar frase", 5 => "Completar frase", 6 => "Associação", _ => ex.nomeTipoBanco
    };

    private void EscolherTipo(Linha ancora)
    {
        if (submodal != null || aviso != null || !MesmaConta) return;
        var painel = VinculosModaisTcc.Painel(gameObject, "ModalSelecionarTipoEx");
        if (!AbrirSubmodal(painel)) { Avisar("Seletor indisponível", "O modal de tipos não foi localizado."); return; }
        var remocoes = new List<Action>();
        limparSubmodal = () => { foreach (var remover in remocoes) remover(); };
        for (int i = 0; i < botoesTipos.Length; i++)
        {
            int tipo = i;
            LigarTemporario(Filho<Button>(painel, botoesTipos[i]), () => AbrirFormulario(null, ancora, tipo), remocoes);
        }
        LigarTemporario(Filho<Button>(painel, "BotaoVoltar"), FecharSubmodal, remocoes);
    }

    private static void LigarTemporario(Button botao, UnityEngine.Events.UnityAction acao, List<Action> remover)
    {
        if (VinculosModaisTcc.Ligar(botao, acao))
            remover.Add(() => botao.onClick.RemoveListener(acao));
    }

    private void AbrirFormulario(Linha row, Linha ancora, int tipo)
    {
        if (tipo < 0 || tipo >= nomesFormularios.Length) return;
        var painel = VinculosModaisTcc.Painel(gameObject, nomesFormularios[tipo]);
        var enunciado = Filho<TMP_InputField>(painel, "InputEnunciado");
        var salvar = Filho<Button>(painel, "BtnSalvar");
        var voltar = Filho<Button>(painel, "BtnVoltar");
        if (painel == null || salvar == null || voltar == null || (tipo != 6 && enunciado == null))
        {
            Avisar("Formulário incompleto", "Confira os controles de " + nomesFormularios[tipo] + ".");
            return;
        }
        if (!AbrirSubmodal(painel)) return;

        var remo = new List<Action>();
        var temporarios = new List<GameObject>();
        limparSubmodal = () =>
        {
            foreach (var apagar in remo) apagar();
            foreach (var obj in temporarios)
                if (obj != null) { obj.SetActive(false); Destroy(obj); }
            foreach (var input in painel.GetComponentsInChildren<TMP_InputField>(true))
                input.SetTextWithoutNotify("");
        };

        var atual = row?.Exercicio;
        TMP_InputField Campo(string nome) => Filho<TMP_InputField>(painel, nome);
        TMP_InputField CampoExtra(string titulo, string valor)
        {
            var modelo = enunciado ?? VinculosModaisTcc.Filho<TMP_InputField>(linhaPar, "PalavraPortugues");
            if (modelo == null) return null;
            var campo = Instantiate(modelo, voltar.transform.parent.parent, false);
            campo.transform.SetSiblingIndex(voltar.transform.parent.GetSiblingIndex());
            campo.gameObject.SetActive(true);
            campo.lineType = TMP_InputField.LineType.MultiLineNewline;
            campo.characterValidation = TMP_InputField.CharacterValidation.None;
            if (campo.placeholder is TMP_Text placeholder) placeholder.text = titulo;
            campo.SetTextWithoutNotify(valor ?? "");
            temporarios.Add(campo.gameObject);
            return campo;
        }

        if (enunciado == null)
            enunciado = CampoExtra("Enunciado", atual?.enunciado);
        if (enunciado == null)
        {
            Avisar("Enunciado indisponível", "Vincule o prefab LinhaPar ou um campo de enunciado na Associação.");
            LigarTemporario(voltar, FecharSubmodal, remo);
            return;
        }
        enunciado.SetTextWithoutNotify(atual?.enunciado ?? "");
        Texto(painel, "Titulo", (row == null ? "Criar" : "Editar") + (tipo == 2 ? " · Speaking livre" : ""));
        TMP_InputField respostas = null, frase = null, opcoesExtras = null;
        TMP_InputField[] opcoes = Array.Empty<TMP_InputField>();
        Toggle[] corretas = Array.Empty<Toggle>();
        var pares = new List<(TMP_InputField pt, TMP_InputField en)>();
        bool completo = true;

        if (tipo == 0 || tipo == 2)
            respostas = CampoExtra("Respostas esperadas (uma por linha, opcional)", Juntar(atual?.respostasEsperadas));
        if (tipo == 1 || tipo == 2 || tipo == 3)
        {
            int qtd = tipo == 3 ? 4 : 3;
            var grupo = Filho<ToggleGroup>(painel, "GrupoRespostasCorretas");
            var dados = (atual as ExercicioSpeaking)?.opcoesFala ?? (atual as ExercicioAlternativas)?.alternativas;
            int correta = (atual as ExercicioSpeaking)?.respostaCorreta ?? (atual as ExercicioAlternativas)?.alternativaCorreta ?? -1;
            if (tipo == 1 && (dados?.Count ?? 0) > qtd || tipo == 3 && atual != null && dados?.Count != qtd)
            {
                completo = false;
                Avisar("Quantidade incompatível", "O formulário atual não representa todas as opções cadastradas com fidelidade.");
            }
            if (grupo != null) grupo.allowSwitchOff = true;
            opcoes = new TMP_InputField[qtd];
            corretas = new Toggle[qtd];
            for (int i = 0; i < qtd; i++)
            {
                var linha = Filho<HorizontalLayoutGroup>(painel, "Opcao" + (i + 1));
                if (linha == null) { completo = false; continue; }
                linha.gameObject.SetActive(tipo != 2);
                opcoes[i] = Filho<TMP_InputField>(linha.gameObject, "InputOpcao" + (i + 1));
                corretas[i] = Filho<Toggle>(linha.gameObject, "Toggle");
                if (opcoes[i] == null || corretas[i] == null || grupo == null) { completo = false; continue; }
                opcoes[i].SetTextWithoutNotify(i < (dados?.Count ?? 0) ? dados[i] : "");
                corretas[i].group = grupo;
                corretas[i].SetIsOnWithoutNotify(i == correta);
            }
            if (grupo != null) grupo.allowSwitchOff = false;
        }

        if (tipo == 4 || tipo == 5)
        {
            var gerar = Filho<Button>(painel, "GerarDistratores");
            if (gerar != null) gerar.interactable = false;
            var quantidade = Campo("InputQuantidadeDistratores");
            if (quantidade != null) quantidade.interactable = false;
            frase = Campo("InputFraseEsperada");
            if (tipo == 4)
            {
                var blocos = atual as ExercicioBlocos;
                frase?.SetTextWithoutNotify(blocos?.respostaCorreta ?? "");
                opcoesExtras = CampoExtra("Blocos (um por linha). Vazio gera blocos da frase.", Juntar(blocos?.blocosPalavras));
            }
            else
            {
                var lacunas = atual as ExercicioCompletarFrase;
                frase?.SetTextWithoutNotify(lacunas == null ? "" : string.Join("__", lacunas.partesFrase?.Count > 1
                    ? lacunas.partesFrase : new List<string>{lacunas.fraseAntes, lacunas.fraseDepois}));
                respostas = Campo("InputRespostaCorreta");
                if (respostas != null) respostas.lineType = TMP_InputField.LineType.MultiLineNewline;
                respostas?.SetTextWithoutNotify(lacunas == null ? "" : Juntar(lacunas.respostasCorretas?.Count > 0
                    ? lacunas.respostasCorretas : new List<string>{lacunas.respostaCorreta}));
                opcoesExtras = CampoExtra("Opções (uma por linha). Comece com quatro no total.", Juntar(lacunas?.opcoes));
            }
            completo &= frase != null && opcoesExtras != null && (tipo != 5 || respostas != null);
            status.text = "Sugestões por IA indisponíveis · opções editáveis manualmente.";
        }

        if (tipo == 6)
        {
            var lista = Filho<VerticalLayoutGroup>(painel, "ListaPares");
            var adicionar = Filho<Button>(painel, "AdicionarPar");
            completo &= lista != null && adicionar != null && linhaPar != null;
            if (completo)
            {
                void NovoPar(string pt, string en)
                {
                    var objeto = Instantiate(linhaPar, lista.transform, false);
                    temporarios.Add(objeto);
                    objeto.SetActive(true);
                    var inputPt = Filho<TMP_InputField>(objeto, "PalavraPortugues");
                    var inputEn = Filho<TMP_InputField>(objeto, "PalavraIngles");
                    if (inputPt == null || inputEn == null) { completo = false; salvar.interactable = false; return; }
                    inputPt.SetTextWithoutNotify(pt);
                    inputEn.SetTextWithoutNotify(en);
                    var item = (inputPt, inputEn);
                    pares.Add(item);
                    VinculosModaisTcc.Ligar(Filho<Button>(objeto, "BtnExcluir"), () =>
                    {
                        pares.Remove(item);
                        objeto.SetActive(false);
                        Destroy(objeto);
                    });
                }
                foreach (var par in (atual as ExercicioAssociacao)?.associacaoCorreta ?? new List<ParAssociacao>())
                    NovoPar(par.palavraPortugues, par.palavraIngles);
                if (pares.Count == 0) NovoPar("", "");
                LigarTemporario(adicionar, () => NovoPar("", ""), remo);
            }
        }

        salvar.interactable = completo;
        LigarTemporario(voltar, () =>
        {
            FecharSubmodal();
            if (ancora != null) EscolherTipo(ancora);
        }, remo);
        LigarTemporario(salvar, () =>
        {
            if (!MesmaConta || aviso != null || submodal != painel) return;
            try
            {
                string TextoObrigatorio(TMP_InputField campo) => !string.IsNullOrWhiteSpace(campo?.text) ? campo.text.Trim()
                    : throw new ArgumentException("Preencha todos os campos obrigatórios.");
                string texto = TextoObrigatorio(enunciado);
                var referencias = Linhas(respostas?.text);
                ExercicioBase ex;
                if (tipo == 0 || tipo == 2)
                    ex = tipo == 0 ? new ExercicioWriting() : new ExercicioSpeaking { modoLivre = true };
                else if (tipo == 1 || tipo == 3)
                {
                    var dados = new List<string>();
                    int certa = -1, marcadas = 0;
                    for (int i = 0; i < opcoes.Length; i++)
                    {
                        string valor = opcoes[i].text.Trim();
                        if (corretas[i].isOn)
                        {
                            if (valor.Length == 0) throw new ArgumentException("A opção correta está vazia.");
                            marcadas++;
                            certa = dados.Count;
                        }
                        if (valor.Length > 0) dados.Add(valor);
                    }
                    if (marcadas != 1 || dados.Count == 0 || (tipo == 3 && dados.Count != 4) || Duplicadas(dados))
                        throw new ArgumentException("Preencha as opções sem duplicatas e marque uma única resposta correta.");
                    ex = tipo == 1 ? new ExercicioSpeaking { opcoesFala = dados, respostaCorreta = certa }
                        : new ExercicioAlternativas { alternativas = dados, alternativaCorreta = certa };
                }
                else if (tipo == 4)
                {
                    string certa = TextoObrigatorio(frase);
                    var palavras = Regex.Split(certa, @"\s+").ToList();
                    var blocos = Linhas(opcoesExtras.text);
                    if (blocos.Count == 0) blocos = new List<string>(palavras);
                    var restantes = BlocosUI.SepararPalavras(string.Join(" ", blocos));
                    if (BlocosUI.SepararPalavras(certa).Any(p => !restantes.Remove(p)))
                        throw new ArgumentException("Inclua todos os blocos da resposta, inclusive repetições e pontuação.");
                    ex = new ExercicioBlocos { respostaCorreta = certa, blocosPalavras = blocos };
                    var anterior = atual as ExercicioBlocos;
                    referencias = new[] { certa }.Concat((anterior?.respostasEsperadas ?? new List<string>())
                        .Where(r => r != anterior.respostaCorreta)).Distinct().ToList();
                }
                else if (tipo == 5)
                {
                    var partes = TextoObrigatorio(frase).Split(new[] { "__" }, StringSplitOptions.None).ToList();
                    if (referencias.Count == 0 || partes.Count != referencias.Count + 1)
                        throw new ArgumentException("Marque cada lacuna com __ e informe uma resposta por linha.");
                    var dados = Linhas(opcoesExtras.text);
                    if (Duplicadas(dados)) throw new ArgumentException("Remova opções duplicadas.");
                    foreach (var correta in referencias)
                        if (!dados.Contains(correta)) dados.Add(correta);
                    int tamanhoOriginal = (atual as ExercicioCompletarFrase)?.opcoes?.Count ?? 0;
                    if (dados.Count < 3 || (dados.Count > 6 && dados.Count > tamanhoOriginal))
                        throw new ArgumentException("Use de 3 a 6 opções no total, incluindo as respostas corretas. Exercícios existentes podem manter a quantidade original.");
                    ex = new ExercicioCompletarFrase { partesFrase = partes, respostasCorretas = referencias, opcoes = dados,
                        fraseAntes = partes[0], fraseDepois = partes.Last(), respostaCorreta = referencias[0] };
                }
                else
                {
                    var dados = pares.Select(p => new ParAssociacao { palavraPortugues = TextoObrigatorio(p.pt),
                        palavraIngles = TextoObrigatorio(p.en) }).ToList();
                    if (dados.Count == 0 || Duplicadas(dados.Select(p => p.palavraPortugues))
                        || Duplicadas(dados.Select(p => p.palavraIngles)))
                        throw new ArgumentException("Informe pares completos e únicos em cada coluna.");
                    var anterior = atual as ExercicioAssociacao;
                    List<string> Ordenar(IEnumerable<string> valores, List<string> ordem)
                    {
                        var lista = valores.ToList();
                        return (ordem ?? new List<string>()).Where(lista.Contains).Concat(lista.Where(x => ordem?.Contains(x) != true)).ToList();
                    }
                    ex = new ExercicioAssociacao { associacaoCorreta = dados,
                        opcoesPort = Ordenar(dados.Select(p => p.palavraPortugues), anterior?.opcoesPort),
                        opcoesIngles = Ordenar(dados.Select(p => p.palavraIngles), anterior?.opcoesIngles) };
                }

                var destino = row ?? new Linha { Etapa = ancora.Etapa, Chave = "draft." + Guid.NewGuid().ToString("N") };
                ex.enunciado = texto;
                ex.respostasEsperadas = (tipo == 0 || tipo == 2 || tipo == 4) ? referencias
                    : atual?.respostasEsperadas ?? new List<string>();
                ex.idExercise = atual?.idExercise ?? 0;
                ex.idTipoBanco = atual?.idTipoBanco ?? 0;
                ex.nomeTipoBanco = atual?.nomeTipoBanco;
                ex.feedback = atual?.feedback;
                ex.nivelBanco = filtro.nivel;
                ex.pointKey = destino.Etapa.Ponto;
                ex.ordemBanco = destino.Etapa.Ordem;
                destino.Exercicio = ex;
                destino.Mudou = true;
                if (row == null)
                    roteiro.Insert(roteiro.IndexOf(ancora), destino);
                FecharSubmodal();
                RegistrarMudanca();
            }
            catch (ArgumentException e) { Avisar("Revise o formulário", e.Message); }
        }, remo);
    }

    private static List<string> Linhas(string valor) => (valor ?? "").Split('\n').Select(x => x.Trim())
        .Where(x => x.Length > 0).ToList();
    private static string Juntar(IEnumerable<string> dados) => dados == null ? "" : string.Join("\n", dados);
    private static bool Duplicadas(IEnumerable<string> dados) => dados.GroupBy(x => x, StringComparer.OrdinalIgnoreCase).Any(g => g.Count() > 1);
}
