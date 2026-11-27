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
using System.Linq;
using Newtonsoft.Json.Linq;

public class CashierNPC : InteracaoNPC, IRoteiroNpcCheckpoint
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
    private const string NpcKey = "cashier_npc";
    private const string GreetingToken = "{{greeting}}";
    private const string TotalToken = "{{total}}";
    private ExerciseLoadResult catalogoExercicios;

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
    private bool chocolateFoiOcultado;

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

    private void InicializarConteudosPorNivel(ConteudoRoteiro descricao = null, string nivelDescricao = null)
    {
        string nivelAtual = nivelDescricao ?? this.nivelAtual;
        var dialogoAtual = this.dialogoAtual;
        var exerciciosCompletarFrase = descricao?.exerciciosCompletarFrase ?? this.exerciciosCompletarFrase;
        var exerciciosCriarFrases = descricao?.exerciciosCriarFrases ?? this.exerciciosCriarFrases;
        var exerciciosSpeaking = descricao?.exerciciosSpeaking ?? this.exerciciosSpeaking;
        var exerciciosAlternativas = descricao?.exerciciosAlternativas ?? this.exerciciosAlternativas;
        var exerciciosWriting = descricao?.exerciciosWriting ?? this.exerciciosWriting;
        var exerciciosAssociacao = descricao?.exerciciosAssociacao ?? this.exerciciosAssociacao;
        var cumprimento = descricao == null ? this.cumprimento : "{{greeting}}";
        var valorTotal = this.valorTotal;

        exerciciosCompletarFrase.Clear();
        exerciciosCriarFrases.Clear();
        exerciciosSpeaking.Clear();
        exerciciosAlternativas.Clear();
        exerciciosWriting.Clear();
        exerciciosAssociacao.Clear();

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
                    modoLivre = true,
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
                        "don't", "need", "yesterday", "your", "I"
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
                        " card, and I would also like a printed ",
                        "."
                    },
                    respostasCorretas = new List<string> { "debit", "receipt" },
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
            "I'm looking for a clothing store.\r\n",
            cumprimento + " Yes, I found everything I needed, thank you.\r\n",
            "I would like to reserve a table.\r\n"
        },
        respostaCorreta = 1
    });

    exerciciosAlternativas.Add(new ExercicioAlternativas()
    {
        enunciado = "The cashier tells you that your total is $12.75. What is the correct amount?",
        alternativas = new List<string>()
        {
            "Twenty dollars and fifty cents.",
            "Twelve dollars and fifteen cents.",
            "Seventeen dollars and twenty-five cents.",
            "Twelve dollars and seventy-five cents."
        },
        alternativaCorreta = 3
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
            "I usually buy cereal on Mondays.\r\n",
            "I'd prefer to pay with my debit card.\r\n",
            "I'd like to try another size.\r\n"
        },
        respostaCorreta = 1
    });

    exerciciosAlternativas.Add(new ExercicioAlternativas()
    {
        enunciado = "The cashier asks whether you would like a printed receipt. What should you say if you want one?",
        alternativas = new List<string>()
        {
            "I'd like to change my shopping cart.",
            "The store closes at nine.",
            "Yes, please. I'd like a receipt.",
            "No, I don't need any milk."
        },
        alternativaCorreta = 2
    });

        exerciciosAssociacao.Add(new ExercicioAssociacao()
        {
            enunciado = "Match the English words with their Portuguese translations:",
            opcoesPort = new List<string>()
            {
                "Embalagem",
                "Peso",
                "Carrinho",
                "Oferta",
                "Etiqueta",
                "Produto",
                "Compra",
                "Estoque"
            },
            opcoesIngles = new List<string>()
            {
                "Purchase",
                "Label",
                "Stock",
                "Product",
                "Weight",
                "Packaging",
                "Sale",
                "Cart"
            },
            associacaoCorreta = new List<ParAssociacao>()
            {
                new ParAssociacao() { palavraPortugues = "Embalagem", palavraIngles = "Packaging" },
                new ParAssociacao() { palavraPortugues = "Peso", palavraIngles = "Weight" },
                new ParAssociacao() { palavraPortugues = "Carrinho", palavraIngles = "Cart" },
                new ParAssociacao() { palavraPortugues = "Oferta", palavraIngles = "Sale" },
                new ParAssociacao() { palavraPortugues = "Etiqueta", palavraIngles = "Label" },
                new ParAssociacao() { palavraPortugues = "Produto", palavraIngles = "Product" },
                new ParAssociacao() { palavraPortugues = "Compra", palavraIngles = "Purchase" },
                new ParAssociacao() { palavraPortugues = "Estoque", palavraIngles = "Stock" }
            }
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
        modoLivre = true,
        enunciado = "Respond to the cashier politely. Confirm that you found everything you needed and thank them."
    });

    exerciciosAlternativas.Add(new ExercicioAlternativas()
    {
        enunciado = "The cashier says that your total comes to $18.40. Which option correctly expresses this amount?",
        alternativas = new List<string>()
        {
            "Eighty dollars and eighteen cents.",
            "Eighteen dollars and fourteen cents.",
            "Fourteen dollars and eighty cents.",
            "Eighteen dollars and forty cents."
        },
        alternativaCorreta = 3
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
                "I would like to order a sandwich.\r\n",
                "I need to find the dairy section.\r\n",
                "I brought my own reusable bag, so I don't need another one.\r\n"
            },
            respostaCorreta = 2
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
                "I brought my own shopping bag.",
                "Could you show me the cereal aisle?",
                "A digital receipt would be great, thank you.",
                "I'd like to pay in cash."
            },
            alternativaCorreta = 2
        });

        exerciciosCompletarFrase.Add(new ExercicioCompletarFrase()
        {
            enunciado = "Complete the sentence with the correct words:",
            partesFrase = new List<string>()
            {
                "I'd prefer to pay with my ",
                " card, and I'd like a digital ",
                "."
            },
            respostasCorretas = new List<string> { "debit", "receipt" },
            opcoes = new List<string>()
            {
                "credit", "cash", "receipt", "bag",
                "money", "paper", "card", "payment",
                "change", "total", "debit"
            }
        });

        exerciciosAssociacao.Add(new ExercicioAssociacao()
        {
                enunciado = "Match the English words with their Portuguese translations:",
                opcoesPort = new List<string>()
                {
                    "Cobrança",
                    "Recusa",
                    "Comprovante",
                    "Valor",
                    "Parcelamento",
                    "Conta",
                    "Taxa",
                    "Reembolso",
                    "Pendência",
                    "Limite"
                },
                opcoesIngles = new List<string>()
                {
                    "Refund",
                    "Limit",
                    "Charge",
                    "Account",
                    "Installment",
                    "Receipt",
                    "Fee",
                    "Amount",
                    "Decline",
                    "Pending"
                },
                associacaoCorreta = new List<ParAssociacao>()
                {
                    new ParAssociacao() { palavraPortugues = "Cobrança", palavraIngles = "Charge" },
                    new ParAssociacao() { palavraPortugues = "Recusa", palavraIngles = "Decline" },
                    new ParAssociacao() { palavraPortugues = "Comprovante", palavraIngles = "Receipt" },
                    new ParAssociacao() { palavraPortugues = "Valor", palavraIngles = "Amount" },
                    new ParAssociacao() { palavraPortugues = "Parcelamento", palavraIngles = "Installment" },
                    new ParAssociacao() { palavraPortugues = "Conta", palavraIngles = "Account" },
                    new ParAssociacao() { palavraPortugues = "Taxa", palavraIngles = "Fee" },
                    new ParAssociacao() { palavraPortugues = "Reembolso", palavraIngles = "Refund" },
                    new ParAssociacao() { palavraPortugues = "Pendência", palavraIngles = "Pending" },
                    new ParAssociacao() { palavraPortugues = "Limite", palavraIngles = "Limit" }
                }
            });
        }

        if (descricao != null)
            descricao.Falas = dialogoAtual;
        else
        {
            this.dialogoAtual = dialogoAtual;
            this.valorTotal = valorTotal;
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

    protected override Task IniciarInteracao() => ExecutarAtendimento(0);

    private async Task ExecutarAtendimento(int inicio, string total = null, bool sacolaJaColetada = false)
    {
        int versao = SessaoCenario.Atual.Versao;
        if (total != null || GameProgress.Instance.PodeFalarComCaixa())
        {
            GameProgress.EstaEmDialogo = true;

            nivelAtual = DadosJogador.nivelUsuario;
            bool intermediario = MissionManager.NivelMarketIntermediario(nivelAtual);
            if (total == null && intermediario)
                MissionManager.Instance?.ConcluirMissaoMarket("falar_caixa", "npc_interagido", "cashier_npc");

            InicializarConteudosPorNivel();

            totalDoRoteiro = total ?? valorTotal.ToString();
            if (total == null)
                await MostrarProdutosNoBalcao();
            else
            {
                checkoutVisualJaFeito = true;
                if (!sacolaJaColetada) PrepararSacolaVisual();
            }


            catalogoExercicios = await ExerciseManager.Instance.LoadForNpcAsync(NpcKey, nivelAtual);
            if (!catalogoExercicios.Success)
                Debug.LogWarning("CashierNPC: usando exerc?cios locais. " + catalogoExercicios.Error);

            ValidarVisita(versao);
            await CriarRoteiro(versao).Executar(inicio);

            if (!intermediario)
                MissionManager.Instance.ConcluirMissaoMarket("falar_caixa", "conversa_encerrada", "cashier_npc");

            if (intermediario)
            {
                bool iniciante = nivelAtual == "A1-A2";
                MissionManager.Instance.IniciarMissaoMarket("pegar_sacola",
                    iniciante ? "Collect your shopping bag and leave the market."
                        : "Pick up your shopping bag at checkout and exit the market.",
                    iniciante ? "Pegue sua sacola de compras e saia do mercado."
                        : "Pegue sua sacola de compras no caixa e saia do mercado.");
            }
            else
            {
                MissionManager.Instance.IniciarMissaoMarket(
                    "pegar_sacola",
                    "Pick up your bag at the checkout and leave the market",
                    "Pegue a sacola no caixa e saia do mercado"
                );
            }

            missaoSacolaLiberada = true;

            aguardandoPegarSacola = true;
            sacolaFoiPega = sacolaJaColetada;

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


    private string totalDoRoteiro;

    async Task<System.Func<Task>> IRoteiroNpcCheckpoint.PrepararRetomada(Newtonsoft.Json.Linq.JObject visita,
        int versao, System.Threading.CancellationToken token)
    {
        ValidarVisita(versao);
        AtualizarVisita();
        nivelAtual = SessaoCenario.Atual.Nivel;
        cumprimento = RoteiroNpcCheckpoint.SaudacaoSalva(visita);
        InicializarConteudosPorNivel();
        var cursor = CriarRoteiro(versao).Validar(visita);
        if (cursor.Situacao.Length != 3 || !System.Text.RegularExpressions.Regex.IsMatch(cursor.Situacao[1],
            @"\Atotal\.[0-9]{1,8}(?:(?:\.|-comma-)[0-9]{1,2})?\z")
            || (cursor.Situacao[2] != "chocolate.hidden" && cursor.Situacao[2] != "chocolate.visible"))
            throw new System.IO.InvalidDataException("Total ou estado do caixa inválido.");
        string total = cursor.Situacao[1].Substring(6).Replace("-comma-", ",");
        var sacola = ((JArray)visita["missions"]).FirstOrDefault(m => (string)m["mission_key"] == "pegar_sacola");
        bool saiuDoMarket = (string)sacola?["status"] == "completed";
        bool sacolaJaColetada = saiuDoMarket || (sacola?["satisfied_events"] as JArray)?.Any(e =>
            (string)e["tipo"] == "marco_fluxo" && (string)e["alvo"] == "sacola_coletada") == true;
        await RoteiroNpcCheckpoint.ExigirCatalogo(this, versao, token);
        return async () =>
        {
            ValidarVisita(versao);
            TravarControlePlayer();
            if (cursor.Situacao[2] == "chocolate.hidden" && produtoChocolate != null)
            {
                chocolateFoiOcultado = true;
                produtoChocolate.SetActive(false);
            }
            await ExecutarAtendimento(cursor.Indice, total, sacolaJaColetada);
            if (saiuDoMarket && SessaoCenario.Atual.Permite("Market", versao))
                await ExecutarAtividadesAposSaida();
        };
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
        var exerciciosCompletarFrase = descricao?.exerciciosCompletarFrase ?? this.exerciciosCompletarFrase;
        var exerciciosCriarFrases = descricao?.exerciciosCriarFrases ?? this.exerciciosCriarFrases;
        var exerciciosSpeaking = descricao?.exerciciosSpeaking ?? this.exerciciosSpeaking;
        var exerciciosAlternativas = descricao?.exerciciosAlternativas ?? this.exerciciosAlternativas;
        var exerciciosWriting = descricao?.exerciciosWriting ?? this.exerciciosWriting;

        var roteiro = new RoteiroNpcCheckpoint(this, visita, () => new[] { RoteiroNpcCheckpoint.ChaveSaudacao(cumprimento),
            "total." + (totalDoRoteiro ?? valorTotal.ToString()).Replace(",", "-comma-"),
            chocolateFoiOcultado ? "chocolate.hidden" : "chocolate.visible" });
        void Fala(int indice, bool mostrarLegenda = true) => roteiro.Fala(dialogoAtual[indice], () => PlayAudioETexto(indice, mostrarLegenda));
        void Exercicio(string ponto, int ordem, ExercicioBase fallback) =>
            roteiro.Exercicio(ponto, ordem, () => AbrirExercicioDoPonto(ponto, ordem, fallback), modelo: fallback);

        switch (nivelAtual)
        {
            case "A1":
                Fala(0, mostrarLegenda: false);
                Exercicio("greeting", 1, exerciciosSpeaking[0]);
                Fala(1, mostrarLegenda: true);
                Exercicio("additional_item", 1, exerciciosCriarFrases[0]);
                roteiro.Acao("PegarChocolate", PegarChocolate);
                Fala(2);
                Exercicio("purchase_total", 1, exerciciosAlternativas[0]);
                Fala(3, mostrarLegenda: true);
                Exercicio("payment_method", 1, exerciciosWriting[0]);
                Fala(4, mostrarLegenda: true);
                break;
            case "A2":
                Fala(0, mostrarLegenda: false);
                Exercicio("greeting", 1, exerciciosSpeaking[0]);
                Fala(1, mostrarLegenda: true);
                Exercicio("purchase_total", 1, exerciciosAlternativas[0]);
                Exercicio("purchase_total", 2, exerciciosSpeaking[1]);
                Fala(2, mostrarLegenda: true);
                Exercicio("bag_choice", 1, exerciciosCriarFrases[0]);
                Fala(3, mostrarLegenda: true);
                Exercicio("anything_else", 1, exerciciosSpeaking[2]);
                Fala(4, mostrarLegenda: true);
                Exercicio("payment_method", 1, exerciciosAlternativas[1]);
                Fala(5, mostrarLegenda: true);
                break;
            case "B1":
                Fala(0, mostrarLegenda: true);
                Exercicio("greeting", 1, exerciciosSpeaking[0]);
                Fala(1, mostrarLegenda: true);
                Exercicio("purchase_total", 1, exerciciosAlternativas[0]);
                Exercicio("purchase_total", 2, exerciciosSpeaking[1]);
                Fala(2, mostrarLegenda: true);
                Exercicio("bag_choice", 1, exerciciosSpeaking[2]);
                Fala(3, mostrarLegenda: true);
                Exercicio("payment_and_receipt", 1, exerciciosCompletarFrase[0]);
                Fala(4, mostrarLegenda: true);
                break;
            case "A1-A2":
                Fala(0, mostrarLegenda: true);
                Exercicio("greeting", 1, exerciciosSpeaking[0]);
                Fala(1, mostrarLegenda: true);
                Exercicio("purchase_total", 1, exerciciosAlternativas[0]);
                Exercicio("purchase_total", 2, exerciciosSpeaking[1]);
                Fala(2, mostrarLegenda: true);
                Exercicio("bag_choice", 1, exerciciosCriarFrases[0]);
                Fala(3, mostrarLegenda: true);
                Exercicio("payment_method", 1, exerciciosSpeaking[2]);
                Fala(4, mostrarLegenda: true);
                Exercicio("receipt_choice", 1, exerciciosAlternativas[1]);
                break;
            case "A2-B1":
                Fala(0, mostrarLegenda: true);
                Exercicio("greeting", 1, exerciciosSpeaking[0]);
                Fala(1, mostrarLegenda: true);
                Exercicio("purchase_total", 1, exerciciosAlternativas[0]);
                Exercicio("purchase_total", 2, exerciciosSpeaking[1]);
                Fala(2, mostrarLegenda: true);
                Exercicio("bag_choice", 1, exerciciosSpeaking[2]);
                Fala(3, mostrarLegenda: true);
                Exercicio("payment_method", 1, exerciciosCriarFrases[0]);
                Fala(4, mostrarLegenda: true);
                Exercicio("receipt_choice", 1, exerciciosAlternativas[1]);
                Fala(5, mostrarLegenda: true);
                Exercicio("payment_and_receipt", 1, exerciciosCompletarFrase[0]);
                break;
        }
        return roteiro;
    }

    private async Task PegarChocolate()
    {
        int visita = SessaoCenario.Atual.Versao;
        await Task.Delay(1500);
        ValidarVisita(visita);

        if (produtoChocolate != null && produtoChocolate.activeSelf)
        {
            chocolateFoiOcultado = true;
            produtoChocolate.SetActive(false);
        }
    }

    private Task AbrirExercicioDoPonto(string pointKey, int ordem, ExercicioBase fallback, bool faseAssociacao = false)
    {
        CheckpointJogo.IdentificarFallback(fallback, pointKey, ordem, nivelAtual);
        TipoExercicio tipo = ObterTipoExercicio(fallback);
        bool? speakingLivre = (fallback as ExercicioSpeaking)?.modoLivre;
        ExerciseCatalogItem item = catalogoExercicios?.Find(pointKey, nivelAtual, ordem, tipo, speakingLivre);
        if (item != null)
        {
            var speaking = item.Exercise as ExercicioSpeaking;
            return AbrirExercicio(item.Tipo, speaking == null ? item.Exercise : PreencherSpeaking(speaking), faseAssociacao);
        }

        Debug.LogWarning($"CashierNPC: usando exerc?cio local em {nivelAtual}/{pointKey}/{ordem}.");
        return AbrirExercicio(tipo, fallback, faseAssociacao);
    }

    private ExercicioSpeaking PreencherSpeaking(ExercicioSpeaking speaking)
    {
        return PreencherSpeaking(speaking, cumprimento, totalDoRoteiro ?? valorTotal.ToString());
    }

    internal static ExercicioSpeaking PreencherSpeaking(ExercicioSpeaking speaking, string greeting, string total)
    {
        string Preencher(string texto) => texto?.Replace(GreetingToken, greeting ?? GreetingToken)
            .Replace(TotalToken, total ?? TotalToken);

        // O cat?logo ? compartilhado pelo cache. O texto preenchido pertence apenas ? intera??o atual.
        return new ExercicioSpeaking
        {
            reviewGreeting = speaking.pointKey == "greeting" ? greeting : null,
            reviewTotal = speaking.pointKey == "purchase_total" && speaking.ordemBanco == 2
                && (speaking.nivelBanco == "A2" || speaking.nivelBanco == "B1") ? total : null,
            idExercise = speaking.idExercise,
            idTipoBanco = speaking.idTipoBanco,
            nomeTipoBanco = speaking.nomeTipoBanco,
            nivelBanco = speaking.nivelBanco,
            pointKey = speaking.pointKey,
            ordemBanco = speaking.ordemBanco,
            enunciado = Preencher(speaking.enunciado),
            feedback = speaking.feedback,
            respostasEsperadas = speaking.respostasEsperadas,
            modoLivre = speaking.modoLivre,
            opcoesFala = speaking.opcoesFala?.ConvertAll(Preencher),
            respostaCorreta = speaking.respostaCorreta
        };
    }

    public async Task AbrirExercicio(TipoExercicio tipo, ExercicioBase ex, bool faseAssociacao = false)
    {
        Debug.Log("Abrindo exercício: " + tipo);

        ModalLegenda legenda = modalLegenda.GetComponent<ModalLegenda>();

        if (modalLegenda != null)
            modalLegenda.SetActive(false);

        bool enquadrarNpc = tipo != TipoExercicio.Associacao;
        if (enquadrarNpc)
            transform.SetPositionAndRotation(posicaoInicialCashier, rotacaoInicialCashier);

        await EntrarModoExercicio(tipo);

        Vector3 posicaoExercicio = transform.position;
        Quaternion rotacaoExercicio = transform.rotation;

        DefinirCursorDesktop(true);

        modalExercicio.Abrir(tipo, ex, this, faseAssociacao: faseAssociacao);

        if (legenda != null)
            legenda.AjustarPosicaoPeloEstadoDoJogo();

        while (!modalExercicio.exercicioFinalizado)
        {
            if (enquadrarNpc)
                transform.SetPositionAndRotation(posicaoExercicio, rotacaoExercicio);
            await Task.Yield();
        }

        if (modalLegenda != null)
            modalLegenda.SetActive(false);

        await SairModoExercicio(tipo);

        DefinirCursorDesktop(false);

        if (legenda != null)
            legenda.AjustarPosicaoPeloEstadoDoJogo();
    }
    public async Task<bool> ExecutarAtividadesAposSaida()
    {
        if (!sacolaFoiPega || MissionManager.Instance?.MissaoConcluida("pegar_sacola") != true)
            return false;

        GameProgress.EstaEmDialogo = true;
        TravarControlePlayer();
        try
        {
            CheckpointJogo.Instance?.CapturarAssociacaoFinal(this, "checkout_vocabulary", 1);
            await AbrirExercicioDoPonto("checkout_vocabulary", 1, exerciciosAssociacao[0], faseAssociacao: true);
            return await FazerRevisao();
        }
        finally
        {
            GameProgress.EstaEmDialogo = false;
        }
    }

    internal Task<bool> RetomarRevisaoCheckpoint() =>
        RevisaoManager.Instance.ExecutarRevisaoEExibirModalFinal(this, modalExercicio, modalAvisoRevisao, modalLegenda);

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

        PrepararSacolaVisual();
    }

    private void PrepararSacolaVisual()
    {
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

    internal void ReiniciarMarket()
    {
        sacolaFoiPega = aguardandoPegarSacola = missaoSacolaLiberada = checkoutVisualJaFeito = false;
        if (sacolaFinal != null)
            sacolaFinal.SetActive(false);
        if (chocolateFoiOcultado && produtoChocolate != null)
            produtoChocolate.SetActive(true);
        chocolateFoiOcultado = false;
    }

    public void SacolaFoiPega()
    {
        if (!PertenceAoCenarioAtivo || !missaoSacolaLiberada || sacolaFoiPega)
            return;
        sacolaFoiPega = true;
        MissionManager.Instance?.RegistrarEventoMarket("marco_fluxo", "sacola_coletada");
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

        if (MissionManager.NivelMarketIntermediario(DadosJogador.nivelUsuario)
            && MissionManager.Instance?.MissaoAtiva("falar_caixa") != true)
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
