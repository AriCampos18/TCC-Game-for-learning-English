using UnityEngine;

public class DestacarObjeto : MonoBehaviour
{
    private Outline outline;

    void Start()
    {
        outline = GetComponentInChildren<Outline>();

        if (outline != null)
            outline.enabled = false;
    }

    public void SetHighlight(bool value)
    {
        if (outline != null)
            outline.enabled = value;
    }
}