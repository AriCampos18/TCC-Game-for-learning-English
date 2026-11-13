using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

public sealed class RegisterController
{
    private static readonly Regex EmailPattern =
        new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    private readonly AuthenticationManager authenticationManager;

    public RegisterController(AuthenticationManager authenticationManager)
    {
        this.authenticationManager = authenticationManager ??
                                     throw new ArgumentNullException(nameof(authenticationManager));
    }

    public async Task<AuthenticationControllerResult> RegisterAsync(
        string email,
        string username,
        string password,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return ValidationFailure("Informe seu e-mail.");
        }

        if (string.IsNullOrWhiteSpace(username))
        {
            return ValidationFailure("Informe seu nome de usuário.");
        }

        if (string.IsNullOrEmpty(password))
        {
            return ValidationFailure("Informe sua senha.");
        }

        string normalizedEmail = email.Trim();
        if (!EmailPattern.IsMatch(normalizedEmail))
        {
            return ValidationFailure("Informe um e-mail válido.");
        }

        if (password.Length < 8)
        {
            return ValidationFailure("A senha deve conter pelo menos 8 caracteres.");
        }

        AuthenticationResult<bool> result = await authenticationManager.RegisterAsync(
            normalizedEmail,
            username.Trim(),
            password,
            cancellationToken);

        if (result.IsSuccess)
        {
            return AuthenticationControllerResult.Success("Cadastro realizado com sucesso. Você já pode entrar.");
        }

        if (result.Error.Type == AuthenticationFailureType.RegistrationCleanup)
        {
            return AuthenticationControllerResult.Warning(
                result.Error.Message,
                result.Error);
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
            case AuthenticationFailureType.Connection:
                return "Não foi possível conectar ao servidor. Verifique sua conexão e tente novamente.";
            case AuthenticationFailureType.Server:
                return "O servidor está temporariamente indisponível. Tente novamente mais tarde.";
            case AuthenticationFailureType.Http:
                return error.StatusCode == System.Net.HttpStatusCode.Conflict
                    ? "Este e-mail ou nome de usuário já está cadastrado."
                    : "Não foi possível concluir o cadastro.";
            case AuthenticationFailureType.UnexpectedResponse:
                return "O servidor retornou uma resposta inesperada.";
            default:
                return "Não foi possível concluir o cadastro.";
        }
    }
}
