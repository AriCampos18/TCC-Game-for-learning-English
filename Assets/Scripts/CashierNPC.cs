// using System.Collections.Generic;
// using System.Threading.Tasks;
// using UnityEngine;
// using UnityEngine.Networking;
// using System.IO;
// using TMPro;
// using System;

// public class CashierNPC : InteracaoNPC
// {
//     int horaAtual = DateTime.Now.Hour;

//     string nivelAtual;
//     public GameObject modalAvisoRevisao;
//     private bool missaoSacolaLiberada = false;
//     public GameObject produtoChocolate;
//     List<string> dialogoAtual;
    
//     public TextMeshProUGUI textoPularDialogo;
//     string cumprimento;

//     public string idNpcParaVoz = "cashier"; // Altere no Inspector para "homem_caixa" ou "mulher_padaria"
//     double valorTotal;
//     private AudioSource audioSource;
//     public ModalExercicio modalExercicio;
//     private BackendManager backendManager;

//     [System.NonSerialized] public List<ExercicioBase> exerciciosCompletarFrase;
//     [System.NonSerialized] public List<ExercicioBase> exerciciosCriarFrases;
//     [System.NonSerialized] public List<ExercicioBase> exerciciosSpeaking;
//     [System.NonSerialized] public List<ExercicioBase> exerciciosAlternativas;
//     [System.NonSerialized] public List<ExercicioBase> exerciciosWriting;
//     [System.NonSerialized] public List<ExercicioBase> exerciciosAssociacao;

//     public List<Transform> pontosProdutosBalcao;
//     public GameObject sacolaFinal;
//     private bool aguardandoPegarSacola = false;
//     private bool sacolaFoiPega = false;
//     public float tempoProdutosNoBalcao = 3f;

//     private List<GameObject> produtosInstanciadosNoCaixa = new List<GameObject>();
//     private bool checkoutVisualJaFeito = false;

//     public ExercicioBase exAtual;
//     public GameObject modalLegenda;

//     private Vector3 posicaoInicialCashier;
//     private Quaternion rotacaoInicialCashier;

//     protected override void Start()
//     {
//         base.Start();
//         posicaoInicialCashier = transform.position;
//         rotacaoInicialCashier = transform.rotation;
//         nomeExibicaoLegenda = "Cashier Attendant";
//         backendManager = new BackendManager();
//         audioSource = GetComponent<AudioSource>();
//         exerciciosCompletarFrase = new List<ExercicioBase>();
//         exerciciosCriarFrases = new List<ExercicioBase>();
//         exerciciosSpeaking = new List<ExercicioBase>();
//         exerciciosAlternativas = new List<ExercicioBase>();
//         exerciciosWriting = new List<ExercicioBase>();
//         exerciciosAssociacao = new List<ExercicioBase>();
//         nivelAtual = DadosJogador.nivelUsuario;

//         if (horaAtual < 12)
//         {
//             cumprimento = "Good morning!";
//         }
//         else if (horaAtual < 18)
//         {
//             cumprimento = "Good afternoon!";
//         }
//         else
//         {
//             cumprimento = "Good evening!";
//         }
//     }

//     private void InicializarConteudosPorNivel()
//     {
//         if (nivelAtual == "A1")
//         {
//             valorTotal = 10.00;
//             dialogoAtual = new List<string>()
//             {
//                 cumprimento,
//                 "Do you want anything else?",
//                 "Here is your chocolate. The total is ten dollars.",
//                 "What are you paying with today?",
//                 "Thank you.",
//             };

//             exerciciosSpeaking.Add(
//                 new ExercicioSpeaking()
//                 {
//                     enunciado = "Return the greeting from the cashier:"
//                 }
//             );

//             exerciciosCriarFrases.Add(
//                 new ExercicioBlocos()
//                 { 
//                     enunciado = "Arrange the words to form the correct sentence in english: Sim, eu quero algum chocolate.",
//                     blocosPalavras = new List<string>() { "No", "Yes", "thank", "you", "that", "chocolate", "was", "want", "all", "hello", "thank", "I", "maybe", "some" },
//                     respostaCorreta = "Yes, I want some chocolate\n",
//                 }
//             );

//             exerciciosAlternativas.Add(
//                 new ExercicioAlternativas()
//                 {
//                     enunciado = "What is the total amount?",
//                     alternativas = new List<string>() { "3 dollars\r\n", "19 dollars\r\n", "10 dollars\r\n", "16 dollars\r\n" },
//                     alternativaCorreta = 2
//                 });

//             exerciciosWriting.Add(
//                 new ExercicioWriting()
//                 {
//                     enunciado = "Write a short sentence saying how you would like to pay for your order."
//                 }
//             );

//             exerciciosAssociacao.Add(
//                 new ExercicioAssociacao()
//                 {
//                     enunciado = "Match the English words with their Portuguese translations:",
//                     opcoesPort = new List<string>()
//                     {
//                         "Chocolate",
//                         "Leite",
//                         "Pão",
//                         "Maçã",
//                         "Sacola",
//                         "Dinheiro",
//                         "Fruta",
//                         "Suco"
//                     },
//                     opcoesIngles = new List<string>()
//                     {
//                         "Money",
//                         "Apple",
//                         "Juice",
//                         "Bag",
//                         "Bread",
//                         "Chocolate",
//                         "Fruit",
//                         "Milk"
//                     },
//                     associacaoCorreta = new List<ParAssociacao>()
//                     {
//                         new ParAssociacao() { palavraPortugues = "Chocolate", palavraIngles = "Chocolate" },
//                         new ParAssociacao() { palavraPortugues = "Leite", palavraIngles = "Milk" },
//                         new ParAssociacao() { palavraPortugues = "Pão", palavraIngles = "Bread" },
//                         new ParAssociacao() { palavraPortugues = "Maçã", palavraIngles = "Apple" },
//                         new ParAssociacao() { palavraPortugues = "Sacola", palavraIngles = "Bag" },
//                         new ParAssociacao() { palavraPortugues = "Dinheiro", palavraIngles = "Money" },
//                         new ParAssociacao() { palavraPortugues = "Fruta", palavraIngles = "Fruit" },
//                         new ParAssociacao() { palavraPortugues = "Suco", palavraIngles = "Juice" }
//                     }
//                 }
//             );
//         }
//         else if (nivelAtual == "A2")
//         {
//             valorTotal = 14.50;
//             dialogoAtual = new List<string>()
//             {
//                 cumprimento,
//                 "Your total is fourteen dollars and fifty cents.",
//                 "Do you usually bring your own bags, or do you need some today?",
//                 "Can I help you with anything else, or do you already have everything you need?",
//                 "How are you paying today: cash or card?",
//                 "Thank you."
//             };

//             exerciciosSpeaking.Add(
//                 new ExercicioSpeaking()
//                 {
//                     enunciado = "Repeat the cashier greeting:",
//                     opcoesFala = new List<string>() { cumprimento+"\n" },
//                     respostaCorreta = 0
//                 }
//             );

//             exerciciosAlternativas.Add(
//                 new ExercicioAlternativas()
//                 {
//                     enunciado = "What is the total amount?",
//                     alternativas = new List<string>() { "14.50 dollars\r\n", "40.50 dollars\r\n", "4.05 dollars\r\n", "4.50 dollars\r\n" },
//                     alternativaCorreta = 0
//                 });

//             exerciciosSpeaking.Add(
//                 new ExercicioSpeaking()
//                 {
//                     enunciado = "Say the total purchase price: "+valorTotal+"\n",
//                     opcoesFala = new List<string>() { valorTotal+"\n" },
//                     respostaCorreta = 0
//                 }
//             );

//             exerciciosCriarFrases.Add(
//                 new ExercicioBlocos()
//                 {
//                     enunciado = "Arrange the words to form the correct sentence in english: Eu geralmente trago minhas próprias sacolas, mas hoje preciso de uma.",
//                     blocosPalavras = new List<string>()
//                     {
//                         "today", "bags", "he", "usually", "my", "need",
//                         "bring", "but", "own", "one", "always", "I",
//                         "don't", "need", "yesterday", "your"
//                     },
//                     respostaCorreta = "I usually bring my own bags, but I need one today."
//                 }
//             );

//             exerciciosSpeaking.Add(
//                 new ExercicioSpeaking()
//                 {
//                     enunciado = "Say the right sentence to respond to the cashier:",
//                     opcoesFala = new List<string>() { "There are many cars in the parking lot.\n", "He is standing between the shelves.\n", "No, I already have everything I need.\n" },
//                     respostaCorreta = 2
//                 }
//             );

//             exerciciosAlternativas.Add(
//                 new ExercicioAlternativas()
//                 {
//                     enunciado = "Answer choosing the right sentence:",
//                     alternativas = new List<string>() { "We are watching a movie tonight.\r\n", "He doesn’t drink coffee either.\r\n", "I’m paying with my debit card.\r\n", "There is a long line at the bakery.\r\n" },
//                     alternativaCorreta = 2
//                 });
                      
//             exerciciosAssociacao.Add(
//                 new ExercicioAssociacao()
//                 {
//                     enunciado = "Match the English words with their Portuguese translations:",
//                     opcoesPort = new List<string>()
//                     {
//                         "Cartão de débito",
//                         "Cartão de crédito",
//                         "Recibo",
//                         "Sacola reutilizável",
//                         "Desconto",
//                         "Prateleira",
//                         "Cereal",
//                         "Lanche",
//                         "Corredor",
//                         "Total"
//                     },
//                     opcoesIngles = new List<string>()
//                     {
//                         "Receipt",
//                         "Snack",
//                         "Discount",
//                         "Shelf",
//                         "Debit card",
//                         "Total",
//                         "Aisle",
//                         "Reusable bag",
//                         "Cereal",
//                         "Credit card"
//                     },
//                     associacaoCorreta = new List<ParAssociacao>()
//                     {
//                         new ParAssociacao() { palavraPortugues = "Cartão de débito", palavraIngles = "Debit card" },
//                         new ParAssociacao() { palavraPortugues = "Cartão de crédito", palavraIngles = "Credit card" },
//                         new ParAssociacao() { palavraPortugues = "Recibo", palavraIngles = "Receipt" },
//                         new ParAssociacao() { palavraPortugues = "Sacola reutilizável", palavraIngles = "Reusable bag" },
//                         new ParAssociacao() { palavraPortugues = "Desconto", palavraIngles = "Discount" },
//                         new ParAssociacao() { palavraPortugues = "Prateleira", palavraIngles = "Shelf" },
//                         new ParAssociacao() { palavraPortugues = "Cereal", palavraIngles = "Cereal" },
//                         new ParAssociacao() { palavraPortugues = "Lanche", palavraIngles = "Snack" },
//                         new ParAssociacao() { palavraPortugues = "Corredor", palavraIngles = "Aisle" },
//                         new ParAssociacao() { palavraPortugues = "Total", palavraIngles = "Total" }
//                     }
//                 }
//             );
//         }
//         else
//         {
//             valorTotal = 16.75;
//             dialogoAtual = new List<string>()
//             { 
//                 cumprimento+" Did you find everything you were looking for today?",
//                 "Your total is sixteen dollars and seventy-five cents.",
//                 "We have reusable bags near the register, or would you prefer paper bags instead?",
//                 "What payment method are you using today, and would you also like a printed receipt?",
//                 "Thank you for shopping with us. Have a great day!"
//             };

//             exerciciosSpeaking.Add(
//                 new ExercicioSpeaking()
//                 {
//                     enunciado = "Repeat the cashier's greeting and give the right answer:",
//                     opcoesFala = new List<string>() { cumprimento+". Yes, I found everything I needed, thank you.\n", cumprimento+". I like chocolate cake and pizza.\n", cumprimento+". The cat is under the table.\n"},
//                     respostaCorreta = 0
//                 }
//             );

//             exerciciosAlternativas.Add(
//                 new ExercicioAlternativas()
//                 {
//                     enunciado = "What is the total amount?",
//                     alternativas = new List<string>() { "14.50 dollars\r\n", "16.50 dollars\r\n", "60.75 dollars\r\n", "16.75 dollars\r\n" },
//                     alternativaCorreta = 3
//                 });

//             exerciciosSpeaking.Add(
//                 new ExercicioSpeaking()
//                 {
//                     enunciado = "Say the total amount for the purchase: " + valorTotal + "\n",
//                     opcoesFala = new List<string>() { valorTotal+"\n" },
//                     respostaCorreta = 0
//                 }
//             );

//             exerciciosSpeaking.Add(
//                 new ExercicioSpeaking()
//                 {
//                     enunciado = "Say the right sentence to respond the cashier:",
//                     opcoesFala = new List<string>() { "She was reading a magazine in the waiting room earlier.\n", "It’s fine, I prefer the reusable bags near the register.\n", "I usually exercise at the gym three times a week.\n" },
//                     respostaCorreta = 1
//                 }
//             );

//             exerciciosCompletarFrase.Add(
//                 new ExercicioCompletarFrase()
//                 {
//                     enunciado = "Complete the sentence with the correct words:",
//                     partesFrase = new List<string>
//                     {
//                         "I would like to pay with my ",
//                         " ",
//                         " card, and I would also like a printed",
//                         " ",
//                         "."
//                     }, 
//                     respostaCorreta = "debit, receipt",                   
//                     opcoes = new List<string>()
//                     {
//                         "receipt", "debit", "cash", "my",
//                         "card", "with", "I'd", "too", "paying",
//                         "gift", "the", "I'm", "like", "money",
//                         "coins", "check", "want", "your", "buy"
//                     }
//                 }
//             );
 
//             exerciciosAssociacao.Add(
//                 new ExercicioAssociacao()
//                 {
//                     enunciado = "Match the English words with their Portuguese translations:",
//                     opcoesPort = new List<string>()
//                     {
//                         "Recém-assado",
//                         "Esgotado",
//                         "Método de pagamento",
//                         "Recibo impresso",
//                         "Caixa registradora",
//                         "Estacionamento",
//                         "Produtos congelados",
//                         "Prateleira",
//                         "Desconto especial",
//                         "Alimentos saudáveis",
//                         "Fazer um pedido",
//                         "Sacola de papel"
//                     },
//                     opcoesIngles = new List<string>()
//                     {
//                         "Frozen food",
//                         "Payment method",
//                         "Paper bag",
//                         "Freshly baked",
//                         "To place an order",
//                         "Parking lot",
//                         "Sold out",
//                         "Printed receipt",
//                         "Healthy food",
//                         "Cash register",
//                         "Special discount",
//                         "Shelf"
//                     },
//                     associacaoCorreta = new List<ParAssociacao>()
//                     {
//                         new ParAssociacao() { palavraPortugues = "Recém-assado", palavraIngles = "Freshly baked" },
//                         new ParAssociacao() { palavraPortugues = "Esgotado", palavraIngles = "Sold out" },
//                         new ParAssociacao() { palavraPortugues = "Método de pagamento", palavraIngles = "Payment method" },
//                         new ParAssociacao() { palavraPortugues = "Recibo impresso", palavraIngles = "Printed receipt" },
//                         new ParAssociacao() { palavraPortugues = "Caixa registradora", palavraIngles = "Cash register" },
//                         new ParAssociacao() { palavraPortugues = "Estacionamento", palavraIngles = "Parking lot" },
//                         new ParAssociacao() { palavraPortugues = "Produtos congelados", palavraIngles = "Frozen food" },
//                         new ParAssociacao() { palavraPortugues = "Prateleira", palavraIngles = "Shelf" },
//                         new ParAssociacao() { palavraPortugues = "Desconto especial", palavraIngles = "Special discount" },
//                         new ParAssociacao() { palavraPortugues = "Alimentos saudáveis", palavraIngles = "Healthy food" },
//                         new ParAssociacao() { palavraPortugues = "Fazer um pedido", palavraIngles = "To place an order" },
//                         new ParAssociacao() { palavraPortugues = "Sacola de papel", palavraIngles = "Paper bag" }
//                     }
//                 }
//             );
//         }
//     }

//     private TipoExercicio ObterTipoExercicio(ExercicioBase exercicio)
//     {
//         if (exercicio is ExercicioSpeaking)
//             return TipoExercicio.Speaking;
//         if (exercicio is ExercicioAlternativas)
//             return TipoExercicio.Alternativas;
//         if (exercicio is ExercicioBlocos)
//             return TipoExercicio.Blocos;
//         if (exercicio is ExercicioCompletarFrase)
//             return TipoExercicio.CompletarFrase;
//         if (exercicio is ExercicioWriting)
//             return TipoExercicio.Writing;
//         if (exercicio is ExercicioAssociacao)
//             return TipoExercicio.Associacao;

//         throw new InvalidOperationException("Tipo de exercício não suportado: " + exercicio.GetType().Name);
//     }

//     protected override async Task IniciarInteracao()
//     {
//         if (GameProgress.Instance.PodeFalarComCaixa())
//         {
//             GameProgress.EstaEmDialogo = true;

//             nivelAtual = DadosJogador.nivelUsuario;
//             InicializarConteudosPorNivel();

//             await MostrarProdutosNoBalcao();

//             if (nivelAtual == "A1")
//                 await interacaoA1();
//             else if (nivelAtual == "A2")
//                 await interacaoA2();
//             else
//                 await interacaoB1();

//             MissionManager.Instance.ConcluirMissao("falar_caixa");

//             if (!await FazerRevisao())
//             {
//                 GameProgress.EstaEmDialogo = false;
//                 LiberarControlePlayer();
//                 return;
//             }

//             MissionManager.Instance.AdicionarMissao(
//                 "pegar_sacola",
//                 "Pick up your bag at the checkout and leave the market",
//                 "Pegue a sacola no caixa e saia do mercado"
//             );

//             missaoSacolaLiberada = true;

//             aguardandoPegarSacola = true;
//             sacolaFoiPega = false;

//             GameProgress.EstaEmDialogo = false;
            
//             if (modalLegenda != null)
//                 modalLegenda.SetActive(false);

//             if (textoPularDialogo != null)
//                 textoPularDialogo.gameObject.SetActive(false);

//             GameProgress.EstaEmDialogo = false;

//             LiberarControlePlayer();

//             while (!sacolaFoiPega)
//             {
//                 await Task.Yield();
//             }

//             aguardandoPegarSacola = false;
//         }
//     }

//     private async Task interacaoA1()
//     {
//         int i = 0;
//         // Usuário deve interagir com o caixa para colocar os produtos primeiro, e depois começa a conversa automaticamente

//         await PlayAudioETexto(i++, mostrarLegenda: false); 

//         exAtual = exerciciosSpeaking[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

//         await PlayAudioETexto(i++, mostrarLegenda: true);  
        
//         // Abre o exercício de blocos ("Yes, I want some chocolate")
//         await AbrirExercicio(ObterTipoExercicio(exerciciosCriarFrases[0]), exerciciosCriarFrases[0]);  //

//         await PegarChocolate();

//         await PlayAudioETexto(i++);  
        
//         exAtual = exerciciosAlternativas[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //
//         await PlayAudioETexto(i++, mostrarLegenda: true);

//         exAtual = exerciciosWriting[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         await PlayAudioETexto(i++, mostrarLegenda: true);

//         await AbrirExercicio(ObterTipoExercicio(exerciciosAssociacao[0]), exerciciosAssociacao[0]);
//     }

//     private async Task interacaoA2() 
//     {
//         int i = 0;
//         //usuario deve interagir com o caixa para colocar os produtos primeiro, e depois começa a conversa automaticamente
        
//         await PlayAudioETexto(i++, mostrarLegenda: false); 

//         exAtual = exerciciosSpeaking[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

//         await PlayAudioETexto(i++, mostrarLegenda: true);

//         exAtual = exerciciosAlternativas[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

//         exAtual = exerciciosSpeaking[1];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

//         await PlayAudioETexto(i++, mostrarLegenda: true);

//         exAtual = exerciciosCriarFrases[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

//         await PlayAudioETexto(i++, mostrarLegenda: true);

//         exAtual = exerciciosSpeaking[2];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

//         await PlayAudioETexto(i++, mostrarLegenda: true);

//         exAtual = exerciciosAlternativas[1];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

//         await PlayAudioETexto(i++, mostrarLegenda: true);

//         await AbrirExercicio(ObterTipoExercicio(exerciciosAssociacao[0]), exerciciosAssociacao[0]);
//     }

//     private async Task interacaoB1()
//     {
//         int i = 0;
//         //usuario deve interagir com o caixa para colocar os produtos primeiro, e depois começa a conversa automaticamente
//         await PlayAudioETexto(i++, mostrarLegenda: true);

//         exAtual = exerciciosSpeaking[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

//         await PlayAudioETexto(i++, mostrarLegenda: true);

//         exAtual = exerciciosAlternativas[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

//         exAtual = exerciciosSpeaking[1];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

//         await PlayAudioETexto(i++, mostrarLegenda: true);

//         exAtual = exerciciosSpeaking[2];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

//         await PlayAudioETexto(i++, mostrarLegenda: true);

//         exAtual = exerciciosCompletarFrase[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

//         await PlayAudioETexto(i++, mostrarLegenda: true);

//         await AbrirExercicio(ObterTipoExercicio(exerciciosAssociacao[0]), exerciciosAssociacao[0]);
//     }

//     private async Task PegarChocolate()
//     {
//         await Task.Delay(1500);

//         if (produtoChocolate != null)
//         {
//             produtoChocolate.SetActive(false);
//         }
//     }

//     public async Task AbrirExercicio(TipoExercicio tipo, ExercicioBase ex)
//     {
//         Debug.Log("Abrindo exercício: " + tipo);

//         ModalLegenda legenda = modalLegenda.GetComponent<ModalLegenda>();

//         if (modalLegenda != null)
//             modalLegenda.SetActive(false);

//         transform.position = posicaoInicialCashier;
//         transform.rotation = rotacaoInicialCashier;

//         await EntrarModoExercicio();

//         Vector3 posicaoExercicio = transform.position;
//         Quaternion rotacaoExercicio = transform.rotation;

//         Cursor.lockState = CursorLockMode.None;
//         Cursor.visible = true;

//         modalExercicio.Abrir(tipo, ex, this);

//         if (legenda != null)
//             legenda.AjustarPosicaoPeloEstadoDoJogo();

//         while (!modalExercicio.exercicioFinalizado)
//         {
//             transform.position = posicaoExercicio;
//             transform.rotation = rotacaoExercicio;
//             await Task.Yield();
//         }

//         if (modalLegenda != null)
//             modalLegenda.SetActive(false);

//         await SairModoExercicio();

//         Cursor.lockState = CursorLockMode.Locked;
//         Cursor.visible = false;

//         if (legenda != null)
//             legenda.AjustarPosicaoPeloEstadoDoJogo();
//     }
//     private Task<bool> FazerRevisao()
//     {
//         if (RevisaoManager.Instance == null)
//         {
//             Debug.LogError("RevisaoManager não está disponível para revisar o Market.");
//             return Task.FromResult(false);
//         }

//         return RevisaoManager.Instance.FazerRevisao(
//             this,
//             modalExercicio,
//             modalAvisoRevisao,
//             modalLegenda
//         );
//     }

//     private async Task MostrarProdutosNoBalcao()
//     {
//         if (checkoutVisualJaFeito) return;

//         checkoutVisualJaFeito = true;

//         if (sacolaFinal != null)
//             sacolaFinal.SetActive(false);

//         produtosInstanciadosNoCaixa.Clear();

//         List<TipoProdutoMercado> produtosPegos = ProdutoManager.Instance.GetProdutosPegos();

//         for (int i = 0; i < produtosPegos.Count; i++)
//         {
//             if (i >= pontosProdutosBalcao.Count) break;

//             GameObject prefab = ProdutoManager.Instance.GetPrefabProduto(produtosPegos[i]);

//             if (prefab == null) continue;

//             GameObject produtoVisual = Instantiate(
//                 prefab,
//                 pontosProdutosBalcao[i].position,
//                 pontosProdutosBalcao[i].rotation
//             );

//             produtosInstanciadosNoCaixa.Add(produtoVisual);
//         }

//         await Task.Delay((int)(tempoProdutosNoBalcao * 1000));

//         foreach (GameObject obj in produtosInstanciadosNoCaixa)
//         {
//             if (obj != null)
//                 Destroy(obj);
//         }

//         produtosInstanciadosNoCaixa.Clear();

//         if (sacolaFinal != null)
//         {
//             sacolaFinal.SetActive(true);

//             SacolaInterativaScript sacola = sacolaFinal.GetComponent<SacolaInterativaScript>();
//             if (sacola == null)
//                 sacola = sacolaFinal.AddComponent<SacolaInterativaScript>();

//             sacola.Configurar(this);

//             Collider col = sacolaFinal.GetComponent<Collider>();
//             if (col != null)
//                 col.enabled = true;

//             sacolaFinal.layer = LayerMask.NameToLayer("Interativo");
//         }
//     }

//     public void SacolaFoiPega()
//     {
//         sacolaFoiPega = true;
//     }

//     private async Task PlayAudioETexto(int i, bool mostrarLegenda = true)
// {
//     ultimaFraseDita = dialogoAtual[i];
//     byte[] audioBytes = await backendManager.GerarAudio(dialogoAtual[i], idNpcParaVoz);
    
//     if (audioBytes != null && audioBytes.Length > 0)
//     {
//         string caminho = Path.Combine(Application.persistentDataPath, "audio_temp.wav");
//         File.WriteAllBytes(caminho, audioBytes);
        
//         UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + caminho, AudioType.WAV);
//         var operation = www.SendWebRequest();
        
//         while (!operation.isDone)
//             await Task.Yield();

//         AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
//         audioSource.clip = clip;

//         if (mostrarLegenda)
//         {
//             textoPularDialogo.gameObject.SetActive(false);
//             // Usa o nome dinâmico do NPC na legenda
//             modalLegenda.GetComponent<ModalLegenda>().MostrarLegenda(nomeExibicaoLegenda, dialogoAtual[i]);
//         }

//         audioSource.Play();
//         while (audioSource.isPlaying)
//         {
//             await Task.Yield();
//         }

//         if (mostrarLegenda)
//         {
//             if (textoPularDialogo != null)
//             {
//                 textoPularDialogo.gameObject.SetActive(true);

//                 Cursor.lockState = CursorLockMode.None;
//                 Cursor.visible = true;
//             }
//         }

//         if (mostrarLegenda)
//         {
//             await Task.Yield();

//             bool clicou = false;
//             while (!clicou)
//             {
//                 if (Input.GetMouseButtonDown(0)) 
//                 {
//                     clicou = true; 
//                 }
//                 else
//                 {
//                     await Task.Yield(); 
//                 }
//             }
//         }

//         if (textoPularDialogo != null)
//         {
//             textoPularDialogo.gameObject.SetActive(false);
//         }

//         if (modalLegenda != null)
//         {
//             modalLegenda.SetActive(false);
//         }
//     }
//     else
//     {
//         Debug.LogError("O servidor retornou um array de bytes vazio.");
//         if (mostrarLegenda)
//         {
//             modalLegenda.GetComponent<ModalLegenda>().MostrarLegenda(nomeExibicaoLegenda, dialogoAtual[i]);
//         }
//     }
// }

//     public override async Task FalarFraseCustomizada(string textoParaFalar)
//     {
//         if (string.IsNullOrEmpty(textoParaFalar)) return;

//         byte[] audioBytes = await backendManager.GerarAudio(textoParaFalar, idNpcParaVoz);
        
//         if (audioBytes != null && audioBytes.Length > 0)
//         {
//             string caminho = Path.Combine(Application.persistentDataPath, "audio_temp_custom.wav");
//             File.WriteAllBytes(caminho, audioBytes);
            
//             UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + caminho, AudioType.WAV);
//             var operation = www.SendWebRequest();
            
//             while (!operation.isDone)
//                 await Task.Yield();

//             AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
//             audioSource.clip = clip;
//             if (modalLegenda != null && modalLegenda.activeSelf)
//             {
//                 if (textoPularDialogo != null) textoPularDialogo.gameObject.SetActive(false);
//                 modalLegenda.GetComponent<ModalLegenda>().MostrarLegenda(nomeExibicaoLegenda, textoParaFalar);
//             }
            
//             audioSource.Play();
            
//             while (audioSource.isPlaying)
//             {
//                 await Task.Yield();
//             }
//         }
//     }
//     public override bool PodeInteragir()
//     {
//         if (!base.PodeInteragir())
//             return false;

//         if (GameProgress.Instance == null)
//         {
//             Debug.LogWarning("GameProgress.Instance está null. Adicione GameProgress em um GameObject da cena.");
//             return false;
//         }

//         if (aguardandoPegarSacola)
//             return false;

//         if (ProdutoManager.Instance == null)
//             return false;

//         if (!ProdutoManager.Instance.ProdutosObrigatoriosPegos())
//             return false;

//         return GameProgress.Instance.PodeFalarComCaixa();
//     }

//     public bool JogadorPegouSacola()
//     {
//         return sacolaFoiPega;
//     }

//     public bool MissaoSacolaLiberada()
//     {
//         return missaoSacolaLiberada;
//     }
// }

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
    private bool missaoSacolaLiberada = false;
    public GameObject produtoChocolate;
    List<string> dialogoAtual;
    
    public TextMeshProUGUI textoPularDialogo;
    string cumprimento;

    public string idNpcParaVoz = "cashier"; // Altere no Inspector para "homem_caixa" ou "mulher_padaria"
    double valorTotal;
    private AudioSource audioSource;
    public ModalExercicio modalExercicio;
    private BackendManager backendManager;

    [System.NonSerialized] public List<ExercicioBase> exerciciosCompletarFrase;
    [System.NonSerialized] public List<ExercicioBase> exerciciosCriarFrases;
    [System.NonSerialized] public List<ExercicioBase> exerciciosSpeaking;
    [System.NonSerialized] public List<ExercicioBase> exerciciosAlternativas;
    [System.NonSerialized] public List<ExercicioBase> exerciciosWriting;
    [System.NonSerialized] public List<ExercicioBase> exerciciosAssociacao;

    public List<Transform> pontosProdutosBalcao;
    public GameObject sacolaFinal;
    private bool aguardandoPegarSacola = false;
    private bool sacolaFoiPega = false;
    public float tempoProdutosNoBalcao = 3f;

    private List<GameObject> produtosInstanciadosNoCaixa = new List<GameObject>();
    private bool checkoutVisualJaFeito = false;

    public ExercicioBase exAtual;
    public GameObject modalLegenda;

    private Vector3 posicaoInicialCashier;
    private Quaternion rotacaoInicialCashier;

    protected override void Start()
    {
        base.Start();
        posicaoInicialCashier = transform.position;
        rotacaoInicialCashier = transform.rotation;
        nomeExibicaoLegenda = "Cashier Attendant";
        backendManager = new BackendManager();
        audioSource = GetComponent<AudioSource>();
        exerciciosCompletarFrase = new List<ExercicioBase>();
        exerciciosCriarFrases = new List<ExercicioBase>();
        exerciciosSpeaking = new List<ExercicioBase>();
        exerciciosAlternativas = new List<ExercicioBase>();
        exerciciosWriting = new List<ExercicioBase>();
        exerciciosAssociacao = new List<ExercicioBase>();
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
                "Do you want anything else?",
                "Here is your chocolate. The total is ten dollars.",
                "What are you paying with today?",
                "Thank you.",
            };

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "Return the greeting from the cashier:"
                }
            );

            exerciciosCriarFrases.Add(
                new ExercicioBlocos()
                { 
                    enunciado = "Arrange the words to form the correct sentence in english: Sim, eu quero algum chocolate.",
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

            exerciciosWriting.Add(
                new ExercicioWriting()
                {
                    enunciado = "Write a short sentence saying how you would like to pay for your order."
                }
            );

            exerciciosAssociacao.Add(
                new ExercicioAssociacao()
                {
                    enunciado = "Match the English words with their Portuguese translations:",
                    opcoesPort = new List<string>()
                    {
                        "Chocolate",
                        "Leite",
                        "Pão",
                        "Maçã",
                        "Sacola",
                        "Dinheiro",
                        "Fruta",
                        "Suco"
                    },
                    opcoesIngles = new List<string>()
                    {
                        "Money",
                        "Apple",
                        "Juice",
                        "Bag",
                        "Bread",
                        "Chocolate",
                        "Fruit",
                        "Milk"
                    },
                    associacaoCorreta = new List<ParAssociacao>()
                    {
                        new ParAssociacao() { palavraPortugues = "Chocolate", palavraIngles = "Chocolate" },
                        new ParAssociacao() { palavraPortugues = "Leite", palavraIngles = "Milk" },
                        new ParAssociacao() { palavraPortugues = "Pão", palavraIngles = "Bread" },
                        new ParAssociacao() { palavraPortugues = "Maçã", palavraIngles = "Apple" },
                        new ParAssociacao() { palavraPortugues = "Sacola", palavraIngles = "Bag" },
                        new ParAssociacao() { palavraPortugues = "Dinheiro", palavraIngles = "Money" },
                        new ParAssociacao() { palavraPortugues = "Fruta", palavraIngles = "Fruit" },
                        new ParAssociacao() { palavraPortugues = "Suco", palavraIngles = "Juice" }
                    }
                }
            );
        }
        else if (nivelAtual == "A2")
        {
            valorTotal = 14.50;
            dialogoAtual = new List<string>()
            {
                cumprimento,
                "Your total is fourteen dollars and fifty cents.",
                "Do you usually bring your own bags, or do you need some today?",
                "Can I help you with anything else, or do you already have everything you need?",
                "How are you paying today: cash or card?",
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
                    enunciado = "Say the total purchase price: "+valorTotal+"\n",
                    opcoesFala = new List<string>() { valorTotal+"\n" },
                    respostaCorreta = 0
                }
            );

            exerciciosCriarFrases.Add(
                new ExercicioBlocos()
                {
                    enunciado = "Arrange the words to form the correct sentence in english: Eu geralmente trago minhas próprias sacolas, mas hoje preciso de uma.",
                    blocosPalavras = new List<string>()
                    {
                        "today", "bags", "he", "usually", "my", "need",
                        "bring", "but", "own", "one", "always", "I",
                        "don't", "need", "yesterday", "your"
                    },
                    respostaCorreta = "I usually bring my own bags, but I need one today."
                }
            );

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "Say the right sentence to respond to the cashier:",
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
                      
            exerciciosAssociacao.Add(
                new ExercicioAssociacao()
                {
                    enunciado = "Match the English words with their Portuguese translations:",
                    opcoesPort = new List<string>()
                    {
                        "Cartão de débito",
                        "Cartão de crédito",
                        "Recibo",
                        "Sacola reutilizável",
                        "Desconto",
                        "Prateleira",
                        "Cereal",
                        "Lanche",
                        "Corredor",
                        "Total"
                    },
                    opcoesIngles = new List<string>()
                    {
                        "Receipt",
                        "Snack",
                        "Discount",
                        "Shelf",
                        "Debit card",
                        "Total",
                        "Aisle",
                        "Reusable bag",
                        "Cereal",
                        "Credit card"
                    },
                    associacaoCorreta = new List<ParAssociacao>()
                    {
                        new ParAssociacao() { palavraPortugues = "Cartão de débito", palavraIngles = "Debit card" },
                        new ParAssociacao() { palavraPortugues = "Cartão de crédito", palavraIngles = "Credit card" },
                        new ParAssociacao() { palavraPortugues = "Recibo", palavraIngles = "Receipt" },
                        new ParAssociacao() { palavraPortugues = "Sacola reutilizável", palavraIngles = "Reusable bag" },
                        new ParAssociacao() { palavraPortugues = "Desconto", palavraIngles = "Discount" },
                        new ParAssociacao() { palavraPortugues = "Prateleira", palavraIngles = "Shelf" },
                        new ParAssociacao() { palavraPortugues = "Cereal", palavraIngles = "Cereal" },
                        new ParAssociacao() { palavraPortugues = "Lanche", palavraIngles = "Snack" },
                        new ParAssociacao() { palavraPortugues = "Corredor", palavraIngles = "Aisle" },
                        new ParAssociacao() { palavraPortugues = "Total", palavraIngles = "Total" }
                    }
                }
            );
        }
        else if(nivelAtual == "B1")
        {
            valorTotal = 16.75;
            dialogoAtual = new List<string>()
            { 
                cumprimento+" Did you find everything you were looking for today?",
                "Your total is sixteen dollars and seventy-five cents.",
                "We have reusable bags near the register, or would you prefer paper bags instead?",
                "What payment method are you using today, and would you also like a printed receipt?",
                "Thank you for shopping with us. Have a great day!"
            };

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "Repeat the cashier's greeting and give the right answer:",
                    opcoesFala = new List<string>() { cumprimento+". Yes, I found everything I needed, thank you.\n", cumprimento+". I like chocolate cake and pizza.\n", cumprimento+". The cat is under the table.\n"},
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

            exerciciosCompletarFrase.Add(
                new ExercicioCompletarFrase()
                {
                    enunciado = "Complete the sentence with the correct words:",
                    partesFrase = new List<string>
                    {
                        "I would like to pay with my ",
                        " ",
                        " card, and I would also like a printed",
                        " ",
                        "."
                    }, 
                    respostaCorreta = "debit, receipt",                   
                    opcoes = new List<string>()
                    {
                        "receipt", "debit", "cash", "my",
                        "card", "with", "I'd", "too", "paying",
                        "gift", "the", "I'm", "like", "money",
                        "coins", "check", "want", "your", "buy"
                    }
                }
            );
 
            exerciciosAssociacao.Add(
                new ExercicioAssociacao()
                {
                    enunciado = "Match the English words with their Portuguese translations:",
                    opcoesPort = new List<string>()
                    {
                        "Recém-assado",
                        "Esgotado",
                        "Método de pagamento",
                        "Recibo impresso",
                        "Caixa registradora",
                        "Estacionamento",
                        "Produtos congelados",
                        "Prateleira",
                        "Desconto especial",
                        "Alimentos saudáveis",
                        "Fazer um pedido",
                        "Sacola de papel"
                    },
                    opcoesIngles = new List<string>()
                    {
                        "Frozen food",
                        "Payment method",
                        "Paper bag",
                        "Freshly baked",
                        "To place an order",
                        "Parking lot",
                        "Sold out",
                        "Printed receipt",
                        "Healthy food",
                        "Cash register",
                        "Special discount",
                        "Shelf"
                    },
                    associacaoCorreta = new List<ParAssociacao>()
                    {
                        new ParAssociacao() { palavraPortugues = "Recém-assado", palavraIngles = "Freshly baked" },
                        new ParAssociacao() { palavraPortugues = "Esgotado", palavraIngles = "Sold out" },
                        new ParAssociacao() { palavraPortugues = "Método de pagamento", palavraIngles = "Payment method" },
                        new ParAssociacao() { palavraPortugues = "Recibo impresso", palavraIngles = "Printed receipt" },
                        new ParAssociacao() { palavraPortugues = "Caixa registradora", palavraIngles = "Cash register" },
                        new ParAssociacao() { palavraPortugues = "Estacionamento", palavraIngles = "Parking lot" },
                        new ParAssociacao() { palavraPortugues = "Produtos congelados", palavraIngles = "Frozen food" },
                        new ParAssociacao() { palavraPortugues = "Prateleira", palavraIngles = "Shelf" },
                        new ParAssociacao() { palavraPortugues = "Desconto especial", palavraIngles = "Special discount" },
                        new ParAssociacao() { palavraPortugues = "Alimentos saudáveis", palavraIngles = "Healthy food" },
                        new ParAssociacao() { palavraPortugues = "Fazer um pedido", palavraIngles = "To place an order" },
                        new ParAssociacao() { palavraPortugues = "Sacola de papel", palavraIngles = "Paper bag" }
                    }
                }
            );
        }
else if (nivelAtual == "A1-A2")
{
    valorTotal = 12.75;

    dialogoAtual = new List<string>()
    {
        cumprimento + " Did you find everything you needed?",
        "Your total is twelve dollars and seventy-five cents.",
        "Would you like a bag for your groceries?",
        "How would you like to pay: cash or card?",
        "Would you like a printed receipt?",
        "Thank you for shopping with us. Have a nice day!"
    };

    exerciciosSpeaking.Add(new ExercicioSpeaking()
    {
        enunciado = "Respond to the cashier's greeting and confirm that you found everything you needed.",
        opcoesFala = new List<string>()
        {
            cumprimento + " Yes, I found everything I needed, thank you.\r\n",
            "I would like to reserve a table.\r\n",
            "I'm looking for a clothing store.\r\n"
        },
        respostaCorreta = 0
    });

    exerciciosAlternativas.Add(new ExercicioAlternativas()
    {
        enunciado = "The cashier tells you that your total is $12.75. What is the correct amount?",
        alternativas = new List<string>()
        {
            "Twelve dollars and seventy-five cents.",
            "Twenty dollars and fifty cents.",
            "Twelve dollars and fifteen cents.",
            "Seventeen dollars and twenty-five cents."
        },
        alternativaCorreta = 0
    });

    exerciciosSpeaking.Add(new ExercicioSpeaking()
    {
        enunciado = "Say the total amount of your purchase.",
        opcoesFala = new List<string>()
        {
            "Twelve dollars and seventy-five cents.\r\n"
        },
        respostaCorreta = 0
    });

    exerciciosCriarFrases.Add(new ExercicioBlocos()
    {
        enunciado = "Arrange the words to form the correct sentence in English: Eu gostaria de uma sacola para minhas compras, por favor.",
        blocosPalavras = new List<string>()
        {
            "I'd", "like", "a", "bag", "for", "my",
            "groceries", "please", "thank", "you", "today"
        },
        respostaCorreta = "I'd like a bag for my groceries, please."
    });

    exerciciosSpeaking.Add(new ExercicioSpeaking()
    {
        enunciado = "The cashier asks how you would like to pay. Tell them that you would prefer to use your debit card.",
        opcoesFala = new List<string>()
        {
            "I'd prefer to pay with my debit card.\r\n",
            "I'd like to try another size.\r\n",
            "I usually buy cereal on Mondays.\r\n"
        },
        respostaCorreta = 0
    });

    exerciciosAlternativas.Add(new ExercicioAlternativas()
    {
        enunciado = "The cashier asks whether you would like a printed receipt. What should you say if you want one?",
        alternativas = new List<string>()
        {
            "Yes, please. I'd like a receipt.",
            "No, I don't need any milk.",
            "I'd like to change my shopping cart.",
            "The store closes at nine."
        },
        alternativaCorreta = 0
    });
}
else if (nivelAtual == "A2-B1")
{
    valorTotal = 18.40;

    dialogoAtual = new List<string>()
    {
        cumprimento + " Did you manage to find everything you were looking for today?",
        "Your total comes to eighteen dollars and forty cents.",
        "Would you like to use one of our reusable bags, or have you brought your own?",
        "We also have paper bags available if you prefer.",
        "Which payment method would you like to use?",
        "Would you like a printed receipt, or would you prefer a digital one?",
        "Thank you for shopping with us. Have a wonderful day!"
    };

    exerciciosSpeaking.Add(new ExercicioSpeaking()
    {
        enunciado = "Respond to the cashier politely. Confirm that you found everything you needed and thank them."
    });

    exerciciosAlternativas.Add(new ExercicioAlternativas()
    {
        enunciado = "The cashier says that your total comes to $18.40. Which option correctly expresses this amount?",
        alternativas = new List<string>()
        {
            "Eighteen dollars and forty cents.",
            "Eighty dollars and eighteen cents.",
            "Eighteen dollars and fourteen cents.",
            "Fourteen dollars and eighty cents."
        },
        alternativaCorreta = 0
    });

    exerciciosSpeaking.Add(new ExercicioSpeaking()
    {
        enunciado = "Say the total amount of your purchase clearly.",
        opcoesFala = new List<string>()
        {
            "Eighteen dollars and forty cents.\r\n"
        },
        respostaCorreta = 0
    });

    exerciciosSpeaking.Add(new ExercicioSpeaking()
    {
        enunciado = "The cashier asks whether you brought your own bag. Explain that you brought a reusable bag and do not need another one.",
        opcoesFala = new List<string>()
        {
            "I brought my own reusable bag, so I don't need another one.\r\n",
            "I would like to order a sandwich.\r\n",
            "I need to find the dairy section.\r\n"
        },
        respostaCorreta = 0
    });

    exerciciosCriarFrases.Add(new ExercicioBlocos()
    {
        enunciado = "Arrange the words to form the correct sentence in English: Eu prefiro pagar com cartão de débito, se possível.",
        blocosPalavras = new List<string>()
        {
            "I'd", "prefer", "to", "pay", "with", "my",
            "debit", "card", "if", "possible", "please",
            "cash", "instead"
        },
        respostaCorreta = "I'd prefer to pay with my debit card, if possible."
    });

    exerciciosAlternativas.Add(new ExercicioAlternativas()
    {
        enunciado = "The cashier offers you a printed or digital receipt. Which response requests a digital receipt?",
        alternativas = new List<string>()
        {
            "A digital receipt would be great, thank you.",
            "I'd like to pay in cash.",
            "I brought my own shopping bag.",
            "Could you show me the cereal aisle?"
        },
        alternativaCorreta = 0
    });

    exerciciosCompletarFrase.Add(new ExercicioCompletarFrase()
    {
        enunciado = "Complete the sentence with the correct words:",
        partesFrase = new List<string>()
        {
            "I'd prefer to pay with my ",
            " ",
            " card, and I'd like a digital ",
            " ",
            "."
        },
        respostaCorreta = "debit, receipt",
        opcoes = new List<string>()
        {
            "debit", "credit", "cash", "receipt",
            "bag", "money", "paper", "card",
            "payment", "change", "total"
        }
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
            else if(nivelAtual=="A1-A2")
                await interacaoA1A2();
            else if(nivelAtual=="A2-B1")
                await interacaoA2B1();
            else if (nivelAtual == "A2")
                await interacaoA2();
            else
                await interacaoB1();

            MissionManager.Instance.ConcluirMissao("falar_caixa");

            if (!await FazerRevisao())
            {
                GameProgress.EstaEmDialogo = false;
                LiberarControlePlayer();
                return;
            }

            MissionManager.Instance.AdicionarMissao(
                "pegar_sacola",
                "Pick up your bag at the checkout and leave the market",
                "Pegue a sacola no caixa e saia do mercado"
            );

            missaoSacolaLiberada = true;

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
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

        await PlayAudioETexto(i++, mostrarLegenda: true);  
        
        // Abre o exercício de blocos ("Yes, I want some chocolate")
        await AbrirExercicio(ObterTipoExercicio(exerciciosCriarFrases[0]), exerciciosCriarFrases[0]);  //

        await PegarChocolate();

        await PlayAudioETexto(i++);  
        
        exAtual = exerciciosAlternativas[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //
        await PlayAudioETexto(i++, mostrarLegenda: true);

        exAtual = exerciciosWriting[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++, mostrarLegenda: true);

        await AbrirExercicio(ObterTipoExercicio(exerciciosAssociacao[0]), exerciciosAssociacao[0]);
    }

    private async Task interacaoA2() 
    {
        int i = 0;
        //usuario deve interagir com o caixa para colocar os produtos primeiro, e depois começa a conversa automaticamente
        
        await PlayAudioETexto(i++, mostrarLegenda: false); 

        exAtual = exerciciosSpeaking[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

        await PlayAudioETexto(i++, mostrarLegenda: true);

        exAtual = exerciciosAlternativas[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

        exAtual = exerciciosSpeaking[1];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

        await PlayAudioETexto(i++, mostrarLegenda: true);

        exAtual = exerciciosCriarFrases[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

        await PlayAudioETexto(i++, mostrarLegenda: true);

        exAtual = exerciciosSpeaking[2];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

        await PlayAudioETexto(i++, mostrarLegenda: true);

        exAtual = exerciciosAlternativas[1];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++, mostrarLegenda: true);

        await AbrirExercicio(ObterTipoExercicio(exerciciosAssociacao[0]), exerciciosAssociacao[0]);
    }

    private async Task interacaoB1()
    {
        int i = 0;
        //usuario deve interagir com o caixa para colocar os produtos primeiro, e depois começa a conversa automaticamente
        await PlayAudioETexto(i++, mostrarLegenda: true);

        exAtual = exerciciosSpeaking[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

        await PlayAudioETexto(i++, mostrarLegenda: true);

        exAtual = exerciciosAlternativas[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

        exAtual = exerciciosSpeaking[1];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

        await PlayAudioETexto(i++, mostrarLegenda: true);

        exAtual = exerciciosSpeaking[2];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

        await PlayAudioETexto(i++, mostrarLegenda: true);

        exAtual = exerciciosCompletarFrase[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

        await PlayAudioETexto(i++, mostrarLegenda: true);

        await AbrirExercicio(ObterTipoExercicio(exerciciosAssociacao[0]), exerciciosAssociacao[0]);
    }

    private async Task interacaoA1A2()
{
    int i = 0;

    await PlayAudioETexto(i++, mostrarLegenda: true);

    exAtual = exerciciosSpeaking[0];
    await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

    await PlayAudioETexto(i++, mostrarLegenda: true);

    exAtual = exerciciosAlternativas[0];
    await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

    exAtual = exerciciosSpeaking[1];
    await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

    await PlayAudioETexto(i++, mostrarLegenda: true);

    exAtual = exerciciosCriarFrases[0];
    await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

    await PlayAudioETexto(i++, mostrarLegenda: true);

    exAtual = exerciciosSpeaking[2];
    await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

    await PlayAudioETexto(i++, mostrarLegenda: true);

    exAtual = exerciciosAlternativas[1];
    await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

    await AbrirExercicio(
        ObterTipoExercicio(exerciciosAssociacao[0]),
        exerciciosAssociacao[0]
    );
}

private async Task interacaoA2B1()
{
    int i = 0;

    await PlayAudioETexto(i++, mostrarLegenda: true);

    exAtual = exerciciosSpeaking[0];
    await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

    await PlayAudioETexto(i++, mostrarLegenda: true);

    exAtual = exerciciosAlternativas[0];
    await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

    exAtual = exerciciosSpeaking[1];
    await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

    await PlayAudioETexto(i++, mostrarLegenda: true);

    exAtual = exerciciosSpeaking[2];
    await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

    await PlayAudioETexto(i++, mostrarLegenda: true);

    exAtual = exerciciosCriarFrases[0];
    await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

    await PlayAudioETexto(i++, mostrarLegenda: true);

    exAtual = exerciciosAlternativas[1];
    await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

    await PlayAudioETexto(i++, mostrarLegenda: true);

    exAtual = exerciciosCompletarFrase[0];
    await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

    await AbrirExercicio(
        ObterTipoExercicio(exerciciosAssociacao[0]),
        exerciciosAssociacao[0]
    );
}

    private async Task PegarChocolate()
    {
        await Task.Delay(1500);

        if (produtoChocolate != null)
        {
            produtoChocolate.SetActive(false);
        }
    }

    public async Task AbrirExercicio(TipoExercicio tipo, ExercicioBase ex)
    {
        Debug.Log("Abrindo exercício: " + tipo);

        ModalLegenda legenda = modalLegenda.GetComponent<ModalLegenda>();

        if (modalLegenda != null)
            modalLegenda.SetActive(false);

        transform.position = posicaoInicialCashier;
        transform.rotation = rotacaoInicialCashier;

        await EntrarModoExercicio();

        Vector3 posicaoExercicio = transform.position;
        Quaternion rotacaoExercicio = transform.rotation;

        DefinirCursorDesktop(true);

        modalExercicio.Abrir(tipo, ex, this);

        if (legenda != null)
            legenda.AjustarPosicaoPeloEstadoDoJogo();

        while (!modalExercicio.exercicioFinalizado)
        {
            transform.position = posicaoExercicio;
            transform.rotation = rotacaoExercicio;
            await Task.Yield();
        }

        if (modalLegenda != null)
            modalLegenda.SetActive(false);

        await SairModoExercicio();

        DefinirCursorDesktop(false);

        if (legenda != null)
            legenda.AjustarPosicaoPeloEstadoDoJogo();
    }
    private Task<bool> FazerRevisao()
    {
        if (RevisaoManager.Instance == null)
        {
            Debug.LogError("RevisaoManager não está disponível para revisar o Market.");
            return Task.FromResult(false);
        }

        return RevisaoManager.Instance.FazerRevisao(
            this,
            modalExercicio,
            modalAvisoRevisao,
            modalLegenda
        );
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

    private async Task PlayAudioETexto(int i, bool mostrarLegenda = true)
{
    ultimaFraseDita = dialogoAtual[i];
    byte[] audioBytes = await backendManager.GerarAudio(dialogoAtual[i], idNpcParaVoz);
    
    if (audioBytes != null && audioBytes.Length > 0)
    {
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

        if (mostrarLegenda)
        {
            if (textoPularDialogo != null)
            {
                textoPularDialogo.gameObject.SetActive(true);

                DefinirCursorDesktop(true);
            }
        }

        if (mostrarLegenda)
        {
            await Task.Yield();

            await AguardarAvancoDialogo();
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

        if (ProdutoManager.Instance == null)
            return false;

        if (!ProdutoManager.Instance.ProdutosObrigatoriosPegos())
            return false;

        return GameProgress.Instance.PodeFalarComCaixa();
    }

    public bool JogadorPegouSacola()
    {
        return sacolaFoiPega;
    }

    public bool MissaoSacolaLiberada()
    {
        return missaoSacolaLiberada;
    }
}