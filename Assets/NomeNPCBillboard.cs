using UnityEngine;

public class NomeNPCBillboard : MonoBehaviour
{
    public Transform pontoAlvo; 
    public float suavizacao = 20f;

    private Camera cameraPrincipal;

    void Start()
    {
        cameraPrincipal = Camera.main;

        // Caso esqueça de arrastar no Inspector, tenta pegar o pai como fallback
        if (pontoAlvo == null)
            pontoAlvo = transform.parent;
    }

    void LateUpdate()
    {
        if (cameraPrincipal == null || pontoAlvo == null)
            return;

        // Segue a posição exata do ponto que você definiu
        transform.position = Vector3.Lerp(
            transform.position,
            pontoAlvo.position,
            Time.deltaTime * suavizacao
        );

        // Mantém o texto virado para a câmera (Billboard)
        transform.LookAt(
            transform.position + cameraPrincipal.transform.rotation * Vector3.forward,
            cameraPrincipal.transform.rotation * Vector3.up
        );
    }
}