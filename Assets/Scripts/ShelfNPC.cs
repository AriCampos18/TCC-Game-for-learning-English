using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System;
using System.Net.Http;
using UnityEngine.Networking;
using System.IO;
using TMPro;
using UnityEngine.AI; // ✨ Essencial para o NavMeshAgent

public class ShelfNPC : InteracaoNPC, IRoteiroNpcCheckpoint
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

    private int indiceDestinoPrateleira;
    private bool podeInteragirCenario = true;
    private int paradasMarketConcluidas;
    private Vector3 posicaoInicial;
    private Quaternion rotacaoInicial;
    public TextMeshProUGUI textoPularDialogo;

    public string idNpcParaVoz = "shelf"; 
    private BackendManager backendManager;
    private const string NpcKey = "shelf_npc";
    private ExerciseLoadResult catalogoExercicios;
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
        posicaoInicial = transform.position;
        rotacaoInicial = transform.rotation;
        nomeExibicaoLegenda = "Shelf Attendant";
        backendManager = new BackendManager();
        missaoManager = MissionManager.Instance; 
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        
        exerciciosCompletarFrase = new List<ExercicioBase>();
        exerciciosCriarFrases = new List<ExercicioBase>();
        exerciciosSpeaking = new List<ExercicioBase>();
        exerciciosAlternativas = new List<ExercicioBase>();
        exerciciosWriting = new List<ExercicioBase>();

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
    protected override void ReiniciarVisita()
    {
        podeInteragirCenario = true;
        paradasMarketConcluidas = indiceDestinoPrateleira = 0;
        if (agent != null && agent.isOnNavMesh)
        {
            agent.ResetPath();
            agent.Warp(posicaoInicial);
        }
        transform.SetPositionAndRotation(posicaoInicial, rotacaoInicial);
    }

    public override bool PodeInteragir()
    {
        return base.PodeInteragir() && podeInteragirCenario;
    }

    protected override Task IniciarInteracao() => ExecutarAtendimento(0, false);

    private async Task ExecutarAtendimento(int inicio, bool retomada)
    {
        int versao = SessaoCenario.Atual.Versao;
        GameProgress.EstaEmDialogo = true; // ✨ Trava tudo aqui
        podeInteragirCenario = false;
        
        nivelAtual = DadosJogador.nivelUsuario;

        bool intermediario = MissionManager.NivelMarketIntermediario(nivelAtual);
        if (!retomada && intermediario)
            MissionManager.Instance?.ConcluirMissaoMarket("falar_atendente", "npc_interagido", "shelf_npc");

        if (!retomada)
            paradasMarketConcluidas = indiceDestinoPrateleira = 0;
        InicializarConteudosPorNivel();
        catalogoExercicios = await ExerciseManager.Instance.LoadForNpcAsync(NpcKey, nivelAtual);
        if (!catalogoExercicios.Success)
            Debug.LogWarning("ShelfNPC: usando exercícios locais. " + catalogoExercicios.Error);

        ValidarVisita(versao);
        await CriarRoteiro(versao).Executar(inicio);
        LiberarProdutosDoRoteiro();

        if (!intermediario)
            MissionManager.Instance.ConcluirMissaoMarket("falar_atendente", "conversa_encerrada", "shelf_npc");

        GameProgress.Instance.falouAtendente = true;
        if (intermediario)
            MissionManager.Instance?.RegistrarEventoMarket("conversa_encerrada", "shelf_npc");
        MissionManager.Instance?.TentarLiberarCaixaMarket();
        GameProgress.EstaEmDialogo = false; // ✨ Só destrava aqui no final de TUDO
        podeInteragirCenario = true;
    
        if (modalLegenda != null)
            modalLegenda.SetActive(false);

        if (textoPularDialogo != null)
            textoPularDialogo.gameObject.SetActive(false);

        GameProgress.EstaEmDialogo = false;

        LiberarControlePlayer();
    }


    private List<Transform> RotaPrateleiras() => nivelAtual switch
    {
        "A1" => pontosPrateleiraA1,
        "A1-A2" or "A2" or "A2-B1" => pontosPrateleiraA2,
        "B1" => pontosPrateleiraB1,
        _ => null
    };

    async Task<Func<Task>> IRoteiroNpcCheckpoint.PrepararRetomada(Newtonsoft.Json.Linq.JObject visita,
        int versao, System.Threading.CancellationToken token)
    {
        ValidarVisita(versao);
        AtualizarVisita();
        nivelAtual = SessaoCenario.Atual.Nivel;
        InicializarConteudosPorNivel();
        var cursor = CriarRoteiro(versao).Validar(visita);
        var rota = RotaPrateleiras();
        if (cursor.Situacao.Length != 2 || !cursor.Situacao[0].StartsWith("route.")
            || !cursor.Situacao[1].StartsWith("stops.")
            || !int.TryParse(cursor.Situacao[0].Substring(6), out int destino)
            || !int.TryParse(cursor.Situacao[1].Substring(6), out int paradas)
            || rota == null || destino < 0 || destino > rota.Count || paradas < 0 || paradas != destino
            || (destino > 0 && (rota[destino - 1] == null || agent == null || !agent.isOnNavMesh)))
            throw new InvalidDataException("A rota salva das prateleiras não pode ser reconstruída.");
        await RoteiroNpcCheckpoint.ExigirCatalogo(this, versao, token);
        return async () =>
        {
            ValidarVisita(versao);
            indiceDestinoPrateleira = destino;
            paradasMarketConcluidas = paradas;
            if (destino > 0)
            {
                var ponto = rota[destino - 1];
                agent.ResetPath();
                if (!agent.Warp(ponto.position))
                    throw new InvalidOperationException("Não foi possível restaurar o atendente na prateleira.");
                npcPosicaoOriginal = ponto.position;
                npcRotacaoOriginal = ponto.rotation;
                transform.SetPositionAndRotation(ponto.position, ponto.rotation);
            }
            TravarControlePlayer();
            await ExecutarAtendimento(cursor.Indice, true);
        };
    }

    private async Task SeguirRoteiro()
    {
        crosshair.SetActive(true);
        await FluxoSeguirNPCAtelarPrateleira();
        crosshair.SetActive(false);
    }

    private void LiberarProdutosDoRoteiro()
    {
        switch (nivelAtual)
        {
            case "A1":
                missaoManager.IniciarMissaoMarket(
                "interagir_produtos",
                "Get the milk and the fruit from the shelves",
                "Pegue o leite e a fruta das prateleiras"
                );

                ProdutoManager.Instance.LiberarProdutos(nivelAtual);
                break;
            case "A2":
                missaoManager.IniciarMissaoMarket(
                "interagir_produtos",
                "Get the milk, the cereal and the snack from the shelves",
                "Pegue o leite, o cereal e o salgadinho das prateleiras"
                );

                ProdutoManager.Instance.LiberarProdutos(nivelAtual);
                break;
            case "B1":
                missaoManager.IniciarMissaoMarket(
                "interagir_produtos",
                "Get the vegetable and the soda from the shelves",
                "Pegue o legumes e o refrigerante das prateleiras"
                );

                ProdutoManager.Instance.LiberarProdutos(nivelAtual);
                break;
            case "A1-A2":
                if (!missaoManager.MissaoConcluida("seguir_atendente"))
                return;

                missaoManager.IniciarMissaoMarket(
                "interagir_produtos",
                "Pick up milk, cereal, and snacks from the shelves.",
                "Pegue leite, cereal e salgadinhos nas prateleiras."
                );

                ProdutoManager.Instance.LiberarProdutos(nivelAtual);
                break;
            case "A2-B1":
                if (!missaoManager.MissaoConcluida("seguir_atendente"))
                return;

                missaoManager.IniciarMissaoMarket(
                "interagir_produtos",
                "Collect milk, cereal, and snacks from their respective shelves.",
                "Recolha leite, cereal e salgadinhos nas respectivas prateleiras."
                );

                ProdutoManager.Instance.LiberarProdutos(nivelAtual);
                break;
        }
    }

    internal RoteiroNpcCheckpoint DescreverRoteiro(string nivel)
    {
        var conteudo = new ConteudoRoteiro();
        InicializarConteudosPorNivel(conteudo, nivel);
        return CriarRoteiro(0, conteudo, nivel);
    }

    private RoteiroNpcCheckpoint CriarRoteiro(int visita, ConteudoRoteiro descricao = null, string nivelDescricao = null)
    {
        string nivelAtual = nivelDescricao ?? this.nivelAtual;
        var dialogoAtual = descricao?.Falas ?? this.dialogoAtual;
        var exerciciosCriarFrases = descricao?.exerciciosCriarFrases ?? this.exerciciosCriarFrases;
        var exerciciosSpeaking = descricao?.exerciciosSpeaking ?? this.exerciciosSpeaking;
        var exerciciosAlternativas = descricao?.exerciciosAlternativas ?? this.exerciciosAlternativas;
        var exerciciosWriting = descricao?.exerciciosWriting ?? this.exerciciosWriting;

        var roteiro = new RoteiroNpcCheckpoint(this, visita, () => new[] { $"route.{indiceDestinoPrateleira}", $"stops.{paradasMarketConcluidas}" });
        void Fala(int indice, bool mostrarLegenda = true) => roteiro.Fala(dialogoAtual[indice], () => PlayAudioETexto(indice, mostrarLegenda));
        void Exercicio(string ponto, int ordem, ExercicioBase fallback) =>
            roteiro.Exercicio(ponto, ordem, () => AbrirExercicioDoPonto(ponto, ordem, fallback), modelo: fallback);

        switch (nivelAtual)
        {
            case "A1":
                Fala(0);
                Exercicio("product_location", 1, exerciciosCriarFrases[0]);
                Fala(1);
                roteiro.Acao("shelf.route.1", SeguirRoteiro);
                Fala(2);
                Exercicio("additional_product", 1, exerciciosSpeaking[0]);
                Fala(3);
                roteiro.Acao("shelf.route.2", SeguirRoteiro);
                Fala(4);
                Exercicio("additional_product", 2, exerciciosWriting[0]);
                Fala(5);
                Exercicio("shopping_complete", 1, exerciciosAlternativas[0]);
                break;
            case "A2":
                Fala(0);
                Exercicio("product_location", 1, exerciciosCriarFrases[0]);
                Fala(1);
                roteiro.Acao("shelf.route.1", SeguirRoteiro);
                Fala(2);
                Exercicio("additional_product", 1, exerciciosSpeaking[0]);
                Exercicio("discounts", 1, exerciciosCriarFrases[1]);
                Fala(3);
                Exercicio("snack_preference", 1, exerciciosSpeaking[1]);
                roteiro.Acao("shelf.route.2", SeguirRoteiro);
                Fala(4);
                Exercicio("shopping_complete", 1, exerciciosAlternativas[0]);
                break;
            case "B1":
                Fala(0);
                Exercicio("discounts", 1, exerciciosSpeaking[0]);
                Fala(1);
                Exercicio("shopping_request", 1, exerciciosSpeaking[1]);
                Fala(2);
                roteiro.Acao("shelf.route.1", SeguirRoteiro);
                Fala(3);
                Exercicio("shopping_complete", 1, exerciciosCriarFrases[0]);
                Fala(4);
                break;
            case "A1-A2":
                Fala(0);
                Exercicio("shopping_request", 1, exerciciosSpeaking[0]);
                Fala(1);
                Exercicio("product_location", 1, exerciciosCriarFrases[0]);
                Fala(2);
                roteiro.Acao("shelf.route.1", SeguirRoteiro);
                Fala(3);
                Exercicio("milk_preference", 1, exerciciosAlternativas[0]);
                Fala(4);
                Exercicio("cereal_preference", 1, exerciciosCriarFrases[1]);
                Fala(5);
                Exercicio("discounts", 1, exerciciosSpeaking[1]);
                roteiro.Acao("shelf.route.2", SeguirRoteiro);
                Fala(6);
                Exercicio("shopping_complete", 1, exerciciosAlternativas[1]);
                Fala(7);
                break;
            case "A2-B1":
                Fala(0);
                Exercicio("shopping_request", 1, exerciciosSpeaking[0]);
                Fala(1);
                Fala(2);
                roteiro.Acao("shelf.route.1", SeguirRoteiro);
                Fala(3);
                Exercicio("milk_preference", 1, exerciciosAlternativas[0]);
                Fala(4);
                Exercicio("discounts", 1, exerciciosSpeaking[1]);
                Exercicio("cereal_preference", 1, exerciciosCriarFrases[0]);
                Fala(5);
                Exercicio("cereal_preference", 2, exerciciosAlternativas[1]);
                Fala(6);
                roteiro.Acao("shelf.route.2", SeguirRoteiro);
                Fala(7);
                Exercicio("discounts", 2, exerciciosSpeaking[2]);
                Exercicio("discounts", 3, exerciciosCriarFrases[1]);
                Fala(8);
                Exercicio("shopping_complete", 1, exerciciosSpeaking[3]);
                Fala(9);
                break;
        }
        return roteiro;
    }

    private async Task FluxoSeguirNPCAtelarPrateleira()
    {
        int visita = SessaoCenario.Atual.Versao;
        if (agent != null)
        {
        var rota = RotaPrateleiras();

        Transform pontoDestinoEscolhido = rota != null && indiceDestinoPrateleira < rota.Count
            ? rota[indiceDestinoPrateleira++]
            : null;
        if (pontoDestinoEscolhido == null)
        {
            Debug.LogError($"Não há um ponto de destino válido configurado para o nível {nivelAtual} no índice atual!");
            return;
        }

        bool intermediario = MissionManager.NivelMarketIntermediario(nivelAtual);
        if (intermediario)
        {
            bool iniciante = nivelAtual == "A1-A2";
            missaoManager.IniciarMissaoMarket("seguir_atendente",
                iniciante ? "Follow the attendant to the correct shelves."
                    : "Follow the attendant to the shelves where the products are located.",
                iniciante ? "Siga o atendente até as prateleiras corretas."
                    : "Siga o atendente até as prateleiras onde estão os produtos.");
        }
        else
        {
            missaoManager.IniciarMissaoMarket(
                "seguir_atendente",
                "Follow the attendant to get the products",
                "Siga o atendente para pegar os produtos"
            );
        }

        if (modalLegenda != null) modalLegenda.SetActive(false);

        LiberarControlePlayer();

        // Faz o NPC andar até o destino atual da sequência
        bool destinoAceito = agent.SetDestination(pontoDestinoEscolhido.position);
        if (intermediario && !destinoAceito)
        {
            TravarControlePlayer();
            Debug.LogError("ShelfNPC: o agente não aceitou o destino da prateleira.");
            return;
        }

        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            if (intermediario && !agent.pathPending && agent.pathStatus != NavMeshPathStatus.PathComplete)
                break;
            await Task.Yield();
            ValidarVisita(visita);
        }

        if (intermediario && agent.pathStatus != NavMeshPathStatus.PathComplete
            && Vector3.Distance(transform.position, pontoDestinoEscolhido.position) > agent.stoppingDistance)
        {
            agent.ResetPath();
            TravarControlePlayer();
            Debug.LogError("ShelfNPC: o agente não alcançou o destino da prateleira.");
            return;
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
            ValidarVisita(visita);
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
                ValidarVisita(visita);
            }
        }
        // Se o player chegou perto clicando ou segurando o mouse, espera ele soltar.
        while (Input.GetMouseButton(0) || Input.GetMouseButtonDown(0))
        {
            await Task.Yield();
            ValidarVisita(visita);
        }

            int paradasNecessarias = nivelAtual == "B1" ? 1 : 2;
            if (++paradasMarketConcluidas == paradasNecessarias)
                missaoManager.ConcluirMissaoMarket("seguir_atendente", "marco_fluxo", "rota_prateleiras_concluida");
            if (modalLegenda != null) 
                modalLegenda.SetActive(false);
        } 
    }

    private void InicializarConteudosPorNivel(ConteudoRoteiro descricao = null, string nivelDescricao = null)
    {
        string nivelAtual = nivelDescricao ?? this.nivelAtual;
        var dialogoAtual = this.dialogoAtual;
        var exerciciosCriarFrases = descricao?.exerciciosCriarFrases ?? this.exerciciosCriarFrases;
        var exerciciosSpeaking = descricao?.exerciciosSpeaking ?? this.exerciciosSpeaking;
        var exerciciosAlternativas = descricao?.exerciciosAlternativas ?? this.exerciciosAlternativas;
        var exerciciosWriting = descricao?.exerciciosWriting ?? this.exerciciosWriting;

        exerciciosCriarFrases.Clear();
        exerciciosSpeaking.Clear();
        exerciciosAlternativas.Clear();
        exerciciosWriting.Clear();

        if (nivelAtual == "A1")
        {
            dialogoAtual = new List<string>()
            {
                "Hi. Are you looking for anything today?",
                "Follow me.",
                "The milk is here. Do you need anything else?",
                "Sure, I will show you",
                "The fruits are here.",
                "Yes, we have vegetables too. Can I do anything else for you?"
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
                    modoLivre = true,
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
                    modoLivre = true,
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
            "I'm waiting for my flight.\r\n",
            "I'm looking for some milk and cereal.\r\n",
            "I would like to book a hotel room.\r\n"
        },
        respostaCorreta = 1
    });

    exerciciosAlternativas.Add(new ExercicioAlternativas()
    {
        enunciado = "The attendant shows you the milk. Which question asks whether it is available in another type?",
        alternativas = new List<string>()
        {
            "Where is the nearest airport?",
            "Can I pay for my dinner?",
            "Do you have another kind of milk?",
            "How much is the hotel room?"
        },
        alternativaCorreta = 2
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
            "Could you bring me the menu?\r\n",
            "Are there any special discounts today?\r\n",
            "I have a reservation for two.\r\n"
        },
        respostaCorreta = 1
    });

    exerciciosAlternativas.Add(new ExercicioAlternativas()
    {
        enunciado = "The attendant asks if you need anything else. Choose the most appropriate response.",
        alternativas = new List<string>()
        {
            "I would like to change my hotel room.",
            "No, thank you. I think that's everything I need.",
            "I usually go to school by bus.",
            "The weather is very cold today."
        },
        alternativaCorreta = 1
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
        modoLivre = true,
        enunciado = "Explain to the attendant which products you need and whether you have any particular preferences."
    });

    exerciciosAlternativas.Add(new ExercicioAlternativas()
    {
        enunciado = "The attendant offers whole, semi-skimmed, and lactose-free milk. Which response clearly expresses a preference?",
        alternativas = new List<string>()
        {
            "I would like to reserve a table.",
            "The cereal is under the chair.",
            "I'd prefer lactose-free milk, if you have it.",
            "I went to the supermarket yesterday."
        },
        alternativaCorreta = 2
    });

    exerciciosSpeaking.Add(new ExercicioSpeaking()
    {
        modoLivre = true,
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
            "Find out how much the shopping cart weighs.",
            "Check whether the store accepts cash.",
            "Ask for a different shopping basket.",
            "Compare the nutritional characteristics of the products."
        },
        alternativaCorreta = 3
    });

    exerciciosSpeaking.Add(new ExercicioSpeaking()
    {
        modoLivre = true,
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
        modoLivre = true,
        enunciado = "The attendant asks whether you need anything else. Explain that you have found everything you need and thank them."
    });
}

        if (descricao != null)
            descricao.Falas = dialogoAtual;
        else
        {
            this.dialogoAtual = dialogoAtual;
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
        int visita = SessaoCenario.Atual.Versao;
        ValidarVisita(visita);

        byte[] audioBytes = await backendManager.GerarAudio(textoParaFalar, idNpcParaVoz);
        ValidarVisita(visita);
        
        if (audioBytes != null && audioBytes.Length > 0)
        {
            string caminho = Path.Combine(Application.persistentDataPath, "audio_temp_custom.wav");
            File.WriteAllBytes(caminho, audioBytes);
            
            UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + caminho, AudioType.WAV);
            var operation = www.SendWebRequest();
            
            while (!operation.isDone)
                await Task.Yield();

            ValidarVisita(visita);
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
            ValidarVisita(visita);
        }
    }
    private Task AbrirExercicioDoPonto(string pointKey, int ordem, ExercicioBase fallback)
    {
        CheckpointJogo.IdentificarFallback(fallback, pointKey, ordem, nivelAtual);
        TipoExercicio tipo = ObterTipoExercicio(fallback);
        bool? speakingLivre = (fallback as ExercicioSpeaking)?.modoLivre;
        ExerciseCatalogItem item = catalogoExercicios?.Find(pointKey, nivelAtual, ordem, tipo, speakingLivre);
        if (item != null)
            return AbrirExercicio(item.Tipo, item.Exercise);

        Debug.LogWarning($"ShelfNPC: usando exercício local em {nivelAtual}/{pointKey}/{ordem}.");
        return AbrirExercicio(tipo, fallback);
    }

    public async Task AbrirExercicio(TipoExercicio tipo, ExercicioBase ex)
    {
        Debug.Log("Abrindo exercício: " + tipo);

        ModalLegenda legenda = modalLegenda.GetComponent<ModalLegenda>();

        if (modalLegenda != null)
        {
            modalLegenda.SetActive(false);
        }

        await EntrarModoExercicio(tipo);

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

        await SairModoExercicio(tipo);

        // ✨ Volta a prender o mouse após terminar o exercício (se o diálogo geral não tiver acabado)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (legenda != null)
        {
            legenda.AjustarPosicaoPeloEstadoDoJogo();
        }
    }
}