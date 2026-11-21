
using TMPro;
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

    private ModalConfiguracoes voltarPara;
    private TMP_Text textoBotaoPlay;
    private string textoOriginal;

    private void Awake()
    {
        if (botaoPlay != null)
        {
            textoBotaoPlay = botaoPlay.GetComponentInChildren<TMP_Text>(true);
            textoOriginal = textoBotaoPlay?.text;
            botaoPlay.onClick.AddListener(ComecarJogo);
        }
    }

    private void Start()
    {
        if (voltarPara == null)
            FindObjectOfType<FirstPlayerController>()?.DesativarControle();
    }

    private void OnEnable()
    {
        // Identifica se o jogo está executando em VR.
        bool estaEmVR = XRSettings.isDeviceActive;

        // Exibe somente o painel correspondente ao dispositivo.
        if (painelInstrucoesPC != null)
            painelInstrucoesPC.SetActive(!estaEmVR);

        if (painelInstrucoesVR != null)
            painelInstrucoesVR.SetActive(estaEmVR);

        if (textoBotaoPlay != null)
            textoBotaoPlay.text = voltarPara != null ? "Voltar" : textoOriginal;
    }

    internal void AbrirPelasConfiguracoes(ModalConfiguracoes configuracoes)
    {
        voltarPara = configuracoes;
        modalInstrucoes.SetActive(true);
        if (textoBotaoPlay != null)
            textoBotaoPlay.text = "Voltar";
    }

    private void ComecarJogo()
    {
        if (voltarPara != null)
        {
            var configuracoes = voltarPara;
            voltarPara = null;
            modalInstrucoes.SetActive(false);
            configuracoes.gameObject.SetActive(true);
            return;
        }
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
