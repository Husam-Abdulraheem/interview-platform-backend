using FluentAssertions;
using InterviewPlatform.Application.Exceptions;
using InterviewPlatform.Application.Interfaces;
using InterviewPlatform.Application.Services;
using InterviewPlatform.Core.Entities;
using InterviewPlatform.Core.Enums;
using Moq;
using Xunit;
using Mapster;
using System.Linq.Expressions;

namespace InterviewPlatform.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IRepository<User>> _mockUserRepo;
    private readonly Mock<IRepository<Course>> _mockCourseRepo;
    private readonly Mock<IRepository<InterviewAttempt>> _mockAttemptRepo;
    
    private readonly UserService _userService;

    public UserServiceTests()
    {
        TypeAdapterConfig.GlobalSettings.Default.PreserveReference(true);
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockUserRepo = new Mock<IRepository<User>>();
        _mockCourseRepo = new Mock<IRepository<Course>>();
        _mockAttemptRepo = new Mock<IRepository<InterviewAttempt>>();

        _mockUnitOfWork.Setup(u => u.Users).Returns(_mockUserRepo.Object);
        _mockUnitOfWork.Setup(u => u.Courses).Returns(_mockCourseRepo.Object);
        _mockUnitOfWork.Setup(u => u.InterviewAttempts).Returns(_mockAttemptRepo.Object);

        _userService = new UserService(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task GetAllUsersAsync_ReturnsAllUserProfiles_MappedCorrectly()
    {
        // Arrange
        var fakeUsers = new List<User>
        {
            new User { Id = Guid.NewGuid(), FullName = "Admin User", Role = Role.Admin },
            new User { Id = Guid.NewGuid(), FullName = "Trainee User", Role = Role.Trainee }
        };

        _mockUserRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(fakeUsers);

        // Act
        var result = await _userService.GetAllUsersAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.First().FullName.Should().Be("Admin User");
        result.Last().FullName.Should().Be("Trainee User");
    }

    [Fact]
    public async Task UpdateUserRoleAsync_UpdatesRoleCorrectly()
    {
        // Arrange
        var targetId = Guid.NewGuid();
        var fakeUser = new User { Id = targetId, Role = Role.Trainee };

        _mockUserRepo.Setup(r => r.GetByIdAsync(targetId)).ReturnsAsync(fakeUser);

        // Act
        await _userService.UpdateUserRoleAsync(targetId, Role.Creator);

        // Assert
        fakeUser.Role.Should().Be(Role.Creator);
        _mockUserRepo.Verify(r => r.Update(fakeUser), Times.Once);
        _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteUserAsync_ThrowsNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var targetId = Guid.NewGuid();
        _mockUserRepo.Setup(r => r.GetByIdAsync(targetId)).ReturnsAsync((User)null!);

        // Act & Assert
        var currentMethod = async () => await _userService.DeleteUserAsync(targetId);
        await currentMethod.Should().ThrowAsync<NotFoundException>();
    }
}
