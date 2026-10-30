using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using TMPro;
using System;

public class SecretaryCollegeNPC: InteracaoNPC
{
    string nivelAtual;
    string cumprimento;
    int horaAtual = DateTime.Now.Hour;
    List<string> dialogoAtual;

    public string idNpcParaVoz = "secretaryCollege";

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
        nomeExibicaoLegenda = "College Secretary";
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
                "Hi. Can I help you?",
                "What is your name?",
                "What course do you want?",
                "What is your date of birth?",
                "Your registration is done.",
                "It is R$500 per month.",
                "Classes start on February 10th.",
                "Your classroom is room 204. It is on the second floor."
            };

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "You are at the college. You want to sign up for a course. What do you say?",
                        opcoesFala = new List<string>()
                        {
                            "I enroll yesterday at the library\n",
                            "I want to enroll\n",
                            "I want to eat my enrollment\n"
                        },
                        respostaCorreta = 1
                    }
            );

            exerciciosWriting.Add(
                new ExercicioWriting()
                {
                    enunciado = "The secretary asks your name. Write your answer."
                }
            );

            exerciciosWriting.Add(
                new ExercicioWriting()
                {
                    enunciado = "The secretary asks what course you want. Write your answer."
                }
            );

            exerciciosWriting.Add(
                new ExercicioWriting()
                {
                    enunciado = "The secretary asks your date of birth. Write your answer."
                }
            );

            exerciciosCriarFrases.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Arrange the words to form the correct sentence in english: Qual é o valor da mensalidade?",
                    blocosPalavras = new List<string>
                    {
                        "What",
                        "tuition",
                        "expensive",
                        "is",
                        "monthly",
                        "cost",
                        "fee",
                        "much",
                        "course",
                        "How",
                        "semester",
                        "pay",
                        "the",
                        "does"
                    },
                    respostaCorreta = "What is the monthly fee?\n"
                }
            );

            exerciciosCriarFrases.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Arrange the words to form the correct sentence in english: Quando as aulas começam?",
                    blocosPalavras = new List<string>
                    {
                        "do",
                        "classes",
                        "exam",
                        "enrollment",
                        "schedule",
                        "where",
                        "begin",
                        "the",
                        "When",
                        "Does",
                        "finish",
                        "students",
                        "start",
                        "semester"
                    },
                    respostaCorreta = "When do classes start?\n"
                }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "You can’t find your class. Ask the secretary.",
                        opcoesFala = new List<string>()
                        {
                            "Where yesterday the stairs?\n",
                            "How much does the hallway cost?\n",
                            "Where is my classroom?\n"
                        },
                        respostaCorreta = 2
                    }
            );

            exerciciosAssociacao.Add(
                new ExercicioAssociacao()
                {
                    enunciado = "Match the Portuguese word with the English word.",

                    opcoesPort = new List<string>
                    {
                        "escritório",
                        "andar",
                        "nome",
                        "curso",
                        "estudante",
                        "sala de aula",
                        "mensalidade",
                        "aula"
                    },

                    opcoesIngles = new List<string>
                    {
                        "class",
                        "name",
                        "office",
                        "student",
                        "floor",
                        "classroom",
                        "tuition",
                        "course"
                    },

                    associacaoCorreta = new List<ParAssociacao>
                    {
                        new ParAssociacao { palavraPortugues = "escritório", palavraIngles = "office" },
                        new ParAssociacao { palavraPortugues = "andar", palavraIngles = "floor" },
                        new ParAssociacao { palavraPortugues = "nome", palavraIngles = "name" },
                        new ParAssociacao { palavraPortugues = "curso", palavraIngles = "course" },
                        new ParAssociacao { palavraPortugues = "estudante", palavraIngles = "student" },
                        new ParAssociacao { palavraPortugues = "sala de aula", palavraIngles = "classroom" },
                        new ParAssociacao { palavraPortugues = "mensalidade", palavraIngles = "tuition" },
                        new ParAssociacao { palavraPortugues = "aula", palavraIngles = "class" }
                    }
                }
            );
        }
        else if(nivelAtual=="A2")
        {
            dialogoAtual = new List<string>()
            {
                cumprimento+". Do you need help with something?",
                "Could I have your name, please?",
                "Which course would you like to study?",
                "What is your date of birth?",
                "Thank you. Your registration is complete. You are now enrolled in Computer Science.",
                "The tuition is R$500 per month.",
                "Classes start on February fifth.",
                "Your classroom is room two hundred four, on the second floor."
            };

            exerciciosCriarFrases.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Arrange the words to form the correct sentence in english: Gostaria de me matricular na universidade.",
                    blocosPalavras = new List<string>
                    {
                        "university",
                        "study",
                        "would",
                        "school",
                        "at",
                        "enroll",
                        "student",
                        "I",
                        "course",
                        "the",
                        "want",
                        "like",
                        "register",
                        "to",
                        "class"
                    },
                    respostaCorreta = "I would like to enroll at the university.\n"
                }
            );

            exerciciosWriting.Add(
                new ExercicioWriting()
                {
                    enunciado = "The secretary asks your name. Write your answer in a complete sentence."
                }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "You want to join the Computer Science course. What do you tell the secretary?",
                        opcoesFala = new List<string>()
                        {
                            "I would like to study a hamburger.\n",
                            "I would like to study Computer Science.\n",
                            "I liked study Computer Science.\n"
                        },
                        respostaCorreta = 1
                    }
            );

            exerciciosWriting.Add(
                new ExercicioWriting()
                {
                    enunciado = "The secretary asks your date of birth. Write your answer in a complete sentence, with the month written out (for example, -May 10th-). "
                }
            );

            exerciciosWriting.Add(
                new ExercicioWriting()
                {
                    enunciado = "You want to know how much the course costs each month. Write a question for the secretary. "
                }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "You want to know the date when your classes begin. What do you ask the secretary?",
                        opcoesFala = new List<string>()
                        {
                            "Which course should I choose?\n",
                            "Where can I pay the tuition?\n",
                            "When do classes start?\n"
                        },
                        respostaCorreta = 2
                    }
            );

            exerciciosAlternativas.Add(
                new ExercicioAlternativas()
                {
                    enunciado = "What is the start date for classes?",
                    alternativas = new List<string>(){ 
                        "February 1st\r\n", 
                        "February 5th\r\n", 
                        "February 10th\r\n", 
                        "February 20th\r\n" },
                    alternativaCorreta = 2
                }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "You need to find your classroom, but you don’t know where it is. Ask the secretary where it is.",
                        opcoesFala = new List<string>()
                        {
                            "Where is my classroom?\n",
                            "When does the library open?\n",
                            "How much is the tuition?\n"
                        },
                        respostaCorreta = 0
                    }
            );

            exerciciosAlternativas.Add(
                new ExercicioAlternativas()
                {
                    enunciado = "Select the correct option for your classroom number, as stated by the secretary.",
                    alternativas = new List<string>(){ 
                        "Room 102\r\n", 
                        "Room 204\r\n", 
                        "Room 301\r\n", 
                        "Room 405\r\n" },
                    alternativaCorreta = 1
                }
            );

            exerciciosAssociacao.Add(
                new ExercicioAssociacao()
                {
                    enunciado = "Match the Portuguese word with the English word.",

                    opcoesPort = new List<string>
                    {
                        "matricular-se",
                        "matrícula",
                        "direções",
                        "repetir",
                        "escadas",
                        "biblioteca",
                        "data de nascimento",
                        "cronograma"
                    },

                    opcoesIngles = new List<string>
                    {
                        "library",
                        "enroll",
                        "schedule",
                        "directions",
                        "date of birth",
                        "stairs",
                        "registration",
                        "repeat"
                    },

                    associacaoCorreta = new List<ParAssociacao>
                    {
                        new ParAssociacao { palavraPortugues = "matricular-se", palavraIngles = "enroll" },
                        new ParAssociacao { palavraPortugues = "matrícula", palavraIngles = "registration" },
                        new ParAssociacao { palavraPortugues = "direções", palavraIngles = "directions" },
                        new ParAssociacao { palavraPortugues = "repetir", palavraIngles = "repeat" },
                        new ParAssociacao { palavraPortugues = "escadas", palavraIngles = "stairs" },
                        new ParAssociacao { palavraPortugues = "biblioteca", palavraIngles = "library" },
                        new ParAssociacao { palavraPortugues = "data de nascimento", palavraIngles = "date of birth" },
                        new ParAssociacao { palavraPortugues = "cronograma", palavraIngles = "schedule" }
                    }
                }
            );
        }
        else if(nivelAtual=="B1")
        {
            dialogoAtual = new List<string>()
            {
                cumprimento+". Are you here to enroll in a course?",
                "Could you give me your full name, please?",
                "Could you also give me your date of birth?",
                "Thank you. I just need to confirm a few details. You are enrolling in Computer Science, correct?",
                "Your enrollment is complete. Your tuition will be 500 dollars per month.",
                "Classes start on February fifteenth in room 325.",
                "Room 325 is on the third floor, just go up the stairs. Good luck with your classes!"
            };

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "The secretary asks if you are here to enroll in a course. You are, and you need her help. What do you say?",
                        opcoesFala = new List<string>()
                        {
                            "I enrolled in Computer Science last year, and I’m here to renew my hotel reservation.\n",
                            "Yes, please. Could you help me? I’m here to enroll in Computer Science\n",
                            "I’m looking for the cafeteria because I need to check in for my flight\n"
                        },
                        respostaCorreta = 1
                    }
            );

            exerciciosWriting.Add(
                new ExercicioWriting()
                {
                    enunciado = "Write your full name, including your first and last names, in a complete sentence."
                }
            );

            exerciciosWriting.Add(
                new ExercicioWriting()
                {
                    enunciado = "Write your date of birth in a complete sentence, with the month written out."
                }
            );

            exerciciosCompletarFrase.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Complete the sentence with the correct word: Yes, ____ __ corret",
                    blocosPalavras = new List<string>
                    {
                        "that",
                        "served",
                        "alright",
                        "Which",
                        "tonight",
                        "don't",
                        "morning",
                        "no",                        
                        "this",
                        "is",
                    },
                    respostaCorreta = "that, is\n"
                }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Your enrollment is complete, but you don't know when your classes begin. Ask the secretary politely.",
                        opcoesFala = new List<string>()
                        {
                            "Could you tell me when the classes start?\n",
                            "Could you tell me when do the classes start?\n",
                            "Could you tell me when the classes started?\n"
                        },
                        respostaCorreta = 0
                    }
            );

            exerciciosAlternativas.Add(
                new ExercicioAlternativas()
                {
                    enunciado = "Which room is your first class in? Choose the correct answer:",
                    alternativas = new List<string>() { 
                        "Room 102\r\n", 
                        "Room 204\r\n", 
                        "Room 301\r\n", 
                        "Room 305\r\n" },
                    alternativaCorreta = 3
                }
            );

            exerciciosWriting.Add(
                new ExercicioWriting()
                {
                    enunciado = "You received your schedule, and your first class is in room 305. You don't know where it is. Write a question to ask the secretary."
                }
            );

            exerciciosAssociacao.Add(
                new ExercicioAssociacao()
                {
                    enunciado = "Match the Portuguese word with the English word.",

                    opcoesPort = new List<string>
                    {
                        "matrícula (efetivada)",
                        "mensalidade",
                        "corredor",
                        "balcão da recepção",
                        "esclarecimento",
                        "confirmar",
                        "detalhes",
                        "nome completo"
                    },

                    opcoesIngles = new List<string>
                    {
                        "hallway",
                        "confirm",
                        "full name",
                        "tuition",
                        "clarification",
                        "details",
                        "enrollment",
                        "reception desk"
                    },

                    associacaoCorreta = new List<ParAssociacao>
                    {
                        new ParAssociacao { palavraPortugues = "matrícula (efetivada)", palavraIngles = "enrollment" },
                        new ParAssociacao { palavraPortugues = "mensalidade", palavraIngles = "tuition" },
                        new ParAssociacao { palavraPortugues = "corredor", palavraIngles = "hallway" },
                        new ParAssociacao { palavraPortugues = "balcão da recepção", palavraIngles = "reception desk" },
                        new ParAssociacao { palavraPortugues = "esclarecimento", palavraIngles = "clarification" },
                        new ParAssociacao { palavraPortugues = "confirmar", palavraIngles = "confirm" },
                        new ParAssociacao { palavraPortugues = "detalhes", palavraIngles = "details" },
                        new ParAssociacao { palavraPortugues = "nome completo", palavraIngles = "full name" }
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
            {
                MissionManager.AdicionarMissaoNPC(
                    "college_a1_02",
                    "Complete your enrollment",
                    "Realize sua matrícula"
                );
            }
            else if (nivelAtual == "A2")
            {
                MissionManager.ConcluirMissaoNPC("college_a2_02");
                MissionManager.AdicionarMissaoNPC(
                    "college_a2_03",
                    "Complete your enrollment",
                    "Faça sua matrícula"
                );
            }
            else
            {
                MissionManager.AdicionarMissaoNPC(
                    "college_b1_02",
                    "Provide your details, confirm your course information, and complete enrollment",
                    "Informe seus dados, confirme as informações do curso e conclua o processo de matrícula"
                );
            }

            if (nivelAtual == "A1")
                await interacaoA1();
            else if (nivelAtual == "A2")
                await interacaoA2();
            else
                await interacaoB1();

            if (nivelAtual == "A1")
                MissionManager.ConcluirMissaoNPC("college_a1_02");
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
        bool cronogramaA2 = nivelAtual == "A2"
            && exercicio.enunciado.Contains("Select the correct option for your classroom number");
        bool cronogramaB1 = nivelAtual == "B1"
            && exercicio.enunciado.Contains("You received your schedule, and your first class is in room 305");

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

        if (cronogramaA2)
            MissionManager.ConcluirMissaoNPC("college_a2_04");

        if (cronogramaB1)
            MissionManager.ConcluirMissaoNPC("college_b1_03");

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
        string fala = dialogoAtual[indiceDialogo];

        if (nivelAtual == "A2" && fala.Contains("registration is complete"))
        {
            MissionManager.ConcluirMissaoNPC("college_a2_03");
            MissionManager.AdicionarMissaoNPC(
                "college_a2_04",
                "Consult your schedule and find the room for your first class",
                "Consulte seu cronograma e identifique a sala da primeira aula"
            );
        }

        if (nivelAtual == "B1" && fala.Contains("enrollment is complete"))
        {
            MissionManager.ConcluirMissaoNPC("college_b1_02");
            MissionManager.AdicionarMissaoNPC(
                "college_b1_03",
                "Consult your schedule, find the first-class room, and ask how to get there",
                "Consulte o cronograma, identifique a sala da primeira aula e pergunte como chegar até ela"
            );
        }

        byte[] audioBytes = await backendManagerComum.GerarAudio(fala, idNpcParaVoz);

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
