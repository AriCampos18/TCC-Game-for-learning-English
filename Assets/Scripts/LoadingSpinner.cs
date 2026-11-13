using UnityEngine;

public class LoadingSpinner : MonoBehaviour
{
    [SerializeField] private float velocidade = 180f;

    void Update()
    {
        transform.Rotate(
            0f,
            0f,
            -velocidade * Time.deltaTime
        );
    }
}