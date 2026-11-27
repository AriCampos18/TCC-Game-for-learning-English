// using UnityEngine;
// using System.Threading.Tasks;
// using System.Collections.Generic;
// using TMPro;

// public class InteracaoNPC : MonoBehaviour
// {
//     public GameObject avisoUI;
//     public Transform playerCamera;
//     public string nomeExibicao = "NPC";
//     public FirstPlayerController movimentoPlayer;
//     protected bool interacaoConcluida = false;

//     private bool interagindo = false;
//     public float alturaFocoCamera = 0.82f;
//     public GameObject crosshair;
//     [SerializeField] protected ModalExercicio modalExercicioComum;
//     [SerializeField] protected GameObject modalLegendaComum;
//     [SerializeField] protected TextMeshProUGUI textoPularDialogoComum;

//     // ✨ Mudado para protected para que ShelfNPC consiga atualizar a posição base após andar
//     protected Vector3 npcPosicaoOriginal;
//     protected Quaternion npcRotacaoOriginal;

//     private Quaternion cameraRotacaoOriginal;
//     public string ultimaFraseDita = "";
//     private Vector3 cameraPosicaoOriginal;
//     protected AudioSource audioSourceComum;
//     protected BackendManager backendManagerComum;

//     protected virtual void Start()
//     {
//         if (avisoUI != null) avisoUI.SetActive(false);
//         audioSourceComum = GetComponent<AudioSource>();
//         backendManagerComum = new BackendManager();
//     }

//     public virtual string nomeExibicaoLegenda 
//     {
//         get => nomeExibicao;
//         set => nomeExibicao = value;
//     }

//     public void MostrarAviso(bool value)
//     {
//         if (avisoUI != null) avisoUI.SetActive(value);
//     }

//     // 1. INÍCIO DA INTERAÇÃO: Trava o player e vira o NPC
//     public async Task Interagir()
//     {
//         if (interagindo || interacaoConcluida) return;

//         interagindo = true;

//         try
//         {
//             MostrarAviso(false);

//             if (movimentoPlayer != null)
//             {
//                 movimentoPlayer.DesativarControle();

//                 Vector3 alvo;
//                 Renderer r = GetComponentInChildren<Renderer>();

//                 if (r != null)
//                 {
//                     Bounds bounds = r.bounds;
//                     alvo = bounds.center;
//                     alvo.y = Mathf.Lerp(bounds.min.y, bounds.max.y, alturaFocoCamera);
//                 }
//                 else
//                 {
//                     alvo = transform.position + Vector3.up * 1.4f;
//                 }

//                 Vector3 direcao = alvo - movimentoPlayer.transform.position;
//                 Quaternion rot = Quaternion.LookRotation(direcao);
//                 movimentoPlayer.transform.rotation = Quaternion.Euler(0, rot.eulerAngles.y, 0);

//                 Vector3 direcaoCamera = alvo - movimentoPlayer.playerCamera.position;
//                 float anguloX = Mathf.Asin(direcaoCamera.normalized.y) * Mathf.Rad2Deg;
//                 movimentoPlayer.playerCamera.localRotation = Quaternion.Euler(-anguloX, 0, 0);

//                 movimentoPlayer.SincronizarRotacaoInterna();
//             }

//             await VirarParaPlayer();
//             await IniciarInteracao();

//             interacaoConcluida = true;
//             MostrarAviso(false);
//         }
//         finally
//         {
//             interagindo = false;
//         }
//     }

//     private async Task VirarParaPlayer()
//     {
//         if (movimentoPlayer == null) return;

//         Vector3 direcao = movimentoPlayer.transform.position - transform.position;
//         direcao.y = 0; 

//         if (direcao != Vector3.zero)
//         {
//             Quaternion rotacaoInicial = transform.rotation;
//             Quaternion rotacaoFinal = Quaternion.LookRotation(direcao);

//             float tempo = 0f;
//             float duracao = 0.25f;

//             while (tempo < duracao)
//             {
//                 tempo += Time.deltaTime;
//                 transform.rotation = Quaternion.Slerp(rotacaoInicial, rotacaoFinal, tempo / duracao);
//                 await Task.Yield();
//             }
//         }
//     }

//     public async Task EntrarModoExercicio()
//     {
//         if (movimentoPlayer == null) return;

//         Transform cam = playerCamera;

//         npcPosicaoOriginal = transform.position;
//         npcRotacaoOriginal = transform.rotation;

//         cameraRotacaoOriginal = cam.localRotation;
//         cameraPosicaoOriginal = cam.position;

//         Vector3 frentePlana = cam.forward;
//         frentePlana.y = 0f;
//         frentePlana.Normalize();

//         Vector3 direitaPlana = cam.right;
//         direitaPlana.y = 0f;
//         direitaPlana.Normalize();

//         Vector3 posicaoAlvoNPC = cam.position + (frentePlana * 1.5f) + (-direitaPlana * 0.6f);
//         posicaoAlvoNPC.y = npcPosicaoOriginal.y; 

//         Vector3 direcaoOlhar = cam.position - posicaoAlvoNPC;
//         direcaoOlhar.y = 0;
//         Quaternion rotNPCFinal = Quaternion.LookRotation(direcaoOlhar);

//         Vector3 npcInicio = transform.position;
//         Quaternion rotNPCInicio = transform.rotation;

//         float t = 0f;
//         while (t < 1f)
//         {
//             t += Time.deltaTime * 2f; 
//             transform.position = Vector3.Lerp(npcInicio, posicaoAlvoNPC, t);
//             transform.rotation = Quaternion.Slerp(rotNPCInicio, rotNPCFinal, t);
//             await Task.Yield();
//         }

//         cam.localRotation = cameraRotacaoOriginal * Quaternion.Euler(0f, 8f, 0f);
//     }

//     public async Task SairModoExercicio()
//     {
//         if (movimentoPlayer == null) return;

//         Transform cam = playerCamera;
//         Vector3 npcInicio = transform.position;
//         Quaternion rotNPCInicio = transform.rotation;

//         float t = 0f;
//         while (t < 1f)
//         {
//             t += Time.deltaTime * 2f;
//             transform.position = Vector3.Lerp(npcInicio, npcPosicaoOriginal, t);
//             transform.rotation = Quaternion.Slerp(rotNPCInicio, npcRotacaoOriginal, t);
//             cam.localRotation = Quaternion.Slerp(cam.localRotation, cameraRotacaoOriginal, t);
//             await Task.Yield();
//         }

//         // ✨ COMANDO REMOVIDO DAQUI! 
//         // Quem controla a liberdade do player agora é o fluxo de diálogo de cada NPC.
//     }

//     // ✨ Métodos utilitários para controle manual externo
//     public void LiberarControlePlayer()
//     {
//         if (movimentoPlayer != null) 
//         {
//             movimentoPlayer.AtivarControle();
//         }

//         if (crosshair != null)
//             crosshair.SetActive(true); //pelo amor de deus funciona

//         Debug.Log("LiberarControlePlayer chamado");
//     }

//     public void TravarControlePlayer()
//     {
//         if (movimentoPlayer != null) movimentoPlayer.DesativarControle();
//     }

//     protected virtual Task IniciarInteracao()
//     {
//         return Task.CompletedTask;
//     }

//     protected static List<ExercicioBase> GarantirListaExercicios(List<ExercicioBase> exercicios)
//     {
//         return exercicios ?? new List<ExercicioBase>();
//     }

//     public virtual bool PodeInteragir()
//     {
//         return !interacaoConcluida && !interagindo;
//     }

//     public virtual async Task FalarFraseCustomizada(string textoParaFalar)
//     {
//         // Se a lógica de gerar áudio for idêntica para todos, ela pode ficar direto aqui!
//         await Task.CompletedTask;
//     }
// }

using UnityEngine;
using UnityEngine.XR;
using System.Threading.Tasks;
using System.Collections.Generic;
using TMPro;

public class InteracaoNPC : MonoBehaviour
{
    public GameObject avisoUI;
    public Transform playerCamera;
    public string nomeExibicao = "NPC";
    public FirstPlayerController movimentoPlayer;

    protected bool interacaoConcluida = false;

    private bool interagindo = false;
    private static InteracaoNPC iniciandoInteracao;
    public float alturaFocoCamera = 0.82f;
    public GameObject crosshair;

    [SerializeField] protected ModalExercicio modalExercicioComum;
    [SerializeField] protected GameObject modalLegendaComum;
    [SerializeField] protected TextMeshProUGUI textoPularDialogoComum;

    protected Vector3 npcPosicaoOriginal;
    protected Quaternion npcRotacaoOriginal;

    private Quaternion cameraRotacaoOriginal;
    public string ultimaFraseDita = "";
    private Vector3 cameraPosicaoOriginal;

    protected AudioSource audioSourceComum;
    protected BackendManager backendManagerComum;

    // Quando o runtime XR está ativo, a orientação da câmera pertence ao headset.
    protected bool ModoVR => XRSettings.isDeviceActive;

    protected async Task ApresentarFalaDoRoteiro(BackendManager backend, AudioSource fonte, string voz,
        string nome, string fala, GameObject legenda, TextMeshProUGUI avisoAvanco,
        bool mostrarLegenda = true, bool aguardarAvanco = true, System.Func<Task> avancar = null)
    {
        int visita = SessaoCenario.Atual.Versao;
        AudioClip clip = await CarregarAudioDaFala(backend, fala, voz, visita);
        ValidarVisita(visita);
        bool exibirLegenda = mostrarLegenda || clip == null;

        try
        {
            await AguardarConfiguracoes(visita);
            if (avisoAvanco != null)
                avisoAvanco.gameObject.SetActive(false);
            if (exibirLegenda && legenda != null)
                legenda.GetComponent<ModalLegenda>().MostrarLegenda(nome, fala);

            if (clip != null)
            {
                fonte.clip = clip;
                await ReproduzirVoz(fonte, visita);
            }

            if (aguardarAvanco || clip == null)
            {
                if (avisoAvanco != null)
                    avisoAvanco.gameObject.SetActive(true);
                DefinirCursorDesktop(true);
                await Task.Yield();
                ValidarVisita(visita);
                await (avancar == null ? AguardarAvancoDialogo() : avancar());
                ValidarVisita(visita);
            }
        }
        finally
        {
            if (clip != null)
            {
                if (fonte != null && fonte.clip == clip)
                {
                    fonte.Stop();
                    fonte.clip = null;
                }
                Destroy(clip);
            }
            if (this != null && SessaoCenario.Atual.Permite(Cenario, visita))
            {
                if (avisoAvanco != null)
                    avisoAvanco.gameObject.SetActive(false);
                if (exibirLegenda && legenda != null)
                    legenda.SetActive(false);
            }
        }
    }

    private async Task AguardarConfiguracoes(int visita)
    {
        while (BotaoConfiguracoes.DialogoSuspenso)
        {
            ValidarVisita(visita);
            await Awaitable.NextFrameAsync();
        }
        ValidarVisita(visita);
    }

    protected async Task ReproduzirVoz(AudioSource fonte, int visita)
    {
        await AguardarConfiguracoes(visita);
        fonte.Play();
        while (fonte.isPlaying)
        {
            ValidarVisita(visita);
            if (BotaoConfiguracoes.DialogoSuspenso)
            {
                AudioClip clip = fonte.clip;
                fonte.Pause();
                try
                {
                    await AguardarConfiguracoes(visita);
                }
                catch (System.OperationCanceledException)
                {
                    if (fonte != null && fonte.clip == clip)
                        fonte.Stop();
                    throw;
                }
                if (fonte == null || fonte.clip != clip)
                    return;
                fonte.UnPause();
            }
            await Awaitable.NextFrameAsync();
        }
        await AguardarConfiguracoes(visita);
    }

    private async Task<AudioClip> CarregarAudioDaFala(BackendManager backend, string fala, string voz, int visita)
    {
        string caminho = null;
        try
        {
            byte[] bytes = await backend.GerarAudio(fala, voz);
            ValidarVisita(visita);
            if (bytes == null || bytes.Length == 0)
                return null;

            caminho = System.IO.Path.Combine(Application.temporaryCachePath, System.Guid.NewGuid() + ".wav");
            System.IO.File.WriteAllBytes(caminho, bytes);
            using var requisicao = UnityEngine.Networking.UnityWebRequestMultimedia.GetAudioClip(
                new System.Uri(caminho).AbsoluteUri, AudioType.WAV);
            var operacao = requisicao.SendWebRequest();
            while (!operacao.isDone)
            {
                ValidarVisita(visita);
                await Task.Yield();
            }
            ValidarVisita(visita);
            if (requisicao.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
                return UnityEngine.Networking.DownloadHandlerAudioClip.GetContent(requisicao);
            Debug.LogWarning($"Áudio de {voz} indisponível: {requisicao.error}", this);
        }
        catch (System.Exception erro) when (erro is not System.OperationCanceledException)
        {
            Debug.LogWarning($"Áudio de {voz} indisponível: {erro.Message}", this);
        }
        finally
        {
            if (caminho != null)
            {
                try { System.IO.File.Delete(caminho); }
                catch (System.IO.IOException erro) { Debug.LogWarning(erro.Message, this); }
                catch (System.UnauthorizedAccessException erro) { Debug.LogWarning(erro.Message, this); }
            }
        }

        return null;
    }


    // Aguarda uma confirmação para avançar o diálogo:
    // PC: clique esquerdo do mouse.
    // VR: botão A (primaryButton) do controle direito.
    // Aguarda também a soltura para um único pressionamento não avançar várias falas.
    protected async Task AguardarAvancoDialogo()
    {
        int visita = SessaoCenario.Atual.Versao;
        await BotaoConfiguracoes.AguardarComandoDialogo(AvancoDialogoPressionado, () => ValidarVisita(visita));
    }

    protected bool AvancoDialogoPressionado()
    {
        if (!ModoVR)
            return Input.GetMouseButton(0);

        var controle = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        return controle.TryGetFeatureValue(CommonUsages.primaryButton, out bool pressionado) && pressionado;
    }

    // Centraliza o controle do cursor. No VR, a interface é controlada pelo raio XR.
    protected void DefinirCursorDesktop(bool desbloquear)
    {
        if (ModoVR)
            return;

        Cursor.lockState = desbloquear ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = desbloquear;
    }

    protected virtual void Start()
    {
        if (avisoUI != null)
            avisoUI.SetActive(false);

        audioSourceComum = GetComponent<AudioSource>();
        backendManagerComum = new BackendManager();
    }

    public virtual string nomeExibicaoLegenda
    {
        get => nomeExibicao;
        set => nomeExibicao = value;
    }

    public void MostrarAviso(bool value)
    {
        if (avisoUI != null)
            avisoUI.SetActive(value);
    }

    public async Task Interagir()
    {
        if (iniciandoInteracao != null || !PodeInteragir() || (GameProgress.EstaEmDialogo && !PodeIniciarComDialogo))
            return;

        int visita = SessaoCenario.Atual.Versao;
        iniciandoInteracao = this;
        interagindo = true;

        try
        {
            AoIniciarInteracao();
            MostrarAviso(false);

            if (movimentoPlayer != null)
            {
                movimentoPlayer.DesativarControle();

                // No desktop, podemos girar o jogador e inclinar a câmera para focar o NPC.
                // Em VR, não alteramos a rotação do XR Origin nem da câmera:
                // a orientação da cabeça deve continuar sob controle do headset.
                if (!ModoVR)
                    FocarNPCNoDesktop();
            }

            await VirarParaPlayer();
            if (this == null || !SessaoCenario.Atual.Permite(Cenario, visita))
                return;

            iniciandoInteracao = null;
            await IniciarInteracao();

            if (this != null && SessaoCenario.Atual.Corresponde(visita))
            {
                interacaoConcluida = true;
                MostrarAviso(false);
            }
        }
        catch (System.OperationCanceledException)
        {
            if (this != null && SessaoCenario.Atual.Permite(Cenario, visita))
            {
                GameProgress.EstaEmDialogo = false;
                LiberarControlePlayer();
            }
        }
        finally
        {
            if (iniciandoInteracao == this)
                iniciandoInteracao = null;
            interagindo = false;
        }
    }

    private void FocarNPCNoDesktop()
    {
        if (movimentoPlayer == null || movimentoPlayer.playerCamera == null)
            return;

        Vector3 alvo = ObterPontoDeFoco();

        Vector3 direcao = alvo - movimentoPlayer.transform.position;
        if (direcao.sqrMagnitude > 0.0001f)
        {
            Quaternion rot = Quaternion.LookRotation(direcao);
            movimentoPlayer.transform.rotation = Quaternion.Euler(0f, rot.eulerAngles.y, 0f);
        }

        Vector3 direcaoCamera = alvo - movimentoPlayer.playerCamera.position;
        if (direcaoCamera.sqrMagnitude > 0.0001f)
        {
            float anguloX = Mathf.Asin(direcaoCamera.normalized.y) * Mathf.Rad2Deg;
            movimentoPlayer.playerCamera.localRotation = Quaternion.Euler(-anguloX, 0f, 0f);
            movimentoPlayer.SincronizarRotacaoInterna();
        }
    }

    private Vector3 ObterPontoDeFoco()
    {
        Renderer r = GetComponentInChildren<Renderer>();

        if (r != null)
        {
            Bounds bounds = r.bounds;
            Vector3 alvo = bounds.center;
            alvo.y = Mathf.Lerp(bounds.min.y, bounds.max.y, alturaFocoCamera);
            return alvo;
        }

        return transform.position + Vector3.up * 1.4f;
    }

    private async Task VirarParaPlayer()
    {
        if (movimentoPlayer == null)
            return;

        Vector3 direcao = movimentoPlayer.transform.position - transform.position;
        direcao.y = 0f;

        if (direcao.sqrMagnitude > 0.0001f)
        {
            Quaternion rotacaoInicial = transform.rotation;
            Quaternion rotacaoFinal = Quaternion.LookRotation(direcao);

            float tempo = 0f;
            const float duracao = 0.25f;

            while (tempo < duracao)
            {
                tempo += Time.deltaTime;
                transform.rotation = Quaternion.Slerp(rotacaoInicial, rotacaoFinal, tempo / duracao);
                await Task.Yield();
            }

            transform.rotation = rotacaoFinal;
        }
    }

    public async Task EntrarModoExercicio(TipoExercicio tipo)
    {
        if (tipo == TipoExercicio.Associacao || movimentoPlayer == null || playerCamera == null)
            return;

        Transform cam = playerCamera;

        npcPosicaoOriginal = transform.position;
        npcRotacaoOriginal = transform.rotation;

        cameraRotacaoOriginal = cam.localRotation;
        cameraPosicaoOriginal = cam.position;

        Vector3 frentePlana = cam.forward;
        frentePlana.y = 0f;

        if (frentePlana.sqrMagnitude < 0.0001f)
            frentePlana = movimentoPlayer.transform.forward;

        frentePlana.Normalize();

        Vector3 direitaPlana = cam.right;
        direitaPlana.y = 0f;

        if (direitaPlana.sqrMagnitude < 0.0001f)
            direitaPlana = movimentoPlayer.transform.right;

        direitaPlana.Normalize();

        Vector3 posicaoAlvoNPC = cam.position + (frentePlana * 1.5f) + (-direitaPlana * 0.6f);
        posicaoAlvoNPC.y = npcPosicaoOriginal.y;

        Vector3 direcaoOlhar = cam.position - posicaoAlvoNPC;
        direcaoOlhar.y = 0f;

        if (direcaoOlhar.sqrMagnitude < 0.0001f)
            direcaoOlhar = -frentePlana;

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

        transform.position = posicaoAlvoNPC;
        transform.rotation = rotNPCFinal;

        // No desktop, mantém o pequeno ajuste de enquadramento já existente.
        // Em VR, nunca modifica a rotação local da câmera/headset.
        if (!ModoVR)
            cam.localRotation = cameraRotacaoOriginal * Quaternion.Euler(0f, 8f, 0f);
    }

    public async Task SairModoExercicio(TipoExercicio tipo)
    {
        if (tipo == TipoExercicio.Associacao || movimentoPlayer == null)
            return;

        Transform cam = playerCamera;

        Vector3 npcInicio = transform.position;
        Quaternion rotNPCInicio = transform.rotation;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            transform.position = Vector3.Lerp(npcInicio, npcPosicaoOriginal, t);
            transform.rotation = Quaternion.Slerp(rotNPCInicio, npcRotacaoOriginal, t);

            // Só restauramos a câmera no desktop. No VR, o headset é a fonte da orientação.
            if (!ModoVR && cam != null)
                cam.localRotation = Quaternion.Slerp(cam.localRotation, cameraRotacaoOriginal, t);

            await Task.Yield();
        }

        transform.position = npcPosicaoOriginal;
        transform.rotation = npcRotacaoOriginal;
    }

    public void LiberarControlePlayer()
    {
        if (movimentoPlayer != null)
            movimentoPlayer.AtivarControle();

        if (crosshair != null)
            crosshair.SetActive(true);

        Debug.Log("LiberarControlePlayer chamado");
    }

    public void TravarControlePlayer()
    {
        if (movimentoPlayer != null)
            movimentoPlayer.DesativarControle();
    }

    protected virtual void AoIniciarInteracao() { }

    protected virtual Task IniciarInteracao()
    {
        return Task.CompletedTask;
    }

    protected static List<ExercicioBase> GarantirListaExercicios(List<ExercicioBase> exercicios)
    {
        return exercicios ?? new List<ExercicioBase>();
    }

    internal string Cenario => SessaoCenario.CenarioDoNpc(CheckpointNpcKey);

    internal string CheckpointNpcKey => this switch
    {
        ShelfNPC _ => "shelf_npc", BakeryNPC _ => "bakery_npc", CashierNPC _ => "cashier_npc",
        AirportInfoNPC _ => "airport_info_npc", checkinNPC _ => "checkin_npc",
        ClothingSellerNPC _ => "clothing_seller_npc", RecepcionistHotelNPC _ => "recepcionist_hotel_npc",
        motherNPC _ => "mother_npc", fatherNPC _ => "father_npc", brotherNPC _ => "brother_npc",
        VisitorCenterNPC _ => "visitor_center_npc", RestaurantWaiterNPC _ => "restaurant_waiter_npc",
        RestaurantClient _ => "restaurant_client", RecepcionistCollegeNPC _ => "recepcionist_college_npc",
        SecretaryCollegeNPC _ => "secretary_college_npc", _ => null
    };

    private int visitaInteracao;
    internal bool PertenceAoCenarioAtivo => SessaoCenario.Atual.Permite(Cenario);

    internal void MarcarAtendimentoRestaurado(int visita)
    {
        ValidarVisita(visita);
        AtualizarVisita();
        interacaoConcluida = true;
    }

    internal void ValidarRoteiroCheckpoint(int visita) => ValidarVisita(visita);

    internal async Task ExecutarRoteiroRetomado(int visita, System.Func<Task> executar)
    {
        ValidarVisita(visita);
        if (interagindo)
            throw new System.InvalidOperationException("O NPC já está executando outra interação.");
        interagindo = true;
        MostrarAviso(false);
        try { await executar(); }
        finally
        {
            interagindo = false;
            if (this != null && SessaoCenario.Atual.Permite(Cenario, visita))
                interacaoConcluida = true;
        }
    }

    protected void ValidarVisita(int visita)
    {
        if (this == null || !SessaoCenario.Atual.Permite(Cenario, visita))
            throw new System.OperationCanceledException("A interação pertence a uma visita encerrada.");
    }

    protected virtual void ReiniciarVisita() { }
    protected virtual bool PodeIniciarComDialogo => false;

    public virtual bool PodeInteragir()
    {
        if (!PertenceAoCenarioAtivo || !isActiveAndEnabled)
            return false;

        AtualizarVisita();
        return (!interacaoConcluida || PermitirReinteracaoAposConclusao) && !interagindo;
    }

    protected internal void AtualizarVisita()
    {
        int visita = SessaoCenario.Atual.Versao;
        if (visitaInteracao != visita && !interagindo)
        {
            visitaInteracao = visita;
            interacaoConcluida = false;
            ReiniciarVisita();
        }
    }

    protected virtual bool PermitirReinteracaoAposConclusao => false;

    public virtual async Task FalarFraseCustomizada(string textoParaFalar)
    {
        await Task.CompletedTask;
    }
}
