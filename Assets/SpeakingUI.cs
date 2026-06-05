using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using TMPro;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine.Networking;

public class SpeakingUI : MonoBehaviour
{
    public Button botaoMicrofone;
    public GameObject microfoneIcon;
    public GameObject ondasSom;
    public bool gravando = false;
    private AudioClip clipGravado;
    public TextMeshProUGUI statusGravacao, audioTranscricao;
    public int frequenciaAmostragem = 16000;
    public int tempoMaximoGravacao = 10;
    
    public ModalExercicio modalExercicio;
    public Button pularEx;
    
    // ✨ Novo/Modificado: Guardará os bytes prontos do áudio gravado
    private byte[] dadosAudioWav; 
    
    public Button botaoRepetirFalaNPC; 
    private int tentativesRestantes = 3;
    private ExercicioSpeaking dadosExercicioAtual;

    void Start()
    {
        botaoMicrofone.onClick.AddListener(ToggleGravacao);
        pularEx.onClick.AddListener(PularExercicio);
        
        if (botaoRepetirFalaNPC != null)
        {
            botaoRepetirFalaNPC.onClick.AddListener(RepetirFalaDoNPC);
            botaoRepetirFalaNPC.gameObject.SetActive(false); 
        }

        ondasSom.SetActive(false);
        statusGravacao.text = "Press the microphone to start recording.";
        audioTranscricao.text = "";

        if (Microphone.devices.Length == 0)
        {
            Debug.LogWarning("Nenhum microfone encontrado!");
            botaoMicrofone.interactable = false;
            statusGravacao.text = "Microfone não disponível. Verifique as configurações.";
        }
    }

    public void InicializarExercicio(ExercicioSpeaking ex)
    {
        dadosExercicioAtual = ex;
        tentativesRestantes = 3;
        dadosAudioWav = null;
        audioTranscricao.text = "";
        statusGravacao.text = "Press the microphone to start recording.";
        
        if (botaoRepetirFalaNPC != null)
            botaoRepetirFalaNPC.gameObject.SetActive(false);
    }

    public int ObterIndiceCorreto() => dadosExercicioAtual.respostaCorreta;
    public int ObterTentativasRestantes() => tentativesRestantes;
    public void ReduzirTentativa() => tentativesRestantes--;

    // ✨ ESSA É A FUNÇÃO PRINCIPAL que o ModalExercicio vai chamar agora usando 'await'
    public async Task<SpeakingResult> VerificarRespostaWhisper()
    {
        if (dadosExercicioAtual == null || dadosAudioWav == null || dadosAudioWav.Length == 0)
        {
            statusGravacao.text = "Please record your answer first!";
            return null;
        }

        statusGravacao.text = "Analyzing speech accuracy...";

        // 1. Cria o formulário MultiPart HTTP (Igual ao Postman/HTML form)
        WWWForm form = new WWWForm();
        form.AddBinaryData("audio", dadosAudioWav, "audio_gravado.wav", "audio/wav");

        // 2. Limpa e empacota a lista de strings em um formato JSON nativo legível para o Python
        List<string> frasesLimpas = new List<string>();
        foreach (var frase in dadosExercicioAtual.opcoesFala)
        {
            frasesLimpas.Add(frase.Replace("\n", "").Replace("\r", "").Trim());
        }
        
        // Cria a string JSON manual ["frase1", "frase2", "frase3"]
        string jsonFrases = "[\"" + string.Join("\",\"", frasesLimpas) + "\"]";
        form.AddField("textos", jsonFrases);

        // 3. Envia os dados para a API Flask
        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost:5000/whisperFast", form))
        {
            var operacao = www.SendWebRequest();

            while (!operacao.isDone)
                await Task.Yield();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonResposta = www.downloadHandler.text;
                Debug.Log($"Resposta do Servidor: {jsonResposta}");
                
                // Conversão direta de JSON string para o objeto C#
                SpeakingResult resultado = JsonUtility.FromJson<SpeakingResult>(jsonResposta);
                return resultado;
            }
            else
            {
                Debug.LogError("Erro de rede/servidor ao enviar áudio: " + www.error);
                statusGravacao.text = "<color=red>Server connection error.</color>";
                return null;
            }
        }
    }

    // ✨ Método modificado para atualizar os textos de feedback com as cores RichText nas palavras erradas
    public void AtualizarTextoFeedback(string mensagemBase, List<string> palavrasErradas)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(mensagemBase);

        if (palavrasErradas != null && palavrasErradas.Count > 0)
        {
            sb.AppendLine("\n<size=85%><color=#FFA500>Words that need adjustment:</color></size>");
            sb.Append("\" ");
            foreach (string palavra in palavrasErradas)
            {
                string limpa = palavra.Replace("\n", "").Replace("\r", "").Trim();
                if (!string.IsNullOrEmpty(limpa))
                {
                    // Tags de RichText do TextMeshPro aplicam a cor vermelha em negrito
                    sb.Append($"<color=red><b>{limpa}</b></color> ");
                }
            }
            sb.Append("\"");
        }

        audioTranscricao.text = sb.ToString();
    }

    // Mantido o feedback padrão de sucesso para quando o Modal passar direto
    public void MostrarSucessoNativo(float acuracia)
    {
        statusGravacao.text = "<color=green>Perfect! Correct answer.</color>";
        audioTranscricao.text = $"Great pronunciation! Accuracy: {acuracia}%";
    }

    private async void RepetirFalaDoNPC()
    {
        // Alterado de 'ShelfNPC' para 'BakeryNPC' já que este é o script da padaria!
        BakeryNPC npc = FindObjectOfType<BakeryNPC>();
        if (npc != null)
        {
            botaoRepetirFalaNPC.interactable = false;
            statusGravacao.text = "Listening to the NPC...";
            
            await npc.FalarFraseCustomizada(npc.ultimaFraseDita);
            
            statusGravacao.text = "Try recording your response now!";
            botaoRepetirFalaNPC.interactable = true;
        }
    }

    void ToggleGravacao()
    {
        if (!gravando) IniciarGravacao();
        else PararGravacao();
    }

    void IniciarGravacao()
    {
        gravando = true;
        microfoneIcon.SetActive(false);
        ondasSom.SetActive(true);
        statusGravacao.text = "Recording...";
        audioTranscricao.text = "";

        clipGravado = Microphone.Start(null, false, tempoMaximoGravacao, frequenciaAmostragem);
    }

    void PararGravacao()
    {
        gravando = false;
        microfoneIcon.SetActive(true);
        ondasSom.SetActive(false);

        if (Microphone.IsRecording(null)) Microphone.End(null);

        if (clipGravado != null)
        {
            statusGravacao.text = "Processing audio file...";
            // Converte os dados gravados da memória do Unity diretamente para o array de bytes em formato WAV
            dadosAudioWav = ConvertAudioClipToWav(clipGravado);
            statusGravacao.text = "Audio ready! Press 'Confirm' to submit.";
            audioTranscricao.text = "<i>Audio recorded. Click the confirmation button to evaluate.</i>";
        }
    }

    private void PularExercicio()
    {
        dadosAudioWav = null;
        modalExercicio.FinalizarExercicio();
    }

    // --- Métodos de Conversão WAV mantidos intactos ---
    byte[] ConvertAudioClipToWav(AudioClip clip) { float[] samples = new float[clip.samples]; clip.GetData(samples, 0); return ConvertFloatsToWav(samples, clip.frequency); }
    private byte[] ConvertFloatsToWav(float[] samples, int frequency) { 
        int channels = 1; int bitsPerSample = 16; int dataSize = samples.Length * 2; int headerSize = 44; byte[] wav = new byte[headerSize + dataSize]; System.Text.Encoding.ASCII.GetBytes("RIFF").CopyTo(wav, 0); BitConverter.GetBytes(36 + dataSize).CopyTo(wav, 4); System.Text.Encoding.ASCII.GetBytes("WAVE").CopyTo(wav, 8); System.Text.Encoding.ASCII.GetBytes("fmt ").CopyTo(wav, 12); BitConverter.GetBytes(16).CopyTo(wav, 16); BitConverter.GetBytes((short)1).CopyTo(wav, 20); BitConverter.GetBytes((short)channels).CopyTo(wav, 22); BitConverter.GetBytes(frequency).CopyTo(wav, 24); BitConverter.GetBytes(frequency * channels * bitsPerSample / 8).CopyTo(wav, 28); BitConverter.GetBytes((short)(channels * bitsPerSample / 8)).CopyTo(wav, 32); BitConverter.GetBytes((short)bitsPerSample).CopyTo(wav, 34); System.Text.Encoding.ASCII.GetBytes("data").CopyTo(wav, 36); BitConverter.GetBytes(dataSize).CopyTo(wav, 40); short[] shortSamples = new short[samples.Length]; for (int i = 0; i < samples.Length; i++) { shortSamples[i] = (short)(samples[i] * 32767f); } Buffer.BlockCopy(shortSamples, 0, wav, 44, dataSize); return wav; }
}