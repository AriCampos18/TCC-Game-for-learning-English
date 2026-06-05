using UnityEngine;

public class AnimacaoOndas : MonoBehaviour
{
    public RectTransform barra1;
    public RectTransform barra2;
    public RectTransform barra3;

    void Update()
    {
        barra1.localScale = new Vector3(
            1,
            Mathf.Abs(Mathf.Sin(Time.time * 4)),
            1
        );

        barra2.localScale = new Vector3(
            1,
            Mathf.Abs(Mathf.Sin(Time.time * 5)),
            1
        );

        barra3.localScale = new Vector3(
            1,
            Mathf.Abs(Mathf.Sin(Time.time * 6)),
            1
        );
    }
}