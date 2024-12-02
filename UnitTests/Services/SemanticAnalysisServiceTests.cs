using Common.Options;
using LanguageDetection;
using Microsoft.Extensions.Options;
using ProfanityFilter.Interfaces;
using Service;

namespace UnitTests.Services;

public class SemanticAnalysisServiceTests
{
    private readonly LanguageDetector _languageDetector = Substitute.For<LanguageDetector>();
    private readonly IProfanityFilter _profanityFilter = Substitute.For<IProfanityFilter>();
    private readonly SemanticAnalysisOptions _options = new();
    private readonly SemanticAnalysisService _service;

    public SemanticAnalysisServiceTests()
    {
        _service = new SemanticAnalysisService(_languageDetector, _profanityFilter, Options.Create(_options));
    }

    [Fact]
    public void Analyze_TextWithProfanities_CensorsTextAndSetsPunishment()
    {
        // Arrange
        _options.CheckProfanity = true;
        _options.DetectLanguage = false;
        var text = "This is a test with profanity.";
        var profanities = new List<string> { "profanity" };
        _profanityFilter.DetectAllProfanities(text).Returns(profanities.AsReadOnly());
        _profanityFilter.CensorString(text).Returns("This is a test with *****.");

        // Act
        var result = _service.Analyze(text);

        // Assert
        result.DoPunish.Should().BeTrue();
        result.PunishmentExplanation.Should().Be($"Text content contains {profanities.Count} profanities");
        result.CensoredText.Should().Be("This is a test with *****.");
    }

    [Fact]
    public void Analyze_TextWithoutProfanities_DoesNotCensorText()
    {
        // Arrange
        _options.CheckProfanity = true;
        const string text = "This is a clean text.";
        _profanityFilter.DetectAllProfanities(text).Returns(new List<string>().AsReadOnly());

        // Act
        var result = _service.Analyze(text);

        // Assert
        result.DoPunish.Should().BeFalse();
        result.PunishmentExplanation.Should().BeNull();
        result.CensoredText.Should().BeNull();
        _profanityFilter.Received(1).DetectAllProfanities(text);
    }

    [Fact]
    public void Analyze_TextWithDisabledLanguageDetection_DoesNotDetectLanguage()
    {
        // Arrange
        _options.DetectLanguage = false;
        var options = Options.Create(_options);
        var serviceWithoutLanguageDetection = new SemanticAnalysisService(_languageDetector, _profanityFilter, options);
        var text = "This text should not detect language.";

        _profanityFilter.DetectAllProfanities(text).Returns(new List<string>().AsReadOnly());

        // Act
        var result = serviceWithoutLanguageDetection.Analyze(text);

        // Assert
        _languageDetector.DidNotReceiveWithAnyArgs().Detect(default);
        result.Language.Should().BeNull();
        result.DoPunish.Should().BeFalse();
    }
}