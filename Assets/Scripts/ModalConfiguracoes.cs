
using UnityEngine;
using UnityEngine.UI;

public class ModalConfiguracoes : MonoBehaviour
{
    public GameObject modalGerenciarExs, modalInstrucoes;
    public Button botaoSmooth, botaoSnap;
    public Button botaoLogout, botaoInstrucoes, botaoSave, botaoSairJogo;
    public Button botaoGerenciarExs, botaoFecharConfig;


    private void Awake()
    {
        if (botaoLogout != null)
            botaoLogout.onClick.AddListener(Logout);

        if (botaoInstrucoes != null)
            botaoInstrucoes.onClick.AddListener(AbrirInstrcoes);

        if (botaoSave != null)
            botaoSave.onClick.AddListener(Save);

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

    public void Logout()
    {
        // Implementar a lógica de logout aqui
    }

    public void Save()
    {
        // Implementar a lógica de salvar configurações aqui
    }

    public void SairJogo()
    {
        // Implementar a lógica de sair do jogo aqui
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
