using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;

public class ModalConfiguracoes : MonoBehaviour
{
    public GameObject modalGerenciarExs, modalInstrucoes;
    public Button botaoSmooth, botaoSnap;
    public Button botaoLogout, botaoInstrucoes, botaoSave, botaoSairJogo;
    public Button botaoGerenciarExs, botaoFecharConfig;

    [SerializeField] private GameObject areaTurn;
    [SerializeField] private SnapTurnProvider snapTurn;
    [SerializeField] private ContinuousTurnProvider smoothTurn;

    private TMP_Text textoSave;
    private BotaoConfiguracoes abertura;

    private void Awake()
    {
        var paiTurn = botaoSnap != null ? botaoSnap.transform.parent : null;
        if (areaTurn == null && paiTurn != null && paiTurn != transform && paiTurn.IsChildOf(transform)
            && botaoSmooth != null && botaoSmooth.transform.parent == paiTurn)
            areaTurn = paiTurn.gameObject;

        if (botaoLogout != null) botaoLogout.onClick.AddListener(Logout);
        if (botaoInstrucoes != null) botaoInstrucoes.onClick.AddListener(AbrirInstrcoes);
        if (botaoSave != null)
        {
            textoSave = botaoSave.GetComponentInChildren<TMP_Text>(true);
            botaoSave.onClick.AddListener(Save);
        }
        if (botaoSairJogo != null) botaoSairJogo.onClick.AddListener(SairJogo);
        if (botaoGerenciarExs != null) botaoGerenciarExs.onClick.AddListener(AbrirGerenciarExs);
        if (botaoFecharConfig != null) botaoFecharConfig.onClick.AddListener(Fechar);
        if (botaoSmooth != null) botaoSmooth.onClick.AddListener(MudarModoTurn);
        if (botaoSnap != null && botaoSnap != botaoSmooth) botaoSnap.onClick.AddListener(MudarModoTurn);
    }

    private void OnEnable()
    {
        if (modalGerenciarExs == null)
            modalGerenciarExs = VinculosModaisTcc.Painel(gameObject, "ModalGerenciarExercicios");
        AtualizarBotoes();
    }

    internal void VincularAbertura(BotaoConfiguracoes origem) => abertura = origem;

    public void AbrirInstrcoes()
    {
        var instrucoes = modalInstrucoes != null ? modalInstrucoes.GetComponent<ModalInstrucoes>() : null;
        if (instrucoes == null)
        {
            Debug.LogWarning("Configurações: associe o objeto com ModalInstrucoes.", this);
            return;
        }

        instrucoes.AbrirPelasConfiguracoes(this);
        gameObject.SetActive(false);
    }

    public void AbrirGerenciarExs()
    {
        if (modalGerenciarExs == null)
            modalGerenciarExs = VinculosModaisTcc.Painel(gameObject, "ModalGerenciarExercicios");
        if (!ModalGerenciarExercicios.TemAcesso)
            return;
        if (modalGerenciarExs != null && modalGerenciarExs.TryGetComponent<ModalGerenciarExercicios>(out var gerenciar)
            && gerenciar.Abrir(this))
            gameObject.SetActive(false);
    }

    private void Update() => AtualizarBotoes();

    private void AtualizarBotoes()
    {
        var checkpoint = CheckpointJogo.Instance;
        if (botaoSave != null) botaoSave.interactable = checkpoint?.PodeSalvarManual == true;
        if (botaoLogout != null) botaoLogout.interactable = checkpoint != null;
        if (botaoGerenciarExs != null) {
            bool admin = ModalGerenciarExercicios.TemAcesso;
            if (botaoGerenciarExs.gameObject.activeSelf != admin)
                botaoGerenciarExs.gameObject.SetActive(admin);
            botaoGerenciarExs.interactable = admin && modalGerenciarExs != null
                && modalGerenciarExs.GetComponent<ModalGerenciarExercicios>() != null;
        }
        if (textoSave != null)
        {
            string texto = "Salvar • " + (checkpoint?.Status ?? "Checkpoint indisponível");
            if (checkpoint != null && SessaoCenario.Atual.EmJogo && CheckpointJogo.PodeAvancar)
                texto += checkpoint.TemMarcoInterno ? " • marco interno do cenário" : " • retomada na entrada";
            if (textoSave.text != texto)
                textoSave.text = texto;
        }

        bool vr = XRSettings.isDeviceActive;
        if (areaTurn != null && areaTurn.activeSelf != vr)
            areaTurn.SetActive(vr);

        bool smooth = checkpoint?.ModoGiro == "smooth";
        bool disponivel = vr && checkpoint?.PodeAlterarGiro == true
            && snapTurn != null && smoothTurn != null;
        AtualizarBotaoTurn(botaoSnap, vr && (!smooth || botaoSmooth == null), disponivel);
        if (botaoSmooth != botaoSnap)
            AtualizarBotaoTurn(botaoSmooth, vr && (smooth || botaoSnap == null), disponivel);
    }

    private static void AtualizarBotaoTurn(Button botao, bool visivel, bool habilitado)
    {
        if (botao == null)
            return;
        if (botao.gameObject.activeSelf != visivel)
            botao.gameObject.SetActive(visivel);
        if (botao.interactable != habilitado)
            botao.interactable = habilitado;
    }

    public void Logout()
    {
        if (CheckpointJogo.Instance != null)
            _ = CheckpointJogo.Instance.Logout();
    }

    public void Save()
    {
        if (CheckpointJogo.Instance != null)
            _ = CheckpointJogo.Instance.SalvarManual();
    }

    public void SairJogo()
    {
        if (CheckpointJogo.Instance != null)
            _ = CheckpointJogo.Instance.Sair();
        else
            Application.Quit();
    }

    public void MudarModoTurn()
    {
        var checkpoint = CheckpointJogo.Instance;
        if (!XRSettings.isDeviceActive || checkpoint == null || snapTurn == null || smoothTurn == null)
            return;
        string modo = checkpoint.ModoGiro == "snap" ? "smooth" : "snap";
        checkpoint.AlterarModoGiro(modo);
        AtualizarBotoes();
    }

    internal void AplicarPreferenciaTurn(string modo, bool permitir)
    {
        if (snapTurn == null || smoothTurn == null)
            return;
        if (modo == "smooth")
        {
            snapTurn.enabled = false;
            smoothTurn.enabled = permitir;
        }
        else
        {
            smoothTurn.enabled = false;
            snapTurn.enabled = permitir;
        }
    }

    public void Fechar()
    {
        if (abertura != null)
            abertura.FecharConfiguracoes();
        else
            gameObject.SetActive(false);
    }
}
