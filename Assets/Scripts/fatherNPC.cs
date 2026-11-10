// using System.Collections.Generic;
// using System.Threading.Tasks;
// using UnityEngine;
// using UnityEngine.Networking;
// using System.IO;
// using TMPro;
// using System;

// public class fatherNPC : InteracaoNPC
// {
//     string nivelAtual;
//     string cumprimento;
//     int horaAtual = DateTime.Now.Hour;
//     List<string> dialogoAtual;
//     private AudioSource audioSource;
//     private BackendManager backendManager;
//     public ModalExercicio modalExercicio;
//     public GameObject modalLegenda;
//     public TextMeshProUGUI textoPularDialogo;
//     public ExercicioBase exAtual;

//     [System.NonSerialized] public List<ExercicioBase> exerciciosCompletarFrase;
//     [System.NonSerialized] public List<ExercicioBase> exerciciosCriarFrases;
//     [System.NonSerialized] public List<ExercicioBase> exerciciosSpeaking;
//     [System.NonSerialized] public List<ExercicioBase> exerciciosAlternativas;
//     [System.NonSerialized] public List<ExercicioBase> exerciciosWriting;
//     [System.NonSerialized] public List<ExercicioBase> exerciciosAssociacao;

//     public string idNpcParaVoz = "father";

//     protected override void Start()
//     {
//         base.Start();
//         nivelAtual = DadosJogador.nivelUsuario;
//         nomeExibicaoLegenda = "Father";
//         exerciciosCompletarFrase = GarantirListaExercicios(exerciciosCompletarFrase);
//         exerciciosCriarFrases = GarantirListaExercicios(exerciciosCriarFrases);
//         exerciciosSpeaking = GarantirListaExercicios(exerciciosSpeaking);
//         exerciciosAlternativas = GarantirListaExercicios(exerciciosAlternativas);
//         exerciciosWriting = GarantirListaExercicios(exerciciosWriting);
//         exerciciosAssociacao = GarantirListaExercicios(exerciciosAssociacao);
//     }

//     private void InicializarConteudosPorNivel()
//     {
//         if (nivelAtual == "A1")
//         {
//             dialogoAtual = new List<string>()
//             {
//                 "Hi! Nice to see you.",
//                 "It was nice to see you."
//             };
//         }
//         else if(nivelAtual=="A2")
//         {
//             dialogoAtual = new List<string>()
//             { 
//                 "Work has been busy lately, but I enjoy what I do.",
//                 "It was great to see you. Come back soon!"     
//             };

//             exerciciosSpeaking.Add(
//                     new ExercicioSpeaking()
//                     {
//                         enunciado = "Ask your father how things are going at work",
//                         opcoesFala = new List<string>()
//                         {
//                             "How are things going at work, dad?\n",
//                             "Where is the bathroom in this house?\n",
//                             "What would you like to eat for dinner?\n"
//                         },
//                         respostaCorreta = 0
//                     }
//             );
//         }
//         else if(nivelAtual=="B1")
//         {
//             dialogoAtual = new List<string>()
//             {
//                 "So, how have things been going lately?",
//                 "Work has been going well. I've been busy with a new project, but I enjoy it.",
//                 "Are you staying for the night?",
//             };

//             exerciciosSpeaking.Add(
//                     new ExercicioSpeaking()
//                     {
//                         enunciado = "Tell your family how you have been feeling lately and mention something about your recent routine",
//                         opcoesFala = new List<string>()
//                         {
//                             "I would like to order a hamburger and a soda\n",
//                             "Could you tell me where the airport check-in desk is?\n",
//                             "I've been doing well, and I've been enjoying my new routine\n"
//                         },
//                         respostaCorreta = 2
//                     }
//             );

//             exerciciosSpeaking.Add(
//                     new ExercicioSpeaking()
//                     {
//                         enunciado = "Ask your father about his recent experiences at work",
//                         opcoesFala = new List<string>()
//                         {
//                             "How have things been going at work lately?\n",
//                             "How many projects are you working tomorrow?\n",
//                             "How did you enjoy your work next week?\n"
//                         },
//                         respostaCorreta = 0
//                     }
//             );

//             exerciciosCompletarFrase.Add(
//                 new ExercicioCompletarFrase()
//                 {
//                     enunciado = "Complete the sentence with the correct words:",
//                     partesFrase = new List<string>
//                     {
//                         "I’m ",
//                         ", I have to go ",
//                         " home early."
//                     },
//                     respostaCorreta = "not, back",
//                     opcoes = new List<string>
//                     {
//                         "not",
//                         "served",
//                         "have",
//                         "You",
//                         "here",
//                         "was",
//                         "Which",
//                         "tonight",
//                         "morning",
//                         "is",
//                         "for",
//                         "back",
//                         "I",
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
//         GameProgress.EstaEmDialogo = true;
//         nivelAtual = DadosJogador.nivelUsuario;
//         InicializarConteudosPorNivel();

//         if (nivelAtual == "A1")
//         {
//             await interacaoA1();
//         }
//         else if (nivelAtual == "A2")
//         {
//             await interacaoA2();
//         }
//         else
//         {
//             await interacaoB1();
//         }

//         // Desativa o estado quando a conversa acabar completamente!
//         GameProgress.EstaEmDialogo = false;
//         //MissionManager.Instance.ConcluirMissao("finalizar_compra");

//         if (modalLegenda != null)
//             modalLegenda.SetActive(false);

//         if (textoPularDialogo != null)
//             textoPularDialogo.gameObject.SetActive(false);

//         GameProgress.EstaEmDialogo = false;

//         LiberarControlePlayer();
//     }

//     private async Task interacaoA1()
//     {
//         int i = 0;

//         await PlayAudioETexto(i++);

//         await PlayAudioETexto(i++);
//     }

//     private async Task interacaoA2()
//     {
//         int i = 0;

//         exAtual = exerciciosSpeaking[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

//         await PlayAudioETexto(i++);

//         await PlayAudioETexto(i++);
//     }

//     private async Task interacaoB1()
//     {
//         int i = 0;
        
//         await PlayAudioETexto(i++);

//         exAtual = exerciciosWriting[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

//         exAtual = exerciciosSpeaking[0];
//         await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

//         await PlayAudioETexto(i++);

//         await AbrirExercicio(ObterTipoExercicio(exerciciosCompletarFrase[0]), exerciciosCompletarFrase[0]);
//     }

//     public async Task AbrirExercicio(TipoExercicio tipo, ExercicioBase ex)
//     {
//         //Debug.Log("Abrindo exercício: " + tipo);

//         ModalLegenda legenda = modalLegenda.GetComponent<ModalLegenda>();

//         if (modalLegenda != null)
//         {
//             modalLegenda.SetActive(false);
//         }

//         await EntrarModoExercicio();

//         Cursor.lockState = CursorLockMode.None;
//         Cursor.visible = true;

//         modalExercicio.Abrir(tipo, ex, this);

//         if (legenda != null) 
//         {
//             legenda.AjustarPosicaoPeloEstadoDoJogo(); 
//         }

//         while (!modalExercicio.exercicioFinalizado)
//         {
//             await Task.Yield();
//         }

//         if (modalLegenda != null)
//         {
//             modalLegenda.SetActive(false);
//         }

//         await SairModoExercicio();

//         // ✨ Volta a prender o mouse após terminar o exercício (se o diálogo geral não tiver acabado)
//         Cursor.lockState = CursorLockMode.Locked;
//         Cursor.visible = false;

//         if (legenda != null)
//         {
//             legenda.AjustarPosicaoPeloEstadoDoJogo();
//         }
//     }
    
//     public void PrepararParaFamilia()
//     {
//         nivelAtual = DadosJogador.nivelUsuario;

//         exerciciosCompletarFrase = GarantirListaExercicios(exerciciosCompletarFrase);
//         exerciciosCriarFrases = GarantirListaExercicios(exerciciosCriarFrases);
//         exerciciosSpeaking = GarantirListaExercicios(exerciciosSpeaking);
//         exerciciosAlternativas = GarantirListaExercicios(exerciciosAlternativas);
//         exerciciosWriting = GarantirListaExercicios(exerciciosWriting);
//         exerciciosAssociacao = GarantirListaExercicios(exerciciosAssociacao);

//         exerciciosCompletarFrase.Clear();
//         exerciciosCriarFrases.Clear();
//         exerciciosSpeaking.Clear();
//         exerciciosAlternativas.Clear();
//         exerciciosWriting.Clear();
//         exerciciosAssociacao.Clear();

//         InicializarConteudosPorNivel();
//     }

//     public async Task ExecutarFala(int indice)
//     {
//         await PlayAudioETexto(indice);
//     }

//     public async Task ExecutarExercicio(ExercicioBase exercicio)
//     {
//         await AbrirExercicio(ObterTipoExercicio(exercicio), exercicio);
//     }

//     public void FinalizarInteracaoFamilia()
//     {
//         if (modalLegendaComum != null)
//             modalLegendaComum.SetActive(false);

//         if (textoPularDialogoComum != null)
//             textoPularDialogoComum.gameObject.SetActive(false);

//         LiberarControlePlayer();
//     }

//     private async Task PlayAudioETexto(int i)
//     {
//         ultimaFraseDita = dialogoAtual[i];
//         // 1. PASSANDO O ID DO NPC JUNTO COM O TEXTO PARA O BACKEND
//         byte[] audioBytes = await backendManager.GerarAudio(dialogoAtual[i], idNpcParaVoz);
        
//         if (audioBytes != null && audioBytes.Length > 0)
//         {
//             //Debug.Log($"Áudio recebido! Tamanho: {audioBytes.Length} bytes");
//             string caminho = Path.Combine(Application.persistentDataPath, "audio_temp.wav");
//             File.WriteAllBytes(caminho, audioBytes);
            
//             UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + caminho, AudioType.WAV);
//             var operation = www.SendWebRequest();
            
//             while (!operation.isDone)
//                 await Task.Yield();

//             AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
//             audioSource.clip = clip;

//             if (textoPularDialogo != null)
//                 textoPularDialogo.gameObject.SetActive(false);

//             modalLegenda.GetComponent<ModalLegenda>().MostrarLegenda(nomeExibicaoLegenda, dialogoAtual[i]);
//             audioSource.Play();
//             while (audioSource.isPlaying)
//             {
//                 await Task.Yield();
//             }

//             if (textoPularDialogo != null)
//             {
//                 textoPularDialogo.gameObject.SetActive(true);

//                 Cursor.lockState = CursorLockMode.None;
//                 Cursor.visible = true;
//             }

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


//             if (textoPularDialogo != null)
//             {
//                 textoPularDialogo.gameObject.SetActive(false);
//             }

//             if (modalLegenda != null)
//             {
//                 modalLegenda.SetActive(false);
//             }
            
//             while (Input.GetMouseButton(0))
//             {
//                 await Task.Yield();
//             }
//         }
//         else
//         {
//             Debug.LogError("O servidor retornou um array de bytes vazio.");
//             modalLegenda.GetComponent<ModalLegenda>().MostrarLegenda(nomeExibicaoLegenda, dialogoAtual[i]);
//         }
//     }

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
//             if (modalLegenda != null)
//             {
//                 modalLegenda.SetActive(true);

//                 if (textoPularDialogo != null)
//                     textoPularDialogo.gameObject.SetActive(false);

//                 modalLegenda.GetComponent<ModalLegenda>()
//                     .MostrarLegenda(nomeExibicaoLegenda, textoParaFalar);
//             }
            
//             audioSource.Play();
            
//             while (audioSource.isPlaying)
//             {
//                 await Task.Yield();
//             }

//             if (modalLegenda != null)
//             {
//                 modalLegenda.SetActive(false);
//             }
//         }
//     }
// }

using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using TMPro;
using System;

public class fatherNPC : InteracaoNPC
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

    [System.NonSerialized] public List<ExercicioBase> exerciciosCompletarFrase;
    [System.NonSerialized] public List<ExercicioBase> exerciciosCriarFrases;
    [System.NonSerialized] public List<ExercicioBase> exerciciosSpeaking;
    [System.NonSerialized] public List<ExercicioBase> exerciciosAlternativas;
    [System.NonSerialized] public List<ExercicioBase> exerciciosWriting;
    [System.NonSerialized] public List<ExercicioBase> exerciciosAssociacao;

    public string idNpcParaVoz = "father";

    protected override void Start()
    {
        base.Start();
        nivelAtual = DadosJogador.nivelUsuario;
        nomeExibicaoLegenda = "Father";
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
                "Hi! Nice to see you.",
                "It was nice to see you."
            };
        }
        else if(nivelAtual=="A2")
        {
            dialogoAtual = new List<string>()
            { 
                "Work has been busy lately, but I enjoy what I do.",
                "It was great to see you. Come back soon!"     
            };

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Ask your father how things are going at work",
                        opcoesFala = new List<string>()
                        {
                            "How are things going at work, dad?\n",
                            "Where is the bathroom in this house?\n",
                            "What would you like to eat for dinner?\n"
                        },
                        respostaCorreta = 0
                    }
            );
        }
        else if(nivelAtual=="B1")
        {
            dialogoAtual = new List<string>()
            {
                "So, how have things been going lately?",
                "Work has been going well. I've been busy with a new project, but I enjoy it.",
                "Are you staying for the night?",
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

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Ask your father about his recent experiences at work",
                        opcoesFala = new List<string>()
                        {
                            "How have things been going at work lately?\n",
                            "How many projects are you working tomorrow?\n",
                            "How did you enjoy your work next week?\n"
                        },
                        respostaCorreta = 0
                    }
            );

            exerciciosCompletarFrase.Add(
                new ExercicioCompletarFrase()
                {
                    enunciado = "Complete the sentence with the correct words:",
                    partesFrase = new List<string>
                    {
                        "I’m ",
                        ", I have to go ",
                        " home early."
                    },
                    respostaCorreta = "not, back",
                    opcoes = new List<string>
                    {
                        "not",
                        "served",
                        "have",
                        "You",
                        "here",
                        "was",
                        "Which",
                        "tonight",
                        "morning",
                        "is",
                        "for",
                        "back",
                        "I",
                    }
                }
            );
        }
        else if (nivelAtual == "A1-A2")
{
    dialogoAtual = new List<string>()
    {
        "Hi! It's great to see you again.",
        "I've been busy at work, but things are going well.",
        "Are you staying with us tonight?"
    };

    exerciciosSpeaking.Add(new ExercicioSpeaking()
    {
        enunciado = "Tell your father how you have been feeling.",
        opcoesFala = new List<string>()
        {
            "I've been doing well, Dad. Thanks for asking.\n",
            "I'd like to see the menu, please.\n",
            "Could you tell me where the bathroom is?\n"
        },
        respostaCorreta = 0
    });

    exerciciosSpeaking.Add(new ExercicioSpeaking()
    {
        enunciado = "Ask your father how things are going at work.",
        opcoesFala = new List<string>()
        {
            "How have things been going at work, Dad?\n",
            "What time does the restaurant open?\n",
            "Would you like to order a dessert?\n"
        },
        respostaCorreta = 0
    });

    exerciciosSpeaking.Add(new ExercicioSpeaking()
    {
        enunciado = "Answer your father's question and say that you are staying for the night."
    });
}
else if (nivelAtual == "A2-B1")
{
    dialogoAtual = new List<string>()
    {
        "It's been a while! How have things been going for you?",
        "Work has been quite busy lately. I'm currently involved in a new project, and it's been an interesting experience.",
        "Are you planning to stay overnight, or will you be heading back home later?"
    };

    exerciciosSpeaking.Add(new ExercicioSpeaking()
    {
        enunciado = "Tell your father about something that has changed in your routine recently.",
        opcoesFala = new List<string>()
        {
            "I've been trying to organize my routine better lately.\n",
            "I'd like to reserve a table for two.\n",
            "Could you tell me where the nearest pharmacy is?\n"
        },
        respostaCorreta = 0
    });

    exerciciosSpeaking.Add(new ExercicioSpeaking()
    {
        enunciado = "Ask your father about the project he is currently working on.",
        opcoesFala = new List<string>()
        {
            "What kind of project are you working on at the moment?\n",
            "How many bedrooms does this house have yesterday?\n",
            "Would you like me to order something from the menu?\n"
        },
        respostaCorreta = 0
    });

    exerciciosCompletarFrase.Add(new ExercicioCompletarFrase()
    {
        enunciado = "Complete the sentence with the correct words:",
        partesFrase = new List<string>()
        {
            "I've been ",
            " on a new project, so I've had less free time."
        },
        respostaCorreta = "working",
        opcoes = new List<string>()
        {
            "working",
            "sleeping",
            "cooking",
            "travel",
            "played",
            "work"
        }
    });

    exerciciosSpeaking.Add(new ExercicioSpeaking()
    {
        enunciado = "Answer your father and explain whether you are staying overnight or going home."
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

        await PlayAudioETexto(i++);
    }

    private async Task interacaoA2()
    {
        int i = 0;

        exAtual = exerciciosSpeaking[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

        await PlayAudioETexto(i++);

        await PlayAudioETexto(i++);
    }

    private async Task interacaoB1()
    {
        int i = 0;
        
        await PlayAudioETexto(i++);

        exAtual = exerciciosWriting[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

        exAtual = exerciciosSpeaking[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual); //

        await PlayAudioETexto(i++);

        await AbrirExercicio(ObterTipoExercicio(exerciciosCompletarFrase[0]), exerciciosCompletarFrase[0]);
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

        DefinirCursorDesktop(true);

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
        DefinirCursorDesktop(false);

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

                DefinirCursorDesktop(true);
            }

            await Task.Yield();

            await AguardarAvancoDialogo();


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
