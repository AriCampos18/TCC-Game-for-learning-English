
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class ModalInstrucoes : MonoBehaviour
{
    [Header("Botões")]
    public Button botaoPlay;
    public Button botaoAjuda;

    [Header("Painéis de instruções")]
    public GameObject painelInstrucoesPC;
    public GameObject painelInstrucoesVR;

    [Header("Outros modais")]
    public GameObject modalInstrucoes;
    public GameObject modalMissoes;
    public GameObject modalBarraProgressao;
    public GameObject modalSelecaoCenario;

    [Header("Elementos do jogo")]
    public GameObject crosshair;

    public bool jogoJaComecou = false;

    private FirstPlayerController firstPlayerControllerScript;

    void Start()
    {
        firstPlayerControllerScript = FindObjectOfType<FirstPlayerController>();

        if (firstPlayerControllerScript != null)
            firstPlayerControllerScript.DesativarControle();

        // Identifica se o jogo está executando em VR.
        bool estaEmVR = XRSettings.isDeviceActive;

        // Exibe somente o painel correspondente ao dispositivo.
        if (painelInstrucoesPC != null)
            painelInstrucoesPC.SetActive(!estaEmVR);

        if (painelInstrucoesVR != null)
            painelInstrucoesVR.SetActive(estaEmVR);

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
