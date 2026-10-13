using UnityEngine;

public class NomeNPCBillboard : MonoBehaviour
{
    public Transform pontoAlvo;

    private Camera cameraPrincipal;

    void Start()
    {
        cameraPrincipal = Camera.main;
    }

    void LateUpdate()
    {
        if (cameraPrincipal != null && pontoAlvo != null)
        {
            transform.position = pontoAlvo.position;

            transform.LookAt(
                transform.position + cameraPrincipal.transform.rotation * Vector3.forward,
                cameraPrincipal.transform.rotation * Vector3.up
            );
        }
    }
}