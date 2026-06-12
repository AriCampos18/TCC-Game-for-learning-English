using UnityEngine;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System;

public class BackendManager 
{
    HttpClient client = new HttpClient();

    // 1. ADICIONADO O PARÂMETRO 'npcId' NA ASSINATURA
    public async Task<byte[]> GerarAudio(string texto, string npcId)
    {
        try
        {
            var dados = new { texto = texto, npc = npcId };
            string json = JsonConvert.SerializeObject(dados);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            Debug.Log($"Enviando requisição para o servidor... NPC: {npcId} | Texto: {texto}");

            var response = await client.PostAsync("http://localhost:5000/coquiVits", content);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            else
            {
                string erro = await response.Content.ReadAsStringAsync();
                Debug.LogError($"Erro no Servidor: {response.StatusCode} | {erro}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Erro de Conexão: {ex.Message}");
            return null;
        }
    }

    public async Task<string> EnviarAudioWhisperFast(byte[] audioBytes, string jsonFrases)
    {
        try
        {
            // Cria o formulário MultiPart equivalente ao WWWForm do Unity
            using (var multipartForm = new MultipartFormDataContent())
            {
                // 1. Adiciona o arquivo de áudio em bytes
                var audioContent = new ByteArrayContent(audioBytes);
                audioContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("audio/wav");
                multipartForm.Add(audioContent, "audio", "audio_gravado.wav");

                // 2. Adiciona o campo de texto com a string JSON das frases
                var textoContent = new StringContent(jsonFrases, Encoding.UTF8);
                multipartForm.Add(textoContent, "textos");

                Debug.Log("Enviando áudio e opções de fala para o whisperFast via BackendManager...");
                var response = await client.PostAsync("http://localhost:5000/whisperFast", multipartForm);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    string erro = await response.Content.ReadAsStringAsync();
                    Debug.LogError($"Erro no Servidor WhisperFast: {response.StatusCode} | {erro}");
                    return null;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Erro de Conexão no WhisperFast: {ex.Message}");
            return null;
        }
    }

    public async Task<RetornoIANivelamento> ProcessarRespostasNivelamento(string json)
    {
        try
        {
            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync(
                "http://localhost:5000/avaliarNivel",
                content
            );

            string respostaJson =
                await response.Content.ReadAsStringAsync();

            Debug.Log("Resposta backend:");
            Debug.Log(respostaJson);

            RetornoIANivelamento retorno =
                JsonConvert.DeserializeObject<RetornoIANivelamento>(
                    respostaJson
                );

            return retorno;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return null;
        }
    }

    public async Task<string> TraduzirTextoDeepL(string textoOriginal)
    {
        try
        {
            var dado = new { texto = textoOriginal };
            string json = JsonConvert.SerializeObject(dado);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Faz o POST assíncrono para o seu app.py
            var response = await client.PostAsync("http://localhost:5000/traduzir", content);

            if (response.IsSuccessStatusCode)
            {
                string jsonResposta = await response.Content.ReadAsStringAsync();
                // Deserializa usando a classe utilitária abaixo
                RespostaDeepL dados = JsonConvert.DeserializeObject<RespostaDeepL>(jsonResposta);
                return dados.traduzido;
            }
            else
            {
                string erro = await response.Content.ReadAsStringAsync();
                Debug.LogError($"Erro no Servidor DeepL: {response.StatusCode} | {erro}");
                return "";
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Erro de Conexão ao tentar traduzir: {ex.Message}");
            return "";
        }
    }

    // Classe utilitária auxiliar externa ou interna na mesma folha
    private class RespostaDeepL
    {
        public string traduzido;
    }
}