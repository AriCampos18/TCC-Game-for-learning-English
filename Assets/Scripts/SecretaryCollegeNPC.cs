// using System.Collections.Generic;
// using System.Threading.Tasks;
// using UnityEngine;
// using UnityEngine.Networking;
// using System.IO;
// using TMPro;
// using System;

// public class SecretaryCollegeNPC: InteracaoNPC
// {
//     string nivelAtual;
//     string cumprimento;
//     int horaAtual = DateTime.Now.Hour;
//     public TextMeshProUGUI textoPularDialogo;
//     public ModalExercicio modalExercicio;
//     private BackendManager backendManager;
//     private AudioSource audioSource;
//     List<string> dialogoAtual;

//     public string idNpcParaVoz = "secretaryCollege";
//     public GameObject modalLegenda;

//     [System.NonSerialized] public List<ExercicioBase> exerciciosCompletarFrase;
//     [System.NonSerialized] public List<ExercicioBase> exerciciosCriarFrases;
//     [System.NonSerialized] public List<ExercicioBase> exerciciosSpeaking;
//     [System.NonSerialized] public List<ExercicioBase> exerciciosAlternativas;
//     [System.NonSerialized] public List<ExercicioBase> exerciciosWriting;

//     [System.NonSerialized] public List<ExercicioBase> exerciciosAssociacao;

//     protected override void Start()
//     {
//         base.Start();
//         nivelAtual = DadosJogador.nivelUsuario;
//         nomeExibicaoLegenda = "College Secretary";
//         exerciciosCompletarFrase = GarantirListaExercicios(exerciciosCompletarFrase);
//         exerciciosCriarFrases = GarantirListaExercicios(exerciciosCriarFrases);
//         exerciciosSpeaking = GarantirListaExercicios(exerciciosSpeaking);
//         exerciciosAlternativas = GarantirListaExercicios(exerciciosAlternativas);
//         exerciciosWriting = GarantirListaExercicios(exerciciosWriting);
//         exerciciosAssociacao = GarantirListaExercicios(exerciciosAssociacao);

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
//             dialogoAtual = new List<string>()
//             {
//                 "Hi. Can I help you?",
//                 "What is your name?",
//                 "What course do you want?",
//                 "What is your date of birth?",
//                 "Your registration is done.",
//                 "It is R$500 per month.",
//                 "Classes start on February 10th.",
//                 "Your classroom is room 204. It is on the second floor."
//             };

//             exerciciosSpeaking.Add(
//                     new ExercicioSpeaking()
//                     {
//                         enunciado = "You are at the college. You want to sign up for a course. What do you say?",
//                         opcoesFala = new List<string>()
//                         {
//                             "I enroll yesterday at the library\n",
//                             "I want to enroll\n",
//                             "I want to eat my enrollment\n"
//                         },
//                         respostaCorreta = 1
//                     }
//             );

//             exerciciosWriting.Add(
//                 new ExercicioWriting()
//                 {
//                     enunciado = "The secretary asks your name. Write your answer."
//                 }
//             );

//             exerciciosWriting.Add(
//                 new ExercicioWriting()
//                 {
//                     enunciado = "The secretary asks what course you want. Write your answer."
//                 }
//             );

//             exerciciosWriting.Add(
//                 new ExercicioWriting()
//                 {
//                     enunciado = "The secretary asks your date of birth. Write your answer."
//                 }
//             );

//             exerciciosCriarFrases.Add(
//                 new ExercicioBlocos()
//                 {
//                     enunciado= "Arrange the words to form the correct sentence in english: Qual é o valor da mensalidade?",
//                     blocosPalavras = new List<string>
//                     {
//                         "What",
//                         "tuition",
//                         "expensive",
//                         "is",
//                         "monthly",
//                         "cost",
//                         "fee",
//                         "much",
//                         "course",
//                         "How",
//                         "semester",
//                         "pay",
//                         "the",
//                         "does"
//                     },
//                     respostaCorreta = "What is the monthly fee?\n"
//                 }
//             );

//             exerciciosCriarFrases.Add(
//                 new ExercicioBlocos()
//                 {
//                     enunciado= "Arrange the words to form the correct sentence in english: Quando as aulas começam?",
//                     blocosPalavras = new List<string>
//                     {
//                         "do",
//                         "classes",
//                         "exam",
//                         "enrollment",
//                         "schedule",
//                         "where",
//                         "begin",
//                         "the",
//                         "When",
//                         "Does",
//                         "finish",
//                         "students",
//                         "start",
//                         "semester"
//                     },
//                     respostaCorreta = "When do classes start?\n"
//                 }
//             );

//             exerciciosSpeaking.Add(
//                     new ExercicioSpeaking()
//                     {
//                         enunciado = "You can’t find your class. Ask the secretary.",
//                         opcoesFala = new List<string>()
//                         {
//                             "Where yesterday the stairs?\n",
//                             "How much does the hallway cost?\n",
//                             "Where is my classroom?\n"
//                         },
//                         respostaCorreta = 2
//                     }
//             );

//             exerciciosAssociacao.Add(
//                 new ExercicioAssociacao()
//                 {
//                     enunciado = "Match the Portuguese word with the English word.",

//                     opcoesPort = new List<string>
//                     {
//                         "escritório",
//                         "andar",
//                         "nome",
//                         "curso",
//                         "estudante",
//                         "sala de aula",
//                         "mensalidade",
//                         "aula"
//                     },

//                     opcoesIngles = new List<string>
//                     {
//                         "class",
//                         "name",
//                         "office",
//                         "student",
//                         "floor",
//                         "classroom",
//                         "tuition",
//                         "course"
//                     },

//                     associacaoCorreta = new List<ParAssociacao>
//                     {
//                         new ParAssociacao { palavraPortugues = "escritório", palavraIngles = "office" },
//                         new ParAssociacao { palavraPortugues = "andar", palavraIngles = "floor" },
//                         new ParAssociacao { palavraPortugues = "nome", palavraIngles = "name" },
//                         new ParAssociacao { palavraPortugues = "curso", palavraIngles = "course" },
//                         new ParAssociacao { palavraPortugues = "estudante", palavraIngles = "student" },
//                         new ParAssociacao { palavraPortugues = "sala de aula", palavraIngles = "classroom" },
//                         new ParAssociacao { palavraPortugues = "mensalidade", palavraIngles = "tuition" },
//                         new ParAssociacao { palavraPortugues = "aula", palavraIngles = "class" }
//                     }
//                 }
//             );
//         }
//         else if(nivelAtual=="A2")
//         {
//             dialogoAtual = new List<string>()
//             {
//                 cumprimento+". Do you need help with something?",
//                 "Could I have your name, please?",
//                 "Which course would you like to study?",
//                 "What is your date of birth?",
//                 "Thank you. Your registration is complete. You are now enrolled in Computer Science.",
//                 "The tuition is R$500 per month.",
//                 "Classes start on February fifth.",
//                 "Your classroom is room two hundred four, on the second floor."
//             };

//             exerciciosCriarFrases.Add(
//                 new ExercicioBlocos()
//                 {
//                     enunciado= "Arrange the words to form the correct sentence in english: Gostaria de me matricular na universidade.",
//                     blocosPalavras = new List<string>
//                     {
//                         "university",
//                         "study",
//                         "would",
//                         "school",
//                         "at",
//                         "enroll",
//                         "student",
//                         "I",
//                         "course",
//                         "the",
//                         "want",
//                         "like",
//                         "register",
//                         "to",
//                         "class"
//                     },
//                     respostaCorreta = "I would like to enroll at the university.\n"
//                 }
//             );

//             exerciciosWriting.Add(
//                 new ExercicioWriting()
//                 {
//                     enunciado = "The secretary asks your name. Write your answer in a complete sentence."
//                 }
//             );

//             exerciciosSpeaking.Add(
//                     new ExercicioSpeaking()
//                     {
//                         enunciado = "You want to join the Computer Science course. What do you tell the secretary?",
//                         opcoesFala = new List<string>()
//                         {
//                             "I would like to study a hamburger.\n",
//                             "I would like to study Computer Science.\n",
//                             "I liked study Computer Science.\n"
//                         },
//                         respostaCorreta = 1
//                     }
//             );

//             exerciciosWriting.Add(
//                 new ExercicioWriting()
//                 {
//                     enunciado = "The secretary asks your date of birth. Write your answer in a complete sentence, with the month written out (for example, -May 10th-). "
//                 }
//             );

//             exerciciosWriting.Add(
//                 new ExercicioWriting()
//                 {
//                     enunciado = "You want to know how much the course costs each month. Write a question for the secretary. "
//                 }
//             );

//             exerciciosSpeaking.Add(
//                     new ExercicioSpeaking()
//                     {
//                         enunciado = "You want to know the date when your classes begin. What do you ask the secretary?",
//                         opcoesFala = new List<string>()
//                         {
//                             "Which course should I choose?\n",
//                             "Where can I pay the tuition?\n",
//                             "When do classes start?\n"
//                         },
//                         respostaCorreta = 2
//                     }
//             );

//             exerciciosAlternativas.Add(
//                 new ExercicioAlternativas()
//                 {
//                     enunciado = "What is the start date for classes?",
//                     alternativas = new List<string>(){ 
//                         "February 1st\r\n", 
//                         "February 5th\r\n", 
//                         "February 10th\r\n", 
//                         "February 20th\r\n" },
//                     alternativaCorreta = 2
//                 }
//             );

//             exerciciosSpeaking.Add(
//                     new ExercicioSpeaking()
//                     {
//                         enunciado = "You need to find your classroom, but you don’t know where it is. Ask the secretary where it is.",
//                         opcoesFala = new List<string>()
//                         {
//                             "Where is my classroom?\n",
//                             "When does the library open?\n",
//                             "How much is the tuition?\n"
//                         },
//                         respostaCorreta = 0
//                     }
//             );

//             exerciciosAlternativas.Add(
//                 new ExercicioAlternativas()
//                 {
//                     enunciado = "Select the correct option for your classroom number, as stated by the secretary.",
//                     alternativas = new List<string>(){ 
//                         "Room 102\r\n", 
//                         "Room 204\r\n", 
//                         "Room 301\r\n", 
//                         "Room 405\r\n" },
//                     alternativaCorreta = 1
//                 }
//             );

//             exerciciosAssociacao.Add(
//                 new ExercicioAssociacao()
//                 {
//                     enunciado = "Match the Portuguese word with the English word.",

//                     opcoesPort = new List<string>
//                     {
//                         "matricular-se",
//                         "matrícula",
//                         "direções",
//                         "repetir",
//                         "escadas",
//                         "biblioteca",
//                         "data de nascimento",
//                         "cronograma"
//                     },

//                     opcoesIngles = new List<string>
//                     {
//                         "library",
//                         "enroll",
//                         "schedule",
//                         "directions",
//                         "date of birth",
//                         "stairs",
//                         "registration",
//                         "repeat"
//                     },

//                     associacaoCorreta = new List<ParAssociacao>
//                     {
//                         new ParAssociacao { palavraPortugues = "matricular-se", palavraIngles = "enroll" },
//                         new ParAssociacao { palavraPortugues = "matrícula", palavraIngles = "registration" },
//                         new ParAssociacao { palavraPortugues = "direções", palavraIngles = "directions" },
//                         new ParAssociacao { palavraPortugues = "repetir", palavraIngles = "repeat" },
//                         new ParAssociacao { palavraPortugues = "escadas", palavraIngles = "stairs" },
//                         new ParAssociacao { palavraPortugues = "biblioteca", palavraIngles = "library" },
//                         new ParAssociacao { palavraPortugues = "data de nascimento", palavraIngles = "date of birth" },
//                         new ParAssociacao { palavraPortugues = "cronograma", palavraIngles = "schedule" }
//                     }
//                 }
//             );
//         }
//         else if(nivelAtual=="B1")
//         {
//             dialogoAtual = new List<string>()
//             {
//                 cumprimento+". Are you here to enroll in a course?",
//                 "Could you give me your full name, please?",
//                 "Could you also give me your date of birth?",
//                 "Thank you. I just need to confirm a few details. You are enrolling in Computer Science, correct?",
//                 "Your enrollment is complete. Your tuition will be 500 dollars per month.",
//                 "Classes start on February fifteenth in room three hundred twenty-five.",
//                 "Room 325 is on the third floor, just go up the stairs. Good luck with your classes!"
//             };

//             exerciciosSpeaking.Add(
//                     new ExercicioSpeaking()
//                     {
//                         enunciado = "The secretary asks if you are here to enroll in a course. You are, and you need her help. What do you say?",
//                         opcoesFala = new List<string>()
//                         {
//                             "I enrolled in Computer Science last year, and I’m here to renew my hotel reservation.\n",
//                             "Yes, please. Could you help me? I’m here to enroll in Computer Science\n",
//                             "I’m looking for the cafeteria because I need to check in for my flight\n"
//                         },
//                         respostaCorreta = 1
//                     }
//             );

//             exerciciosWriting.Add(
//                 new ExercicioWriting()
//                 {
//                     enunciado = "Write your full name, including your first and last names, in a complete sentence."
//                 }
//             );

//             exerciciosWriting.Add(
//                 new ExercicioWriting()
//                 {
//                     enunciado = "Write your date of birth in a complete sentence, with the month written out."
//                 }
//             );

//             exerciciosCompletarFrase.Add(
//                 new ExercicioCompletarFrase()
//                 {
//                     enunciado = "Complete the sentence with the correct words:",
//                     partesFrase = new List<string>
//                     {
//                         "Yes, ",
//                         " ",
//                         " correct."
//                     },
//                     respostaCorreta = "that, is",
//                     opcoes = new List<string>
//                     {
//                         "that",
//                         "served",
//                         "alright",
//                         "Which",
//                         "tonight",
//                         "don't",
//                         "morning",
//                         "no",                        
//                         "this",
//                         "is"
//                     }
//                 }
//             );

//             exerciciosSpeaking.Add(
//                     new ExercicioSpeaking()
//                     {
//                         enunciado = "Your enrollment is complete, but you don't know when your classes begin. Ask the secretary politely.",
//                         opcoesFala = new List<string>()
//                         {
//                             "Could you tell me when the classes start?\n",
//                             "Could you tell me when do the classes start?\n",
//                             "Could you tell me when the classes started?\n"
//                         },
//                         respostaCorreta = 0
//                     }
//             );

//             exerciciosAlternativas.Add(
//                 new ExercicioAlternativas()
//                 {
//                     enunciado = "Which room is your first class in? Choose the correct answer:",
//                     alternativas = new List<string>() { 
//                         "Room 102\r\n", 
//                         "Room 204\r\n", 
//                         "Room 305\r\n", 
//                         "Room 325\r\n" },
//                     alternativaCorreta = 3
//                 }
//             );

//             exerciciosWriting.Add(
//                 new ExercicioWriting()
//                 {
//                     enunciado = "You received your schedule, and your first class is in room 305. You don't know where it is. Write a question to ask the secretary."
//                 }
//             );

//             exerciciosAssociacao.Add(
//                 new ExercicioAssociacao()
//                 {
//                     enunciado = "Match the Portuguese word with the English word.",

//                     opcoesPort = new List<string>
//                     {
//                         "matrícula (efetivada)",
//                         "mensalidade",
//                         "corredor",
//                         "balcão da recepção",
//                         "esclarecimento",
//                         "confirmar",
//                         "detalhes",
//                         "nome completo"
//                     },

//                     opcoesIngles = new List<string>
//                     {
//                         "hallway",
//                         "confirm",
//                         "full name",
//                         "tuition",
//                         "clarification",
//                         "details",
//                         "enrollment",
//                         "reception desk"
//                     },

//                     associacaoCorreta = new List<ParAssociacao>
//                     {
//                         new ParAssociacao { palavraPortugues = "matrícula (efetivada)", palavraIngles = "enrollment" },
//                         new ParAssociacao { palavraPortugues = "mensalidade", palavraIngles = "tuition" },
//                         new ParAssociacao { palavraPortugues = "corredor", palavraIngles = "hallway" },
//                         new ParAssociacao { palavraPortugues = "balcão da recepção", palavraIngles = "reception desk" },
//                         new ParAssociacao { palavraPortugues = "esclarecimento", palavraIngles = "clarification" },
//                         new ParAssociacao { palavraPortugues = "confirmar", palavraIngles = "confirm" },
//                         new ParAssociacao { palavraPortugues = "detalhes", palavraIngles = "details" },
//                         new ParAssociacao { palavraPortugues = "nome completo", palavraIngles = "full name" }
//                     }
//                 }
//             );
//         }
//     }

//     private void IniciarMissao(string id, string texto, string traducao)
//     {
//         MissionManager.AdicionarMissaoNPC(id, texto, traducao);
//     }

//     protected override async Task IniciarInteracao()
//     {
//         GameProgress.EstaEmDialogo = true;
//         nivelAtual = DadosJogador.nivelUsuario;
//         InicializarConteudosPorNivel();

//         if (nivelAtual == "A1")
//         {
//             IniciarMissao("universidade_a1_matricula",
//                 "Realize sua matrícula",
//                 "Complete your enrollment");
//             await interacaoA1();
//         }
//         else if (nivelAtual == "A2")
//         {
//             MissionManager.ConcluirMissaoNPC("universidade_a2_encontrar_secretaria");
//             IniciarMissao("universidade_a2_matricula",
//                 "Faça sua matrícula",
//                 "Complete your enrollment");
//             await interacaoA2();
//         }
//         else
//         {
//             IniciarMissao("universidade_b1_matricula",
//                 "Informe seus dados, confirme as informações do curso e conclua o processo de matrícula",
//                 "Provide your details, confirm the course information, and complete your enrollment");
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

//         if (RevisaoManager.Instance != null)
//             await RevisaoManager.Instance.ExecutarRevisaoEExibirModalFinal(this);
//         else
//             Debug.LogError("RevisaoManager não está disponível para finalizar College.");
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

//     private async Task interacaoA1()
//     {
//         int i = 0;

//         await PlayAudioETexto(i++);

//         await AbrirExercicio(ObterTipoExercicio(exerciciosSpeaking[0]), exerciciosSpeaking[0]);

//         await PlayAudioETexto(i++);

//         await AbrirExercicio(ObterTipoExercicio(exerciciosWriting[0]), exerciciosWriting[0]);

//         await PlayAudioETexto(i++);

//         await AbrirExercicio(ObterTipoExercicio(exerciciosWriting[1]), exerciciosWriting[1]);

//         await PlayAudioETexto(i++);

//         await AbrirExercicio(ObterTipoExercicio(exerciciosWriting[2]), exerciciosWriting[2]);

//         await PlayAudioETexto(i++);
//         MissionManager.ConcluirMissaoNPC("universidade_a1_matricula");

//         await AbrirExercicio(ObterTipoExercicio(exerciciosCriarFrases[0]), exerciciosCriarFrases[0]);

//         await PlayAudioETexto(i++);

//         await AbrirExercicio(ObterTipoExercicio(exerciciosCriarFrases[1]), exerciciosCriarFrases[1]);

//         await PlayAudioETexto(i++);

//         await AbrirExercicio(ObterTipoExercicio(exerciciosSpeaking[1]), exerciciosSpeaking[1]);

//         await PlayAudioETexto(i++);

//         await AbrirExercicio(ObterTipoExercicio(exerciciosAssociacao[0]), exerciciosAssociacao[0]);
//     }

//     private async Task interacaoA2()
//     {
//         int i = 0;

//         await PlayAudioETexto(i++);
//         await AbrirExercicio(ObterTipoExercicio(exerciciosCriarFrases[0]), exerciciosCriarFrases[0]);

//         await PlayAudioETexto(i++);
//         await AbrirExercicio(ObterTipoExercicio(exerciciosWriting[0]), exerciciosWriting[0]);

//         await PlayAudioETexto(i++);
//         await AbrirExercicio(ObterTipoExercicio(exerciciosSpeaking[0]), exerciciosSpeaking[0]);

//         await PlayAudioETexto(i++);
//         await AbrirExercicio(ObterTipoExercicio(exerciciosWriting[1]), exerciciosWriting[1]);

//         // Confirmação de matrícula.
//         await PlayAudioETexto(i++);
//         MissionManager.ConcluirMissaoNPC("universidade_a2_matricula");

//         IniciarMissao("universidade_a2_cronograma",
//             "Consulte seu cronograma",
//             "Check your class schedule");

//         // Pergunta sobre a mensalidade e recebe a resposta.
//         await AbrirExercicio(ObterTipoExercicio(exerciciosWriting[2]), exerciciosWriting[2]);
//         await PlayAudioETexto(i++);

//         // Pergunta quando as aulas começam e confere a data.
//         await AbrirExercicio(ObterTipoExercicio(exerciciosSpeaking[1]), exerciciosSpeaking[1]);
//         await PlayAudioETexto(i++);
//         await AbrirExercicio(ObterTipoExercicio(exerciciosAlternativas[0]), exerciciosAlternativas[0]);

//         // Pergunta onde fica a sala e confere o número no cronograma.
//         await AbrirExercicio(ObterTipoExercicio(exerciciosSpeaking[2]), exerciciosSpeaking[2]);
//         await PlayAudioETexto(i++);
//         await AbrirExercicio(ObterTipoExercicio(exerciciosAlternativas[1]), exerciciosAlternativas[1]);

//         MissionManager.ConcluirMissaoNPC("universidade_a2_cronograma");

//         await AbrirExercicio(ObterTipoExercicio(exerciciosAssociacao[0]), exerciciosAssociacao[0]);
//     }

//     private async Task interacaoB1()
//     {
//         int i = 0;

//         await PlayAudioETexto(i++);
//         await AbrirExercicio(ObterTipoExercicio(exerciciosSpeaking[0]), exerciciosSpeaking[0]);

//         await PlayAudioETexto(i++);
//         await AbrirExercicio(ObterTipoExercicio(exerciciosWriting[0]), exerciciosWriting[0]);

//         await PlayAudioETexto(i++);
//         await AbrirExercicio(ObterTipoExercicio(exerciciosWriting[1]), exerciciosWriting[1]);

//         await PlayAudioETexto(i++);
//         await AbrirExercicio(ObterTipoExercicio(exerciciosCompletarFrase[0]), exerciciosCompletarFrase[0]);

//         // Confirmação de matrícula concluída.
//         await PlayAudioETexto(i++);
//         MissionManager.ConcluirMissaoNPC("universidade_b1_matricula");

//         IniciarMissao("universidade_b1_cronograma",
//             "Consulte o cronograma, identifique a sala da primeira aula e pergunte como chegar até ela",
//             "Check your schedule, identify the room for your first class, and ask how to get there");

//         await AbrirExercicio(ObterTipoExercicio(exerciciosSpeaking[1]), exerciciosSpeaking[1]);

//         await PlayAudioETexto(i++);
//         await AbrirExercicio(ObterTipoExercicio(exerciciosAlternativas[0]), exerciciosAlternativas[0]);

//         await AbrirExercicio(ObterTipoExercicio(exerciciosWriting[2]), exerciciosWriting[2]);

//         await PlayAudioETexto(i++);

//         MissionManager.ConcluirMissaoNPC("universidade_b1_cronograma");

//         await AbrirExercicio(ObterTipoExercicio(exerciciosAssociacao[0]), exerciciosAssociacao[0]);
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
using UnityEngine.XR;
using System.IO;
using TMPro;
using System;

public class SecretaryCollegeNPC: InteracaoNPC, IRoteiroNpcCheckpoint
{
    string nivelAtual;
    string cumprimento;
    private const string NpcKey = MissoesCollege.Secretaria;
    [SerializeField] private DocumentoCursoCollege documentoCurso;
    private int ultimaVisitaCollege;
    private int VisitaAtual => MissionManager.Instance?.VisitaCollege ?? 0;
    protected override bool PermitirReinteracaoAposConclusao =>
        VisitaAtual != 0 && ultimaVisitaCollege != VisitaAtual;

    public override bool PodeInteragir() =>
        base.PodeInteragir() && MissionManager.Instance?.PodeAtenderSecretariaCollege == true;
    private ExerciseLoadResult catalogoExercicios;
    int horaAtual = DateTime.Now.Hour;
    public TextMeshProUGUI textoPularDialogo;
    public ModalExercicio modalExercicio;
    List<string> dialogoAtual;

    public string idNpcParaVoz = "secretaryCollege";
    public GameObject modalLegenda;

    [System.NonSerialized] public List<ExercicioBase> exerciciosCompletarFrase;
    [System.NonSerialized] public List<ExercicioBase> exerciciosCriarFrases;
    [System.NonSerialized] public List<ExercicioBase> exerciciosSpeaking;
    [System.NonSerialized] public List<ExercicioBase> exerciciosAlternativas;
    [System.NonSerialized] public List<ExercicioBase> exerciciosWriting;

    [System.NonSerialized] public List<ExercicioBase> exerciciosAssociacao;

    protected override void Start()
    {
        base.Start();
        if (audioSourceComum == null)
        {
            audioSourceComum = gameObject.AddComponent<AudioSource>();
            audioSourceComum.playOnAwake = false;
        }

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

        exerciciosCompletarFrase.Clear();
        exerciciosCriarFrases.Clear();
        exerciciosSpeaking.Clear();
        exerciciosAlternativas.Clear();
        exerciciosWriting.Clear();
        exerciciosAssociacao.Clear();

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
                    alternativaCorreta = 1
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
                "Classes start on February fifteenth in room four hundred twenty.",
                "Room 420 is on the third floor, just go up the stairs. Good luck with your classes!"
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
                new ExercicioCompletarFrase()
                {
                    enunciado = "Complete the sentence with the correct words:",
                    partesFrase = new List<string>
                    {
                        "Yes, ",
                        " ",
                        " correct."
                    },
                    respostasCorretas = new List<string> { "that", "is" },
                    opcoes = new List<string>
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
                        "is"
                    }
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
                        "Room 305\r\n", 
                        "Room 420\r\n" },
                    alternativaCorreta = 3
                }
            );

            exerciciosWriting.Add(
                new ExercicioWriting()
                {
                    enunciado = "You received your schedule, and your first class is in room 420. You don't know where it is. Write a question to ask the secretary."
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
else if (nivelAtual == "A1-A2")
{
    dialogoAtual = new List<string>()
    {
        cumprimento + ". How can I help you today?",
        "Could you tell me your full name, please?",
        "Which course would you like to enroll in?",
        "Thank you. Could you also provide your date of birth?",
        "Your enrollment in Computer Science is complete.",
        "The tuition fee is R$500 per month. Classes start on February tenth.",
        "Your first classroom is room 204, on the second floor. You can follow the signs near the stairs."
    };

    exerciciosSpeaking.Add(
        new ExercicioSpeaking()
        {
            enunciado = "The secretary asks how she can help you. Explain that you want to enroll in Computer Science.",
            opcoesFala = new List<string>()
            {
                "I would like to pay for the library's books.\n",
                "I would like to enroll in Computer Science.\n",
                "I would like to teach the secretary's office.\n"
            },
            respostaCorreta = 1
        }
    );

    exerciciosWriting.Add(
        new ExercicioWriting()
        {
            enunciado = "The secretary asks for your full name. Write your answer in a complete sentence."
        }
    );

    exerciciosSpeaking.Add(
        new ExercicioSpeaking()
        {
            enunciado = "The secretary asks which course you would like to study. Choose the appropriate answer.",
            opcoesFala = new List<string>()
            {
                "I would like to enroll on the second floor.\n",
                "I would like to enroll on February tenth.\n",
                "I would like to enroll in Computer Science.\n"
            },
            respostaCorreta = 2
        }
    );

    exerciciosWriting.Add(
        new ExercicioWriting()
        {
            enunciado = "The secretary asks for your date of birth. Write your answer in a complete sentence, with the month written out."
        }
    );

    exerciciosAlternativas.Add(
        new ExercicioAlternativas()
        {
            enunciado = "What is the monthly tuition fee mentioned by the secretary?",
            alternativas = new List<string>()
            {
                "R$600 per month\r\n",
                "R$300 per month\r\n",
                "R$400 per month\r\n",
                "R$500 per month\r\n"
            },
            alternativaCorreta = 3
        }
    );

    exerciciosSpeaking.Add(
        new ExercicioSpeaking()
        {
            enunciado = "You want to confirm when your classes begin. What do you ask?",
            opcoesFala = new List<string>()
            {
                "Where do classes pay?\n",
                "How much do classes enroll?\n",
                "When do classes start?\n"
            },
            respostaCorreta = 2
        }
    );

    exerciciosAlternativas.Add(
        new ExercicioAlternativas()
        {
            enunciado = "According to the secretary, when do classes start?",
            alternativas = new List<string>()
            {
                "February 20th\r\n",
                "February 5th\r\n",
                "February 10th\r\n",
                "February 15th\r\n"
            },
            alternativaCorreta = 2
        }
    );

    exerciciosSpeaking.Add(
        new ExercicioSpeaking()
        {
            enunciado = "You need to find your first classroom. Ask the secretary where it is.",
            opcoesFala = new List<string>()
            {
                "How much is my tuition?\n",
                "When does my enrollment start?\n",
                "Where is my classroom?\n"
            },
            respostaCorreta = 2
        }
    );

    exerciciosAssociacao.Add(
        new ExercicioAssociacao()
        {
            enunciado = "Match the Portuguese word with the English word.",
            opcoesPort = new List<string>()
            {
                "nome completo",
                "data de nascimento",
                "matrícula",
                "mensalidade",
                "curso",
                "sala de aula",
                "andar",
                "mensal"
            },
            opcoesIngles = new List<string>()
            {
                "classroom",
                "monthly",
                "enrollment",
                "full name",
                "floor",
                "tuition fee",
                "course",
                "date of birth"
            },
            associacaoCorreta = new List<ParAssociacao>()
            {
                new ParAssociacao { palavraPortugues = "nome completo", palavraIngles = "full name" },
                new ParAssociacao { palavraPortugues = "data de nascimento", palavraIngles = "date of birth" },
                new ParAssociacao { palavraPortugues = "matrícula", palavraIngles = "enrollment" },
                new ParAssociacao { palavraPortugues = "mensalidade", palavraIngles = "tuition fee" },
                new ParAssociacao { palavraPortugues = "curso", palavraIngles = "course" },
                new ParAssociacao { palavraPortugues = "sala de aula", palavraIngles = "classroom" },
                new ParAssociacao { palavraPortugues = "andar", palavraIngles = "floor" },
                new ParAssociacao { palavraPortugues = "mensal", palavraIngles = "monthly" }
            }
        }
    );
}
else if (nivelAtual == "A2-B1")
{
    dialogoAtual = new List<string>()
    {
        cumprimento + ". Are you here to complete your enrollment?",
        "Of course. Could you provide your full name and date of birth?",
        "Thank you. I found your application. You are applying for the Computer Science program, correct?",
        "Before I finalize your enrollment, I'd like to confirm that all your information is correct.",
        "Everything is confirmed. Your enrollment is now complete. The tuition fee is R$500 per month.",
        "The semester begins on February fifteenth. Your first class will take place in room 325.",
        "Room 325 is on the third floor. Go up the stairs and follow the signs along the hallway.",
        "Do you have any other questions about your enrollment or class schedule?"
    };

    exerciciosSpeaking.Add(
        new ExercicioSpeaking()
        {
            enunciado = "Explain to the secretary that you are there to complete your enrollment in Computer Science.",
            opcoesFala = new List<string>()
            {
                "I'm here to confirm the library's opening hours.\n",
                "I'm here to complete my enrollment in Computer Science.\n",
                "I'm here to complete my classroom on the third floor.\n"
            },
            respostaCorreta = 1
        }
    );

    exerciciosWriting.Add(
        new ExercicioWriting()
        {
            enunciado = "The secretary asks for your full name and date of birth. Write both pieces of information in complete sentences."
        }
    );

    exerciciosSpeaking.Add(
        new ExercicioSpeaking()
        {
            enunciado = "The secretary confirms that you are applying for Computer Science. Confirm that the information is correct.",
            opcoesFala = new List<string>()
            {
                "Yes, that's correct. I'm applying for room 325.\n",
                "Yes, that's correct. I'm applying for the tuition fee.\n",
                "Yes, that's correct. I'm applying for Computer Science.\n"
            },
            respostaCorreta = 2
        }
    );

    exerciciosCompletarFrase.Add(
        new ExercicioCompletarFrase()
        {
            enunciado = "Complete the sentence with the correct words:",
            partesFrase = new List<string>()
            {
                "Before I finalize your enrollment, I'd like ",
                " ",
                " all your information is correct."
            },
            respostasCorretas = new List<string> { "to confirm", "that" },
            opcoes = new List<string>()
            {
                "where",
                "when",
                "to enroll",
                "if",
                "how",
                "to confirm",
                "to check",
                "that"
            }
        }
    );

    exerciciosWriting.Add(
        new ExercicioWriting()
        {
            enunciado = "The secretary asks you to confirm your course information. Write a polite sentence confirming that you are enrolling in Computer Science."
        }
    );

    exerciciosAlternativas.Add(
        new ExercicioAlternativas()
        {
            enunciado = "What is the monthly tuition fee for the course?",
            alternativas = new List<string>()
            {
                "R$550 per month\r\n",
                "R$400 per month\r\n",
                "R$450 per month\r\n",
                "R$500 per month\r\n"
            },
            alternativaCorreta = 3
        }
    );

    exerciciosSpeaking.Add(
        new ExercicioSpeaking()
        {
            enunciado = "You want to know when the semester begins. Ask the secretary politely.",
            opcoesFala = new List<string>()
            {
                "Could you tell me where the semester is located?\n",
                "Could you tell me how much the semester enrolls?\n",
                "Could you tell me when the semester begins?\n"
            },
            respostaCorreta = 2
        }
    );

    exerciciosAlternativas.Add(
        new ExercicioAlternativas()
        {
            enunciado = "According to the secretary, where will your first class take place?",
            alternativas = new List<string>()
            {
                "Room 325, on the second floor\r\n",
                "Room 204, on the second floor\r\n",
                "Room 305, on the third floor\r\n",
                "Room 325, on the third floor\r\n"
            },
            alternativaCorreta = 3
        }
    );

    exerciciosWriting.Add(
        new ExercicioWriting()
        {
            enunciado = "You know your classroom number but need directions. Write a polite question asking how to get to room 325."
        }
    );

    exerciciosSpeaking.Add(
        new ExercicioSpeaking()
        {
            enunciado = "The secretary asks if you have any other questions. Ask for clarification about the location of your classroom.",
            opcoesFala = new List<string>()
            {
                "Could you explain how much room 325 costs?\n",
                "Could you explain when room 325 enrolls?\n",
                "Could you explain how to get to room 325?\n"
            },
            respostaCorreta = 2
        }
    );

    exerciciosAssociacao.Add(
        new ExercicioAssociacao()
        {
            enunciado = "Match the Portuguese expression with the English expression.",
            opcoesPort = new List<string>()
            {
                "concluir a matrícula",
                "data de nascimento",
                "confirmar informações",
                "mensalidade",
                "período letivo",
                "primeira aula",
                "andar",
                "esclarecimento"
            },
            opcoesIngles = new List<string>()
            {
                "tuition fee",
                "to confirm information",
                "first class",
                "clarification",
                "to complete enrollment",
                "floor",
                "date of birth",
                "semester"
            },
            associacaoCorreta = new List<ParAssociacao>()
            {
                new ParAssociacao { palavraPortugues = "concluir a matrícula", palavraIngles = "to complete enrollment" },
                new ParAssociacao { palavraPortugues = "data de nascimento", palavraIngles = "date of birth" },
                new ParAssociacao { palavraPortugues = "confirmar informações", palavraIngles = "to confirm information" },
                new ParAssociacao { palavraPortugues = "mensalidade", palavraIngles = "tuition fee" },
                new ParAssociacao { palavraPortugues = "período letivo", palavraIngles = "semester" },
                new ParAssociacao { palavraPortugues = "primeira aula", palavraIngles = "first class" },
                new ParAssociacao { palavraPortugues = "andar", palavraIngles = "floor" },
                new ParAssociacao { palavraPortugues = "esclarecimento", palavraIngles = "clarification" }
            }
        }
    );
}

        if (descricao != null)
            descricao.Falas = dialogoAtual;
        else
        {
            this.dialogoAtual = dialogoAtual;
        }
    }

    protected override Task IniciarInteracao() => ExecutarAtendimento(0);

    private async Task ExecutarAtendimento(int inicio, string estadoDocumento = null)
    {
        int versao = SessaoCenario.Atual.Versao;
        ultimaVisitaCollege = VisitaAtual;
        GameProgress.EstaEmDialogo = true;

        try
        {
            documentoCurso?.Resetar();
            if (estadoDocumento == null)
                MissionManager.Instance?.RegistrarEventoCollege(MissoesCollege.InicioConversa, NpcKey);
            nivelAtual = DadosJogador.nivelUsuario;
            InicializarConteudosPorNivel();
            catalogoExercicios = await ExerciseManager.Instance.LoadForNpcAsync(NpcKey, nivelAtual);
            if (!catalogoExercicios.Success)
                Debug.LogWarning("SecretaryCollegeNPC: usando exercícios locais. " + catalogoExercicios.Error);

            if (estadoDocumento == "document.available" || estadoDocumento == "document.read")
            {
                if (documentoCurso == null || !documentoCurso.Disponibilizar(nivelAtual))
                    throw new System.IO.InvalidDataException("Documento do curso indisponível para retomada.");
                documentoCurso.RestaurarConsulta(estadoDocumento == "document.read");
            }
            ValidarVisita(versao);
            var roteiro = CriarRoteiro(versao);
            if (documentoCurso != null)
                documentoCurso.ConsultaConfirmada += roteiro.AtualizarCursor;
            try { await roteiro.Executar(inicio); }
            finally
            {
                if (documentoCurso != null)
                    documentoCurso.ConsultaConfirmada -= roteiro.AtualizarCursor;
            }

            if (modalLegenda != null)
                modalLegenda.SetActive(false);
            if (textoPularDialogo != null)
                textoPularDialogo.gameObject.SetActive(false);

            MissionManager.Instance?.RegistrarEventoCollege(MissoesCollege.FimConversa, NpcKey);
            if (nivelAtual == "A1-A2" || nivelAtual == "A2-B1")
            {
                CheckpointJogo.Instance?.CapturarCaminhoCollege(true);
                if (!await ConfirmarConsultaDoDocumento())
                    return;
            }
            CheckpointJogo.Instance?.CapturarRevisaoCollege();
        }
        finally
        {
            documentoCurso?.Resetar();
            GameProgress.EstaEmDialogo = false;
            if (modalLegenda != null)
                modalLegenda.SetActive(false);
            if (textoPularDialogo != null)
                textoPularDialogo.gameObject.SetActive(false);

            LiberarControlePlayer();
        }

        if (RevisaoManager.Instance != null)
            await RevisaoManager.Instance.ExecutarRevisaoEExibirModalFinal(this);
        else
            Debug.LogError("RevisaoManager não está disponível para finalizar College.");
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


    async Task<System.Func<Task>> IRoteiroNpcCheckpoint.PrepararRetomada(Newtonsoft.Json.Linq.JObject visita,
        int versao, System.Threading.CancellationToken token)
    {
        ValidarVisita(versao);
        AtualizarVisita();
        nivelAtual = SessaoCenario.Atual.Nivel;
        cumprimento = RoteiroNpcCheckpoint.SaudacaoSalva(visita);
        InicializarConteudosPorNivel();
        var cursor = CriarRoteiro(versao).Validar(visita);
        if (cursor.Situacao.Length != 2 || (cursor.Situacao[1] != "document.unavailable"
            && cursor.Situacao[1] != "document.available" && cursor.Situacao[1] != "document.read"))
            throw new System.IO.InvalidDataException("Estado do documento do College inválido.");
        var recepcao = FindAnyObjectByType<RecepcionistCollegeNPC>();
        if (recepcao != null)
            recepcao.RetomarAposAtendimentoCheckpoint(versao);
        await RoteiroNpcCheckpoint.ExigirCatalogo(this, versao, token);
        return async () =>
        {
            ValidarVisita(versao);
            TravarControlePlayer();
            await ExecutarAtendimento(cursor.Indice, cursor.Situacao[1]);
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
        var exerciciosAssociacao = descricao?.exerciciosAssociacao ?? this.exerciciosAssociacao;

        var roteiro = new RoteiroNpcCheckpoint(this, visita, () => new[] { RoteiroNpcCheckpoint.ChaveSaudacao(cumprimento),
            documentoCurso?.Consultado == true ? "document.read"
                : documentoCurso?.Disponivel == true ? "document.available" : "document.unavailable" });
        void Fala(int indice, bool apresentarDocumento = false) =>
            roteiro.Fala(dialogoAtual[indice], () => PlayAudioETexto(indice, apresentarDocumento),
                anotacao: apresentarDocumento ? "Disponibilizar a consulta ao documento do curso." : null);
        void Exercicio(string ponto, int ordem, ExercicioBase fallback) =>
            roteiro.Exercicio(ponto, ordem, () => AbrirExercicioDoPonto(ponto, ordem, fallback), modelo: fallback);

        switch (nivelAtual)
        {
            case "A1":
                Fala(0);
                Exercicio("enrollment_request", 1, exerciciosSpeaking[0]);
                Fala(1);
                Exercicio("personal_details", 1, exerciciosWriting[0]);
                Fala(2);
                Exercicio("course_choice", 1, exerciciosWriting[1]);
                Fala(3);
                Exercicio("personal_details", 2, exerciciosWriting[2]);
                Fala(4);
                Exercicio("tuition", 1, exerciciosCriarFrases[0]);
                Fala(5);
                Exercicio("class_start", 1, exerciciosCriarFrases[1]);
                Fala(6);
                Exercicio("classroom", 1, exerciciosSpeaking[1]);
                Fala(7);
                Exercicio("enrollment_vocabulary", 1, exerciciosAssociacao[0]);
                break;
            case "A2":
                Fala(0);
                Exercicio("enrollment_request", 1, exerciciosCriarFrases[0]);
                Fala(1);
                Exercicio("personal_details", 1, exerciciosWriting[0]);
                Fala(2);
                Exercicio("course_choice", 1, exerciciosSpeaking[0]);
                Fala(3);
                Exercicio("personal_details", 2, exerciciosWriting[1]);
                Fala(4);
                Exercicio("tuition", 1, exerciciosWriting[2]);
                Fala(5);
                Exercicio("class_start", 1, exerciciosSpeaking[1]);
                Fala(6);
                Exercicio("class_start", 2, exerciciosAlternativas[0]);
                Exercicio("classroom", 1, exerciciosSpeaking[2]);
                Fala(7);
                Exercicio("classroom", 2, exerciciosAlternativas[1]);
                Exercicio("enrollment_vocabulary", 1, exerciciosAssociacao[0]);
                break;
            case "B1":
                Fala(0);
                Exercicio("enrollment_request", 1, exerciciosSpeaking[0]);
                Fala(1);
                Exercicio("personal_details", 1, exerciciosWriting[0]);
                Fala(2);
                Exercicio("personal_details", 2, exerciciosWriting[1]);
                Fala(3);
                Exercicio("enrollment_confirmation", 1, exerciciosCompletarFrase[0]);
                Fala(4);
                Exercicio("class_start", 1, exerciciosSpeaking[1]);
                Fala(5);
                Exercicio("classroom", 1, exerciciosAlternativas[0]);
                Exercicio("classroom", 2, exerciciosWriting[2]);
                Fala(6);
                Exercicio("enrollment_vocabulary", 1, exerciciosAssociacao[0]);
                break;
            case "A1-A2":
                Fala(0);
                Exercicio("enrollment_request", 1, exerciciosSpeaking[0]);
                Fala(1);
                Exercicio("personal_details", 1, exerciciosWriting[0]);
                Fala(2);
                Exercicio("course_choice", 1, exerciciosSpeaking[1]);
                Fala(3);
                Exercicio("personal_details", 2, exerciciosWriting[1]);
                Fala(4);
                Exercicio("tuition", 1, exerciciosAlternativas[0]);
                Fala(5);
                Exercicio("class_start", 1, exerciciosSpeaking[2]);
                Fala(6, apresentarDocumento: true);
                Exercicio("class_start", 2, exerciciosAlternativas[1]);
                Exercicio("classroom", 1, exerciciosSpeaking[3]);
                Exercicio("enrollment_vocabulary", 1, exerciciosAssociacao[0]);
                break;
            case "A2-B1":
                Fala(0);
                Exercicio("enrollment_request", 1, exerciciosSpeaking[0]);
                Fala(1);
                Exercicio("personal_details", 1, exerciciosWriting[0]);
                Fala(2);
                Exercicio("course_choice", 1, exerciciosSpeaking[1]);
                Fala(3);
                Exercicio("enrollment_confirmation", 1, exerciciosCompletarFrase[0]);
                Fala(4);
                Exercicio("enrollment_confirmation", 2, exerciciosWriting[1]);
                Fala(5, apresentarDocumento: true);
                Exercicio("tuition", 1, exerciciosAlternativas[0]);
                Fala(6);
                Exercicio("class_start", 1, exerciciosSpeaking[2]);
                Fala(7);
                Exercicio("classroom", 1, exerciciosAlternativas[1]);
                Exercicio("classroom", 2, exerciciosWriting[2]);
                Fala(8);
                Exercicio("classroom", 3, exerciciosSpeaking[3]);
                Exercicio("enrollment_vocabulary", 1, exerciciosAssociacao[0]);
                break;
        }
        return roteiro;
    }

    private async Task AbrirExercicioDoPonto(string pointKey, int ordem, ExercicioBase fallback)
    {
        CheckpointJogo.IdentificarFallback(fallback, pointKey, ordem, nivelAtual);
        TipoExercicio tipo = ObterTipoExercicio(fallback);
        bool? speakingLivre = (fallback as ExercicioSpeaking)?.modoLivre;
        ExerciseCatalogItem item = catalogoExercicios?.Find(pointKey, nivelAtual, ordem, tipo, speakingLivre);
        if (item != null)
            await AbrirExercicio(item.Tipo, item.Exercise);
        else
        {
            Debug.LogWarning($"SecretaryCollegeNPC: usando exercício local em {nivelAtual}/{pointKey}/{ordem}.");
            await AbrirExercicio(tipo, fallback);
        }

        if (nivelAtual == "B1" && pointKey == "class_start" && ordem == 1)
            MissionManager.Instance?.RegistrarEventoCollege(MissoesCollege.ExercicioEncerrado, MissoesCollege.InicioAulas);
    }

    internal async Task RetomarConsultaCheckpoint(int visita)
    {
        ValidarVisita(visita);
        AtualizarVisita();
        ultimaVisitaCollege = VisitaAtual;
        interacaoConcluida = true;
        nivelAtual = SessaoCenario.Atual.Nivel;
        GameProgress.EstaEmDialogo = true;
        TravarControlePlayer();
        try
        {
            if (documentoCurso == null || !documentoCurso.Disponibilizar(nivelAtual))
                throw new InvalidOperationException("College: configure a interface do documento para retomar a consulta.");
            if (!await ConfirmarConsultaDoDocumento())
                return;
            ValidarVisita(visita);
            CheckpointJogo.Instance?.CapturarRevisaoCollege();
        }
        finally
        {
            if (SessaoCenario.Atual.Permite(Cenario, visita))
            {
                documentoCurso?.Resetar();
                GameProgress.EstaEmDialogo = false;
                LiberarControlePlayer();
            }
        }
        await RevisaoManager.Instance.ExecutarRevisaoEExibirModalFinal(this);
    }

    private async Task<bool> ConfirmarConsultaDoDocumento()
    {
        if (UnityEngine.EventSystems.EventSystem.current == null)
        {
            Debug.LogError($"College ({nivelAtual}): a interface do documento precisa de um EventSystem ativo. Revisão bloqueada.", this);
            return false;
        }

        if (documentoCurso != null && documentoCurso.Disponivel)
        {
            DefinirCursorDesktop(true);
            documentoCurso.MostrarBotao(true);
            while (documentoCurso != null && documentoCurso.PodeAguardarConsulta
                && (!documentoCurso.Consultado || documentoCurso.Aberto))
                await Task.Yield();

            if (documentoCurso != null && documentoCurso.Consultado && !documentoCurso.Aberto)
            {
                string missao = nivelAtual == "A1-A2" ? "universidade_a1a2_matricula" : "universidade_a2b1_matricula";
                if (MissionManager.Instance?.MissaoConcluida(missao) == true)
                    return true;

                Debug.LogError($"College ({nivelAtual}): documento aberto, mas a missão {missao} continua pendente. Revisão bloqueada.", this);
                return false;
            }
        }

        Debug.LogError($"College ({nivelAtual}): configure o DocumentoCursoCollege ativo com botão de abertura, painel, texto e botão de fechamento utilizáveis. Revisão bloqueada sem a consulta.", this);
        return false;
    }

    private async Task AguardarAvancoComDocumento(bool apresentarDocumento)
    {
        int visita = SessaoCenario.Atual.Versao;
        if (apresentarDocumento && (documentoCurso == null || !documentoCurso.Disponibilizar(nivelAtual)))
            Debug.LogWarning("SecretaryCollegeNPC: configure o documento do curso para completar esta missão.", this);

        if (documentoCurso == null || !documentoCurso.Disponivel)
        {
            await AguardarAvancoDialogo();
            return;
        }

        DefinirCursorDesktop(true);
        documentoCurso.MostrarBotao(true);
        try
        {
            while (true)
            {
                int quadroPressionado = 0;
                bool painelEstavaAberto = false;
                await BotaoConfiguracoes.AguardarComandoDialogo(AvancoDialogoPressionado,
                    () => ValidarVisita(visita), () =>
                    {
                        quadroPressionado = Time.frameCount;
                        painelEstavaAberto = documentoCurso.Aberto;
                    });
                if (!painelEstavaAberto && !documentoCurso.Aberto
                    && documentoCurso.UltimoQuadroInteracao < quadroPressionado)
                    return;
            }
        }
        finally
        {
            if (documentoCurso != null && SessaoCenario.Atual.Permite(Cenario, visita))
                documentoCurso.MostrarBotao(false);
        }
    }

    public async Task AbrirExercicio(TipoExercicio tipo, ExercicioBase ex)
    {
        //Debug.Log("Abrindo exercício: " + tipo);

        ModalLegenda legenda = modalLegenda.GetComponent<ModalLegenda>();

        if (modalLegenda != null)
        {
            modalLegenda.SetActive(false);
        }

        await EntrarModoExercicio(tipo);

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

        await SairModoExercicio(tipo);

        // ✨ Volta a prender o mouse após terminar o exercício (se o diálogo geral não tiver acabado)
        DefinirCursorDesktop(false);

        if (legenda != null)
        {
            legenda.AjustarPosicaoPeloEstadoDoJogo();
        }
    }

    private async Task PlayAudioETexto(int i, bool apresentarDocumento = false)
    {
        ultimaFraseDita = dialogoAtual[i];
        await ApresentarFalaDoRoteiro(backendManagerComum, audioSourceComum, idNpcParaVoz,
            nomeExibicaoLegenda, ultimaFraseDita, modalLegenda, textoPularDialogo, avancar: () => AguardarAvancoComDocumento(apresentarDocumento));
    }

    public override async Task FalarFraseCustomizada(string textoParaFalar)
    {
        if (string.IsNullOrEmpty(textoParaFalar)) return;
        int visita = SessaoCenario.Atual.Versao;
        ValidarVisita(visita);

        byte[] audioBytes = await backendManagerComum.GerarAudio(textoParaFalar, idNpcParaVoz);
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
            audioSourceComum.clip = clip;
            if (modalLegenda != null)
            {
                modalLegenda.SetActive(true);

                if (textoPularDialogo != null)
                    textoPularDialogo.gameObject.SetActive(false);

                modalLegenda.GetComponent<ModalLegenda>()
                    .MostrarLegenda(nomeExibicaoLegenda, textoParaFalar);
            }
            
            await ReproduzirVoz(audioSourceComum, visita);
            ValidarVisita(visita);

            if (modalLegenda != null)
            {
                modalLegenda.SetActive(false);
            }
        }
    }
}
