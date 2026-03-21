using FluentAssertions;
using InterviewPlatform.Application.DTOs;
using InterviewPlatform.Application.Interfaces;
using InterviewPlatform.Application.Services;
using InterviewPlatform.Core.Entities;
using InterviewPlatform.Core.Enums;
using Moq;
using Mapster;

namespace InterviewPlatform.Tests.Services;

public class InterviewAttemptServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IAiEvaluationService> _mockAiService;
    private readonly Mock<IRepository<InterviewAttempt>> _mockAttemptRepo;
    private readonly Mock<IRepository<Question>> _mockQuestionRepo;
    private readonly Mock<IRepository<AnswerAttempt>> _mockAnswerRepo;
    
    private readonly InterviewAttemptService _attemptService;

    public InterviewAttemptServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockAiService = new Mock<IAiEvaluationService>();
        
        _mockAttemptRepo = new Mock<IRepository<InterviewAttempt>>();
        _mockQuestionRepo = new Mock<IRepository<Question>>();
        _mockAnswerRepo = new Mock<IRepository<AnswerAttempt>>();

        _mockUnitOfWork.Setup(u => u.InterviewAttempts).Returns(_mockAttemptRepo.Object);
        _mockUnitOfWork.Setup(u => u.Questions).Returns(_mockQuestionRepo.Object);
        _mockUnitOfWork.Setup(u => u.AnswerAttempts).Returns(_mockAnswerRepo.Object);

        _attemptService = new InterviewAttemptService(_mockUnitOfWork.Object, _mockAiService.Object);
    }

    [Fact]
    public async Task SubmitAnswerAsync_CalculatesScoreCorrectly_UsingAiService()
    {
        // Arrange
        var attemptId = Guid.NewGuid();
        var activeAttempt = new InterviewAttempt { Id = attemptId, Status = InterviewStatus.InProgress };
        var question = new Question { Id = Guid.NewGuid(), Content = "What is React?" };
        var dto = new SubmitAnswerDto { AttemptId = attemptId, QuestionId = question.Id, AnswerText = "It is a UI library." };

        _mockAttemptRepo.Setup(r => r.GetByIdAsync(attemptId)).ReturnsAsync(activeAttempt);
        _mockQuestionRepo.Setup(r => r.GetByIdAsync(question.Id)).ReturnsAsync(question);
        
        // Setup AI evaluation response
        _mockAiService.Setup(ai => ai.EvaluateAnswerAsync(question.Content, dto.AnswerText))
            .ReturnsAsync(new AiEvaluationResultDto { Score = 85, Strengths = "Good", Weaknesses = "None" });

        // Act
        var result = await _attemptService.SubmitAnswerAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Score.Should().Be(85);
        result.AiFeedback.Should().Contain("Good");
        
        _mockAnswerRepo.Verify(r => r.AddAsync(It.Is<AnswerAttempt>(a => a.Score == 85)), Times.Once);
        _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task StartAttemptAsync_CreatesNewAttempt_WithInProgressStatus()
    {
        // Arrange
        var interviewId = Guid.NewGuid();
        var traineeId = Guid.NewGuid();

        // Act
        var result = await _attemptService.StartAttemptAsync(interviewId, traineeId);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(InterviewStatus.InProgress);
        result.InterviewId.Should().Be(interviewId);
        result.TraineeId.Should().Be(traineeId);

        _mockAttemptRepo.Verify(r => r.AddAsync(It.IsAny<InterviewAttempt>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
    }
}
