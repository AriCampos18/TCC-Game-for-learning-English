using System;

public interface IRefreshTokenStore
{
    string Load();
    void Save(string refreshToken);
    void Clear();
}

public sealed class RefreshTokenStoreException : Exception
{
    public RefreshTokenStoreException(string message)
        : base(message)
    {
    }

    public RefreshTokenStoreException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
