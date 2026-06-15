using UnityEngine;
using UnityEngine.SceneManagement;

public class ModalFinalManager : MonoBehaviour
{
    public void PlayAgain()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}