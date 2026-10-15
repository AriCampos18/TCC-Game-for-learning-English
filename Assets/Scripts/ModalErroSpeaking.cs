
using UnityEngine;
using UnityEngine.UI;

public class ModalErroSpeaking : MonoBehaviour
{
    public GameObject painelModal;
    public Button botaoTentarNovamente;
    public Button botaoDescartar;

    [HideInInspector] public SpeakingUI speakingUI;
    [HideInInspector] public ModalNivelmanetoScript nivelamentoScript;

    private void Awake()
    {
        if (botaoTentarNovamente != null)
            botaoTentarNovamente.onClick.AddListener(TentarNovamente);

        if (botaoDescartar != null)
            botaoDescartar.onClick.AddListener(DescartarSpeaking);
    }

    public void Abrir()
    {
        if (painelModal != null)
            painelModal.SetActive(true);
        else
            gameObject.SetActive(true);
    }

    public void Fechar()
    {
        if (painelModal != null)
            painelModal.SetActive(false);
        else
            gameObject.SetActive(false);
    }

    public void TentarNovamente()
    {
        Fechar();

        if (speakingUI != null)
            speakingUI.TentarNovamente();
        else
            Debug.LogError("SpeakingUI não configurado no modal de erro.");
    }

    public void DescartarSpeaking()
    {
        Fechar();

        if (nivelamentoScript != null)
            nivelamentoScript.DescartarSpeakingAtual();
        else
            Debug.LogError("ModalNivelmanetoScript não configurado no modal de erro.");
    }
}
