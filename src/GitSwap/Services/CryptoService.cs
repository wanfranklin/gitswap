using System.Security.Cryptography;
using System.Text;

namespace GitSwap.Services;

public static class CryptoService
{
    private static readonly byte[] Salt = Encoding.UTF8.GetBytes("GitSwap_Salt_2024");

    public static string Encrypt(string plainText, string password)
    {
        using var aes = Aes.Create();
        var key = Rfc2898DeriveBytes.Pbkdf2(password, Salt, 100000, HashAlgorithmName.SHA256, 32);
        var iv = Rfc2898DeriveBytes.Pbkdf2(password, Salt, 100000, HashAlgorithmName.SHA256, 16);
        aes.Key = key;
        aes.IV = iv;

        using var encryptor = aes.CreateEncryptor();
        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var writer = new StreamWriter(cs))
        {
            writer.Write(plainText);
        }
        return Convert.ToBase64String(ms.ToArray());
    }

    public static string Decrypt(string cipherText, string password)
    {
        using var aes = Aes.Create();
        var key = Rfc2898DeriveBytes.Pbkdf2(password, Salt, 100000, HashAlgorithmName.SHA256, 32);
        var iv = Rfc2898DeriveBytes.Pbkdf2(password, Salt, 100000, HashAlgorithmName.SHA256, 16);
        aes.Key = key;
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var reader = new StreamReader(cs);
        return reader.ReadToEnd();
    }

    public static string GetOrCreateEncryptionKey()
    {
        var keyFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "GitSwap",
            ".key");

        if (File.Exists(keyFile))
            return File.ReadAllText(keyFile).Trim();

        var key = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        Directory.CreateDirectory(Path.GetDirectoryName(keyFile)!);
        File.WriteAllText(keyFile, key);
        return key;
    }
}
