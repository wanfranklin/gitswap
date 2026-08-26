using GitSwap.Services;

namespace GitSwap.Tests;

public class GitServiceTests
{
    private readonly GitService _gitService = new();

    [Fact]
    public void IsGitInstalled_WhenGitExists_ReturnsTrue()
    {
        var result = _gitService.IsGitInstalled();
        Assert.True(result);
    }

    [Fact]
    public void IsGitRepository_WithNonExistentPath_ReturnsFalse()
    {
        var result = _gitService.IsGitRepository("/nonexistent/path/that/does/not/exist");
        Assert.False(result);
    }

    [Fact]
    public async Task GetCurrentConfigAsync_ReturnsValidResult()
    {
        var (name, email) = await _gitService.GetCurrentConfigAsync();

        Assert.NotNull(name);
        Assert.NotNull(email);
    }

    [Fact]
    public void IsGitInstalled_WithInvalidExecutable_ReturnsFalse()
    {
        var service = new GitService("nonexistent-git-binary");
        var result = service.IsGitInstalled();
        Assert.False(result);
    }

    [Fact]
    public async Task SetUserNameAsync_WithInvalidPath_ThrowsInvalidOperationException()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _gitService.SetUserNameAsync("test", "/nonexistent/path"));
    }

    [Fact]
    public async Task SetEmailAsync_WithInvalidPath_ThrowsInvalidOperationException()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _gitService.SetEmailAsync("test@example.com", "/nonexistent/path"));
    }

    [Fact]
    public async Task GetGlobalConfigAsync_ReturnsValidResult()
    {
        var (name, email) = await _gitService.GetGlobalConfigAsync();

        Assert.NotNull(name);
        Assert.NotNull(email);
    }

    [Fact]
    public async Task GetGlobalConfigAsync_ReturnsDifferentResultThanCurrentConfig()
    {
        var (currentName, currentEmail) = await _gitService.GetCurrentConfigAsync();
        var (globalName, globalEmail) = await _gitService.GetGlobalConfigAsync();

        Assert.NotNull(currentName);
        Assert.NotNull(currentEmail);
        Assert.NotNull(globalName);
        Assert.NotNull(globalEmail);
    }
}
