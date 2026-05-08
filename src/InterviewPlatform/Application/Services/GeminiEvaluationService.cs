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

        var prompt = $@"
Evaluate the following interview answer. 
You MUST respond with a VALID JSON object ONLY. 
Do not include any markdown formatting like ```json. 
Do not include any text before or after the JSON.

JSON Structure:
{{
  ""score"": (integer between 0 and 100),
  ""generalFeedback"": ""a concise summary of the evaluation"",
  ""strengths"": [""point 1"", ""point 2""],
  ""weaknesses"": [""point 1"", ""point 2""],
  ""suggestions"": [""point 1"", ""point 2""]
}}

Question: {questionContent}
Answer: {traineeAnswer}
";

        try
        {
            var client = new Client(apiKey: _apiKey);

            var config = new GenerateContentConfig
            {
                ResponseMimeType = "application/json",
                Temperature = 0.4f, // Lower temperature for more consistent JSON
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
                // Remove markdown code blocks if present
                textObj = textObj.Replace("```json", "").Replace("```", "").Trim();

                // Robustly extract the JSON object in case of extra text
                int start = textObj.IndexOf('{');
                int end = textObj.LastIndexOf('}');
                if (start != -1 && end != -1 && end > start)
                {
                    textObj = textObj.Substring(start, end - start + 1);
                }

                // Ensure your JSON options are flexible enough to handle AI output
                var jsonOptions = new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                };

                // 3. Parse the result directly into your DTO
                try
                {
                    var evaluationResult = JsonSerializer.Deserialize<AiEvaluationResultDto>(textObj, jsonOptions);
                    
                    // Proceed to map 'evaluationResult' to your InterviewAttempt/AnswerAttempt entities
                    if (evaluationResult != null)
                        return evaluationResult;
                }
                catch (JsonException ex)
                {
                    // Log the actual text returned by Gemini to see why it failed parsing
                    Console.WriteLine($"JSON Parsing Error: {ex.Message}");
                    Console.WriteLine($"AI Response Text: {response.Text}");
                    
                    // Throw a proper architectural exception, not a raw crash
                    throw new InvalidOperationException("Failed to parse AI evaluation data. Check the AI prompt or output format.", ex);
                }
            }

            return new AiEvaluationResultDto
            {
                Score = 0,
                GeneralFeedback = "Failed to parse AI response.",
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
                GeneralFeedback = $"AI evaluation failed: {ex.Message}",
                Strengths = new List<string>(),
                Weaknesses = new List<string> { $"AI evaluation failed: {ex.Message}" },
                Suggestions = new List<string>()
            };
        }
    }

    public async Task<AiEvaluationResultDto> EvaluateDirectAsync(EvaluationRequestDto request)
    {
        return await EvaluateAnswerAsync(request.Question, request.Answer);
    }
}