using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Net.Http;
using UnityEngine.Networking;
using System.IO;
using TMPro;
using UnityEngine.AI; // ✨ Essencial para o NavMeshAgent

public class ShelfNPC : InteracaoNPC
{
    List<string> dialogoAtual;
    string nivelAtual;
    private Animator animator;

    private AudioSource audioSource;
    public ModalExercicio modalExercicio;

    public NavMeshAgent agent; 
    public float distanciaParaRetomarConversa = 2.5f; 
    public List<Transform> pontosPrateleiraA1;
    //ok
    public List<Transform> pontosPrateleiraA2;
    public List<Transform> pontosPrateleiraB1;

    // Contadores para saber em qual parada o NPC está em cada nível
    private int indiceDestinoA1 = 0;
    private int indiceDestinoA2 = 0;
    private int indiceDestinoB1 = 0;

    private Transform pontoDestinoEscolhido;
    private bool podeInteragirCenario = true;
    public TextMeshProUGUI textoPularDialogo;

    public string idNpcParaVoz = "shelf"; 
    private BackendManager backendManager;
    private MissionManager missaoManager; 

    public List<ExercicioBase> exerciciosBlocos;
    public List<ExercicioBase> exerciciosSpeaking;
    public List<ExercicioBase> exerciciosAlternativas;
    public ExercicioBase exAtual;

    public GameObject modalLegenda, modalMissoes;

    protected override void Start()
    {
        base.Start();
        nomeExibicaoLegenda = "Shelf Attendant";
        backendManager = new BackendManager();
        missaoManager = MissionManager.Instance; 
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        
        exerciciosBlocos = new List<ExercicioBase>();
        exerciciosSpeaking = new List<ExercicioBase>();
        exerciciosAlternativas = new List<ExercicioBase>();

        if (agent == null) 
        {
            agent = GetComponent<NavMeshAgent>();
        }
    }

    void Update()
    {
        if (agent != null && animator != null)
        {
            bool estaAndando = agent.hasPath && agent.remainingDistance > agent.stoppingDistance + 0.05f &&
            agent.velocity.magnitude > 0.05f;

            animator.SetBool("IsWalking", estaAndando);
        }
    }

    // Sobrescreve a checagem de interação global para travar o cenário se o jogador estiver seguindo o NPC
    public override bool PodeInteragir()
    {
        return base.PodeInteragir() && podeInteragirCenario;
    }

    protected override async Task IniciarInteracao()
    {
        GameProgress.EstaEmDialogo = true; // ✨ Trava tudo aqui
        podeInteragirCenario = false;
        
        nivelAtual = DadosJogador.nivelUsuario;
        Debug.Log("Nível do jogador capturado na Interação: " + nivelAtual);

        InicializarConteudosPorNivel();

        if (nivelAtual == "A1")
        {
            await interacaoA1();
        }
        else if (nivelAtual == "A2")
        {
            await interacaoA2();
        }
        else
        {
            await interacaoB1();
        }
        
        MissionManager.Instance.ConcluirMissao("falar_atendente");

        GameProgress.Instance.falouAtendente = true;
        GameProgress.EstaEmDialogo = false; // ✨ Só destrava aqui no final de TUDO
        podeInteragirCenario = true;
    
        if (modalLegenda != null)
            modalLegenda.SetActive(false);

        if (textoPularDialogo != null)
            textoPularDialogo.gameObject.SetActive(false);

        GameProgress.EstaEmDialogo = false;

        LiberarControlePlayer();
    }

    private async Task interacaoA1()
    {
        int i = 0;
        await PlayAudioETexto(i++); // "Hi. Are you looking for anything today?"

        exAtual = exerciciosBlocos[0];
        await AbrirExercicio(TipoExercicio.Blocos, exAtual);

        await PlayAudioETexto(i++); // "Follow me."

        crosshair.SetActive(true); //Esconde a mira durante o "siga-me" para evitar distrações
        await FluxoSeguirNPCAtelarPrateleira();
        crosshair.SetActive(false); 

        await PlayAudioETexto(i++); // "The milk is here. Do you need anything else?"

        exAtual = exerciciosSpeaking[0];
        await AbrirExercicio(TipoExercicio.Speaking, exAtual);

        await PlayAudioETexto(i++); // "Ok, I will show you"
        crosshair.SetActive(true); // ✨ Esconde a mira durante o "siga-me" para evitar distrações
        await FluxoSeguirNPCAtelarPrateleira();
        crosshair.SetActive(false); // ✨ Volta a mostrar a mira após o "siga-me"
        await PlayAudioETexto(i++); // "The fruits are here. Can I do anything else for you?"
        await AbrirExercicio(TipoExercicio.Blocos, exerciciosBlocos[1]);

        await PlayAudioETexto(i++); // "Yes, we have apples too."
        await AbrirExercicio(TipoExercicio.Alternativas, exerciciosAlternativas[0]);

        missaoManager.AdicionarMissao(
            "interagir_produtos",
            "Get the milk and the fruit from the shelves",
            "Pegue o leite e a fruta das prateleiras"
        );

        ProdutoManager.Instance.LiberarProdutos(nivelAtual);
    }

    private async Task interacaoA2() 
    {
        int i = 0;
        await PlayAudioETexto(i++); // "Hi! What are you looking for..."

        exAtual = exerciciosBlocos[0];
        await AbrirExercicio(TipoExercicio.Blocos, exAtual);

        await PlayAudioETexto(i++); // "Sure. Follow me, please."

        crosshair.SetActive(true); // ✨ Esconde a mira durante o "siga-me" para evitar distrações
        await FluxoSeguirNPCAtelarPrateleira();
        crosshair.SetActive(false); 

        await PlayAudioETexto(i++); // "The milk is on this shelf..."

        exAtual = exerciciosSpeaking[0];
        await AbrirExercicio(TipoExercicio.Speaking, exAtual);

        await AbrirExercicio(TipoExercicio.Blocos, exerciciosBlocos[1]);

        await PlayAudioETexto(i++); // "Do you often buy snacks from this aisle..."
        await AbrirExercicio(TipoExercicio.Speaking, exerciciosSpeaking[1]);

        crosshair.SetActive(true); // ✨ Esconde a mira durante o "siga-me" para evitar distrações
        await FluxoSeguirNPCAtelarPrateleira();
        crosshair.SetActive(false); 
        await PlayAudioETexto(i++); // "I see. Is there anything else you need?"
        await AbrirExercicio(TipoExercicio.Alternativas, exerciciosAlternativas[0]);

        missaoManager.AdicionarMissao(
            "interagir_produtos",
            "Get the milk, the cereal and the snack from the shelves",
            "Pegue o leite, o cereal e o salgadinho das prateleiras"
        );

        ProdutoManager.Instance.LiberarProdutos(nivelAtual);
    }

    private async Task interacaoB1()
    {
        int i = 0;
        await PlayAudioETexto(i++); // "Hi! What are you searching for..."

        exAtual = exerciciosSpeaking[0];
        await AbrirExercicio(TipoExercicio.Speaking, exAtual);

        await PlayAudioETexto(i++); // "Yes, there are still some discounts..."
        await AbrirExercicio(TipoExercicio.Speaking, exerciciosSpeaking[1]);

        crosshair.SetActive(true); // ✨ Esconde a mira durante o "siga-me" para evitar distrações
        await FluxoSeguirNPCAtelarPrateleira();
        crosshair.SetActive(false); 

        await PlayAudioETexto(i++); // "The soda is on this shelf..."

        exAtual = exerciciosBlocos[0]; 
        await AbrirExercicio(TipoExercicio.Blocos, exAtual);
        await PlayAudioETexto(i++); // "Enjoy the discounts. Happy shopping!"

        missaoManager.AdicionarMissao(
            "interagir_produtos",
            "Get the vegetable and the soda from the shelves",
            "Pegue o legumes e o refrigerante das prateleiras"
        );

        ProdutoManager.Instance.LiberarProdutos(nivelAtual);
    }

    private async Task FluxoSeguirNPCAtelarPrateleira()
    {
        if (agent != null)
        {
            switch (nivelAtual)
        {
            case "A1":
                if (pontosPrateleiraA1 != null && indiceDestinoA1 < pontosPrateleiraA1.Count)
                {
                    pontoDestinoEscolhido = pontosPrateleiraA1[indiceDestinoA1];
                    indiceDestinoA1++; // Avança para o próximo ponto na próxima vez que chamar
                }
                break;

            case "A2":
                if (pontosPrateleiraA2 != null && indiceDestinoA2 < pontosPrateleiraA2.Count)
                {
                    pontoDestinoEscolhido = pontosPrateleiraA2[indiceDestinoA2];
                    indiceDestinoA2++;
                }
                break;

            case "B1":
                if (pontosPrateleiraB1 != null && indiceDestinoB1 < pontosPrateleiraB1.Count)
                {
                    pontoDestinoEscolhido = pontosPrateleiraB1[indiceDestinoB1];
                    indiceDestinoB1++;
                }
                break;

            default:
                break;
        }

        // Validação de segurança
        if (pontoDestinoEscolhido == null)
        {
            Debug.LogError($"Não há um ponto de destino válido configurado para o nível {nivelAtual} no índice atual!");
            return;
        }

        missaoManager.ResetarMissao("seguir_atendente");

        missaoManager.AdicionarMissao(
            "seguir_atendente",
            "Follow the attendant to get the products",
            "Siga o atendente para pegar os produtos"
        );

        if (modalLegenda != null) modalLegenda.SetActive(false);

        LiberarControlePlayer();

        // Faz o NPC andar até o destino atual da sequência
        agent.SetDestination(pontoDestinoEscolhido.position); 

        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            await Task.Yield();
        }

        agent.ResetPath();

        if (animator != null)
        {
            animator.SetBool("IsWalking", false);
        }

        // Atualiza a posição base para o modo exercício funcionar no lugar certo
        npcPosicaoOriginal = pontoDestinoEscolhido.position;
        npcRotacaoOriginal = pontoDestinoEscolhido.rotation;
        transform.position = pontoDestinoEscolhido.position;
        transform.rotation = pontoDestinoEscolhido.rotation;

        Debug.Log($"NPC chegou na parada atual do nível {nivelAtual}. Monitorando player...");

        bool jogadorPerto = false;
        while (!jogadorPerto)
        {
            if (movimentoPlayer != null)
            {
                float distancia = Vector3.Distance(movimentoPlayer.transform.position, transform.position);
                if (distancia <= distanciaParaRetomarConversa)
                {
                    jogadorPerto = true;
                }
            }
            await Task.Yield();
        }

        TravarControlePlayer();

        // =========================================================================
        // ✨ CORREÇÃO INTEGRAL DE ENQUADRAMENTO (IGUAL AO SISTEMA DO BOTÃO F)
        // =========================================================================
        if (movimentoPlayer != null)
        {
            // 1. Descobre onde está o centro do NPC para mirar a câmera
            Vector3 alvoFoco;
            Renderer r = GetComponentInChildren<Renderer>();
            if (r != null)
            {
                Bounds bounds = r.bounds;
                alvoFoco = bounds.center;
                alvoFoco.y = Mathf.Lerp(bounds.min.y, bounds.max.y, alturaFocoCamera);
            }
            else
            {
                alvoFoco = transform.position + Vector3.up * 1.4f;
            }

            // 2. Força o corpo do Jogador a virar de frente para o NPC
            Vector3 direcaoCorpoPlayer = alvoFoco - movimentoPlayer.transform.position;
            Quaternion rotacaoCorpoAlvo = Quaternion.LookRotation(direcaoCorpoPlayer);
            movimentoPlayer.transform.rotation = Quaternion.Euler(0, rotacaoCorpoAlvo.eulerAngles.y, 0);

            // 3. Força a Câmera do Jogador a olhar verticalmente para o rosto do NPC
            Vector3 direcaoCameraPlayer = alvoFoco - movimentoPlayer.playerCamera.position;
            float anguloX = Mathf.Asin(direcaoCameraPlayer.normalized.y) * Mathf.Rad2Deg;
            movimentoPlayer.playerCamera.localRotation = Quaternion.Euler(-anguloX, 0, 0);

            // 4. Avisa o controlador interno do FirstPerson para não resetar a câmera ao mover o mouse
            movimentoPlayer.SincronizarRotacaoInterna();
        }

        // 5. Faz o NPC também virar de frente para o Jogador suavemente
        Vector3 direcaoOlharNpc = movimentoPlayer.transform.position - transform.position;
        direcaoOlharNpc.y = 0;
        if (direcaoOlharNpc != Vector3.zero)
        {
            Quaternion rotacaoAlvoNpc = Quaternion.LookRotation(direcaoOlharNpc);
            float tempoRotacao = 0f;
            while (tempoRotacao < 0.25f) // Slerp suave de 0.25 segundos
            {
                tempoRotacao += Time.deltaTime;
                transform.rotation = Quaternion.Slerp(transform.rotation, rotacaoAlvoNpc, tempoRotacao / 0.25f);
                await Task.Yield();
            }
        }
        // Se o player chegou perto clicando ou segurando o mouse, espera ele soltar.
        while (Input.GetMouseButton(0) || Input.GetMouseButtonDown(0))
        {
            await Task.Yield();
        }

            missaoManager.ConcluirMissao("seguir_atendente");
            if (modalLegenda != null) 
                modalLegenda.SetActive(false);
        } 
    }

    private void InicializarConteudosPorNivel()
    {
        exerciciosBlocos.Clear();
        exerciciosSpeaking.Clear();
        exerciciosAlternativas.Clear();

        if (nivelAtual == "A1")
        {
            dialogoAtual = new List<string>()
            {
                "Hi. Are you looking for anything today?",
                "Follow me.",
                "The milk is here. Do you need anything else?",
                "Ok, I will show you",
                "The fruits are here. Can I do anything else for you?",
                "Yes, we have apples too."
            };

            exerciciosBlocos.Add(new ExercicioBlocos() 
            { 
                enunciado = "Arrange the words to form the sentence in english: Onde está o leite?",
                blocosPalavras = new List<string>() { "Where", "milk", "thank", "you", "that", "is", "all", "hello", "bread", "the" },
                respostaCorreta = "Where is the milk?",
            });

            exerciciosBlocos.Add(new ExercicioBlocos() 
            { 
                enunciado = "Arrange the words to form the sentence in english: Há maçãs hoje?",
                blocosPalavras = new List<string>() { "Where", "Are", "thank", "apples", "that", "is", "any", "hello", "bread", "there", "today" },
                respostaCorreta = "Are there any apples today?",
            });

            exerciciosSpeaking.Add(new ExercicioSpeaking()
            {
                enunciado = "Say the right sentence:",
                opcoesFala = new List<string>() { "Yes, I need some fruits.\n", "I am cleaning the house.\n", "There are some books.\r\n" },
                respostaCorreta = 0
            });

            exerciciosAlternativas.Add(new ExercicioAlternativas()
            {
                enunciado = "Answer choosing the right sentence:",
                alternativas = new List<string>() { "She is my teacher.\r\n", "They goes to school.\r\n", "That is all I needed, thank you\r\n", "I like to play soccer.\r\n" },
                alternativaCorreta = 2
            });
        }
        else if (nivelAtual == "A2")
        {
           dialogoAtual = new List<string>()
           {
                "Hi! What are you looking for in the supermarket today?",
                "Sure. Follow me, please.",
                "The milk is on this shelf, and the cereal is over there. Do you need anything else?",
                "There are special discounts today.Do you often buy snacks from this aisle, or do you prefer the one near the bakery?",
                "The snacks are here. Can I help you find anything else, or do you already have everything?"
           };

            exerciciosBlocos.Add(new ExercicioBlocos()
            {
                enunciado = "Ask where the products Milk and Cereal are with the words to form a correct sentence:",
                blocosPalavras = new List<string>() { "Where", "milk", "thank", "you", "that", "is", "all", "hello", "bread", "the", "cereal", "ok", "and" },
                respostaCorreta = "Where is the milk and cereal?"
            });

            exerciciosSpeaking.Add(new ExercicioSpeaking()
            {
                enunciado = "Say the right sentence from the list:",
                opcoesFala = new List<string>() { "Yes, I want some snacks too.\n", "Yes, I went to the park yesterday.\n", "No, the computer is very fast.\r\n" },
                respostaCorreta = 0
            });

            exerciciosBlocos.Add(new ExercicioBlocos()
            {
                enunciado = "Arrange the words to form the sentence in english: Há descontos especiais hoje?",
                blocosPalavras = new List<string>() { "Where", "thank", "you", "there", "Are", "all", "hello", "speacial", "the", "cereal", "any", "and", "discounts", "bread", "today" },
                respostaCorreta = "Are there any special discounts today?"
            });

            exerciciosAlternativas.Add(new ExercicioAlternativas()
            {
                enunciado = "Answer choosing the right sentence:",
                alternativas = new List<string>() { "The cat is sleeping under the table.\r\n", "My brother studies at the university.\r\n", "No, thank you. I have everything I need.\r\n", "We usually eat dinner at 7 p.m.\r\n" },
                alternativaCorreta = 2
            });
        }
        else // B1
        {
            dialogoAtual = new List<string>()
            { 
                "Hi! What are you searching for today, and would you like me to show you where the products are?",
                "Yes, there are still some discounts available on pizzas and vegetables.",
                "Of course, follow me and I'll show you where they are.",
                "The soda is on this shelf and the vegetables are right in front of it. Can I help you find anything else, or have you already picked up everything you needed today?",
                "Enjoy the discounts. Happy shopping!"
            };

            exerciciosSpeaking.Add(new ExercicioSpeaking()
            {
                enunciado = "How should you respond? Say the right answer:",
                opcoesFala = new List<string>() { "My cousin usually travels by train during the holidays.\n", "The parking lot was completely full after lunchtime.\n", "Are there any special discounts in the frozen food section today, or has the promotion already ended?\r\n" },
                respostaCorreta = 2
            });

            exerciciosSpeaking.Add(new ExercicioSpeaking()
            {
                enunciado = "How should you respond? Say the right answer:",
                opcoesFala = new List<string>() { "Nice. I’m searching for some healthy food and bottled soda too.\n", "There is a pharmacy next to the supermarket entrance. He doesn’t enjoy waking up early either.\n", "He doesn’t enjoy waking up early either.\r\n" },
                respostaCorreta = 0
            });

            exerciciosBlocos.Add(new ExercicioBlocos() 
            { 
                enunciado = "Arrange the words to form the sentence in english: Isso era tudo que eu precisava. Se eu precisar de qualquer coisa eu aviso. Obrigado/a.",
                blocosPalavras = new List<string>() { "No", "milk", "I", "That", "else", "you", "you", "know", "needed", "was", "same", "all", "hello", "need", "Thank", "If", "I'll", "maybe", "ok", "perfect", "so", "I", "let", "anything" },
                respostaCorreta = "That was all I needed. If I need anything else, I'll let you know. Thank you.",
            });
        }
    }

    private async Task PlayAudioETexto(int i, bool mostrarLegenda = true)
    {
        ultimaFraseDita = dialogoAtual[i];
        byte[] audioBytes = await backendManager.GerarAudio(dialogoAtual[i], idNpcParaVoz);
        
        if (audioBytes != null && audioBytes.Length > 0)
        {
            Debug.Log($"Áudio recebido! Tamanho: {audioBytes.Length} bytes");
            string caminho = Path.Combine(Application.persistentDataPath, "audio_temp.wav");
            File.WriteAllBytes(caminho, audioBytes);
            
            UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + caminho, AudioType.WAV);
            var operation = www.SendWebRequest();
            
            while (!operation.isDone)
                await Task.Yield();

            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
            audioSource.clip = clip;

            if (mostrarLegenda)
            {
                textoPularDialogo.gameObject.SetActive(false);
                modalLegenda.GetComponent<ModalLegenda>().MostrarLegenda(nomeExibicaoLegenda, dialogoAtual[i]);
            }

            audioSource.Play();
            while (audioSource.isPlaying)
            {
                await Task.Yield();
            }

                if (textoPularDialogo != null)
                {
                    textoPularDialogo.gameObject.SetActive(true);

                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }

            // ✨ ESPERA SEGURA: Aguarda 150ms antes de começar a ouvir o clique.
            // Isso evita que o clique que fechou o modal ou moveu o player passe para cá.
            await Task.Delay(150);

            // ✨ Garante que o jogador soltou o botão antes de aceitar um novo clique
            while (Input.GetMouseButton(0))
            {
                await Task.Yield();
            }

            bool clicou = false;
            while (!clicou)
            {
                if (Input.GetMouseButtonDown(0)) 
                {
                    clicou = true; 
                }
                else
                {
                    await Task.Yield(); 
                }
            }

            if (textoPularDialogo != null)
            {
                textoPularDialogo.gameObject.SetActive(false);
            }

            if (modalLegenda != null)
            {
                modalLegenda.SetActive(false);
            }
        }
        else
        {
            Debug.LogError("O servidor retornou um array de bytes vazio.");
            if (mostrarLegenda)
            {
                modalLegenda.GetComponent<ModalLegenda>().MostrarLegenda(nomeExibicaoLegenda, dialogoAtual[i]);
            }
        }
    }

    public override async Task FalarFraseCustomizada(string textoParaFalar)
    {
        if (string.IsNullOrEmpty(textoParaFalar)) return;

        byte[] audioBytes = await backendManager.GerarAudio(textoParaFalar, idNpcParaVoz);
        
        if (audioBytes != null && audioBytes.Length > 0)
        {
            string caminho = Path.Combine(Application.persistentDataPath, "audio_temp_custom.wav");
            File.WriteAllBytes(caminho, audioBytes);
            
            UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + caminho, AudioType.WAV);
            var operation = www.SendWebRequest();
            
            while (!operation.isDone)
                await Task.Yield();

            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
            audioSource.clip = clip;
            if (modalLegenda != null && modalLegenda.activeSelf)
            {
                if (textoPularDialogo != null) textoPularDialogo.gameObject.SetActive(false);
                modalLegenda.GetComponent<ModalLegenda>().MostrarLegenda(nomeExibicaoLegenda, textoParaFalar);
            }
            
            audioSource.Play();
            
            while (audioSource.isPlaying)
            {
                await Task.Yield();
            }
        }
    }
    public async Task AbrirExercicio(TipoExercicio tipo, ExercicioBase ex)
    {
        Debug.Log("Abrindo exercício: " + tipo);

        ModalLegenda legenda = modalLegenda.GetComponent<ModalLegenda>();

        if (modalLegenda != null)
        {
            modalLegenda.SetActive(false);
        }

        await EntrarModoExercicio();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        modalExercicio.Abrir(tipo, ex, this);

        if (legenda != null) 
        {
            legenda.AjustarPosicaoPeloEstadoDoJogo(); 
        }

        while (!modalExercicio.exercicioFinalizado)
        {
            await Task.Yield();
        }

        if (modalLegenda != null)
        {
            modalLegenda.SetActive(false);
        }

        await SairModoExercicio();

        // ✨ Volta a prender o mouse após terminar o exercício (se o diálogo geral não tiver acabado)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (legenda != null)
        {
            legenda.AjustarPosicaoPeloEstadoDoJogo();
        }
    }
}