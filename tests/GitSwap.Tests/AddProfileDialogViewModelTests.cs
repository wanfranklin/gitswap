using GitSwap.ViewModels;

namespace GitSwap.Tests;

public class AddProfileDialogViewModelTests
{
    [Fact]
    public void IsValid_WithAllFields_ReturnsTrue()
    {
        var vm = new AddProfileDialogViewModel
        {
            ProfileName = "Work",
            UserName = "John",
            Email = "john@example.com"
        };
        Assert.True(vm.IsValid());
    }

    [Fact]
    public void IsValid_WithEmptyProfileName_ReturnsFalse()
    {
        var vm = new AddProfileDialogViewModel
        {
            ProfileName = "",
            UserName = "John",
            Email = "john@example.com"
        };
        Assert.False(vm.IsValid());
    }

    [Fact]
    public void IsValid_WithEmptyUserName_ReturnsFalse()
    {
        var vm = new AddProfileDialogViewModel
        {
            ProfileName = "Work",
            UserName = "",
            Email = "john@example.com"
        };
        Assert.False(vm.IsValid());
    }

    [Fact]
    public void IsValid_WithEmptyEmail_ReturnsFalse()
    {
        var vm = new AddProfileDialogViewModel
        {
            ProfileName = "Work",
            UserName = "John",
            Email = ""
        };
        Assert.False(vm.IsValid());
    }

    [Fact]
    public void IsValid_WithWhitespaceOnly_ReturnsFalse()
    {
        var vm = new AddProfileDialogViewModel
        {
            ProfileName = "  ",
            UserName = "  ",
            Email = "  "
        };
        Assert.False(vm.IsValid());
    }
}
