using FluentAssertions;
using InterviewPlatform.Application.DTOs;
using InterviewPlatform.Application.Interfaces;
using InterviewPlatform.Application.Services;
using InterviewPlatform.Core.Entities;
using Moq;
using Xunit;
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
        TypeAdapterConfig.GlobalSettings.Default.PreserveReference(true);
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
    public async Task SubmitAnswerAsync_EvaluatesUsingAi_AndSavesResult()
    {
        // Arrange
        var attemptId = Guid.NewGuid();
        var questionId = Guid.NewGuid();
        var dto = new SubmitAnswerDto { InterviewAttemptId = attemptId, QuestionId = questionId, SubmittedText = "C# is an OOP language." };

        _mockAttemptRepo.Setup(r => r.GetByIdAsync(attemptId)).ReturnsAsync(new InterviewAttempt { Id = attemptId });
        _mockQuestionRepo.Setup(r => r.GetByIdAsync(questionId)).ReturnsAsync(new Question { Id = questionId, Content = "What is C#?" });

        _mockAiService.Setup(s => s.EvaluateAnswerAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new AiEvaluationResultDto { 
                Score = 85, 
                Strengths = new List<string> { "Good definition", "Clear explanation" }, 
                Weaknesses = new List<string> { "Could add more specs", "Needs examples" },
                Suggestions = new List<string> { "Consider adding specific examples", "Include technical details" }
            });

        // Act
        var result = await _attemptService.SubmitAnswerAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.AiScore.Should().Be(85);
        result.AiStrengths.Should().HaveCount(2);
        result.AiStrengths.Should().Contain("Good definition");
        result.AiSuggestions.Should().HaveCount(2);
        result.AiSuggestions.Should().Contain("Consider adding specific examples");

        _mockAnswerRepo.Verify(r => r.AddAsync(It.Is<AnswerAttempt>(a => a.AiScore == 85 && a.AiSuggestions.Contains("Consider adding specific examples"))), Times.Once);
        _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task StartAttemptAsync_CreatesNewAttempt()
    {
        // Arrange
        var interviewId = Guid.NewGuid();
        var traineeId = Guid.NewGuid();

        // Act
        var result = await _attemptService.StartAttemptAsync(traineeId, interviewId);

        // Assert
        result.Should().NotBeNull();
        result.InterviewId.Should().Be(interviewId);
        
        _mockAttemptRepo.Verify(r => r.AddAsync(It.IsAny<InterviewAttempt>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
    }
}
