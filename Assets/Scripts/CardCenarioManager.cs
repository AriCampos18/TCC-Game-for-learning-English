using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardCenarioManager : MonoBehaviour
{
    public string nomeCenario;
    public TMP_Text textoNome;
    public Button botao;

    private ModalSelecaoCenario modal;

    void Start()
    {
        if (botao == null)
            botao = GetComponent<Button>();

        if (textoNome != null)
            textoNome.text = nomeCenario;

        modal = FindObjectOfType<ModalSelecaoCenario>();

        if (botao != null)
            botao.onClick.AddListener(AoClicar);
    }

    public void AoClicar()
    {
        if (modal != null)
        {
            modal.SelecionarCenario(nomeCenario);
        }
    }
}
