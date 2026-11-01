using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using TMPro;
using System;

public class brotherNPC : InteracaoNPC
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

    public string idNpcParaVoz = "brother";

    protected override void Start()
    {
        base.Start();
        nivelAtual = DadosJogador.nivelUsuario;
        nomeExibicaoLegenda = "Brother";
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
                "Hi! I am happy to see you.",  
                "I usually do my homework and then play with the dog.",
                "He likes playing with his ball."
            };

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Ask your brother about his daily routine after school",
                        opcoesFala = new List<string>()
                        {
                            "What do you usually do after school?\n",
                            "What is your favorite subject at school?\n",
                            "Do you have homework tonight?\n"
                        },
                        respostaCorreta = 0
                    }
            ); 

            exerciciosCompletarFrase.Add(
                new ExercicioCompletarFrase()
                {
                    enunciado = "Complete the sentence with the correct word:",
                    fraseAntes = "What does the dog ",
                    fraseDepois = " to play with?",
                    respostaCorreta = "like",
                    opcoes = new List<string>
                    {
                        "want",
                        "fetch",
                        "like",
                        "eat",
                        "sleep",
                        "need",
                        "run",
                        "have",
                        "walk",
                        "play",
                        "likes"
                    }
                }
            );

            exerciciosAlternativas.Add(
                new ExercicioAlternativas()
                {
                        enunciado = "What does the dog play with?",
                        alternativas = new List<string>() { 
                        "A ball\r\n", 
                        "A book\r\n", 
                        "A chair\r\n", 
                        "A shoe\r\n" },
                        alternativaCorreta = 0
                }
            ); 
        }
        else if(nivelAtual=="A2")
        {
            dialogoAtual = new List<string>()
            {
                "Hey! Long time no see.", 
                "I like school, but I prefer spending time playing with the dog.",
                "He really likes his ball. Yesterday we played in the backyard for an hour."
            };

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Ask your brother how he is doing at school.",
                        opcoesFala = new List<string>()
                        {
                            "How much does your school lunch cost yesterday?\n",
                            "How have things been going at school lately?\n",
                            "Where is the school yesterday going?\n"
                        },
                        respostaCorreta = 1
                    }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Ask your brother what his dog likes to play with."
                    }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Talk about the pets you have or the animals you would like to have."
                    }
            );
        }
        else if(nivelAtual=="B1")
        {
            dialogoAtual = new List<string>()
            {
                "Hey! It's been a while. I miss you.",
                "I like school, but honestly I'd rather spend time with the dog after classes.",
                "He loves playing fetch with his ball and running around in the backyard."
            };

            exerciciosCriarFrases.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Arrange the words to form the correct sentence in english: Como as coisas têm ido na escola ultimamente?",
                    blocosPalavras = new List<string>
                    {
                        "lately",
                        "been",
                        "school",
                        "How",
                        "things",
                        "at",
                        "going",
                        "have",
                        "work",
                        "is",
                        "your",
                        "doing",
                        "the",
                        "well"
                    },
                    respostaCorreta = "How have things been going at school lately?\n"
                }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Ask your brother about the activities the dog enjoys doing lately.",
                        opcoesFala = new List<string>()
                        {
                            "How many classes does the dog have?\n",
                            "Where does the dog study?\n",
                            "What does the dog like doing these days?\n"
                        },
                        respostaCorreta = 2
                    }
            );

            exerciciosWriting.Add(
                new ExercicioWriting()
                {
                    enunciado = "Write a short paragraph about your relationship with animals, describing your experiences with pets or the animals you would like to have"
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

    private async Task interacaoA1()
    {
        int i = 0;

        await PlayAudioETexto(i++);

        exAtual = exerciciosSpeaking[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

        await PlayAudioETexto(i++);

        exAtual = exerciciosCompletarFrase[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

        await PlayAudioETexto(i++);

        exAtual = exerciciosAlternativas[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //
    }

    private async Task interacaoA2()
    {
        int i = 0;

        await PlayAudioETexto(i++);
        
        exAtual = exerciciosSpeaking[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

        await PlayAudioETexto(i++);
        
        exAtual = exerciciosSpeaking[1];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

        await PlayAudioETexto(i++);

        await AbrirExercicio(ObterTipoExercicio(exerciciosWriting[0]), exerciciosWriting[0]); //
    }

    private async Task interacaoB1()
    {
        int i = 0;
        
        await PlayAudioETexto(i++);

        exAtual = exerciciosCriarFrases[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

        await PlayAudioETexto(i++);

        await AbrirExercicio(ObterTipoExercicio(exerciciosSpeaking[0]), exerciciosSpeaking[0]); //

        await PlayAudioETexto(i++);

        exAtual = exerciciosWriting[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //
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
    
    public void PrepararParaFamilia()
    {
        nivelAtual = DadosJogador.nivelUsuario;

        exerciciosCompletarFrase = GarantirListaExercicios(exerciciosCompletarFrase);
        exerciciosCriarFrases = GarantirListaExercicios(exerciciosCriarFrases);
        exerciciosSpeaking = GarantirListaExercicios(exerciciosSpeaking);
        exerciciosAlternativas = GarantirListaExercicios(exerciciosAlternativas);
        exerciciosWriting = GarantirListaExercicios(exerciciosWriting);
        exerciciosAssociacao = GarantirListaExercicios(exerciciosAssociacao);

        exerciciosCompletarFrase.Clear();
        exerciciosCriarFrases.Clear();
        exerciciosSpeaking.Clear();
        exerciciosAlternativas.Clear();
        exerciciosWriting.Clear();
        exerciciosAssociacao.Clear();

        InicializarConteudosPorNivel();
    }

    public async Task ExecutarFala(int indice)
    {
        await PlayAudioETexto(indice);
    }

    public async Task ExecutarExercicio(ExercicioBase exercicio)
    {
        await AbrirExercicio(ObterTipoExercicio(exercicio), exercicio);
    }

    public void FinalizarInteracaoFamilia()
    {
        if (modalLegendaComum != null)
            modalLegendaComum.SetActive(false);

        if (textoPularDialogoComum != null)
            textoPularDialogoComum.gameObject.SetActive(false);

        LiberarControlePlayer();
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
