using FluentAssertions;
using InterviewPlatform.Application.DTOs;
using InterviewPlatform.Application.Interfaces;
using InterviewPlatform.Application.Services;
using InterviewPlatform.Core.Entities;
using InterviewPlatform.Core.Enums;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace InterviewPlatform.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IRepository<User>> _mockUserRepo;
    private readonly Mock<IConfiguration> _mockConfiguration;
    
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockUserRepo = new Mock<IRepository<User>>();
        _mockConfiguration = new Mock<IConfiguration>();

        _mockUnitOfWork.Setup(u => u.Users).Returns(_mockUserRepo.Object);

        _mockConfiguration.Setup(c => c["Jwt:Key"]).Returns("SuperSecretKeyThatIsVeryLongAndSecureForUnitTests123!");
        _mockConfiguration.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
        _mockConfiguration.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");

        _authService = new AuthService(_mockConfiguration.Object, _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task RegisterAsync_CreatesTrainee_WhenNoRoleRequested()
    {
        // Arrange
        _mockUserRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User>());

        User? captured = null;
        _mockUserRepo.Setup(r => r.AddAsync(It.IsAny<User>()))
            .Callback<User>(u => captured = u)
            .Returns(Task.CompletedTask);

        var dto = new RegisterDto { Email = "new@test.com", Password = "P@ssword1", FullName = "New User" };

        // Act
        var result = await _authService.RegisterAsync(dto);

        // Assert
        result.Should().BeTrue();
        captured.Should().NotBeNull();
        captured!.Role.Should().Be(Role.Trainee);
        captured.IsApproved.Should().BeTrue();
        captured.RequestedRole.Should().BeNull();
        // password should be hashed and verify correctly
        PasswordHasher.Verify(dto.Password, captured.PasswordHash).Should().BeTrue();
    }

    [Fact]
    public async Task RegisterAsync_RequestCreator_SetsRequestedRoleAndNotApproved()
    {
        // Arrange
        _mockUserRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>() ))
            .ReturnsAsync(new List<User>());

        User? captured = null;
        _mockUserRepo.Setup(r => r.AddAsync(It.IsAny<User>()))
            .Callback<User>(u => captured = u)
            .Returns(Task.CompletedTask);

        var dto = new RegisterDto { Email = "creator@test.com", Password = "CreatorPwd1", FullName = "Creator User", Role = Role.Creator };

        // Act
        var result = await _authService.RegisterAsync(dto);

        // Assert
        result.Should().BeTrue();
        captured.Should().NotBeNull();
        captured!.Role.Should().Be(Role.Trainee); // still trainee until approved
        captured.RequestedRole.Should().Be(Role.Creator);
        captured.IsApproved.Should().BeFalse();
        PasswordHasher.Verify(dto.Password, captured.PasswordHash).Should().BeTrue();
    }

    [Fact]
    public async Task LoginAsync_ReturnsToken_WhenCredentialsAreValid()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid(), Email = "valid@test.com", FullName = "Valid User" };
        user.PasswordHash = PasswordHasher.Hash("validpwd");

        _mockUserRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>() ))
            .ReturnsAsync(new List<User> { user });

        var dto = new LoginDto { Email = "valid@test.com", Password = "validpwd" };

        // Act
        var token = await _authService.LoginAsync(dto);

        // Assert
        token.Should().NotBeNull();
        token!.Token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task LoginAsync_ReturnsNull_WhenCredentialsAreInvalid()
    {
        // Arrange
        _mockUserRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User>());

        var dto = new LoginDto { Email = "test@test.com", Password = "wrongpassword" };

        // Act
        var result = await _authService.LoginAsync(dto);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task RegisterAsync_ReturnsFalse_WhenEmailAlreadyExists()
    {
        // Arrange
        _mockUserRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User> { new User { Id = Guid.NewGuid() } }); 

        var dto = new RegisterDto { Email = "existing@test.com", Password = "pwd", FullName = "Test", Role = Role.Trainee };

        // Act
        var result = await _authService.RegisterAsync(dto);

        // Assert
        result.Should().BeFalse();
        _mockUserRepo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }
}
