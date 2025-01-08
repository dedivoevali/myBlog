using System.Runtime.InteropServices;
using Common.Options;
using Common.Validation;
using Microsoft.Extensions.Options;
using Service.Auth;

namespace UnitTests.Services;

public class EncryptionServiceTests
{
    private readonly IOptions<JsonWebTokenOptions> _options;
    private readonly EncryptionService _subject;

    public EncryptionServiceTests()
    {
        _options = new OptionsWrapper<JsonWebTokenOptions>(new()
        {
            Key = "key",
            Issuer = "issuer",
            Audience = "audience2232",
            AccessTokenValidityTimeMinutes = 24
        });
        _subject = new EncryptionService(_options);
    }


    [Fact]
    public async Task GenerateNewUniqueRefreshToken_ShouldGenerateToken_Of_RefreshTokenLength()
    {
        // Arrange
        const int expectedLength = EntityConfigurationConstants.RefreshTokenLength;
        
        // Act
        var response = _subject.GenerateRefreshToken();

        // Assert
        response.Value.Length.Should().Be(expectedLength);
    }

    [Theory]
    [InlineData("password", "5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8")]
    [InlineData("lovemama123", "9bcd53793c43361386708990a5a7827140deb591910da5fd8649a9b81759ffa6")]
    public async Task EncryptPassword_ShouldEncrypt_Sha256(string input, string expected)
    {
        // Arrange

        // Act
        var actual = _subject.EncryptPassword(input);

        // Assert
        actual.Should().Be(expected);
    }
}