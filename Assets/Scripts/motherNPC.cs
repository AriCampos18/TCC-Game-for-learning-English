using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using TMPro;
using System;

public class motherNPC : InteracaoNPC
{
    string nivelAtual;
    string cumprimento;
    int horaAtual = DateTime.Now.Hour;
    List<string> dialogoAtual;

    public string idNpcParaVoz = "mom";

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
        nomeExibicaoLegenda = "Mom";
        exerciciosCompletarFrase = GarantirListaExercicios(exerciciosCompletarFrase);
        exerciciosCriarFrases = GarantirListaExercicios(exerciciosCriarFrases);
        exerciciosSpeaking = GarantirListaExercicios(exerciciosSpeaking);
        exerciciosAlternativas = GarantirListaExercicios(exerciciosAlternativas);
        exerciciosWriting = GarantirListaExercicios(exerciciosWriting);
        exerciciosAssociacao = GarantirListaExercicios(exerciciosAssociacao);
    }

    private void InicializarConteudosPorNivel()
    {
        if (nivelAtual == "A1")
        {
            dialogoAtual = new List<string>()
            {
                "Hey. How are you?",
                "How was your day?",
                "Look! We have a new bathroom.",
                "How is your work?",
                "Dinner is ready!"
            };

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Choose the correct response to the question.",
                        opcoesFala = new List<string>()
                        {
                            "The bathroom is next to the kitchen\n",
                            "I would like to book a flight to London\n",
                            "I’m fine, thank you\n"
                        },
                        respostaCorreta = 2
                    }
            ); 

            exerciciosAlternativas.Add(
                new ExercicioAlternativas()
                {
                        enunciado = "Choose the correct answer to the question.",
                        alternativas = new List<string>() { 
                        "This is my dog\r\n", 
                        "I is good\r\n", 
                        "It was good\r\n", 
                        "It is a bathroom\r\n" },
                        alternativaCorreta = 2
                }
            );  

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Say something nice about the bathroom"
                    }
            ); 

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Tell about your day at work",
                        opcoesFala = new List<string>()
                        {
                            "I’m looking for a new job?\n",
                            "I had a busy day at the office\n",
                            "Yes, please. Could you help me?\n"
                        },
                        respostaCorreta = 1
                    }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Say goodbye to your family"
                    }
            );

            exerciciosAssociacao.Add(
                new ExercicioAssociacao()
                {
                    enunciado = "Match the Portuguese word with the English word.",

                    opcoesPort = new List<string>
                    {
                        "banheiro",
                        "sala de estar",
                        "mãe",
                        "pai",
                        "irmão",
                        "cachorro",
                        "bola",
                        "mesa",
                        "jantar"
                    },

                    opcoesIngles = new List<string>
                    {
                        "dog",
                        "father",
                        "dinner",
                        "living room",
                        "ball",
                        "bathroom",
                        "table",
                        "brother",
                        "mother"
                    },

                    associacaoCorreta = new List<ParAssociacao>
                    {
                        new ParAssociacao { palavraPortugues = "banheiro", palavraIngles = "bathroom" },
                        new ParAssociacao { palavraPortugues = "sala de estar", palavraIngles = "living room" },
                        new ParAssociacao { palavraPortugues = "mãe", palavraIngles = "mother" },
                        new ParAssociacao { palavraPortugues = "pai", palavraIngles = "father" },
                        new ParAssociacao { palavraPortugues = "irmão", palavraIngles = "brother" },
                        new ParAssociacao { palavraPortugues = "cachorro", palavraIngles = "dog" },
                        new ParAssociacao { palavraPortugues = "bola", palavraIngles = "ball" },
                        new ParAssociacao { palavraPortugues = "mesa", palavraIngles = "table" },
                        new ParAssociacao { palavraPortugues = "jantar", palavraIngles = "dinner" }
                    }
                }
            );
        }
        else if(nivelAtual=="A2")
        {
            dialogoAtual = new List<string>()
            {
                "Hi! It's so good to see you. How have you been?",
                "I’m great, thanks. How was your day?",
                "We changed the room while you were away. Do you like it?",
                "How is work going?",
                "Dinner is ready. Come to the table, please."
            };

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Tell your family how you have been feeling and ask how they are doing",
                        opcoesFala = new List<string>()
                        {
                            "The dog needs a new ball to play with\n",
                            "I've been good. How about you?\n",
                            "I would like to reserve a table for two\n"
                        },
                        respostaCorreta = 1
                    }
            );

            exerciciosWriting.Add(
                new ExercicioWriting()
                {
                    enunciado = "Talk about something you did during the day"
                }
            );

            exerciciosAlternativas.Add(
                new ExercicioAlternativas()
                {
                    enunciado = "Which object can you find in the room?",
                    alternativas = new List<string>() { 
                        "Shower\r\n", 
                        "Bedside table\r\n", 
                        "Dining table\r\n", 
                        "Refrigerator\r\n" },
                    alternativaCorreta = 1
                }
            );

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "Describe what you like about the room using a complete sentence."
                }
            );

            exerciciosCriarFrases.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Arrange the words to form the correct sentence in english: Obrigado pelo jantar. Foi ótimo ver vocês.",
                    blocosPalavras = new List<string>
                    {
                        "you",
                        "great",
                        "Thanks",
                        "all",
                        "for",
                        "was",
                        "lunch",
                        "It",
                        "see",
                        "dinner",
                        "nice",
                        "to",
                        "you",
                        "meet",
                        "everyone"
                    },
                    respostaCorreta = "Thanks for dinner. It was great to see you all.\n"
                }
            );

            exerciciosAssociacao.Add(
                new ExercicioAssociacao()
                {
                    enunciado = "Match the Portuguese word or expression with the English equivalent.",

                    opcoesPort = new List<string>
                    {
                        "mudanças",
                        "ultimamente",
                        "ocupado",
                        "passar tempo",
                        "preferir",
                        "chuveiro",
                        "mesa de cabeceira",
                        "rotina",
                        "animal de estimação",
                        "durante o jantar"
                    },

                    opcoesIngles = new List<string>
                    {
                        "spend time",
                        "bedside table",
                        "lately",
                        "pet",
                        "shower",
                        "prefer",
                        "busy",
                        "during dinner",
                        "changes",
                        "routine"
                    },

                    associacaoCorreta = new List<ParAssociacao>
                    {
                        new ParAssociacao { palavraPortugues = "mudanças", palavraIngles = "changes" },
                        new ParAssociacao { palavraPortugues = "ultimamente", palavraIngles = "lately" },
                        new ParAssociacao { palavraPortugues = "ocupado", palavraIngles = "busy" },
                        new ParAssociacao { palavraPortugues = "passar tempo", palavraIngles = "spend time" },
                        new ParAssociacao { palavraPortugues = "preferir", palavraIngles = "prefer" },
                        new ParAssociacao { palavraPortugues = "chuveiro", palavraIngles = "shower" },
                        new ParAssociacao { palavraPortugues = "mesa de cabeceira", palavraIngles = "bedside table" },
                        new ParAssociacao { palavraPortugues = "rotina", palavraIngles = "routine" },
                        new ParAssociacao { palavraPortugues = "animal de estimação", palavraIngles = "pet" },
                        new ParAssociacao { palavraPortugues = "durante o jantar", palavraIngles = "during dinner" }
                    }
                }
            );
        }
        else if(nivelAtual=="B1")
        {
            dialogoAtual = new List<string>()
            {
                "Hey! It's been a while. How have you been?",
                "We made a few changes to the kitchen while you were away. What do you think?",
                "It's been good. My team is working on an interesting project, so the days go by quickly.",
                "Dinner is ready, everyone. Come to the table, please!",
                "It was really nice having you here."
            };

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Tell your family how you have been feeling lately and mention something about your recent routine",
                        opcoesFala = new List<string>()
                        {
                            "I would like to order a hamburger and a soda\n",
                            "Could you tell me where the airport check-in desk is?\n",
                            "I've been doing well, and I've been enjoying my new routine\n"
                        },
                        respostaCorreta = 2
                    }
            );

            exerciciosWriting.Add(
                new ExercicioWriting()
                {
                    enunciado = "Write a short message complimenting your family on the new kitchen and describing what you like about it"
                }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Ask your mother about his recent experiences at work",
                        opcoesFala = new List<string>()
                        {
                            "How have things been going at work, Mom?\n",
                            "Have you had a chance to buy any new clothes lately?\n",
                            "What kind of work does Mom usually do?\n"
                        },
                        respostaCorreta = 0
                    }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Say goodbye to your family after dinner, expressing appreciation for the time you spent together.",
                        opcoesFala = new List<string>()
                        {
                            "It was boring to see you all. I’ll call you soon\n",
                            "It was great seeing everyone again. Thank you for having me\n",
                            "Thanks for dinner. I’ll see you at work tomorrow\n"
                        },
                        respostaCorreta = 1
                    }
            );

            exerciciosAssociacao.Add(
                new ExercicioAssociacao()
                {
                    enunciado = "Match the Portuguese word or expression with the English equivalent.",

                    opcoesPort = new List<string>
                    {
                        "saudade",
                        "ultimamente",
                        "equipe",
                        "projeto",
                        "conquistas",
                        "quintal",
                        "brincar de buscar",
                        "ficar durante a noite",
                        "voltar para casa",
                        "colega"
                    },

                    opcoesIngles = new List<string>
                    {
                        "backyard",
                        "spend the night",
                        "team",
                        "colleague",
                        "lately",
                        "archievements",
                        "miss",
                        "project",
                        "play fetch",
                        "go back home"
                    },

                    associacaoCorreta = new List<ParAssociacao>
                    {
                        new ParAssociacao { palavraPortugues = "saudade", palavraIngles = "miss" },
                        new ParAssociacao { palavraPortugues = "ultimamente", palavraIngles = "lately" },
                        new ParAssociacao { palavraPortugues = "equipe", palavraIngles = "team" },
                        new ParAssociacao { palavraPortugues = "projeto", palavraIngles = "project" },
                        new ParAssociacao { palavraPortugues = "conquistas", palavraIngles = "archievements" },
                        new ParAssociacao { palavraPortugues = "quintal", palavraIngles = "backyard" },
                        new ParAssociacao { palavraPortugues = "brincar de buscar", palavraIngles = "play fetch" },
                        new ParAssociacao { palavraPortugues = "ficar durante a noite", palavraIngles = "spend the night" },
                        new ParAssociacao { palavraPortugues = "voltar para casa", palavraIngles = "go back home" },
                        new ParAssociacao { palavraPortugues = "colega", palavraIngles = "colleague" }
                    }
                }
            );
        }
    }

    protected override async Task IniciarInteracao()
    {
        GameProgress.EstaEmDialogo = true;
        try
        {
            nivelAtual = DadosJogador.nivelUsuario;
            InicializarConteudosPorNivel();

            if (dialogoAtual == null || dialogoAtual.Count == 0)
                throw new InvalidOperationException("O diálogo do NPC não foi inicializado para o nível atual.");

            if (nivelAtual == "A1")
                await interacaoA1();
            else if (nivelAtual == "A2")
                await interacaoA2();
            else
                await interacaoB1();
        }
        finally
        {
            GameProgress.EstaEmDialogo = false;
            if (modalLegendaComum != null)
                modalLegendaComum.SetActive(false);
            if (textoPularDialogoComum != null)
                textoPularDialogoComum.gameObject.SetActive(false);
            LiberarControlePlayer();
        }
    }



    private List<ExercicioBase> ObterFilaExercicios()
    {
        List<ExercicioBase> fila = new List<ExercicioBase>();
        fila.AddRange(exerciciosSpeaking);
        fila.AddRange(exerciciosAlternativas);
        fila.AddRange(exerciciosCompletarFrase);
        fila.AddRange(exerciciosCriarFrases);
        fila.AddRange(exerciciosWriting);
        fila.AddRange(exerciciosAssociacao);
        return fila;
    }

    private TipoExercicio ObterTipoExercicio(ExercicioBase exercicio)
    {
        if (exercicio is ExercicioSpeaking)
            return TipoExercicio.Speaking;
        if (exercicio is ExercicioAlternativas)
            return TipoExercicio.Alternativas;
        if (exercicio is ExercicioBlocos)
            return TipoExercicio.Blocos;
        if (exercicio is ExercicioWriting)
            return TipoExercicio.Writing;
        if (exercicio is ExercicioAssociacao)
            return TipoExercicio.Associacao;

        throw new InvalidOperationException("Tipo de exercício não suportado: " + exercicio.GetType().Name);
    }

    public async Task AbrirExercicio(TipoExercicio tipo, ExercicioBase exercicio)
    {
        Debug.Log("Abrindo exerc\u00edcio: " + tipo);

        ModalLegenda legenda = modalLegendaComum.GetComponent<ModalLegenda>();

        if (modalLegendaComum != null)
        {
            modalLegendaComum.SetActive(false);
        }

        await EntrarModoExercicio();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        modalExercicioComum.Abrir(tipo, exercicio, this);

        if (legenda != null)
        {
            legenda.AjustarPosicaoPeloEstadoDoJogo();
        }

        while (!modalExercicioComum.exercicioFinalizado)
        {
            await Task.Yield();
        }

        if (modalLegendaComum != null)
        {
            modalLegendaComum.SetActive(false);
        }

        await SairModoExercicio();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (legenda != null)
        {
            legenda.AjustarPosicaoPeloEstadoDoJogo();
        }
    }

    private async Task PlayAudioETexto(int indiceDialogo, bool mostrarLegenda = true)
    {
        ultimaFraseDita = dialogoAtual[indiceDialogo];
        byte[] audioBytes = await backendManagerComum.GerarAudio(dialogoAtual[indiceDialogo], idNpcParaVoz);

        if (audioBytes != null && audioBytes.Length > 0)
        {
            Debug.Log($"\u00c1udio recebido! Tamanho: {audioBytes.Length} bytes");
            string caminho = Path.Combine(Application.persistentDataPath, "audio_temp.wav");
            File.WriteAllBytes(caminho, audioBytes);

            UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + caminho, AudioType.WAV);
            var operation = www.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();

            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
            audioSourceComum.clip = clip;

            if (mostrarLegenda)
            {
                textoPularDialogoComum.gameObject.SetActive(false);
                modalLegendaComum.GetComponent<ModalLegenda>().MostrarLegenda(nomeExibicaoLegenda, dialogoAtual[indiceDialogo]);
            }

            audioSourceComum.Play();
            while (audioSourceComum.isPlaying)
            {
                await Task.Yield();
            }

            if (textoPularDialogoComum != null)
            {
                textoPularDialogoComum.gameObject.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            await Task.Delay(150);

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

            if (textoPularDialogoComum != null)
            {
                textoPularDialogoComum.gameObject.SetActive(false);
            }

            if (modalLegendaComum != null)
            {
                modalLegendaComum.SetActive(false);
            }
        }
        else
        {
            Debug.LogError("O servidor retornou um array de bytes vazio.");
            if (mostrarLegenda)
            {
                modalLegendaComum.GetComponent<ModalLegenda>().MostrarLegenda(nomeExibicaoLegenda, dialogoAtual[indiceDialogo]);
            }
        }
    }

    private async Task interacaoA1()
    {
        List<ExercicioBase> filaExercicios = ObterFilaExercicios();
        int indiceExercicio = 0;

        for (int i = 0; i < dialogoAtual.Count; i++)
        {
            await PlayAudioETexto(i);

            int dialogosRestantes = dialogoAtual.Count - i;
            int exerciciosRestantes = filaExercicios.Count - indiceExercicio;
            int exerciciosNesteTrecho =
                (exerciciosRestantes + dialogosRestantes - 1) / dialogosRestantes;

            for (int j = 0; j < exerciciosNesteTrecho; j++)
            {
                ExercicioBase exercicio = filaExercicios[indiceExercicio++];
                await AbrirExercicio(ObterTipoExercicio(exercicio), exercicio);
            }
        }
    }

    private async Task interacaoA2()
    {
        List<ExercicioBase> filaExercicios = ObterFilaExercicios();
        int indiceExercicio = 0;

        for (int i = 0; i < dialogoAtual.Count; i++)
        {
            await PlayAudioETexto(i);

            int dialogosRestantes = dialogoAtual.Count - i;
            int exerciciosRestantes = filaExercicios.Count - indiceExercicio;
            int exerciciosNesteTrecho =
                (exerciciosRestantes + dialogosRestantes - 1) / dialogosRestantes;

            for (int j = 0; j < exerciciosNesteTrecho; j++)
            {
                ExercicioBase exercicio = filaExercicios[indiceExercicio++];
                await AbrirExercicio(ObterTipoExercicio(exercicio), exercicio);
            }
        }
    }

    private async Task interacaoB1()
    {
        List<ExercicioBase> filaExercicios = ObterFilaExercicios();
        int indiceExercicio = 0;

        for (int i = 0; i < dialogoAtual.Count; i++)
        {
            await PlayAudioETexto(i);

            int dialogosRestantes = dialogoAtual.Count - i;
            int exerciciosRestantes = filaExercicios.Count - indiceExercicio;
            int exerciciosNesteTrecho =
                (exerciciosRestantes + dialogosRestantes - 1) / dialogosRestantes;

            for (int j = 0; j < exerciciosNesteTrecho; j++)
            {
                ExercicioBase exercicio = filaExercicios[indiceExercicio++];
                await AbrirExercicio(ObterTipoExercicio(exercicio), exercicio);
            }
        }
    }

    public override async Task FalarFraseCustomizada(string textoParaFalar)
    {
        if (string.IsNullOrEmpty(textoParaFalar)) return;

        byte[] audioBytes = await backendManagerComum.GerarAudio(textoParaFalar, idNpcParaVoz);

        if (audioBytes != null && audioBytes.Length > 0)
        {
            string caminho = Path.Combine(Application.persistentDataPath, "audio_temp_custom.wav");
            File.WriteAllBytes(caminho, audioBytes);

            UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + caminho, AudioType.WAV);
            var operation = www.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();

            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
            audioSourceComum.clip = clip;
            if (modalLegendaComum != null && modalLegendaComum.activeSelf)
            {
                if (textoPularDialogoComum != null) textoPularDialogoComum.gameObject.SetActive(false);
                modalLegendaComum.GetComponent<ModalLegenda>().MostrarLegenda(nomeExibicaoLegenda, textoParaFalar);
            }

            audioSourceComum.Play();

            while (audioSourceComum.isPlaying)
            {
                await Task.Yield();
            }
        }
    }
}
