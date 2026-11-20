using System;
using System.Collections.Generic;

internal static class MissoesClothingStore
{
    internal const string Cenario = "Clothing Store";
    internal const string NpcKey = "clothing_seller_npc";
    internal const string ExercicioEncerrado = "exercicio_encerrado";
    internal const string RoupaExperimentada = "roupa_experimentada";

    internal static string AlvoExercicio(string ponto, int ordem) => $"{NpcKey}/{ponto}/{ordem}";

    private static int Quantidade(string nivel)
    {
        switch (nivel)
        {
            case "A1":
            case "A1-A2":
            case "A2": return 4;
            case "A2-B1":
            case "B1": return 5;
            default: return 0;
        }
    }

    private static string Chave(string nivel, int ordem) =>
        $"roupas_{nivel.ToLowerInvariant().Replace("-", "")}_{ordem}";

    internal static HashSet<string> ChavesEsperadas(string nivel)
    {
        var chaves = new HashSet<string>();
        for (int ordem = 1; ordem <= Quantidade(nivel); ordem++)
            chaves.Add(Chave(nivel, ordem));
        return chaves;
    }

    internal static bool ValidaEvento(CatalogoMissoes.Evento evento)
    {
        if (evento.tipo == RoupaExperimentada)
            return evento.alvo == NpcKey;
        if (evento.tipo != ExercicioEncerrado)
            return false;

        string[] partes = evento.alvo.Split('/');
        return partes.Length == 3 && partes[0] == NpcKey && !string.IsNullOrWhiteSpace(partes[1])
            && int.TryParse(partes[2], out int ordem) && ordem > 0;
    }

    private static CatalogoMissoes.Evento Evento(string tipo, string alvo) =>
        new CatalogoMissoes.Evento { tipo = tipo, alvo = alvo };

    private static CatalogoMissoes.Evento Exercicio(string ponto, int ordem = 1) =>
        Evento(ExercicioEncerrado, AlvoExercicio(ponto, ordem));

    private static CatalogoMissoes.Regra Regra(params CatalogoMissoes.Evento[] eventos) =>
        new CatalogoMissoes.Regra
        {
            operador = eventos.Length == 1 ? "EVENTO" : "AND",
            eventos = new List<CatalogoMissoes.Evento>(eventos)
        };

    internal static CatalogoMissoes Criar(string nivel)
    {
        var definicoes = new List<CatalogoMissoes.Definicao>();
        var prova = Evento(RoupaExperimentada, NpcKey);

        void Adicionar(string texto, string traducao, params CatalogoMissoes.Evento[] conclusao)
        {
            int ordem = definicoes.Count + 1;
            definicoes.Add(new CatalogoMissoes.Definicao
            {
                mission_key = Chave(nivel, ordem),
                nivel = nivel,
                texto = texto,
                traducao = traducao,
                ordem = ordem,
                inicio = ordem == 1 ? Regra(Evento("cenario_iniciado", Cenario))
                    : Regra(Evento("missao_concluida", Chave(nivel, ordem - 1))),
                conclusao = Regra(conclusao)
            });
        }

        switch (nivel)
        {
            case "A1":
                Adicionar("Talk to the salesperson in the clothing store",
                    "Converse com o vendedor na loja de roupas", Exercicio("shopping_request"));
                Adicionar("Choose a T-shirt and tell the salesperson your size and preferred color",
                    "Escolha uma camiseta e diga ao vendedor seu tamanho e a cor que prefere",
                    Exercicio("size_choice"), Exercicio("color_choice"));
                Adicionar("Try on the T-shirt and explain how it fits",
                    "Experimente a camiseta e explique como ela ficou", prova, Exercicio("fit_feedback"));
                Adicionar("Choose your T-shirt and pay for it.",
                    "Escolha sua camiseta e pague por ela.", Exercicio("bag_choice"));
                break;
            case "A1-A2":
                Adicionar("Talk to the salesperson and explain what kind of clothing you need",
                    "Converse com o vendedor e explique de que tipo de roupa você precisa", Exercicio("shopping_request"));
                Adicionar("Choose a T-shirt and explain your style, color, and size preferences",
                    "Escolha uma camiseta e explique suas preferências de estilo, cor e tamanho", Exercicio("size_choice"));
                Adicionar("Try on the T-shirt and explain whether it fits comfortably",
                    "Experimente a camiseta e explique se ela ficou confortável", prova, Exercicio("fit_feedback", 2));
                Adicionar("Decide which T-shirt you want and finish your purchase.",
                    "Decida qual camiseta você quer e finalize sua compra.", Exercicio("payment"));
                break;
            case "A2":
                Adicionar("Talk to the salesperson and explain what you are looking for",
                    "Converse com o vendedor e explique o que está procurando", Exercicio("shopping_request"));
                Adicionar("Explore the available options and ask for recommendations",
                    "Explore as opções disponíveis e peça recomendações", Exercicio("size_choice"));
                Adicionar("Try on the shirt and explain if you need a different size",
                    "Experimente a camisa e explique se precisa de outro tamanho", prova, Exercicio("fit_feedback"));
                Adicionar("Select the shirt you prefer and proceed to checkout.",
                    "Escolha a camisa que prefere e prossiga para o pagamento.", Exercicio("payment"));
                break;
            case "A2-B1":
                Adicionar("Explain the occasion and describe the outfit you are looking for to the salesperson",
                    "Explique a ocasião ao vendedor e descreva o conjunto que está procurando", Exercicio("shopping_request"));
                Adicionar("Explore the clothing options and ask about the blazer's characteristics",
                    "Explore as opções de roupas e pergunte sobre as características do blazer", Exercicio("garment_care"));
                Adicionar("Ask about matching trousers and compare the available outfit options",
                    "Pergunte sobre calças que combinem e compare as opções de conjuntos disponíveis", Exercicio("price"));
                Adicionar("Try on the outfit and explain whether any adjustments are needed",
                    "Experimente o conjunto e explique se algum ajuste é necessário", prova, Exercicio("fit_feedback"));
                Adicionar("Decide on the most suitable outfit and finalize your purchase.",
                    "Escolha o conjunto mais adequado e finalize sua compra.", Exercicio("payment"));
                break;
            case "B1":
                Adicionar("Explain what kind of outfit you need and for what occasion",
                    "Explique de que tipo de conjunto precisa e para qual ocasião", Exercicio("shopping_request"));
                Adicionar("Explore the options and ask about the characteristics of the clothing",
                    "Explore as opções e pergunte sobre as características das peças", Exercicio("product_recommendation"));
                Adicionar("Compare the options and ask about price and fabric care",
                    "Compare as opções e pergunte sobre o preço e os cuidados com o tecido", Exercicio("fitting"));
                Adicionar("Try on the outfit, explain what needs adjusting, and ask for another size",
                    "Experimente o conjunto, explique o que precisa de ajuste e peça outro tamanho", prova, Exercicio("fit_feedback", 2));
                Adicionar("Choose the outfit that best meets your needs and complete the purchase",
                    "Escolha o conjunto que melhor atende às suas necessidades e finalize a compra", Exercicio("price", 2));
                break;
        }

        return definicoes.Count == 0 ? null : new CatalogoMissoes(definicoes, nivel, Cenario, catalogoLocal: true);
    }
}
