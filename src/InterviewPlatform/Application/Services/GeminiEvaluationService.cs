using System.Net.Http.Json;
using System.Text.Json;
using InterviewPlatform.Application.DTOs;
using InterviewPlatform.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace InterviewPlatform.Application.Services;

public class GeminiEvaluationService : IAiEvaluationService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _modelName = "gemini-1.5-pro";

    public GeminiEvaluationService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["Gemini:ApiKey"] ?? string.Empty;
    }

    public async Task<AiEvaluationResultDto> EvaluateAnswerAsync(string questionContent, string traineeAnswer)
    {
        if (string.IsNullOrEmpty(_apiKey))
            throw new InvalidOperationException("Gemini API Key is missing.");

        // Construct standard prompt
        var prompt = $"Evaluate the answer for the question. Respond ONLY with a JSON object. Ensure the format adheres to:\n{{\n  \"score\": 0 to 100 integer,\n  \"strengths\": \"string describing strengths\",\n  \"weaknesses\": \"string describing weaknesses\",\n  \"suggestions\": \"string with actionable suggestions for improvement\"\n}}\n\nQuestion: {questionContent}\n\nAnswer: {traineeAnswer}";

        var requestBody = new
        {
            contents = new[]
            {
            new { parts = new[] { new { text = prompt } } }
        },
            generationConfig = new
            {
                responseMimeType = "application/json",
                temperature = 0.7,
                maxOutputTokens = 1000
            }
        };

        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_modelName}:generateContent?key={_apiKey}";

        try
        {
            var response = await _httpClient.PostAsJsonAsync(url, requestBody);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Gemini API Error ({response.StatusCode}): {errorContent}");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Gemini API Response: {jsonResponse}");

            using var jsonDoc = JsonDocument.Parse(jsonResponse);

            // Parse Google's response structure
            var root = jsonDoc.RootElement;

            if (root.TryGetProperty("candidates", out var candidates) && candidates.GetArrayLength() > 0)
            {
                var content = candidates[0].GetProperty("content");
                var parts = content.GetProperty("parts");
                if (parts.GetArrayLength() > 0)
                {
                    var textObj = parts[0].GetProperty("text").GetString();
                    if (!string.IsNullOrEmpty(textObj))
                    {
                        textObj = textObj.Replace("```json", "").Replace("```", "").Trim();

                        // Deserialize the strongly typed our expected JSON format
                        var evaluation = JsonSerializer.Deserialize<AiEvaluationResultDto>(textObj, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        if (evaluation != null)
                            return evaluation;
                    }
                }
            }

            return new AiEvaluationResultDto { Score = 0, Strengths = string.Empty, Weaknesses = "Failed to parse AI response.", Suggestions = string.Empty };
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Gemini API HTTP Error: {ex.Message}");
            throw new InvalidOperationException($"Failed to call Gemini API: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Gemini API Error: {ex.Message}");
            return new AiEvaluationResultDto { Score = 0, Strengths = string.Empty, Weaknesses = $"AI evaluation failed: {ex.Message}", Suggestions = string.Empty };
        }
    }
}
