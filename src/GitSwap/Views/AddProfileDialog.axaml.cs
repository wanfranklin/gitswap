using Avalonia.Controls;
using Avalonia.Platform.Storage;
using GitSwap.Models;
using GitSwap.Services;
using GitSwap.ViewModels;

namespace GitSwap.Views;

public partial class AddProfileDialog : Window
{
    public GitProfile? Result { get; private set; }

    public AddProfileDialog()
    {
        InitializeComponent();

        var browseButton = this.FindControl<Button>("BrowseButton");
        if (browseButton is not null)
            browseButton.Click += BrowseClick;

        var saveButton = this.FindControl<Button>("SaveButton");
        if (saveButton is not null)
            saveButton.Click += SaveClick;

        var cancelButton = this.FindControl<Button>("CancelButton");
        if (cancelButton is not null)
            cancelButton.Click += (_, _) => Close(false);
    }

    private async void BrowseClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null) return;

        var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Selecionar pasta do repositório",
            AllowMultiple = false
        });

        if (folders.Count > 0)
        {
            var path = folders[0].TryGetLocalPath();
            if (path is not null && DataContext is AddProfileDialogViewModel vm)
            {
                vm.RepositoryPath = path;
            }
        }
    }

    private async void SaveClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is not AddProfileDialogViewModel vm)
            return;

        if (!vm.IsValid())
        {
            await ShowMessage("Preencha todos os campos obrigatórios:\n\n- Nome do perfil\n- Nome do usuário Git\n- Email");
            return;
        }

        if (!string.IsNullOrWhiteSpace(vm.RepositoryPath) && !Directory.Exists(vm.RepositoryPath))
        {
            await ShowMessage("A pasta selecionada não existe.\n\nSelecione uma pasta válida ou deixe o campo vazio.");
            return;
        }

        Result = new GitProfile
        {
            Name = vm.ProfileName,
            UserName = vm.UserName,
            Email = vm.Email,
            RepositoryPath = vm.RepositoryPath
        };

        Close(true);
    }

    private async Task ShowMessage(string message)
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

        var okButton = new Button
        {
            Content = "OK",
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            Padding = new Avalonia.Thickness(40, 12),
            FontSize = 15,
            FontWeight = Avalonia.Media.FontWeight.SemiBold,
            CornerRadius = new Avalonia.CornerRadius(10),
            Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#2563EB")),
            Foreground = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#FFFFFF"))
        };

        var buttonBorder = new Border
        {
            Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#FFFFFF")),
            Padding = new Avalonia.Thickness(24, 16, 24, 16),
            Child = okButton
        };
        Grid.SetRow(buttonBorder, 1);
        grid.Children.Add(buttonBorder);

        var messageBox = new Window
        {
            Title = "Aviso",
            Width = 400,
            Height = 200,
            CanResize = false,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#F1F5F9")),
            Content = grid
        };

        okButton.Click += (_, _) => messageBox.Close();

        await messageBox.ShowDialog(this);
    }
}
