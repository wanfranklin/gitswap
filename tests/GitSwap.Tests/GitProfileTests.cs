using GitSwap.Models;

namespace GitSwap.Tests;

public class GitProfileTests
{
    [Fact]
    public void NewProfile_HasGeneratedId()
    {
        var profile = new GitProfile();
        Assert.False(string.IsNullOrEmpty(profile.Id));
    }

    [Fact]
    public void NewProfile_DefaultValues_AreEmpty()
    {
        var profile = new GitProfile();
        Assert.Equal(string.Empty, profile.Name);
        Assert.Equal(string.Empty, profile.UserName);
        Assert.Equal(string.Empty, profile.Email);
    }

    [Fact]
    public void IsValid_WithValidData_ReturnsTrue()
    {
        var profile = new GitProfile
        {
            UserName = "John",
            Email = "john@example.com"
        };
        Assert.True(profile.IsValid());
    }

    [Fact]
    public void IsValid_WithEmptyUserName_ReturnsFalse()
    {
        var profile = new GitProfile
        {
            UserName = "",
            Email = "john@example.com"
        };
        Assert.False(profile.IsValid());
    }

    [Fact]
    public void IsValid_WithEmptyEmail_ReturnsFalse()
    {
        var profile = new GitProfile
        {
            UserName = "John",
            Email = ""
        };
        Assert.False(profile.IsValid());
    }

    [Fact]
    public void IsValid_WithWhitespaceOnly_ReturnsFalse()
    {
        var profile = new GitProfile
        {
            UserName = "   ",
            Email = "   "
        };
        Assert.False(profile.IsValid());
    }

    [Fact]
    public void NewProfile_IsActiveGlobal_DefaultsToFalse()
    {
        var profile = new GitProfile();
        Assert.False(profile.IsActiveGlobal);
    }

    [Fact]
    public void IsActiveGlobal_WhenSetToTrue_ReturnsTrue()
    {
        var profile = new GitProfile
        {
            IsActiveGlobal = true
        };
        Assert.True(profile.IsActiveGlobal);
    }

    [Fact]
    public void IsGlobal_WithoutRepository_ReturnsTrue()
    {
        var profile = new GitProfile
        {
            RepositoryPath = ""
        };
        Assert.True(profile.IsGlobal);
    }

    [Fact]
    public void IsGlobal_WithRepository_ReturnsFalse()
    {
        var profile = new GitProfile
        {
            RepositoryPath = "/some/path"
        };
        Assert.False(profile.IsGlobal);
    }

    [Fact]
    public void DisplayName_WithLowercaseName_ReturnsCapitalized()
    {
        var profile = new GitProfile
        {
            Name = "joão da silva"
        };
        Assert.Equal("João da silva", profile.DisplayName);
    }

    [Fact]
    public void DisplayName_WithUppercaseName_ReturnsSame()
    {
        var profile = new GitProfile
        {
            Name = "João da Silva"
        };
        Assert.Equal("João da Silva", profile.DisplayName);
    }

    [Fact]
    public void DisplayName_WithEmptyName_ReturnsEmpty()
    {
        var profile = new GitProfile
        {
            Name = ""
        };
        Assert.Equal("", profile.DisplayName);
    }

    [Fact]
    public void DisplayName_WithWhitespaceName_ReturnsWhitespace()
    {
        var profile = new GitProfile
        {
            Name = "   "
        };
        Assert.Equal("   ", profile.DisplayName);
    }
}
