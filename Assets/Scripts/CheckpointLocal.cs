using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

internal sealed class CheckpointPendente
{
    public string Id;
    public long Base;
    public JObject Estado;
}

internal sealed class CheckpointDispositivo
{
    public int Conta;
    public string Ambiente;
    public string Vinculo;
    public long Revisao;
    public long Geracao;
    public JObject Atual;
    public JObject MarcoAnterior;
    public bool Alterado;
    public CheckpointPendente Pendente;
}

internal sealed class CheckpointLocalObsoletoException : IOException
{
    internal CheckpointLocalObsoletoException() : base("Outro processo já gravou um checkpoint mais recente.") { }
}

internal sealed class CheckpointLocal
{
    private readonly string caminho;
    private readonly int conta;
    private readonly string ambiente;
    private static readonly object Acesso = new object();
    private static readonly Dictionary<string, long> UltimasGravacoes = new Dictionary<string, long>();
    private static long sequenciaGlobal;

    internal static long ProximaSequencia() => Interlocked.Increment(ref sequenciaGlobal);

    internal CheckpointLocal(string raiz, int conta, string ambiente)
    {
        this.conta = conta;
        this.ambiente = ambiente;
        caminho = Path.Combine(raiz, "checkpoints-v1", Hash(ambiente), $"{conta}.json");
    }

    internal CheckpointDispositivo Ler() => Ler(caminho) ?? Ler(caminho + ".bak");

    private CheckpointDispositivo Ler(string arquivo)
    {
        try
        {
            if (!File.Exists(arquivo) || new FileInfo(arquivo).Length > 4_000_000)
                return null;
            var envelope = JObject.Parse(File.ReadAllText(arquivo, Encoding.UTF8));
            string conteudo = (string)envelope["payload"];
            if ((int?)envelope["version"] != 1 || conteudo == null || Hash(conteudo) != (string)envelope["hash"])
                return null;
            var dado = JsonConvert.DeserializeObject<CheckpointDispositivo>(conteudo);
            return dado != null && dado.Conta == conta && dado.Ambiente == ambiente
                && dado.Vinculo != null && dado.Vinculo.Length == 64 && dado.Revisao >= 0 && dado.Geracao >= 0
                && (dado.Atual != null || !dado.Alterado)
                && (dado.Pendente == null || (dado.Alterado && dado.Pendente.Estado != null
                    && dado.Pendente.Base == dado.Revisao && Guid.TryParse(dado.Pendente.Id, out _)))
                ? dado : null;
        }
        catch (Exception e) when (e is IOException || e is UnauthorizedAccessException
            || e is JsonException || e is FormatException || e is InvalidCastException
            || e is ArgumentException || e is OverflowException || e is InvalidOperationException)
        {
            return null;
        }
    }

    internal void Gravar(string conteudo, long sequencia)
    {
        lock (Acesso)
        {
            if (UltimasGravacoes.TryGetValue(caminho, out long ultima) && sequencia <= ultima)
                return;
            Directory.CreateDirectory(Path.GetDirectoryName(caminho));
            using (var trava = new FileStream(caminho + ".lock", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            {
                var atual = Ler();
                var novo = JsonConvert.DeserializeObject<CheckpointDispositivo>(conteudo);
                if (atual != null && novo != null && atual.Vinculo == novo.Vinculo
                    && (novo.Geracao < atual.Geracao || novo.Revisao < atual.Revisao))
                {
                    if (novo.Alterado)
                        Arquivar(novo);
                    throw new CheckpointLocalObsoletoException();
                }

                Escrever(caminho, conteudo);
                UltimasGravacoes[caminho] = sequencia;
            }
        }
    }

    internal void Arquivar(CheckpointDispositivo dado) =>
        Escrever(caminho + ".conflict-" + Guid.NewGuid().ToString("N"), JsonConvert.SerializeObject(dado));

    private void Escrever(string destino, string conteudo)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(destino));
        string temporario = destino + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            var envelope = new JObject { ["version"] = 1, ["payload"] = conteudo, ["hash"] = Hash(conteudo) };
            byte[] bytes = Encoding.UTF8.GetBytes(envelope.ToString(Formatting.None));
            using (var stream = new FileStream(temporario, FileMode.CreateNew, FileAccess.Write))
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush(true);
            }
            if (File.Exists(destino))
                File.Replace(temporario, destino, Ler(destino) != null ? destino + ".bak" : null);
            else
                File.Move(temporario, destino);
        }
        finally
        {
            if (File.Exists(temporario))
                File.Delete(temporario);
        }
    }

    private static string Hash(string valor)
    {
        using (var sha = SHA256.Create())
            return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(valor))).Replace("-", "");
    }
}
