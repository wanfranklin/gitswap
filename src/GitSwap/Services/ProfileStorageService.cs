using System.Text.Json;
using System.Text.RegularExpressions;
using GitSwap.Models;

namespace GitSwap.Services;

public class ProfileStorageService
{
    private readonly string _filePath;
    private readonly string? _encryptionKey;

    public ProfileStorageService(bool enableEncryption = false)
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var directoryPath = Path.Combine(appDataPath, "GitSwap");
        Directory.CreateDirectory(directoryPath);
        _filePath = Path.Combine(directoryPath, "profiles.json");
        _encryptionKey = enableEncryption ? CryptoService.GetOrCreateEncryptionKey() : null;
    }

    public ProfileStorageService(string directoryPath, bool enableEncryption = false)
    {
        Directory.CreateDirectory(directoryPath);
        _filePath = Path.Combine(directoryPath, "profiles.json");
        _encryptionKey = enableEncryption ? CryptoService.GetOrCreateEncryptionKey() : null;
    }

    public List<GitProfile> LoadProfiles()
    {
        if (!File.Exists(_filePath))
            return [];

        try
        {
            var content = File.ReadAllText(_filePath);

            if (_encryptionKey is not null && IsEncrypted(content))
                content = CryptoService.Decrypt(content, _encryptionKey);

            return JsonSerializer.Deserialize<List<GitProfile>>(content) ?? [];
        }
        catch (JsonException)
        {
            return [];
        }
        catch (Exception)
        {
            return [];
        }
    }

    public void SaveProfiles(List<GitProfile> profiles)
    {
        var sanitizedProfiles = profiles.Select(SanitizeProfile).ToList();
        var json = JsonSerializer.Serialize(sanitizedProfiles, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        var content = _encryptionKey is not null
            ? CryptoService.Encrypt(json, _encryptionKey)
            : json;

        File.WriteAllText(_filePath, content);
    }

    public static bool IsValidRepositoryPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return true;

        var normalized = Path.GetFullPath(path);
        return normalized.Contains(".git") || !normalized.Contains("..");
    }

    private static GitProfile SanitizeProfile(GitProfile profile)
    {
        profile.Name = SanitizeInput(profile.Name);
        profile.UserName = SanitizeInput(profile.UserName);
        profile.Email = SanitizeInput(profile.Email);
        return profile;
    }

    private static string SanitizeInput(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return Regex.Replace(input, @"[<>&""']", "");
    }

    private static bool IsEncrypted(string content)
    {
        return content.StartsWith("U2Ft");
    }
}
