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
    private bool associacaoAposSaidaIniciada;

    public async Task IniciarVisita()
    {
        if (visitaEmAndamento || associacaoAposSaidaIniciada)
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

                if (nivel == "A1")
                    await InteracaoA1();
                else if (nivel == "A1-A2")
                    await InteracaoA1A2();
                else if (nivel == "A2")
                    await InteracaoA2();
                else if (nivel == "A2-B1")
                    await InteracaoA2B1();
                else
                    await InteracaoB1();

                if (nivel != "B1")
                {
                    Emitir("marco_fluxo", "dialogos_concluidos");
                    await AguardarComMovimento(ObterInteracaoHome().AguardarTravessiaDaSaida);
                    Emitir("marco_fluxo", "saida_home");
                    string despedida = MissoesHome.UltimaMissao(nivel);
                    if (MissionManager.Instance?.MissaoConcluida(despedida) != true)
                        throw new InvalidOperationException("A miss?o de despedida precisa estar conclu?da antes da Associa??o.");

                    associacaoAposSaidaIniciada = true;
                    int ordem = nivel == "A1" ? 9 : nivel == "A2" ? 10 : 13;
                    await mae.ExecutarExercicio("farewell", ordem, mae.exerciciosAssociacao[0]);
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
        InteracaoHome interacao = ObterInteracaoHome();
        await AguardarComMovimento(() => interacao.AguardarPermanencia(comodo));
        Emitir("permanencia_confirmada", comodo);
    }

    private async Task Jantar()
    {
        InteracaoHome interacao = ObterInteracaoHome();
        await AguardarComMovimento(interacao.AguardarMesa);
        await interacao.SimularJantar();
        Emitir("marco_fluxo", "jantar_concluido");
    }

    private async Task InteracaoA1()
    {
        await pai.ExecutarFala(0);
        await irmao.ExecutarFala(0);
        await mae.ExecutarFala(0);
        await mae.ExecutarExercicio("greeting", 1, mae.exerciciosSpeaking[0]);
        Emitir("exercicio_encerrado", "mother_npc/greeting/1");

        await mae.ExecutarFala(1);
        await mae.ExecutarExercicio("day_conversation", 2, mae.exerciciosAlternativas[0]);
        Emitir("exercicio_encerrado", "mother_npc/day_conversation/2");

        await mae.ExecutarFala(2);
        await Visitar("banheiro");
        await mae.ExecutarExercicio("home_changes", 3, mae.exerciciosSpeaking[1]);

        await mae.ExecutarFala(3);
        await mae.ExecutarExercicio("work_conversation", 4, mae.exerciciosSpeaking[2]);
        Emitir("exercicio_encerrado", "mother_npc/work_conversation/4");

        await irmao.ExecutarExercicio("school_and_dog", 5, irmao.exerciciosSpeaking[0]);
        await irmao.ExecutarFala(1);
        await irmao.ExecutarExercicio("school_and_dog", 6, irmao.exerciciosCompletarFrase[0]);
        await irmao.ExecutarFala(2);
        await irmao.ExecutarExercicio("school_and_dog", 7, irmao.exerciciosAlternativas[0]);
        Emitir("conversa_encerrada", "brother_npc");

        await mae.ExecutarFala(4);
        await Jantar();

        await pai.ExecutarFala(1);
        await mae.ExecutarExercicio("farewell", 8, mae.exerciciosSpeaking[3]);

    }

    private async Task InteracaoA1A2()
    {
        await pai.ExecutarFala(0);
        await irmao.ExecutarFala(0);
        await mae.ExecutarFala(0);

        await mae.ExecutarExercicio("greeting", 1, mae.exerciciosSpeaking[0]);
        Emitir("exercicio_encerrado", "mother_npc/greeting/1");

        await mae.ExecutarFala(1);
        await mae.ExecutarExercicio("day_conversation", 2, mae.exerciciosWriting[0]);
        Emitir("exercicio_encerrado", "mother_npc/day_conversation/2");

        await mae.ExecutarFala(2);
        await Visitar("banheiro");
        await mae.ExecutarExercicio("home_changes", 3, mae.exerciciosAlternativas[0]);
        await mae.ExecutarExercicio("home_changes", 4, mae.exerciciosSpeaking[1]);

        await mae.ExecutarExercicio("work_conversation", 5, mae.exerciciosSpeaking[2]);
        await mae.ExecutarFala(3);
        Emitir("marco_fluxo", "mae_fala_3");

        await pai.ExecutarFala(1);
        Emitir("marco_fluxo", "pai_fala_1");
        await pai.ExecutarExercicio("work_conversation", 7, pai.exerciciosSpeaking[0]);

        await irmao.ExecutarExercicio("school_and_dog", 8, irmao.exerciciosSpeaking[0]);
        await irmao.ExecutarFala(1);
        await irmao.ExecutarExercicio("school_and_dog", 9, irmao.exerciciosSpeaking[1]);
        await irmao.ExecutarFala(2);
        await irmao.ExecutarExercicio("school_and_dog", 10, irmao.exerciciosSpeaking[2]);
        await irmao.ExecutarExercicio("school_and_dog", 11, irmao.exerciciosAlternativas[0]);
        Emitir("conversa_encerrada", "brother_npc");

        await mae.ExecutarFala(4);
        await Jantar();
        await pai.ExecutarFala(2);

        await pai.ExecutarExercicio("farewell", 12, pai.exerciciosSpeaking[1]);

    }

    private async Task InteracaoA2()
    {
        await irmao.ExecutarFala(0);
        await mae.ExecutarFala(0);
        await mae.ExecutarExercicio("greeting", 1, mae.exerciciosSpeaking[0]);
        Emitir("exercicio_encerrado", "mother_npc/greeting/1");

        await mae.ExecutarFala(1);
        await mae.ExecutarExercicio("day_conversation", 2, mae.exerciciosWriting[0]);
        Emitir("exercicio_encerrado", "mother_npc/day_conversation/2");

        await mae.ExecutarFala(2);
        await Visitar("quarto");
        await mae.ExecutarExercicio("home_changes", 3, mae.exerciciosAlternativas[0]);
        await mae.ExecutarExercicio("home_changes", 4, mae.exerciciosSpeaking[1]);

        await pai.ExecutarExercicio("work_conversation", 5, pai.exerciciosSpeaking[0]);
        await pai.ExecutarFala(0);
        Emitir("marco_fluxo", "pai_fala_0");

        await irmao.ExecutarExercicio("school_and_dog", 6, irmao.exerciciosSpeaking[0]);
        await irmao.ExecutarFala(1);
        await irmao.ExecutarExercicio("school_and_dog", 7, irmao.exerciciosSpeaking[1]);
        await irmao.ExecutarFala(2);
        Emitir("marco_fluxo", "irmao_fala_2");

        await irmao.ExecutarExercicio("pets", 8, irmao.exerciciosSpeaking[2]);
        Emitir("exercicio_encerrado", "brother_npc/pets/8");

        await mae.ExecutarFala(3);
        await Jantar();

        await pai.ExecutarFala(1);
        await mae.ExecutarExercicio("farewell", 9, mae.exerciciosCriarFrases[0]);

    }

    private async Task InteracaoA2B1()
    {
        await irmao.ExecutarFala(0);
        await pai.ExecutarFala(0);

        await pai.ExecutarExercicio("greeting", 1, pai.exerciciosSpeaking[0]);

        await mae.ExecutarFala(0);
        await mae.ExecutarExercicio("routine", 3, mae.exerciciosSpeaking[0]);
        Emitir("exercicio_encerrado", "mother_npc/routine/3");
        await pai.ExecutarExercicio("greeting", 2, pai.exerciciosCompletarFrase[0]);
        Emitir("exercicio_encerrado", "father_npc/greeting/2");

        await mae.ExecutarFala(1);
        await Visitar("banheiro");
        await mae.ExecutarExercicio("home_changes", 4, mae.exerciciosWriting[0]);
        await mae.ExecutarExercicio("home_changes", 5, mae.exerciciosCriarFrases[0]);
        Emitir("exercicio_encerrado", "mother_npc/home_changes/5");

        await pai.ExecutarExercicio("work_conversation", 6, pai.exerciciosSpeaking[1]);
        await pai.ExecutarFala(1);
        Emitir("marco_fluxo", "pai_fala_1");

        await mae.ExecutarExercicio("work_conversation", 7, mae.exerciciosSpeaking[1]);
        Emitir("exercicio_encerrado", "mother_npc/work_conversation/7");
        await mae.ExecutarFala(2);

        await irmao.ExecutarExercicio("school_and_dog", 8, irmao.exerciciosCriarFrases[0]);
        await irmao.ExecutarFala(1);
        await irmao.ExecutarExercicio("school_and_dog", 9, irmao.exerciciosSpeaking[0]);

        await irmao.ExecutarFala(2);
        await irmao.ExecutarExercicio("pets", 10, irmao.exerciciosWriting[0]);
        Emitir("exercicio_encerrado", "brother_npc/pets/10");
        await irmao.ExecutarExercicio("pets", 11, irmao.exerciciosSpeaking[1]);
        Emitir("conversa_encerrada", "brother_npc");

        await mae.ExecutarFala(3);
        await Jantar();
        await pai.ExecutarFala(2);
        await pai.ExecutarExercicio("dinner_plans", 12, pai.exerciciosSpeaking[2]);

        await mae.ExecutarFala(4);

    }

    private async Task InteracaoB1()
    {
        await irmao.ExecutarFala(0);
        await mae.ExecutarFala(0);
        await pai.ExecutarFala(0);
        await pai.ExecutarExercicio("greeting", 1, pai.exerciciosSpeaking[0]);
        Emitir("exercicio_encerrado", "father_npc/greeting/1");

        await pai.ExecutarExercicio("routine", 2, pai.exerciciosSpeaking[1]);

        await pai.ExecutarFala(1);
        Emitir("marco_fluxo", "pai_fala_1");

        await mae.ExecutarFala(1);
        await Visitar("cozinha");
        await mae.ExecutarExercicio("home_changes", 3, mae.exerciciosWriting[0]);

        await mae.ExecutarExercicio("work_conversation", 4, mae.exerciciosSpeaking[0]);
        Emitir("exercicio_encerrado", "mother_npc/work_conversation/4");
        await mae.ExecutarFala(2);

        await irmao.ExecutarExercicio("school_and_dog", 5, irmao.exerciciosCriarFrases[0]);
        await irmao.ExecutarFala(1);
        await irmao.ExecutarExercicio("school_and_dog", 6, irmao.exerciciosSpeaking[0]);
        await irmao.ExecutarFala(2);
        Emitir("marco_fluxo", "irmao_fala_2");

        await irmao.ExecutarExercicio("pets", 7, irmao.exerciciosWriting[0]);
        Emitir("exercicio_encerrado", "brother_npc/pets/7");

        await mae.ExecutarFala(3);
        await Jantar();
        await pai.ExecutarFala(2);
        await pai.ExecutarExercicio("dinner_plans", 8, pai.exerciciosCompletarFrase[0]);
        Emitir("exercicio_encerrado", "father_npc/dinner_plans/8");

        await mae.ExecutarFala(4);
        await mae.ExecutarExercicio("farewell", 9, mae.exerciciosSpeaking[1]);
        Emitir("exercicio_encerrado", "mother_npc/farewell/9");
        await mae.ExecutarExercicio("farewell", 10, mae.exerciciosAssociacao[0]);

    }
}
