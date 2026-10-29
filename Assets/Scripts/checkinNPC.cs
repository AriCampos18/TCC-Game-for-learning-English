using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using TMPro;
using System;

public class checkinNPC : InteracaoNPC
{
    string nivelAtual;
    string cumprimento;
    int horaAtual = DateTime.Now.Hour;
    List<string> dialogoAtual;

    [System.NonSerialized] public List<ExercicioBase> exerciciosCompletarFrase;
    [System.NonSerialized] public List<ExercicioBase> exerciciosCriarFrases;
    [System.NonSerialized] public List<ExercicioBase> exerciciosSpeaking;
    [System.NonSerialized] public List<ExercicioBase> exerciciosAlternativas;
    [System.NonSerialized] public List<ExercicioBase> exerciciosWriting;
    [System.NonSerialized] public List<ExercicioBase> exerciciosAssociacao;

    public string idNpcParaVoz = "airportInfo";

    protected override void Start()
    {
        base.Start();
        nivelAtual = DadosJogador.nivelUsuario;
        nomeExibicaoLegenda = "Airport Employee";
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
                "Hello. Where are you going?", 
                "Can I see your passport, please?",
                "Here is your boarding pass. Your gate is twelve."
            };

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Show your passport to the airport staff.",
                        opcoesFala = new List<string>()
                        {
                            "Here is my passport\n",
                            "Here is my breakfast\n",
                            "My passport is a boarding gate\n"
                        },
                        respostaCorreta = 0
                    }
            );

            exerciciosCompletarFrase.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Complete the sentence with the correct word: Your boarding gate is ____.",
                    blocosPalavras = new List<string>
                    {
                        "20",
                        "12",
                        "passport",
                        "18",
                        "boarding",
                        "21",
                        "airport",
                        "2",
                        "24",
                        "ticket"
                    },
                    respostaCorreta = "12\n"
                }
            );

            exerciciosAssociacao.Add(
                new ExercicioAssociacao()
                {
                    enunciado = "Match the Portuguese word with the English word.",

                    opcoesPort = new List<string>
                    {
                        "passaporte",
                        "aeroporto",
                        "portão de embarque",
                        "cartão de embarque",
                        "voo",
                        "bagagem",
                        "balcão de check-in",
                        "escadas"
                    },

                    opcoesIngles = new List<string>
                    {
                        "flight",
                        "stairs",
                        "passport",
                        "check-in desk",
                        "luggage",
                        "boarding pass",
                        "airport",
                        "boarding gate"
                    },

                    associacaoCorreta = new List<ParAssociacao>
                    {
                        new ParAssociacao { palavraPortugues = "passaporte", palavraIngles = "passport" },
                        new ParAssociacao { palavraPortugues = "aeroporto", palavraIngles = "airport" },
                        new ParAssociacao { palavraPortugues = "portão de embarque", palavraIngles = "boarding gate" },
                        new ParAssociacao { palavraPortugues = "cartão de embarque", palavraIngles = "boarding pass" },
                        new ParAssociacao { palavraPortugues = "voo", palavraIngles = "flight" },
                        new ParAssociacao { palavraPortugues = "bagagem", palavraIngles = "luggage" },
                        new ParAssociacao { palavraPortugues = "balcão de check-in", palavraIngles = "check-in desk" },
                        new ParAssociacao { palavraPortugues = "escadas", palavraIngles = "stairs" }
                    }
                }
            );
        }
        else if(nivelAtual=="A2")
        {
            dialogoAtual = new List<string>()
            {
                cumprimento+". Where are you flying today?",
                "May I see your passport and booking confirmation, please?",
                "Do you have any bags to check in?",
                "Would you like a window seat or an aisle seat?", 
                "Your gate is eighteen, and boarding starts at 2:30 p.m. Have a nice flight!"
            };

            exerciciosCriarFrases.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Arrange the words to form the correct sentence in english: Eu estou indo para Londres",
                    blocosPalavras = new List<string>
                    {
                        "London",
                        "going",
                        "to",
                        "I",
                        "Paris",
                        "am",
                        "traveling",
                        "from",
                        "is",
                        "fly",
                        "the"
                    },
                    respostaCorreta = "I am going to London\n"
                }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "The airport staff asks to see your passport and booking confirmation. Choose the most appropriate response.",
                        opcoesFala = new List<string>()
                        {
                            "Here is my passport and booking confirmation\n",
                            "I left my booking confirmation at the hotel\n",
                            "I showed my passport to the hotel receptionist last week\n"
                        },
                        respostaCorreta = 1
                    }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "The airport staff asks if you have any luggage to check in. Choose the most appropriate response.",
                        opcoesFala = new List<string>()
                        {
                            "Yes, I need to check in at gate 18\n",
                            "Yes, I have one suitcase to check in\n",
                            "Yes, I have already arrived at my destination\n"
                        },
                        respostaCorreta = 1
                    }
            );

            exerciciosAlternativas.Add(
                    new ExercicioAlternativas()
                    {
                        enunciado = "What time does your boarding start?",
                        alternativas = new List<string>() { 
                        "Two p.m.\r\n", 
                        "Two thirdy p.m.\r\n", 
                        "Three thirdy p.m.\r\n", 
                        "Eleven thirdy p.m.\r\n" },
                        alternativaCorreta = 1
                    }
            );     

            exerciciosCompletarFrase.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Complete the sentence with the correct word:  The boarding gate is ____.",
                    blocosPalavras = new List<string>
                    {
                        "passport",
                        "24",
                        "18",
                        "luggage",
                        "12",
                        "ticket",
                        "8",
                        "28",
                        "airport",
                        "21",
                        "boarding",
                        "80"
                    },
                    respostaCorreta = "18\n"
                }
            );

            exerciciosAssociacao.Add(
                new ExercicioAssociacao()
                {
                    enunciado = "Match the Portuguese word or expression with the English equivalent.",

                    opcoesPort = new List<string>
                    {
                        "mala de viagem",
                        "confirmação da reserva",
                        "despachar bagagem",
                        "embarque",
                        "destino",
                        "horário",
                        "portão",
                        "seguir para"
                    },

                    opcoesIngles = new List<string>
                    {
                        "boarding",
                        "head to",
                        "suitcase",
                        "gate",
                        "departure time",
                        "destination",
                        "booking confirmation",
                        "check in a bag"
                    },

                    associacaoCorreta = new List<ParAssociacao>
                    {
                        new ParAssociacao { palavraPortugues = "mala de viagem", palavraIngles = "suitcase" },
                        new ParAssociacao { palavraPortugues = "confirmação da reserva", palavraIngles = "booking confirmation" },
                        new ParAssociacao { palavraPortugues = "despachar bagagem", palavraIngles = "check in a bag" },
                        new ParAssociacao { palavraPortugues = "embarque", palavraIngles = "boarding" },
                        new ParAssociacao { palavraPortugues = "destino", palavraIngles = "destination" },
                        new ParAssociacao { palavraPortugues = "horário", palavraIngles = "departure time" },
                        new ParAssociacao { palavraPortugues = "portão", palavraIngles = "gate" },
                        new ParAssociacao { palavraPortugues = "seguir para", palavraIngles = "head to" }
                    }
                }
            );
        }
        else if(nivelAtual=="B1")
        {
            dialogoAtual = new List<string>()
            {
                cumprimento+". Where are you travelling today?",
                "Could I see your passport and booking confirmation, please?",
                "Would you prefer a window or an aisle seat? Are you checking any bags today?",
                "You have a connecting flight, so your checked bag will be transferred automatically. Your second flight leaves from gate 24.",
                "Please check the departure screens in case the gate changes. Boarding usually starts 40 minutes before departure."
            };

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Explain your travel plans to the airport staff, mentioning where is your final destination",
                        opcoesFala = new List<string>()
                        {
                            " I’m travelling to Toronto, and I have a connecting flight\n",
                            "I have a stop in Toronto before my final destination\n",
                            "My connecting flight depart from different terminal\n"
                        },
                        respostaCorreta = 0
                    }
            ); 

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "The airport staff asks for your passport and booking confirmation. You have both documents ready. Choose the most appropriate response.",
                    opcoesFala = new List<string>()
                    {
                        "Certainly. I have both documents with me, and I can show them to you now.\n",
                        "I need to collect my suitcase before I can show you my passport.",
                        "My connecting flight leaves from another terminal, so I booked a hotel nearby."
                    },
                    respostaCorreta = 0
                }
            );

            exerciciosAlternativas.Add(
                    new ExercicioAlternativas()
                    {
                        enunciado = "What will happen to the checked bag?",
                        alternativas = new List<string>() { 
                        "The user must collect it before boarding the first flight\r\n", 
                        "It will be transferred automatically\r\n", 
                        " It will be sent to the information desk\r\n", 
                        "It must be carried as hand luggage\r\n" },
                        alternativaCorreta = 1
                    }
            );   

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "The airport staff advises you to check the departure screens for updates about your connecting flight. Choose the most appropriate response.",
                    opcoesFala = new List<string>()
                    {
                        "hank you. I checked the gate tomorrow\n",
                        "Thank you. I’ll check the restaurant menu",
                        "Thank you. I’ll check the departure screens"
                    },
                    respostaCorreta = 2
                }
            );

            exerciciosAssociacao.Add(
                new ExercicioAssociacao()
                {
                    enunciado = "Match the Portuguese word or expression with the English equivalent.",

                    opcoesPort = new List<string>
                    {
                        "voo de conexão",
                        "destino final",
                        "bagagem despachada",
                        "ser transferido automaticamente",
                        "telas de partidas",
                        "atrasado",
                        "confirmação da reserva",
                        "mudança de portão",
                        "balcão de informações",
                        "terminal"
                    },

                    opcoesIngles = new List<string>
                    {
                        "departure screens",
                        "checked baggage",
                        "terminal",
                        "booking confirmation",
                        "connecting flight",
                        "gate change",
                        "information desk",
                        "delayed",
                        "final destination",
                        "be transferred automatically"
                    },

                    associacaoCorreta = new List<ParAssociacao>
                    {
                        new ParAssociacao { palavraPortugues = "voo de conexão", palavraIngles = "connecting flight" },
                        new ParAssociacao { palavraPortugues = "destino final", palavraIngles = "final destination" },
                        new ParAssociacao { palavraPortugues = "bagagem despachada", palavraIngles = "checked baggage" },
                        new ParAssociacao { palavraPortugues = "ser transferido automaticamente", palavraIngles = "be transferred automatically" },
                        new ParAssociacao { palavraPortugues = "telas de partidas", palavraIngles = "departure screens" },
                        new ParAssociacao { palavraPortugues = "atrasado", palavraIngles = "delayed" },
                        new ParAssociacao { palavraPortugues = "confirmação da reserva", palavraIngles = "booking confirmation" },
                        new ParAssociacao { palavraPortugues = "mudança de portão", palavraIngles = "gate change" },
                        new ParAssociacao { palavraPortugues = "balcão de informações", palavraIngles = "information desk" },
                        new ParAssociacao { palavraPortugues = "terminal", palavraIngles = "terminal" }
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
