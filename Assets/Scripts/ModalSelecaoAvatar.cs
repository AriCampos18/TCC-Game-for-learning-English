
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ModalSelecaoAvatar : MonoBehaviour
{
    [Header("Botões de seleção")]
    public Button botaoFeminino;
    public Button botaoMasculino;
    public Button botaoConfirmar;

    [Header("Modelos 3D do jogador")]
    public GameObject avatarFemininoJogador;
    public GameObject avatarMasculinoJogador;

    [Header("Modais")]
    public GameObject modalSelecaoAvatar;
    public GameObject modalInstrucoes;

    [Header("Mensagem de erro")]
    public TextMeshProUGUI textoErro;

    private enum TipoAvatar
    {
        Nenhum,
        Feminino,
        Masculino
    }

    private TipoAvatar avatarSelecionado = TipoAvatar.Nenhum;
    private Outline outlineFeminino;
    private Outline outlineMasculino;

    public GameObject containerFeminino;
    public GameObject containerMasculino;

    void Start()
    {
        if (botaoFeminino != null)
            botaoFeminino.onClick.AddListener(SelecionarFeminino);

        if (botaoMasculino != null)
            botaoMasculino.onClick.AddListener(SelecionarMasculino);

        if (botaoConfirmar != null)
            botaoConfirmar.onClick.AddListener(ConfirmarSelecao);

        if (textoErro != null)
            textoErro.gameObject.SetActive(false);
        
        if (containerFeminino != null)
        {
            outlineFeminino = containerFeminino.GetComponent<Outline>();

            if (outlineFeminino != null)
                outlineFeminino.enabled = false;
            else
                Debug.LogError("Outline não encontrado no CardF.");
        }

        if (containerMasculino != null)
        {
            outlineMasculino = containerMasculino.GetComponent<Outline>();

            if (outlineMasculino != null)
                outlineMasculino.enabled = false;
            else
                Debug.LogError("Outline não encontrado no CardM.");
        }
    }

    public void SelecionarFeminino()
    {
        avatarSelecionado = TipoAvatar.Feminino;

        if (outlineFeminino != null)
            outlineFeminino.enabled = true;

        if (outlineMasculino != null)
            outlineMasculino.enabled = false;

        if (textoErro != null)
            textoErro.gameObject.SetActive(false);

        Debug.Log("Avatar feminino selecionado.");
    }

    public void SelecionarMasculino()
    {
        avatarSelecionado = TipoAvatar.Masculino;

        if (outlineMasculino != null)
            outlineMasculino.enabled = true;

        if (outlineFeminino != null)
            outlineFeminino.enabled = false;

        if (textoErro != null)
            textoErro.gameObject.SetActive(false);

        Debug.Log("Avatar masculino selecionado.");
    }

    void ConfirmarSelecao()
    {
        if (avatarSelecionado == TipoAvatar.Nenhum)
        {
            if (textoErro != null)
            {
                textoErro.text =
                    "Please select an avatar before continuing.";

                textoErro.gameObject.SetActive(true);
            }

            return;
        }

        if (avatarFemininoJogador != null)
        {
            avatarFemininoJogador.SetActive(
                avatarSelecionado == TipoAvatar.Feminino
            );
        }

        if (avatarMasculinoJogador != null)
        {
            avatarMasculinoJogador.SetActive(
                avatarSelecionado == TipoAvatar.Masculino
            );
        }

        if (modalSelecaoAvatar != null)
            modalSelecaoAvatar.SetActive(false);

        if (modalInstrucoes != null)
            modalInstrucoes.SetActive(true);
        else
            Debug.LogError(
                "Modal de instruções não foi configurado."
            );
    }
}
