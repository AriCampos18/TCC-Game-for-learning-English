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

    [Header("Final do jogo")]
    public CashierNPC cashier;
    public GameObject modalFinal;
    public GameObject modalAvisoSacola;

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
        if (other.CompareTag("Player"))
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

            if (cashier == null || !cashier.MissaoSacolaLiberada())
                return;

            // Só verifica se ele estava vindo de dentro
            if (!jogadorEntrouPeloLadoDeDentro)
                return;

            StartCoroutine(VerificarSaidaDepois(other.transform));
        }
    }

    private IEnumerator VerificarSaidaDepois(Transform player)
    {
        yield return new WaitForSeconds(0.2f);

        if (finalJaMostrado)
            yield break;

        if (!JogadorSaiuDoMercado(player.position))
            yield break;

        if (cashier == null || !cashier.JogadorPegouSacola())
        {
            if (modalAvisoSacola != null)
                modalAvisoSacola.SetActive(true);

            yield break;
        }

        finalJaMostrado = true;

        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.ConcluirMissao("pegar_sacola");
        }

        if (modalFinal != null)
        {
            modalFinal.SetActive(true);
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