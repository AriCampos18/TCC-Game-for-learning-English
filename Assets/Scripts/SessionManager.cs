using System;
using System.Threading;
using System.Threading.Tasks;

public sealed class SessionManager
{
    private readonly AuthenticationManager authenticationManager;
    private readonly IRefreshTokenStore refreshTokenStore;
    private readonly SemaphoreSlim operationLock = new SemaphoreSlim(1, 1);

    private string accessToken;
    private string refreshToken;
    private DateTimeOffset accessTokenExpiresAtUtc;
    private bool rememberMe;
    private int? authenticatedUserId;

    public string AccessToken
    {
        get
        {
            return DateTimeOffset.UtcNow < accessTokenExpiresAtUtc ? accessToken : null;
        }
    }

    public DateTimeOffset? AccessTokenExpiresAtUtc
    {
        get { return accessToken == null ? (DateTimeOffset?)null : accessTokenExpiresAtUtc; }
    }

    public bool IsAuthenticated
    {
        get { return AccessToken != null; }
    }

    public bool RememberMe
    {
        get { return rememberMe; }
    }

    public int? AuthenticatedUserId
    {
        get { return authenticatedUserId; }
    }

    public SessionManager(AuthenticationManager authenticationManager, IRefreshTokenStore refreshTokenStore)
    {
        this.authenticationManager = authenticationManager ??
                                     throw new ArgumentNullException(nameof(authenticationManager));
        this.refreshTokenStore = refreshTokenStore ??
                                 throw new ArgumentNullException(nameof(refreshTokenStore));
    }

    public async Task<AuthenticationResult<bool>> LoginAsync(
        string emailOrUsername,
        string password,
        bool remember,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        await operationLock.WaitAsync(cancellationToken);
        try
        {
            ClearInMemorySession();
            AuthenticationError clearError = ClearStoredRefreshToken();
            if (clearError != null)
            {
                return AuthenticationResult<bool>.Failure(clearError);
            }

            AuthenticationResult<AuthenticationTokens> loginResult =
                await authenticationManager.LoginAsync(emailOrUsername, password, cancellationToken);
            if (!loginResult.IsSuccess)
            {
                return AuthenticationResult<bool>.Failure(loginResult.Error);
            }

            AuthenticationTokens tokens = loginResult.Value;
            if (remember)
            {
                try
                {
                    refreshTokenStore.Save(tokens.RefreshToken);
                }
                catch (RefreshTokenStoreException exception)
                {
                    return await HandlePersistenceFailureAsync(
                        tokens.RefreshToken,
                        "Não foi possível salvar com segurança o refresh token.",
                        exception);
                }
            }

            SetInMemorySession(tokens, remember, tokens.User.Id);
            return AuthenticationResult<bool>.Success(true);
        }
        finally
        {
            operationLock.Release();
        }
    }

    public async Task<AuthenticationResult<bool>> RestoreSessionAsync(
        CancellationToken cancellationToken = default(CancellationToken))
    {
        await operationLock.WaitAsync(cancellationToken);
        try
        {
            ClearInMemorySession();

            string savedRefreshToken;
            try
            {
                savedRefreshToken = refreshTokenStore.Load();
            }
            catch (RefreshTokenStoreException exception)
            {
                AuthenticationError clearError = ClearStoredRefreshToken();
                return AuthenticationResult<bool>.Failure(
                    CreateStorageError("Não foi possível ler o refresh token protegido.", exception, clearError));
            }

            if (string.IsNullOrWhiteSpace(savedRefreshToken))
            {
                return AuthenticationResult<bool>.Success(false);
            }

            AuthenticationResult<bool> refreshResult =
                await RefreshSessionCoreAsync(savedRefreshToken, true, cancellationToken);
            if (!refreshResult.IsSuccess)
            {
                return refreshResult;
            }

            return await ConfirmCurrentUserAsync(cancellationToken);
        }
        finally
        {
            operationLock.Release();
        }
    }

    public async Task<AuthenticationResult<bool>> RefreshAsync(
        CancellationToken cancellationToken = default(CancellationToken))
    {
        await operationLock.WaitAsync(cancellationToken);
        try
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                ClearInMemorySession();
                return AuthenticationResult<bool>.Failure(new AuthenticationError(
                    AuthenticationFailureType.InvalidSession,
                    "Não há refresh token em memória para renovar a sessão."));
            }

            AuthenticationResult<bool> refreshResult =
                await RefreshSessionCoreAsync(refreshToken, rememberMe, cancellationToken);
            if (!refreshResult.IsSuccess)
            {
                return refreshResult;
            }

            return await ConfirmCurrentUserAsync(cancellationToken);
        }
        finally
        {
            operationLock.Release();
        }
    }

    public async Task<AuthenticationResult<bool>> LogoutAsync(
        CancellationToken cancellationToken = default(CancellationToken))
    {
        await operationLock.WaitAsync(cancellationToken);
        try
        {
            string tokenToRevoke = refreshToken;
            ClearInMemorySession();
            AuthenticationError clearError = ClearStoredRefreshToken();

            AuthenticationResult<bool> logoutResult = null;
            if (!string.IsNullOrWhiteSpace(tokenToRevoke))
            {
                logoutResult = await authenticationManager.LogoutAsync(tokenToRevoke, cancellationToken);
            }

            if (clearError != null)
            {
                string details = logoutResult != null && !logoutResult.IsSuccess
                    ? clearError.Details + " Falha no logout remoto: " + logoutResult.Error.Message
                    : clearError.Details;
                return AuthenticationResult<bool>.Failure(new AuthenticationError(
                    AuthenticationFailureType.Storage,
                    clearError.Message,
                    details));
            }

            if (logoutResult == null ||
                (logoutResult.IsSuccess && logoutResult.Value) ||
                (!logoutResult.IsSuccess &&
                 logoutResult.Error.Type == AuthenticationFailureType.InvalidSession))
            {
                return AuthenticationResult<bool>.Success(true);
            }

            return AuthenticationResult<bool>.Failure(logoutResult.Error);
        }
        finally
        {
            operationLock.Release();
        }
    }

    private async Task<AuthenticationResult<bool>> RefreshSessionCoreAsync(
        string tokenToRefresh,
        bool persistRotatedToken,
        CancellationToken cancellationToken)
    {
        ClearInMemorySession();
        AuthenticationResult<AuthenticationTokens> refreshResult;
        try
        {
            refreshResult = await authenticationManager.RefreshAsync(tokenToRefresh, cancellationToken);
        }
        catch (OperationCanceledException cancellationException)
        {
            AuthenticationError clearError = InvalidateLocalSessionAfterRefreshFailure();
            if (clearError != null)
            {
                throw new AggregateException(
                    "A renovação foi cancelada e não foi possível remover o refresh token local.",
                    cancellationException,
                    new RefreshTokenStoreException(clearError.Message));
            }

            throw;
        }

        if (!refreshResult.IsSuccess)
        {
            AuthenticationError clearError = InvalidateLocalSessionAfterRefreshFailure();
            if (clearError != null)
            {
                return AuthenticationResult<bool>.Failure(new AuthenticationError(
                    AuthenticationFailureType.Storage,
                    "A renovação falhou e não foi possível remover o refresh token local.",
                    clearError.Message + " Falha de renovação: " + refreshResult.Error.Message,
                    refreshResult.Error.StatusCode,
                    refreshResult.Error.Code));
            }

            return AuthenticationResult<bool>.Failure(refreshResult.Error);
        }

        AuthenticationTokens tokens = refreshResult.Value;
        if (persistRotatedToken)
        {
            try
            {
                refreshTokenStore.Save(tokens.RefreshToken);
            }
            catch (RefreshTokenStoreException exception)
            {
                return await HandlePersistenceFailureAsync(
                    tokens.RefreshToken,
                    "A sessão foi renovada, mas não foi possível persistir com segurança o refresh token rotacionado.",
                    exception);
            }
        }

        SetInMemorySession(tokens, persistRotatedToken, null);
        return AuthenticationResult<bool>.Success(true);
    }

    private async Task<AuthenticationResult<bool>> ConfirmCurrentUserAsync(
        CancellationToken cancellationToken)
    {
        authenticatedUserId = null;
        AuthenticationResult<AuthenticatedUser> userResult =
            await authenticationManager.GetCurrentUserAsync(AccessToken, cancellationToken);
        if (userResult.IsSuccess)
        {
            authenticatedUserId = userResult.Value.Id;
            return AuthenticationResult<bool>.Success(true);
        }

        if (userResult.Error.Type == AuthenticationFailureType.InvalidSession)
        {
            ClearInMemorySession();
            AuthenticationError clearError = ClearStoredRefreshToken();
            if (clearError != null)
            {
                return AuthenticationResult<bool>.Failure(new AuthenticationError(
                    AuthenticationFailureType.Storage,
                    "A identidade da sessão foi rejeitada e não foi possível limpar o refresh token local.",
                    clearError.Message,
                    userResult.Error.StatusCode,
                    userResult.Error.Code));
            }
        }

        return AuthenticationResult<bool>.Failure(userResult.Error);
    }

    private async Task<AuthenticationResult<bool>> HandlePersistenceFailureAsync(
        string tokenToRevoke,
        string message,
        RefreshTokenStoreException persistenceException)
    {
        ClearInMemorySession();
        AuthenticationError clearError = ClearStoredRefreshToken();
        AuthenticationResult<bool> logoutResult =
            await authenticationManager.LogoutAsync(tokenToRevoke, CancellationToken.None);

        string details = persistenceException.Message;
        if (clearError != null)
        {
            details += " A limpeza local também falhou: " + clearError.Details;
        }

        if (!logoutResult.IsSuccess)
        {
            details += " O logout remoto também falhou: " + logoutResult.Error.Message;
        }

        return AuthenticationResult<bool>.Failure(new AuthenticationError(
            AuthenticationFailureType.Storage,
            message,
            details));
    }

    private AuthenticationError InvalidateLocalSessionAfterRefreshFailure()
    {
        ClearInMemorySession();
        return ClearStoredRefreshToken();
    }

    private AuthenticationError ClearStoredRefreshToken()
    {
        try
        {
            refreshTokenStore.Clear();
            return null;
        }
        catch (RefreshTokenStoreException exception)
        {
            return new AuthenticationError(
                AuthenticationFailureType.Storage,
                "Não foi possível remover o refresh token protegido do dispositivo.",
                exception.Message);
        }
    }

    private static AuthenticationError CreateStorageError(
        string message,
        RefreshTokenStoreException exception,
        AuthenticationError clearError)
    {
        string details = exception.Message;
        if (clearError != null)
        {
            details += " A limpeza local também falhou: " + clearError.Details;
        }

        return new AuthenticationError(AuthenticationFailureType.Storage, message, details);
    }

    private void SetInMemorySession(
        AuthenticationTokens tokens,
        bool isRemembered,
        int? userId)
    {
        accessToken = tokens.AccessToken;
        refreshToken = tokens.RefreshToken;
        accessTokenExpiresAtUtc = tokens.AccessTokenExpiresAtUtc;
        rememberMe = isRemembered;
        authenticatedUserId = userId;
    }

    private void ClearInMemorySession()
    {
        accessToken = null;
        refreshToken = null;
        accessTokenExpiresAtUtc = default(DateTimeOffset);
        rememberMe = false;
        authenticatedUserId = null;
    }
}
