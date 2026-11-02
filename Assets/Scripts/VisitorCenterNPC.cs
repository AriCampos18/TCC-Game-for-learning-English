using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using TMPro;
using System;

public class VisitorCenterNPC: InteracaoNPC
{
    string nivelAtual;
    string cumprimento;
    int horaAtual = DateTime.Now.Hour;
    List<string> dialogoAtual;

    public string idNpcParaVoz = "tourGuide";
    public Transform pontoChegadaCityPark;

    private bool aguardandoChegadaCityPark;
    private string idMissaoChegadaCityPark;
    private const float DistanciaChegadaCityPark = 2f;

    [System.NonSerialized] public List<ExercicioBase> exerciciosCompletarFrase;
    [System.NonSerialized] public List<ExercicioBase> exerciciosCriarFrases;
    [System.NonSerialized] public List<ExercicioBase> exerciciosSpeaking;
    [System.NonSerialized] public List<ExercicioBase> exerciciosAlternativas;
    [System.NonSerialized] public List<ExercicioBase> exerciciosWriting;
    [System.NonSerialized] public List<ExercicioBase> exerciciosAssociacao;

    protected override void Start()
    {
        base.Start();
        nivelAtual = DadosJogador.nivelUsuario;
        nomeExibicaoLegenda = "Tour Guide";
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

    private void Update()
    {
        if (!aguardandoChegadaCityPark)
            return;

        if (pontoChegadaCityPark == null || movimentoPlayer == null)
            return;

        Vector3 deslocamento = movimentoPlayer.transform.position - pontoChegadaCityPark.position;
        deslocamento.y = 0f;

        if (deslocamento.sqrMagnitude <= DistanciaChegadaCityPark * DistanciaChegadaCityPark)
        {
            MissionManager.ConcluirMissaoNPC(idMissaoChegadaCityPark);
            aguardandoChegadaCityPark = false;
        }
    }

    private void InicializarConteudosPorNivel()
    {
        if (nivelAtual == "A1")
        {
            dialogoAtual = new List<string>()
            {
                "Hello. Can I help you?",
                "Is this your first time here?",
                "Do you want to know how to get there?",
                "EXPLICA A DIREÇÃO DE FORMA SIMPLES"
            };

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "You are a tourist. You are at the tourist information center. Say something about the city.",
                    opcoesFala = new List<string>()
                    {
                        "I need to buy a train ticket\n",
                        "My hotel room is on the third floor\n",
                        "I want to visit some point of the city\n"
                    },
                    respostaCorreta = 2
                }
            );

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "Say that is your first time in the city",
                    opcoesFala = new List<string>()
                    {
                        "Yes, it is\n",
                        "Yes, I do\n",
                        "Yes, it are\n"
                    },
                    respostaCorreta = 0
                }
            );

            exerciciosAlternativas.Add(
                new ExercicioAlternativas()
                {
                    enunciado = "What place ould you like to visit?",
                    alternativas = new List<string>() { 
                        "Living room\r\n", 
                        "City park\r\n", 
                        "University clasroom\r\n", 
                        "Hotel room\r\n" },
                    alternativaCorreta = 1
                }
            );

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "You want directions. What do you say?",
                    opcoesFala = new List<string>()
                    {
                        "Yes, I went yesterday\n",
                        "Yes, plase\n",
                        "No, I am a park\n"
                    },
                    respostaCorreta = 1
                }
            );

            exerciciosAssociacao.Add(
                    new ExercicioAssociacao()
                    {
                        enunciado = "Match the Portuguese word with the English word.",
                        opcoesPort = new List<string>
                        {
                            "cidade",
                            "parque",
                            "museu",
                            "passagem",
                            "ajuda",
                            "direita",
                            "esquerda",
                            "em frente"
                        },
                        opcoesIngles = new List<string>
                        {
                            "help",
                            "city",
                            "park",
                            "left",
                            "straight",
                            "right",
                            "museum",
                            "ticket",
                        },
                        associacaoCorreta = new List<ParAssociacao>
                        {
                        new ParAssociacao { palavraPortugues = "cidade", palavraIngles = "city" },
                        new ParAssociacao { palavraPortugues = "passagem", palavraIngles = "ticket" },
                        new ParAssociacao { palavraPortugues = "parque", palavraIngles = "park" },
                        new ParAssociacao { palavraPortugues = "museu", palavraIngles = "museum" },
                        new ParAssociacao { palavraPortugues = "ajuda", palavraIngles = "help" },
                        new ParAssociacao { palavraPortugues = "em frente", palavraIngles = "straight" },
                        new ParAssociacao { palavraPortugues = "direita", palavraIngles = "right" },
                        new ParAssociacao { palavraPortugues = "esquerda", palavraIngles = "left" }
                        }
                    }
                );
        }
        else if(nivelAtual=="A2")
        {
            dialogoAtual = new List<string>()
            {
                cumprimento+". How can I help you?",
                "Have you visited our city before?",
                "Here are some tourist attractions. The city garden is very popular.",
                "Would you like to know how to get there?",
                "DIREÇÕES UM POUCO MAIS COMPLEXAS QUE NO A1.",
                "No, it isn't far. It's about a ten-minute walk."
            };

            exerciciosCriarFrases.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Arrange the words to form the correct sentence in english: Eu gostaria de saber mais sobre a cidade",
                    blocosPalavras = new List<string>
                    {
                        "would",
                        "city",
                        "breakfast",
                        "I",
                        "more",
                        "like",
                        "dinner",
                        "the",
                        "know",
                        "to",
                        "about",
                        "room",
                        "lunch",
                        "is",
                        "are"
                    },
                    respostaCorreta = "I would like to know more about the city\n"
                }
            );

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "This is your first time in the city. You need help. What do you say?",
                    opcoesFala = new List<string>()
                    {
                        "I am the menu\n",
                        "The clasroom is right there\n",
                        "Never, could you help me?\n"
                    },
                    respostaCorreta = 2
                }
            );

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "You are looking at the tourist attractions in the brochure. Say which place you want to visit.",
                    opcoesFala = new List<string>()
                    {
                        "I need to check in at the hotel\n",
                        "The city park looks interesting\n",
                        "I would like to order a hamburger\n"
                    },
                    respostaCorreta = 1
                }
            );

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "Ask for directions to the city park:",
                    opcoesFala = new List<string>()
                    {
                        "Yes, plase. I am the directions\n",
                        "Yes, plase. How can I get there?\n",
                        "Yes, please. How much is the clothe?\n"
                    },
                    respostaCorreta = 1
                }
            );

            //MUDAR O EXERCICIO ABAIXO DE ACORDO COM AS DIREÇÕES REAIS
            exerciciosAlternativas.Add(
                new ExercicioAlternativas()
                {
                    enunciado = "What should you do after walking two blocks?", //ou a direção correta dada no cenário real
                    alternativas = new List<string>() { 
                        "Go back\r\n", 
                        "Turn right\r\n", 
                        "Turn left\r\n", 
                        "Enter the bank\r\n" },
                    alternativaCorreta = 2
                }
            );

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "You have the directions to the park. Now you want to know the distance. Ask the guide",
                    opcoesFala = new List<string>()
                    {
                        "Is the park far from here?\n",
                        "How much does the park cost?\n",
                        "Is the park far from yesterday?\n"
                    },
                    respostaCorreta = 0
                }
            );

            exerciciosCompletarFrase.Add(
                new ExercicioCompletarFrase()
                {
                    enunciado = "Complete the sentence with the correct word:",
                    fraseAntes = "The park is about ",
                    fraseDepois = " minutes away.",
                    respostaCorreta = "ten",
                    opcoes = new List<string>
                    {
                        "11:00",
                        "served",
                        "have",
                        "Which",
                        "five",
                        "ten",
                        "fifteen",
                        "is",
                        "for",
                        "twenty",
                        "I",
                    }
                }
            );

            exerciciosAssociacao.Add(
                new ExercicioAssociacao()
                {
                    enunciado = "Match the Portuguese word with the English word.",
                    opcoesPort = new List<string>
                    {
                        "ponto turístico",
                        "ajuda",
                        "quarteirão",
                        "virar",
                        "continuar",
                        "do outro lado de",
                        "longe",
                        "caminhada"
                    },
                    opcoesIngles = new List<string>
                    {
                        "far",
                        "turn",
                        "tourist attraction",
                        "walk",
                        "block",
                        "continue",
                        "help",
                        "across from",
                        "straight", // distrator
                        "ticket"    // distrator
                    },
                    associacaoCorreta = new List<ParAssociacao>
                    {
                        new ParAssociacao { palavraPortugues = "ponto turístico", palavraIngles = "tourist attraction" },
                        new ParAssociacao { palavraPortugues = "ajuda", palavraIngles = "help" },
                        new ParAssociacao { palavraPortugues = "quarteirão", palavraIngles = "block" },
                        new ParAssociacao { palavraPortugues = "virar", palavraIngles = "turn" },
                        new ParAssociacao { palavraPortugues = "continuar", palavraIngles = "continue" },
                        new ParAssociacao { palavraPortugues = "do outro lado de", palavraIngles = "across from" },
                        new ParAssociacao { palavraPortugues = "longe", palavraIngles = "far" },
                        new ParAssociacao { palavraPortugues = "caminhada", palavraIngles = "walk" }
                    }
                }
            );
        }
        else if(nivelAtual=="B1")
        {
            dialogoAtual = new List<string>()
            {
                cumprimento+". Welcome to the tourist information center. How can I help you today?",
                "Have you been here before?",
                "If it's your first visit, I'd recommend starting with the city park. It's one of the most famous places in the city.",
                "DIREÇÕES MAIS COMPLEXAS QUE NO A2",
                "If you don't feel like walking, you can also take bus 42. It stops right in front of the park.",
                "I'd go early, though, because it gets crowded in the afternoon."
            };

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "How would you say that you would like to get to know the city?",
                    opcoesFala = new List<string>()
                    {
                        "I'm cooking the city and I'd like a room.\n",
                        "I visit the city yesterday and places can.\n",
                        "I'm visiting the city and I'd like to know what places I can visit.\n"
                    },
                    respostaCorreta = 2
                }
            );

            exerciciosCriarFrases.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Arrange the words to form the correct sentence in english: Não, esta é a primeira vez que visito a cidade",
                    blocosPalavras = new List<string>
                    {
                        "this",
                        "is",
                        "morning",
                        "time",
                        "visiting",
                        "the",
                        "city",
                        "hotel",
                        "last",
                        "No",
                        "your",
                        "been",
                        "first",
                        "my",
                        "ever"
                    },
                    respostaCorreta = "No, this is my first time visiting the city.\n"
                }
            );

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "After choosing the City Park as a tourist attraction to visit, the user must express interest in the place and ask how to get there.",
                    opcoesFala = new List<string>()
                    {
                        "The City Park looks interesting. How can I get there?\n",
                        "The City Park tastes interesting. Can I pay there?\n",
                        "I am interesting where the city park.\n"
                    },
                    respostaCorreta = 0
                }
            );

            exerciciosWriting.Add(
                new ExercicioWriting()
                {
                    enunciado = "You did not understand one part of the directions. Ask the employee to clarify."
                }
            );

            //MUDAR O EXERCICIO ABAIXO PRA FICAR DE ACORDO COM A DIREÇÃO REAL
            exerciciosAlternativas.Add(
                new ExercicioAlternativas()
                {
                    enunciado = "Where is the city park?",
                    alternativas = new List<string>() { 
                        "Behind the bank\r\n", 
                        "Next to the hotel\r\n", 
                        "Across from the museum\r\n", 
                        "Inside the museum\r\n" },
                    alternativaCorreta = 2
                }
            );

            exerciciosAssociacao.Add(
                new ExercicioAssociacao()
                {
                    enunciado = "Match the Portuguese word with the English word.",

                    opcoesPort = new List<string>
                    {
                        "ponto turístico",
                        "arredores",
                        "indicar",
                        "cruzamento",
                        "seguir em frente",
                        "esclarecer",
                        "folheto",
                        "chegar a",
                        "ponto de referência",
                        "direções"
                    },

                    opcoesIngles = new List<string>
                    {
                        "directions",
                        "landmark",
                        "brochure",
                        "reach",
                        "intersection",
                        "clarify",
                        "surroundings",
                        "recommend",
                        "go straight",
                        "tourist attraction"
                    },

                    associacaoCorreta = new List<ParAssociacao>
                    {
                        new ParAssociacao { palavraPortugues = "ponto turístico", palavraIngles = "tourist attraction" },
                        new ParAssociacao { palavraPortugues = "arredores", palavraIngles = "surroundings" },
                        new ParAssociacao { palavraPortugues = "indicar", palavraIngles = "recommend" },
                        new ParAssociacao { palavraPortugues = "cruzamento", palavraIngles = "intersection" },
                        new ParAssociacao { palavraPortugues = "seguir em frente", palavraIngles = "go straight" },
                        new ParAssociacao { palavraPortugues = "esclarecer", palavraIngles = "clarify" },
                        new ParAssociacao { palavraPortugues = "folheto", palavraIngles = "brochure" },
                        new ParAssociacao { palavraPortugues = "chegar a", palavraIngles = "reach" },
                        new ParAssociacao { palavraPortugues = "ponto de referência", palavraIngles = "landmark" },
                        new ParAssociacao { palavraPortugues = "direções", palavraIngles = "directions" }
                    }
                }
            );
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

        // Desativa o estado quando a conversa acabar completamente!
        GameProgress.EstaEmDialogo = false;
        //MissionManager.Instance.ConcluirMissao("finalizar_compra");

        if (modalLegenda != null)
            modalLegenda.SetActive(false);

        if (textoPularDialogo != null)
            textoPularDialogo.gameObject.SetActive(false);

        GameProgress.EstaEmDialogo = false;

        LiberarControlePlayer();
    }

    private void IniciarMissao(string id, string texto, string traducao)
    {
        MissionManager.AdicionarMissaoNPC(id, texto, traducao);
    }

    private void IniciarMissaoChegadaCityPark(string id, string texto, string traducao)
    {
        IniciarMissao(id, texto, traducao);

        idMissaoChegadaCityPark = id;
        aguardandoChegadaCityPark = true;

        if (pontoChegadaCityPark == null)
            Debug.LogWarning("VisitorCenterNPC: atribua o ponto de chegada do City Park no Inspector.");
    }

    private async Task interacaoA1()
    {
        int i = 0;

        IniciarMissao("visitor_center_a1_conversa", "Start a conversation with the employee", "Inicie uma conversa com a funcionária");

        await PlayAudioETexto(i++);

        exAtual = exerciciosSpeaking[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++);

        exAtual = exerciciosSpeaking[1];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        MissionManager.ConcluirMissaoNPC("visitor_center_a1_conversa");

        IniciarMissao("visitor_center_a1_atracoes", "Learn about the city's tourist attractions", "Conheça os pontos turísticos da cidade");

        exAtual = exerciciosAlternativas[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++);

        exAtual = exerciciosSpeaking[2];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        MissionManager.ConcluirMissaoNPC("visitor_center_a1_atracoes");

        IniciarMissaoChegadaCityPark(
            "visitor_center_a1_city_park",
            "Find the City Park",
            "Encontre o City Park"
        );

        await PlayAudioETexto(i++); // Substituir pela direção real até o City Park.

        await AbrirExercicio(ObterTipoExercicio(exerciciosAssociacao[0]), exerciciosAssociacao[0]);
    }

    private async Task interacaoA2()
    {
        int i = 0;

        IniciarMissao("visitor_center_a2_conversa", "Talk to the employee", "Converse com a funcionária");

        await PlayAudioETexto(i++);
        
        exAtual = exerciciosCriarFrases[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++);
        
        exAtual = exerciciosSpeaking[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        MissionManager.ConcluirMissaoNPC("visitor_center_a2_conversa");

        IniciarMissao("visitor_center_a2_atracoes", "Explore the attractions in the brochure", "Conheça as atrações disponíveis no panfleto");

        await PlayAudioETexto(i++);

        await AbrirExercicio(ObterTipoExercicio(exerciciosSpeaking[1]), exerciciosSpeaking[1]);

        MissionManager.ConcluirMissaoNPC("visitor_center_a2_atracoes");

        await PlayAudioETexto(i++);

        await AbrirExercicio(ObterTipoExercicio(exerciciosSpeaking[2]), exerciciosSpeaking[2]);

        IniciarMissaoChegadaCityPark(
            "visitor_center_a2_destino",
            "Follow the directions to the destination",
            "Siga as instruções até o destino"
        );

        await PlayAudioETexto(i++); // Substituir pela direção real até o City Park.

        exAtual = exerciciosAlternativas[0]; //EXERCICIO QUE TEM QUE MUDAR DE ACORDO COM A DIREÇÃO CORRETA DO PARQUE
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await AbrirExercicio(ObterTipoExercicio(exerciciosSpeaking[3]), exerciciosSpeaking[3]);

        await PlayAudioETexto(i++);

        await AbrirExercicio(ObterTipoExercicio(exerciciosCompletarFrase[0]), exerciciosCompletarFrase[0]);

        await AbrirExercicio(ObterTipoExercicio(exerciciosAssociacao[0]), exerciciosAssociacao[0]);
    }

    private async Task interacaoB1()
    {
        int i = 0;

        IniciarMissao("visitor_center_b1_conversa", "Talk to the visitor center employee", "Converse com a funcionária do visitor center");

        await PlayAudioETexto(i++);

        exAtual = exerciciosSpeaking[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++);

        await AbrirExercicio(ObterTipoExercicio(exerciciosCriarFrases[0]), exerciciosCriarFrases[0]);

        MissionManager.ConcluirMissaoNPC("visitor_center_b1_conversa");

        IniciarMissao("visitor_center_b1_escolher_city_park", "Choose the City Park as your destination", "Escolha o City Park como destino");

        await PlayAudioETexto(i++);

        exAtual = exerciciosSpeaking[1];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        MissionManager.ConcluirMissaoNPC("visitor_center_b1_escolher_city_park");

        IniciarMissaoChegadaCityPark(
            "visitor_center_b1_percorrer_trajeto",
            "Understand the detailed directions and follow the route to the City Park",
            "Interprete as instruções detalhadas e percorra o trajeto até chegar ao City Park"
        );

        await PlayAudioETexto(i++); // Substituir pela direção real até o City Park.

        exAtual = exerciciosWriting[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++);

        exAtual = exerciciosAlternativas[0]; //EXERCICIO QUE TEM QUE MUDAR DE ACORDO COM A DIREÇÃO CORRETA DO PARQUE
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++); // Informações adicionais sobre transporte e visita.

        await PlayAudioETexto(i++); // Recomendação sobre o melhor horário para visitar.

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
