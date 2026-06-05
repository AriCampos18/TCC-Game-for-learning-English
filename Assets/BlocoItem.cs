using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BlocoItem : MonoBehaviour
{
    private TextMeshProUGUI textoComponente;
    private Transform containerDisponiveis;
    private Transform containerMontagem;
    private Button botao;

    private bool estaNaMontagem = false;

    public void Setup(Transform disponiveis, Transform montagem)
    {
        containerDisponiveis = disponiveis;
        containerMontagem = montagem;

        textoComponente = GetComponentInChildren<TextMeshProUGUI>();
        botao = GetComponent<Button>();

        if (botao != null)
        {
            botao.onClick.RemoveAllListeners();
            botao.onClick.AddListener(AlternarPosicao);
        }
    }

    public string ObterPalavra()
    {
        return textoComponente != null ? textoComponente.text : "";
    }

    private void AlternarPosicao()
    {
        if (!estaNaMontagem)
        {
            // Move para a área onde o jogador está montando a frase
            transform.SetParent(containerMontagem, false);
            estaNaMontagem = true;
        }
        else
        {
            // Devolve para a área de palavras disponíveis
            transform.SetParent(containerDisponiveis, false);
            estaNaMontagem = false;
        }
    }
}