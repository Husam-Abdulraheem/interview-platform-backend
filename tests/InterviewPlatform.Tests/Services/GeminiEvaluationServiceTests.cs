using FluentAssertions;
using InterviewPlatform.Application.DTOs;
using InterviewPlatform.Application.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using System.Text.Json;

namespace InterviewPlatform.Tests.Services;

public class GeminiEvaluationServiceTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly GeminiEvaluationService _geminiService;

    public GeminiEvaluationServiceTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();

        _mockConfiguration.Setup(c => c["GEMINI_API_KEY"]).Returns("test-api-key");

        _geminiService = new GeminiEvaluationService(_mockConfiguration.Object);
    }

    [Fact]
    public void Constructor_WithValidConfiguration_CreatesService()
    {
        // Act
        var service = new GeminiEvaluationService(_mockConfiguration.Object);

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithMissingApiKey_CreatesService()
    {
        // Arrange
        _mockConfiguration.Setup(c => c["GEMINI_API_KEY"]).Returns("");

        // Act & Assert - Constructor doesn't throw, only EvaluateAnswerAsync does
        var service = new GeminiEvaluationService(_mockConfiguration.Object);
        service.Should().NotBeNull();
    }

    [Fact]
    public async Task EvaluateAnswerAsync_WithMissingApiKey_ThrowsInvalidOperationException()
    {
        // Arrange
        _mockConfiguration.Setup(c => c["GEMINI_API_KEY"]).Returns("");
        var service = new GeminiEvaluationService(_mockConfiguration.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.EvaluateAnswerAsync("Test question", "Test answer"));
    }
}
