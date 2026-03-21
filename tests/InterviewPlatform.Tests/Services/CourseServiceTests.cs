using FluentAssertions;
using InterviewPlatform.Application.Exceptions;
using InterviewPlatform.Application.Interfaces;
using InterviewPlatform.Application.Services;
using InterviewPlatform.Core.Entities;
using InterviewPlatform.Core.Enums;
using Moq;
using Mapster;

namespace InterviewPlatform.Tests.Services;

public class CourseServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ICurrentUserContext> _mockUserContext;
    private readonly Mock<IRepository<Course>> _mockCourseRepo;
    private readonly CourseService _courseService;

    public CourseServiceTests()
    {
        // Global Mapster init
        TypeAdapterConfig.GlobalSettings.Default.PreserveReference(true);

        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockUserContext = new Mock<ICurrentUserContext>();
        _mockCourseRepo = new Mock<IRepository<Course>>();

        _mockUnitOfWork.Setup(u => u.Courses).Returns(_mockCourseRepo.Object);

        _courseService = new CourseService(_mockUnitOfWork.Object, _mockUserContext.Object);
    }

    [Fact]
    public async Task DeleteCourseAsync_ThrowsForbiddenException_WhenCallerIsCreator_ButNotTheOwner()
    {
        // Arrange
        var targetCourseId = Guid.NewGuid();
        var fakeCourse = new Course { Id = targetCourseId, CreatorId = Guid.NewGuid() }; // Notice random owner
        
        _mockCourseRepo.Setup(repo => repo.GetByIdAsync(targetCourseId)).ReturnsAsync(fakeCourse);
        
        // Caller is NOT Admin, and user ID does not match CreatorId
        _mockUserContext.Setup(c => c.IsAdmin).Returns(false);
        _mockUserContext.Setup(c => c.UserId).Returns(Guid.NewGuid());

        // Act & Assert
        var act = async () => await _courseService.DeleteCourseAsync(targetCourseId);
        await act.Should().ThrowAsync<ForbiddenException>()
            .WithMessage("You do not have permission to delete this course.");
    }

    [Fact]
    public async Task DeleteCourseAsync_Succeeds_WhenCallerIsAdmin_EvenIfNotOwner()
    {
        // Arrange
        var targetCourseId = Guid.NewGuid();
        var fakeCourse = new Course { Id = targetCourseId, CreatorId = Guid.NewGuid() };
        
        _mockCourseRepo.Setup(repo => repo.GetByIdAsync(targetCourseId)).ReturnsAsync(fakeCourse);
        
        // Caller IS Admin, even though caller is not owner 
        _mockUserContext.Setup(c => c.IsAdmin).Returns(true);
        _mockUserContext.Setup(c => c.UserId).Returns(Guid.NewGuid());

        // Act
        await _courseService.DeleteCourseAsync(targetCourseId);

        // Assert
        _mockCourseRepo.Verify(repo => repo.Remove(fakeCourse), Times.Once);
        _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateCourseAsync_AssignsCreatorId_FromUserContext()
    {
        // Arrange
        var callerId = Guid.NewGuid();
        var dto = new InterviewPlatform.Application.DTOs.CreateCourseDto { Title = "Test Title", Description = "Desc" };
        
        _mockUserContext.Setup(c => c.UserId).Returns(callerId);

        // Act
        var result = await _courseService.CreateCourseAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("Test Title");
        _mockCourseRepo.Verify(r => r.AddAsync(It.Is<Course>(c => c.CreatorId == callerId)), Times.Once);
        _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
    }
}
