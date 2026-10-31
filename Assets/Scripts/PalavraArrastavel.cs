using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PalavraArrastavel : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private CompletarFraseUI controlador;
    private RectTransform canvasRaiz;
    private CanvasGroup canvasGroup;
    private Transform paiOriginal;
    private int indiceOriginal;
    private Vector3 posicaoLocalOriginal;
    private Vector2 posicaoAncoradaOriginal;
    private Quaternion rotacaoLocalOriginal;
    private Vector3 escalaLocalOriginal;
    private bool arrastando;
    private bool dropAceito;
    private bool bloqueado;

    public void Configurar(
        Transform containerOrigem,
        CompletarFraseUI ui)
    {
        paiOriginal = containerOrigem;
        controlador = ui;
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public string ObterPalavra()
    {
        TextMeshProUGUI texto = GetComponentInChildren<TextMeshProUGUI>();
        return texto != null ? texto.text : string.Empty;
    }

    public bool PodeSerRecebidoEm(DropZoneCompletarFrase zona)
    {
        return !bloqueado && !dropAceito && zona != null;
    }

    public bool TentarSoltarEm(DropZoneCompletarFrase zona)
    {
        return PodeSerRecebidoEm(zona) && controlador != null &&
            controlador.ProcessarSoltura(this, zona);
    }

    public void FinalizarSoltura(DropZoneCompletarFrase zona, bool aceita)
    {
        if (!aceita)
            return;

        dropAceito = true;
        transform.SetParent(zona.transform, false);

        RectTransform rectTransform = transform as RectTransform;
        if (rectTransform != null)
            rectTransform.anchoredPosition = Vector2.zero;

        if (canvasGroup != null)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = true;
        }
    }

    public void Bloquear()
    {
        bloqueado = true;
        if (canvasGroup != null)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (bloqueado || dropAceito)
            return;

        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("O bloco arrastável precisa estar dentro de um Canvas.");
            return;
        }

        canvasRaiz = canvas.rootCanvas.transform as RectTransform;
        if (canvasRaiz == null)
        {
            Debug.LogError("O Canvas raiz do bloco arrastável precisa ter um RectTransform.");
            return;
        }

        arrastando = true;
        dropAceito = false;
        paiOriginal = transform.parent;
        indiceOriginal = transform.GetSiblingIndex();
        posicaoLocalOriginal = transform.localPosition;
        rotacaoLocalOriginal = transform.localRotation;
        escalaLocalOriginal = transform.localScale;
        RectTransform rectTransform = transform as RectTransform;
        if (rectTransform != null)
            posicaoAncoradaOriginal = rectTransform.anchoredPosition;

        canvasGroup.blocksRaycasts = false;
        transform.SetParent(canvasRaiz, true);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (arrastando &&
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                canvasRaiz,
                eventData.position,
                eventData.pressEventCamera,
                out Vector3 posicao))
        {
            transform.position = posicao;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!arrastando)
            return;

        arrastando = false;
        if (!dropAceito)
        {
            transform.SetParent(paiOriginal, false);
            transform.SetSiblingIndex(indiceOriginal);
            transform.localPosition = posicaoLocalOriginal;
            transform.localRotation = rotacaoLocalOriginal;
            transform.localScale = escalaLocalOriginal;

            RectTransform rectTransform = transform as RectTransform;
            if (rectTransform != null)
                rectTransform.anchoredPosition = posicaoAncoradaOriginal;
        }

        canvasGroup.interactable = !bloqueado && !dropAceito;
        canvasGroup.blocksRaycasts = !bloqueado || dropAceito;
    }
}
