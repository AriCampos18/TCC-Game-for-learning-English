using UnityEngine;

public class FirstPlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float mouseSensitivity = 2f;
    public float jumpHeight = 1.5f;
    public float gravity = -15f;

    public Transform playerCamera;

    private CharacterController controller;
    private float cameraRotationX = 0f;
    private float velocityY = 0f;

    // ✨ Nova variável de controle para não precisar desativar o componente inteiro (enabled = false)
    private bool controleAtivo = true;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Time.timeScale = 1;
        
        // Garante o estado inicial do cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // ✨ Agora checa se o controle está ativo e se não está em modo UI
        if (controleAtivo && !ModoJogoManager.Instance.uiMode)
        {
            // MOVIMENTO
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.TransformDirection(new Vector3(x, 0, z));
            move *= speed;

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
    }

    // ✨ FUNÇÃO MÁGICA: Sincroniza a rotação interna após o InteracaoNPC forçar o olhar
    public void SincronizarRotacaoInterna()
    {
        // Extrai a rotação X atual que a câmera recebeu de fora e atualiza a variável do mouse
        float angulo = playerCamera.localEulerAngles.x;
        
        // O Unity trabalha com ângulos de 0 a 360. Convertemos para a escala de -180 a 180 que o Clamp usa
        if (angulo > 180f) angulo -= 360f;
        
        cameraRotationX = angulo;
    }

    public void AtivarControle()
    {
        controleAtivo = true; // ✨ Ativa as checagens no Update sem desligar o script

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void DesativarControle()
    {
        controleAtivo = false; // ✨ Trava teclado e mouse de andar, mas o script continua "vivo" para atualizações

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}