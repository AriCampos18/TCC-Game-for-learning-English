using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public sealed class AuthenticationManager : IDisposable
{
    private readonly HttpClient httpClient;
    private readonly Uri backendBaseUri;

    public AuthenticationManager(string backendBaseUrl, HttpMessageHandler handler = null)
    {
        Uri parsedBaseUri;
        if (string.IsNullOrWhiteSpace(backendBaseUrl) ||
            !Uri.TryCreate(backendBaseUrl, UriKind.Absolute, out parsedBaseUri) ||
            (parsedBaseUri.Scheme != Uri.UriSchemeHttp && parsedBaseUri.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException("Informe uma URL HTTP ou HTTPS válida para o backend.", nameof(backendBaseUrl));
        }

        string normalizedBaseUrl = parsedBaseUri.AbsoluteUri.EndsWith("/", StringComparison.Ordinal)
            ? parsedBaseUri.AbsoluteUri
            : parsedBaseUri.AbsoluteUri + "/";
        backendBaseUri = new Uri(normalizedBaseUrl, UriKind.Absolute);
        httpClient = handler == null ? new HttpClient() : new HttpClient(handler);
    }

    public async Task<AuthenticationResult<AuthenticationTokens>> LoginAsync(
        string emailOrUsername,
        string password,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        if (string.IsNullOrWhiteSpace(emailOrUsername) || string.IsNullOrEmpty(password))
        {
            return AuthenticationResult<AuthenticationTokens>.Failure(
                new AuthenticationError(AuthenticationFailureType.Validation, "Informe usuário/e-mail e senha."));
        }

        return await PostForTokensAsync(
            "api/auth/login",
            new LoginRequest { email_or_username = emailOrUsername, password = password },
            AuthenticationFailureType.InvalidCredentials,
            true,
            cancellationToken);
    }

    public async Task<AuthenticationResult<bool>> RegisterAsync(
        string email,
        string username,
        string password,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrEmpty(password) ||
            password.Length < 8)
        {
            return AuthenticationResult<bool>.Failure(
                new AuthenticationError(
                    AuthenticationFailureType.Validation,
                    "Informe e-mail, nome de usuário e senha com pelo menos 8 caracteres."));
        }

        AuthenticationResult<AuthenticationTokens> registerResult =
            await PostForTokensAsync(
                "api/auth/register",
                new RegisterRequest
                {
                    email = email,
                    username = username,
                    password = password
                },
                AuthenticationFailureType.Http,
                false,
                cancellationToken);

        if (!registerResult.IsSuccess)
        {
            return AuthenticationResult<bool>.Failure(registerResult.Error);
        }

        AuthenticationResult<bool> cleanupResult =
            await LogoutAsync(registerResult.Value.RefreshToken, CancellationToken.None);
        if (!cleanupResult.IsSuccess)
        {
            return AuthenticationResult<bool>.Failure(new AuthenticationError(
                AuthenticationFailureType.RegistrationCleanup,
                "O cadastro foi criado, mas não foi possível encerrar a sessão técnica. Não envie o cadastro novamente; tente entrar.",
                cleanupResult.Error.Message,
                cleanupResult.Error.StatusCode,
                cleanupResult.Error.Code));
        }

        return AuthenticationResult<bool>.Success(true);
    }

    public Task<AuthenticationResult<AuthenticationTokens>> RefreshAsync(
        string refreshToken,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Task.FromResult(AuthenticationResult<AuthenticationTokens>.Failure(
                new AuthenticationError(AuthenticationFailureType.Validation, "Refresh token ausente.")));
        }

        return PostForTokensAsync(
            "api/auth/refresh",
            new RefreshRequest { refresh_token = refreshToken },
            AuthenticationFailureType.InvalidSession,
            false,
            cancellationToken);
    }

    public async Task<AuthenticationResult<AuthenticatedUser>> GetCurrentUserAsync(
        string accessToken,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return AuthenticationResult<AuthenticatedUser>.Failure(new AuthenticationError(
                AuthenticationFailureType.InvalidSession,
                "Access token ausente."));
        }

        ApiResponse response = await SendAsync(
            HttpMethod.Get,
            "api/users/me",
            null,
            accessToken,
            AuthenticationFailureType.InvalidSession,
            cancellationToken);
        if (response.Error != null)
        {
            return AuthenticationResult<AuthenticatedUser>.Failure(response.Error);
        }

        return ParseUser(response.Data);
    }

    public async Task<AuthenticationResult<bool>> LogoutAsync(
        string refreshToken,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return AuthenticationResult<bool>.Failure(
                new AuthenticationError(AuthenticationFailureType.Validation, "Refresh token ausente."));
        }

        ApiResponse response = await PostAsync(
            "api/auth/logout",
            new RefreshRequest { refresh_token = refreshToken },
            AuthenticationFailureType.InvalidSession,
            cancellationToken);

        if (response.Error != null)
        {
            return AuthenticationResult<bool>.Failure(response.Error);
        }

        JToken messageToken;
        if (response.Data == null ||
            !response.Data.TryGetValue("message", StringComparison.Ordinal, out messageToken) ||
            messageToken.Type != JTokenType.String ||
            string.IsNullOrWhiteSpace(messageToken.Value<string>()))
        {
            return UnexpectedResponse<bool>("A resposta de logout não contém uma confirmação válida.");
        }

        return AuthenticationResult<bool>.Success(true);
    }

    public void Dispose()
    {
        httpClient.Dispose();
    }

    private async Task<AuthenticationResult<AuthenticationTokens>> PostForTokensAsync(
        string endpoint,
        object request,
        AuthenticationFailureType unauthorizedFailure,
        bool requireUser,
        CancellationToken cancellationToken)
    {
        ApiResponse response = await PostAsync(endpoint, request, unauthorizedFailure, cancellationToken);
        if (response.Error != null)
        {
            return AuthenticationResult<AuthenticationTokens>.Failure(response.Error);
        }

        JToken accessToken;
        JToken refreshToken;
        JToken expiresIn;
        if (response.Data == null ||
            !response.Data.TryGetValue("access_token", StringComparison.Ordinal, out accessToken) ||
            !response.Data.TryGetValue("refresh_token", StringComparison.Ordinal, out refreshToken) ||
            !response.Data.TryGetValue("expires_in", StringComparison.Ordinal, out expiresIn) ||
            accessToken.Type != JTokenType.String ||
            refreshToken.Type != JTokenType.String ||
            expiresIn.Type != JTokenType.Integer)
        {
            return UnexpectedResponse<AuthenticationTokens>(
                "A resposta de autenticação não contém os tokens ou a validade esperados.");
        }

        string accessTokenValue = accessToken.Value<string>();
        string refreshTokenValue = refreshToken.Value<string>();
        long expiresInValue = expiresIn.Value<long>();
        if (string.IsNullOrWhiteSpace(accessTokenValue) ||
            string.IsNullOrWhiteSpace(refreshTokenValue) ||
            expiresInValue <= 0 ||
            expiresInValue > int.MaxValue)
        {
            return UnexpectedResponse<AuthenticationTokens>(
                "A resposta de autenticação contém tokens ou validade inválidos.");
        }

        AuthenticatedUser user = null;
        if (requireUser)
        {
            JToken userToken;
            if (!response.Data.TryGetValue("user", StringComparison.Ordinal, out userToken))
            {
                return UnexpectedResponse<AuthenticationTokens>(
                    "A resposta de login não contém o usuário autenticado.");
            }

            AuthenticationResult<AuthenticatedUser> userResult = ParseUser(userToken);
            if (!userResult.IsSuccess)
            {
                return AuthenticationResult<AuthenticationTokens>.Failure(userResult.Error);
            }

            user = userResult.Value;
        }

        return AuthenticationResult<AuthenticationTokens>.Success(new AuthenticationTokens(
            accessTokenValue,
            refreshTokenValue,
            (int)expiresInValue,
            DateTimeOffset.UtcNow.AddSeconds(expiresInValue),
            user));
    }

    private async Task<ApiResponse> PostAsync(
        string endpoint,
        object request,
        AuthenticationFailureType unauthorizedFailure,
        CancellationToken cancellationToken)
    {
        string requestJson = JsonConvert.SerializeObject(request);
        var content = new StringContent(requestJson, Encoding.UTF8, "application/json");
        return await SendAsync(
            HttpMethod.Post,
            endpoint,
            content,
            null,
            unauthorizedFailure,
            cancellationToken);
    }

    private async Task<ApiResponse> SendAsync(
        HttpMethod method,
        string endpoint,
        HttpContent content,
        string bearerToken,
        AuthenticationFailureType unauthorizedFailure,
        CancellationToken cancellationToken)
    {
        using (var request = new HttpRequestMessage(method, new Uri(backendBaseUri, endpoint)))
        {
            request.Content = content;
            if (!string.IsNullOrWhiteSpace(bearerToken))
            {
                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);
            }

            HttpResponseMessage httpResponse;
            try
            {
                httpResponse = await httpClient.SendAsync(request, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (OperationCanceledException exception)
            {
                return ApiResponse.FromError(new AuthenticationError(
                    AuthenticationFailureType.Connection,
                    "A conexão com o backend expirou.",
                    exception.Message));
            }
            catch (HttpRequestException exception)
            {
                return ApiResponse.FromError(new AuthenticationError(
                    AuthenticationFailureType.Connection,
                    "Não foi possível conectar ao backend.",
                    exception.Message));
            }

            using (httpResponse)
            {
                string responseJson;
                try
                {
                    responseJson = await httpResponse.Content.ReadAsStringAsync();
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (OperationCanceledException exception)
                {
                    return ApiResponse.FromError(new AuthenticationError(
                        AuthenticationFailureType.Connection,
                        "A conexão com o backend expirou.",
                        exception.Message));
                }
                catch (HttpRequestException exception)
                {
                    return ApiResponse.FromError(new AuthenticationError(
                        AuthenticationFailureType.Connection,
                        "Não foi possível ler a resposta do backend.",
                        exception.Message));
                }
                catch (IOException exception)
                {
                    return ApiResponse.FromError(new AuthenticationError(
                        AuthenticationFailureType.Connection,
                        "Não foi possível ler a resposta do backend.",
                        exception.Message));
                }

                JObject responseObject;
                try
                {
                    responseObject = JObject.Parse(responseJson);
                }
                catch (JsonException exception)
                {
                    if (httpResponse.IsSuccessStatusCode)
                    {
                        return ApiResponse.FromError(new AuthenticationError(
                            AuthenticationFailureType.UnexpectedResponse,
                            "O backend retornou uma resposta JSON inválida.",
                            exception.Message,
                            httpResponse.StatusCode));
                    }

                    return ApiResponse.FromError(CreateHttpError(
                        httpResponse.StatusCode,
                        unauthorizedFailure,
                        null));
                }

                if (!httpResponse.IsSuccessStatusCode)
                {
                    return ApiResponse.FromError(CreateHttpError(
                        httpResponse.StatusCode,
                        unauthorizedFailure,
                        ReadApiError(responseObject)));
                }

                JToken data;
                if (!responseObject.TryGetValue("data", StringComparison.Ordinal, out data) ||
                    data.Type != JTokenType.Object)
                {
                    return ApiResponse.FromError(new AuthenticationError(
                        AuthenticationFailureType.UnexpectedResponse,
                        "O backend retornou uma resposta sem o objeto 'data'.",
                        responseJson,
                        httpResponse.StatusCode));
                }

                return ApiResponse.FromData((JObject)data);
            }
        }
    }

    private static AuthenticationResult<AuthenticatedUser> ParseUser(JToken userToken)
    {
        if (userToken == null || userToken.Type != JTokenType.Object)
        {
            return UnexpectedResponse<AuthenticatedUser>(
                "A resposta do usuário autenticado não é um objeto válido.");
        }

        JToken idToken;
        if (!((JObject)userToken).TryGetValue("id", StringComparison.Ordinal, out idToken) ||
            idToken.Type != JTokenType.Integer)
        {
            return UnexpectedResponse<AuthenticatedUser>(
                "A resposta do usuário autenticado não contém um ID inteiro.");
        }

        long idValue;
        try
        {
            idValue = idToken.Value<long>();
        }
        catch (Exception exception) when (exception is OverflowException || exception is FormatException)
        {
            return UnexpectedResponse<AuthenticatedUser>(
                "O ID do usuário autenticado está fora do intervalo permitido.");
        }

        if (idValue <= 0 || idValue > int.MaxValue)
        {
            return UnexpectedResponse<AuthenticatedUser>(
                "O ID do usuário autenticado está fora do intervalo permitido.");
        }

        return AuthenticationResult<AuthenticatedUser>.Success(new AuthenticatedUser((int)idValue));
    }

    private static AuthenticationError CreateHttpError(
        HttpStatusCode statusCode,
        AuthenticationFailureType unauthorizedFailure,
        ApiError apiError)
    {
        AuthenticationFailureType failureType;
        if (statusCode == HttpStatusCode.Unauthorized)
        {
            failureType = unauthorizedFailure;
        }
        else if ((int)statusCode >= 500)
        {
            failureType = AuthenticationFailureType.Server;
        }
        else
        {
            failureType = AuthenticationFailureType.Http;
        }

        string message = apiError != null && !string.IsNullOrWhiteSpace(apiError.Message)
            ? apiError.Message
            : "O backend respondeu com HTTP " + (int)statusCode + ".";
        string code = apiError == null ? null : apiError.Code;
        return new AuthenticationError(failureType, message, null, statusCode, code);
    }

    private static ApiError ReadApiError(JObject responseObject)
    {
        JToken errorToken;
        if (!responseObject.TryGetValue("error", StringComparison.Ordinal, out errorToken) ||
            errorToken.Type != JTokenType.Object)
        {
            return null;
        }

        JObject errorObject = (JObject)errorToken;
        JToken codeToken;
        JToken messageToken;
        string code = errorObject.TryGetValue("code", StringComparison.Ordinal, out codeToken) &&
                      codeToken.Type == JTokenType.String
            ? codeToken.Value<string>()
            : null;
        string message = errorObject.TryGetValue("message", StringComparison.Ordinal, out messageToken) &&
                         messageToken.Type == JTokenType.String
            ? messageToken.Value<string>()
            : null;
        return new ApiError(code, message);
    }

    private static AuthenticationResult<T> UnexpectedResponse<T>(string message)
    {
        return AuthenticationResult<T>.Failure(
            new AuthenticationError(AuthenticationFailureType.UnexpectedResponse, message));
    }

    private sealed class LoginRequest
    {
        public string email_or_username { get; set; }
        public string password { get; set; }
    }

    private sealed class RefreshRequest
    {
        public string refresh_token { get; set; }
    }

    private sealed class RegisterRequest
    {
        public string email { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }

    private sealed class ApiResponse
    {
        public JObject Data { get; private set; }
        public AuthenticationError Error { get; private set; }

        public static ApiResponse FromData(JObject data)
        {
            return new ApiResponse { Data = data };
        }

        public static ApiResponse FromError(AuthenticationError error)
        {
            return new ApiResponse { Error = error };
        }
    }

    private sealed class ApiError
    {
        public string Code { get; private set; }
        public string Message { get; private set; }

        public ApiError(string code, string message)
        {
            Code = code;
            Message = message;
        }
    }
}

public sealed class AuthenticationTokens
{
    public string AccessToken { get; private set; }
    public string RefreshToken { get; private set; }
    public int ExpiresIn { get; private set; }
    public DateTimeOffset AccessTokenExpiresAtUtc { get; private set; }
    public AuthenticatedUser User { get; private set; }

    public AuthenticationTokens(
        string accessToken,
        string refreshToken,
        int expiresIn,
        DateTimeOffset accessTokenExpiresAtUtc,
        AuthenticatedUser user = null)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        ExpiresIn = expiresIn;
        AccessTokenExpiresAtUtc = accessTokenExpiresAtUtc;
        User = user;
    }
}

public sealed class AuthenticatedUser
{
    public int Id { get; private set; }

    public AuthenticatedUser(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id));
        }

        Id = id;
    }
}

public enum AuthenticationFailureType
{
    Validation,
    InvalidCredentials,
    InvalidSession,
    Connection,
    Http,
    Server,
    UnexpectedResponse,
    Storage,
    RegistrationCleanup
}

public sealed class AuthenticationError
{
    public AuthenticationFailureType Type { get; private set; }
    public string Message { get; private set; }
    public string Details { get; private set; }
    public HttpStatusCode? StatusCode { get; private set; }
    public string Code { get; private set; }

    public AuthenticationError(
        AuthenticationFailureType type,
        string message,
        string details = null,
        HttpStatusCode? statusCode = null,
        string code = null)
    {
        Type = type;
        Message = message;
        Details = details;
        StatusCode = statusCode;
        Code = code;
    }
}

public sealed class AuthenticationResult<T>
{
    public bool IsSuccess { get; private set; }
    public T Value { get; private set; }
    public AuthenticationError Error { get; private set; }

    private AuthenticationResult(bool isSuccess, T value, AuthenticationError error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static AuthenticationResult<T> Success(T value)
    {
        return new AuthenticationResult<T>(true, value, null);
    }

    public static AuthenticationResult<T> Failure(AuthenticationError error)
    {
        if (error == null)
        {
            throw new ArgumentNullException(nameof(error));
        }

        return new AuthenticationResult<T>(false, default(T), error);
    }
}
