using UnityEngine;

public class FirstPlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float mouseSensitivity = 2f;
    public float jumpHeight = 1.5f;
    public float gravity = -15f;

    public AudioSource audioPassos;
    public AudioClip somPasso;

    private float timerPasso;
    public float intervaloPasso = 0.5f;

    public Transform playerCamera;

    [Header("Animação")]
    public Animator animator;

    private CharacterController controller;
    private float cameraRotationX = 0f;
    private float velocityY = 0f;

    private bool controleAtivo = true;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Time.timeScale = 1;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (controleAtivo && !ModoJogoManager.Instance.uiMode)
        {
            // MOVIMENTO
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            bool andando = Mathf.Abs(x) > 0.1f || Mathf.Abs(z) > 0.1f;
            if (andando && controller.isGrounded)
            {
                timerPasso += Time.deltaTime;

                if (timerPasso >= intervaloPasso)
                {
                    if (audioPassos != null && somPasso != null)
                    {
                        audioPassos.PlayOneShot(somPasso);
                    }

                    timerPasso = 0f;
                }
            }
            else
            {
                timerPasso = intervaloPasso;

                if (audioPassos != null && audioPassos.isPlaying)
                {
                    audioPassos.Stop();
                }
            }

            Vector3 move = transform.TransformDirection(new Vector3(x, 0, z));
            move *= speed;

            // ANIMAÇÃO
            andando = Mathf.Abs(x) > 0.1f || Mathf.Abs(z) > 0.1f;

            if (animator != null)
            {
                animator.SetBool("andando", andando);
            }

            // Gravidade
            if (controller.isGrounded && velocityY < 0)
            {
                velocityY = -2f;
            }

            velocityY += gravity * Time.deltaTime;
            move.y = velocityY;

            controller.Move(move * Time.deltaTime);

            // MOUSE
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            transform.Rotate(Vector3.up * mouseX);

            cameraRotationX -= mouseY;
            cameraRotationX = Mathf.Clamp(cameraRotationX, -80f, 80f);

            playerCamera.localRotation = Quaternion.Euler(cameraRotationX, 0f, 0f);
        }
        else
        {
            if (animator != null)
            {
                animator.SetBool("andando", false);
            }
        }
    }

    public void SincronizarRotacaoInterna()
    {
        float angulo = playerCamera.localEulerAngles.x;

        if (angulo > 180f) angulo -= 360f;

        cameraRotationX = angulo;
    }

    public void AtivarControle()
    {
        controleAtivo = true;

        if (ModoJogoManager.Instance != null)
            ModoJogoManager.Instance.uiMode = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("Controle ativado. controleAtivo=true, uiMode=false");
    }

    public void DesativarControle()
    {
        controleAtivo = false;

        if (animator != null)
        {
            animator.SetBool("andando", false);
        }

        if (audioPassos != null)
        {
            audioPassos.Stop();
        }

        timerPasso = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}