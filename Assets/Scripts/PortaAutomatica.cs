using UnityEngine;
using System.Collections;

public class PortaAutomatica : MonoBehaviour
{
    public Transform portaEsquerda;
    public Transform portaDireita;

    public float distanciaAbrir = 2f;
    public float velocidade = 2f;

    private Vector3 posInicialEsq;
    private Vector3 posInicialDir;

    private Vector3 posAbertaEsq;
    private Vector3 posAbertaDir;

    private bool abrindo = false;
    private bool finalJaMostrado = false;
    private bool jogadorEntrouPeloLadoDeDentro = false;
    public FirstPlayerController movimentoPlayer;

    [Header("Final do jogo")]
    public CashierNPC cashier;
    public ModalFinalManager modalFinalManager;

    [Header("Detecção de saída")]
    public Transform pontoDentroMercado;
    public Transform pontoForaMercado;

    void Start()
    {
        posInicialEsq = portaEsquerda.position;
        posInicialDir = portaDireita.position;

        posAbertaEsq = posInicialEsq - portaEsquerda.right * distanciaAbrir;
        posAbertaDir = posInicialDir + portaDireita.right * distanciaAbrir;
    }

    void Update()
    {
        if (abrindo)
        {
            portaEsquerda.position = Vector3.Lerp(portaEsquerda.position, posAbertaEsq, Time.deltaTime * velocidade);
            portaDireita.position = Vector3.Lerp(portaDireita.position, posAbertaDir, Time.deltaTime * velocidade);
        }
        else
        {
            portaEsquerda.position = Vector3.Lerp(portaEsquerda.position, posInicialEsq, Time.deltaTime * velocidade);
            portaDireita.position = Vector3.Lerp(portaDireita.position, posInicialDir, Time.deltaTime * velocidade);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (SessaoCenario.Atual.Permite("Market") && other.CompareTag("Player")
            && pontoDentroMercado != null && pontoForaMercado != null)
        {
            abrindo = true;

            // Guarda de qual lado ele veio
            float distanciaDentro = Vector3.Distance(
                other.transform.position,
                pontoDentroMercado.position
            );

            float distanciaFora = Vector3.Distance(
                other.transform.position,
                pontoForaMercado.position
            );

            jogadorEntrouPeloLadoDeDentro = distanciaDentro < distanciaFora;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            abrindo = false;

            if (!SessaoCenario.Atual.Permite("Market") || cashier == null || !cashier.MissaoSacolaLiberada())
                return;

            // Só verifica se ele estava vindo de dentro
            if (!jogadorEntrouPeloLadoDeDentro)
                return;

            StartCoroutine(VerificarSaidaDepois(other.transform, SessaoCenario.Atual.Versao));
        }
    }

    internal void ReiniciarMarket()
    {
        abrindo = finalJaMostrado = jogadorEntrouPeloLadoDeDentro = false;
    }

    private IEnumerator VerificarSaidaDepois(Transform player, int visita)
    {
        yield return new WaitForSeconds(0.2f);

        if (!SessaoCenario.Atual.Permite("Market", visita) || finalJaMostrado || player == null)
            yield break;

        if (!JogadorSaiuDoMercado(player.position))
            yield break;

        if (cashier == null || !cashier.JogadorPegouSacola())
        {
            yield break;
        }

        finalJaMostrado = true;

        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.ConcluirMissaoMarket("pegar_sacola", "marco_fluxo", "saida_market");
        }

        var atividades = cashier.ExecutarAtividadesAposSaida();
        while (!atividades.IsCompleted)
            yield return null;

        if (!SessaoCenario.Atual.Permite("Market", visita))
            yield break;

        if (atividades.Status != System.Threading.Tasks.TaskStatus.RanToCompletion || !atividades.Result)
        {
            Debug.LogError("Market: falha ao concluir a Associa??o ou a revis?o ap?s a sa?da.", this);
            if (atividades.Exception != null)
                Debug.LogException(atividades.Exception, this);
            cashier.LiberarControlePlayer();
            yield break;
        }

        if (movimentoPlayer != null)
        {
            movimentoPlayer.DesativarControle();
        }

        if (modalFinalManager != null)
        {
            modalFinalManager.MostrarResultadoDaVisita(
                visita, "ContinuarCenarios",
                DadosJogador.nivelUsuario,
                DadosJogador.nivelUsuario
            );
        }
        else
        {
            Debug.LogError(
                "ModalFinalManager não foi atribuído na PortaAutomatica."
            );
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private bool JogadorSaiuDoMercado(Vector3 posicaoPlayer)
    {
        if (pontoDentroMercado == null || pontoForaMercado == null)
            return true;

        float distanciaDentro = Vector3.Distance(posicaoPlayer, pontoDentroMercado.position);
        float distanciaFora = Vector3.Distance(posicaoPlayer, pontoForaMercado.position);

        return distanciaFora < distanciaDentro;
    }
}