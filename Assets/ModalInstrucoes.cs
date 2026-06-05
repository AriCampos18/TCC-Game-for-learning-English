using UnityEngine;
using UnityEngine.UI;

public class ModalInstrucoes : MonoBehaviour
{
    public Button botaoPlay;
    private FirstPlayerController firstPlayerControllerScript;
    public GameObject modalInstrucoes, modalMissoes;
    public GameObject crosshair;
    public Button botaoAjuda;
    public GameObject instrucaoIdioma;
    
    // Start is called before the first frame update
    void Start()
    {
        firstPlayerControllerScript = FindObjectOfType<FirstPlayerController>();

        if(firstPlayerControllerScript != null)
            firstPlayerControllerScript.DesativarControle();

        if (botaoPlay != null)
            botaoPlay.onClick.AddListener(ComecarJogo);

        string nivel = DadosJogador.nivelUsuario;

        if (nivel == "B1")
        {
            instrucaoIdioma.SetActive(false);
        }
    }

    void ComecarJogo()
    {
        modalInstrucoes.SetActive(false);
        botaoAjuda.gameObject.SetActive(true);
        modalMissoes.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (firstPlayerControllerScript != null)
            firstPlayerControllerScript.AtivarControle();
        if (crosshair != null)
            crosshair.SetActive(true);
    }
}
