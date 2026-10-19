
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ModalFinalManager : MonoBehaviour
{
    public GameObject modalFinal;
    public TMP_Text titulo;
    public TMP_Text textoIngles;
    public TMP_Text textoPortugues;
    public TMP_Text textoBotao;
    public Button botaoContinuar;
    public GameObject modalBarraProgressao;
    public GameObject modalMissoes;
    public Button botaoAjuda;
    public GameObject crosshair;

    private string modoAtual;
    private string cenarioAtual;

    [Header("Modo de teste")]
    public bool modoTesteSemExercicios = true;

    private void Awake()
    {
        if (modalFinal != null)
            modalFinal.SetActive(false);

        if (botaoContinuar != null)
        {
            botaoContinuar.onClick.RemoveListener(
                Continuar
            );

            botaoContinuar.onClick.AddListener(
                Continuar
            );
        }
    }

    public void MostrarInicioCenario(string cenario)
    {
        modoAtual = "InicioCenario";
        cenarioAtual = cenario;

        string nome;
        string ingles;
        string portugues;

        switch (cenario)
        {
            case "Home":
                nome = "At Home";
                ingles = "Practice talking about daily routines, "
                    + "family, and objects at home.";
                portugues = "Pratique conversas sobre a rotina "
                    + "diária, a família e os objetos de casa.";
                break;

            case "Market":
                nome = "At the Market";
                ingles = "Practice asking for products, "
                    + "talking about food, and shopping.";
                portugues = "Pratique como perguntar sobre "
                    + "produtos, falar sobre alimentos e fazer compras.";
                break;

            case "College":
                nome = "At College";
                ingles = "Practice talking about studies, "
                    + "subjects, and college life.";
                portugues = "Pratique conversas sobre estudos, "
                    + "disciplinas e a vida na faculdade.";
                break;

            case "Hotel":
                nome = "At the Hotel";
                ingles = "Practice checking in, asking about "
                    + "your room, and talking to the receptionist.";
                portugues = "Pratique como fazer o check-in, "
                    + "perguntar sobre o quarto e conversar com "
                    + "o recepcionista.";
                break;

            case "Airport":
                nome = "At the Airport";
                ingles = "Practice checking in, asking for "
                    + "directions, and understanding travel information.";
                portugues = "Pratique como fazer o check-in, "
                    + "pedir informações e entender orientações "
                    + "sobre viagens.";
                break;

            case "Restaurant":
                nome = "At the Restaurant";
                ingles = "Practice ordering food, asking about "
                    + "the menu, and talking to the waiter.";
                portugues = "Pratique como pedir comida, "
                    + "perguntar sobre o cardápio e conversar "
                    + "com o garçom.";
                break;

            case "Tourist Information Center":
                nome = "Visitor Center";
                ingles = "Practice asking about tourist "
                    + "attractions, directions, and local places.";
                portugues = "Pratique como perguntar sobre "
                    + "atrações turísticas, direções e lugares "
                    + "da cidade.";
                break;

            default:
                Debug.LogError(
                    "Não há descrição cadastrada para: " + cenario
                );
                return;
        }

        titulo.text = "Welcome to " + nome + "!";
        textoIngles.text = ingles;
        textoPortugues.text = portugues;
        textoBotao.text = "Start Scenario";

        Abrir();
    }

    public void MostrarResultado(string resultado, string nivelAnterior, string nivelNovo)
    {
        modoAtual = "FimCenario";

        switch (resultado)
        {
            case "ContinuarCenarios":
                titulo.text = "Great job!";
                textoIngles.text =
                    "You have completed this scenario. "
                    + "Choose another scenario to continue "
                    + "practicing at your current level.";
                textoPortugues.text =
                    "Você concluiu este cenário. Escolha outro "
                    + "cenário para continuar praticando no "
                    + "nível atual.";
                break;

            case "CenarioExtra":
                titulo.text = "One More Challenge!";
                textoIngles.text =
                    "You need to complete one extra scenario "
                    + "to evaluate your English level.";
                textoPortugues.text =
                    "Você precisa concluir um cenário extra "
                    + "para avaliar seu nível de inglês.";
                break;

            case "Avancar":
                titulo.text = "Congratulations!";
                textoIngles.text =
                    $"You advanced from level {nivelAnterior} "
                    + $"to level {nivelNovo}!";
                textoPortugues.text =
                    $"Parabéns! Você avançou do nível "
                    + $"{nivelAnterior} para o nível {nivelNovo}!";
                break;

            case "Regredir":
                titulo.text = "Keep Practicing!";
                textoIngles.text =
                    $"You will continue at level {nivelNovo}. "
                    + "Practice more to strengthen your English skills.";
                textoPortugues.text =
                    $"Você continuará no nível {nivelNovo}. "
                    + "Pratique mais para fortalecer suas "
                    + "habilidades em inglês.";
                break;

            case "Manter":
                titulo.text = "Keep Going!";
                textoIngles.text =
                    $"You will remain at level {nivelNovo}. "
                    + "Keep practicing to improve your English.";
                textoPortugues.text =
                    $"Você permanecerá no nível {nivelNovo}. "
                    + "Continue praticando para melhorar seu inglês.";
                break;

            case "Concluido":
                titulo.text = "Congratulations!";
                textoIngles.text =
                    "You have completed all English levels! "
                    + "Choose any scenario to keep practicing.";
                textoPortugues.text =
                    "Parabéns! Você concluiu todos os níveis "
                    + "de inglês! Escolha qualquer cenário "
                    + "para continuar praticando.";
                break;
            case "Teste":
                titulo.text = "Scenario Completed!";

                textoIngles.text =
                    "This is a test message. No exercises were completed, "
                    + "and your English level has not changed.";

                textoPortugues.text =
                    "Esta é uma mensagem de teste. Nenhum exercício foi "
                    + "realizado e seu nível de inglês não foi alterado.";

                break;
            default:
                Debug.LogWarning(
                    "Resultado não reconhecido: " + resultado
                );
                return;
        }

        textoBotao.text = "Choose Scenario";
        Abrir();
    }

    private void Abrir()
    {
        if (modalFinal != null)
            modalFinal.SetActive(true);
    }

    public void Continuar()
    {
        if (modalFinal != null)
            modalFinal.SetActive(false);
        
        if (modoAtual == "InicioCenario")
        {
            // Os outros cenários ainda estão em modo de teste.
            if (cenarioAtual != "Market" && modoTesteSemExercicios)
            {
                modoAtual = "FimCenario";

                MostrarResultado(
                    "Teste",
                    DadosJogador.nivelUsuario,
                    DadosJogador.nivelUsuario
                );

                return;
            }

            // Inicia o cenário do mercado.
            GerenciadorProgressao gerenciador =
                GerenciadorProgressao.Instance;

            if (gerenciador != null)
                gerenciador.IniciarCenario();

            if (modalBarraProgressao != null)
                modalBarraProgressao.SetActive(true);

            if (modalMissoes != null)
                modalMissoes.SetActive(true);

            if (botaoAjuda != null)
                botaoAjuda.gameObject.SetActive(true);

            if (crosshair != null)
                crosshair.SetActive(true);

            FirstPlayerController player =
                FindObjectOfType<FirstPlayerController>();

            if (player != null)
                player.AtivarControle();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            return;
        }

        // O botão do modal final sempre encaminha
        // o jogador para a seleção de cenários.
        if (modoAtual == "FimCenario")
        {
            GerenciadorProgressao gerenciador =
                GerenciadorProgressao.Instance;

            if (gerenciador != null)
                gerenciador.ContinuarParaSelecao();
        }
    }
}
