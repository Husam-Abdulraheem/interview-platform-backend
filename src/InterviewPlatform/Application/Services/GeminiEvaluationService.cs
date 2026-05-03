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

        var prompt = $"Evaluate the answer for the question. Respond ONLY with a JSON object. Ensure the format adheres to:\n{{\n  \"score\": 0 to 100 integer,\n  \"strengths\": [\"array of strings describing strengths\"],\n  \"weaknesses\": [\"array of strings describing weaknesses\"],\n  \"suggestions\": [\"array of strings with actionable suggestions for improvement\"]\n}}\n\nProvide 3-5 specific points for each array.\n\nQuestion: {questionContent}\n\nAnswer: {traineeAnswer}";

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
                Strengths = new List<string>(),
                Weaknesses = new List<string> { "Failed to parse AI response." },
                Suggestions = new List<string>()
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Gemini API Error: {ex.Message}");
            return new AiEvaluationResultDto
            {
                Score = 0,
                Strengths = new List<string>(),
                Weaknesses = new List<string> { $"AI evaluation failed: {ex.Message}" },
                Suggestions = new List<string>()
            };
        }
    }
}