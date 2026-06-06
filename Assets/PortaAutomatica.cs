using UnityEngine;

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
        Debug.Log("Entrou no trigger");

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player detectado");
            abrindo = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            abrindo = false;
        }
    }
}