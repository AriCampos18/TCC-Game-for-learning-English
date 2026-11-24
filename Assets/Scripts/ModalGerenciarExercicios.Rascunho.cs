using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed partial class ModalGerenciarExercicios
{
    [SerializeField] private GameObject itemFala, itemExercicio, itemMarco, itemAdicionar, linhaPar;
    [SerializeField] private Button botaoSalvar;
    private readonly Dictionary<GameObject, (Linha linha, int cenario, string nivel)> linhasGeradas = new();
    private readonly Dictionary<(int, string), List<Linha>> rascunhos = new();
    private List<Linha> roteiro;
    private (int cenario, string nivel) filtro;
    private GameObject submodal, aviso;
    private CanvasGroup bloqueio;
    private int indiceSubmodal;
    private bool interagiaAntes, bloqueavaAntes;
    private Action limparSubmodal;
    private readonly string[] nomesFormularios = { "FormWriting", "FormularioSpeaking", "FormularioSpeaking",
        "FormularioAlternativas", "FormularioCriarFrase", "FormularioCompletarFrase", "FormularioAssociacao" };
    private readonly string[] botoesTipos = { "BtnWriting", "BtnSpeaking", "BtnSpeakingLivre", "BtnAlternativas",
        "BtnCriarFrase", "BtnCompletarFrase", "BtnAssociacao" };

    private sealed class Linha
    {
        internal DescricaoEtapaRoteiro Etapa;
        internal string Chave;
        internal ExercicioBase Exercicio;
        internal ExercicioBase Original;
        internal bool Ancora, Mudou, Excluida, FalaNova, FalaMovida;
        internal string TextoFala;
        internal string FalaAtual => TextoFala ?? Etapa.Texto;
        internal string aviso;
        internal DescricaoEtapaRoteiro Destino;
        internal DescricaoEtapaRoteiro Posicao => Destino ?? Etapa;
    }

    private bool RascunhoPendente => rascunhos.Count > 0;

    [ContextMenu("Vincular prefabs e controles existentes")]
    private void ResolverVinculos()
    {
        itemFala = VinculosModaisTcc.Prefab(itemFala, "ItemFalaNPC");
        itemExercicio = VinculosModaisTcc.Prefab(itemExercicio, "ItemExercicio");
        itemMarco = VinculosModaisTcc.Prefab(itemMarco, "ItemMarcoInteracao");
        itemAdicionar = VinculosModaisTcc.Prefab(itemAdicionar, "ItemAdicionarExercicio");
        linhaPar = VinculosModaisTcc.Prefab(linhaPar, "LinhaPar");
        seletorCenario ??= VinculosModaisTcc.Filho<TMP_Dropdown>(gameObject, "DropdownCenario");
        seletorNivel ??= VinculosModaisTcc.Filho<TMP_Dropdown>(gameObject, "DropdownNivel");
        rolagem ??= VinculosModaisTcc.Filho<ScrollRect>(gameObject, "ScrollViewDialogo");
        status ??= VinculosModaisTcc.Filho<TMP_Text>(gameObject, "TxtStatus");
        botaoVoltar ??= VinculosModaisTcc.Filho<Button>(gameObject, "BotaoVoltar");
        botaoSalvar ??= VinculosModaisTcc.Filho<Button>(gameObject, "BotaoSalvarAlteracoes");
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.EditorUtility.SetDirty(this);
            if (gameObject.scene.IsValid())
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }
#endif
    }

    private static T Filho<T>(GameObject raiz, string nome) where T : Component => VinculosModaisTcc.Filho<T>(raiz, nome);
    private static void Texto(GameObject raiz, string campo, string valor) =>
        VinculosModaisTcc.Texto(Filho<TMP_Text>(raiz, campo), valor);

    private void LimparLinhas()
    {
        foreach (var objeto in linhasGeradas.Keys)
            if (objeto != null) { objeto.SetActive(false); Destroy(objeto); }
        linhasGeradas.Clear();
    }

    private void ExibirRoteiro(int cenario, string nivel, LinhaDoTempoExercicios linha, Dictionary<string, ExerciseLoadResult> catalogos)
    {
        filtro = (cenario, nivel);
        if (!rascunhos.TryGetValue(filtro, out roteiro))
        {
            roteiro = new List<Linha>();
            foreach (var etapa in linha.Etapas)
            {
                var row = new Linha { Etapa = etapa, Chave = etapa.Chave };
                if (etapa.Tipo == TipoEtapaRoteiro.Exercicio)
                {
                    var catalogo = catalogos[etapa.Npc];
                    if (catalogo.Success && catalogo.ByPoint.TryGetValue(etapa.Ponto, out var itens))
                        row.Original = itens.FirstOrDefault(i => i.Exercise.ordemBanco == etapa.Ordem)?.Exercise;
                    row.Exercicio = row.Original;
                    bool compativel = row.Exercicio != null && (etapa.ModeloEsperado == null
                        || (row.Exercicio.GetType() == etapa.ModeloEsperado && (etapa.SpeakingLivre == null
                        || (row.Exercicio as ExercicioSpeaking)?.modoLivre == etapa.SpeakingLivre)));
                    row.Ancora = compativel && etapa.AncoraDeInsercao;
                    if (!compativel)
                        row.aviso = row.Exercicio == null ? "Sem catálogo correspondente." : "Tipo diferente do esperado pelo roteiro.";
                }
                roteiro.Add(row);
            }
        }
        Renderizar();
    }

    private GameObject Instanciar(GameObject modelo, Linha linha)
    {
        var objeto = Instantiate(modelo, rolagem.content, false);
        linhasGeradas.Add(objeto, (linha, filtro.cenario, filtro.nivel));
        objeto.SetActive(true);
        return objeto;
    }

    private void Renderizar()
    {
        LimparLinhas();
        if (itemFala == null || itemExercicio == null || itemMarco == null || itemAdicionar == null)
            throw new InvalidOperationException("Os quatro prefabs da linha do tempo devem estar vinculados.");
        foreach (var row in roteiro)
        {
            if (row.Excluida) continue;
            var etapa = row.Etapa;
            if (etapa.Tipo == TipoEtapaRoteiro.Fala)
            {
                if (movendo != null ? DestinoValido(movendo, row) : AncoraFala(row))
                    RenderizarAncora(row);
                var item = Instanciar(itemFala, row);
                bool editavel = FalaEditavel(row);
                Texto(item, "NomeNPC", etapa.Npc + (row.FalaNova ? " · Nova no rascunho"
                    : !editavel ? " · Fala protegida" : row.FalaMovida ? " · Movida no rascunho"
                    : row.Mudou ? " · Editada no rascunho" : ""));
                Texto(item, "FalaNPC", row.FalaAtual);
                VincularAcao(item, "Editar", editavel, () => AbrirFala(row, null));
                VincularAcao(item, "Excluir", editavel, () => ExcluirFala(row));
                VincularAcao(item, "Mover", editavel, () => Mover(row));
            }
            else if (etapa.Tipo == TipoEtapaRoteiro.Exercicio)
            {
                if (movendo != null ? DestinoValido(movendo, row) : row.Ancora)
                {
                    var botao = Filho<Button>(Instanciar(itemAdicionar, row), "AdicionarExercicio");
                    if (movendo != null)
                    {
                        if (botao != null)
                            VinculosModaisTcc.Texto(botao.GetComponentInChildren<TMP_Text>(true), "Mover para cá");
                        VinculosModaisTcc.Ligar(botao, () => AplicarMovimento(row));
                    }
                    else
                        VinculosModaisTcc.Ligar(botao, () => EscolherInsercao(row));
                }
                var item = Instanciar(itemExercicio, row);
                var ex = row.Exercicio;
                var identificacao = item.GetComponentsInChildren<TMP_Text>(true)
                    .FirstOrDefault(t => t.name != "TipoEx" && t.name != "Enunciado" && t.transform.parent == item.transform);
                VinculosModaisTcc.Texto(identificacao, $"{row.Posicao.Npc} · {filtro.nivel} · {row.Posicao.Ponto}/{row.Posicao.Ordem} · "
                    + (row.Original == null && ex != null ? "Novo rascunho" : $"ID {ex?.idExercise}")
                    + (row == movendo ? " · em movimentação" : row.Destino != null ? " · movido no rascunho" : ""));
                Texto(item, "TipoEx", ex == null ? "" : NomeTipo(ex));
                Texto(item, "Enunciado", (ex?.enunciado ?? "") + (row.aviso == null ? "" : "\n" + row.aviso));
                VincularAcao(item, "Editar", ex != null, () => AbrirFormulario(row, null, Tipo(ex)));
                VincularAcao(item, "Excluir", ex != null, () => Excluir(row));
                VincularAcao(item, "Mover", ex != null, () => Mover(row));
            }
            else
                Texto(Instanciar(itemMarco, row), "DescricaoInteracao", etapa.Texto ?? NomeMarco(etapa.Chave));
        }
        if (RascunhoPendente)
            status.text = "Há alterações em rascunho nesta cena · nenhuma publicação.";
    }

    private void VincularAcao(GameObject item, string nome, bool habilitado, Action executar)
    {
        var botao = Filho<Button>(item, nome);
        if (botao == null) return;
        botao.interactable = habilitado && movendo == null;
        VinculosModaisTcc.Ligar(botao, () => { if (MesmaConta && submodal == null && aviso == null) executar(); });
    }

    private void RegistrarMudanca()
    {
        if (roteiro.Any(row => row.Mudou || row.Destino != null || row.Excluida || row.FalaNova || row.FalaMovida))
            rascunhos[filtro] = roteiro;
        else
            rascunhos.Remove(filtro);
        Renderizar();
    }

    private void Excluir(Linha row)
    {
        if (row.Original != null)
        {
            Avisar("Exercício publicado", "A desativação exige validar dependências do roteiro e publicar uma nova versão.");
            return;
        }
        Avisar("Excluir do rascunho?", row.Exercicio.enunciado, () =>
        {
            roteiro.Remove(row);
            RegistrarMudanca();
        });
    }

    private void ResumirRascunho()
    {
        if (!MesmaConta || submodal != null || aviso != null) return;
        int novos = rascunhos.Values.Sum(l => l.Count(r => r.Original == null && r.Exercicio != null));
        int editados = rascunhos.Values.Sum(l => l.Count(r => r.Exercicio != null && r.Original != null && r.Mudou));
        int movidos = rascunhos.Values.Sum(l => l.Count(r => r.Destino != null));
        var falas = rascunhos.Values.SelectMany(l => l).Where(r => r.Etapa.Tipo == TipoEtapaRoteiro.Fala).ToArray();
        string resumoFalas = $"Falas: {falas.Count(r => r.FalaNova)} novas, {falas.Count(r => !r.Excluida && !r.FalaNova && r.Mudou)} editadas, "
            + $"{falas.Count(r => !r.Excluida && r.FalaMovida)} movidas e {falas.Count(r => r.Excluida)} excluídas.";
        Avisar("Resumo do rascunho", $"{novos} novos, {editados} editados e {movidos} movidos em {rascunhos.Count} filtros. "
            + resumoFalas + " O rascunho fica nesta cena. A publicação versionada ainda não está disponível.", () =>
            { status.text = "Rascunho confirmado nesta cena · não publicado."; });
    }

    private bool AbrirSubmodal(GameObject painel)
    {
        if (!MesmaConta || painel == null || aviso != null) return false;
        FecharSubmodal();
        bloqueio = gameObject.GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        interagiaAntes = bloqueio.interactable;
        bloqueavaAntes = bloqueio.blocksRaycasts;
        bloqueio.interactable = false;
        bloqueio.blocksRaycasts = false;
        submodal = painel;
        indiceSubmodal = painel.transform.GetSiblingIndex();
        painel.transform.SetAsLastSibling();
        painel.SetActive(true);
        return true;
    }

    private void FecharSubmodal()
    {
        fecharAviso?.Invoke();
        limparSubmodal?.Invoke();
        limparSubmodal = null;
        if (submodal != null)
        {
            submodal.SetActive(false);
            submodal.transform.SetSiblingIndex(indiceSubmodal);
        }
        submodal = null;
        if (bloqueio != null)
        {
            bloqueio.interactable = interagiaAntes;
            bloqueio.blocksRaycasts = bloqueavaAntes;
            bloqueio = null;
        }
    }

    private Action fecharAviso;

    private void Avisar(string titulo, string mensagem, Action confirmar = null)
    {
        if (aviso != null) return;
        GameObject painel = VinculosModaisTcc.Painel(gameObject, "ModalErroAviso");
        var ok = Filho<Button>(painel, "Ok");
        if (painel == null || ok == null || ok.onClick.GetPersistentEventCount() != 0)
        {
            status.text = titulo + " · " + mensagem;
            return;
        }
        aviso = painel;
        int indiceAviso = painel.transform.GetSiblingIndex();
        painel.transform.SetAsLastSibling();
        Texto(painel, "Titulo", titulo);
        Texto(painel, "MsgErroAviso", mensagem);
        var fundo = submodal != null ? submodal : gameObject;
        var grupo = fundo.GetComponent<CanvasGroup>() ?? fundo.AddComponent<CanvasGroup>();
        bool interagia = grupo.interactable, bloqueava = grupo.blocksRaycasts;
        grupo.interactable = grupo.blocksRaycasts = false;
        Button cancelar = null;
        UnityEngine.Events.UnityAction fechar = () => fecharAviso?.Invoke();
        UnityEngine.Events.UnityAction aceitar = () => { fechar(); if (MesmaConta) confirmar?.Invoke(); };
        fecharAviso = () =>
        {
            ok.onClick.RemoveListener(aceitar);
            if (cancelar != null) { cancelar.gameObject.SetActive(false); Destroy(cancelar.gameObject); }
            painel.SetActive(false);
            painel.transform.SetSiblingIndex(indiceAviso);
            grupo.interactable = interagia;
            grupo.blocksRaycasts = bloqueava;
            aviso = null;
            fecharAviso = null;
        };
        if (confirmar != null)
        {
            cancelar = Instantiate(ok, ok.transform.parent, false);
            VinculosModaisTcc.Texto(cancelar.GetComponentInChildren<TMP_Text>(true), "Cancelar");
            cancelar.onClick.AddListener(fechar);
        }
        ok.onClick.AddListener(aceitar);
        painel.SetActive(true);
    }
}
