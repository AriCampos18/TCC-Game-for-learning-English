using UnityEngine;
using UnityEngine.UI;

public class ModalInstrucoes : MonoBehaviour
{
    public Button botaoPlay;
    private FirstPlayerController firstPlayerControllerScript;
    public GameObject modalInstrucoes, modalMissoes, modalBarraProgressao;
    public GameObject crosshair;
    public Button botaoAjuda;
    public bool jogoJaComecou = false;
    
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
        modalInstrucoes.SetActive(false);

        if (botaoAjuda != null)
            botaoAjuda.gameObject.SetActive(true);

        if (!jogoJaComecou)
        {
            jogoJaComecou = true;

            modalBarraProgressao.SetActive(true);
            botaoAjuda.gameObject.SetActive(true);
            modalMissoes.SetActive(true);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (firstPlayerControllerScript != null)
                firstPlayerControllerScript.AtivarControle();

            if (crosshair != null)
                crosshair.SetActive(true);
        }
        else
        {
            if (GameProgress.EstaEmDialogo)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                if (firstPlayerControllerScript != null)
                    firstPlayerControllerScript.DesativarControle();

                if (crosshair != null)
                    crosshair.SetActive(false);
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                if (firstPlayerControllerScript != null)
                    firstPlayerControllerScript.AtivarControle();

                if (crosshair != null)
                    crosshair.SetActive(true);
            }
        }
    }
}
