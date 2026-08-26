using System.Text.Json;
using GitSwap.Models;

namespace GitSwap.Tests;

public class JsonSerializationTests
{
    [Fact]
    public void Serialize_Profile_ReturnsValidJson()
    {
        var profile = new GitProfile
        {
            Id = "test-id-123",
            Name = "Work",
            UserName = "John",
            Email = "john@work.com"
        };

        var json = JsonSerializer.Serialize(profile);

        Assert.Contains("\"Id\":\"test-id-123\"", json);
        Assert.Contains("\"Name\":\"Work\"", json);
        Assert.Contains("\"UserName\":\"John\"", json);
        Assert.Contains("\"Email\":\"john@work.com\"", json);
    }

    [Fact]
    public void Deserialize_ValidJson_ReturnsProfile()
    {
        var json = """
        {
            "Id": "test-id-123",
            "Name": "Work",
            "UserName": "John",
            "Email": "john@work.com"
        }
        """;

        var profile = JsonSerializer.Deserialize<GitProfile>(json);

        Assert.NotNull(profile);
        Assert.Equal("test-id-123", profile!.Id);
        Assert.Equal("Work", profile.Name);
        Assert.Equal("John", profile.UserName);
        Assert.Equal("john@work.com", profile.Email);
    }

    [Fact]
    public void Serialize_Deserialize_ListOfProfiles()
    {
        var profiles = new List<GitProfile>
        {
            new() { Name = "Work", UserName = "John", Email = "john@work.com" },
            new() { Name = "Personal", UserName = "John", Email = "john@gmail.com" }
        };

        var json = JsonSerializer.Serialize(profiles);
        var deserialized = JsonSerializer.Deserialize<List<GitProfile>>(json);

        Assert.NotNull(deserialized);
        Assert.Equal(2, deserialized!.Count);
        Assert.Equal("Work", deserialized[0].Name);
        Assert.Equal("Personal", deserialized[1].Name);
    }
}
