using System.Diagnostics;

namespace GitSwap.Services;

public class GitService
{
    private readonly string _gitExecutable;

    public GitService(string gitExecutable = "git")
    {
        _gitExecutable = gitExecutable;
    }

    public bool IsGitInstalled()
    {
        try
        {
            var result = RunGitCommand(["--version"]);
            return result.ExitCode == 0;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool IsGitRepository(string path)
    {
        if (!Directory.Exists(path))
            return false;

        var gitDir = Path.Combine(path, ".git");
        return Directory.Exists(gitDir);
    }

    public async Task<(string UserName, string Email)> GetCurrentConfigAsync(string? repositoryPath = null)
    {
        var nameResult = await RunGitCommandAsync(["config", "user.name"], repositoryPath);
        var emailResult = await RunGitCommandAsync(["config", "user.email"], repositoryPath);

        var name = nameResult.ExitCode == 0 ? nameResult.Output.Trim() : string.Empty;
        var email = emailResult.ExitCode == 0 ? emailResult.Output.Trim() : string.Empty;

        return (name, email);
    }

    public async Task<(string UserName, string Email)> GetGlobalConfigAsync()
    {
        var nameResult = await RunGitCommandAsync(["config", "--global", "user.name"]);
        var emailResult = await RunGitCommandAsync(["config", "--global", "user.email"]);

        var name = nameResult.ExitCode == 0 ? nameResult.Output.Trim() : string.Empty;
        var email = emailResult.ExitCode == 0 ? emailResult.Output.Trim() : string.Empty;

        return (name, email);
    }

    public async Task SetUserNameAsync(string userName, string? repositoryPath = null)
    {
        var args = repositoryPath is null
            ? new[] { "config", "--global", "user.name", userName }
            : new[] { "config", "--local", "user.name", userName };
        var result = await RunGitCommandAsync(args, repositoryPath);
        if (result.ExitCode != 0)
            throw new InvalidOperationException($"Falha ao definir user.name: {result.Error}");
    }

    public async Task SetEmailAsync(string email, string? repositoryPath = null)
    {
        var args = repositoryPath is null
            ? new[] { "config", "--global", "user.email", email }
            : new[] { "config", "--local", "user.email", email };
        var result = await RunGitCommandAsync(args, repositoryPath);
        if (result.ExitCode != 0)
            throw new InvalidOperationException($"Falha ao definir user.email: {result.Error}");
    }

    public async Task<string> GetFullConfigAsync()
    {
        var result = await RunGitCommandAsync(["config", "--list", "--show-origin"]);
        if (result.ExitCode != 0)
            return $"Erro ao ler configuração: {result.Error}";

        return result.Output;
    }

    private GitCommandResult RunGitCommand(string[] arguments, string? workingDirectory = null)
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = _gitExecutable,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            if (workingDirectory is not null)
                startInfo.WorkingDirectory = workingDirectory;

            foreach (var arg in arguments)
                startInfo.ArgumentList.Add(arg);

            using var process = new Process { StartInfo = startInfo };

            process.Start();
            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            return new GitCommandResult(process.ExitCode, output, error);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Falha ao executar git: {ex.Message}", ex);
        }
    }

    private async Task<GitCommandResult> RunGitCommandAsync(string[] arguments, string? workingDirectory = null)
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = _gitExecutable,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            if (workingDirectory is not null)
                startInfo.WorkingDirectory = workingDirectory;

            foreach (var arg in arguments)
                startInfo.ArgumentList.Add(arg);

            using var process = new Process { StartInfo = startInfo };

            process.Start();
            var output = await process.StandardOutput.ReadToEndAsync();
            var error = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            return new GitCommandResult(process.ExitCode, output, error);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Falha ao executar git: {ex.Message}", ex);
        }
    }

    private record GitCommandResult(int ExitCode, string Output, string Error);
}
