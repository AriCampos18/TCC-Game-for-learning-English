using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;    

public class ModalBoasVindasGerenciador : MonoBehaviour
{
    public enum TipoModal
    {
        BoasVindas,
        AvisoProgressao
    }
    public GameObject modalBoasVindas, modalSelecaoAvatar, modalErroSpeaking;
    public GameObject modalNivelamento, modalAvisoRevisao, modalSelecaoCenario;
    public GameObject modalResultadoNivelmento, modalInstrucoes, modalMissoes, modalAvisoCampoVazio;
    public TextMeshProUGUI explicacao, explicacaoPort;
    public Button botaoComecar, botaoAjuda, botaoRepetirFala;
    private FirstPlayerController firstPlayerControllerScript;
    public GameObject crosshair;
    public GameObject modalLegenda, modalCarregando, modalBarraProgressao, legendaMenor;
    public GameObject modalAvisoInteracaoMouse;

    public GameObject modalFinal;

    private string tipoModal;

    // Start is called before the first frame update
    void Start()
    {
        modalBoasVindas.SetActive(true);
        modalNivelamento.SetActive(false);
        modalLegenda.SetActive(false);
        modalSelecaoCenario.SetActive(false);
        modalResultadoNivelmento.SetActive(false); 
        modalInstrucoes.SetActive(false);
        botaoAjuda.gameObject.SetActive(false);
        modalFinal.SetActive(false);
        modalErroSpeaking.SetActive(false);
        modalSelecaoAvatar.SetActive(false);
        legendaMenor.SetActive(false);
        botaoRepetirFala.gameObject.SetActive(false);
        modalAvisoRevisao.SetActive(false);
        modalAvisoInteracaoMouse.SetActive(false);
        modalBarraProgressao.SetActive(false);
        modalAvisoCampoVazio.SetActive(false);
        modalCarregando.SetActive(false);
        modalMissoes.SetActive(false);
        AbrirModal(TipoModal.BoasVindas);
        firstPlayerControllerScript = FindObjectOfType<FirstPlayerController>();
        if (firstPlayerControllerScript != null)
            firstPlayerControllerScript.DesativarControle();
        if (botaoComecar != null)
            botaoComecar.onClick.AddListener(FecharModal);
        if (crosshair != null)
            crosshair.SetActive(false);
    }

    public void AbrirModal(TipoModal tipo, string resultado = "", string nivelAnterior = "", string nivelNovo = "")
    {
        modalBoasVindas.SetActive(true);

        if (tipo == TipoModal.BoasVindas)
        {
            tipoModal = "BoasVindas";
            botaoComecar.GetComponentInChildren<TMP_Text>().text = "Start Game";
            explicacao.text =
                "Welcome to our English learning game! Here, you will explore different virtual environments and practice English in everyday situations. First, we will ask you a few questions to assess your current English level. Please answer the writing questions with as much detail as possible. Get ready to learn, practice, and improve your English. Let's get started!";

            explicacaoPort.text =
                "Boas-vindas ao nosso jogo de aprendizagem de inglês! Aqui, você explorará diferentes ambientes virtuais e praticará inglês em situações do dia a dia. Primeiro, faremos algumas perguntas para avaliar seu nível atual de inglês. Responda às questões de escrita com o máximo de detalhes possível. Prepare-se para aprender, praticar e melhorar seu inglês. Vamos começar!";
        }
        else if (tipo == TipoModal.AvisoProgressao)
        {

            tipoModal = "AvisoProgressao";
            botaoComecar.GetComponentInChildren<TMP_Text>().text = "Escolher Cenário";
            if (resultado == "Avancar")
            {
                explicacao.text =
                    $"Congratulations! You advanced from level {nivelAnterior} to level {nivelNovo}!";

                explicacaoPort.text =
                    $"Parabéns! Você avançou do nível {nivelAnterior} para o nível {nivelNovo}!";
            }
            else if (resultado == "Regredir")
            {
                explicacao.text =
                    $"You moved back from level {nivelAnterior} to level {nivelNovo}. Keep practicing to improve your English!";

                explicacaoPort.text =
                    $"Você regrediu do nível {nivelAnterior} para o nível {nivelNovo}. Continue praticando para melhorar seu inglês!";
            }
            else if (resultado == "Manter")
            {
                explicacao.text =
                    $"You will remain at level {nivelNovo}. Keep practicing to improve your English!";

                explicacaoPort.text =
                    $"Você permanecerá no nível {nivelNovo}. Continue praticando para melhorar seu inglês!";
            }
            else if (resultado == "Concluido")
            {
                explicacao.text =
                    "Congratulations! You completed all English levels! " +
                    "You can now choose any scenario and keep practicing.";

                explicacaoPort.text =
                    "Parabéns! Você concluiu todos os níveis de inglês! " +
                    "Agora você pode escolher qualquer cenário e continuar praticando.";
            }
        }
    }

    public void FecharModal()
    {
        modalBoasVindas.SetActive(false);
        if(tipoModal == "BoasVindas")
        {
            modalNivelamento.SetActive(true);
        }
        else if(tipoModal == "AvisoProgressao")
        {
            modalSelecaoCenario.SetActive(true);
        }
    }
}
