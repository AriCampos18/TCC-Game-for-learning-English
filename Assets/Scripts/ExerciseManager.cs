using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public sealed class ExerciseCatalogItem
{
    public TipoExercicio Tipo { get; private set; }
    public ExercicioBase Exercise { get; private set; }

    public ExerciseCatalogItem(TipoExercicio type, ExercicioBase exercise)
    {
        Tipo = type;
        Exercise = exercise;
    }
}

public sealed class ExerciseLoadResult
{
    public bool Success { get; private set; }
    public string Error { get; private set; }
    public int? HttpStatus { get; private set; }
    public Dictionary<string, List<ExerciseCatalogItem>> ByPoint { get; private set; }
    public List<string> PointOrder { get; private set; }
    public int Count { get; private set; }

    private ExerciseLoadResult(bool success, string error, int? httpStatus,
        Dictionary<string, List<ExerciseCatalogItem>> byPoint, int count)
    {
        Success = success;
        Error = error;
        HttpStatus = httpStatus;
        ByPoint = byPoint ?? new Dictionary<string, List<ExerciseCatalogItem>>();
        Count = count;
        PointOrder = new List<string>(ByPoint.Keys);
        PointOrder.Sort(StringComparer.Ordinal);
    }

    public static ExerciseLoadResult Failed(string error, int? status = null)
    {
        return new ExerciseLoadResult(false, error, status, null, 0);
    }

    public static ExerciseLoadResult Loaded(Dictionary<string, List<ExerciseCatalogItem>> byPoint)
    {
        int count = 0;
        foreach (List<ExerciseCatalogItem> items in byPoint.Values) count += items.Count;
        return new ExerciseLoadResult(true, null, null, byPoint, count);
    }

    public ExerciseCatalogItem Find(string pointKey, string level, int order,
        TipoExercicio expectedType, bool? expectedSpeakingFreeMode = null)
    {
        List<ExerciseCatalogItem> items;
        if (ByPoint == null || !ByPoint.TryGetValue(pointKey ?? string.Empty, out items)) return null;
        foreach (ExerciseCatalogItem item in items)
        {
            if (item == null || item.Exercise == null || item.Exercise.idExercise <= 0)
                continue;
            if (item.Exercise.nivelBanco != level || item.Exercise.ordemBanco != order)
                continue;
            if (item.Tipo != expectedType)
                continue;

            if (expectedType == TipoExercicio.Speaking && expectedSpeakingFreeMode.HasValue)
            {
                ExercicioSpeaking speaking = item.Exercise as ExercicioSpeaking;
                if (speaking == null || speaking.modoLivre != expectedSpeakingFreeMode.Value)
                    continue;
            }
            return item;
        }
        return null;
    }
}

public sealed class ExerciseManager : MonoBehaviour
{
    private static ExerciseManager instance;
    public static ExerciseManager Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<ExerciseManager>();
            if (instance == null)
            {
                GameObject service = new GameObject("ExerciseManager");
                instance = service.AddComponent<ExerciseManager>();
            }
            return instance;
        }
    }

    [SerializeField] private AuthenticationUIController authenticationController;
    private HttpClient httpClient;
    private readonly Dictionary<string, ExerciseLoadResult> cache =
        new Dictionary<string, ExerciseLoadResult>();
    private readonly Dictionary<string, Task<ExerciseLoadResult>> inFlight =
        new Dictionary<string, Task<ExerciseLoadResult>>();
    private readonly object cacheLock = new object();
    private int visitaCache;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        ResolveAuthenticationController();
        httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(20) };
    }

    private void OnDestroy()
    {
        if (instance == this) instance = null;
        if (httpClient != null) httpClient.Dispose();
    }

    public Task<ExerciseLoadResult> LoadForNpcAsync(string npcKey, string level,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        return LoadAsync(npcKey, null, level, cancellationToken);
    }

    public Task<ExerciseLoadResult> LoadForPointAsync(string npcKey, string pointKey, string level,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        return LoadAsync(npcKey, pointKey, level, cancellationToken);
    }

    private async Task<ExerciseLoadResult> LoadAsync(string npcKey, string pointKey, string level,
        CancellationToken cancellationToken)
    {
        var sessao = SessaoCenario.Atual;
        string cenario = SessaoCenario.CenarioDoNpc(npcKey);
        int visita = sessao.Versao;
        if (!sessao.Permite(cenario, visita) || level != sessao.Nivel)
            throw new OperationCanceledException("Consulta de exercícios fora da visita ativa.");

        cancellationToken.ThrowIfCancellationRequested();
        ResolveAuthenticationController();
        string url = authenticationController != null ? authenticationController.BackendBaseUrl : null;
        var carregamento = LoadCatalogAsync(visita, url, SessaoCenario.Id(cenario), npcKey, pointKey, level);
        var resultado = await AguardarCatalogoAsync(carregamento, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        if (!sessao.Permite(cenario, visita))
            throw new OperationCanceledException("A visita mudou durante a consulta de exercícios.");

        return resultado;
    }

    private static async Task<ExerciseLoadResult> AguardarCatalogoAsync(
        Task<ExerciseLoadResult> carregamento, CancellationToken cancellationToken)
    {
        if (cancellationToken.CanBeCanceled && !carregamento.IsCompleted)
        {
            var cancelado = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            using (cancellationToken.Register(() => cancelado.TrySetResult(true)))
                await Task.WhenAny(carregamento, cancelado.Task);

            cancellationToken.ThrowIfCancellationRequested();
        }

        return await carregamento;
    }

    private Task<ExerciseLoadResult> LoadCatalogAsync(int visita, string url, int scenarioId, string npcKey, string pointKey, string level)
    {
        if (string.IsNullOrWhiteSpace(npcKey))
            return Task.FromResult(ExerciseLoadResult.Failed("npc_key não pode ser vazio."));
        if (string.IsNullOrWhiteSpace(level))
            return Task.FromResult(ExerciseLoadResult.Failed("nivel não pode ser vazio."));

        string cacheKey = visita + "|" + url + "|" + scenarioId + "|" + npcKey + "|" + (pointKey ?? "*") + "|" + level;
        lock (cacheLock)
        {
            if (visitaCache != visita)
            {
                cache.Clear();
                visitaCache = visita;
            }

            ExerciseLoadResult cached;
            if (cache.TryGetValue(cacheKey, out cached)) return Task.FromResult(cached);
            Task<ExerciseLoadResult> pending;
            if (inFlight.TryGetValue(cacheKey, out pending)) return pending;
            var completion = new TaskCompletionSource<ExerciseLoadResult>();
            pending = completion.Task;
            inFlight[cacheKey] = pending;
            CompleteLoadAsync(cacheKey, visita, url, scenarioId, npcKey, pointKey, level, completion);
            return pending;
        }
    }

    private async void CompleteLoadAsync(string cacheKey, int visita, string url, int scenarioId, string npcKey,
        string pointKey, string level,
        TaskCompletionSource<ExerciseLoadResult> completion)
    {
        ExerciseLoadResult result;
        try
        {
            result = await FetchAsync(url, scenarioId, npcKey, pointKey, level, CancellationToken.None);
        }
        catch (OperationCanceledException)
        {
            result = ExerciseLoadResult.Failed("A consulta de exercícios foi cancelada.");
        }
        catch (Exception exception)
        {
            result = ExerciseLoadResult.Failed("Falha ao carregar exercícios: " + exception.Message);
        }

        lock (cacheLock)
        {
            inFlight.Remove(cacheKey);
            if (result.Success && visitaCache == visita && SessaoCenario.Atual.Corresponde(visita))
                cache[cacheKey] = result;
        }
        completion.SetResult(result);
    }

    internal async Task<ExerciseLoadResult> LerCatalogoAdministrativoAsync(int cenario, string npc, string nivel,
        CancellationToken token)
    {
        ResolveAuthenticationController();
        var auth = authenticationController;
        var sessao = auth != null ? auth.Session : null;
        if (sessao?.IsAuthenticated != true || !sessao.IsAdmin)
            return ExerciseLoadResult.Failed("Acesso administrativo necessário.");
        if (SessaoCenario.Id(SessaoCenario.CenarioDoNpc(npc)) != cenario || cenario == 0)
            return ExerciseLoadResult.Failed("O NPC não pertence ao cenário selecionado.");
        int? conta = sessao.AuthenticatedUserId;
        var resultado = await FetchAsync(auth.BackendBaseUrl, cenario, npc, null, nivel, token);
        token.ThrowIfCancellationRequested();
        return sessao.IsAuthenticated && sessao.IsAdmin && sessao.AuthenticatedUserId == conta
            ? resultado : ExerciseLoadResult.Failed("A sessão administrativa mudou.");
    }

    internal async Task<CatalogoMissoes.Resposta> LerMissoesAdministrativasAsync(int cenario, string nivel,
        CancellationToken token)
    {
        ResolveAuthenticationController();
        var auth = authenticationController;
        var sessao = auth != null ? auth.Session : null;
        if (sessao?.IsAuthenticated != true || !sessao.IsAdmin)
            return null;
        var conta = sessao.AuthenticatedUserId;
        var baseUri = new Uri(auth.BackendBaseUrl.TrimEnd('/') + "/");
        var destino = new Uri(baseUri, "api/missions?id_cenario=" + cenario + "&nivel=" + Uri.EscapeDataString(nivel));
        using var req = new HttpRequestMessage(HttpMethod.Get, destino);
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", sessao.AccessToken);
        using var resposta = await httpClient.SendAsync(req, token);
        if (!resposta.IsSuccessStatusCode || sessao.AuthenticatedUserId != conta || !sessao.IsAdmin)
            return null;
        string corpo = await resposta.Content.ReadAsStringAsync();
        token.ThrowIfCancellationRequested();
        return JsonConvert.DeserializeObject<CatalogoMissoes.Resposta>(corpo);
    }

    internal async Task<Newtonsoft.Json.Linq.JObject> SugerirFalaAdministrativaAsync(object contexto,
        CancellationToken token)
    {
        ResolveAuthenticationController();
        var auth = authenticationController;
        var sessao = auth != null ? auth.Session : null;
        if (sessao?.IsAuthenticated != true || !sessao.IsAdmin)
            throw new InvalidOperationException("Sessão administrativa indisponível.");
        var conta = sessao.AuthenticatedUserId;
        var url = new Uri(new Uri(auth.BackendBaseUrl.TrimEnd('/') + "/"), "api/admin/dialogue/suggest");
        using var req = new HttpRequestMessage(HttpMethod.Post, url);
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", sessao.AccessToken);
        req.Content = new StringContent(JsonConvert.SerializeObject(contexto), System.Text.Encoding.UTF8, "application/json");
        using var resposta = await httpClient.SendAsync(req, token);
        string json = await resposta.Content.ReadAsStringAsync();
        token.ThrowIfCancellationRequested();
        return sessao.AuthenticatedUserId == conta && sessao.IsAuthenticated && sessao.IsAdmin
            ? Newtonsoft.Json.Linq.JObject.Parse(json) : null;
    }

    private async Task<ExerciseLoadResult> FetchAsync(string baseUrl, int scenarioId, string npcKey, string pointKey, string level,
        CancellationToken cancellationToken)
    {
        ResolveAuthenticationController();
        if (authenticationController == null || authenticationController.Session == null)
            return ExerciseLoadResult.Failed("A sessão autenticada da Unity ainda não está disponível.");

        SessionManager session = authenticationController.Session;
        string token = session.AccessToken;
        if (string.IsNullOrWhiteSpace(token))
        {
            AuthenticationResult<bool> refresh = await session.RefreshAsync(cancellationToken);
            if (!refresh.IsSuccess || string.IsNullOrWhiteSpace(session.AccessToken))
                return ExerciseLoadResult.Failed(
                    refresh.Error == null ? "Faça login antes de carregar exercícios." : refresh.Error.Message);
            token = session.AccessToken;
        }

        if (string.IsNullOrWhiteSpace(baseUrl))
            return ExerciseLoadResult.Failed("Backend Base Url não foi configurada.");

        string query = "api/exercises?npc_key=" + Uri.EscapeDataString(npcKey)
            + "&id_cenario=" + scenarioId + "&nivel=" + Uri.EscapeDataString(level);
        if (!string.IsNullOrWhiteSpace(pointKey))
            query += "&point_key=" + Uri.EscapeDataString(pointKey);
        Uri baseUri = new Uri(baseUrl.EndsWith("/") ? baseUrl : baseUrl + "/", UriKind.Absolute);
        Uri requestUri = new Uri(baseUri, query);

        for (int attempt = 0; attempt < 2; attempt++)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, requestUri))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                using (HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken))
                {
                    string body = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized && attempt == 0)
                    {
                        AuthenticationResult<bool> refresh = await session.RefreshAsync(cancellationToken);
                        if (refresh.IsSuccess && !string.IsNullOrWhiteSpace(session.AccessToken))
                        {
                            token = session.AccessToken;
                            continue;
                        }
                        return ExerciseLoadResult.Failed(
                            refresh.Error == null ? "Sessão expirada; faça login novamente." : refresh.Error.Message,
                            (int)response.StatusCode);
                    }
                    if (!response.IsSuccessStatusCode)
                        return ExerciseLoadResult.Failed(
                            "GET /api/exercises retornou " + (int)response.StatusCode + ": " + body,
                            (int)response.StatusCode);

                    ExerciseApiListDTO envelope;
                    try
                    {
                        envelope = JsonConvert.DeserializeObject<ExerciseApiListDTO>(body);
                    }
                    catch (JsonException exception)
                    {
                        return ExerciseLoadResult.Failed("Resposta JSON inválida: " + exception.Message);
                    }
                    if (envelope == null || envelope.data == null)
                        return ExerciseLoadResult.Failed("Resposta da API não contém a lista 'data'.");
                    if (envelope.data.Count == 0)
                        return ExerciseLoadResult.Loaded(new Dictionary<string, List<ExerciseCatalogItem>>());

                    var byPoint = new Dictionary<string, List<ExerciseCatalogItem>>();
                    try
                    {
                        foreach (ExerciseApiDTO dto in envelope.data)
                        {
                            if (dto == null || dto.idCenario != scenarioId || dto.nivel != level
                                || string.IsNullOrWhiteSpace(dto.pointKey) || (pointKey != null && dto.pointKey != pointKey))
                                throw new InvalidOperationException("Exercício fora do cenário, nível ou ponto solicitado.");
                            ExercicioBase exercise;
                            TipoExercicio type = dto.ConvertToExecutable(out exercise);
                            List<ExerciseCatalogItem> pointItems;
                            if (!byPoint.TryGetValue(dto.pointKey, out pointItems))
                            {
                                pointItems = new List<ExerciseCatalogItem>();
                                byPoint.Add(dto.pointKey, pointItems);
                            }
                            pointItems.Add(new ExerciseCatalogItem(type, exercise));
                        }
                    }
                    catch (Exception exception)
                    {
                        return ExerciseLoadResult.Failed("Não foi possível converter o catálogo: " + exception.Message);
                    }

                    foreach (List<ExerciseCatalogItem> items in byPoint.Values)
                        items.Sort((left, right) =>
                        {
                            int levelCompare = string.Compare(left.Exercise.nivelBanco,
                                right.Exercise.nivelBanco, StringComparison.Ordinal);
                            if (levelCompare != 0) return levelCompare;
                            int orderCompare = left.Exercise.ordemBanco.CompareTo(right.Exercise.ordemBanco);
                            return orderCompare != 0
                                ? orderCompare
                                : left.Exercise.idExercise.CompareTo(right.Exercise.idExercise);
                        });
                    return ExerciseLoadResult.Loaded(byPoint);
                }
            }
        }
        return ExerciseLoadResult.Failed("Não foi possível autenticar a consulta de exercícios.");
    }

    private void ResolveAuthenticationController()
    {
        if (authenticationController == null)
            authenticationController = AuthenticationUIController.Instance;
        if (authenticationController == null)
            authenticationController = FindObjectOfType<AuthenticationUIController>();
    }
}
