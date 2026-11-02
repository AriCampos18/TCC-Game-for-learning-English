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
                            "Yes, this is my first time here. \n",
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
            IniciarMissao("universidade_a1_recepcao",
                "Converse com o recepcionista e descubra como chegar ao escritório da secretaria",
                "Talk to the receptionist and find out how to get to the secretary's office");
            await interacaoA1();
            MissionManager.ConcluirMissaoNPC("universidade_a1_recepcao");
        }
        else if (nivelAtual == "A2")
        {
            IniciarMissao("universidade_a2_recepcao",
                "Converse com a recepcionista",
                "Talk to the receptionist");
            await interacaoA2();
            MissionManager.ConcluirMissaoNPC("universidade_a2_recepcao");

            IniciarMissao("universidade_a2_encontrar_secretaria",
                "Encontre o escritório da secretaria",
                "Find the secretary's office");
        }
        else
        {
            IniciarMissao("universidade_b1_orientacoes",
                "Peça orientações detalhadas ao recepcionista e esclareça dúvidas sobre a localização do escritório",
                "Ask the receptionist for detailed directions and clarify the location of the office");
            await interacaoB1();
            MissionManager.ConcluirMissaoNPC("universidade_b1_orientacoes");
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

    private async Task interacaoA1()
    {
        int i=0;

        await PlayAudioETexto(i++);

        await AbrirExercicio(ObterTipoExercicio(exerciciosCriarFrases[0]), exerciciosCriarFrases[0]);
        
        await PlayAudioETexto(i++);

        await AbrirExercicio(ObterTipoExercicio(exerciciosSpeaking[0]), exerciciosSpeaking[0]);

        await PlayAudioETexto(i++);

        await AbrirExercicio(ObterTipoExercicio(exerciciosAlternativas[0]), exerciciosAlternativas[0]);
    }

    private async Task interacaoA2()
    {
        int i = 0;

        await PlayAudioETexto(i++);

        await AbrirExercicio(ObterTipoExercicio(exerciciosCriarFrases[0]), exerciciosCriarFrases[0]);

        await PlayAudioETexto(i++);

        await AbrirExercicio(ObterTipoExercicio(exerciciosSpeaking[0]), exerciciosSpeaking[0]);

        // A recepcionista repete as direções após o pedido do usuário.
        await PlayAudioETexto(1);

        await AbrirExercicio(ObterTipoExercicio(exerciciosSpeaking[1]), exerciciosSpeaking[1]);

        await PlayAudioETexto(i++);
    }

    private async Task interacaoB1()
    {
        int i = 0;

        await PlayAudioETexto(i++);

        await AbrirExercicio(ObterTipoExercicio(exerciciosSpeaking[0]), exerciciosSpeaking[0]);

        await PlayAudioETexto(i++);

        await AbrirExercicio(ObterTipoExercicio(exerciciosSpeaking[1]), exerciciosSpeaking[1]);

        await PlayAudioETexto(i++);

        await AbrirExercicio(ObterTipoExercicio(exerciciosSpeaking[2]), exerciciosSpeaking[2]);

        // A recepcionista confirma a informação repetindo as direções detalhadas.
        await PlayAudioETexto(2);

        await PlayAudioETexto(i++);

        await AbrirExercicio(ObterTipoExercicio(exerciciosSpeaking[3]), exerciciosSpeaking[3]);
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
