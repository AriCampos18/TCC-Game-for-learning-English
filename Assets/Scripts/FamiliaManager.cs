using UnityEngine;
using System.Threading.Tasks;

public class FamiliaManager : MonoBehaviour
{
    [Header("NPCs da família")]
    [SerializeField] private motherNPC mae;
    [SerializeField] private fatherNPC pai;
    [SerializeField] private brotherNPC irmao;

    private bool visitaEmAndamento = false;

    public async Task IniciarVisita()
    {
        if (visitaEmAndamento)
            return;

        visitaEmAndamento = true;

        try
        {
            GameProgress.EstaEmDialogo = true;

            // Prepara os conteúdos dos três NPCs.
            mae.PrepararParaFamilia();
            pai.PrepararParaFamilia();
            irmao.PrepararParaFamilia();

            string nivel = DadosJogador.nivelUsuario;

            if (nivel == "A1")
                await InteracaoA1();
            else if (nivel == "A2")
                await InteracaoA2();
            else
                await InteracaoB1();
        }
        finally
        {
            GameProgress.EstaEmDialogo = false;
            mae.FinalizarInteracaoFamilia();
            visitaEmAndamento = false;
        }
    }

    private void IniciarMissao(string id, string texto, string traducao)
    {
        MissionManager.AdicionarMissaoNPC(id, texto, traducao);
    }

    private void ConcluirMissao(string id)
    {
        MissionManager.ConcluirMissaoNPC(id);
    }

    // =====================================================
    // A1
    // =====================================================

    private async Task InteracaoA1()
    {
        // MISSÃO 1 — Chegada e cumprimentos.
        IniciarMissao("casa_a1_chegada", "Greet your family", "Cumprimente sua família");

        await pai.ExecutarFala(0);
        await irmao.ExecutarFala(0);
        await mae.ExecutarFala(0);
        await mae.ExecutarExercicio(mae.exerciciosSpeaking[0]);

        ConcluirMissao("casa_a1_chegada");

        // MISSÃO 2 — Conversar com a mãe sobre o dia.
        IniciarMissao("casa_a1_dia_mae", "Talk to your mother about your day", "Converse com sua mãe sobre seu dia");

        await mae.ExecutarFala(1);
        await mae.ExecutarExercicio(mae.exerciciosAlternativas[0]);

        ConcluirMissao("casa_a1_dia_mae");

        // MISSÃO 3 — Conhecer o banheiro reformado.
        IniciarMissao("casa_a1_banheiro", "See the renovated bathroom", "Conheça o banheiro reformado");

        await mae.ExecutarFala(2);
        // O deslocamento até o banheiro ainda não é controlado pelo FamiliaManager.
        await mae.ExecutarExercicio(mae.exerciciosSpeaking[1]);

        ConcluirMissao("casa_a1_banheiro");

        // MISSÃO 4 — Conversar com a mãe sobre o trabalho.
        IniciarMissao("casa_a1_trabalho_mae", "Talk to your mother about work", "Converse com sua mãe sobre o trabalho");

        await mae.ExecutarFala(3);
        await mae.ExecutarExercicio(mae.exerciciosSpeaking[2]);

        ConcluirMissao("casa_a1_trabalho_mae");

        // MISSÃO 5 — Conversar com o irmão sobre a escola e o cachorro.
        IniciarMissao("casa_a1_irmao", "Talk to your brother about school and the dog", "Converse com seu irmão sobre a escola e o cachorro");

        await irmao.ExecutarExercicio(irmao.exerciciosSpeaking[0]);
        await irmao.ExecutarFala(1);
        await irmao.ExecutarExercicio(irmao.exerciciosCompletarFrase[0]);
        await irmao.ExecutarFala(2);
        await irmao.ExecutarExercicio(irmao.exerciciosAlternativas[0]);

        ConcluirMissao("casa_a1_irmao");

        // MISSÃO 6 — Ir jantar.
        IniciarMissao("casa_a1_jantar", "Go to the dinner table", "Vá até a mesa para jantar");

        await mae.ExecutarFala(4);
        // O deslocamento até a mesa ainda não é controlado pelo FamiliaManager.

        ConcluirMissao("casa_a1_jantar");

        // MISSÃO 7 — Despedir-se da família.
        IniciarMissao("casa_a1_despedida", "Say goodbye to your family", "Despeça-se da sua família");

        await pai.ExecutarFala(1);
        await mae.ExecutarExercicio(mae.exerciciosSpeaking[3]);
        await mae.ExecutarExercicio(mae.exerciciosAssociacao[0]);

        ConcluirMissao("casa_a1_despedida");
    }

    // =====================================================
    // A2
    // =====================================================

    private async Task InteracaoA2()
    {
        // MISSÃO 1 — Chegada e cumprimentos.
        IniciarMissao("casa_a2_chegada", "Greet your family", "Cumprimente sua família");

        await irmao.ExecutarFala(0);
        await mae.ExecutarFala(0);
        await mae.ExecutarExercicio(mae.exerciciosSpeaking[0]);

        ConcluirMissao("casa_a2_chegada");

        // MISSÃO 2 — Conversar com a mãe sobre o dia.
        IniciarMissao("casa_a2_dia_mae", "Talk to your mother about your day", "Converse com sua mãe sobre seu dia");

        await mae.ExecutarFala(1);
        await mae.ExecutarExercicio(mae.exerciciosWriting[0]);

        ConcluirMissao("casa_a2_dia_mae");

        // MISSÃO 3 — Conhecer as mudanças no cômodo.
        IniciarMissao("casa_a2_comodo", "See the changes in the room", "Conheça as mudanças no cômodo");

        await mae.ExecutarFala(2);
        // O deslocamento até o cômodo e o retorno à sala ainda não são controlados aqui.
        await mae.ExecutarExercicio(mae.exerciciosAlternativas[0]);
        await mae.ExecutarExercicio(mae.exerciciosSpeaking[1]);

        ConcluirMissao("casa_a2_comodo");

        // MISSÃO 4 — Conversar com o pai sobre o trabalho.
        IniciarMissao("casa_a2_pai_trabalho", "Talk to your father about his work", "Converse com seu pai sobre o trabalho dele");

        await pai.ExecutarExercicio(pai.exerciciosSpeaking[0]);
        await pai.ExecutarFala(0);

        ConcluirMissao("casa_a2_pai_trabalho");

        // MISSÃO 5 — Conversar com o irmão sobre escola e cachorro.
        IniciarMissao("casa_a2_irmao", "Talk to your brother about school and the dog", "Converse com seu irmão sobre a escola e o cachorro");

        await irmao.ExecutarExercicio(irmao.exerciciosSpeaking[0]);
        await irmao.ExecutarFala(1);
        await irmao.ExecutarExercicio(irmao.exerciciosSpeaking[1]);
        await irmao.ExecutarFala(2);

        ConcluirMissao("casa_a2_irmao");

        // MISSÃO 6 — Compartilhar experiências com animais de estimação.
        IniciarMissao("casa_a2_animais", "Share your experiences with pets", "Compartilhe suas experiências com animais de estimação");

        await irmao.ExecutarExercicio(irmao.exerciciosSpeaking[2]);

        ConcluirMissao("casa_a2_animais");

        // MISSÃO 7 — Ir jantar.
        IniciarMissao("casa_a2_jantar", "Go to the dinner table", "Vá até a mesa para jantar");

        await mae.ExecutarFala(3);
        // O deslocamento até a mesa ainda não é controlado pelo FamiliaManager.

        ConcluirMissao("casa_a2_jantar");

        // MISSÃO 8 — Agradecer e despedir-se.
        IniciarMissao("casa_a2_despedida", "Thank your family and say goodbye", "Agradeça à sua família e se despeça");

        await pai.ExecutarFala(1);
        await mae.ExecutarExercicio(mae.exerciciosCriarFrases[0]);
        await mae.ExecutarExercicio(mae.exerciciosAssociacao[0]);

        ConcluirMissao("casa_a2_despedida");
    }

    // =====================================================
    // B1
    // =====================================================

    private async Task InteracaoB1()
    {
        // MISSÃO 1 — Chegada e cumprimentos.
        IniciarMissao("casa_b1_chegada", "Greet your family", "Cumprimente sua família");

        await irmao.ExecutarFala(0);
        await mae.ExecutarFala(0);
        await pai.ExecutarFala(0);
        await pai.ExecutarExercicio(pai.exerciciosSpeaking[0]);

        ConcluirMissao("casa_b1_chegada");

        // MISSÃO 2 — Conversar com o pai sobre a rotina.
        IniciarMissao("casa_b1_rotina_pai", "Talk to your father about your recent routine", "Converse com seu pai sobre sua rotina recente");

        await pai.ExecutarExercicio(pai.exerciciosSpeaking[1]);

        ConcluirMissao("casa_b1_rotina_pai");

        // MISSÃO 3 — Ouvir sobre o trabalho do pai.
        IniciarMissao("casa_b1_trabalho_pai", "Talk about your father's work", "Converse sobre o trabalho do seu pai");

        await pai.ExecutarFala(1);

        ConcluirMissao("casa_b1_trabalho_pai");

        // MISSÃO 4 — Conhecer as mudanças na cozinha.
        IniciarMissao("casa_b1_cozinha", "See the changes in the kitchen", "Conheça as mudanças na cozinha");

        await mae.ExecutarFala(1);
        // O deslocamento até a cozinha ainda não é controlado pelo FamiliaManager.
        await mae.ExecutarExercicio(mae.exerciciosWriting[0]);

        ConcluirMissao("casa_b1_cozinha");

        // MISSÃO 5 — Conversar com a mãe sobre o trabalho.
        IniciarMissao("casa_b1_trabalho_mae", "Ask your mother about her work", "Pergunte à sua mãe sobre o trabalho dela");

        await mae.ExecutarExercicio(mae.exerciciosSpeaking[0]);
        await mae.ExecutarFala(2);

        ConcluirMissao("casa_b1_trabalho_mae");

        // MISSÃO 6 — Conversar com o irmão sobre escola e cachorro.
        IniciarMissao("casa_b1_irmao", "Talk to your brother about school and the dog", "Converse com seu irmão sobre a escola e o cachorro");

        await irmao.ExecutarExercicio(irmao.exerciciosCriarFrases[0]);
        await irmao.ExecutarFala(1);
        await irmao.ExecutarExercicio(irmao.exerciciosSpeaking[0]);
        await irmao.ExecutarFala(2);

        ConcluirMissao("casa_b1_irmao");

        // MISSÃO 7 — Compartilhar experiências com animais.
        IniciarMissao("casa_b1_animais", "Share your experiences with animals", "Compartilhe suas experiências com animais");

        await irmao.ExecutarExercicio(irmao.exerciciosWriting[0]);

        ConcluirMissao("casa_b1_animais");

        // MISSÃO 8 — Jantar e conversar sobre os planos para a noite.
        IniciarMissao("casa_b1_jantar_planos", "Have dinner and talk about your plans for the night", "Jante e converse sobre seus planos para a noite");

        await mae.ExecutarFala(3);
        // O deslocamento até a mesa ainda não é controlado pelo FamiliaManager.
        await pai.ExecutarFala(2);
        await pai.ExecutarExercicio(pai.exerciciosCompletarFrase[0]);

        ConcluirMissao("casa_b1_jantar_planos");

        // MISSÃO 9 — Agradecer e despedir-se.
        IniciarMissao("casa_b1_despedida", "Thank your family and say goodbye", "Agradeça à sua família e se despeça");

        await mae.ExecutarFala(4);
        await mae.ExecutarExercicio(mae.exerciciosSpeaking[1]);
        await mae.ExecutarExercicio(mae.exerciciosAssociacao[0]);

        ConcluirMissao("casa_b1_despedida");
    }
}
