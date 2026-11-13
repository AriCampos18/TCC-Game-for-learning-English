using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public sealed class WindowsDpapiRefreshTokenStore : IRefreshTokenStore
{
    private const uint CryptProtectUiForbidden = 0x1;
    private static readonly UTF8Encoding StrictUtf8 = new UTF8Encoding(false, true);

    private readonly string filePath;
    private readonly string temporaryFilePath;

    public WindowsDpapiRefreshTokenStore()
        : this(Path.Combine(Application.persistentDataPath, "authentication.refresh.dpapi"))
    {
    }

    public WindowsDpapiRefreshTokenStore(string storageFilePath)
    {
        if (string.IsNullOrWhiteSpace(storageFilePath))
        {
            throw new ArgumentException("Informe o caminho do armazenamento protegido.", nameof(storageFilePath));
        }

        filePath = storageFilePath;
        temporaryFilePath = filePath + ".pending";
    }

    public string Load()
    {
        EnsureWindows();
        if (!File.Exists(filePath))
        {
            return null;
        }

        byte[] encrypted = null;
        byte[] plaintext = null;
        try
        {
            encrypted = File.ReadAllBytes(filePath);
            plaintext = Unprotect(encrypted, GetEntropy());
            string refreshToken = StrictUtf8.GetString(plaintext);
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new RefreshTokenStoreException("O refresh token protegido está vazio.");
            }

            return refreshToken;
        }
        catch (RefreshTokenStoreException)
        {
            throw;
        }
        catch (Exception exception) when (IsStorageException(exception))
        {
            throw new RefreshTokenStoreException("Não foi possível recuperar o refresh token protegido.", exception);
        }
        finally
        {
            ClearBytes(encrypted);
            ClearBytes(plaintext);
        }
    }

    public void Save(string refreshToken)
    {
        EnsureWindows();
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new RefreshTokenStoreException("Não é permitido armazenar um refresh token vazio.");
        }

        byte[] plaintext = StrictUtf8.GetBytes(refreshToken);
        byte[] encrypted = null;
        try
        {
            encrypted = Protect(plaintext, GetEntropy());
            string directory = Path.GetDirectoryName(filePath);
            if (string.IsNullOrWhiteSpace(directory))
            {
                throw new RefreshTokenStoreException("O caminho do armazenamento protegido não é válido.");
            }

            Directory.CreateDirectory(directory);
            File.WriteAllBytes(temporaryFilePath, encrypted);
            if (File.Exists(filePath))
            {
                File.Replace(temporaryFilePath, filePath, null);
            }
            else
            {
                File.Move(temporaryFilePath, filePath);
            }
        }
        catch (RefreshTokenStoreException)
        {
            throw;
        }
        catch (Exception exception) when (IsStorageException(exception))
        {
            throw new RefreshTokenStoreException("Não foi possível salvar o refresh token protegido.", exception);
        }
        finally
        {
            ClearBytes(plaintext);
            ClearBytes(encrypted);
            DeletePendingFile();
        }
    }

    public void Clear()
    {
        EnsureWindows();
        DeleteFile(filePath);
        DeletePendingFile();
    }

    private byte[] GetEntropy()
    {
        string applicationScope = "TCC-Game-for-learning-English|" +
                                  Application.companyName + "|" +
                                  Application.productName + "|refresh-token-v1";
        return Encoding.UTF8.GetBytes(applicationScope);
    }

    private static byte[] Protect(byte[] plaintext, byte[] entropy)
    {
        DataBlob input = AllocateBlob(plaintext);
        DataBlob optionalEntropy = AllocateBlob(entropy);
        DataBlob output = default(DataBlob);
        try
        {
            if (!CryptProtectData(
                    ref input,
                    null,
                    ref optionalEntropy,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    CryptProtectUiForbidden,
                    out output))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            return CopyBlob(output);
        }
        finally
        {
            FreeHGlobalBlob(ref input, true);
            FreeHGlobalBlob(ref optionalEntropy, true);
            FreeLocalBlob(ref output, true);
        }
    }

    private static byte[] Unprotect(byte[] encrypted, byte[] entropy)
    {
        DataBlob input = AllocateBlob(encrypted);
        DataBlob optionalEntropy = AllocateBlob(entropy);
        DataBlob output = default(DataBlob);
        IntPtr description = IntPtr.Zero;
        try
        {
            if (!CryptUnprotectData(
                    ref input,
                    out description,
                    ref optionalEntropy,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    CryptProtectUiForbidden,
                    out output))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            return CopyBlob(output);
        }
        finally
        {
            FreeHGlobalBlob(ref input, true);
            FreeHGlobalBlob(ref optionalEntropy, true);
            FreeLocalBlob(ref output, true);
            if (description != IntPtr.Zero)
            {
                LocalFree(description);
            }
        }
    }

    private static DataBlob AllocateBlob(byte[] value)
    {
        IntPtr pointer = Marshal.AllocHGlobal(value.Length);
        Marshal.Copy(value, 0, pointer, value.Length);
        return new DataBlob { Size = value.Length, Data = pointer };
    }

    private static byte[] CopyBlob(DataBlob blob)
    {
        if (blob.Size <= 0 || blob.Data == IntPtr.Zero)
        {
            throw new CryptographicException("O DPAPI retornou dados vazios.");
        }

        byte[] result = new byte[blob.Size];
        Marshal.Copy(blob.Data, result, 0, blob.Size);
        return result;
    }

    private static void FreeHGlobalBlob(ref DataBlob blob, bool clear)
    {
        if (blob.Data == IntPtr.Zero)
        {
            return;
        }

        if (clear)
        {
            ClearNativeBytes(blob.Data, blob.Size);
        }

        Marshal.FreeHGlobal(blob.Data);
        blob = default(DataBlob);
    }

    private static void FreeLocalBlob(ref DataBlob blob, bool clear)
    {
        if (blob.Data == IntPtr.Zero)
        {
            return;
        }

        if (clear)
        {
            ClearNativeBytes(blob.Data, blob.Size);
        }

        LocalFree(blob.Data);
        blob = default(DataBlob);
    }

    private static void ClearNativeBytes(IntPtr pointer, int length)
    {
        for (int index = 0; index < length; index++)
        {
            Marshal.WriteByte(pointer, index, 0);
        }
    }

    private static void ClearBytes(byte[] value)
    {
        if (value != null)
        {
            Array.Clear(value, 0, value.Length);
        }
    }

    private void DeletePendingFile()
    {
        DeleteFile(temporaryFilePath);
    }

    private static void DeleteFile(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (Exception exception) when (IsStorageException(exception))
        {
            throw new RefreshTokenStoreException("Não foi possível remover os dados protegidos locais.", exception);
        }
    }

    private static bool IsStorageException(Exception exception)
    {
        return exception is IOException ||
               exception is UnauthorizedAccessException ||
               exception is Win32Exception ||
               exception is ExternalException ||
               exception is DllNotFoundException ||
               exception is EntryPointNotFoundException ||
               exception is PlatformNotSupportedException ||
               exception is SecurityException ||
               exception is CryptographicException ||
               exception is DecoderFallbackException ||
               exception is ArgumentException;
    }

    private static void EnsureWindows()
    {
        if (Application.platform != RuntimePlatform.WindowsEditor &&
            Application.platform != RuntimePlatform.WindowsPlayer)
        {
            throw new RefreshTokenStoreException("O armazenamento DPAPI está disponível somente no Windows nesta etapa.");
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct DataBlob
    {
        public int Size;
        public IntPtr Data;
    }

    [DllImport("crypt32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CryptProtectData(
        ref DataBlob input,
        string description,
        ref DataBlob optionalEntropy,
        IntPtr reserved,
        IntPtr prompt,
        uint flags,
        out DataBlob output);

    [DllImport("crypt32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CryptUnprotectData(
        ref DataBlob input,
        out IntPtr description,
        ref DataBlob optionalEntropy,
        IntPtr reserved,
        IntPtr prompt,
        uint flags,
        out DataBlob output);

    [DllImport("kernel32.dll")]
    private static extern IntPtr LocalFree(IntPtr memory);
}
