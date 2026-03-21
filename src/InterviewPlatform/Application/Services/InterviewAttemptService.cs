using InterviewPlatform.Application.DTOs;
using InterviewPlatform.Application.Interfaces;
using InterviewPlatform.Core.Entities;
using Mapster;

namespace InterviewPlatform.Application.Services;

public class InterviewAttemptService : IInterviewAttemptService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAiEvaluationService _aiEvaluationService;

    public InterviewAttemptService(IUnitOfWork unitOfWork, IAiEvaluationService aiEvaluationService)
    {
        _unitOfWork = unitOfWork;
        _aiEvaluationService = aiEvaluationService;
    }

    public async Task<InterviewAttemptDto> StartAttemptAsync(Guid traineeId, Guid interviewId)
    {
        var attempt = new InterviewAttempt
        {
            Id = Guid.NewGuid(),
            TraineeId = traineeId,
            InterviewId = interviewId,
            StartedAt = DateTime.UtcNow,
            TotalScore = 0
        };

        await _unitOfWork.InterviewAttempts.AddAsync(attempt);
        await _unitOfWork.CompleteAsync();

        return attempt.Adapt<InterviewAttemptDto>();
    }

    public async Task<AnswerAttemptDto> SubmitAnswerAsync(SubmitAnswerDto dto)
    {
        var attempt = await _unitOfWork.InterviewAttempts.GetByIdAsync(dto.InterviewAttemptId);
        if (attempt == null || attempt.CompletedAt.HasValue)
            throw new InvalidOperationException("Attempt not found or already completed.");

        var question = await _unitOfWork.Questions.GetByIdAsync(dto.QuestionId);
        if (question == null || question.InterviewId != attempt.InterviewId)
            throw new InvalidOperationException("Invalid question.");

        // AI Evaluation
        var evaluationResult = await _aiEvaluationService.EvaluateAnswerAsync(question.Content, dto.SubmittedText);

        var answer = new AnswerAttempt
        {
            Id = Guid.NewGuid(),
            InterviewAttemptId = dto.InterviewAttemptId,
            QuestionId = dto.QuestionId,
            SubmittedText = dto.SubmittedText,
            AiScore = evaluationResult.Score,
            AiStrengths = evaluationResult.Strengths,
            AiWeaknesses = evaluationResult.Weaknesses
        };

        await _unitOfWork.AnswerAttempts.AddAsync(answer);
        await _unitOfWork.CompleteAsync();

        return answer.Adapt<AnswerAttemptDto>();
    }

    public async Task<InterviewAttemptDto> CompleteAttemptAsync(Guid attemptId)
    {
        var attempt = await _unitOfWork.InterviewAttempts.GetByIdAsync(attemptId);
        if (attempt == null)
            throw new InvalidOperationException("Attempt not found.");

        var answers = await _unitOfWork.AnswerAttempts.FindAsync(a => a.InterviewAttemptId == attemptId);
        
        var answerList = answers.ToList();
        decimal totalScore = answerList.Any() ? answerList.Average(a => a.AiScore) : 0;

        attempt.CompletedAt = DateTime.UtcNow;
        attempt.TotalScore = totalScore;

        _unitOfWork.InterviewAttempts.Update(attempt);
        await _unitOfWork.CompleteAsync();

        return attempt.Adapt<InterviewAttemptDto>();
    }

    public async Task<InterviewAttemptDto?> GetAttemptDetailsAsync(Guid attemptId)
    {
        var attempt = await _unitOfWork.InterviewAttempts.GetByIdAsync(attemptId);
        if (attempt == null) return null;

        var answers = await _unitOfWork.AnswerAttempts.FindAsync(a => a.InterviewAttemptId == attemptId);
        attempt.AnswerAttempts = answers.ToList();

        return attempt.Adapt<InterviewAttemptDto>();
    }
}
