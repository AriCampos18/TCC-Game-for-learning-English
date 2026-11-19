using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[Serializable]
public class ExerciseApiTypeDTO
{
    public int id;
    public string nome;
    [JsonProperty("modo_livre")] public bool modoLivre;
}

[Serializable]
public class ExerciseApiAlternativeDTO
{
    public int id;
    public string description;
    public bool correta;
    public int? ordem;
}

[Serializable]
public class ExerciseApiWordBlockDTO
{
    public int id;
    public string texto;
    public int ordem;
}

[Serializable]
public class ExerciseApiPhraseCompletionDTO
{
    public int id;
    [JsonProperty("frase_antes")] public string fraseAntes;
    [JsonProperty("frase_depois")] public string fraseDepois;
    [JsonProperty("resposta_correta")] public string respostaCorreta;
    [JsonProperty("partes_frase")] public List<string> partesFrase;
    [JsonProperty("respostas_corretas")] public List<string> respostasCorretas;
}

[Serializable]
public class ExerciseApiAssociationPairDTO
{
    public int id;
    public string portugues;
    public string ingles;
}

[Serializable]
public class ExerciseApiExpectedAnswerDTO
{
    public int id;
    public string resposta;
}

[Serializable]
public class ExerciseApiDTO
{
    public int id;
    public string enunciado;
    public string feedback;
    public string nivel;
    [JsonProperty("id_tipo")] public int idTipo;
    public ExerciseApiTypeDTO tipo;
    [JsonProperty("id_cenario")] public int idCenario;
    [JsonProperty("id_execution_point")] public int? idExecutionPoint;
    [JsonProperty("point_key")] public string pointKey;
    public int? ordem;
    public List<ExerciseApiAlternativeDTO> alternatives = new List<ExerciseApiAlternativeDTO>();
    [JsonProperty("word_blocks")] public List<ExerciseApiWordBlockDTO> wordBlocks = new List<ExerciseApiWordBlockDTO>();
    [JsonProperty("phrase_completion")] public ExerciseApiPhraseCompletionDTO phraseCompletion;
    [JsonProperty("association_pairs")] public List<ExerciseApiAssociationPairDTO> associationPairs = new List<ExerciseApiAssociationPairDTO>();
    [JsonProperty("expected_answers")] public List<ExerciseApiExpectedAnswerDTO> expectedAnswers = new List<ExerciseApiExpectedAnswerDTO>();

    public TipoExercicio ConvertToExecutable(out ExercicioBase exercise)
    {
        if (tipo == null)
            throw new InvalidOperationException("O exercício não contém o objeto 'tipo'.");

        string typeName = (tipo.nome ?? string.Empty).Trim();
        TipoExercicio modality;

        if (string.Equals(typeName, "Speaking", StringComparison.OrdinalIgnoreCase))
        {
            modality = TipoExercicio.Speaking;
            exercise = ToSpeakingExercise();
        }
        else if (IsOneOf(typeName, "Writing"))
        {
            modality = TipoExercicio.Writing;
            exercise = new ExercicioWriting();
        }
        else if (IsOneOf(typeName, "PhraseCompletion", "CompletarFrase", "Completar Frase"))
        {
            modality = TipoExercicio.CompletarFrase;
            exercise = ToPhraseCompletionExercise();
        }
        else if (IsOneOf(typeName, "CreateSentences", "WordBlocks", "Blocos", "Criar Frase"))
        {
            modality = TipoExercicio.Blocos;
            exercise = ToWordBlocksExercise();
        }
        else if (IsOneOf(typeName, "Association", "Associacao", "Associação"))
        {
            modality = TipoExercicio.Associacao;
            exercise = ToAssociationExercise();
        }
        else if (IsOneOf(typeName, "Alternatives", "Alternativas"))
        {
            modality = TipoExercicio.Alternativas;
            exercise = ToAlternativesExercise();
        }
        else
        {
            throw new InvalidOperationException("Tipo de exercício não suportado: " + typeName);
        }

        exercise.idExercise = id;
        exercise.idTipoBanco = idTipo;
        exercise.nomeTipoBanco = typeName;
        exercise.nivelBanco = nivel;
        exercise.pointKey = pointKey;
        exercise.ordemBanco = ordem ?? 0;
        exercise.enunciado = enunciado;
        exercise.feedback = feedback;
        exercise.respostasEsperadas = GetExpectedAnswerTexts();
        return modality;
    }

    public ExercicioSpeaking ToSpeakingExercise()
    {
        if (tipo == null || !string.Equals(tipo.nome, "Speaking", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("O exercício retornado pela API não é do tipo Speaking.");

        List<ExerciseApiAlternativeDTO> ordered = OrderedAlternatives();
        var result = new ExercicioSpeaking
        {
            enunciado = enunciado,
            feedback = feedback,
            modoLivre = tipo.modoLivre,
            opcoesFala = tipo.modoLivre ? null : new List<string>(),
            respostaCorreta = -1
        };

        if (!tipo.modoLivre)
        {
            for (int i = 0; i < ordered.Count; i++)
            {
                result.opcoesFala.Add(ordered[i].description);
                if (ordered[i].correta)
                    result.respostaCorreta = i;
            }
        }

        return result;
    }

    private ExercicioAlternativas ToAlternativesExercise()
    {
        List<ExerciseApiAlternativeDTO> ordered = OrderedAlternatives();
        var result = new ExercicioAlternativas
        {
            alternativas = new List<string>(),
            alternativaCorreta = -1
        };
        for (int i = 0; i < ordered.Count; i++)
        {
            result.alternativas.Add(ordered[i].description);
            if (ordered[i].correta)
                result.alternativaCorreta = i;
        }
        return result;
    }

    private ExercicioBlocos ToWordBlocksExercise()
    {
        var result = new ExercicioBlocos
        {
            blocosPalavras = new List<string>(),
            respostaCorreta = FirstExpectedAnswer()
        };
        if (wordBlocks != null)
        {
            var orderedBlocks = new List<ExerciseApiWordBlockDTO>(wordBlocks);
            orderedBlocks.Sort((left, right) =>
            {
                int orderCompare = left.ordem.CompareTo(right.ordem);
                return orderCompare != 0 ? orderCompare : left.id.CompareTo(right.id);
            });
            foreach (ExerciseApiWordBlockDTO item in orderedBlocks)
                if (item != null) result.blocosPalavras.Add(item.texto);
        }
        if (string.IsNullOrEmpty(result.respostaCorreta))
            result.respostaCorreta = string.Join(" ", result.blocosPalavras);
        return result;
    }

    private ExercicioCompletarFrase ToPhraseCompletionExercise()
    {
        string answer = phraseCompletion == null ? FirstExpectedAnswer() : phraseCompletion.respostaCorreta;
        var options = new List<string>();
        foreach (ExerciseApiAlternativeDTO item in OrderedAlternatives())
            options.Add(item.description);
        var answers = phraseCompletion?.respostasCorretas ?? new List<string> { answer };
        foreach (string correct in answers)
        {
            if (!string.IsNullOrWhiteSpace(correct) &&
                !options.Exists(option => string.Equals(option?.Trim(), correct.Trim(), StringComparison.OrdinalIgnoreCase)))
                options.Add(correct);
        }

        return new ExercicioCompletarFrase
        {
            fraseAntes = phraseCompletion == null ? string.Empty : phraseCompletion.fraseAntes,
            fraseDepois = phraseCompletion == null ? string.Empty : phraseCompletion.fraseDepois,
            respostaCorreta = answer,
            opcoes = options,
            partesFrase = phraseCompletion?.partesFrase,
            respostasCorretas = answers
        };
    }

    private ExercicioAssociacao ToAssociationExercise()
    {
        var result = new ExercicioAssociacao
        {
            opcoesPort = new List<string>(),
            opcoesIngles = new List<string>(),
            associacaoCorreta = new List<ParAssociacao>()
        };
        if (associationPairs == null)
            return result;

        foreach (ExerciseApiAssociationPairDTO item in associationPairs)
        {
            if (item == null) continue;
            result.opcoesPort.Add(item.portugues);
            result.opcoesIngles.Add(item.ingles);
            result.associacaoCorreta.Add(new ParAssociacao
            {
                palavraPortugues = item.portugues,
                palavraIngles = item.ingles
            });
        }
        return result;
    }

    private List<ExerciseApiAlternativeDTO> OrderedAlternatives()
    {
        var result = alternatives == null
            ? new List<ExerciseApiAlternativeDTO>()
            : new List<ExerciseApiAlternativeDTO>(alternatives);
        result.Sort((left, right) =>
        {
            int leftOrder = left.ordem ?? left.id;
            int rightOrder = right.ordem ?? right.id;
            int compare = leftOrder.CompareTo(rightOrder);
            return compare != 0 ? compare : left.id.CompareTo(right.id);
        });
        return result;
    }

    private List<string> GetExpectedAnswerTexts()
    {
        var result = new List<string>();
        if (expectedAnswers != null)
            foreach (ExerciseApiExpectedAnswerDTO item in expectedAnswers)
                if (item != null && item.resposta != null) result.Add(item.resposta);
        return result;
    }

    private string FirstExpectedAnswer()
    {
        List<string> answers = GetExpectedAnswerTexts();
        return answers.Count == 0 ? string.Empty : answers[0];
    }

    private static bool IsOneOf(string value, params string[] names)
    {
        foreach (string name in names)
            if (string.Equals(value, name, StringComparison.OrdinalIgnoreCase)) return true;
        return false;
    }
}

[Serializable]
public class ExerciseApiListDTO
{
    public List<ExerciseApiDTO> data = new List<ExerciseApiDTO>();
}
