using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;    

public class ModalBoasVindasGerenciador : MonoBehaviour
{
    public GameObject modalBoasVindas;
    public GameObject modalNivelamento, modalAvisoRevisao;
    public GameObject modalResultadoNivelmento, modalInstrucoes, modalMissoes, modalAvisoCampoVazio;
    public TextMeshProUGUI explicacao, explicacaoPort;
    public Button botaoComecar, botaoAjuda, botaoRepetirFala;
    private FirstPlayerController firstPlayerControllerScript;
    public GameObject crosshair;
    public GameObject modalLegenda, modalCarregando, modalBarraProgressao, legendaMenor;
    public GameObject modalAvisoInteracaoMouse;

    // Start is called before the first frame update
    void Start()
    {
        modalBoasVindas.SetActive(true);
        modalNivelamento.SetActive(false);
        modalLegenda.SetActive(false);
        modalResultadoNivelmento.SetActive(false); 
        modalInstrucoes.SetActive(false);
        botaoAjuda.gameObject.SetActive(false);
        legendaMenor.SetActive(false);
        botaoRepetirFala.gameObject.SetActive(false);
        modalAvisoRevisao.SetActive(false);
        modalAvisoInteracaoMouse.SetActive(false);
        modalBarraProgressao.SetActive(false);
        modalAvisoCampoVazio.SetActive(false);
        modalCarregando.SetActive(false);
        modalMissoes.SetActive(false);
        explicacao.text = "Welcome to our English learning game! In this game, you will explore a virtual market while practicing your English skills. To start, we will ask you a few questions to assess your current level of English proficiency. Please answer the essay questions with as much detail as possible, this will help us tailor the game experience to your needs and make it more enjoyable for you. Let's get started!";
        explicacaoPort.text = "Bem-vindo ao nosso jogo de aprendizado de inglês! Neste jogo, você explorará um mercado virtual enquanto pratica suas habilidades em inglês. Para começar, vamos fazer algumas perguntas para avaliar seu nível atual de proficiência em inglês. Por favor, responda com o máximo de detalhes que conseguir nas perguntas discursivas, isso nos ajudará a personalizar a experiência do jogo de acordo com suas necessidades e torná-la mais agradável para você. Vamos começar!";
        firstPlayerControllerScript = FindObjectOfType<FirstPlayerController>();
        if (firstPlayerControllerScript != null)
            firstPlayerControllerScript.DesativarControle();
        if (botaoComecar != null)
            botaoComecar.onClick.AddListener(FecharModal);
        if (crosshair != null)
            crosshair.SetActive(false);
    }

    public void FecharModal()
    {
        modalBoasVindas.SetActive(false);
        modalNivelamento.SetActive(true);
        modalInstrucoes.SetActive(false);
    }
}
