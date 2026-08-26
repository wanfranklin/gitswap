using System.Text.RegularExpressions;
using GitSwap.Services;

namespace GitSwap.ViewModels;

public class AddProfileDialogViewModel : ViewModelBase
{
    private string _profileName = string.Empty;
    private string _userName = string.Empty;
    private string _email = string.Empty;
    private string _repositoryPath = string.Empty;
    private string _errorMessage = string.Empty;

    public string ProfileName
    {
        get => _profileName;
        set
        {
            SetProperty(ref _profileName, value);
            OnPropertyChanged(nameof(CanSave));
        }
    }

    public string UserName
    {
        get => _userName;
        set
        {
            SetProperty(ref _userName, value);
            OnPropertyChanged(nameof(CanSave));
        }
    }

    public string Email
    {
        get => _email;
        set
        {
            SetProperty(ref _email, value);
            OnPropertyChanged(nameof(CanSave));
        }
    }

    public string RepositoryPath
    {
        get => _repositoryPath;
        set
        {
            SetProperty(ref _repositoryPath, value);
            OnPropertyChanged(nameof(CanSave));
        }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public bool CanSave => IsValid() && string.IsNullOrEmpty(Validate());

    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(ProfileName)
               && !string.IsNullOrWhiteSpace(UserName)
               && !string.IsNullOrWhiteSpace(Email);
    }

    public string? Validate()
    {
        if (string.IsNullOrWhiteSpace(ProfileName))
            return "Nome do perfil é obrigatório.";

        if (string.IsNullOrWhiteSpace(UserName))
            return "Nome do usuário é obrigatório.";

        if (string.IsNullOrWhiteSpace(Email))
            return "Email é obrigatório.";

        if (!Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            return "Email inválido.";

        if (UserName.Any(c => char.IsWhiteSpace(c)))
            return "Nome do usuário não pode conter espaços.";

        if (!string.IsNullOrWhiteSpace(RepositoryPath) &&
            !ProfileStorageService.IsValidRepositoryPath(RepositoryPath))
            return "Caminho do repositório inválido.";

        return null;
    }
}
