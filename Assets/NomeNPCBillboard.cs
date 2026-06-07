using UnityEngine;

public class NomeNPCBillboard : MonoBehaviour
{
    private Camera cameraPrincipal;

    void Start()
    {
        cameraPrincipal = Camera.main;
    }

    void LateUpdate()
    {
        if (cameraPrincipal == null) return;

        transform.LookAt(transform.position + cameraPrincipal.transform.rotation * Vector3.forward,
                         cameraPrincipal.transform.rotation * Vector3.up);
    }
}