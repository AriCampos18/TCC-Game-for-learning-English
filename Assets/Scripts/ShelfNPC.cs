using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System;
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

    [System.NonSerialized] public List<ExercicioBase> exerciciosCompletarFrase;
    [System.NonSerialized] public List<ExercicioBase> exerciciosCriarFrases;
    [System.NonSerialized] public List<ExercicioBase> exerciciosSpeaking;
    [System.NonSerialized] public List<ExercicioBase> exerciciosAlternativas;
    [System.NonSerialized] public List<ExercicioBase> exerciciosWriting;
    [System.NonSerialized] public List<ExercicioBase> exerciciosAssociacao;
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
        
        exerciciosCompletarFrase = new List<ExercicioBase>();
        exerciciosCriarFrases = new List<ExercicioBase>();
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

        InicializarConteudosPorNivel();

        if (nivelAtual == "A1")
        {
            await interacaoA1();
        }
        else if (nivelAtual == "A2")
        {
            await interacaoA2();
        }
        else if (nivelAtual == "A1-A2")
        {
            await interacaoA1A2();
        }
        else if (nivelAtual == "A2-B1")
        {
            await interacaoA2B1();
        }
        else if(nivelAtual == "B1")
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

        exAtual = exerciciosCriarFrases[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++); // "Follow me."

        crosshair.SetActive(true); //Esconde a mira durante o "siga-me" para evitar distrações
        await FluxoSeguirNPCAtelarPrateleira();
        crosshair.SetActive(false); 

        await PlayAudioETexto(i++); // "The milk is here. Do you need anything else?"

        exAtual = exerciciosSpeaking[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++); // "Ok, I will show you"
        crosshair.SetActive(true); // ✨ Esconde a mira durante o "siga-me" para evitar distrações
        await FluxoSeguirNPCAtelarPrateleira();
        crosshair.SetActive(false); // ✨ Volta a mostrar a mira após o "siga-me"
        await PlayAudioETexto(i++); // "The fruits are here. Can I do anything else for you?"
        await AbrirExercicio(ObterTipoExercicio(exerciciosWriting[0]), exerciciosWriting[0]);

        await PlayAudioETexto(i++); // "Yes, we have apples too."
        await AbrirExercicio(ObterTipoExercicio(exerciciosAlternativas[0]), exerciciosAlternativas[0]);

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

        exAtual = exerciciosCriarFrases[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

        await PlayAudioETexto(i++); // "Sure. Follow me, please."

        crosshair.SetActive(true); // ✨ Esconde a mira durante o "siga-me" para evitar distrações
        await FluxoSeguirNPCAtelarPrateleira();
        crosshair.SetActive(false); 

        await PlayAudioETexto(i++); // "The milk is on this shelf..."

        exAtual = exerciciosSpeaking[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

        await AbrirExercicio(ObterTipoExercicio(exerciciosCriarFrases[1]), exerciciosCriarFrases[1]); //

        await PlayAudioETexto(i++); // "Do you often buy snacks from this aisle..."
        await AbrirExercicio(ObterTipoExercicio(exerciciosSpeaking[1]), exerciciosSpeaking[1]);

        crosshair.SetActive(true); // ✨ Esconde a mira durante o "siga-me" para evitar distrações
        await FluxoSeguirNPCAtelarPrateleira();
        crosshair.SetActive(false); 
        await PlayAudioETexto(i++); // "I see. Is there anything else you need?"
        await AbrirExercicio(ObterTipoExercicio(exerciciosAlternativas[0]), exerciciosAlternativas[0]);

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
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++); // "Yes, there are still some discounts..."
        await AbrirExercicio(ObterTipoExercicio(exerciciosSpeaking[1]), exerciciosSpeaking[1]); //

        await PlayAudioETexto(i++);

        crosshair.SetActive(true); // ✨ Esconde a mira durante o "siga-me" para evitar distrações
        await FluxoSeguirNPCAtelarPrateleira();
        crosshair.SetActive(false); 

        await PlayAudioETexto(i++); // "The soda is on this shelf..."

        exAtual = exerciciosCriarFrases[0]; 
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);
        await PlayAudioETexto(i++); // "Enjoy the discounts. Happy shopping!"

        missaoManager.AdicionarMissao(
            "interagir_produtos",
            "Get the vegetable and the soda from the shelves",
            "Pegue o legumes e o refrigerante das prateleiras"
        );

        ProdutoManager.Instance.LiberarProdutos(nivelAtual);
    }

    private async Task interacaoA1A2()
{
    int i = 0;

    await PlayAudioETexto(i++);

    exAtual = exerciciosSpeaking[0];
    await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

    await PlayAudioETexto(i++);

    exAtual = exerciciosCriarFrases[0];
    await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

    crosshair.SetActive(true);
    await FluxoSeguirNPCAtelarPrateleira();
    crosshair.SetActive(false);

    await PlayAudioETexto(i++);

    exAtual = exerciciosAlternativas[0];
    await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

    await PlayAudioETexto(i++);

    exAtual = exerciciosCriarFrases[1];
    await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

    await PlayAudioETexto(i++);

    exAtual = exerciciosSpeaking[1];
    await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

    crosshair.SetActive(true);
    await FluxoSeguirNPCAtelarPrateleira();
    crosshair.SetActive(false);

    await PlayAudioETexto(i++);

    exAtual = exerciciosAlternativas[1];
    await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

    missaoManager.AdicionarMissao(
        "interagir_produtos",
        "Get the milk, cereal and snacks from the shelves",
        "Pegue o leite, o cereal e os salgadinhos nas prateleiras"
    );

    ProdutoManager.Instance.LiberarProdutos(nivelAtual);
}

private async Task interacaoA2B1()
{
    int i = 0;

    await PlayAudioETexto(i++);

    exAtual = exerciciosSpeaking[0];
    await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

    await PlayAudioETexto(i++);

    exAtual = exerciciosAlternativas[0];
    await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

    await PlayAudioETexto(i++);

    crosshair.SetActive(true);
    await FluxoSeguirNPCAtelarPrateleira();
    crosshair.SetActive(false);

    await PlayAudioETexto(i++);

    exAtual = exerciciosSpeaking[1];
    await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

    exAtual = exerciciosCriarFrases[0];
    await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

    await PlayAudioETexto(i++);

    exAtual = exerciciosAlternativas[1];
    await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

    await PlayAudioETexto(i++);

    crosshair.SetActive(true);
    await FluxoSeguirNPCAtelarPrateleira();
    crosshair.SetActive(false);

    await PlayAudioETexto(i++);

    exAtual = exerciciosSpeaking[2];
    await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

    exAtual = exerciciosCriarFrases[1];
    await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

    await PlayAudioETexto(i++);

    exAtual = exerciciosSpeaking[3];
    await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

    missaoManager.AdicionarMissao(
        "interagir_produtos",
        "Get the selected products from the shelves",
        "Pegue os produtos escolhidos nas prateleiras"
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
        exerciciosCriarFrases.Clear();
        exerciciosSpeaking.Clear();
        exerciciosAlternativas.Clear();

        if (nivelAtual == "A1")
        {
            dialogoAtual = new List<string>()
            {
                "Hi. Are you looking for anything today?",
                "Follow me.",
                "The milk is here. Do you need anything else?",
                "Sure, I will show you",
                "The fruits are here. Can I do anything else for you?",
                "Yes, we have apples too."
            };

            exerciciosCriarFrases.Add(new ExercicioBlocos() 
            { 
                enunciado = "Complete the sentence with the correct word: Onde está o leite?",
                blocosPalavras = new List<string>() { "Where", "milk", "thank", "you", "that", "is", "all", "hello", "bread", "the" },
                respostaCorreta = "Where is the milk?",
            });

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "Ask the supermarket employee if there are any apples available today."
                }
            );

            exerciciosWriting.Add(new ExercicioWriting()
            {
                enunciado = "Write a sentence saying you need vegetables"
            });

            exerciciosAlternativas.Add(new ExercicioAlternativas()
            {
                enunciado = "Answer choosing the right sentence:",
                alternativas = new List<string>() { "She is my teacher.\r\n", "They go to school.\r\n", "That's all I need, thank you\r\n", "I like to play soccer.\r\n" },
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
                "There are special discounts today. Do you often buy snacks from this aisle, or do you prefer the one near the bakery?",
                "The snacks are here. Can I help you find anything else, or do you already have everything?"
           };

            exerciciosCriarFrases.Add(new ExercicioBlocos()
            {
                enunciado = "Ask where you can find the products 'Milk' and 'Cereal' with the words bellow:",
                blocosPalavras = new List<string>() { "Where", "milk", "thank", "you", "that", "can", "all", "hello", "bread", "the", "cereal", "ok", "and", "I", "find" },
                respostaCorreta = "Where can I find the milk and cereal?"
            });

            exerciciosSpeaking.Add(new ExercicioSpeaking()
            {
                enunciado = "Say the right sentence from the list:",
                opcoesFala = new List<string>() { "Yes, I want some snacks too.\n", "Yes, I went to the park yesterday.\n", "No, the computer is very fast.\r\n" },
                respostaCorreta = 0
            });

            exerciciosCriarFrases.Add(new ExercicioBlocos()
            {
                enunciado = "Arrange the words to form the sentence in english: Há descontos especiais hoje?",
                blocosPalavras = new List<string>() { "Where", "thank", "you", "there", "Are", "all", "hello", "special", "the", "cereal", "any", "and", "discounts", "bread", "today" },
                respostaCorreta = "Are there any special discounts today?"
            });

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "Tell the employee whether you usually buy snacks from this aisle or prefer the one near the bakery."
                }
            );

            exerciciosAlternativas.Add(new ExercicioAlternativas()
            {
                enunciado = "Answer choosing the right sentence:",
                alternativas = new List<string>() { "The cat is sleeping under the table.\r\n", "My brother studies at the university.\r\n", "No, thank you. I have everything I need.\r\n", "We usually eat dinner at 7 p.m.\r\n" },
                alternativaCorreta = 2
            });
        }
        else if(nivelAtual == "B1") // B1
        {
            dialogoAtual = new List<string>()
            { 
                "Hi! What are you searching for today, and would you like me to show you where the products are?",
                "Yes, there are still some discounts available on pizzas and vegetables.",
                "Of course, follow me, and I'll show you where they are.",
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
                opcoesFala = new List<string>() { "Nice. I'm looking for some healthy food and bottled soda too.\n", "There is a pharmacy next to the supermarket entrance. He doesn’t enjoy waking up early either.\n", "He doesn’t enjoy waking up early either.\r\n" },
                respostaCorreta = 0
            });

            exerciciosCriarFrases.Add(new ExercicioBlocos() 
            { 
                enunciado = "Arrange the words to form the sentence in english: Isso era tudo que eu precisava. Se eu precisar de qualquer coisa eu aviso. Obrigado/a.",
                blocosPalavras = new List<string>() { "No", "milk", "I", "That", "else", "you", "you", "know", "needed", "was", "same", "all", "hello", "need", "Thank", "If", "I'll", "maybe", "ok", "perfect", "so", "I", "let", "anything" },
                respostaCorreta = "That was all I needed. If I need anything else, I'll let you know. Thank you.",
            });
        }
else if (nivelAtual == "A1-A2")
{
    dialogoAtual = new List<string>()
    {
        "Hi! Are you looking for something specific today?",
        "Of course! What products do you need?",
        "Sure, follow me. I'll show you where the milk is.",
        "Here it is. We also have different types of milk. Do you usually buy this one?",
        "I can show you the cereal aisle too. It's just around the corner.",
        "Here are the cereals. Some brands are on sale today. Would you like to check them out?",
        "The snacks are in the next aisle. Do you need anything else?",
        "Of course! Let me know if you need more help. Happy shopping!"
    };

    exerciciosCriarFrases.Add(new ExercicioBlocos()
    {
        enunciado = "Arrange the words to form the correct sentence in English: Você poderia me mostrar onde fica o leite?",
        blocosPalavras = new List<string>()
        {
            "Could", "me", "where", "the", "milk", "show", "you", "is",
            "please", "find", "I", "can", "it"
        },
        respostaCorreta = "Could you show me where the milk is?"
    });

    exerciciosSpeaking.Add(new ExercicioSpeaking()
    {
        enunciado = "The attendant asks what you are looking for. Tell them that you need milk and cereal.",
        opcoesFala = new List<string>()
        {
            "I'm looking for some milk and cereal.\r\n",
            "I would like to book a hotel room.\r\n",
            "I'm waiting for my flight.\r\n"
        },
        respostaCorreta = 0
    });

    exerciciosAlternativas.Add(new ExercicioAlternativas()
    {
        enunciado = "The attendant shows you the milk. Which question asks whether it is available in another type?",
        alternativas = new List<string>()
        {
            "Do you have another kind of milk?",
            "How much is the hotel room?",
            "Where is the nearest airport?",
            "Can I pay for my dinner?"
        },
        alternativaCorreta = 0
    });

    exerciciosCriarFrases.Add(new ExercicioBlocos()
    {
        enunciado = "Arrange the words to form the correct sentence in English: Você costuma comprar essa marca de cereal?",
        blocosPalavras = new List<string>()
        {
            "Do", "you", "usually", "buy", "this", "brand", "of",
            "cereal", "does", "where", "are", "today"
        },
        respostaCorreta = "Do you usually buy this brand of cereal?"
    });

    exerciciosSpeaking.Add(new ExercicioSpeaking()
    {
        enunciado = "The attendant tells you that some cereals are on sale. Ask whether there are any special discounts.",
        opcoesFala = new List<string>()
        {
            "Are there any special discounts today?\r\n",
            "I have a reservation for two.\r\n",
            "Could you bring me the menu?\r\n"
        },
        respostaCorreta = 0
    });

    exerciciosAlternativas.Add(new ExercicioAlternativas()
    {
        enunciado = "The attendant asks if you need anything else. Choose the most appropriate response.",
        alternativas = new List<string>()
        {
            "No, thank you. I think that's everything I need.",
            "I usually go to school by bus.",
            "The weather is very cold today.",
            "I would like to change my hotel room."
        },
        alternativaCorreta = 0
    });
}

else if (nivelAtual == "A2-B1")
{
    dialogoAtual = new List<string>()
    {
        "Hi! Are you looking for anything in particular, or would you like some recommendations?",
        "Certainly. Which products are you planning to buy?",
        "Follow me, please. I'll show you where the dairy products are.",
        "Here is the milk section. We have whole, semi-skimmed, and lactose-free options. Do you have a preference?",
        "The cereal aisle is just ahead. Some brands are currently discounted.",
        "This brand is quite popular, especially because it contains less sugar than some of the alternatives.",
        "We also have a promotion on snacks in the next aisle. Would you like to take a look?",
        "Here are the available options. Some products have a discount when you buy more than one.",
        "Is there anything else you would like me to help you find?",
        "You're welcome! Enjoy your shopping."
    };

    exerciciosSpeaking.Add(new ExercicioSpeaking()
    {
        enunciado = "Explain to the attendant which products you need and whether you have any particular preferences."
    });

    exerciciosAlternativas.Add(new ExercicioAlternativas()
    {
        enunciado = "The attendant offers whole, semi-skimmed, and lactose-free milk. Which response clearly expresses a preference?",
        alternativas = new List<string>()
        {
            "I'd prefer lactose-free milk, if you have it.",
            "I went to the supermarket yesterday.",
            "I would like to reserve a table.",
            "The cereal is under the chair."
        },
        alternativaCorreta = 0
    });

    exerciciosSpeaking.Add(new ExercicioSpeaking()
    {
        enunciado = "You want to know more about the cereal promotion. Ask the attendant whether the discount applies when buying more than one package."
    });

    exerciciosCriarFrases.Add(new ExercicioBlocos()
    {
        enunciado = "Arrange the words to form the correct sentence in English: Gostaria de comparar as opções antes de escolher uma marca.",
        blocosPalavras = new List<string>()
        {
            "I'd", "like", "to", "compare", "the", "options",
            "before", "choosing", "a", "brand", "because",
            "they", "are", "expensive", "today"
        },
        respostaCorreta = "I'd like to compare the options before choosing a brand."
    });

    exerciciosAlternativas.Add(new ExercicioAlternativas()
    {
        enunciado = "The attendant says that one cereal contains less sugar than the alternatives. What does this information help you do?",
        alternativas = new List<string>()
        {
            "Compare the nutritional characteristics of the products.",
            "Find out how much the shopping cart weighs.",
            "Check whether the store accepts cash.",
            "Ask for a different shopping basket."
        },
        alternativaCorreta = 0
    });

    exerciciosSpeaking.Add(new ExercicioSpeaking()
    {
        enunciado = "You are interested in the snack promotion, but you are unsure whether buying multiple packages is worthwhile. Ask the attendant for more details."
    });

    exerciciosCriarFrases.Add(new ExercicioBlocos()
    {
        enunciado = "Arrange the words to form the correct sentence in English: Se eu comprar duas embalagens, receberei algum desconto?",
        blocosPalavras = new List<string>()
        {
            "If", "I", "buy", "two", "packages,",
            "will", "I", "get", "a", "discount",
            "today", "because", "they", "are", "available"
        },
        respostaCorreta = "If I buy two packages, will I get a discount?"
    });

    exerciciosSpeaking.Add(new ExercicioSpeaking()
    {
        enunciado = "The attendant asks whether you need anything else. Explain that you have found everything you need and thank them."
    });
}
    }

    private TipoExercicio ObterTipoExercicio(ExercicioBase exercicio)
    {
        if (exercicio is ExercicioSpeaking)
            return TipoExercicio.Speaking;
        if (exercicio is ExercicioAlternativas)
            return TipoExercicio.Alternativas;
        if (exercicio is ExercicioBlocos)
            return TipoExercicio.Blocos;
        if (exercicio is ExercicioCompletarFrase)
            return TipoExercicio.CompletarFrase;
        if (exercicio is ExercicioWriting)
            return TipoExercicio.Writing;
        if (exercicio is ExercicioAssociacao)
            return TipoExercicio.Associacao;

        throw new InvalidOperationException("Tipo de exercício não suportado: " + exercicio.GetType().Name);
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