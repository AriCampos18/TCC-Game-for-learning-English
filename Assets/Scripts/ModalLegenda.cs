using UnityEngine;
using TMPro;

public class ModalLegenda : MonoBehaviour
{
    public TextMeshProUGUI textoLegenda, fonteLegenda;
    public ModalExercicio modalExercicio; 

    private RectTransform rect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void MostrarLegenda(string fonte, string texto)
    {
        fonteLegenda.text = fonte;
        textoLegenda.text = texto;

        // Executa a verificação de posição baseada no estado real do jogo
        AjustarPosicaoPeloEstadoDoJogo();

        gameObject.SetActive(true);
    }

    public void AjustarPosicaoPeloEstadoDoJogo()
    {
        // Se o modal de exercício existe e está ativo na tela (mesmo com Time.timeScale = 0)
        if (modalExercicio != null && modalExercicio.panel.activeSelf)
        {
            MoverParaExercicio();
        }
        else
        {
            VoltarParaCentro();
        }
    }

    public void MoverParaExercicio()
    {
        if (rect == null) rect = GetComponent<RectTransform>();

        // Alinha a âncora e o pivot à esquerda da tela
        rect.anchorMin = new Vector2(0f, 0f);
        rect.anchorMax = new Vector2(0f, 0f);
        rect.pivot = new Vector2(0f, 0f);

        // Define a distância das bordas (X = 50 pixels da esquerda, Y = 50 pixels de baixo)
        // Mude esses números se quiser mais para o lado ou para cima
        rect.anchoredPosition = new Vector2(50f, 50f); 
    }

    public void VoltarParaCentro()
    {
        if (rect == null) rect = GetComponent<RectTransform>();

        // Alinha a âncora e o pivot exatamente no centro-baixo da tela
        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0f);

        // No centro horizontal, o X deve ser estritamente 0! (Y = 50 pixels de baixo)
        rect.anchoredPosition = new Vector2(0f, 50f);
    }
}