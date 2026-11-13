using System;
using System.Threading;
using System.Threading.Tasks;

public sealed class LoginController
{
    private readonly SessionManager sessionManager;

    public LoginController(SessionManager sessionManager)
    {
        this.sessionManager = sessionManager ??
                             throw new ArgumentNullException(nameof(sessionManager));
    }

    public async Task<AuthenticationControllerResult> LoginAsync(
        string emailOrUsername,
        string password,
        bool rememberMe,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        if (string.IsNullOrWhiteSpace(emailOrUsername))
        {
            return ValidationFailure("Informe seu e-mail ou nome de usuário.");
        }

        if (string.IsNullOrEmpty(password))
        {
            return ValidationFailure("Informe sua senha.");
        }

        AuthenticationResult<bool> result = await sessionManager.LoginAsync(
            emailOrUsername.Trim(),
            password,
            rememberMe,
            cancellationToken);

        if (result.IsSuccess)
        {
            return AuthenticationControllerResult.Success("Login realizado com sucesso.");
        }

        return AuthenticationControllerResult.Failure(
            GetUserMessage(result.Error),
            result.Error);
    }

    private static AuthenticationControllerResult ValidationFailure(string message)
    {
        return AuthenticationControllerResult.Failure(
            message,
            new AuthenticationError(AuthenticationFailureType.Validation, message));
    }

    private static string GetUserMessage(AuthenticationError error)
    {
        switch (error.Type)
        {
            case AuthenticationFailureType.InvalidCredentials:
                return "E-mail/usuário ou senha inválidos.";
            case AuthenticationFailureType.Connection:
                return "Não foi possível conectar ao servidor. Verifique sua conexão e tente novamente.";
            case AuthenticationFailureType.InvalidSession:
                return "A sessão expirou. Faça login novamente.";
            case AuthenticationFailureType.Storage:
                return "Não foi possível manter a sessão segura neste dispositivo.";
            case AuthenticationFailureType.Server:
                return "O servidor está temporariamente indisponível. Tente novamente mais tarde.";
            default:
                return "Não foi possível realizar o login.";
        }
    }
}
