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
    public GameObject modalSelecaoCenario;

    
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

        if (modalSelecaoCenario != null)
        {
            modalSelecaoCenario.SetActive(true);

            ModalSelecaoCenario modal =
                modalSelecaoCenario.GetComponent<ModalSelecaoCenario>();

            if (modal != null)
            {
                modal.AtualizarCards();
            }
            else
            {
                Debug.LogError(
                    "ModalSelecaoCenario não encontrado no objeto associado."
                );
            }
        }
        else
        {
            Debug.LogError(
                "Modal de seleção de cenário não foi associado!"
            );
        }
    }
}
