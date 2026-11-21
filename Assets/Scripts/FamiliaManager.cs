using System;
using UnityEngine;
using System.Threading.Tasks;

public class FamiliaManager : MonoBehaviour
{
    [Header("NPCs da família")]
    [SerializeField] private motherNPC mae;
    [SerializeField] private fatherNPC pai;
    [SerializeField] private brotherNPC irmao;

    [SerializeField] private InteracaoHome interacaoHome;

    private bool visitaEmAndamento = false;
    private int visitaComAssociacaoAposSaida;

    public async Task IniciarVisita()
    {
        int visita = SessaoCenario.Atual.Versao;
        if (!SessaoCenario.Atual.Permite("Home") || visitaEmAndamento || visitaComAssociacaoAposSaida == visita)
            return;

        visitaEmAndamento = true;

        try
        {
            try
            {
                GameProgress.EstaEmDialogo = true;
                string nivel = DadosJogador.nivelUsuario;
                await Task.WhenAll(
                    mae.PrepararParaFamilia(nivel),
                    pai.PrepararParaFamilia(nivel),
                    irmao.PrepararParaFamilia(nivel));

                await CriarRoteiro(nivel, visita, mae, pai, irmao, this).Executar();

                if (nivel != "B1")
                {
                    Emitir("marco_fluxo", "dialogos_concluidos");
                    CheckpointJogo.Instance?.CapturarCaminhoHome();
                    await ConcluirSaidaEAbrirAssociacao(nivel, visita);
                }
            }
            catch (InvalidOperationException e)
            {
                Debug.LogError($"Home: {e.Message}", this);
                return;
            }
            catch (OperationCanceledException)
            {
                return;
            }
            finally
            {
                GameProgress.EstaEmDialogo = false;
                mae.FinalizarInteracaoFamilia();
            }

            if (RevisaoManager.Instance != null)
                await RevisaoManager.Instance.ExecutarRevisaoEExibirModalFinal(mae);
            else
                Debug.LogError("RevisaoManager não está disponível para finalizar Home.");
        }
        finally
        {
            visitaEmAndamento = false;
        }
    }

    internal async Task RetomarSaidaCheckpoint(int visita)
    {
        if (!SessaoCenario.Atual.Permite("Home", visita) || visitaEmAndamento)
            return;
        visitaEmAndamento = true;
        GameProgress.EstaEmDialogo = true;
        try
        {
            string nivel = SessaoCenario.Atual.Nivel;
            mae.AtualizarVisita();
            await mae.PrepararParaFamilia(nivel);
            if (!SessaoCenario.Atual.Permite("Home", visita))
                return;
            await ConcluirSaidaEAbrirAssociacao(nivel, visita);
        }
        finally
        {
            visitaEmAndamento = false;
            if (SessaoCenario.Atual.Permite("Home", visita))
            {
                GameProgress.EstaEmDialogo = false;
                mae.FinalizarInteracaoFamilia();
            }
        }
        if (SessaoCenario.Atual.Permite("Home", visita))
            await RevisaoManager.Instance.ExecutarRevisaoEExibirModalFinal(mae);
    }

    private async Task ConcluirSaidaEAbrirAssociacao(string nivel, int visita)
    {
        await AguardarComMovimento(ObterInteracaoHome().AguardarTravessiaDaSaida);
        if (!SessaoCenario.Atual.Permite("Home", visita))
            throw new OperationCanceledException();
        Emitir("marco_fluxo", "saida_home");
        string despedida = MissoesHome.UltimaMissao(nivel);
        if (MissionManager.Instance?.MissaoConcluida(despedida) != true)
            throw new InvalidOperationException("A missão de despedida precisa estar concluída antes da Associação.");

        visitaComAssociacaoAposSaida = visita;
        int ordem = nivel == "A1" ? 9 : nivel == "A2" ? 10 : 13;
        CheckpointJogo.Instance?.CapturarAssociacaoFinal(mae, "farewell", ordem);
        await mae.ExecutarExercicio("farewell", ordem, mae.exerciciosAssociacao[0]);
    }

    private static void Emitir(string tipo, string alvo)
    {
        MissionManager.Instance?.RegistrarEventoHome(tipo, alvo);
    }

    private InteracaoHome ObterInteracaoHome()
    {
        if (interacaoHome == null || !interacaoHome.isActiveAndEnabled)
            throw new InvalidOperationException("Vincule um componente InteracaoHome ativo ao FamiliaManager.");

        return interacaoHome;
    }

    private async Task AguardarComMovimento(Func<Task> aguardar)
    {
        mae.LiberarControlePlayer();
        try
        {
            await aguardar();
        }
        finally
        {
            mae.TravarControlePlayer();
        }
    }

    private async Task Visitar(string comodo)
    {
        int visita = SessaoCenario.Atual.Versao;
        InteracaoHome interacao = ObterInteracaoHome();
        await AguardarComMovimento(() => interacao.AguardarPermanencia(comodo));
        mae.ValidarRoteiroCheckpoint(visita);
        Emitir("permanencia_confirmada", comodo);
    }

    private async Task Jantar()
    {
        int visita = SessaoCenario.Atual.Versao;
        InteracaoHome interacao = ObterInteracaoHome();
        await AguardarComMovimento(interacao.AguardarMesa);
        await interacao.SimularJantar();
        mae.ValidarRoteiroCheckpoint(visita);
        Emitir("marco_fluxo", "jantar_concluido");
    }

    internal async Task<Func<Task>> PrepararRetomadaRoteiro(Newtonsoft.Json.Linq.JObject visita,
        int versao, System.Threading.CancellationToken token)
    {
        string nivel = SessaoCenario.Atual.Nivel;
        if (visitaEmAndamento || mae == null || pai == null || irmao == null)
            throw new System.IO.InvalidDataException("A família não está disponível para retomar Home.");
        mae.AtualizarVisita();
        pai.AtualizarVisita();
        irmao.AtualizarVisita();
        await Task.WhenAll(mae.PrepararParaFamilia(nivel), pai.PrepararParaFamilia(nivel), irmao.PrepararParaFamilia(nivel));
        token.ThrowIfCancellationRequested();
        mae.ValidarRoteiroCheckpoint(versao);
        var roteiro = CriarRoteiro(nivel, versao, mae, pai, irmao, this);
        var cursor = roteiro.Validar(visita);
        if (cursor.Situacao.Length != 0)
            throw new System.IO.InvalidDataException("Estado inesperado no roteiro da família.");

        return async () =>
        {
            mae.ValidarRoteiroCheckpoint(versao);
            visitaEmAndamento = true;
            GameProgress.EstaEmDialogo = true;
            mae.TravarControlePlayer();
            try
            {
                await roteiro.Executar(cursor.Indice);
                if (nivel != "B1")
                {
                    Emitir("marco_fluxo", "dialogos_concluidos");
                    CheckpointJogo.Instance?.CapturarCaminhoHome();
                    await ConcluirSaidaEAbrirAssociacao(nivel, versao);
                }
            }
            finally
            {
                visitaEmAndamento = false;
                if (SessaoCenario.Atual.Permite("Home", versao))
                {
                    GameProgress.EstaEmDialogo = false;
                    mae.FinalizarInteracaoFamilia();
                }
            }
            if (SessaoCenario.Atual.Permite("Home", versao) && RevisaoManager.Instance != null)
                await RevisaoManager.Instance.ExecutarRevisaoEExibirModalFinal(mae);
        };
    }

    internal static RoteiroNpcCheckpoint DescreverRoteiro(string nivel, motherNPC mae, fatherNPC pai, brotherNPC irmao) =>
        CriarRoteiro(nivel, 0, mae, pai, irmao, null);

    private static RoteiroNpcCheckpoint CriarRoteiro(string nivel, int visita, motherNPC mae, fatherNPC pai,
        brotherNPC irmao, FamiliaManager origem)
    {
        var roteiro = new RoteiroNpcCheckpoint(mae, visita);
        var maeDescricao = origem == null ? mae.DescreverConteudo(nivel) : null;
        var paiDescricao = origem == null ? pai.DescreverConteudo(nivel) : null;
        var irmaoDescricao = origem == null ? irmao.DescreverConteudo(nivel) : null;
        void Evento(string tipo, string alvo) => roteiro.Acao("event." + tipo + "." + alvo.Replace('/', '.'), () =>
        {
            Emitir(tipo, alvo);
            return Task.CompletedTask;
        });
        void FalaMae(int indice) => roteiro.Fala(maeDescricao?.Falas[indice] ?? mae.TextoFalaCheckpoint(indice), () => mae.ExecutarFala(indice), mae);
        void FalaPai(int indice) => roteiro.Fala(paiDescricao?.Falas[indice] ?? pai.TextoFalaCheckpoint(indice), () => pai.ExecutarFala(indice), pai);
        void FalaIrmao(int indice) => roteiro.Fala(irmaoDescricao?.Falas[indice] ?? irmao.TextoFalaCheckpoint(indice), () => irmao.ExecutarFala(indice), irmao);
        void ExercicioMae(string ponto, int ordem, ExercicioBase ex) =>
            roteiro.Exercicio(ponto, ordem, () => mae.ExecutarExercicio(ponto, ordem, ex), mae, modelo: ex);
        void ExercicioPai(string ponto, int ordem, ExercicioBase ex) =>
            roteiro.Exercicio(ponto, ordem, () => pai.ExecutarExercicio(ponto, ordem, ex), pai, modelo: ex);
        void ExercicioIrmao(string ponto, int ordem, ExercicioBase ex) =>
            roteiro.Exercicio(ponto, ordem, () => irmao.ExecutarExercicio(ponto, ordem, ex), irmao, modelo: ex);
        void Explorar(string comodo) => roteiro.Acao("explore." + comodo, () => origem.Visitar(comodo));
        void Mesa() => roteiro.Acao("dinner", () => origem.Jantar());

        switch (nivel)
        {
            case "A1":
                FalaPai(0);
                FalaIrmao(0);
                FalaMae(0);
                ExercicioMae("greeting", 1, (maeDescricao?.exerciciosSpeaking ?? mae.exerciciosSpeaking)[0]);
                Evento("exercicio_encerrado", "mother_npc/greeting/1");
                FalaMae(1);
                ExercicioMae("day_conversation", 2, (maeDescricao?.exerciciosAlternativas ?? mae.exerciciosAlternativas)[0]);
                Evento("exercicio_encerrado", "mother_npc/day_conversation/2");
                FalaMae(2);
                Explorar("banheiro");
                ExercicioMae("home_changes", 3, (maeDescricao?.exerciciosSpeaking ?? mae.exerciciosSpeaking)[1]);
                FalaMae(3);
                ExercicioMae("work_conversation", 4, (maeDescricao?.exerciciosSpeaking ?? mae.exerciciosSpeaking)[2]);
                Evento("exercicio_encerrado", "mother_npc/work_conversation/4");
                ExercicioIrmao("school_and_dog", 5, (irmaoDescricao?.exerciciosSpeaking ?? irmao.exerciciosSpeaking)[0]);
                FalaIrmao(1);
                ExercicioIrmao("school_and_dog", 6, (irmaoDescricao?.exerciciosCompletarFrase ?? irmao.exerciciosCompletarFrase)[0]);
                FalaIrmao(2);
                ExercicioIrmao("school_and_dog", 7, (irmaoDescricao?.exerciciosAlternativas ?? irmao.exerciciosAlternativas)[0]);
                Evento("conversa_encerrada", "brother_npc");
                FalaMae(4);
                Mesa();
                FalaPai(1);
                ExercicioMae("farewell", 8, (maeDescricao?.exerciciosSpeaking ?? mae.exerciciosSpeaking)[3]);
                break;
            case "A1-A2":
                FalaPai(0);
                FalaIrmao(0);
                FalaMae(0);
                ExercicioMae("greeting", 1, (maeDescricao?.exerciciosSpeaking ?? mae.exerciciosSpeaking)[0]);
                Evento("exercicio_encerrado", "mother_npc/greeting/1");
                FalaMae(1);
                ExercicioMae("day_conversation", 2, (maeDescricao?.exerciciosWriting ?? mae.exerciciosWriting)[0]);
                Evento("exercicio_encerrado", "mother_npc/day_conversation/2");
                FalaMae(2);
                Explorar("banheiro");
                ExercicioMae("home_changes", 3, (maeDescricao?.exerciciosAlternativas ?? mae.exerciciosAlternativas)[0]);
                ExercicioMae("home_changes", 4, (maeDescricao?.exerciciosSpeaking ?? mae.exerciciosSpeaking)[1]);
                ExercicioMae("work_conversation", 5, (maeDescricao?.exerciciosSpeaking ?? mae.exerciciosSpeaking)[2]);
                FalaMae(3);
                Evento("marco_fluxo", "mae_fala_3");
                FalaPai(1);
                Evento("marco_fluxo", "pai_fala_1");
                ExercicioPai("work_conversation", 7, (paiDescricao?.exerciciosSpeaking ?? pai.exerciciosSpeaking)[0]);
                ExercicioIrmao("school_and_dog", 8, (irmaoDescricao?.exerciciosSpeaking ?? irmao.exerciciosSpeaking)[0]);
                FalaIrmao(1);
                ExercicioIrmao("school_and_dog", 9, (irmaoDescricao?.exerciciosSpeaking ?? irmao.exerciciosSpeaking)[1]);
                FalaIrmao(2);
                ExercicioIrmao("school_and_dog", 10, (irmaoDescricao?.exerciciosSpeaking ?? irmao.exerciciosSpeaking)[2]);
                ExercicioIrmao("school_and_dog", 11, (irmaoDescricao?.exerciciosAlternativas ?? irmao.exerciciosAlternativas)[0]);
                Evento("conversa_encerrada", "brother_npc");
                FalaMae(4);
                Mesa();
                FalaPai(2);
                ExercicioPai("farewell", 12, (paiDescricao?.exerciciosSpeaking ?? pai.exerciciosSpeaking)[1]);
                break;
            case "A2":
                FalaIrmao(0);
                FalaMae(0);
                ExercicioMae("greeting", 1, (maeDescricao?.exerciciosSpeaking ?? mae.exerciciosSpeaking)[0]);
                Evento("exercicio_encerrado", "mother_npc/greeting/1");
                FalaMae(1);
                ExercicioMae("day_conversation", 2, (maeDescricao?.exerciciosWriting ?? mae.exerciciosWriting)[0]);
                Evento("exercicio_encerrado", "mother_npc/day_conversation/2");
                FalaMae(2);
                Explorar("quarto");
                ExercicioMae("home_changes", 3, (maeDescricao?.exerciciosAlternativas ?? mae.exerciciosAlternativas)[0]);
                ExercicioMae("home_changes", 4, (maeDescricao?.exerciciosSpeaking ?? mae.exerciciosSpeaking)[1]);
                ExercicioPai("work_conversation", 5, (paiDescricao?.exerciciosSpeaking ?? pai.exerciciosSpeaking)[0]);
                FalaPai(0);
                Evento("marco_fluxo", "pai_fala_0");
                ExercicioIrmao("school_and_dog", 6, (irmaoDescricao?.exerciciosSpeaking ?? irmao.exerciciosSpeaking)[0]);
                FalaIrmao(1);
                ExercicioIrmao("school_and_dog", 7, (irmaoDescricao?.exerciciosSpeaking ?? irmao.exerciciosSpeaking)[1]);
                FalaIrmao(2);
                Evento("marco_fluxo", "irmao_fala_2");
                ExercicioIrmao("pets", 8, (irmaoDescricao?.exerciciosSpeaking ?? irmao.exerciciosSpeaking)[2]);
                Evento("exercicio_encerrado", "brother_npc/pets/8");
                FalaMae(3);
                Mesa();
                FalaPai(1);
                ExercicioMae("farewell", 9, (maeDescricao?.exerciciosCriarFrases ?? mae.exerciciosCriarFrases)[0]);
                break;
            case "A2-B1":
                FalaIrmao(0);
                FalaPai(0);
                ExercicioPai("greeting", 1, (paiDescricao?.exerciciosSpeaking ?? pai.exerciciosSpeaking)[0]);
                FalaMae(0);
                ExercicioMae("routine", 3, (maeDescricao?.exerciciosSpeaking ?? mae.exerciciosSpeaking)[0]);
                Evento("exercicio_encerrado", "mother_npc/routine/3");
                ExercicioPai("greeting", 2, (paiDescricao?.exerciciosCompletarFrase ?? pai.exerciciosCompletarFrase)[0]);
                Evento("exercicio_encerrado", "father_npc/greeting/2");
                FalaMae(1);
                Explorar("banheiro");
                ExercicioMae("home_changes", 4, (maeDescricao?.exerciciosWriting ?? mae.exerciciosWriting)[0]);
                ExercicioMae("home_changes", 5, (maeDescricao?.exerciciosCriarFrases ?? mae.exerciciosCriarFrases)[0]);
                Evento("exercicio_encerrado", "mother_npc/home_changes/5");
                ExercicioPai("work_conversation", 6, (paiDescricao?.exerciciosSpeaking ?? pai.exerciciosSpeaking)[1]);
                FalaPai(1);
                Evento("marco_fluxo", "pai_fala_1");
                ExercicioMae("work_conversation", 7, (maeDescricao?.exerciciosSpeaking ?? mae.exerciciosSpeaking)[1]);
                Evento("exercicio_encerrado", "mother_npc/work_conversation/7");
                FalaMae(2);
                ExercicioIrmao("school_and_dog", 8, (irmaoDescricao?.exerciciosCriarFrases ?? irmao.exerciciosCriarFrases)[0]);
                FalaIrmao(1);
                ExercicioIrmao("school_and_dog", 9, (irmaoDescricao?.exerciciosSpeaking ?? irmao.exerciciosSpeaking)[0]);
                FalaIrmao(2);
                ExercicioIrmao("pets", 10, (irmaoDescricao?.exerciciosWriting ?? irmao.exerciciosWriting)[0]);
                Evento("exercicio_encerrado", "brother_npc/pets/10");
                ExercicioIrmao("pets", 11, (irmaoDescricao?.exerciciosSpeaking ?? irmao.exerciciosSpeaking)[1]);
                Evento("conversa_encerrada", "brother_npc");
                FalaMae(3);
                Mesa();
                FalaPai(2);
                ExercicioPai("dinner_plans", 12, (paiDescricao?.exerciciosSpeaking ?? pai.exerciciosSpeaking)[2]);
                FalaMae(4);
                break;
            case "B1":
                FalaIrmao(0);
                FalaMae(0);
                FalaPai(0);
                ExercicioPai("greeting", 1, (paiDescricao?.exerciciosSpeaking ?? pai.exerciciosSpeaking)[0]);
                Evento("exercicio_encerrado", "father_npc/greeting/1");
                ExercicioPai("routine", 2, (paiDescricao?.exerciciosSpeaking ?? pai.exerciciosSpeaking)[1]);
                FalaPai(1);
                Evento("marco_fluxo", "pai_fala_1");
                FalaMae(1);
                Explorar("cozinha");
                ExercicioMae("home_changes", 3, (maeDescricao?.exerciciosWriting ?? mae.exerciciosWriting)[0]);
                ExercicioMae("work_conversation", 4, (maeDescricao?.exerciciosSpeaking ?? mae.exerciciosSpeaking)[0]);
                Evento("exercicio_encerrado", "mother_npc/work_conversation/4");
                FalaMae(2);
                ExercicioIrmao("school_and_dog", 5, (irmaoDescricao?.exerciciosCriarFrases ?? irmao.exerciciosCriarFrases)[0]);
                FalaIrmao(1);
                ExercicioIrmao("school_and_dog", 6, (irmaoDescricao?.exerciciosSpeaking ?? irmao.exerciciosSpeaking)[0]);
                FalaIrmao(2);
                Evento("marco_fluxo", "irmao_fala_2");
                ExercicioIrmao("pets", 7, (irmaoDescricao?.exerciciosWriting ?? irmao.exerciciosWriting)[0]);
                Evento("exercicio_encerrado", "brother_npc/pets/7");
                FalaMae(3);
                Mesa();
                FalaPai(2);
                ExercicioPai("dinner_plans", 8, (paiDescricao?.exerciciosCompletarFrase ?? pai.exerciciosCompletarFrase)[0]);
                Evento("exercicio_encerrado", "father_npc/dinner_plans/8");
                FalaMae(4);
                ExercicioMae("farewell", 9, (maeDescricao?.exerciciosSpeaking ?? mae.exerciciosSpeaking)[1]);
                Evento("exercicio_encerrado", "mother_npc/farewell/9");
                roteiro.Acao("association.ready", () => { CheckpointJogo.Instance?.CapturarAssociacaoFinal(mae, "farewell", 10); return Task.CompletedTask; });
                ExercicioMae("farewell", 10, (maeDescricao?.exerciciosAssociacao ?? mae.exerciciosAssociacao)[0]);
                break;
        }
        return roteiro;
    }
}
