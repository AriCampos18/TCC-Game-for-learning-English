using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using TMPro;
using System;
using System.Text;
using System.Collections.Generic;

public class SpeakingUI : MonoBehaviour
{
    public Button botaoMicrofone;
    public GameObject modalLegendaNPC;
    public GameObject microfoneIcon;
    public TextMeshProUGUI[] textosOpcoesFala;
    private InteracaoNPC npcAtivo;
    public GameObject ondasSom;
    public bool gravando = false;
    private AudioClip clipGravado;
    public TextMeshProUGUI statusGravacao;
    public int frequenciaAmostragem = 16000;
    public int tempoMaximoGravacao = 10;

    public ModalErroSpeaking modalErroSpeaking;
    
    public ModalExercicio modalExercicio;
    public Button pularEx;
    
    // ✨ Novo/Modificado: Guardará os bytes prontos do áudio gravado
    private byte[] dadosAudioWav; 
    
    public Button botaoRepetirFalaNPC; 
    public TextMeshProUGUI textoFeedback;
    private string microfoneDispositivo = null;
    private int tentativesRestantes = 3;
    private ExercicioSpeaking dadosExercicioAtual;
    private bool bloqueado = false;

    void Start()
    {
        botaoMicrofone.onClick.AddListener(ToggleGravacao);
        if (pularEx != null && modalExercicio != null)
        {
            pularEx.onClick.AddListener(PularExercicio);
        }

        ondasSom.SetActive(false);
        statusGravacao.text = "Press the microphone to start recording.";

        if (Microphone.devices.Length == 0)
        {
            Debug.LogWarning("Nenhum microfone encontrado no sistema!");
            botaoMicrofone.interactable = false;
            statusGravacao.text = "Microfone não disponível. Verifique as configurações.";
            return;
        }

        // Prioriza o microfone físico do PC
        microfoneDispositivo = null;

        foreach (string device in Microphone.devices)
        {
            Debug.Log("Microfone detectado: " + device);

            if (device.Contains("Realtek", StringComparison.OrdinalIgnoreCase))
            {
                microfoneDispositivo = device;
                break;
            }
        }

        if (string.IsNullOrEmpty(microfoneDispositivo))
        {
            microfoneDispositivo = Microphone.devices[0];
        }

        Debug.Log("Microfone selecionado: " + microfoneDispositivo);
    }

    private string NormalizarPalavra(string palavra)
    {
        return palavra
            .ToLower()
            .Replace(".", "")
            .Replace(",", "")
            .Replace("?", "")
            .Replace("!", "")
            .Replace(":", "")
            .Replace(";", "")
            .Replace("\n", "")
            .Replace("\r", "")
            .Trim();
    }

    public void MostrarFeedbackPronuncia(List<string> palavrasErradas, bool passou)
    {
        if (dadosExercicioAtual == null || textoFeedback == null) return;

        string fraseCorreta = dadosExercicioAtual.opcoesFala[dadosExercicioAtual.respostaCorreta]
            .Replace("\n", "")
            .Replace("\r", "")
            .Trim();

        string[] palavras = fraseCorreta.Split(' ');

        List<string> erradasNormalizadas = new List<string>();

        if (palavrasErradas != null)
        {
            foreach (string p in palavrasErradas)
            {
                string limpa = NormalizarPalavra(p);

                if (!string.IsNullOrEmpty(limpa))
                    erradasNormalizadas.Add(limpa);
            }
        }

        StringBuilder boas = new StringBuilder();
        StringBuilder ruins = new StringBuilder();

        foreach (string palavra in palavras)
        {
            string limpa = palavra.Trim();
            string normal = NormalizarPalavra(limpa);

            if (string.IsNullOrEmpty(normal)) continue;

            if (erradasNormalizadas.Contains(normal))
                ruins.Append($"<color=#C62828><b>{limpa}</b></color> ");
            else
                boas.Append($"<color=#2E7D32><b>{limpa}</b></color> ");
        }

        StringBuilder feedbackFinal = new StringBuilder();

        if (passou)
            feedbackFinal.Append("<color=#2E7D32>Good job! Your answer was accepted.</color>");
        else
            feedbackFinal.Append("<color=#000000>The sentence is correct, but some words need more practice.</color>");

        if (boas.Length > 0)
        {
            feedbackFinal.Append("\n<size=80%><color=#2E7D32>Good words:</color></size> ");
            feedbackFinal.Append(boas);
        }

        if (ruins.Length > 0)
        {
            feedbackFinal.Append("\n<size=80%><color=#C62828>Words to practice:</color></size> ");
            feedbackFinal.Append(ruins);
        }

        textoFeedback.text = feedbackFinal.ToString();
    }

    public void InicializarExercicio(ExercicioSpeaking ex, InteracaoNPC npcQueChamou)
    {
        dadosExercicioAtual = ex;
        npcAtivo = npcQueChamou;
        tentativesRestantes = 3;
        dadosAudioWav = null;
        bloqueado = false;
        botaoMicrofone.interactable = true;
        statusGravacao.text = "Press the microphone to start recording.";
        if (textoFeedback != null) textoFeedback.text = "";

        // ✨ Se a sua legenda pequena estiver ligada de um exercício anterior, desativa ela aqui
        if (modalLegendaNPC != null)
        {
            modalLegendaNPC.SetActive(false);
        }

        if (botaoRepetirFalaNPC != null)
        {
            botaoRepetirFalaNPC.gameObject.SetActive(false);
        }

        if (pularEx != null)
            pularEx.interactable = true;

        for (int i = 0; i < textosOpcoesFala.Length; i++)
        {
            if (i < ex.opcoesFala.Count)
            {
                textosOpcoesFala[i].gameObject.SetActive(true);
                textosOpcoesFala[i].text = ex.opcoesFala[i];
            }
            else
            {
                textosOpcoesFala[i].gameObject.SetActive(false);
            }
        }
    }

    public void PrepararParaNivelamento()
    {
        if (gravando)
            CancelarGravacao();

        dadosAudioWav = null;
        bloqueado = false;

        if (string.IsNullOrEmpty(microfoneDispositivo))
        {
            string[] dispositivos = Microphone.devices;
            if (dispositivos.Length > 0)
                microfoneDispositivo = dispositivos[0];
        }

        if (textoFeedback != null)
            textoFeedback.text = "";

        if (statusGravacao != null)
            statusGravacao.text = "Press the microphone to start recording.";

        if (botaoMicrofone != null)
            botaoMicrofone.interactable = !string.IsNullOrEmpty(microfoneDispositivo);

        //if (pularEx != null)
            //pularEx.interactable = false;

        if (botaoRepetirFalaNPC != null)
            botaoRepetirFalaNPC.gameObject.SetActive(false);

        if (modalLegendaNPC != null)
            modalLegendaNPC.SetActive(false);

        if (textosOpcoesFala != null)
        {
            foreach (TextMeshProUGUI textoOpcao in textosOpcoesFala)
            {
                if (textoOpcao != null)
                    textoOpcao.gameObject.SetActive(false);
            }
        }
    }

    public byte[] ObterAudioWav()
    {
        return dadosAudioWav == null ? null : (byte[])dadosAudioWav.Clone();
    }

    public void CancelarGravacao()
    {
        if (gravando && !string.IsNullOrEmpty(microfoneDispositivo) &&
            Microphone.IsRecording(microfoneDispositivo))
        {
            Microphone.End(microfoneDispositivo);
        }

        gravando = false;
        dadosAudioWav = null;

        if (statusGravacao != null)
            statusGravacao.text = "Recording cancelled.";
        if (microfoneIcon != null)
            microfoneIcon.SetActive(true);
        if (ondasSom != null)
            ondasSom.SetActive(false);
        if (botaoMicrofone != null)
            botaoMicrofone.interactable = !string.IsNullOrEmpty(microfoneDispositivo);
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
            if (!bloqueado && botaoMicrofone != null)
                botaoMicrofone.interactable = true;
            return null;
        }

        if (botaoMicrofone != null)
            botaoMicrofone.interactable = false;

        statusGravacao.text = "Analyzing speech accuracy...";

        // 1. Limpa e empacota a lista de strings em um formato JSON nativo legível para o Python
        List<string> frasesLimpas = new List<string>();
        foreach (var frase in dadosExercicioAtual.opcoesFala)
        {
            frasesLimpas.Add(frase.Replace("\n", "").Replace("\r", "").Trim());
        }
        
        // Cria a string JSON manual ["frase1", "frase2", "frase3"]
        string jsonFrases = "[\"" + string.Join("\",\"", frasesLimpas) + "\"]";

        // 2. Chama o gerenciador de backend para fazer a requisição pesada
        BackendManager backendManager = new BackendManager();
        string jsonResposta = await backendManager.EnviarAudioWhisperFast(dadosAudioWav, jsonFrases);

        // 3. Processa a string de resposta que veio do backend
        if (!string.IsNullOrEmpty(jsonResposta))
        {
            Debug.Log($"Resposta do Servidor recebida via BackendManager: {jsonResposta}");
            
            // Conversão direta de JSON string para o objeto C# (mantendo o JsonUtility nativo da sua UI)
            SpeakingResult resultado = JsonUtility.FromJson<SpeakingResult>(jsonResposta);
            if (!bloqueado && botaoMicrofone != null)
                botaoMicrofone.interactable = true;
            return resultado;
        }
        else
        {
            statusGravacao.text = "<color=red>Server connection error.</color>";
            if (!bloqueado && botaoMicrofone != null)
                botaoMicrofone.interactable = true;
            return null;
        }
    }
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

        if (textoFeedback != null)
            textoFeedback.text = sb.ToString();
    }

    // Mantido o feedback padrão de sucesso para quando o Modal passar direto
    public void MostrarSucessoNativo(float acuracia)
    {
        statusGravacao.text = "<color=#2E7D32>Correct answer.</color>";

        if (textoFeedback != null)
            textoFeedback.text = "Great pronunciation!";
    }
    void ToggleGravacao()
    {
        if (bloqueado) return;

        if (!gravando) IniciarGravacao();
        else PararGravacao();
    }

    public void BloquearSpeaking()
    {
        bloqueado = true;

        if (gravando)
        {
            PararGravacao();
        }

        if (botaoMicrofone != null)
            botaoMicrofone.interactable = false;

        if (ondasSom != null)
            ondasSom.SetActive(false);

        if (microfoneIcon != null)
            microfoneIcon.SetActive(true);
    }

    public void LiberarSpeaking()
    {
        bloqueado = false;

        if (botaoMicrofone != null)
            botaoMicrofone.interactable = true;
    }

    public void MostrarFeedbackFimTentativas()
    {
        if (textoFeedback != null)
        {
            textoFeedback.text =
                "You have used all your chances. You can try again later in the revision section.";
        }

        if (statusGravacao != null)
        {
            statusGravacao.text = "No attempts remaining.";
        }
    }

    void IniciarGravacao()
    {
        dadosAudioWav = null;
        clipGravado = null;

        if (string.IsNullOrEmpty(microfoneDispositivo))
        {
            Debug.LogError("Não há um microfone disponível para iniciar a gravação.");
            if (statusGravacao != null)
                statusGravacao.text = "Microphone not available.";
            return;
        }

        clipGravado = Microphone.Start(microfoneDispositivo, false, tempoMaximoGravacao, frequenciaAmostragem);
        if (clipGravado == null)
        {
            Debug.LogError("O Unity não conseguiu iniciar a gravação do microfone.");
            if (statusGravacao != null)
                statusGravacao.text = "Could not start recording.";
            return;
        }

        gravando = true;
        if (microfoneIcon != null)
            microfoneIcon.SetActive(false);
        if (ondasSom != null)
            ondasSom.SetActive(true);
        if (statusGravacao != null)
            statusGravacao.text = "Recording...";

        if(textoFeedback != null) textoFeedback.text = "";
    }

    void PararGravacao()
    {
        gravando = false;
        if (microfoneIcon != null)
            microfoneIcon.SetActive(true);
        if (ondasSom != null)
            ondasSom.SetActive(false);

        int framesGravados = 0;
        if (!string.IsNullOrEmpty(microfoneDispositivo) &&
            Microphone.IsRecording(microfoneDispositivo))
        {
            framesGravados = Microphone.GetPosition(microfoneDispositivo);
            Microphone.End(microfoneDispositivo);
        }
        else if (clipGravado != null)
        {
            framesGravados = clipGravado.samples;
        }

        if (clipGravado != null)
        {
            if (statusGravacao != null)
                statusGravacao.text = "Processing audio file...";
            if (framesGravados <= 0)
            {
                dadosAudioWav = null;
                Debug.LogError("A gravação terminou sem amostras de áudio.");
                if (statusGravacao != null)
                    statusGravacao.text = "No audio was recorded. Please try again.";
                return;
            }

            if (!AudioTemSomSuficiente(clipGravado, framesGravados))
            {
                dadosAudioWav = null;

                if (statusGravacao != null)
                    statusGravacao.text = "No speech detected. Please try again.";

                if (modalErroSpeaking != null)
                    modalErroSpeaking.Abrir();
                else
                    Debug.LogError("Configure o ModalErroSpeaking no SpeakingUI.");

                if (botaoMicrofone != null)
                    botaoMicrofone.interactable = false;

                return;
            }

            dadosAudioWav = ConvertAudioClipToWav(clipGravado, framesGravados);

            if (statusGravacao != null)
                statusGravacao.text = "Audio ready! Press 'Confirm' to submit.";
            if (textoFeedback != null)
                textoFeedback.text = "<i>Audio recorded. Click the confirmation button to evaluate.</i>";
        }
        else
        {
            Debug.LogError("Não há áudio gravado para processar.");
        }
    }

    private bool AudioTemSomSuficiente(AudioClip clip, int framesGravados)
    {
        if (clip == null || framesGravados <= 0)
            return false;

        int quantidade = Mathf.Min(framesGravados, clip.samples)
                        * clip.channels;

        if (quantidade <= 0)
            return false;

        float[] amostras = new float[quantidade];

        if (!clip.GetData(amostras, 0))
            return false;

        double somaQuadrados = 0;

        foreach (float amostra in amostras)
            somaQuadrados += amostra * amostra;

        double rms = System.Math.Sqrt(somaQuadrados / quantidade);

        // Limite inicial para detectar silêncio ou volume muito baixo.
        return rms >= 0.01;
    }


    private void PularExercicio()
    {
        Debug.Log("BOTAO FOI CLICADO PORRRA!!!!!!!");
        dadosAudioWav = null;
        modalExercicio.FinalizarExercicio();
    }

    // --- Métodos de Conversão WAV mantidos intactos ---
    private byte[] ConvertAudioClipToWav(AudioClip clip, int framesGravados)
    {
        int sampleCount = Mathf.Min(framesGravados, clip.samples) * clip.channels;
        float[] samples = new float[sampleCount];
        clip.GetData(samples, 0);
        return ConvertFloatsToWav(samples, clip.frequency, clip.channels);
    }

    private byte[] ConvertFloatsToWav(float[] samples, int frequency, int channels)
    {
        const int bitsPerSample = 16;
        const int headerSize = 44;
        int dataSize = samples.Length * sizeof(short);
        byte[] wav = new byte[headerSize + dataSize];

        Encoding.ASCII.GetBytes("RIFF").CopyTo(wav, 0);
        BitConverter.GetBytes(36 + dataSize).CopyTo(wav, 4);
        Encoding.ASCII.GetBytes("WAVE").CopyTo(wav, 8);
        Encoding.ASCII.GetBytes("fmt ").CopyTo(wav, 12);
        BitConverter.GetBytes(16).CopyTo(wav, 16);
        BitConverter.GetBytes((short)1).CopyTo(wav, 20);
        BitConverter.GetBytes((short)channels).CopyTo(wav, 22);
        BitConverter.GetBytes(frequency).CopyTo(wav, 24);
        BitConverter.GetBytes(frequency * channels * bitsPerSample / 8).CopyTo(wav, 28);
        BitConverter.GetBytes((short)(channels * bitsPerSample / 8)).CopyTo(wav, 32);
        BitConverter.GetBytes((short)bitsPerSample).CopyTo(wav, 34);
        Encoding.ASCII.GetBytes("data").CopyTo(wav, 36);
        BitConverter.GetBytes(dataSize).CopyTo(wav, 40);

        short[] shortSamples = new short[samples.Length];
        for (int i = 0; i < samples.Length; i++)
        {
            shortSamples[i] = (short)(Mathf.Clamp(samples[i], -1f, 1f) * short.MaxValue);
        }

        Buffer.BlockCopy(shortSamples, 0, wav, headerSize, dataSize);
        return wav;
    }

    public void TentarNovamente()
    {
        CancelarGravacao();

        clipGravado = null;
        dadosAudioWav = null;

        if (textoFeedback != null)
            textoFeedback.text = "";

        if (statusGravacao != null)
            statusGravacao.text = "Press the microphone to start recording.";

        if (!bloqueado && botaoMicrofone != null)
            botaoMicrofone.interactable =
                !string.IsNullOrEmpty(microfoneDispositivo);
    }
}