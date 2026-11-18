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

    private Task<ExerciseLoadResult> LoadAsync(string npcKey, string pointKey, string level,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(npcKey))
            return Task.FromResult(ExerciseLoadResult.Failed("npc_key não pode ser vazio."));
        if (string.IsNullOrWhiteSpace(level))
            return Task.FromResult(ExerciseLoadResult.Failed("nivel não pode ser vazio."));

        string cacheKey = npcKey + "|" + (pointKey ?? "*") + "|" + level;
        lock (cacheLock)
        {
            ExerciseLoadResult cached;
            if (cache.TryGetValue(cacheKey, out cached)) return Task.FromResult(cached);
            Task<ExerciseLoadResult> pending;
            if (inFlight.TryGetValue(cacheKey, out pending)) return pending;
            var completion = new TaskCompletionSource<ExerciseLoadResult>();
            pending = completion.Task;
            inFlight[cacheKey] = pending;
            CompleteLoadAsync(cacheKey, npcKey, pointKey, level, cancellationToken, completion);
            return pending;
        }
    }

    private async void CompleteLoadAsync(string cacheKey, string npcKey,
        string pointKey, string level, CancellationToken cancellationToken,
        TaskCompletionSource<ExerciseLoadResult> completion)
    {
        ExerciseLoadResult result;
        try
        {
            result = await FetchAsync(npcKey, pointKey, level, cancellationToken);
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
            if (result.Success) cache[cacheKey] = result;
        }
        completion.SetResult(result);
    }

    private async Task<ExerciseLoadResult> FetchAsync(string npcKey, string pointKey, string level,
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

        string baseUrl = authenticationController.BackendBaseUrl;
        if (string.IsNullOrWhiteSpace(baseUrl))
            return ExerciseLoadResult.Failed("Backend Base Url não foi configurada.");

        string query = "api/exercises?npc_key=" + Uri.EscapeDataString(npcKey)
            + "&nivel=" + Uri.EscapeDataString(level);
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
                            if (dto == null || string.IsNullOrWhiteSpace(dto.pointKey))
                                throw new InvalidOperationException("Um exercício não contém point_key.");
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
