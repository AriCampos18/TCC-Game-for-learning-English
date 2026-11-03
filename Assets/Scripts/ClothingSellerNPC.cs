using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using TMPro;
using System;

public class ClothingSellerNPC : InteracaoNPC
{
    string nivelAtual;
    string cumprimento;
    int horaAtual = DateTime.Now.Hour;
    List<string> dialogoAtual;
    private AudioSource audioSource;
    private BackendManager backendManager;
    public ModalExercicio modalExercicio;
    public GameObject modalLegenda;
    public TextMeshProUGUI textoPularDialogo;
    public ExercicioBase exAtual;

    public FirstPlayerController playerController;

    [System.NonSerialized] public List<ExercicioBase> exerciciosCompletarFrase;
    [System.NonSerialized] public List<ExercicioBase> exerciciosCriarFrases;
    [System.NonSerialized] public List<ExercicioBase> exerciciosSpeaking;
    [System.NonSerialized] public List<ExercicioBase> exerciciosAlternativas;
    [System.NonSerialized] public List<ExercicioBase> exerciciosWriting;
    [System.NonSerialized] public List<ExercicioBase> exerciciosAssociacao;

    public string idNpcParaVoz = "clothingSeller";

    protected override void Start()
    {
        base.Start();
        nivelAtual = DadosJogador.nivelUsuario;
        nomeExibicaoLegenda = "Clothing Seller";
        exerciciosCompletarFrase = GarantirListaExercicios(exerciciosCompletarFrase);
        exerciciosCriarFrases = GarantirListaExercicios(exerciciosCriarFrases);
        exerciciosSpeaking = GarantirListaExercicios(exerciciosSpeaking);
        exerciciosAlternativas = GarantirListaExercicios(exerciciosAlternativas);
        exerciciosWriting = GarantirListaExercicios(exerciciosWriting);
        exerciciosAssociacao = GarantirListaExercicios(exerciciosAssociacao);

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
            dialogoAtual = new List<string>()
            {
                "Hello! Welcome to our clothing store. Can I help you?",
                "What size do you need?",
                "What color would you like?",
                "Sure! Here is a blue T-shirt in size small.",
                "Would you like to try it on?",
                "How does it fit?",
                "I'll bring you a larger one.",
                "Here is a larger one. Would you like to buy it?",
                "Of course! Would you like a bag?",
                "Thank you. Have a nice day!"
            };

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "You are in a clothing store. Tell the salesperson what you are looking for.",
                    opcoesFala = new List<string>() {
                        "I'm looking for a T-shirt\r\n",
                        "I need to book a hotel room\r\n",
                        "Where is the airport?\r\n"
                    },
                    respostaCorreta = 0
                }
            );

            exerciciosAlternativas.Add(
                new ExercicioAlternativas()
                {
                    enunciado = "Say your size to the salesperson:",
                    alternativas = new List<string>() { 
                        "At five o'clock\r\n", 
                        "It's blue yesterday\r\n", 
                        "Small, please\r\n", 
                        "On the second floor\r\n" },
                    alternativaCorreta = 2
                }
            );

            exerciciosWriting.Add(
                new ExercicioWriting()
                {
                    enunciado = "Tell the salesperson what color T-shirt you want with a sentence."
                }
            );

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "Choose the correct answer to the salesperson's question.",
                    opcoesFala = new List<string>() {
                        "I would like the bill\r\n",
                        "Yes, please\r\n",
                        "I am a T-shirt\r\n"
                    },
                    respostaCorreta = 1
                }
            );

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "Choose the correct answer to the salesperson's question about the T-shirt.",
                    opcoesFala = new List<string>() {
                        "It's too small\r\n",
                        "It's on the table\r\n",
                        "I have a blue yesterday\r\n"
                    },
                    respostaCorreta = 0
                }
            );

            exerciciosCriarFrases.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Arrange the words to form the correct sentence in english: Eu gostaria de comprar esta camiseta, por favor.",
                    blocosPalavras = new List<string>
                    {
                        "please.",
                        "buy",
                        "that",
                        "would",
                        "T-shirt",
                        "sell",
                        "I",
                        "shirt",
                        "to",
                        "could",
                        "this",
                        "like",
                        "want",
                        "the",
                        "you"
                    },
                    respostaCorreta = "I would like to buy this T-shirt, please.\n"
                }
            );

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "Choose the correct answer to the question.",
                    opcoesFala = new List<string>() {
                        "I would like a car\r\n",
                        "The shirt it's looks good\r\n",
                        "Yes, plase. Thank you\r\n"
                    },
                    respostaCorreta = 2
                }
            );

            exerciciosAssociacao.Add(new ExercicioAssociacao
            {
                enunciado = "Match the Portuguese word with the English word.",

                opcoesPort = new List<string>
                {
                    "camiseta",
                    "calça",
                    "vestido",
                    "sapato",
                    "camisa",
                    "saia",
                    "azul",
                    "tamanho"
                },

                opcoesIngles = new List<string>
                {
                    "shoes",
                    "shirt",
                    "blue",
                    "skirt",
                    "T-shirt",
                    "size",
                    "dress",
                    "pants"
                },

                associacaoCorreta = new List<ParAssociacao>
                {
                    new ParAssociacao { palavraPortugues = "camiseta", palavraIngles = "T-shirt" },
                    new ParAssociacao { palavraPortugues = "calça", palavraIngles = "pants" },
                    new ParAssociacao { palavraPortugues = "vestido", palavraIngles = "dress" },
                    new ParAssociacao { palavraPortugues = "sapato", palavraIngles = "shoes" },
                    new ParAssociacao { palavraPortugues = "camisa", palavraIngles = "shirt" },
                    new ParAssociacao { palavraPortugues = "saia", palavraIngles = "skirt" },
                    new ParAssociacao { palavraPortugues = "azul", palavraIngles = "blue" },
                    new ParAssociacao { palavraPortugues = "tamanho", palavraIngles = "size" }
                }
            });
        }
        else if (nivelAtual == "A2")
        {
            dialogoAtual = new List<string>()
            {
                cumprimento+" Welcome to our store. Are you looking for something specific?",
                "Sure! What kind of style do you prefer?",
                "How about this shirt? It's one of our most popular models",
                "Yes! We have it in black and white. What size do you usually wear?",
                "Great! Let me check. We have a medium in black. It's thirty-five dollars.",
                "Of course! You can try it on.",
                "How does it fit?",
                "Of course! I have a large one.",
                "Would you like to buy it?",
                "Great! That will be thirty-five dollars. How would you like to pay?",
                "Perfect! Thank you for shopping with us. Have a great day!"
            };

            exerciciosCriarFrases.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Arrange the words to form the correct sentence in english: Estou procurando uma camisa para um jantar de família.",
                    blocosPalavras = new List<string>
                    {
                        "dinner",
                        "a",
                        "family",
                        "I'm",
                        "dress",
                        "for",
                        "shirt",
                        "buying",
                        "the",
                        "at",
                        "is",
                        "my",
                        "for",
                        "looking"
                    },
                    respostaCorreta = "I'm looking for a shirt for a family dinner\n"
                }
            );

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "Say what kind of style you want for your dinner:",
                    opcoesFala = new List<string>() {
                        "I would like to check in at the restaurant\r\n",
                        "I usually eat dinner at seven\r\n",
                        "Something casual but elegant\r\n"
                    },
                    respostaCorreta = 2
                }
            );

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "You would like to see another color. Ask the salesperson if it is available in a different color",
                    opcoesFala = new List<string>() {
                        "How much does the dress cost?\r\n",
                        "Do you have this in another color?\r\n",
                        "Where can I find the park?\r\n"
                    },
                    respostaCorreta = 1
                }
            );

            exerciciosAlternativas.Add(
                new ExercicioAlternativas()
                {
                    enunciado = "Which answer tells the salesperson your clothing size?",
                    alternativas = new List<string>() { 
                        "I have a reservation for two\r\n", 
                        "I would like to pay tomorrow\r\n", 
                        "I usually go to work by bus\r\n", 
                        "I usually wear a medium\r\n" },
                    alternativaCorreta = 3
                }
            );

            exerciciosAlternativas.Add(
                new ExercicioAlternativas()
                {
                    enunciado = "How much does the shirt cost?",
                    alternativas = new List<string>() { 
                        "$35\r\n", 
                        "$15\r\n", 
                        "$25\r\n", 
                        "$50\r\n" },
                    alternativaCorreta = 0
                }
            );

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "You found a black shirt you like. Ask the salesperson if you can try it on."
                }
            );

            exerciciosWriting.Add(
                new ExercicioWriting()
                {
                    enunciado = "The shirt feels a little tight. Ask the salesperson for a bigger size with a sentece."
                }
            );

            exerciciosCompletarFrase.Add(
                new ExercicioCompletarFrase()
                {
                    enunciado = "Complete the sentence with the correct word:",
                    fraseAntes = "This size is ",
                    fraseDepois = " comfortable than the medium one.",
                    respostaCorreta = "more",
                    opcoes = new List<string>
                    {
                        "better",
                        "very",
                        "more",
                        "less",
                        "too",
                        "much",
                        "most",
                        "enough",
                        "quite",
                        "really",
                        "as"
                    }
                }
            );

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "You have decided to buy the shirt. Tell the salesperson you would like to purchase it.",
                    opcoesFala = new List<string>() {
                        "I would like to try the restaurant\r\n",
                        "Yes, I'll take it, please\r\n",
                        "I have a class at eight\r\n"
                    },
                    respostaCorreta = 1
                }
            );

            exerciciosAlternativas.Add(
                new ExercicioAlternativas()
                {
                    enunciado = "How can you tell the salesperson that you want to pay by card?",
                    alternativas = new List<string>() { 
                        "I'd like to wear a card, please\r\n", 
                        "I'd like to pay by card, please\r\n", 
                        "I'd like to pay by shirt, plase\r\n", 
                        "I'd like to buy a different color, plase\r\n" },
                    alternativaCorreta = 1
                }
            );

            exerciciosAssociacao.Add(new ExercicioAssociacao
            {
                enunciado = "Match the Portuguese word with the English word.",

                opcoesPort = new List<string>
                {
                    "experimentar",
                    "vendedor",
                    "tamanho médio",
                    "caro",
                    "barato",
                    "outra cor",
                    "ajustar",
                    "vestir bem"
                },

                opcoesIngles = new List<string>
                {
                    "affordable",
                    "try on",
                    "fit well",
                    "salesperson",
                    "another color",
                    "medium size",
                    "adjust",
                    "expensive"
                },

                associacaoCorreta = new List<ParAssociacao>
                {
                    new ParAssociacao { palavraPortugues = "experimentar", palavraIngles = "try on" },
                    new ParAssociacao { palavraPortugues = "vendedor", palavraIngles = "salesperson" },
                    new ParAssociacao { palavraPortugues = "tamanho médio", palavraIngles = "medium size" },
                    new ParAssociacao { palavraPortugues = "caro", palavraIngles = "expensive" },
                    new ParAssociacao { palavraPortugues = "barato", palavraIngles = "affordable" },
                    new ParAssociacao { palavraPortugues = "outra cor", palavraIngles = "another color" },
                    new ParAssociacao { palavraPortugues = "ajustar", palavraIngles = "adjust" },
                    new ParAssociacao { palavraPortugues = "vestir bem", palavraIngles = "fit well" }
                }
            });

        }
        else if (nivelAtual == "B1")
        {
            dialogoAtual = new List<string>()
            {
                cumprimento+" Welcome to our store. How can I assist you today?",
                "Certainly! Do you have any particular preferences regarding style or color?",
                "I think this blazer could be a good option. It has a smart-casual design and can be combined with different types of trousers.",
                "Yes, we have it in navy. It's made from a cotton blend, and we recommend dry cleaning to keep the fabric in good condition.",
                "Would you like to see some trousers that would match the blazer?",
                "Certainly! We have these navy trousers for seventy-five dollars. They match the blazer quite well.",
                "Would you like to try both pieces on before making a decision?",
                "How do they fit? Are you comfortable with them?",
                "Of course! I'll get a larger size for you. Just a moment, please.",
                "How about this one? Does it feel more comfortable?",
                "Great! Would you like to purchase the blazer and the trousers?",
                "Excellent choice! The total is one hundred and sixty-four dollars. Would you like to pay by card or cash?",
                "Perfect! Your purchase is complete. Thank you for shopping with us. I hope you enjoy your new outfit!"
            };

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "You are attending a work event next week and need an appropriate outfit. Explain to the salesperson what kind of clothing you are looking for."
                }
            );

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "Tell the salesperson what colors and style you prefer for your outfit.",
                    opcoesFala = new List<string>() {
                        "I would like to reserve a table for two\r\n",
                        "I'd prefer something in neutral colors, preferably navy or black\r\n",
                        "I'd prefer something in neutral colors, preferably navy or black, because it match the event\r\n"
                    },
                    respostaCorreta = 1
                }
            );

            exerciciosAlternativas.Add(
                new ExercicioAlternativas()
                {
                    enunciado = "Why does the salesperson recommend the blazer?",
                    alternativas = new List<string>() { 
                        "Because it has a smart-casual design and can be combined with different trousers\r\n", 
                        "Because it is the cheapest item in the store\r\n", 
                        "Because it is only available in sports sizes\r\n", 
                        "Because it is designed for cold weather only\r\n" },
                    alternativaCorreta = 0
                }
            );

            exerciciosWriting.Add(
                new ExercicioWriting()
                {
                    enunciado = "You like the blazer and want to know more about it. Ask the salesperson if it is available in navy and how to care for the fabric."
                }
            );

            exerciciosAlternativas.Add(
                new ExercicioAlternativas()
                {
                    enunciado = "How should the blazer be cleaned?",
                    alternativas = new List<string>() { 
                        "It should be cleaned with a wet cloth\r\n", 
                        "It should be washed with cold water only\r\n", 
                        "It should be machine washed\r\n", 
                        "It should be dry cleaned\r\n" },
                    alternativaCorreta = 3
                }
            );

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "You would like to complete your outfit with a pair of trousers. Ask the salesperson about the available styles and their prices."
                }
            );

            exerciciosCriarFrases.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Arrange the words to form the correct sentence in english: Embora o blazer seja um pouco caro, acho que vale a pena comprá-lo",
                    blocosPalavras = new List<string>
                    {
                        "buying",
                        "Although",
                        "worth",
                        "the",
                        "expensive,",
                        "think",
                        "blazer",
                        "a",
                        "it's",
                        "little",
                        "I",
                        "worth",
                        "because",
                        "is",
                        "buy"
                    },
                    respostaCorreta = "Although the blazer is a little expensive, I think it's worth buying\n"
                }
            );

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "The salesperson shows you how the blazer and trousers look together. Tell them what you think about the combination.",
                    opcoesFala = new List<string>() {
                        "No, I would like to order some food\r\n",
                        "Yes, I have been working at the airport\r\n",
                        "Yes, I'd like to see how they look together\r\n"
                    },
                    respostaCorreta = 2
                }
            );

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "The blazer looks good, but the shoulders feel a little tight. Explain the problem to the salesperson and ask if they have a larger size."
                }
            );

            exerciciosCompletarFrase.Add(
                new ExercicioCompletarFrase()
                {
                    enunciado = "Complete the sentence with the correct word:",
                    fraseAntes = "The larger size fits ",
                    fraseDepois = " than the previous one.",
                    respostaCorreta = "better",
                    opcoes = new List<string>
                    {
                        "quite",
                        "best",
                        "as",
                        "comfortable",
                        "too",
                        "worse",
                        "more",
                        "enough",
                        "less",
                        "really",
                        "better"
                    }
                }
            );

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "The salesperson asks if you would like to buy both the blazer and the trousers. Tell them you have decided to purchase both items.",
                    opcoesFala = new List<string>() {
                        "No, I didn’t like it this way. Do have another combination?\r\n",
                        "Yes, I'll take both, please. They are exactly what I was looking for\r\n",
                        "Yes, and I have a flight at five o'clock\r\n"
                    },
                    respostaCorreta = 1
                }
            );

            exerciciosAlternativas.Add(
                new ExercicioAlternativas()
                {
                    enunciado = "You have chosen a shirt and are ready to pay. How would you politely inform the salesperson that you prefer to use your credit card instead of cash?",
                    alternativas = new List<string>() {
                        "Would you mind giving me a discount?",
                        "I'd prefer to pay by credit card, if that's possible.",
                        "Could you exchange this shirt for a larger one?",
                        "I'd rather take another look at the other options."
                    },
                    alternativaCorreta = 1
                }
            );

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "The salesperson tells you the total price of both items. Say how much they cost together.",
                    opcoesFala = new List<string>() {
                        "Two hundred and sixty-four dollars\r\n",
                        "One hundred and six dollars\r\n",
                        "One hundred and sixty-four dollars\r\n"
                    },
                    respostaCorreta = 2
                }
            );

            exerciciosAssociacao.Add(new ExercicioAssociacao
            {
                enunciado = "Match the Portuguese word with the English word.",

                opcoesPort = new List<string>
                {
                    "tecido",
                    "mistura de algodão",
                    "lavagem a seco",
                    "elegante",
                    "confortável",
                    "ombros",
                    "ajustado",
                    "vale a pena"
                },

                opcoesIngles = new List<string>
                {
                    "worth it",
                    "shoulders",
                    "fabric",
                    "comfortable",
                    "dry cleaning",
                    "fitted",
                    "cotton blend",
                    "elegant"
                },

                associacaoCorreta = new List<ParAssociacao>
                {
                    new ParAssociacao { palavraPortugues = "tecido", palavraIngles = "fabric" },
                    new ParAssociacao { palavraPortugues = "mistura de algodão", palavraIngles = "cotton blend" },
                    new ParAssociacao { palavraPortugues = "lavagem a seco", palavraIngles = "dry cleaning" },
                    new ParAssociacao { palavraPortugues = "elegante", palavraIngles = "elegant" },
                    new ParAssociacao { palavraPortugues = "confortável", palavraIngles = "comfortable" },
                    new ParAssociacao { palavraPortugues = "ombros", palavraIngles = "shoulders" },
                    new ParAssociacao { palavraPortugues = "ajustado", palavraIngles = "fitted" },
                    new ParAssociacao { palavraPortugues = "vale a pena", palavraIngles = "worth it" }
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

    private void IniciarMissao(string id, string texto, string traducao)
    {
        MissionManager.AdicionarMissaoNPC(id, texto, traducao);
    }

    protected override async Task IniciarInteracao()
    {
        GameProgress.EstaEmDialogo = true;
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
        else
        {
            await interacaoB1();
        }

        GameProgress.EstaEmDialogo = false;

        if (modalLegenda != null)
            modalLegenda.SetActive(false);

        if (textoPularDialogo != null)
            textoPularDialogo.gameObject.SetActive(false);

        LiberarControlePlayer();
    }

    private async Task interacaoA1()
    {
        int i = 0;

        // Missão 1: aparece quando a conversa com o vendedor começa.
        IniciarMissao(
            "roupas_a1_1",
            "Talk to the salesperson in the clothing store",
            "Converse com o vendedor na loja de roupas"
        );

        // Missão 1: conversar com o vendedor e explicar o que procura.
        await PlayAudioETexto(i++);

        exAtual = exerciciosSpeaking[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);
        MissionManager.ConcluirMissaoNPC("roupas_a1_1");

        // Missão 2: informar tamanho e cor e escolher a camiseta.
        IniciarMissao(
            "roupas_a1_2",
            "Choose a T-shirt and tell the salesperson your size and preferred color",
            "Escolha uma camiseta e informe o tamanho e a cor desejados"
        );
        await PlayAudioETexto(i++);

        exAtual = exerciciosAlternativas[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++);

        exAtual = exerciciosWriting[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await AndarAteRoupa("blue_tshirt");
        await PlayAudioETexto(i++);
        MissionManager.ConcluirMissaoNPC("roupas_a1_2");

        // O vendedor apresenta a camiseta e pergunta se o usuário deseja experimentá-la.
        await PlayAudioETexto(i++);

        // Missão 3: aparece quando o usuário já pode começar a experimentar a camiseta.
        IniciarMissao(
            "roupas_a1_3",
            "Try on the T-shirt and explain how it fits",
            "Experimente a camiseta e explique como ela serviu"
        );
        await PlayAudioETexto(i++);

        exAtual = exerciciosSpeaking[1];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++);

        exAtual = exerciciosSpeaking[2];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);
        MissionManager.ConcluirMissaoNPC("roupas_a1_3");

        // O vendedor avisa que vai buscar uma camiseta maior.
        await PlayAudioETexto(i++);

        // Missão 4: aparece quando a camiseta maior é apresentada para a compra.
        IniciarMissao(
            "roupas_a1_4",
            "Choose the T-shirt and complete the purchase",
            "Escolha a camiseta e finalize a compra"
        );
        await PlayAudioETexto(i++);

        exAtual = exerciciosCriarFrases[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++);

        await PlayAudioETexto(i++);
        MissionManager.ConcluirMissaoNPC("roupas_a1_4");

        await AbrirExercicio(ObterTipoExercicio(exerciciosAssociacao[0]), exerciciosAssociacao[0]);
    }

    private async Task interacaoA2()
    {
        int i = 0;

        // Missão 1: aparece quando a conversa com o vendedor começa.
        IniciarMissao(
            "roupas_a2_1",
            "Talk to the salesperson and explain what you are looking for",
            "Converse com o vendedor e explique o que está procurando"
        );

        // Missão 1: explicar o que está procurando.
        await PlayAudioETexto(i++);

        exAtual = exerciciosCriarFrases[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);
        MissionManager.ConcluirMissaoNPC("roupas_a2_1");

        // Missão 2: conhecer as opções, pedir outra cor e verificar tamanho/preço.
        IniciarMissao(
            "roupas_a2_2",
            "Explore the available options and ask for recommendations",
            "Conheça as opções disponíveis e peça recomendações"
        );
        await PlayAudioETexto(i++);

        exAtual = exerciciosSpeaking[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++);

        exAtual = exerciciosSpeaking[1];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++);

        exAtual = exerciciosAlternativas[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++);

        exAtual = exerciciosAlternativas[1];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);
        MissionManager.ConcluirMissaoNPC("roupas_a2_2");

        // Missão 3: começa quando o usuário decide experimentar a camisa.
        IniciarMissao(
            "roupas_a2_3",
            "Try on the shirt and explain if you need a different size",
            "Experimente a camisa e informe se precisa de outro tamanho"
        );

        // O usuário pede para experimentar a camisa antes da resposta do vendedor.
        exAtual = exerciciosSpeaking[2];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++);

        // Missão 3: experimentar, explicar o problema e solicitar outro tamanho.
        await PlayAudioETexto(i++);

        exAtual = exerciciosWriting[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++);

        await AbrirExercicio(ObterTipoExercicio(exerciciosCompletarFrase[0]), exerciciosCompletarFrase[0]);
        MissionManager.ConcluirMissaoNPC("roupas_a2_3");

        // Missão 4: começa quando o vendedor pergunta se o usuário deseja comprar.
        IniciarMissao(
            "roupas_a2_4",
            "Choose the shirt and complete the purchase",
            "Escolha a roupa desejada e finalize a compra"
        );
        await PlayAudioETexto(i++);

        exAtual = exerciciosSpeaking[3];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++);

        exAtual = exerciciosAlternativas[2];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++);
        MissionManager.ConcluirMissaoNPC("roupas_a2_4");

        await AbrirExercicio(ObterTipoExercicio(exerciciosAssociacao[0]), exerciciosAssociacao[0]);
    }

    private async Task interacaoB1()
    {
        int i = 0;

        // Missão 1: aparece quando a conversa com o vendedor começa.
        IniciarMissao(
            "roupas_b1_1",
            "Explain what kind of outfit you need and for what occasion",
            "Explique que tipo de roupa você precisa e para qual ocasião"
        );

        // Missão 1: explicar a ocasião e o tipo de roupa desejado.
        await PlayAudioETexto(i++);

        exAtual = exerciciosSpeaking[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);
        MissionManager.ConcluirMissaoNPC("roupas_b1_1");

        // Missão 2: começa quando o vendedor pergunta sobre preferências.
        IniciarMissao(
            "roupas_b1_2",
            "Explore the options and ask about the characteristics of the clothing",
            "Conheça as opções disponíveis e pergunte sobre as características das peças"
        );
        await PlayAudioETexto(i++);

        exAtual = exerciciosSpeaking[1];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++);

        exAtual = exerciciosAlternativas[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        exAtual = exerciciosWriting[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++);

        exAtual = exerciciosAlternativas[1];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);
        MissionManager.ConcluirMissaoNPC("roupas_b1_2");

        // Missão 3: começa quando o vendedor oferece calças para combinar com o blazer.
        IniciarMissao(
            "roupas_b1_3",
            "Compare the options and ask about price and fabric care",
            "Compare as opções e esclareça suas dúvidas sobre preço e conservação"
        );
        await PlayAudioETexto(i++);

        exAtual = exerciciosSpeaking[2];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++);

        exAtual = exerciciosCriarFrases[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);
        MissionManager.ConcluirMissaoNPC("roupas_b1_3");

        // Missão 4: começa quando o vendedor pergunta se o usuário quer experimentar as peças.
        IniciarMissao(
            "roupas_b1_4",
            "Try on the outfit, explain what needs adjusting, and ask for another size",
            "Experimente a roupa, explique o que precisa ser ajustado e solicite outro tamanho"
        );
        await PlayAudioETexto(i++);

        exAtual = exerciciosSpeaking[3];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++);

        exAtual = exerciciosSpeaking[4];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++);

        await PlayAudioETexto(i++);

        exAtual = exerciciosCompletarFrase[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);
        MissionManager.ConcluirMissaoNPC("roupas_b1_4");

        // Missão 5: começa quando o vendedor pergunta se o usuário deseja comprar as peças.
        IniciarMissao(
            "roupas_b1_5",
            "Choose the outfit that best meets your needs and complete the purchase",
            "Escolha a roupa que melhor atende às suas necessidades e finalize a compra"
        );
        await PlayAudioETexto(i++);

        exAtual = exerciciosSpeaking[5];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++);

        // Primeiro, o usuário informa o valor total; depois, escolhe a forma de pagamento.
        exAtual = exerciciosSpeaking[6];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        exAtual = exerciciosAlternativas[2];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++);
        MissionManager.ConcluirMissaoNPC("roupas_b1_5");

        await AbrirExercicio(ObterTipoExercicio(exerciciosAssociacao[0]), exerciciosAssociacao[0]);
    }

    public async Task AbrirExercicio(TipoExercicio tipo, ExercicioBase ex)
    {
        //Debug.Log("Abrindo exercício: " + tipo);

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

    //MODIFICAR PARA FICAR DO JEITO CORRETO
    private async Task<string> AndarAteRoupa(string nomeRoupa)
    {
       return await Task.Run(() =>
        {
            return "Roupa não encontrada: " + nomeRoupa;
        });
    }

    private async Task PlayAudioETexto(int i)
    {
        ultimaFraseDita = dialogoAtual[i];
        // 1. PASSANDO O ID DO NPC JUNTO COM O TEXTO PARA O BACKEND
        byte[] audioBytes = await backendManager.GerarAudio(dialogoAtual[i], idNpcParaVoz);
        
        if (audioBytes != null && audioBytes.Length > 0)
        {
            //Debug.Log($"Áudio recebido! Tamanho: {audioBytes.Length} bytes");
            string caminho = Path.Combine(Application.persistentDataPath, "audio_temp.wav");
            File.WriteAllBytes(caminho, audioBytes);
            
            UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + caminho, AudioType.WAV);
            var operation = www.SendWebRequest();
            
            while (!operation.isDone)
                await Task.Yield();

            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
            audioSource.clip = clip;

            if (textoPularDialogo != null)
                textoPularDialogo.gameObject.SetActive(false);

            modalLegenda.GetComponent<ModalLegenda>().MostrarLegenda(nomeExibicaoLegenda, dialogoAtual[i]);
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
            
            while (Input.GetMouseButton(0))
            {
                await Task.Yield();
            }
        }
        else
        {
            Debug.LogError("O servidor retornou um array de bytes vazio.");
            modalLegenda.GetComponent<ModalLegenda>().MostrarLegenda(nomeExibicaoLegenda, dialogoAtual[i]);
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
            if (modalLegenda != null)
            {
                modalLegenda.SetActive(true);

                if (textoPularDialogo != null)
                    textoPularDialogo.gameObject.SetActive(false);

                modalLegenda.GetComponent<ModalLegenda>()
                    .MostrarLegenda(nomeExibicaoLegenda, textoParaFalar);
            }
            
            audioSource.Play();
            
            while (audioSource.isPlaying)
            {
                await Task.Yield();
            }

            if (modalLegenda != null)
            {
                modalLegenda.SetActive(false);
            }
        }
    }
}