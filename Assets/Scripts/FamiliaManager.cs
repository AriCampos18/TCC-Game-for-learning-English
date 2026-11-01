
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

            // Prepara os conteúdos dos três NPCs
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

            // Encerra legendas e libera o jogador
            mae.FinalizarInteracaoFamilia();

            visitaEmAndamento = false;
        }
    }

    // =====================================================
    // A1
    // =====================================================

    private async Task InteracaoA1()
    {
        // CHEGADA
        await pai.ExecutarFala(0);
        await irmao.ExecutarFala(0);
        await mae.ExecutarFala(0);

        await mae.ExecutarExercicio(mae.exerciciosSpeaking[0]);

        // CONVERSA COM A MÃE
        await mae.ExecutarFala(1);
        await mae.ExecutarExercicio(mae.exerciciosAlternativas[0]);

        await mae.ExecutarFala(2);

        // Aqui o jogador visita o banheiro e retorna.
        // O deslocamento poderá ser adicionado depois.

        await mae.ExecutarExercicio(mae.exerciciosSpeaking[1]);

        await mae.ExecutarFala(3);
        await mae.ExecutarExercicio(mae.exerciciosSpeaking[2]);

        // CONVERSA COM O IRMÃO
        await irmao.ExecutarExercicio(irmao.exerciciosSpeaking[0]);
        await irmao.ExecutarFala(1);
        await irmao.ExecutarExercicio(irmao.exerciciosCompletarFrase[0]);
        await irmao.ExecutarFala(2);
        await irmao.ExecutarExercicio(irmao.exerciciosAlternativas[0]);

        // JANTAR
        await mae.ExecutarFala(4);

        // Aqui o jogador vai até a mesa.
        // O deslocamento poderá ser adicionado depois.

        // DESPEDIDA
        await pai.ExecutarFala(1);

        await mae.ExecutarExercicio(mae.exerciciosSpeaking[3]);

        // EXERCÍCIO FINAL
        await mae.ExecutarExercicio(mae.exerciciosAssociacao[0]);
    }

    // =====================================================
    // A2
    // =====================================================

    private async Task InteracaoA2()
    {
        // CHEGADA
        await irmao.ExecutarFala(0);
        await mae.ExecutarFala(0);
        await mae.ExecutarExercicio(mae.exerciciosSpeaking[0]);

        // CONVERSA COM A MÃE
        await mae.ExecutarFala(1);
        await mae.ExecutarExercicio(mae.exerciciosWriting[0]);

        await mae.ExecutarFala(2);

        // Aqui o jogador visita o cômodo

        await mae.ExecutarExercicio(mae.exerciciosAlternativas[0]);

        //retorna para sala

        await mae.ExecutarExercicio(mae.exerciciosSpeaking[1]);

        // CONVERSA COM O PAI
        await pai.ExecutarExercicio(pai.exerciciosSpeaking[0]);
        await pai.ExecutarFala(0);

        // CONVERSA COM O IRMÃO
        await irmao.ExecutarExercicio(irmao.exerciciosSpeaking[0]);
        await irmao.ExecutarFala(1);
        await irmao.ExecutarExercicio(irmao.exerciciosSpeaking[1]);
        await irmao.ExecutarFala(2);
        await irmao.ExecutarExercicio(irmao.exerciciosSpeaking[2]);

        // JANTAR
        await mae.ExecutarFala(3);

        // Aqui o jogador vai até a mesa.

        // DESPEDIDA
        await pai.ExecutarFala(1);
        await mae.ExecutarExercicio(mae.exerciciosCriarFrases[0]);

        // EXERCÍCIO FINAL
        await mae.ExecutarExercicio(mae.exerciciosAssociacao[0]);
    }

    // =====================================================
    // B1
    // =====================================================

    private async Task InteracaoB1()
    {
        // CHEGADA
        await irmao.ExecutarFala(0);
        await mae.ExecutarFala(0);
        await pai.ExecutarFala(0);
        await pai.ExecutarExercicio(pai.exerciciosSpeaking[0]);


        await pai.ExecutarExercicio(pai.exerciciosSpeaking[1]);
        await pai.ExecutarFala(1);

        // CONVERSA COM A MÃE
        await mae.ExecutarFala(1);
        // Aqui o jogador visita a cozinha e retorna.

        await mae.ExecutarExercicio(mae.exerciciosWriting[0]);

        await mae.ExecutarExercicio(mae.exerciciosSpeaking[0]);
        await mae.ExecutarFala(2);

        // CONVERSA COM O IRMÃO
        await irmao.ExecutarExercicio(irmao.exerciciosCriarFrases[0]);
        await irmao.ExecutarFala(1);
        await irmao.ExecutarExercicio(irmao.exerciciosSpeaking[0]);
        await irmao.ExecutarFala(2);
        await irmao.ExecutarExercicio(irmao.exerciciosWriting[0]);

        // JANTAR
        await mae.ExecutarFala(3);

        // Aqui o jogador vai até a mesa.

        // PERGUNTA DO PAI
        await pai.ExecutarFala(2);
        await pai.ExecutarExercicio(pai.exerciciosCompletarFrase[0]);

        // DESPEDIDA
        await mae.ExecutarFala(4);
        await mae.ExecutarExercicio(mae.exerciciosSpeaking[2]);

        // EXERCÍCIO FINAL
        await mae.ExecutarExercicio(mae.exerciciosAssociacao[0]);
    }
}
