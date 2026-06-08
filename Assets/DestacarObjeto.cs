using UnityEngine;

public class DestacarObjeto : MonoBehaviour
{
    private Outline outline;

    void Awake()
    {
        outline = GetComponent<Outline>();

        if (outline != null)
            outline.enabled = false;
    }

    public void SetHighlight(bool value)
    {
        if (outline != null)
            outline.enabled = value;
    }
}