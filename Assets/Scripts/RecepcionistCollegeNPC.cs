using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using TMPro;
using System;

public class RecepcionistCollegeNPC: InteracaoNPC
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

    public string idNpcParaVoz = "recepcionistCollege";

    protected override void Start()
    {
        base.Start();
        nivelAtual = DadosJogador.nivelUsuario;
        nomeExibicaoLegenda = "College Recepcionist";
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
                "Hello. Can I help you?",
                "The secretary's office is on the first floor",
                "Go straight and turn right."
            };

            exerciciosCriarFrases.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Arrange the words to form the correct sentence in english: Onde fica o escritório da secretária?",
                    blocosPalavras = new List<string>
                    {
                        "Where",
                        "located",
                        "manager",
                        "is",
                        "secretary's",
                        "meeting",
                        "office",
                        "How",
                        "floor",
                        "the",
                        "reception",
                        "does",
                        "find",
                        "When"
                    },
                    respostaCorreta = "Where is the secretary's office located?\n"
                }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "How do you ask for directions for secretary's office?",
                        opcoesFala = new List<string>()
                        {
                            "How do I get there?\n",
                            "Where can I find the student cafeteria?\n",
                            "Is the library open on Sundays?\n"
                        },
                        respostaCorreta = 0
                    }
            );

            exerciciosAlternativas.Add(
                new ExercicioAlternativas()
                {
                    enunciado = "Choose the correct option to express thanks",
                    alternativas = new List<string>(){ 
                        "I am a student\r\n", 
                        "It is a library\r\n", 
                        "Thank you\r\n", 
                        "My class is at eight\r\n" },
                    alternativaCorreta = 2
                }
            );
        }
        else if(nivelAtual=="A2")
        {
            dialogoAtual = new List<string>()
            {
                cumprimento+". Welcome to the university. How can I help you?",
                "The secretary's office is on the first floor. Go straight and take the first door on your right.",
                "You're welcome. Have a nice day!" 
            };

            exerciciosCriarFrases.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Arrange the words to form the correct sentence in english: Eu gostaria de encontrar o escritório da secretária.",
                    blocosPalavras = new List<string>
                    {
                        "I",
                        "library",
                        "secretary's",
                        "to",
                        "find",
                        "the",
                        "would",
                        "office",
                        "classroom",
                        "teacher",
                        "Where",
                        "student",
                        "is",
                        "like",
                    },
                    respostaCorreta = "I would like to find the secretary's office.\n"
                }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "You did not understand the directions. Ask the receptionist to repeat them.",
                        opcoesFala = new List<string>()
                        {
                            "Where can I buy a notebook?\n",
                            "What time does the library close?\n",
                            "Could you repeat the directions, please?\n"
                        },
                        respostaCorreta = 2
                    }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "The secretary helped you. Thank her and say goodbye"
                    }
            );
        }
        else if(nivelAtual=="B1")
        {
            dialogoAtual = new List<string>()
            {
                cumprimento+". Welcome to the university. How may I help you today?",
                "Are you a new student?",
                "The secretary's office is on the first floor. Go straight past the reception desk and you'll see a door on your right. The secretary can help you with enrollment inside.",
                "If you get lost, just come back and I'll help you."
            };

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "The receptionist asks how she can help you. Explain that you need to find the secretary’s office and say why you are there.",
                        opcoesFala = new List<string>()
                        {
                            " I need to eat the secretary's office because I enroll yesterday.\n",
                            "I need to go to the secretary's office because I have to enroll in a course. \n",
                            "I am a course at the second floor.\n"
                        },
                        respostaCorreta = 1
                    }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "This is your first visit to the university. Answer her question.",
                        opcoesFala = new List<string>()
                        {
                            "No, this is my first time here. \n",
                            "I need to enroll in Computer Science.\n",
                            "The library closes at six o’clock.\n"
                        },
                        respostaCorreta = 0
                    }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "You are not sure which door is the secretary's office. Ask the receptionist for clarification.",
                        opcoesFala = new List<string>()
                        {
                            "Excuse me, could you tell me what time the secretary’s office opens?\n",
                            "Excuse me, I don’t know where is the secretary’s office door\n",
                            "Sorry, could you repeat which door is the secretary’s office?\n"
                        },
                        respostaCorreta = 2
                    }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "The secretary has helped you with everything you needed. Thank her for her help and end the conversation politely."
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

            string idMissaoInicial = "college_" + nivelAtual.ToLowerInvariant() + "_01";
            string textoMissaoInicial = nivelAtual == "A1"
                ? "Talk to the receptionist and find out how to get to the secretary's office"
                : nivelAtual == "A2"
                    ? "Talk to the receptionist"
                    : "Ask the receptionist for detailed directions and clarify the office location";
            string traducaoMissaoInicial = nivelAtual == "A1"
                ? "Converse com o recepcionista e descubra como chegar ao escritório da secretaria"
                : nivelAtual == "A2"
                    ? "Converse com a recepcionista"
                    : "Peça orientações detalhadas ao recepcionista e esclareça dúvidas sobre a localização do escritório";
            MissionManager.AdicionarMissaoNPC(idMissaoInicial, textoMissaoInicial, traducaoMissaoInicial);

            if (nivelAtual == "A1")
                await interacaoA1();
            else if (nivelAtual == "A2")
                await interacaoA2();
            else
                await interacaoB1();

            if (nivelAtual == "A1")
                MissionManager.ConcluirMissaoNPC(idMissaoInicial);
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
        if (exercicio is ExercicioCompletarFrase)
            return TipoExercicio.CompletarFrase;
        if (exercicio is ExercicioWriting)
            return TipoExercicio.Writing;
        if (exercicio is ExercicioAssociacao)
            return TipoExercicio.Associacao;

        throw new InvalidOperationException("Tipo de exercício não suportado: " + exercicio.GetType().Name);
    }

    public async Task AbrirExercicio(TipoExercicio tipo, ExercicioBase exercicio)
    {
        Debug.Log("Abrindo exerc\u00edcio: " + tipo);
        bool orientacaoA2 = nivelAtual == "A2"
            && exercicio.enunciado.Contains("Eu gostaria de encontrar o escritório da secretária");
        bool esclarecimentoB1 = nivelAtual == "B1"
            && exercicio.enunciado.Contains("Ask the receptionist for clarification");

        if (orientacaoA2)
        {
            MissionManager.ConcluirMissaoNPC("college_a2_01");
            MissionManager.AdicionarMissaoNPC(
                "college_a2_02",
                "Find the secretary's office",
                "Encontre o escritório da secretaria"
            );
        }

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

        if (esclarecimentoB1)
            MissionManager.ConcluirMissaoNPC("college_b1_01");

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
