
using UnityEngine;
using UnityEngine.UI;

public class ModalErroSpeaking : MonoBehaviour
{
    public GameObject painelModal;
    public Button botaoTentarNovamente;
    public Button botaoDescartar;

    [HideInInspector] public SpeakingUI speakingUI;
    [HideInInspector] public ModalNivelmanetoScript nivelamentoScript;

    private SpeakingUI origem;

    private void Awake()
    {
        if (botaoTentarNovamente != null)
            botaoTentarNovamente.onClick.AddListener(TentarNovamente);

        if (botaoDescartar != null)
            botaoDescartar.onClick.AddListener(DescartarSpeaking);
    }

    public void AbrirPara(SpeakingUI solicitante)
    {
        origem = solicitante;
        Abrir();
    }

    public void Abrir()
    {
        var atual = origem != null ? origem : speakingUI;
        if (botaoDescartar != null)
            botaoDescartar.interactable = atual != null
                && ((atual == speakingUI && nivelamentoScript != null) || atual.PodePularPeloAviso);

        if (painelModal != null)
            painelModal.SetActive(true);
        else
            gameObject.SetActive(true);
    }

    public void Fechar()
    {
        origem = null;
        if (painelModal != null)
            painelModal.SetActive(false);
        else
            gameObject.SetActive(false);
    }

    public void TentarNovamente()
    {
        var atual = origem != null ? origem : speakingUI;
        Fechar();

        if (atual != null)
            atual.TentarNovamente();
        else
            Debug.LogError("SpeakingUI não configurado no modal de erro.");
    }

    public void DescartarSpeaking()
    {
        var atual = origem != null ? origem : speakingUI;
        Fechar();

        if (atual != null && atual == speakingUI && nivelamentoScript != null)
            nivelamentoScript.DescartarSpeakingAtual();
        else if (atual != null && atual.PodePularPeloAviso)
            atual.PularExercicio();
        else
            Debug.LogWarning("A atividade atual não possui ação de descarte disponível.", this);
    }
}
