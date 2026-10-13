using UnityEngine;

public class DestacarObjeto : MonoBehaviour
{
    private Outline[] outlines;

    void Awake()
    {
        outlines = GetComponentsInChildren<Outline>(true);
        SetHighlight(false);
    }

    void Start()
    {
        SetHighlight(false);
    }

    public void SetHighlight(bool value)
    {
        if (outlines == null) return;

        foreach (Outline o in outlines)
        {
            if (o != null)
                o.enabled = value;
        }
    }
}