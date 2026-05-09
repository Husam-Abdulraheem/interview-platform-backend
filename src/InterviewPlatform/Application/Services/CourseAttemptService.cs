using InterviewPlatform.Application.DTOs;
using InterviewPlatform.Application.Exceptions;
using InterviewPlatform.Application.Interfaces;
using InterviewPlatform.Core.Entities;
using Mapster;

namespace InterviewPlatform.Application.Services;

public class CourseAttemptService : ICourseAttemptService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IAiEvaluationService _aiEvaluationService;

    public CourseAttemptService(
        IUnitOfWork unitOfWork,
        ICurrentUserContext currentUserContext,
        IAiEvaluationService aiEvaluationService)
    {
        _unitOfWork = unitOfWork;
        _currentUserContext = currentUserContext;
        _aiEvaluationService = aiEvaluationService;
    }

    public async Task<CourseAttemptDto> StartAttemptAsync(StartCourseAttemptDto dto)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(dto.CourseId);
        if (course == null) throw new NotFoundException($"Course with id {dto.CourseId} not found");

        var attempt = new CourseAttempt
        {
            Id = Guid.NewGuid(),
            CourseId = dto.CourseId,
            UserId = _currentUserContext.UserId,
            StartedAt = DateTime.UtcNow,
            TotalScore = 0
        };

        await _unitOfWork.CourseAttempts.AddAsync(attempt);
        await _unitOfWork.CompleteAsync();

        return attempt.Adapt<CourseAttemptDto>();
    }

    public async Task<QuestionAttemptDto> SubmitAnswerAsync(Guid attemptId, SubmitAnswerDto dto)
    {
        var attempt = await _unitOfWork.CourseAttempts.GetByIdAsync(attemptId);
        if (attempt == null) throw new NotFoundException($"Attempt with id {attemptId} not found");

        if (attempt.UserId != _currentUserContext.UserId)
            throw new ForbiddenException("You do not have permission to access this attempt.");

        if (attempt.CompletedAt != null)
            throw new InvalidOperationException("This attempt has already been completed.");

        var question = await _unitOfWork.Questions.GetByIdAsync(dto.QuestionId);
        if (question == null) throw new NotFoundException($"Question with id {dto.QuestionId} not found");

        if (question.CourseId != attempt.CourseId)
            throw new InvalidOperationException("This question does not belong to the attempted course.");

        // Check if already answered
        var existingAnswers = await _unitOfWork.QuestionAttempts.FindAsync(qa => qa.CourseAttemptId == attemptId && qa.QuestionId == dto.QuestionId);
        if (existingAnswers.Any())
            throw new InvalidOperationException("You have already answered this question.");

        // Evaluate answer
        var evaluation = await _aiEvaluationService.EvaluateAnswerAsync(question.Content, dto.Answer);

        var questionAttempt = new QuestionAttempt
        {
            Id = Guid.NewGuid(),
            CourseAttemptId = attemptId,
            QuestionId = dto.QuestionId,
            TraineeAnswer = dto.Answer,
            Score = evaluation.Score,
            GeneralFeedback = evaluation.GeneralFeedback,
            Strengths = evaluation.Strengths,
            Weaknesses = evaluation.Weaknesses,
            Suggestions = evaluation.Suggestions,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.QuestionAttempts.AddAsync(questionAttempt);
        await _unitOfWork.CompleteAsync();

        return questionAttempt.Adapt<QuestionAttemptDto>();
    }

    public async Task<CourseAttemptDto> CompleteAttemptAsync(Guid attemptId)
    {
        var attempt = await _unitOfWork.CourseAttempts.GetByIdAsync(attemptId);
        if (attempt == null) throw new NotFoundException($"Attempt with id {attemptId} not found");

        if (attempt.UserId != _currentUserContext.UserId)
            throw new ForbiddenException("You do not have permission to access this attempt.");

        if (attempt.CompletedAt != null)
            throw new InvalidOperationException("This attempt is already completed.");

        var answers = await _unitOfWork.QuestionAttempts.FindAsync(qa => qa.CourseAttemptId == attemptId);
        
        attempt.CompletedAt = DateTime.UtcNow;
        if (answers.Any())
        {
            attempt.TotalScore = answers.Average(a => a.Score);
        }

        _unitOfWork.CourseAttempts.Update(attempt);
        await _unitOfWork.CompleteAsync();

        var attemptDto = attempt.Adapt<CourseAttemptDto>();
        attemptDto.QuestionAttempts = answers.Adapt<List<QuestionAttemptDto>>();
        return attemptDto;
    }

    public async Task<IEnumerable<CourseAttemptDto>> GetMyAttemptsAsync()
    {
        var attempts = await _unitOfWork.CourseAttempts.FindAsync(ca => ca.UserId == _currentUserContext.UserId);
        var attemptDtos = attempts.Adapt<List<CourseAttemptDto>>();

        var attemptIds = attemptDtos.Select(a => a.Id).ToList();
        
        var allQuestionAttempts = await _unitOfWork.QuestionAttempts.FindAsync(qa => attemptIds.Contains(qa.CourseAttemptId));
        var questionAttemptsByAttempt = allQuestionAttempts.GroupBy(qa => qa.CourseAttemptId)
                                                           .ToDictionary(g => g.Key, g => g.ToList());

        // Get courses to attach details if needed
        var courseIds = attemptDtos.Select(a => a.CourseId).Distinct().ToList();
        var courses = await _unitOfWork.Courses.FindAsync(c => courseIds.Contains(c.Id));
        var coursesDict = courses.ToDictionary(c => c.Id, c => c.Adapt<CourseDto>());

        foreach (var dto in attemptDtos)
        {
            if (questionAttemptsByAttempt.TryGetValue(dto.Id, out var qaList))
            {
                dto.QuestionAttempts = qaList.Adapt<List<QuestionAttemptDto>>();
            }
            if (coursesDict.TryGetValue(dto.CourseId, out var courseDto))
            {
                dto.Course = courseDto;
            }
        }

        return attemptDtos.OrderByDescending(a => a.StartedAt);
    }

    public async Task<CourseAttemptDto?> GetAttemptByIdAsync(Guid attemptId)
    {
        var attempt = await _unitOfWork.CourseAttempts.GetByIdAsync(attemptId);
        if (attempt == null) return null;

        if (attempt.UserId != _currentUserContext.UserId && !_currentUserContext.IsAdmin)
            throw new ForbiddenException("You do not have permission to view this attempt.");

        var attemptDto = attempt.Adapt<CourseAttemptDto>();

        var answers = await _unitOfWork.QuestionAttempts.FindAsync(qa => qa.CourseAttemptId == attemptId);
        attemptDto.QuestionAttempts = answers.Adapt<List<QuestionAttemptDto>>();

        var course = await _unitOfWork.Courses.GetByIdAsync(attempt.CourseId);
        if (course != null)
        {
            attemptDto.Course = course.Adapt<CourseDto>();
        }

        return attemptDto;
    }
}
