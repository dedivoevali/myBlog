using Common.Options;
using Common.Validation;
using DAL.Repositories.Abstract;
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
}