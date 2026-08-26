using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace GitSwap.Models;

public class GitProfile : INotifyPropertyChanged
{
    private string _id = Guid.NewGuid().ToString();
    private string _name = string.Empty;
    private string _userName = string.Empty;
    private string _email = string.Empty;
    private string _repositoryPath = string.Empty;
    private bool _isActiveGlobal;

    public string Id
    {
        get => _id;
        set => SetField(ref _id, value);
    }

    public string Name
    {
        get => _name;
        set => SetField(ref _name, value);
    }

    public string UserName
    {
        get => _userName;
        set => SetField(ref _userName, value);
    }

    public string Email
    {
        get => _email;
        set => SetField(ref _email, value);
    }

    public string RepositoryPath
    {
        get => _repositoryPath;
        set => SetField(ref _repositoryPath, value);
    }

    [JsonIgnore]
    public bool IsActiveGlobal
    {
        get => _isActiveGlobal;
        set => SetField(ref _isActiveGlobal, value);
    }

    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(UserName)
               && !string.IsNullOrWhiteSpace(Email);
    }

    [JsonIgnore]
    public bool HasRepository => !string.IsNullOrWhiteSpace(RepositoryPath);

    [JsonIgnore]
    public bool IsGlobal => !HasRepository;

    public string GetRepositoryDisplayName()
    {
        if (string.IsNullOrWhiteSpace(RepositoryPath))
            return string.Empty;
        return Path.GetFileName(RepositoryPath);
    }

    public GitProfile Clone()
    {
        return new GitProfile
        {
            Name = $"{Name} (Copia)",
            UserName = UserName,
            Email = Email,
            RepositoryPath = RepositoryPath
        };
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
