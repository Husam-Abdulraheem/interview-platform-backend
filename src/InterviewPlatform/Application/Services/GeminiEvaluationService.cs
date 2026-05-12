using System.Text.Json;
using System.Text.Json.Serialization;
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
        _apiKey = configuration["Gemini:ApiKey"] 
                  ?? configuration["Gemini__ApiKey"]
                  ?? configuration["GEMINI_API_KEY"]
                  ?? System.Environment.GetEnvironmentVariable("Gemini__ApiKey")
                  ?? System.Environment.GetEnvironmentVariable("GEMINI_API_KEY")
                  ?? string.Empty;
    }

    public async Task<AiEvaluationResultDto> EvaluateAnswerAsync(string questionContent, string traineeAnswer)
    {
        if (string.IsNullOrEmpty(_apiKey))
            throw new InvalidOperationException("Gemini API Key is missing.");

        // 1. Define the Response Schema to enforce structured JSON output
        var responseSchema = new Schema
        {
            Type = Google.GenAI.Types.Type.Object,
            Properties = new Dictionary<string, Schema>
            {
                { "score", new Schema { Type = Google.GenAI.Types.Type.Number } },
                { "generalFeedback", new Schema { Type = Google.GenAI.Types.Type.String } },
                { "strengths", new Schema { Type = Google.GenAI.Types.Type.Array, Items = new Schema { Type = Google.GenAI.Types.Type.String } } },
                { "weaknesses", new Schema { Type = Google.GenAI.Types.Type.Array, Items = new Schema { Type = Google.GenAI.Types.Type.String } } },
                { "suggestions", new Schema { Type = Google.GenAI.Types.Type.Array, Items = new Schema { Type = Google.GenAI.Types.Type.String } } }
            },
            Required = new List<string> { "score", "generalFeedback", "strengths", "weaknesses", "suggestions" }
        };

        var prompt = $@"
Aşağıdaki mülakat cevabını değerlendir. 
0-100 arası bir puan, performansın bir özeti, belirli güçlü yönler, zayıf yönler ve iyileştirme için öneriler sağla.

Soru: {questionContent}
Cevap: {traineeAnswer}
";

        try
        {
            var client = new Client(apiKey: _apiKey);

            var config = new GenerateContentConfig
            {
                ResponseMimeType = "application/json",
                ResponseSchema = responseSchema,
                Temperature = 0.2f, // Lower temperature for more consistent output
                MaxOutputTokens = 8192
            };

            var response = await client.Models.GenerateContentAsync(
                model: _modelName,
                contents: prompt,
                config: config
            );

            var textObj = response.Text;

            if (string.IsNullOrEmpty(textObj))
                throw new InvalidOperationException("AI returned an empty response.");

            // Clean up potential markdown formatting
            textObj = textObj.Trim();
            if (textObj.StartsWith("```json", StringComparison.OrdinalIgnoreCase))
            {
                textObj = textObj.Substring(7);
            }
            if (textObj.StartsWith("```", StringComparison.OrdinalIgnoreCase))
            {
                textObj = textObj.Substring(3);
            }
            if (textObj.EndsWith("```"))
            {
                textObj = textObj.Substring(0, textObj.Length - 3);
            }
            textObj = textObj.Trim();

            // 2. Use robust JSON options to handle variations in AI output
            var jsonOptions = new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
                NumberHandling = JsonNumberHandling.AllowReadingFromString
            };

            var evaluationResult = JsonSerializer.Deserialize<AiEvaluationResultDto>(textObj, jsonOptions);
            
            return evaluationResult ?? throw new InvalidOperationException("Failed to deserialize AI evaluation data.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Gemini API Error: {ex.Message}");
            return new AiEvaluationResultDto
            {
                Score = 0,
                GeneralFeedback = $"AI evaluation failed: {ex.Message}",
                Strengths = new List<string>(),
                Weaknesses = new List<string> { "Sistem cevabı değerlendirirken bir hatayla karşılaştı. Lütfen tekrar deneyin." },
                Suggestions = new List<string>()
            };
        }
    }

    public async Task<AiEvaluationResultDto> EvaluateDirectAsync(EvaluationRequestDto request)
    {
        return await EvaluateAnswerAsync(request.Question, request.Answer);
    }
}