using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using TMPro;
using System;
using System.Globalization;

public class RecepcionistHotelNPC: InteracaoNPC
{
    string nivelAtual;
    string cumprimento, periodo, periodoIng;
    int horaAtual = DateTime.Now.Hour;
    List<string> dialogoAtual;
    private AudioSource audioSource;
    private BackendManager backendManager;
    public ModalExercicio modalExercicio;
    public GameObject modalLegenda;
    public TextMeshProUGUI textoPularDialogo;
    public ExercicioBase exAtual;

    public string idNpcParaVoz = "recepcionistHotel";

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
        nomeExibicaoLegenda = "Hotel Recepcionist";
        exerciciosCompletarFrase = GarantirListaExercicios(exerciciosCompletarFrase);
        exerciciosCriarFrases = GarantirListaExercicios(exerciciosCriarFrases);
        exerciciosSpeaking = GarantirListaExercicios(exerciciosSpeaking);
        exerciciosAlternativas = GarantirListaExercicios(exerciciosAlternativas);
        exerciciosWriting = GarantirListaExercicios(exerciciosWriting);
        exerciciosAssociacao = GarantirListaExercicios(exerciciosAssociacao);

        if (horaAtual < 12)
        {
            cumprimento = "Good morning!";
            periodo = "de manhã";
            periodoIng = "this morning";

        }
        else if (horaAtual < 18)
        {
            cumprimento = "Good afternoon!";
            periodo = "a tarde";
            periodoIng = "this afternoon";
        }
        else
        {
            cumprimento = "Good evening!";
            periodo = "a noite";
            periodoIng = "tonight";
        }
    }

    private void InicializarConteudosPorNivel()
    {
        // Evita duplicar exercícios caso o jogador interaja com o NPC novamente.
        exerciciosCompletarFrase.Clear();
        exerciciosCriarFrases.Clear();
        exerciciosSpeaking.Clear();
        exerciciosAlternativas.Clear();
        exerciciosWriting.Clear();
        exerciciosAssociacao.Clear();

        string dataCheckout = ObterDataCheckout();
        if (nivelAtual == "A1")
        {
            dialogoAtual = new List<string>()
            {
                "Hello. Can I help you?",
                "What is yout name?",
                "Would you like to pay by cash or card?",
                "Your room is 103. It is on the first floor",
                "Breakfast is from 7:00 a.m. to 9:00 a.m.",
                "Checkout is on "+dataCheckout+".",
                "Call me if you have a question."
            };

            exerciciosSpeaking.Add(
                new ExercicioSpeaking()
                {
                    enunciado = "You are at the hotel. Say the correct sentence",
                    opcoesFala = new List<string>()
                    {
                        "I have a reservation\n",
                        "I am the breakfast yesterday\n",
                        "Where are the classroom do?\n"
                    },
                    respostaCorreta = 0
                }
            );

            exerciciosWriting.Add(
                new ExercicioWriting()
                {
                    enunciado = "What is your name? Write your answer."
                }
            );

            exerciciosAlternativas.Add(
                new ExercicioAlternativas()
                {
                    enunciado = "The receptionist asked how you would like to pay. Choose the correct answer:",
                    alternativas = new List<string>() { 
                        "I would like to pay by room\r\n", 
                        "I would like to pay by breakfast\r\n", 
                        "I did pay by card\r\n", 
                        "I would like to pay by card\r\n" },
                    alternativaCorreta = 3
                }
            );

            exerciciosCriarFrases.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Arrange the words to form the correct sentence in english: Qual o horário do café da manhã?",
                    blocosPalavras = new List<string>
                    {
                        "dinner",
                        "served",
                        "What",
                        "Which",
                        "breakfast",
                        "when",
                        "time",
                        "is",
                        "the",
                        "lunch",
                        "are"
                    },
                    respostaCorreta = "What time is breakfast served?\n"
                }
            );

                exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Speak the corret answaer to ask about the checkout",
                        opcoesFala = new List<string>()
                        {
                            "Where is my breakfast yesterday?\n",
                            "When is my checkout?\n",
                            "When my checkout are?\n"
                        },
                        respostaCorreta = 1
                    }
                );   

                exerciciosAlternativas.Add(
                    new ExercicioAlternativas()
                    {
                        enunciado = "How long will you be staying at the hotel?",
                        alternativas = new List<string>() { 
                        "Two days\r\n", 
                        "One week\r\n", 
                        "Five days\r\n", 
                        "Two weeks\r\n" },
                        alternativaCorreta = 1
                    }
                );     

                exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Thank the receptionist",
                        opcoesFala = new List<string>() { "Thank you"+"\n" },
                        respostaCorreta = 0
                    }
                );    

                exerciciosAssociacao.Add(
                    new ExercicioAssociacao()
                    {
                        enunciado = "Match the Portuguese word with the English word.",
                        opcoesPort = new List<string>
                        {
                            "reserva",
                            "nome",
                            "cartão",
                            "quarto",
                            "andar",
                            "café da manhã",
                            "saída",
                            "semana"
                        },
                        opcoesIngles = new List<string>
                        {
                            "room",
                            "week",
                            "name",
                            "card",
                            "checkout",
                            "reservation",
                            "floor",
                            "breakfast",
                        },
                        associacaoCorreta = new List<ParAssociacao>
                        {
                        new ParAssociacao { palavraPortugues = "reserva", palavraIngles = "reservation" },
                        new ParAssociacao { palavraPortugues = "nome", palavraIngles = "name" },
                        new ParAssociacao { palavraPortugues = "cartão", palavraIngles = "card" },
                        new ParAssociacao { palavraPortugues = "quarto", palavraIngles = "room" },
                        new ParAssociacao { palavraPortugues = "andar", palavraIngles = "floor" },
                        new ParAssociacao { palavraPortugues = "café da manhã", palavraIngles = "breakfast" },
                        new ParAssociacao { palavraPortugues = "saída", palavraIngles = "checkout" },
                        new ParAssociacao { palavraPortugues = "semana", palavraIngles = "week" }
                        }
                    }
                );
        }
        else if(nivelAtual=="A2")
        {
            dialogoAtual = new List<string>()
            {
                cumprimento+". Welcome to the hotel. Do you have a reservation?",
                "Great. Could I have your name, please?",
                "Thank you. How many nights are you staying with us?",
                "Would you like to pay by card or cash?",
                "Your room is 205, on the second floor. Here is your key card.",
                "Breakfast is from 7:00 a.m. to 10:00 a.m. in the restaurant. It's on the first floor.",
                "Is there anything else you would like to know?",
                "Checkout is at 12:00 p.m. on " + dataCheckout + ".",
                "Is there anything else I can help you with?",
                "Enjoy your stay!"
            };

            exerciciosCriarFrases.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Arrange the words to form the correct sentence in english: Eu tenho uma reserva para hoje "+periodo,
                    blocosPalavras = new List<string>
                    {
                        "room",
                        "served",
                        "have",
                        "Which",
                        "tonight",
                        "afternoon",
                        "morning",
                        "when",
                        "reservation",
                        "is",
                        "for",
                        "lunch",
                        "I",
                        "this"
                    },
                    respostaCorreta = "I have a reservation for "+periodoIng+"\n"
                }
            );

            exerciciosWriting.Add(
                new ExercicioWriting()
                {
                    enunciado = "What is your name? Write your answer."
                }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "How long will you be staying at the hotel?",
                        opcoesFala = new List<string>()
                        {
                            "I staying one week ago\n",
                            "I'll be paying on the fifth floor\n",
                            "I'll be staying for one week\n"
                        },
                        respostaCorreta = 2
                    }
            ); 

            exerciciosAlternativas.Add(
                new ExercicioAlternativas()
                {
                    enunciado = "The receptionist asked how you would like to pay. Choose the correct answer:",
                    alternativas = new List<string>() { 
                        "I'd like to pay on the fifth floor\r\n", 
                        "I'd like to pay by card\r\n", 
                        "I'd like to pay at seven o'clock\r\n", 
                        "I'd like to pay with my room\r\n" },
                    alternativaCorreta = 1
                }
            );

            exerciciosWriting.Add(
                new ExercicioWriting()
                {
                    enunciado = "You want to know where you can have breakfast and what time it is. Ask these informations for the receptionist"
                }
            );

            exerciciosAlternativas.Add(
                new ExercicioAlternativas()
                {
                    enunciado = "Where can you have breakfast?",
                    alternativas = new List<string>() { 
                        "At the reception desk\r\n", 
                        "On the second floor\r\n", 
                        "In room 205\r\n", 
                        "On the first floor\r\n" },
                    alternativaCorreta = 3
                }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "You want to know the checkout time. What do you ask the receptionist?",
                        opcoesFala = new List<string>()
                        {
                            "What time is the checkout?\n",
                            "Where can I leave my luggage?\n",
                            "Does checkout include breakfast?\n"
                        },
                        respostaCorreta = 0
                    }
            ); 

            exerciciosCompletarFrase.Add(
                new ExercicioCompletarFrase()
                {
                    enunciado = "Complete the sentence with the correct word:",
                    fraseAntes = "I need to leave at ",
                    fraseDepois = " p.m.",
                    respostaCorreta = "12:00",
                    opcoes = new List<string>
                    {
                        "10:00",
                        "served",
                        "have",
                        "Which",
                        "tonight",
                        "11:00",
                        "morning",
                        "12:00",
                        "is",
                        "for",
                        "20:00",
                        "I",
                        "1:00"
                    }
                }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "You don’t need anything else. What do you say?",
                        opcoesFala = new List<string>()
                        {
                            "No, I am in the first floor\n",
                            "No, thank you. That's all\n",
                            "Thank you, I want the breakfast in\n"
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
                            "noite",
                            "hospedagem",
                            "pagar",
                            "cartão do quarto",
                            "perguntar",
                            "sair",
                            "bagagem",
                            "recepção"
                        },
                        opcoesIngles = new List<string>
                        {
                            "leave",
                            "pay",
                            "night",
                            "luggage",
                            "reception desk",
                            "stay",
                            "room card",
                            "ask"
                        },
                        associacaoCorreta = new List<ParAssociacao>
                        {
                        new ParAssociacao { palavraPortugues = "noite", palavraIngles = "night" },
                        new ParAssociacao { palavraPortugues = "hospedagem", palavraIngles = "stay" },
                        new ParAssociacao { palavraPortugues = "pagar", palavraIngles = "pay" },
                        new ParAssociacao { palavraPortugues = "cartão do quarto", palavraIngles = "room card" },
                        new ParAssociacao { palavraPortugues = "perguntar", palavraIngles = "ask" },
                        new ParAssociacao { palavraPortugues = "bagagem", palavraIngles = "luggage" },
                        new ParAssociacao { palavraPortugues = "sair", palavraIngles = "leave" },
                        new ParAssociacao { palavraPortugues = "recepção", palavraIngles = "reception desk" }
                        }
                    }
            );
        }
        else if(nivelAtual=="B1")
        {
            dialogoAtual = new List<string>()
            {
                cumprimento+". Welcome to the hotel. How can I help you today?",
                "Of course. Could you tell me the name the reservation is under?",
                "I found it. You booked a double room for seven nights, with breakfast included. Is that right?",
                "Perfect. Would you like to pay now by card or cash, or when you check out?",
                "Your room is 308, on the third floor. Here is your key card. The Wi-Fi is free, and the password is on the card.",
                "The elevator is just around the corner, next to the restaurant.",
                "Breakfast is served from 6:30 a.m. to 10:00 a.m. If you prefer, we can also bring it to your room.",
                "Is there anything else you'd like to know about your stay?",
                "Checkout is at 12:00 p.m. on " + dataCheckout + ". If you need a late checkout, please let us know in advance.",
                "We can offer a late checkout until 2:00 p.m., depending on availability.",
                "I hope you enjoy your stay. Just call the front desk if you need anything."
            };

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Tell her you have a reservation, and say how many people it is for.",
                        opcoesFala = new List<string>()
                        {
                            "I have a reservation for one person\n",
                            "I have a breakfast for two people\n",
                            "I reservation for one person\n"
                        },
                        respostaCorreta = 0
                    }
            ); 

            exerciciosWriting.Add(
                new ExercicioWriting()
                {
                    enunciado = "The receptionist asks for the name on the reservation. Write your answer in a complete sentence using -under the name-."
                }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Confirm the question made by the recepcionist",
                        opcoesFala = new List<string>()
                        {
                            "Yes, that's correct\n",
                            "No, I am a double room\n",
                            "Yes, that are correct\n"
                        },
                        respostaCorreta = 0
                    }
            ); 

            exerciciosAlternativas.Add(
                new ExercicioAlternativas()
                {
                    enunciado = "Select the correct answer to say how would you like to pay",
                    alternativas = new List<string>() { 
                        "I'd like to pay at breakfast\r\n", 
                        "I'd like to pay on the second floor\r\n", 
                        "I'd like to pay by card, please\r\n", 
                        "I'd like to pay for seven nights ago\r\n" },
                    alternativaCorreta = 2
                }
            );

            exerciciosCriarFrases.Add(
                new ExercicioBlocos()
                {
                    enunciado= "Arrange the words to form the correct sentence in english: Onde fica o elevador?"+periodo,
                    blocosPalavras = new List<string>
                    {
                        "Where",
                        "served",
                        "elevator",
                        "Which",
                        "how",
                        "elevator",
                        "perfect",
                        "when",
                        "Who",
                        "is",
                        "for",
                        "lunch",
                        "the",
                        "are"
                    },
                    respostaCorreta = "Where is the elevator? \n"
                }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Speak the correct answer to ask about the place and time of the breakfast",
                        opcoesFala = new List<string>()
                        {
                            "Where breakfast are the time?\n",
                            "What time is breakfast and where is it?\n",
                            "What time is breakfast yesterday?\n"
                        },
                        respostaCorreta = 1
                    }
            );

            exerciciosAlternativas.Add(
                new ExercicioAlternativas()
                {
                    enunciado = "What time does the breakfast end?",
                    alternativas = new List<string>(){ 
                        "6:30 p.m\r\n", 
                        "9:00 a.m\r\n", 
                        "10:00 a.m\r\n", 
                        "12:00 p.m\r\n" },
                    alternativaCorreta = 3
                }
            );

            exerciciosWriting.Add(
                new ExercicioWriting()
                {
                    enunciado = "Write a sentece asking about the time of the checkout."
                }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "The receptionist says you can ask for a late checkout. Your flight is in the evening and you need more time in the room. What do you say?",
                        opcoesFala = new List<string>()
                        {
                            "I am the menu yesterday\n",
                            "Yes, please. Could you help me?\n",
                            "The classroom is amazing\n"
                        },
                        respostaCorreta = 1
                    }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "You have a flight at 5:00 p.m. Ask the recepcionist if you can checkout later. ",
                    }
            );

            exerciciosCompletarFrase.Add(
                new ExercicioCompletarFrase()
                {
                    enunciado = "Complete the sentence with the correct word:",
                    fraseAntes = "The late checkout is available until ",
                    fraseDepois = " p.m.",
                    respostaCorreta = "2:00",
                    opcoes = new List<string>
                    {
                        "10:00",
                        "served",
                        "2:00",
                        "Which",
                        "tonight",
                        "1:00",
                        "morning",
                        "12:00",                        "5:00",
                        "I",
                    }
                }
            );

            exerciciosSpeaking.Add(
                    new ExercicioSpeaking()
                    {
                        enunciado = "Choose the correct sentence to say you don't need anything else.",
                        opcoesFala = new List<string>()
                        {
                            "No, I need the room 308\n",
                            "Thank you, all I need are today\n",
                            "No, thank you. That's all I need\n"
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
                            "reservar",
                            "quarto de casal",
                            "ao lado de",
                            "servido",
                            "com antecedência",
                            "voo",
                            "sair",
                            "disponibilidade",
                        },
                        opcoesIngles = new List<string>
                        {
                            "double room",
                            "next to",
                            "availability",
                            "served",
                            "book",
                            "in advance",
                            "flight",
                            "leave",
                        },
                        associacaoCorreta = new List<ParAssociacao>
                        {
                        new ParAssociacao { palavraPortugues = "reservar", palavraIngles = "book" },
                        new ParAssociacao { palavraPortugues = "quarto de casal", palavraIngles = "double room" },
                        new ParAssociacao { palavraPortugues = "ao lado de", palavraIngles = "next to" },
                        new ParAssociacao { palavraPortugues = "servido", palavraIngles = "served" },
                        new ParAssociacao { palavraPortugues = "com antecedência", palavraIngles = "in advance" },
                        new ParAssociacao { palavraPortugues = "voo", palavraIngles = "flight" },
                        new ParAssociacao { palavraPortugues = "sair", palavraIngles = "leave" },
                        new ParAssociacao { palavraPortugues = "disponibilidade", palavraIngles = "availability" }
                        }
                    }
            );
        }
    }

    private string ObterDataCheckout()
    {
        DateTime dataCheckout = DateTime.Now.Date.AddDays(7);

        int dia = dataCheckout.Day;
        string sufixo;

        if (dia % 100 >= 11 && dia % 100 <= 13)
        {
            sufixo = "th";
        }
        else
        {
            switch (dia % 10)
            {
                case 1:
                    sufixo = "st";
                    break;
                case 2:
                    sufixo = "nd";
                    break;
                case 3:
                    sufixo = "rd";
                    break;
                default:
                    sufixo = "th";
                    break;
            }
        }

        string mes = dataCheckout.ToString(
            "MMMM",
            CultureInfo.GetCultureInfo("en-US")
        );

        return $"{mes} {dia}{sufixo}";
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
            IniciarMissao(
                "hotel_a1_conversar",
                "Fale com o recepcionista do hotel",
                "Talk to the hotel receptionist"
            );

            await interacaoA1();
        }
        else if (nivelAtual == "A2")
        {
            IniciarMissao(
                "hotel_a2_checkin",
                "Fale com o recepcionista do hotel e realize seu check-in",
                "Talk to the hotel receptionist and check in"
            );

            await interacaoA2();
        }
        else
        {
            IniciarMissao(
                "hotel_b1_checkin",
                "Informe seus dados e confirme as informações da reserva",
                "Provide your details and confirm your reservation information"
            );

            await interacaoB1();
        }

        GameProgress.EstaEmDialogo = false;

        if (modalLegenda != null)
            modalLegenda.SetActive(false);

        if (textoPularDialogo != null)
            textoPularDialogo.gameObject.SetActive(false);

        GameProgress.EstaEmDialogo = false;
        LiberarControlePlayer();

        if (RevisaoManager.Instance != null)
            await RevisaoManager.Instance.ExecutarRevisaoEExibirModalFinal(this);
        else
            Debug.LogError("RevisaoManager não está disponível para finalizar Hotel.");
    }

    private async Task interacaoA1()
    {
        int i = 0;

        // Conversa inicial e identificação do hóspede.
        await PlayAudioETexto(i++);
        exAtual = exerciciosSpeaking[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++);
        exAtual = exerciciosWriting[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        // Pagamento: finaliza a missão de conversar com a recepção.
        await PlayAudioETexto(i++);
        exAtual = exerciciosAlternativas[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        MissionManager.ConcluirMissaoNPC("hotel_a1_conversar");

        IniciarMissao(
            "hotel_a1_servicos",
            "Conheça os serviços do hotel durante a conversa",
            "Learn about the hotel's services during the conversation"
        );

        // Informações do quarto, café da manhã e checkout.
        await PlayAudioETexto(i++);
        await AbrirExercicio(
            ObterTipoExercicio(exerciciosCriarFrases[0]),
            exerciciosCriarFrases[0]
        );

        await PlayAudioETexto(i++);
        exAtual = exerciciosSpeaking[1];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++);
        exAtual = exerciciosAlternativas[1];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        // A recepcionista encerra oferecendo ajuda; o usuário agradece em seguida.
        await PlayAudioETexto(i++);
        exAtual = exerciciosSpeaking[2];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        MissionManager.ConcluirMissaoNPC("hotel_a1_servicos");

        await AbrirExercicio(
            ObterTipoExercicio(exerciciosAssociacao[0]),
            exerciciosAssociacao[0]
        );
    }

    private async Task interacaoA2()
    {
        int i = 0;

        await PlayAudioETexto(i++);
        exAtual = exerciciosCriarFrases[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++);
        exAtual = exerciciosWriting[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++);
        await AbrirExercicio(
            ObterTipoExercicio(exerciciosSpeaking[0]),
            exerciciosSpeaking[0]
        );

        await PlayAudioETexto(i++);
        exAtual = exerciciosAlternativas[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        // Check-in concluído após a escolha da forma de pagamento.
        MissionManager.ConcluirMissaoNPC("hotel_a2_checkin");

        IniciarMissao(
            "hotel_a2_cartao_quarto",
            "Pegue o cartão do quarto",
            "Collect your room card"
        );

        await PlayAudioETexto(i++);
        MissionManager.ConcluirMissaoNPC("hotel_a2_cartao_quarto");

        // Informações sobre café da manhã e checkout.
        exAtual = exerciciosWriting[1];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++);
        await AbrirExercicio(
            ObterTipoExercicio(exerciciosAlternativas[1]),
            exerciciosAlternativas[1]
        );

        await PlayAudioETexto(i++);
        await AbrirExercicio(
            ObterTipoExercicio(exerciciosSpeaking[1]),
            exerciciosSpeaking[1]
        );

        await PlayAudioETexto(i++);
        await AbrirExercicio(
            ObterTipoExercicio(exerciciosCompletarFrase[0]),
            exerciciosCompletarFrase[0]
        );

        await PlayAudioETexto(i++);
        await AbrirExercicio(
            ObterTipoExercicio(exerciciosSpeaking[2]),
            exerciciosSpeaking[2]
        );

        await PlayAudioETexto(i++);

        await AbrirExercicio(
            ObterTipoExercicio(exerciciosAssociacao[0]),
            exerciciosAssociacao[0]
        );
    }

    private async Task interacaoB1()
    {
        int i = 0;

        // Reserva, identificação e confirmação dos dados.
        await PlayAudioETexto(i++);
        exAtual = exerciciosSpeaking[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++);
        await AbrirExercicio(
            ObterTipoExercicio(exerciciosWriting[0]),
            exerciciosWriting[0]
        );

        await PlayAudioETexto(i++);
        exAtual = exerciciosSpeaking[1];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++);
        exAtual = exerciciosAlternativas[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        MissionManager.ConcluirMissaoNPC("hotel_b1_checkin");

        IniciarMissao(
            "hotel_b1_servicos",
            "Conheça os serviços do hotel e obtenha informações sobre o quarto e o café da manhã",
            "Learn about the hotel services and get information about your room and breakfast"
        );

        // Cartão do quarto, elevador e café da manhã.
        await PlayAudioETexto(i++);
        exAtual = exerciciosCriarFrases[0];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        await PlayAudioETexto(i++);
        await AbrirExercicio(
            ObterTipoExercicio(exerciciosSpeaking[2]),
            exerciciosSpeaking[2]
        );

        await PlayAudioETexto(i++);
        exAtual = exerciciosAlternativas[1];
        await AbrirExercicio(ObterTipoExercicio(exAtual), exAtual);

        MissionManager.ConcluirMissaoNPC("hotel_b1_servicos");

        IniciarMissao(
            "hotel_b1_late_checkout",
            "Consulte o horário do checkout e solicite um checkout tardio, se necessário",
            "Check the checkout time and request a late checkout if needed"
        );

        // Checkout: primeiro pergunta o horário; depois solicita a saída tardia.
        await PlayAudioETexto(i++);
        await AbrirExercicio(
            ObterTipoExercicio(exerciciosWriting[1]),
            exerciciosWriting[1]
        );

        await PlayAudioETexto(i++);
        await AbrirExercicio(
            ObterTipoExercicio(exerciciosSpeaking[3]),
            exerciciosSpeaking[3]
        );

        // O pedido livre vem antes da resposta da recepcionista sobre disponibilidade.
        await AbrirExercicio(
            ObterTipoExercicio(exerciciosSpeaking[4]),
            exerciciosSpeaking[4]
        );

        await PlayAudioETexto(i++);
        await AbrirExercicio(
            ObterTipoExercicio(exerciciosCompletarFrase[0]),
            exerciciosCompletarFrase[0]
        );

        await PlayAudioETexto(i++);

        MissionManager.ConcluirMissaoNPC("hotel_b1_late_checkout");

        await AbrirExercicio(
            ObterTipoExercicio(exerciciosAssociacao[0]),
            exerciciosAssociacao[0]
        );
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
