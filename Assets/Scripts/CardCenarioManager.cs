
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardCenarioManager : MonoBehaviour
{
    public string nomeCenario;
    public TMP_Text textoNome;
    public Button botao;

    private ModalSelecaoCenario modal;

    void Awake()
    {
        // Busca o botão nos filhos do cartão, se não foi associado.
        if (botao == null)
            botao = GetComponentInChildren<Button>(true);

        // Busca o texto dentro do botão.
        if (textoNome == null && botao != null)
            textoNome = botao.GetComponentInChildren<TMP_Text>(true);

        if (textoNome != null)
            textoNome.text = nomeCenario;
        else
            Debug.LogWarning(
                "Texto do botão não encontrado no cartão: " +
                gameObject.name
            );
    }

    void Start()
    {
        modal = FindObjectOfType<ModalSelecaoCenario>();

        if (botao != null)
        {
            botao.onClick.RemoveListener(AoClicar);
            botao.onClick.AddListener(AoClicar);
        }
        else
        {
            Debug.LogError(
                "Botão não encontrado no cartão: " +
                gameObject.name
            );
        }
    }

    public void AoClicar()
    {
        if (modal != null)
            modal.SelecionarCenario(nomeCenario);
        else
            Debug.LogError("ModalSelecaoCenario não encontrado.");
    }
}
