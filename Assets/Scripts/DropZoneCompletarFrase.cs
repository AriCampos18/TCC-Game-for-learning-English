using UnityEngine;
using UnityEngine.EventSystems;

public class DropZoneCompletarFrase : MonoBehaviour, IDropHandler
{
    private CompletarFraseUI controlador;
    private int indiceResposta;

    public int IndiceResposta => indiceResposta;

    public void Configurar(CompletarFraseUI ui, int indice)
    {
        controlador = ui;
        indiceResposta = indice;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (controlador == null || eventData.pointerDrag == null)
            return;

        PalavraArrastavel palavra = eventData.pointerDrag.GetComponent<PalavraArrastavel>();
        if (palavra == null)
            return;

        bool aceita = palavra.TentarSoltarEm(this);
        palavra.FinalizarSoltura(this, aceita);
    }
}
