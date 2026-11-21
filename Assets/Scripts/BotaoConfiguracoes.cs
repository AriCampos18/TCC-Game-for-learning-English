using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class BotaoConfiguracoes : MonoBehaviour
{
    public Button botaoConfiguracoes;
    public GameObject modalConfiguracoes;
    public FirstPlayerController firstPlayerControllerScript;

    private int visitaAoAbrir;
    private bool aberto;
    private bool controleAnterior;
    private bool emJogoAnterior;
    private bool emDialogoAnterior;
    private bool modoInterfaceAnterior;
    private CursorLockMode cursorAnterior;
    private bool cursorEstavaVisivel;

    private void Start()
    {
        if (botaoConfiguracoes != null)
            botaoConfiguracoes.onClick.AddListener(AbrirConfiguracoes);
    }

    private void AbrirConfiguracoes()
    {
        if (aberto || modalConfiguracoes == null)
            return;

        aberto = true;
        visitaAoAbrir = SessaoCenario.Atual.Versao;
        emJogoAnterior = SessaoCenario.Atual.EmJogo;
        emDialogoAnterior = GameProgress.EstaEmDialogo;
        controleAnterior = firstPlayerControllerScript != null && firstPlayerControllerScript.ControleAtivo;
        modoInterfaceAnterior = ModoJogoManager.Instance != null && ModoJogoManager.Instance.uiMode;
        cursorAnterior = Cursor.lockState;
        cursorEstavaVisivel = Cursor.visible;

        botaoConfiguracoes.gameObject.SetActive(false);
        firstPlayerControllerScript?.DesativarControle();
        modalConfiguracoes.GetComponent<ModalConfiguracoes>()?.VincularAbertura(this);
        modalConfiguracoes.SetActive(true);
    }

    public void FecharConfiguracoes()
    {
        if (modalConfiguracoes != null) modalConfiguracoes.SetActive(false);
        if (botaoConfiguracoes != null) botaoConfiguracoes.gameObject.SetActive(true);
        if (!aberto)
            return;

        aberto = false;
        bool mesmaVisita = visitaAoAbrir == SessaoCenario.Atual.Versao && CheckpointJogo.PodeAvancar;
        if (mesmaVisita && controleAnterior && SessaoCenario.Atual.EmJogo && !GameProgress.EstaEmDialogo)
            firstPlayerControllerScript?.AtivarControle();
        else
            firstPlayerControllerScript?.DesativarControle();

        bool mesmoContexto = mesmaVisita && emJogoAnterior == SessaoCenario.Atual.EmJogo
            && emDialogoAnterior == GameProgress.EstaEmDialogo;
        if (mesmoContexto && ModoJogoManager.Instance != null)
            ModoJogoManager.Instance.DefinirModoUI(modoInterfaceAnterior);
        if (mesmoContexto && !XRSettings.isDeviceActive)
        {
            Cursor.lockState = cursorAnterior;
            Cursor.visible = cursorEstavaVisivel;
        }
    }
}
