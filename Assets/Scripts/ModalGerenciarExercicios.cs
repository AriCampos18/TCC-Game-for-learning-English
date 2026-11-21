using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class ModalGerenciarExercicios : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown seletorCenario;
    [SerializeField] private TMP_Dropdown seletorNivel;
    [SerializeField] private TMP_Text textoLinhaDoTempo;
    [SerializeField] private TMP_Text status;
    [SerializeField] private ScrollRect rolagem;
    [SerializeField] private Button botaoVoltar;

    private ModalConfiguracoes configuracoes;
    private CancellationTokenSource consulta;
    private SessionManager sessao;
    private int? conta;

    internal static bool TemAcesso => Acesso(AuthenticationUIController.Instance?.Session);
    private static bool Acesso(SessionManager atual) => atual?.IsAuthenticated == true && atual.IsAdmin;
    private bool MesmaConta => Acesso(sessao) && AuthenticationUIController.Instance?.Session == sessao
        && conta == sessao.AuthenticatedUserId;

    internal bool Abrir(ModalConfiguracoes origem)
    {
        if (!TemAcesso)
            return false;
        if (seletorCenario == null || seletorNivel == null || textoLinhaDoTempo == null || status == null || botaoVoltar == null)
        {
            Debug.LogWarning("Gerenciar exercícios: vincule cenário, nível, linha do tempo, status e botão Voltar.", this);
            return false;
        }
        configuracoes = origem;
        sessao = AuthenticationUIController.Instance.Session;
        conta = sessao.AuthenticatedUserId;
        gameObject.SetActive(true);
        return true;
    }

    private void OnEnable()
    {
        if (!MesmaConta)
        {
            gameObject.SetActive(false);
            return;
        }
        textoLinhaDoTempo.richText = false;
        textoLinhaDoTempo.text = "";
        status.text = "Selecione o cenário.";
        seletorCenario.ClearOptions();
        seletorCenario.AddOptions(new[] { "Selecione o cenário" }
            .Concat(Enumerable.Range(1, 8).Select(CheckpointJogo.NomeCenario)).ToList());
        seletorCenario.SetValueWithoutNotify(0);
        seletorNivel.ClearOptions();
        seletorNivel.AddOptions(new[] { "Selecione o nível" }.Concat(LinhaDoTempoExercicios.Niveis).ToList());
        seletorNivel.SetValueWithoutNotify(0);
        seletorNivel.interactable = false;
        seletorCenario.onValueChanged.AddListener(SelecionarCenario);
        seletorNivel.onValueChanged.AddListener(SelecionarNivel);
        botaoVoltar.onClick.AddListener(Voltar);
    }

    private void Update()
    {
        if (!MesmaConta)
            Voltar();
    }

    private void OnDisable()
    {
        CancelarConsulta();
        if (seletorCenario != null) seletorCenario.onValueChanged.RemoveListener(SelecionarCenario);
        if (seletorNivel != null) seletorNivel.onValueChanged.RemoveListener(SelecionarNivel);
        if (botaoVoltar != null) botaoVoltar.onClick.RemoveListener(Voltar);
        if (textoLinhaDoTempo != null) textoLinhaDoTempo.text = "";
    }

    private void Voltar()
    {
        var origem = configuracoes;
        configuracoes = null;
        gameObject.SetActive(false);
        if (origem != null)
            origem.gameObject.SetActive(true);
    }

    private void SelecionarCenario(int valor)
    {
        CancelarConsulta();
        seletorNivel.SetValueWithoutNotify(0);
        seletorNivel.interactable = valor > 0;
        textoLinhaDoTempo.text = "";
        status.text = valor > 0 ? "Selecione o nível." : "Selecione o cenário.";
    }

    private void SelecionarNivel(int valor)
    {
        CancelarConsulta();
        textoLinhaDoTempo.text = "";
        int cenario = seletorCenario.value;
        if (valor <= 0 || valor > LinhaDoTempoExercicios.Niveis.Length || cenario <= 0 || !MesmaConta)
            return;
        consulta = new CancellationTokenSource();
        _ = Carregar(cenario, LinhaDoTempoExercicios.Niveis[valor - 1], consulta);
    }

    private void CancelarConsulta()
    {
        if (consulta == null)
            return;
        consulta.Cancel();
        consulta = null;
    }

    private async Task Carregar(int cenario, string nivel, CancellationTokenSource atual)
    {
        var token = atual.Token;
        bool Vigente() => this != null && consulta == atual && !token.IsCancellationRequested && MesmaConta;
        status.text = "Verificando a sessão administrativa…";
        try
        {
            if (!await sessao.ConfirmarAdminAtualAsync(token))
            {
                if (Vigente())
                    status.text = "Não foi possível confirmar o acesso administrativo.";
                return;
            }
            if (!Vigente())
                return;
            status.text = "Consultando o catálogo…";
            string nome = CheckpointJogo.NomeCenario(cenario);
            var linha = LinhaDoTempoExercicios.Criar(nome, nivel);
            var gerente = ExerciseManager.Instance;
            var npcs = linha.Etapas.Where(e => e.Tipo == TipoEtapaRoteiro.Exercicio).Select(e => e.Npc).Distinct().ToArray();
            var catalogos = new Dictionary<string, ExerciseLoadResult>();
            foreach (string npc in npcs)
            {
                if (!Vigente())
                    return;
                catalogos[npc] = gerente == null ? ExerciseLoadResult.Failed("ExerciseManager indisponível.")
                    : await gerente.LerCatalogoAdministrativoAsync(cenario, npc, nivel, token);
            }
            if (!Vigente())
                return;
            textoLinhaDoTempo.text = Formatar(nome, nivel, linha, catalogos);
            if (rolagem != null)
            {
                Canvas.ForceUpdateCanvases();
                rolagem.verticalNormalizedPosition = 1f;
            }
            status.text = catalogos.Values.All(c => c.Success)
                ? "Linha do tempo · somente leitura"
                : "Catálogo incompleto · confira os avisos na linha do tempo.";
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested) { }
        catch (Exception e) when (e is InvalidOperationException || e is ArgumentException
            || e is System.Net.Http.HttpRequestException || e is OperationCanceledException)
        {
            if (Vigente())
                status.text = "Não foi possível carregar a linha do tempo. Selecione novamente o nível.";
            Debug.LogWarning("Gerenciar exercícios: " + e.Message, this);
        }
        finally
        {
            if (consulta == atual)
                consulta = null;
            atual.Dispose();
        }
    }

    private static string Formatar(string cenario, string nivel, LinhaDoTempoExercicios linha,
        Dictionary<string, ExerciseLoadResult> catalogos)
    {
        var texto = new StringBuilder().AppendLine(cenario + " · " + nivel).AppendLine();
        foreach (var etapa in linha.Etapas)
        {
            switch (etapa.Tipo)
            {
                case TipoEtapaRoteiro.Fala:
                    texto.Append("FALA · ").AppendLine(etapa.Npc).AppendLine(etapa.Texto).AppendLine();
                    break;
                case TipoEtapaRoteiro.Exercicio:
                    FormatarExercicio(texto, etapa, catalogos[etapa.Npc]);
                    break;
                case TipoEtapaRoteiro.Secao:
                    texto.AppendLine(etapa.Texto.ToUpperInvariant()).AppendLine();
                    break;
                default:
                    texto.Append(etapa.Tipo == TipoEtapaRoteiro.Lacuna ? "NOTA · " : "MARCO · ")
                        .AppendLine(etapa.Texto ?? NomeMarco(etapa.Chave)).AppendLine();
                    break;
            }
        }
        return texto.ToString();
    }

    private static void FormatarExercicio(StringBuilder texto, DescricaoEtapaRoteiro etapa, ExerciseLoadResult catalogo)
    {
        texto.Append("EXERCÍCIO · ").AppendLine(etapa.Npc);
        texto.Append("Ponto ").Append(etapa.Ponto).Append(" · posição ").AppendLine(etapa.Ordem.ToString());
        ExerciseCatalogItem item = null;
        if (catalogo.Success && catalogo.ByPoint.TryGetValue(etapa.Ponto, out var itens))
            item = itens.FirstOrDefault(e => e.Exercise.ordemBanco == etapa.Ordem);
        bool compativel = false;
        if (item != null)
        {
            var ex = item.Exercise;
            compativel = etapa.ModeloEsperado == null || (ex.GetType() == etapa.ModeloEsperado
                && (etapa.SpeakingLivre == null || (ex as ExercicioSpeaking)?.modoLivre == etapa.SpeakingLivre));
            string modo = ex is ExercicioSpeaking speaking ? (speaking.modoLivre ? " livre" : " com opções") : "";
            texto.Append(ex.nomeTipoBanco).Append(modo).Append(" · ID ").AppendLine(ex.idExercise.ToString());
            texto.AppendLine(ex.enunciado);
            if (!compativel)
                texto.AppendLine("O tipo cadastrado difere do esperado pelo roteiro.");
        }
        else
            texto.AppendLine(catalogo.Success ? "Sem cadastro correspondente." : "Catálogo indisponível para este NPC.");
        if (etapa.AncoraDeInsercao && compativel)
            texto.AppendLine("◇ Possível âncora de inserção futura antes deste exercício.");
        texto.AppendLine();
    }

    private static string NomeMarco(string chave)
    {
        if (chave == null)
            return "";
        if (chave.StartsWith("action.shelf.route.", StringComparison.Ordinal))
            return "Seguir o atendente até a prateleira · parada " + chave.Substring("action.shelf.route.".Length);
        if (chave.StartsWith("action.explore.", StringComparison.Ordinal))
            return "Explorar " + chave.Substring("action.explore.".Length);
        if (chave.StartsWith("action.PegarProduto.", StringComparison.Ordinal))
            return "Atendente mostra o produto · " + chave.Substring("action.PegarProduto.".Length);
        return chave switch
        {
            "action.fitting.open" => "Liberar o provador.",
            "action.fitting.wait" => "Aguardar a prova da roupa.",
            "action.dinner" => "Interagir com a mesa e simular o jantar.",
            "action.table.stand" => "Levantar da mesa.",
            "action.table.customer" => "Interagir com o cliente.",
            "action.table.return" => "Retornar à mesa.",
            "action.table.meal" => "Interagir com o prato e simular a refeição.",
            _ => chave
        };
    }
}
