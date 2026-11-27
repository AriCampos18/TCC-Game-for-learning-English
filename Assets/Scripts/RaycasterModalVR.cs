using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class RaycasterModalVR : GraphicRaycaster
{
    internal static readonly Dictionary<int, RectTransform> PaineisPorPonteiro = new();
    private static readonly List<RaycastResult> candidatos = new();
    private static bool coletando;
    internal static Camera CameraDeProjecao { get; private set; }
    private Canvas canvasLocal;

    public override Camera eventCamera
    {
        get
        {
            if (ModoJogoManager.Instance != null && ModoJogoManager.Instance.ModoVR)
            {
                if (canvasLocal == null) canvasLocal = GetComponent<Canvas>();
                if (canvasLocal != null && canvasLocal.rootCanvas.worldCamera != null)
                    return canvasLocal.rootCanvas.worldCamera;
            }
            return base.eventCamera;
        }
    }

    public override void Raycast(PointerEventData evento, List<RaycastResult> resultados)
    {
        var manager = ModoJogoManager.Instance;
        if (coletando || manager == null || !manager.ModoVR)
        {
            base.Raycast(evento, resultados);
            return;
        }

        if (!PaineisPorPonteiro.TryGetValue(evento.pointerId, out var painel) ||
            painel == null || !ModoJogoManager.PermitePonteiro(painel))
            return;

        if (canvasLocal == null) canvasLocal = GetComponent<Canvas>();
        var canvasPainel = painel.GetComponent<Canvas>();
        if (canvasLocal == null || canvasPainel == null || canvasLocal != canvasPainel.rootCanvas)
            return;

        // Collect the normal, globally sorted uGUI raycast on behalf of this root.
        // A foreground graphic owned by another modal occludes this pointer.
        var sistema = EventSystem.current;
        if (sistema == null) return;

        CameraDeProjecao = canvasLocal.worldCamera;
        coletando = true;
        try
        {
            sistema.RaycastAll(evento, candidatos);
            foreach (var candidato in candidatos)
            {
                if (candidato.module is not GraphicRaycaster || candidato.gameObject == null) continue;
                var alvo = candidato.gameObject.transform;
                var canvasAlvo = alvo.GetComponentInParent<Canvas>();
                if (canvasAlvo == null || canvasAlvo.rootCanvas != canvasLocal) continue;
                if (manager.PainelDe(alvo) == painel)
                {
                    var resultado = candidato;
                    resultado.index = resultados.Count;
                    resultados.Add(resultado);
                }
                break;
            }
        }
        finally
        {
            candidatos.Clear();
            coletando = false;
        }
    }
}
