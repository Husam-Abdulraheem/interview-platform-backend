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

public class InterviewServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ICurrentUserContext> _mockUserContext;
    private readonly Mock<IRepository<Interview>> _mockInterviewRepo;
    private readonly Mock<IRepository<Course>> _mockCourseRepo;
    
    private readonly InterviewService _interviewService;

    public InterviewServiceTests()
    {
        TypeAdapterConfig.GlobalSettings.Default.PreserveReference(true);
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockUserContext = new Mock<ICurrentUserContext>();
        _mockInterviewRepo = new Mock<IRepository<Interview>>();
        _mockCourseRepo = new Mock<IRepository<Course>>();

        _mockUnitOfWork.Setup(u => u.Interviews).Returns(_mockInterviewRepo.Object);
        _mockUnitOfWork.Setup(u => u.Courses).Returns(_mockCourseRepo.Object);

        _interviewService = new InterviewService(_mockUnitOfWork.Object, _mockUserContext.Object);
    }

    [Fact]
    public async Task CreateInterviewAsync_ThrowsForbidden_WhenCreatorDoesNotOwnParentCourse()
    {
        // Arrange
        var targetCourseId = Guid.NewGuid();
        var fakeCourse = new Course { Id = targetCourseId, CreatorId = Guid.NewGuid() }; // Course belongs to someone else
        var dto = new CreateInterviewDto { CourseId = targetCourseId, Title = "Unit Test" };

        _mockCourseRepo.Setup(r => r.GetByIdAsync(targetCourseId)).ReturnsAsync(fakeCourse);
        _mockUserContext.Setup(c => c.IsAdmin).Returns(false);
        _mockUserContext.Setup(c => c.UserId).Returns(Guid.NewGuid()); // Caller is NOT the owner

        // Act & Assert
        var currentMethod = async () => await _interviewService.CreateInterviewAsync(dto);
        await currentMethod.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task CreateInterviewAsync_Succeeds_WhenCallerOwnsParentCourse()
    {
        // Arrange
        var callerId = Guid.NewGuid();
        var targetCourseId = Guid.NewGuid();
        var fakeCourse = new Course { Id = targetCourseId, CreatorId = callerId }; // Course belongs to Caller
        var dto = new CreateInterviewDto { CourseId = targetCourseId, Title = "My Interview" };

        _mockCourseRepo.Setup(r => r.GetByIdAsync(targetCourseId)).ReturnsAsync(fakeCourse);
        _mockUserContext.Setup(c => c.IsAdmin).Returns(false);
        _mockUserContext.Setup(c => c.UserId).Returns(callerId);

        // Act
        var result = await _interviewService.CreateInterviewAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("My Interview");
        _mockInterviewRepo.Verify(r => r.AddAsync(It.IsAny<Interview>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task GetInterviewByIdAsync_ReturnsDto_WhenInterviewExists()
    {
        // Arrange
        var interviewId = Guid.NewGuid();
        _mockInterviewRepo.Setup(r => r.GetByIdAsync(interviewId))
            .ReturnsAsync(new Interview { Id = interviewId, Title = "Test Interview" });

        // Act
        var result = await _interviewService.GetInterviewByIdAsync(interviewId);

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be("Test Interview");
    }
}
