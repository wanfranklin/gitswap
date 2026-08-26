using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace GitSwap.ViewModels;

public partial class ConfirmSwitchViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _profileName = string.Empty;

    [ObservableProperty]
    private string _profileUserName = string.Empty;

    [ObservableProperty]
    private string _profileEmail = string.Empty;

    [ObservableProperty]
    private string _commandsPreview = string.Empty;

    [RelayCommand]
    private void Confirm()
    {
        if (App.Current?.ApplicationLifetime is
            Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
        {
            var dialog = desktop.Windows.OfType<Views.ConfirmSwitchDialog>().FirstOrDefault();
            dialog?.Close(true);
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        if (App.Current?.ApplicationLifetime is
            Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
        {
            var dialog = desktop.Windows.OfType<Views.ConfirmSwitchDialog>().FirstOrDefault();
            dialog?.Close(false);
        }
    }
}
