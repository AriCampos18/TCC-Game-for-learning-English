
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModalConfiguracoes : MonoBehaviour
{
    public GameObject modalGerenciarExs, modalInstrucoes;
    public Button botaoSmooth, botaoSnap;
    public Button botaoLogout, botaoInstrucoes, botaoSave, botaoSairJogo;
    public Button botaoGerenciarExs, botaoFecharConfig;


    private TMP_Text textoSave;

    private void Awake()
    {
        if (botaoLogout != null)
            botaoLogout.onClick.AddListener(Logout);

        if (botaoInstrucoes != null)
            botaoInstrucoes.onClick.AddListener(AbrirInstrcoes);

        if (botaoSave != null)
        {
            textoSave = botaoSave.GetComponentInChildren<TMP_Text>(true);
            botaoSave.onClick.AddListener(Save);
        }

        if (botaoSairJogo != null)
            botaoSairJogo.onClick.AddListener(SairJogo);

        if (botaoGerenciarExs != null)
            botaoGerenciarExs.onClick.AddListener(AbrirGerenciarExs);

        if(botaoFecharConfig != null)
            botaoFecharConfig.onClick.AddListener(Fechar);

        if(botaoSmooth != null && botaoSnap != null){
            botaoSmooth.onClick.AddListener(MudarModoTurn);
            botaoSnap.onClick.AddListener(MudarModoTurn);
        }
    }

    public void AbrirInstrcoes()
    {
        if (modalInstrucoes != null)
            modalInstrucoes.SetActive(true);
        else
            gameObject.SetActive(true);
    }

    public void AbrirGerenciarExs()
    {
        if (modalGerenciarExs != null)
            modalGerenciarExs.SetActive(true);
        else
            gameObject.SetActive(true);
    }

    private void Update()
    {
        if (textoSave != null && CheckpointJogo.Instance != null)
        {
            string texto = "Salvar • " + CheckpointJogo.Instance.Status;
            if (SessaoCenario.Atual.EmJogo && CheckpointJogo.PodeAvancar)
                texto += " • retomada na entrada";
            if (textoSave.text != texto)
                textoSave.text = texto;
        }
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
        
    }

    public void Fechar()
    {
        gameObject.SetActive(false);
        //deixar o usuário livre para interagir com o jogo novamente
    }
}
