using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using TMPro;
using System;

public class RestaurantWaiterNPC : InteracaoNPC
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

    public string idNpcParaVoz = "clothingSeller";

    protected override void Start()
    {
        base.Start();
        nivelAtual = DadosJogador.nivelUsuario;
        nomeExibicaoLegenda = "Restaurant Waiter";
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
                "Hello! Welcome to our clothing store. Can I help you?",
                "What size do you need?",
                "What color would you like?",
                "Sure! Here is a blue T-shirt in size small.",
                "Would you like to try it on?",
                "How does it fit?",
                "Would you like a larger size?",
                "Here is a larger one. Would you like to buy it?",
                "Of course! Would you like a bag?",
                "Thank you. Have a nice day!"
            };

            exerciciosAssociacao.Add(new ExercicioAssociacao
            {
                enunciado = "Match the Portuguese word with the English word.",

                opcoesPort = new List<string>
                {
                    "camiseta",
                    "calça",
                    "vestido",
                    "sapato",
                    "camisa",
                    "saia",
                    "azul",
                    "tamanho"
                },

                opcoesIngles = new List<string>
                {
                    "shoes",
                    "shirt",
                    "blue",
                    "skirt",
                    "T-shirt",
                    "size",
                    "dress",
                    "pants"
                },

                associacaoCorreta = new List<ParAssociacao>
                {
                    new ParAssociacao { palavraPortugues = "camiseta", palavraIngles = "T-shirt" },
                    new ParAssociacao { palavraPortugues = "calça", palavraIngles = "pants" },
                    new ParAssociacao { palavraPortugues = "vestido", palavraIngles = "dress" },
                    new ParAssociacao { palavraPortugues = "sapato", palavraIngles = "shoes" },
                    new ParAssociacao { palavraPortugues = "camisa", palavraIngles = "shirt" },
                    new ParAssociacao { palavraPortugues = "saia", palavraIngles = "skirt" },
                    new ParAssociacao { palavraPortugues = "azul", palavraIngles = "blue" },
                    new ParAssociacao { palavraPortugues = "tamanho", palavraIngles = "size" }
                }
            });
        }
        else if (nivelAtual == "A2")
        {
            dialogoAtual = new List<string>()
            {
                cumprimento+"Welcome to our store. Are you looking for something specific?",
                "Sure! What kind of style do you prefer?",
                "How about this shirt? It's one of our most popular models",
                "Yes! We have it in black and white. What size do you usually wear?",
                "Great! Let me check. We have a medium in black. It's thirty-five dollars.",
                "Of course! You can try it on.",
                "How does it fit?",
                "Of course! I have a large one. Would you like to try it?",
                "Of course! Would you like a bag?",
                "Does this one fit better?",
                "Would you like to buy it?",
                "Great! That will be thirty-five dollars. How would you like to pay?",
                "Perfect! Thank you for shopping with us. Have a great day!"
            };

            exerciciosAssociacao.Add(new ExercicioAssociacao
            {
                enunciado = "Match the Portuguese word with the English word.",

                opcoesPort = new List<string>
                {
                    "experimentar",
                    "vendedor",
                    "tamanho médio",
                    "caro",
                    "barato",
                    "outra cor",
                    "ajustar",
                    "vestir bem"
                },

                opcoesIngles = new List<string>
                {
                    "affordable",
                    "try on",
                    "fit well",
                    "salesperson",
                    "another color",
                    "medium size",
                    "adjust",
                    "expensive"
                },

                associacaoCorreta = new List<ParAssociacao>
                {
                    new ParAssociacao { palavraPortugues = "experimentar", palavraIngles = "try on" },
                    new ParAssociacao { palavraPortugues = "vendedor", palavraIngles = "salesperson" },
                    new ParAssociacao { palavraPortugues = "tamanho médio", palavraIngles = "medium size" },
                    new ParAssociacao { palavraPortugues = "caro", palavraIngles = "expensive" },
                    new ParAssociacao { palavraPortugues = "barato", palavraIngles = "affordable" },
                    new ParAssociacao { palavraPortugues = "outra cor", palavraIngles = "another color" },
                    new ParAssociacao { palavraPortugues = "ajustar", palavraIngles = "adjust" },
                    new ParAssociacao { palavraPortugues = "vestir bem", palavraIngles = "fit well" }
                }
            });

        }
        else if (nivelAtual == "B1")
        {
            dialogoAtual = new List<string>()
            {
                cumprimento+"Welcome to our store. How can I assist you today?",
                "Certainly! Do you have any particular preferences regarding style or color?",
                "I think this blazer could be a good option. It has a smart-casual design and can be combined with different types of trousers.",
                "Yes, we have it in navy. It's made from a cotton blend, and we recommend dry cleaning to keep the fabric in good condition.",
                "Would you like to see some trousers that would match the blazer?",
                "Certainly! We have these navy trousers for seventy-five dollars. They match the blazer quite well.",
                "Would you like to try both pieces on before making a decision?",
                "How do they fit? Are you comfortable with them?",
                "Of course! I'll get a larger size for you. Just a moment, please.",
                "How about this one? Does it feel more comfortable?",
                "Great! Would you like to purchase the blazer and the trousers?",
                "Excellent choice! The total is one hundred and sixty-four dollars. Would you like to pay by card or cash?",
                "Perfect! Your purchase is complete. Thank you for shopping with us. I hope you enjoy your new outfit!"
            };

            exerciciosAssociacao.Add(new ExercicioAssociacao
            {
                enunciado = "Match the Portuguese word with the English word.",

                opcoesPort = new List<string>
                {
                    "tecido",
                    "mistura de algodão",
                    "lavagem a seco",
                    "elegante",
                    "confortável",
                    "ombros",
                    "ajustado",
                    "vale a pena"
                },

                opcoesIngles = new List<string>
                {
                    "worth it",
                    "shoulders",
                    "fabric",
                    "comfortable",
                    "dry cleaning",
                    "fitted",
                    "cotton blend",
                    "elegant"
                },

                associacaoCorreta = new List<ParAssociacao>
                {
                    new ParAssociacao { palavraPortugues = "tecido", palavraIngles = "fabric" },
                    new ParAssociacao { palavraPortugues = "mistura de algodão", palavraIngles = "cotton blend" },
                    new ParAssociacao { palavraPortugues = "lavagem a seco", palavraIngles = "dry cleaning" },
                    new ParAssociacao { palavraPortugues = "elegante", palavraIngles = "elegant" },
                    new ParAssociacao { palavraPortugues = "confortável", palavraIngles = "comfortable" },
                    new ParAssociacao { palavraPortugues = "ombros", palavraIngles = "shoulders" },
                    new ParAssociacao { palavraPortugues = "ajustado", palavraIngles = "fitted" },
                    new ParAssociacao { palavraPortugues = "vale a pena", palavraIngles = "worth it" }
                }
            });
        }
    }
}