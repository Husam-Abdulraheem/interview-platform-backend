using FluentAssertions;
using InterviewPlatform.Application.DTOs;
using InterviewPlatform.Application.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Text;
using Xunit;
using System.Text.Json;

namespace InterviewPlatform.Tests.Services;

public class GeminiEvaluationServiceTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpHandler;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly HttpClient _httpClient;
    private readonly GeminiEvaluationService _geminiService;

    public GeminiEvaluationServiceTests()
    {
        _mockHttpHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpHandler.Object);
        _mockConfiguration = new Mock<IConfiguration>();

        _mockConfiguration.Setup(c => c["Gemini:ApiKey"]).Returns("test-api-key");

        _geminiService = new GeminiEvaluationService(_httpClient, _mockConfiguration.Object);
    }

    [Fact]
    public async Task EvaluateAnswerAsync_WithValidResponse_ReturnsEvaluationResult()
    {
        // Arrange
        var questionContent = "What is C#?";
        var traineeAnswer = "C# is a modern, object-oriented programming language developed by Microsoft.";

        var expectedResponse = new
        {
            candidates = new[]
            {
                new
                {
                    content = new
                    {
                        parts = new[]
                        {
                            new
                            {
                                text = JsonSerializer.Serialize(new
                                {
                                    score = 85,
                                    strengths = "Good definition of C#",
                                    weaknesses = "Could mention more specific features",
                                    suggestions = "Consider mentioning .NET framework, type safety, and garbage collection"
                                })
                            }
                        }
                    }
                }
            }
        };

        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(expectedResponse), Encoding.UTF8, "application/json")
        };

        _mockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _geminiService.EvaluateAnswerAsync(questionContent, traineeAnswer);

        // Assert
        result.Should().NotBeNull();
        result.Score.Should().Be(85);
        result.Strengths.Should().Be("Good definition of C#");
        result.Weaknesses.Should().Be("Could mention more specific features");
        result.Suggestions.Should().Be("Consider mentioning .NET framework, type safety, and garbage collection");
    }

    [Fact]
    public async Task EvaluateAnswerAsync_WithMissingApiKey_ThrowsInvalidOperationException()
    {
        // Arrange
        _mockConfiguration.Setup(c => c["Gemini:ApiKey"]).Returns("");

        var service = new GeminiEvaluationService(_httpClient, _mockConfiguration.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.EvaluateAnswerAsync("Test question", "Test answer"));
    }

    [Fact]
    public async Task EvaluateAnswerAsync_WithInvalidResponse_ReturnsDefaultResult()
    {
        // Arrange
        var invalidResponse = new { invalid = "response" };

        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(invalidResponse), Encoding.UTF8, "application/json")
        };

        _mockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _geminiService.EvaluateAnswerAsync("Test question", "Test answer");

        // Assert
        result.Should().NotBeNull();
        result.Score.Should().Be(0);
        result.Strengths.Should().BeEmpty();
        result.Weaknesses.Should().Be("Failed to parse AI response.");
        result.Suggestions.Should().BeEmpty();
    }

    [Fact]
    public async Task EvaluateAnswerAsync_WithHttpError_ReturnsDefaultResult()
    {
        // Arrange
        var httpResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);

        _mockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(
            () => _geminiService.EvaluateAnswerAsync("Test question", "Test answer"));
    }

    [Fact]
    public async Task EvaluateAnswerAsync_VerifyRequestFormat()
    {
        // Arrange
        var questionContent = "What is polymorphism?";
        var traineeAnswer = "Polymorphism allows objects to take multiple forms.";

        var expectedResponse = new
        {
            candidates = new[]
            {
                new
                {
                    content = new
                    {
                        parts = new[]
                        {
                            new { text = "{\"score\":90,\"strengths\":\"Excellent\",\"weaknesses\":\"None\",\"suggestions\":\"Add examples\"}" }
                        }
                    }
                }
            }
        };

        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(expectedResponse), Encoding.UTF8, "application/json")
        };

        HttpRequestMessage? capturedRequest = null;

        _mockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((request, _) => capturedRequest = request)
            .ReturnsAsync(httpResponse);

        // Act
        await _geminiService.EvaluateAnswerAsync(questionContent, traineeAnswer);

        // Assert
        capturedRequest.Should().NotBeNull();
        capturedRequest!.Method.Should().Be(HttpMethod.Post);
        capturedRequest.RequestUri!.ToString().Should().Contain("generativelanguage.googleapis.com");
        capturedRequest.RequestUri!.ToString().Should().Contain("test-api-key");

        var requestBody = await capturedRequest.Content!.ReadAsStringAsync();
        requestBody.Should().Contain("Evaluate the answer for the question");
        requestBody.Should().Contain("suggestions");
        requestBody.Should().Contain(questionContent);
        requestBody.Should().Contain(traineeAnswer);
    }
}
