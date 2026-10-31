using UnityEngine;
using UnityEngine.EventSystems;

public class DropZoneCompletarFrase : MonoBehaviour, IDropHandler
{
    private CompletarFraseUI controlador;

    public void Configurar(CompletarFraseUI ui)
    {
        controlador = ui;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        PalavraArrastavel palavra = eventData.pointerDrag.GetComponent<PalavraArrastavel>();
        if (palavra == null)
            return;

        bool aceita = palavra.TentarSoltarEm(this);
        palavra.FinalizarSoltura(this, aceita);
    }
}
