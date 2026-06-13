using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using TMPro;
using System;

public class CashierNPC : InteracaoNPC
{
    int horaAtual = DateTime.Now.Hour;

    string nivelAtual;
    public GameObject modalAvisoRevisao;

    public UnityEngine.AI.NavMeshAgent navMeshAgent; 
    public Transform pontoProduto;         // Arraste o Objeto Vazio da prateleira aqui
    public Transform pontoOriginalCaixa;   // Arraste um Objeto Vazio na posição inicial do Caixa aqui
    public GameObject produtoChocolate;
    List<string> dialogoAtual;
    private Animator animator;
    
    public TextMeshProUGUI textoPularDialogo;
    string cumprimento;

    public string idNpcParaVoz = "cashier"; // Altere no Inspector para "homem_caixa" ou "mulher_padaria"
    double valorTotal;
    private AudioSource audioSource;
    public ModalExercicio modalExercicio;
    private BackendManager backendManager;

    public List<ExercicioBase> exerciciosBlocos;
    public List<ExercicioBase> exerciciosSpeaking;
    public List<ExercicioBase> exerciciosAlternativas;

    public List<Transform> pontosProdutosBalcao;
    public GameObject sacolaFinal;
    private bool aguardandoPegarSacola = false;
    private bool sacolaFoiPega = false;
    public float tempoProdutosNoBalcao = 3f;

    private List<GameObject> produtosInstanciadosNoCaixa = new List<GameObject>();
    private bool checkoutVisualJaFeito = false;

    public ExercicioBase exAtual;
    public GameObject modalLegenda;

    protected override void Start()
    {
        base.Start();
        nomeExibicaoLegenda = "Cashier Attendant";
        backendManager = new BackendManager();
        audioSource = GetComponent<AudioSource>();
        exerciciosBlocos = new List<ExercicioBase>();
        exerciciosSpeaking = new List<ExercicioBase>();
        animator = GetComponent<Animator>();
        exerciciosAlternativas = new List<ExercicioBase>();
        nivelAtual = DadosJogador.nivelUsuario;

        if (horaAtual < 12)
        {
            cumprimento = "Good morning!";
        }
        else if (horaAtual < 18)
        {
            cumprimento = "Good afternoon!";
        }
        else
        {
            cumprimento = "Good evening!";
        }
    }

    private void InicializarConteudosPorNivel()
    {
        if (nivelAtual == "A1")
        {
            valorTotal = 10.00;
            dialogoAtual = new List<string>()
            {
                cumprimento,
                "Do you want anything else too?",
                "The total is ten dollars",
                "What are you paying with today?",
                "Thank you.",
            };

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "Say hello like the cashier:",
                    opcoesFala = new List<string>() { cumprimento+"\n" },
                    respostaCorreta = 0
                }
            );

            exerciciosBlocos.Add(
                new ExercicioBlocos() 
                { 
                    enunciado = "Arrange the words to form the correct sentence in english: Sim, eu quero algum chocolate",
                    blocosPalavras = new List<string>() { "No", "Yes", "thank", "you", "that", "chocolate", "was", "want", "all", "hello", "thank", "I", "maybe", "some" },
                    respostaCorreta = "Yes, I want some chocolate\n",
                }
            );

            exerciciosAlternativas.Add(
                new ExercicioAlternativas()
                {
                    enunciado = "What is the total amount?",
                    alternativas = new List<string>() { "3 dollars\r\n", "19 dollars\r\n", "10 dollars\r\n", "16 dollars\r\n" },
                    alternativaCorreta = 2
                });

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "Say the right sentence:",
                    opcoesFala = new List<string>() { "I’m paying with my card, please.\n", "There is a chair near the door.\n", "He plays soccer every week.\n" },
                    respostaCorreta = 0
                }
            );
        }
        else if (nivelAtual == "A2")
        {
            valorTotal = 14.50;
            dialogoAtual = new List<string>()
            {
                cumprimento,
                "Your total is fourteen dollars and fifty cents",
                "Do you usually bring your own bags, or do you need some today?",
                "Can I help you with anything else, or do you already have everything you need too?",
                "What are you paying with today: cash or card?",
                "Thank you."
            };

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "Repeat the cashier greeting:",
                    opcoesFala = new List<string>() { cumprimento+"\n" },
                    respostaCorreta = 0
                }
            );

            exerciciosAlternativas.Add(
                new ExercicioAlternativas()
                {
                    enunciado = "What is the total amount?",
                    alternativas = new List<string>() { "14.50 dollars\r\n", "40.50 dollars\r\n", "4.05 dollars\r\n", "4.50 dollars\r\n" },
                    alternativaCorreta = 0
                });

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "Speak the total purchase price: "+valorTotal+"\n",
                    opcoesFala = new List<string>() { valorTotal+"\n" },
                    respostaCorreta = 0
                }
            );

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "Say the right sentence to respond the cashier:",
                    opcoesFala = new List<string>() { "I am waiting for my friend near the entrance.\n", "There are some magazines under the counter.\n", "I usually bring my own bags, but I need one today.\n" },
                    respostaCorreta = 2
                }
            );

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "Say the right sentence to respond the cashier:",
                    opcoesFala = new List<string>() { "There are many cars in the parking lot.\n", "He is standing between the shelves.\n", "No, I already have everything I need.\n" },
                    respostaCorreta = 2
                }
            );

            exerciciosAlternativas.Add(
                new ExercicioAlternativas()
                {
                    enunciado = "Answer choosing the right sentence:",
                    alternativas = new List<string>() { "We are watching a movie tonight.\r\n", "He doesn’t drink coffee either.\r\n", "I’m paying with my debit card.\r\n", "There is a long line at the bakery.\r\n" },
                    alternativaCorreta = 2
                });
        }
        else
        {
            valorTotal = 16.75;
            dialogoAtual = new List<string>()
            { 
                cumprimento+" Did you find everything you were looking for today?",
                "Your total is sixteen dollars and seventy-five cents",
                "There are reusable bags left near the register, or would you prefer paper bags instead?",
                "What payment method are you using today, and would you also like a printed receipt?",
                "Thank you for shopping with us. Have a great day!"
            };

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "After listening to the cashier greetings, repeat what she said and say the right answer:",
                    opcoesFala = new List<string>() { cumprimento+"Yes, I found everything I needed, thank you.\n", cumprimento+"I like chocolate cake and pizza.\n", cumprimento+"The cat is under the table.\n" },
                    respostaCorreta = 0
                }
            );

            exerciciosAlternativas.Add(
                new ExercicioAlternativas()
                {
                    enunciado = "What is the total amount?",
                    alternativas = new List<string>() { "14.50 dollars\r\n", "16.50 dollars\r\n", "60.75 dollars\r\n", "16.75 dollars\r\n" },
                    alternativaCorreta = 3
                });

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "Say the total amount for the purchase: " + valorTotal + "\n",
                    opcoesFala = new List<string>() { valorTotal+"\n" },
                    respostaCorreta = 0
                }
            );

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "Say the right sentence to respond the cashier:",
                    opcoesFala = new List<string>() { "She was reading a magazine in the waiting room earlier.\n", "It’s fine, I prefer the reusable bags near the register.\n", "I usually exercise at the gym three times a week.\n" },
                    respostaCorreta = 1
                }
            );

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "Say the right sentence to respond the cashier:",
                    opcoesFala = new List<string>() { "I’m paying with my credit card, and I’d like the receipt too.\n", "He doesn’t enjoy crowded supermarkets either.\n", "There is a long line near the bakery section.\n" },
                    respostaCorreta = 0
                }
            );
        }
    }

    protected override async Task IniciarInteracao()
    {
        if (GameProgress.Instance.PodeFalarComCaixa())
        {
            GameProgress.EstaEmDialogo = true;

            nivelAtual = DadosJogador.nivelUsuario;
            InicializarConteudosPorNivel();

            await MostrarProdutosNoBalcao();

            if (nivelAtual == "A1")
                await interacaoA1();
            else if (nivelAtual == "A2")
                await interacaoA2();
            else
                await interacaoB1();

            await FazerRevisao();

            aguardandoPegarSacola = true;
            sacolaFoiPega = false;

            GameProgress.EstaEmDialogo = false;
            
            if (modalLegenda != null)
                modalLegenda.SetActive(false);

            if (textoPularDialogo != null)
                textoPularDialogo.gameObject.SetActive(false);

            GameProgress.EstaEmDialogo = false;

            LiberarControlePlayer();

            while (!sacolaFoiPega)
            {
                await Task.Yield();
            }

            aguardandoPegarSacola = false;
        }
    }

    private async Task interacaoA1()
    {
        int i = 0;
        // Usuário deve interagir com o caixa para colocar os produtos primeiro, e depois começa a conversa automaticamente

        await PlayAudioETexto(i++, mostrarLegenda: false); 

        exAtual = exerciciosSpeaking[0];
        await AbrirExercicio(TipoExercicio.Speaking, exAtual);

        await PlayAudioETexto(i++, mostrarLegenda: true);  
        
        // Abre o exercício de blocos ("Yes, I want some chocolate")
        await AbrirExercicio(TipoExercicio.Blocos, exerciciosBlocos[0]); 

        await CaminharAteDestino(pontoProduto);

        await Task.Delay(2000);

        if (produtoChocolate != null)
        {
            produtoChocolate.SetActive(false);
        }

        // 3. NPC volta para o local original do caixa
        await CaminharAteDestino(pontoOriginalCaixa);
        
        // Ajusta a rotação final para encarar o jogador novamente se necessário
        this.transform.rotation = pontoOriginalCaixa.rotation;

        await PlayAudioETexto(i++);  
        
        exAtual = exerciciosAlternativas[0];
        await AbrirExercicio(TipoExercicio.Alternativas, exAtual);
        await PlayAudioETexto(i++, mostrarLegenda: true);

        exAtual = exerciciosSpeaking[1];
        await AbrirExercicio(TipoExercicio.Speaking, exAtual);

        await PlayAudioETexto(i++, mostrarLegenda: true);
    }

    private async Task interacaoA2() 
    {
        int i = 0;
        //usuario deve interagir com o caixa para colocar os produtos primeiro, e depois começa a conversa automaticamente
        
        await PlayAudioETexto(i++, mostrarLegenda: false); 

        exAtual = exerciciosSpeaking[0];
        await AbrirExercicio(TipoExercicio.Speaking, exAtual);

        await PlayAudioETexto(i++, mostrarLegenda: true);

        exAtual = exerciciosAlternativas[0];
        await AbrirExercicio(TipoExercicio.Alternativas, exAtual);

        exAtual = exerciciosSpeaking[1];
        await AbrirExercicio(TipoExercicio.Speaking, exAtual);

        await PlayAudioETexto(i++, mostrarLegenda: true);

        exAtual = exerciciosSpeaking[2];
        await AbrirExercicio(TipoExercicio.Speaking, exAtual);

        await PlayAudioETexto(i++, mostrarLegenda: true);
        exAtual = exerciciosSpeaking[3];
        await AbrirExercicio(TipoExercicio.Speaking, exAtual);

        await PlayAudioETexto(i++, mostrarLegenda: true);
        exAtual = exerciciosAlternativas[1];
        await AbrirExercicio(TipoExercicio.Alternativas, exAtual);

        await PlayAudioETexto(i++, mostrarLegenda: true);
    }

    private async Task interacaoB1()
    {
        int i = 0;
        //usuario deve interagir com o caixa para colocar os produtos primeiro, e depois começa a conversa automaticamente
        await PlayAudioETexto(i++, mostrarLegenda: true);

        exAtual = exerciciosSpeaking[0];
        await AbrirExercicio(TipoExercicio.Speaking, exAtual);

        await PlayAudioETexto(i++, mostrarLegenda: true);

        exAtual = exerciciosAlternativas[0];
        await AbrirExercicio(TipoExercicio.Alternativas, exAtual);

        await PlayAudioETexto(i++, mostrarLegenda: true);

        exAtual = exerciciosSpeaking[1];
        await AbrirExercicio(TipoExercicio.Speaking, exAtual);

        await PlayAudioETexto(i++, mostrarLegenda: true);

        exAtual = exerciciosSpeaking[2];
        await AbrirExercicio(TipoExercicio.Speaking, exAtual);

        await PlayAudioETexto(i++, mostrarLegenda: true);

        exAtual = exerciciosSpeaking[3];
        await AbrirExercicio(TipoExercicio.Speaking, exAtual);

        await PlayAudioETexto(i++, mostrarLegenda: true);
    }

    public async Task AbrirExercicio(TipoExercicio tipo, ExercicioBase ex)
    {
        Debug.Log("Abrindo exercício: " + tipo);

        ModalLegenda legenda = modalLegenda.GetComponent<ModalLegenda>();

        if (modalLegenda != null)
        {
            modalLegenda.SetActive(false);
        }

        if (modalExercicio != null)
        {
            modalExercicio.titulo.text = ex.enunciado;
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

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (legenda != null)
        {
            legenda.AjustarPosicaoPeloEstadoDoJogo();
        }
    }

    private async Task FazerRevisao()
    {
        if (RevisaoManager.Instance != null)
        {
            if (RevisaoManager.Instance.TemRevisao())
            {
                if (modalAvisoRevisao != null)
                {
                    modalAvisoRevisao.SetActive(true);

                    while (modalAvisoRevisao.activeSelf)
                    {
                        await Task.Yield();
                    }
                }

                List<ExercicioRevisao> revisoes = RevisaoManager.Instance.GetExerciciosErrados();

                foreach (ExercicioRevisao item in revisoes)
                {
                    string enunciadoOriginal = item.exercicio.enunciado;

                    if (item.tipo == TipoExercicio.Speaking || item.tipo == TipoExercicio.Alternativas)
                    {
                        item.exercicio.enunciado =
                            "Context: " + item.contextoNpc + "\n\n" + enunciadoOriginal;
                    }

                    await AbrirExercicioRevisao(item.tipo, item.exercicio);

                    item.exercicio.enunciado = enunciadoOriginal;
                }

                RevisaoManager.Instance.Limpar();
            }
        }
    }

    private async Task AbrirExercicioRevisao(TipoExercicio tipo, ExercicioBase ex)
    {
        if (modalLegenda != null)
        {
            modalLegenda.SetActive(false);
        }

        if (modalExercicio != null)
        {
            modalExercicio.Abrir(tipo, ex, this, true);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        while (!modalExercicio.exercicioFinalizado)
        {
            await Task.Yield();
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private async Task MostrarProdutosNoBalcao()
    {
        if (checkoutVisualJaFeito) return;

        checkoutVisualJaFeito = true;

        if (sacolaFinal != null)
            sacolaFinal.SetActive(false);

        produtosInstanciadosNoCaixa.Clear();

        List<TipoProdutoMercado> produtosPegos = ProdutoManager.Instance.GetProdutosPegos();

        for (int i = 0; i < produtosPegos.Count; i++)
        {
            if (i >= pontosProdutosBalcao.Count) break;

            GameObject prefab = ProdutoManager.Instance.GetPrefabProduto(produtosPegos[i]);

            if (prefab == null) continue;

            GameObject produtoVisual = Instantiate(
                prefab,
                pontosProdutosBalcao[i].position,
                pontosProdutosBalcao[i].rotation
            );

            produtosInstanciadosNoCaixa.Add(produtoVisual);
        }

        await Task.Delay((int)(tempoProdutosNoBalcao * 1000));

        foreach (GameObject obj in produtosInstanciadosNoCaixa)
        {
            if (obj != null)
                Destroy(obj);
        }

        produtosInstanciadosNoCaixa.Clear();

        if (sacolaFinal != null)
        {
            sacolaFinal.SetActive(true);

            SacolaInterativaScript sacola = sacolaFinal.GetComponent<SacolaInterativaScript>();
            if (sacola == null)
                sacola = sacolaFinal.AddComponent<SacolaInterativaScript>();

            sacola.Configurar(this);

            Collider col = sacolaFinal.GetComponent<Collider>();
            if (col != null)
                col.enabled = true;

            sacolaFinal.layer = LayerMask.NameToLayer("Interativo");
        }
    }

    public void SacolaFoiPega()
    {
        sacolaFoiPega = true;
    }

    private async Task CaminharAteDestino(Transform destino)
    {
        if (navMeshAgent == null || destino == null) 
        {
            return;
        }

        navMeshAgent.SetDestination(destino.position);
        
        // Liga a animação de andar se você tiver uma configurada no seu Animator
        if (animator != null) 
        {
            animator.SetBool("IsWalking", true);
        }

        // Aguarda até que o agente chegue bem perto do destino
        while (navMeshAgent.pathPending || navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
        {
            await Task.Yield();
        }

        // Desliga a animação de andar ao chegar
        if (animator != null) animator.SetBool("IsWalking", false); 
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
            // Usa o nome dinâmico do NPC na legenda
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

        await Task.Yield();

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
    public override bool PodeInteragir()
    {
        if (!base.PodeInteragir())
            return false;

        if (GameProgress.Instance == null)
        {
            Debug.LogWarning("GameProgress.Instance está null. Adicione GameProgress em um GameObject da cena.");
            return false;
        }

        if (aguardandoPegarSacola)
            return false;

        return GameProgress.Instance.PodeFalarComCaixa();
    }
}