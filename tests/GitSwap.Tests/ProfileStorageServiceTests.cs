using GitSwap.Models;
using GitSwap.Services;

namespace GitSwap.Tests;

public class ProfileStorageServiceTests : IDisposable
{
    private readonly string _tempDir;

    public ProfileStorageServiceTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"GitSwapTests_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }

    private ProfileStorageService CreateService()
    {
        var service = new ProfileStorageService(_tempDir);
        return service;
    }

    [Fact]
    public void LoadProfiles_WhenFileDoesNotExist_ReturnsEmptyList()
    {
        var service = CreateService();
        var profiles = service.LoadProfiles();
        Assert.NotNull(profiles);
        Assert.Empty(profiles);
    }

    [Fact]
    public void SaveProfiles_ThenLoadProfiles_ReturnsSameData()
    {
        var service = CreateService();
        var profiles = new List<GitProfile>
        {
            new() { Name = "Work", UserName = "John", Email = "john@work.com" },
            new() { Name = "Personal", UserName = "John", Email = "john@gmail.com" }
        };

        service.SaveProfiles(profiles);
        var loaded = service.LoadProfiles();

        Assert.Equal(2, loaded.Count);
        Assert.Equal("Work", loaded[0].Name);
        Assert.Equal("Personal", loaded[1].Name);
        Assert.Equal("john@work.com", loaded[0].Email);
    }

    [Fact]
    public void SaveProfiles_OverwritesExistingData()
    {
        var service = CreateService();
        var profiles = new List<GitProfile>
        {
            new() { Name = "Work", UserName = "John", Email = "john@work.com" }
        };
        service.SaveProfiles(profiles);

        var updatedProfiles = new List<GitProfile>
        {
            new() { Name = "NewProfile", UserName = "Jane", Email = "jane@example.com" }
        };
        service.SaveProfiles(updatedProfiles);

        var loaded = service.LoadProfiles();
        Assert.Single(loaded);
        Assert.Equal("NewProfile", loaded[0].Name);
    }

    [Fact]
    public void SaveProfiles_EmptyList_ReturnsEmptyList()
    {
        var service = CreateService();
        service.SaveProfiles([]);
        var loaded = service.LoadProfiles();
        Assert.Empty(loaded);
    }

    [Fact]
    public void SaveProfiles_CreatesDirectory_IfNotExists()
    {
        var newDir = Path.Combine(_tempDir, "nested", "dir");
        var service = new ProfileStorageService(newDir);
        service.SaveProfiles([]);
        var loaded = service.LoadProfiles();
        Assert.NotNull(loaded);
    }

    [Fact]
    public void SaveProfiles_SanitizesInput_RemovesDangerousChars()
    {
        var service = CreateService();
        var profiles = new List<GitProfile>
        {
            new() { Name = "Test<script>", UserName = "user\"name", Email = "test@email.com" }
        };

        service.SaveProfiles(profiles);
        var loaded = service.LoadProfiles();

        Assert.DoesNotContain("<script>", loaded[0].Name);
        Assert.DoesNotContain("\"", loaded[0].UserName);
    }

    [Fact]
    public void IsValidRepositoryPath_ValidPath_ReturnsTrue()
    {
        Assert.True(ProfileStorageService.IsValidRepositoryPath("/home/user/project"));
        Assert.True(ProfileStorageService.IsValidRepositoryPath(""));
        Assert.True(ProfileStorageService.IsValidRepositoryPath(null!));
    }

    [Fact]
    public void IsValidRepositoryPath_PathTraversal_ReturnsFalse()
    {
        Assert.False(ProfileStorageService.IsValidRepositoryPath("/home/user/../../etc/passwd"));
    }

    [Fact]
    public void IsValidRepositoryPath_GitFolder_ReturnsTrue()
    {
        Assert.True(ProfileStorageService.IsValidRepositoryPath("/home/user/.git/project"));
    }
}
