using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ModalRespostaNivelamentoScript : MonoBehaviour
{
    public Button botaoPlay;
    private FirstPlayerController firstPlayerControllerScript;
    public GameObject modalRespostaNivelamentoScript;
    public GameObject modalInstrucoes, modalMissoes;
    public Button botaoAjuda;

    public GameObject crosshair;

    public TextMeshProUGUI textoNivel;
    public TextMeshProUGUI textoFeedback;
    // Start is called before the first frame update
    void Start()
    {
        firstPlayerControllerScript = FindObjectOfType<FirstPlayerController>();
        if(firstPlayerControllerScript != null)
            firstPlayerControllerScript.DesativarControle();
        if (botaoPlay != null)
            botaoPlay.onClick.AddListener(ComecarJogo);
    }

    void ComecarJogo()
    {
        modalRespostaNivelamentoScript.SetActive(false);
        modalInstrucoes.SetActive(true);
        //aqui, passa o nivel recebido para o GameManager, para ele decidir quais exerccios mostrar
    }

    public void MostrarResultado(string nivel,string feedback)
    {
        DadosJogador.nivelUsuario = nivel;
        textoNivel.text = "Nível " + nivel;
        textoFeedback.text = feedback;
    }
}
