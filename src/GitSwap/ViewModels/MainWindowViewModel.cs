using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GitSwap.Models;
using GitSwap.Services;
using GitSwap.Views;

namespace GitSwap.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly ProfileStorageService _storageService;
    private readonly GitService _gitService;

    [ObservableProperty]
    private ObservableCollection<GitProfile> _profiles = [];

    [ObservableProperty]
    private string _currentUserName = string.Empty;

    [ObservableProperty]
    private string _currentEmail = string.Empty;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private string _currentScope = "Global";

    [ObservableProperty]
    private string _searchText = string.Empty;

    public int ProfileCount => Profiles.Count;

    public ObservableCollection<GitProfile> FilteredProfiles
    {
        get
        {
            if (string.IsNullOrWhiteSpace(SearchText))
                return Profiles;

            var query = SearchText.ToLowerInvariant();
            return new ObservableCollection<GitProfile>(
                Profiles.Where(p =>
                    p.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    p.UserName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    p.Email.Contains(query, StringComparison.OrdinalIgnoreCase)));
        }
    }

    public string FilteredProfileCount
    {
        get
        {
            if (string.IsNullOrWhiteSpace(SearchText))
                return Profiles.Count.ToString();

            return $"{FilteredProfiles.Count}/{Profiles.Count}";
        }
    }

    partial void OnProfilesChanged(ObservableCollection<GitProfile> value)
    {
        OnPropertyChanged(nameof(ProfileCount));
        OnPropertyChanged(nameof(FilteredProfiles));
    }

    partial void OnSearchTextChanged(string value)
    {
        OnPropertyChanged(nameof(FilteredProfiles));
        OnPropertyChanged(nameof(FilteredProfileCount));
    }

    public MainWindowViewModel()
    {
        _storageService = new ProfileStorageService();
        _gitService = new GitService();
        LoadProfiles();
        _ = LoadCurrentGitConfigAsync();
    }

    public MainWindowViewModel(ProfileStorageService storageService, GitService gitService)
    {
        _storageService = storageService;
        _gitService = gitService;
        LoadProfiles();
        _ = LoadCurrentGitConfigAsync();
    }

    private async Task LoadCurrentGitConfigAsync()
    {
        if (!_gitService.IsGitInstalled())
        {
            StatusMessage = "Git não está instalado.";
            return;
        }

        try
        {
            var (name, email) = await _gitService.GetCurrentConfigAsync();
            CurrentUserName = name;
            CurrentEmail = email;

            await IdentifyActiveGlobalProfileAsync();

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(email))
            {
                var existing = Profiles.FirstOrDefault(p =>
                    p.UserName.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                    p.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

                StatusMessage = existing is not null
                    ? $"Conta ativa: {existing.Name}"
                    : "Configuração Git carregada.";
            }
            else
            {
                StatusMessage = "Nenhuma conta Git configurada.";
            }
        }
        catch
        {
            StatusMessage = "Não foi possível ler a configuração do Git.";
        }
    }

    private async Task IdentifyActiveGlobalProfileAsync()
    {
        try
        {
            var (globalName, globalEmail) = await _gitService.GetGlobalConfigAsync();

            foreach (var profile in Profiles)
            {
                profile.IsActiveGlobal = false;
            }

            if (!string.IsNullOrEmpty(globalName) && !string.IsNullOrEmpty(globalEmail))
            {
                var matchingProfile = Profiles.FirstOrDefault(p =>
                    p.UserName.Equals(globalName, StringComparison.OrdinalIgnoreCase) &&
                    p.Email.Equals(globalEmail, StringComparison.OrdinalIgnoreCase));

                if (matchingProfile is not null)
                {
                    matchingProfile.IsActiveGlobal = true;
                }
            }

            Profiles = new ObservableCollection<GitProfile>(Profiles);
        }
        catch
        {
            foreach (var profile in Profiles)
            {
                profile.IsActiveGlobal = false;
            }
            Profiles = new ObservableCollection<GitProfile>(Profiles);
        }
    }

    [RelayCommand]
    private async Task AddProfileAsync()
    {
        var dialog = new AddProfileDialog
        {
            DataContext = new AddProfileDialogViewModel()
        };
        var mainWindow = App.Current?.ApplicationLifetime is
            Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow : null;

        if (mainWindow is null) return;

        var result = await dialog.ShowDialog<bool?>(mainWindow);

        if (result is true && dialog.Result is not null)
        {
            Profiles.Add(dialog.Result);
            _storageService.SaveProfiles([.. Profiles]);
            StatusMessage = $"Conta '{dialog.Result.Name}' criada.";
        }
    }

    [RelayCommand]
    private async Task ExportProfilesAsync()
    {
        var mainWindow = App.Current?.ApplicationLifetime is
            Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow : null;

        if (mainWindow is null) return;

        var provider = mainWindow.StorageProvider;
        var file = await provider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Exportar Perfis",
            SuggestedFileName = "gitswap-profiles.json",
            FileTypeChoices =
            [
                new FilePickerFileType("JSON") { Patterns = ["*.json"] }
            ]
        });

        if (file is null) return;

        try
        {
            var json = JsonSerializer.Serialize(Profiles.ToList(), new JsonSerializerOptions
            {
                WriteIndented = true
            });
            await File.WriteAllTextAsync(file.Path.LocalPath, json);
            StatusMessage = $"{Profiles.Count} perfil(is) exportado(s) com sucesso.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Erro ao exportar: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task ImportProfilesAsync()
    {
        var mainWindow = App.Current?.ApplicationLifetime is
            Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow : null;

        if (mainWindow is null) return;

        var provider = mainWindow.StorageProvider;
        var files = await provider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Importar Perfis",
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType("JSON") { Patterns = ["*.json"] }
            ]
        });

        if (files.Count == 0) return;

        try
        {
            var json = await File.ReadAllTextAsync(files[0].Path.LocalPath);
            var imported = JsonSerializer.Deserialize<List<GitProfile>>(json);

            if (imported is null || imported.Count == 0)
            {
                StatusMessage = "Nenhum perfil encontrado no arquivo.";
                return;
            }

            var existing = Profiles.Select(p =>
                p.UserName.ToLowerInvariant() + "|" + p.Email.ToLowerInvariant()).ToHashSet();

            var added = 0;
            foreach (var profile in imported)
            {
                var key = profile.UserName.ToLowerInvariant() + "|" + profile.Email.ToLowerInvariant();
                if (!existing.Contains(key))
                {
                    Profiles.Add(profile);
                    existing.Add(key);
                    added++;
                }
            }

            _storageService.SaveProfiles([.. Profiles]);
            StatusMessage = added > 0
                ? $"{added} perfil(is) importado(s) com sucesso."
                : "Todos os perfis ja existem.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Erro ao importar: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task ShowAboutAsync()
    {
        var dialog = new AboutDialog();
        var mainWindow = App.Current?.ApplicationLifetime is
            Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow : null;

        if (mainWindow is not null)
            await dialog.ShowDialog(mainWindow);
    }

    [RelayCommand]
    private void OpenDonation()
    {
        var url = "https://wanfranklin.com.br/apoio";
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        catch
        {
            StatusMessage = $"Acesse: {url}";
        }
    }

    [RelayCommand]
    private async Task ShowHelpAsync()
    {
        var dialog = new HelpDialog();
        var mainWindow = App.Current?.ApplicationLifetime is
            Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow : null;

        if (mainWindow is not null)
            await dialog.ShowDialog(mainWindow);
    }

    [RelayCommand]
    private async Task ShowGitConfigAsync()
    {
        var mainWindow = App.Current?.ApplicationLifetime is
            Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow : null;

        if (mainWindow is null) return;

        var config = await _gitService.GetFullConfigAsync();

        var dialog = new GitConfigDialog();
        dialog.SetConfigText(config);

        await dialog.ShowDialog(mainWindow);
    }

    [RelayCommand]
    private void Exit()
    {
        if (App.Current?.ApplicationLifetime is
            Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
    }

    [RelayCommand]
    private async Task DeleteProfileAsync(GitProfile? profile)
    {
        if (profile is null) return;

        var confirmed = await ShowConfirmMessage($"Tem certeza que deseja excluir o perfil '{profile.Name}'?");
        if (!confirmed) return;

        Profiles.Remove(profile);
        _storageService.SaveProfiles([.. Profiles]);
        StatusMessage = $"'{profile.Name}' removida.";
    }

    [RelayCommand]
    private void MoveProfileUp(GitProfile? profile)
    {
        if (profile is null) return;

        var index = Profiles.IndexOf(profile);
        if (index <= 0) return;

        Profiles.Move(index, index - 1);
        _storageService.SaveProfiles([.. Profiles]);
    }

    [RelayCommand]
    private void MoveProfileDown(GitProfile? profile)
    {
        if (profile is null) return;

        var index = Profiles.IndexOf(profile);
        if (index < 0 || index >= Profiles.Count - 1) return;

        Profiles.Move(index, index + 1);
        _storageService.SaveProfiles([.. Profiles]);
    }

    private async Task<bool> ShowConfirmMessage(string message)
    {
        var grid = new Grid
        {
            RowDefinitions = new RowDefinitions("*,Auto")
        };

        var textBlock = new TextBlock
        {
            Text = message,
            FontSize = 15,
            Foreground = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#475569")),
            TextWrapping = Avalonia.Media.TextWrapping.Wrap,
            Margin = new Avalonia.Thickness(24, 24, 24, 0),
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
        };
        Grid.SetRow(textBlock, 0);
        grid.Children.Add(textBlock);

        var buttonPanel = new StackPanel
        {
            Orientation = Avalonia.Layout.Orientation.Horizontal,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            Spacing = 12
        };

        var cancelBtn = new Button
        {
            Content = "Cancelar",
            Padding = new Avalonia.Thickness(24, 12),
            FontSize = 14,
            FontWeight = Avalonia.Media.FontWeight.SemiBold,
            CornerRadius = new Avalonia.CornerRadius(10),
            Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#FEE2E2")),
            Foreground = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#DC2626"))
        };

        var confirmBtn = new Button
        {
            Content = "Excluir",
            Padding = new Avalonia.Thickness(24, 12),
            FontSize = 14,
            FontWeight = Avalonia.Media.FontWeight.SemiBold,
            CornerRadius = new Avalonia.CornerRadius(10),
            Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#DC2626")),
            Foreground = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#FFFFFF"))
        };

        buttonPanel.Children.Add(cancelBtn);
        buttonPanel.Children.Add(confirmBtn);

        var buttonBorder = new Border
        {
            Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#FFFFFF")),
            Padding = new Avalonia.Thickness(24, 16, 24, 24),
            Child = buttonPanel
        };
        Grid.SetRow(buttonBorder, 1);
        grid.Children.Add(buttonBorder);

        var result = false;
        var messageBox = new Window
        {
            Title = "Confirmar Exclusão",
            Width = 400,
            Height = 200,
            CanResize = false,
            WindowStartupLocation = Avalonia.Controls.WindowStartupLocation.CenterOwner,
            Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#F1F5F9")),
            Content = grid
        };

        cancelBtn.Click += (_, _) => messageBox.Close();
        confirmBtn.Click += (_, _) => { result = true; messageBox.Close(); };

        var mainWindow = App.Current?.ApplicationLifetime is
            Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow : null;

        if (mainWindow is not null)
            await messageBox.ShowDialog(mainWindow);

        return result;
    }

    [RelayCommand]
    private async Task EditProfileAsync(GitProfile? profile)
    {
        if (profile is null) return;

        var dialog = new EditProfileDialog();
        dialog.SetData(profile.Name, profile.UserName, profile.Email, profile.RepositoryPath);

        var mainWindow = App.Current?.ApplicationLifetime is
            Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow : null;

        if (mainWindow is null) return;

        var result = await dialog.ShowDialog<bool?>(mainWindow);

        if (result is true && dialog.Result is not null)
        {
            var index = Profiles.IndexOf(profile);
            if (index >= 0)
            {
                dialog.Result.Id = profile.Id;
                Profiles[index] = dialog.Result;
                _storageService.SaveProfiles([.. Profiles]);
                StatusMessage = $"Conta '{dialog.Result.Name}' atualizada.";
            }
        }
    }

    [RelayCommand]
    private async Task CloneProfileAsync(GitProfile? profile)
    {
        if (profile is null) return;

        var cloned = profile.Clone();
        Profiles.Add(cloned);
        _storageService.SaveProfiles([.. Profiles]);
        StatusMessage = $"Perfil '{profile.Name}' clonado como '{cloned.Name}'.";

        var dialog = new EditProfileDialog();
        dialog.SetData(cloned.Name, cloned.UserName, cloned.Email, cloned.RepositoryPath);

        var mainWindow = App.Current?.ApplicationLifetime is
            Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow : null;

        if (mainWindow is null) return;

        var result = await dialog.ShowDialog<bool?>(mainWindow);

        if (result is true && dialog.Result is not null)
        {
            var index = Profiles.IndexOf(cloned);
            if (index >= 0)
            {
                dialog.Result.Id = cloned.Id;
                Profiles[index] = dialog.Result;
                _storageService.SaveProfiles([.. Profiles]);
                StatusMessage = $"Perfil '{dialog.Result.Name}' atualizado.";
            }
        }
    }

    [RelayCommand]
    private async Task SwitchProfileAsync(GitProfile? profile)
    {
        if (profile is null || !profile.IsValid())
        {
            StatusMessage = "Perfil inválido. Preencha nome e email.";
            return;
        }

        if (!string.IsNullOrWhiteSpace(profile.RepositoryPath) &&
            !ProfileStorageService.IsValidRepositoryPath(profile.RepositoryPath))
        {
            StatusMessage = "Caminho do repositório inválido.";
            return;
        }

        if (!_gitService.IsGitInstalled())
        {
            StatusMessage = "Git não está instalado.";
            return;
        }

        var mainWindow = App.Current?.ApplicationLifetime is
            Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow : null;

        if (mainWindow is null) return;

        var commands = BuildCommandsPreview(profile);

        var vm = new ConfirmSwitchViewModel
        {
            ProfileName = profile.Name,
            ProfileUserName = profile.UserName,
            ProfileEmail = profile.Email,
            CommandsPreview = commands
        };

        var dialog = new ConfirmSwitchDialog { DataContext = vm };
        var result = await dialog.ShowDialog<bool?>(mainWindow);

        if (result is not true) return;

        try
        {
            bool hasFolder = !string.IsNullOrWhiteSpace(profile.RepositoryPath)
                             && Directory.Exists(profile.RepositoryPath);
            bool isGitRepo = hasFolder && _gitService.IsGitRepository(profile.RepositoryPath);

            if (isGitRepo)
            {
                await _gitService.SetUserNameAsync(profile.UserName, profile.RepositoryPath);
                await _gitService.SetEmailAsync(profile.Email, profile.RepositoryPath);

                var (name, email) = await _gitService.GetCurrentConfigAsync(profile.RepositoryPath);
                CurrentUserName = name;
                CurrentEmail = email;
                CurrentScope = "Local";

                foreach (var p in Profiles)
                {
                    p.IsActiveGlobal = false;
                }
                OnPropertyChanged(nameof(Profiles));

                StatusMessage = $"Conta '{profile.Name}' ativa em {Path.GetFileName(profile.RepositoryPath)}";
            }
            else
            {
                await _gitService.SetUserNameAsync(profile.UserName);
                await _gitService.SetEmailAsync(profile.Email);

                CurrentUserName = profile.UserName;
                CurrentEmail = profile.Email;
                CurrentScope = "Global";

                foreach (var p in Profiles)
                {
                    p.IsActiveGlobal = false;
                }
                profile.IsActiveGlobal = true;
                OnPropertyChanged(nameof(Profiles));

                if (hasFolder)
                    StatusMessage = $"Conta '{profile.Name}' ativa (global) — pasta: {Path.GetFileName(profile.RepositoryPath)}";
                else
                    StatusMessage = $"Conta '{profile.Name}' ativa (global)";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Erro: {ex.Message}";
        }
    }

    private static string BuildCommandsPreview(GitProfile profile)
    {
        var lines = new List<string>();
        bool hasFolder = !string.IsNullOrWhiteSpace(profile.RepositoryPath)
                         && Directory.Exists(profile.RepositoryPath);

        if (hasFolder)
        {
            lines.Add($"cd {profile.RepositoryPath}");
            lines.Add($"git config user.name \"{profile.UserName}\"");
            lines.Add($"git config user.email \"{profile.Email}\"");
        }
        else
        {
            lines.Add($"git config --global user.name \"{profile.UserName}\"");
            lines.Add($"git config --global user.email \"{profile.Email}\"");
        }

        return string.Join("\n", lines);
    }

    private void LoadProfiles()
    {
        Profiles = new ObservableCollection<GitProfile>(_storageService.LoadProfiles());
    }
}
