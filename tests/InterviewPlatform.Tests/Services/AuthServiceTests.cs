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
