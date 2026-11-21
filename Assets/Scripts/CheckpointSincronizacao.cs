using System;
using Newtonsoft.Json.Linq;

internal static class CheckpointSincronizacao
{
    internal static CheckpointPendente Preparar(CheckpointDispositivo local)
    {
        if (!local.Alterado || local.Atual == null)
            return null;
        return local.Pendente ?? (local.Pendente = new CheckpointPendente
        {
            Id = Guid.NewGuid().ToString(), Base = local.Revisao, Estado = (JObject)local.Atual.DeepClone()
        });
    }

    internal static void Confirmar(CheckpointDispositivo local, CheckpointPendente envio, long revisao)
    {
        if (!ReferenceEquals(local.Pendente, envio) || revisao != envio.Base + 1 || local.Revisao != envio.Base)
            throw new InvalidOperationException("Recibo de checkpoint fora da revisão esperada.");
        local.Revisao = revisao;
        local.Alterado = !JToken.DeepEquals(local.Atual, envio.Estado);
        local.Pendente = null;
    }

    internal static bool Reconciliar(CheckpointDispositivo local, string vinculo, long revisao,
        long geracao, JObject remoto)
    {
        if (local.Vinculo != vinculo)
            return false;
        if (local.Pendente != null && revisao == local.Pendente.Base + 1
            && JToken.DeepEquals(remoto, local.Pendente.Estado))
            Confirmar(local, local.Pendente, revisao);
        if (local.Alterado)
            return local.Revisao == revisao && local.Geracao == geracao;
        if (revisao < local.Revisao)
            return false;
        local.Revisao = revisao;
        local.Atual = remoto;
        return true;
    }
}
