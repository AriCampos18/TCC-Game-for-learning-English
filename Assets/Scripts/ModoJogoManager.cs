// using UnityEngine;

// public class ModoJogoManager : MonoBehaviour
// {
//     public static ModoJogoManager Instance;
//     public bool uiMode;

//     void Awake()
//     {
//         if (Instance == null)
//         {
//             Instance = this;
//             uiMode = false;
//             DontDestroyOnLoad(gameObject);
//         }
//         else
//         {
//             Destroy(gameObject);
//         }
//     }

//     void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.LeftAlt))
//         {
//             uiMode = !uiMode;

//             Cursor.lockState = uiMode ? CursorLockMode.None : CursorLockMode.Locked;
//             Cursor.visible = uiMode;
//         }
//     }
// }

using UnityEngine;
using UnityEngine.XR;

public class ModoJogoManager : MonoBehaviour
{
    public static ModoJogoManager Instance;

    [Header("Estado da interface")]
    public bool uiMode;

    // Detecta se um dispositivo XR está ativo (ex.: Meta Quest via Quest Link).
    public bool ModoVR => XRSettings.isDeviceActive;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            uiMode = false;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // No PC, mantém o atalho original para liberar/travar o cursor.
        // No VR, a interação com a interface é feita pelo Ray Interactor
        // dos controles, então não usamos o cursor do mouse.
        if (!ModoVR && Input.GetKeyDown(KeyCode.LeftAlt))
        {
            AlternarModoUI();
        }
    }

    public void AlternarModoUI()
    {
        DefinirModoUI(!uiMode);
    }

    public void AtivarModoUI()
    {
        DefinirModoUI(true);
    }

    public void DesativarModoUI()
    {
        DefinirModoUI(false);
    }

    public void DefinirModoUI(bool ativar)
    {
        uiMode = ativar;

        // Cursor só existe no fluxo desktop.
        if (!ModoVR)
        {
            Cursor.lockState = uiMode
                ? CursorLockMode.None
                : CursorLockMode.Locked;

            Cursor.visible = uiMode;
        }
    }
}
