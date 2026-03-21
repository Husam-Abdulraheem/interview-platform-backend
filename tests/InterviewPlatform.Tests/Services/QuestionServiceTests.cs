using FluentAssertions;
using InterviewPlatform.Application.DTOs;
using InterviewPlatform.Application.Exceptions;
using InterviewPlatform.Application.Interfaces;
using InterviewPlatform.Application.Services;
using InterviewPlatform.Core.Entities;
using Moq;
using Xunit;
using Mapster;

namespace InterviewPlatform.Tests.Services;

public class QuestionServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ICurrentUserContext> _mockUserContext;
    private readonly Mock<IRepository<Question>> _mockQuestionRepo;
    private readonly Mock<IRepository<Interview>> _mockInterviewRepo;
    private readonly Mock<IRepository<Course>> _mockCourseRepo;
    
    private readonly QuestionService _questionService;

    public QuestionServiceTests()
    {
        TypeAdapterConfig.GlobalSettings.Default.PreserveReference(true);
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockUserContext = new Mock<ICurrentUserContext>();
        _mockQuestionRepo = new Mock<IRepository<Question>>();
        _mockInterviewRepo = new Mock<IRepository<Interview>>();
        _mockCourseRepo = new Mock<IRepository<Course>>();

        _mockUnitOfWork.Setup(u => u.Questions).Returns(_mockQuestionRepo.Object);
        _mockUnitOfWork.Setup(u => u.Interviews).Returns(_mockInterviewRepo.Object);
        _mockUnitOfWork.Setup(u => u.Courses).Returns(_mockCourseRepo.Object);

        _questionService = new QuestionService(_mockUnitOfWork.Object, _mockUserContext.Object);
    }

    [Fact]
    public async Task AddQuestionAsync_ThrowsForbidden_WhenCreatorDoesNotOwnGrandparentCourse()
    {
        // Arrange
        var interviewId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        
        var fakeInterview = new Interview { Id = interviewId, CourseId = courseId };
        var fakeCourse = new Course { Id = courseId, CreatorId = Guid.NewGuid() }; // Notice random owner
        
        var dto = new CreateQuestionDto { InterviewId = interviewId, Content = "Q1", OrderIndex = 1 };

        _mockInterviewRepo.Setup(r => r.GetByIdAsync(interviewId)).ReturnsAsync(fakeInterview);
        _mockCourseRepo.Setup(r => r.GetByIdAsync(courseId)).ReturnsAsync(fakeCourse);
        
        _mockUserContext.Setup(c => c.IsAdmin).Returns(false);
        _mockUserContext.Setup(c => c.UserId).Returns(Guid.NewGuid()); // Caller is NOT the owner

        // Act & Assert
        var currentMethod = async () => await _questionService.AddQuestionAsync(dto);
        await currentMethod.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task AddQuestionAsync_Succeeds_WhenCreatorOwnsGrandparentCourse()
    {
        // Arrange
        var callerId = Guid.NewGuid();
        var interviewId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        
        var fakeInterview = new Interview { Id = interviewId, CourseId = courseId };
        var fakeCourse = new Course { Id = courseId, CreatorId = callerId }; // Ownership matches

        var dto = new CreateQuestionDto { InterviewId = interviewId, Content = "What is LINQ?", OrderIndex = 1 };

        _mockInterviewRepo.Setup(r => r.GetByIdAsync(interviewId)).ReturnsAsync(fakeInterview);
        _mockCourseRepo.Setup(r => r.GetByIdAsync(courseId)).ReturnsAsync(fakeCourse);
        
        _mockUserContext.Setup(c => c.IsAdmin).Returns(false);
        _mockUserContext.Setup(c => c.UserId).Returns(callerId);

        // Act
        var result = await _questionService.AddQuestionAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Content.Should().Be("What is LINQ?");
        _mockQuestionRepo.Verify(r => r.AddAsync(It.IsAny<Question>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
    }
}
