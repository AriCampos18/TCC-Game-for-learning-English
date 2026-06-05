using UnityEngine;
using UnityEngine.UI;

public class BotaoAjuda : MonoBehaviour
{
    public Button botaoAjuda;
    public GameObject modalInstrucoes;
    public FirstPlayerController firstPlayerControllerScript;

    void Start()
    {
        if (botaoAjuda != null)
            botaoAjuda.onClick.AddListener(AbrirInstrucoes);
    }

    void AbrirInstrucoes()
    {
        botaoAjuda.gameObject.SetActive(false);
        if (firstPlayerControllerScript != null)
            firstPlayerControllerScript.DesativarControle();
        
        modalInstrucoes.SetActive(true);
    }

    // ADICIONE OU ADAPTE ESTE MÉTODO QUE SEU BOTÃO DE FECHAR/PLAY CHAMA:
    public void FecharInstrucoes()
    {
        modalInstrucoes.SetActive(false);
        botaoAjuda.gameObject.SetActive(true);

        // O PULO DO GATO: Checa se o player estava conversando antes de liberar a câmera
        if (GameProgress.EstaEmDialogo)
        {
            // Se está em diálogo, mantemos o controle desativado e o mouse solto
            if (firstPlayerControllerScript != null)
            {
                firstPlayerControllerScript.DesativarControle(); 
                // Se o seu FirstPlayerController não força o cursor visível ao desativar, force aqui:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            Debug.Log("Instruções fechadas: Mantendo controles bloqueados pois está em diálogo.");
        }
        else
        {
            // Se NÃO está em diálogo, o player pode voltar a andar e olhar pro lado de boa
            if (firstPlayerControllerScript != null)
                firstPlayerControllerScript.AtivarControle(); // Ou o método equivalente que você usa para liberar
            
            Debug.Log("Instruções fechadas: Controles liberados para o gameplay.");
        }
    }
}