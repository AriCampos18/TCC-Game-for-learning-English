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
            // 2. INCLUÍDO O PARÂMETRO 'npc' NO OBJETO ANÔNIMO
            // Isso gera exatamente o JSON esperado pelo Flask: {"texto": "...", "npc": "..."}
            var dados = new { texto = texto, npc = npcId };
            string json = JsonConvert.SerializeObject(dados);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            Debug.Log($"Enviando requisição para o servidor... NPC: {npcId} | Texto: {texto}");
            var response = await client.PostAsync("http://localhost:5000/coquiGlow", content);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            else
            {
                Debug.LogError($"Erro no Servidor: {response.StatusCode}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Erro de Conexão: {ex.Message}");
            return null;
        }
    }

    public async Task<string> GerarTextoSTT(byte[] audioBytes)
    {
        try
        {
            var content = new ByteArrayContent(audioBytes);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("audio/wav");
            Debug.Log("Enviando áudio para o servidor...");

            var response = await client.PostAsync("http://localhost:5000/whisperFast", content);

            if(!response.IsSuccessStatusCode)
            {
                Debug.LogWarning($"Whisper Fast falhou (Status: {response.StatusCode}). Tentando Vosk...");
                response = await client.PostAsync("http://localhost:5000/vosk", content);
            }

            if (response.IsSuccessStatusCode)
            {
                string resultado = await response.Content.ReadAsStringAsync();
                Debug.Log($"Transcrição: {resultado}");
                return resultado;
            }
            else
            {
                Debug.LogError($"Erro no Servidor: {response.StatusCode}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Erro de Conexão: {ex.Message}");
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
}