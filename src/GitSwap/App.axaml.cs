using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using GitSwap.ViewModels;
using GitSwap.Views;

namespace GitSwap;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };

            // Set window icon programmatically for macOS dock
            using var stream = AssetLoader.Open(new System.Uri("avares://GitSwap/Assets/icon.png"));
            if (stream is not null)
            {
                var bitmap = new Bitmap(stream);
                mainWindow.Icon = new WindowIcon(bitmap);
            }

            desktop.MainWindow = mainWindow;

            if (!IsGitInstalled())
            {
                ShowGitNotInstalledMessage(mainWindow);
            }
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static bool IsGitInstalled()
    {
        try
        {
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "git",
                Arguments = "--version",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using var process = System.Diagnostics.Process.Start(psi);
            if (process is null) return false;
            process.WaitForExit();
            return process.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }

    private static void ShowGitNotInstalledMessage(Window mainWindow)
    {
        var panel = new StackPanel
        {
            Spacing = 12,
            Margin = new Thickness(24)
        };

        var icon = new TextBlock
        {
            Text = "!",
            FontSize = 32,
            FontWeight = Avalonia.Media.FontWeight.Bold,
            Foreground = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#DC2626")),
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
        };

        var title = new TextBlock
        {
            Text = "Git não encontrado",
            FontSize = 18,
            FontWeight = Avalonia.Media.FontWeight.SemiBold,
            Foreground = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#111827"))
        };

        var message = new TextBlock
        {
            Text = "O Git não está instalado ou não foi encontrado no PATH do sistema.\n\nO GitSwap precisa do Git para funcionar. Instale o Git e reinicie o aplicativo.",
            FontSize = 14,
            Foreground = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#4B5563")),
            TextWrapping = Avalonia.Media.TextWrapping.Wrap
        };

        var linkButton = new Button
        {
            Content = "Baixar Git: git-scm.com",
            FontSize = 14,
            FontWeight = Avalonia.Media.FontWeight.SemiBold,
            Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#EFF6FF")),
            Foreground = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#3B82F6")),
            BorderBrush = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#BFDBFE")),
            BorderThickness = new Thickness(1),
            Padding = new Thickness(16, 10),
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left
        };

        linkButton.Click += (_, _) =>
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://git-scm.com",
                    UseShellExecute = true
                });
            }
            catch
            {
            }
        };

        var closeButton = new Button
        {
            Content = "Fechar",
            FontSize = 14,
            FontWeight = Avalonia.Media.FontWeight.SemiBold,
            Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#DC2626")),
            Foreground = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#FFFFFF")),
            BorderBrush = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#B91C1C")),
            BorderThickness = new Thickness(1),
            Padding = new Thickness(32, 10),
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
        };

        closeButton.Click += (_, _) =>
        {
            if (App.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.Shutdown();
        };

        var buttonPanel = new StackPanel
        {
            Orientation = Avalonia.Layout.Orientation.Horizontal,
            Spacing = 12,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
        };
        buttonPanel.Children.Add(linkButton);
        buttonPanel.Children.Add(closeButton);

        panel.Children.Add(icon);
        panel.Children.Add(title);
        panel.Children.Add(message);
        panel.Children.Add(buttonPanel);

        var dialog = new Window
        {
            Title = "GitSwap - Aviso",
            Width = 420,
            Height = 280,
            CanResize = false,
            WindowStartupLocation = Avalonia.Controls.WindowStartupLocation.CenterOwner,
            Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#FFFFFF")),
            Content = panel
        };

        dialog.ShowDialog(mainWindow);
    }
}