using UnityEngine;

public class ModoJogoManager : MonoBehaviour
{
    public static ModoJogoManager Instance;
    public bool uiMode;

    void Awake()
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            uiMode = !uiMode;

            Cursor.lockState = uiMode ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = uiMode;
        }
    }
}