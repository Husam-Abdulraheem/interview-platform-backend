using System.Text.Json;
using InterviewPlatform.Application.DTOs;
using InterviewPlatform.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Google.GenAI;
using Google.GenAI.Types;
using Environment = System.Environment;

namespace InterviewPlatform.Application.Services;

public class GeminiEvaluationService : IAiEvaluationService
{
    private readonly string _apiKey;
    private readonly string _modelName = "gemini-2.5-flash"; 

    public GeminiEvaluationService(IConfiguration configuration)
    {
        _apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY")
                          ?? configuration["GEMINI_API_KEY"]
                          ?? string.Empty;
    }

    public async Task<AiEvaluationResultDto> EvaluateAnswerAsync(string questionContent, string traineeAnswer)
    {
        if (string.IsNullOrEmpty(_apiKey))
            throw new InvalidOperationException("Gemini API Key is missing.");

        var prompt = $"Evaluate the answer for the question. Respond ONLY with a JSON object. Ensure the format adheres to:\n{{\n  \"score\": 0 to 100 integer,\n  \"strengths\": \"string describing strengths\",\n  \"weaknesses\": \"string describing weaknesses\",\n  \"suggestions\": \"string with actionable suggestions for improvement\"\n}}\n\nQuestion: {questionContent}\n\nAnswer: {traineeAnswer}";

        try
        {
            var client = new Client(apiKey: _apiKey);

            var config = new GenerateContentConfig
            {
                ResponseMimeType = "application/json",
                Temperature = 0.7f,
                MaxOutputTokens = 1000
            };

            var response = await client.Models.GenerateContentAsync(
                model: _modelName,
                contents: prompt,
                config: config
            );

            var textObj = response.Text;

            if (!string.IsNullOrEmpty(textObj))
            {
                textObj = textObj.Replace("```json", "").Replace("```", "").Trim();

                var evaluation = JsonSerializer.Deserialize<AiEvaluationResultDto>(
                    textObj,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (evaluation != null)
                    return evaluation;
            }

            return new AiEvaluationResultDto
            {
                Score = 0,
                Strengths = string.Empty,
                Weaknesses = "Failed to parse AI response.",
                Suggestions = string.Empty
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Gemini API Error: {ex.Message}");
            return new AiEvaluationResultDto
            {
                Score = 0,
                Strengths = string.Empty,
                Weaknesses = $"AI evaluation failed: {ex.Message}",
                Suggestions = string.Empty
            };
        }
    }
}