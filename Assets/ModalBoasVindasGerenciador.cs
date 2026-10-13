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
    public GameObject modalBoasVindas;
    public GameObject modalNivelamento, modalAvisoRevisao, modalSelecaoCenario;
    public GameObject modalResultadoNivelmento, modalInstrucoes, modalMissoes, modalAvisoCampoVazio;
    public TextMeshProUGUI explicacao, explicacaoPort;
    public Button botaoComecar, botaoAjuda, botaoRepetirFala;
    private FirstPlayerController firstPlayerControllerScript;
    public GameObject crosshair;
    public GameObject modalLegenda, modalCarregando, modalBarraProgressao, legendaMenor;
    public GameObject modalAvisoInteracaoMouse;

    public GameObject modalFinal;

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
            botaoComecar.gameObject.text = "Start Game";
            explicacao.text =
                "Welcome to our English learning game! In this game, you will explore a virtual market while practicing your English skills. To start, we will ask you a few questions to assess your current level of English proficiency. Please answer the essay questions with as much detail as possible. Let's get started!";

            explicacaoPort.text =
                "Bem-vindo ao nosso jogo de aprendizado de inglês! Neste jogo, você explorará um mercado virtual enquanto pratica suas habilidades em inglês. Para começar, vamos fazer algumas perguntas para avaliar seu nível atual de proficiência em inglês. Vamos começar!";
        }
        else if (tipo == TipoModal.AvisoProgressao)
        {
            botaoComecar.gameObject.text = "Escolher Cenário";
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
        }
    }

    public void FecharModal()
    {
        modalBoasVindas.SetActive(false);
        modalNivelamento.SetActive(true);
        modalInstrucoes.SetActive(false);
    }
}
