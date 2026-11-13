public sealed class AuthenticationControllerResult
{
    public bool IsSuccess { get; private set; }
    public bool IsWarning { get; private set; }
    public string Message { get; private set; }
    public AuthenticationError Error { get; private set; }

    private AuthenticationControllerResult(
        bool isSuccess,
        bool isWarning,
        string message,
        AuthenticationError error)
    {
        IsSuccess = isSuccess;
        IsWarning = isWarning;
        Message = message;
        Error = error;
    }

    public static AuthenticationControllerResult Success(string message)
    {
        return new AuthenticationControllerResult(true, false, message, null);
    }

    public static AuthenticationControllerResult Warning(string message, AuthenticationError error)
    {
        return new AuthenticationControllerResult(true, true, message, error);
    }

    public static AuthenticationControllerResult Failure(string message, AuthenticationError error)
    {
        return new AuthenticationControllerResult(false, false, message, error);
    }
}
