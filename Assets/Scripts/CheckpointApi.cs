using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

internal sealed class CheckpointApi
{
    private static readonly HttpClient HttpCompartilhado = new HttpClient();
    private readonly HttpClient http;
    private readonly Uri url;
    private readonly SessionManager sessao;
    private readonly int conta;
    internal string Ambiente => url.AbsoluteUri;

    internal CheckpointApi(string backend, SessionManager sessao, int conta, HttpClient http = null)
    {
        url = new Uri(backend.TrimEnd('/') + "/", UriKind.Absolute);
        this.sessao = sessao;
        this.conta = conta;
        this.http = http ?? HttpCompartilhado;
    }

    internal Task<JObject> Estado(CancellationToken ct) =>
        Enviar("api/game/state?schema_version=1", null, ct);

    internal Task<JObject> Assumir(string escritor, string claim, long esperada, CancellationToken ct) =>
        Enviar("api/game/save", new JObject {
            ["schema_version"] = 1, ["operation"] = "claim", ["writer_id"] = escritor,
            ["claim_id"] = claim, ["expected_writer_epoch"] = esperada
        }, ct);

    internal Task<JObject> Salvar(string escritor, string vinculo, long geracao, CheckpointPendente pendente,
        CancellationToken ct) =>
        Enviar("api/game/save", new JObject {
            ["schema_version"] = 1, ["writer_id"] = escritor, ["checkpoint_binding"] = vinculo,
            ["writer_epoch"] = geracao, ["base_revision"] = pendente.Base, ["save_id"] = pendente.Id,
            ["checkpoint"] = pendente.Estado.DeepClone()
        }, ct);

    private async Task<JObject> Enviar(string rota, JObject corpo, CancellationToken ct)
    {
        using (var prazo = CancellationTokenSource.CreateLinkedTokenSource(ct))
        {
            prazo.CancelAfter(TimeSpan.FromSeconds(8));
            for (int tentativa = 0; tentativa < 2; tentativa++)
            {
                if (sessao.AuthenticatedUserId != conta)
                    throw new OperationCanceledException("A conta autenticada mudou.");
                if (tentativa > 0 || sessao.AccessToken == null)
                {
                    var refresh = await sessao.RefreshAsync(prazo.Token);
                    if (!refresh.IsSuccess && (refresh.Error?.Type == AuthenticationFailureType.Connection
                        || refresh.Error?.Type == AuthenticationFailureType.Server))
                        throw new HttpRequestException("Não foi possível renovar a sessão do checkpoint.");
                    if (!refresh.IsSuccess || sessao.AuthenticatedUserId != conta)
                        throw new CheckpointApiException(401, "checkpoint_auth");
                }
                using (var request = new HttpRequestMessage(corpo == null ? HttpMethod.Get : HttpMethod.Post, new Uri(url, rota)))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", sessao.AccessToken);
                    if (corpo != null)
                        request.Content = new StringContent(corpo.ToString(Formatting.None), Encoding.UTF8, "application/json");
                    using (var resposta = await http.SendAsync(request, prazo.Token))
                    {
                        if ((int)resposta.StatusCode == 401 && tentativa == 0)
                            continue;
                        string json = await resposta.Content.ReadAsStringAsync();
                        prazo.Token.ThrowIfCancellationRequested();
                        if (sessao.AuthenticatedUserId != conta)
                            throw new OperationCanceledException("A resposta pertence a outra conta.");

                        if (!resposta.IsSuccessStatusCode)
                        {
                            string codigo = null;
                            try { codigo = (string)JObject.Parse(json)["error"]?["code"]; }
                            catch (JsonException) { }
                            throw new CheckpointApiException((int)resposta.StatusCode, codigo);
                        }
                        return JObject.Parse(json)["data"] as JObject
                            ?? throw new JsonException("Resposta de checkpoint sem data.");
                    }
                }
            }
        }
        throw new CheckpointApiException(401, "checkpoint_auth");
    }
}

internal sealed class CheckpointApiException : Exception
{
    internal readonly int Status;
    internal readonly string Codigo;

    internal CheckpointApiException(int status, string codigo) : base($"Checkpoint HTTP {status} ({codigo}).")
    {
        Status = status;
        Codigo = codigo;
    }
}
