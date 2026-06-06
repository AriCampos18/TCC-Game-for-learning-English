using UnityEngine;
using System.Threading.Tasks;

public class InteracaoNPC : MonoBehaviour
{
    public GameObject avisoUI;
    public Transform playerCamera;
    public FirstPlayerController movimentoPlayer;
    public float alturaFocoCamera = 0.82f;
    public GameObject crosshair;

    // ✨ Mudado para protected para que ShelfNPC consiga atualizar a posição base após andar
    protected Vector3 npcPosicaoOriginal;
    protected Quaternion npcRotacaoOriginal;

    private Quaternion cameraRotacaoOriginal;
    public string ultimaFraseDita = "";
    private Vector3 cameraPosicaoOriginal;

    protected virtual void Start()
    {
        if (avisoUI != null) avisoUI.SetActive(false);
    }

    public void MostrarAviso(bool value)
    {
        if (avisoUI != null) avisoUI.SetActive(value);
    }

    // 1. INÍCIO DA INTERAÇÃO: Trava o player e vira o NPC
    public async Task Interagir()
    {
        MostrarAviso(false);

        if (movimentoPlayer != null)
        {
            movimentoPlayer.DesativarControle();

            // Faz a câmera do player olhar para o NPC
            Vector3 alvo;
            Renderer r = GetComponentInChildren<Renderer>();

            if (r != null)
            {
                Bounds bounds = r.bounds;
                alvo = bounds.center;
                alvo.y = Mathf.Lerp(bounds.min.y, bounds.max.y, alturaFocoCamera);
            }
            else
            {
                alvo = transform.position + Vector3.up * 1.4f;
            }

            Vector3 direcao = alvo - movimentoPlayer.transform.position;
            Quaternion rot = Quaternion.LookRotation(direcao);
            movimentoPlayer.transform.rotation = Quaternion.Euler(0, rot.eulerAngles.y, 0);

            Vector3 direcaoCamera = alvo - movimentoPlayer.playerCamera.position;
            float anguloX = Mathf.Asin(direcaoCamera.normalized.y) * Mathf.Rad2Deg;
            movimentoPlayer.playerCamera.localRotation = Quaternion.Euler(-anguloX, 0, 0);

            movimentoPlayer.SincronizarRotacaoInterna(); 
        }

        await VirarParaPlayer();
        await IniciarInteracao();
    }

    private async Task VirarParaPlayer()
    {
        if (movimentoPlayer == null) return;

        Vector3 direcao = movimentoPlayer.transform.position - transform.position;
        direcao.y = 0; 

        if (direcao != Vector3.zero)
        {
            Quaternion rotacaoInicial = transform.rotation;
            Quaternion rotacaoFinal = Quaternion.LookRotation(direcao);

            float tempo = 0f;
            float duracao = 0.25f;

            while (tempo < duracao)
            {
                tempo += Time.deltaTime;
                transform.rotation = Quaternion.Slerp(rotacaoInicial, rotacaoFinal, tempo / duracao);
                await Task.Yield();
            }
        }
    }

    public async Task EntrarModoExercicio()
    {
        if (movimentoPlayer == null) return;

        Transform cam = playerCamera;

        npcPosicaoOriginal = transform.position;
        npcRotacaoOriginal = transform.rotation;

        cameraRotacaoOriginal = cam.localRotation;
        cameraPosicaoOriginal = cam.position;

        Vector3 frentePlana = cam.forward;
        frentePlana.y = 0f;
        frentePlana.Normalize();

        Vector3 direitaPlana = cam.right;
        direitaPlana.y = 0f;
        direitaPlana.Normalize();

        Vector3 posicaoAlvoNPC = cam.position + (frentePlana * 1.5f) + (-direitaPlana * 0.6f);
        posicaoAlvoNPC.y = npcPosicaoOriginal.y; 

        Vector3 direcaoOlhar = cam.position - posicaoAlvoNPC;
        direcaoOlhar.y = 0;
        Quaternion rotNPCFinal = Quaternion.LookRotation(direcaoOlhar);

        Vector3 npcInicio = transform.position;
        Quaternion rotNPCInicio = transform.rotation;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f; 
            transform.position = Vector3.Lerp(npcInicio, posicaoAlvoNPC, t);
            transform.rotation = Quaternion.Slerp(rotNPCInicio, rotNPCFinal, t);
            await Task.Yield();
        }

        cam.localRotation = cameraRotacaoOriginal * Quaternion.Euler(0f, 8f, 0f);
    }

    public async Task SairModoExercicio()
    {
        if (movimentoPlayer == null) return;

        Transform cam = playerCamera;
        Vector3 npcInicio = transform.position;
        Quaternion rotNPCInicio = transform.rotation;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            transform.position = Vector3.Lerp(npcInicio, npcPosicaoOriginal, t);
            transform.rotation = Quaternion.Slerp(rotNPCInicio, npcRotacaoOriginal, t);
            cam.localRotation = Quaternion.Slerp(cam.localRotation, cameraRotacaoOriginal, t);
            await Task.Yield();
        }

        // ✨ COMANDO REMOVIDO DAQUI! 
        // Quem controla a liberdade do player agora é o fluxo de diálogo de cada NPC.
    }

    // ✨ Métodos utilitários para controle manual externo
    public void LiberarControlePlayer()
    {
        if (movimentoPlayer != null) 
        {
            movimentoPlayer.AtivarControle();
        }
        crosshair.SetActive(true); 
    }

    public void TravarControlePlayer()
    {
        if (movimentoPlayer != null) movimentoPlayer.DesativarControle();
    }

    protected virtual Task IniciarInteracao()
    {
        return Task.CompletedTask;
    }

    public virtual bool PodeInteragir()
    {
        return true;
    }

    public virtual async Task FalarFraseCustomizada(string textoParaFalar)
    {
        // Se a lógica de gerar áudio for idêntica para todos, ela pode ficar direto aqui!
        await Task.CompletedTask;
    }
}