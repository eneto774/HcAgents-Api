using HcAgents.Application.Handlers.Commands;
using HcAgents.Domain.Abstractions;
using HcAgents.Domain.Entities;
using HcAgents.Domain.Models.Response;
using Moq;

namespace HcAgents.Tests.Handlers.Commands;

public class CreateSessionCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IOtpService> _mockOtpService;
    private readonly Mock<IJwtService> _mockJwtService;
    private readonly CreateSessionCommandHandler _handler;

    public CreateSessionCommandHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockOtpService = new Mock<IOtpService>();
        _mockJwtService = new Mock<IJwtService>();

        _mockUnitOfWork.Setup(x => x.UserRepository).Returns(_mockUserRepository.Object);

        _handler = new CreateSessionCommandHandler(
            _mockUnitOfWork.Object,
            _mockOtpService.Object,
            _mockJwtService.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidEmailAndOtp_ShouldCreateSessionSuccessfully()
    {
        var userId = Guid.NewGuid();
        var email = "user@example.com";
        var otp = "123456";
        var accessToken = "jwt-token-12345";

        var user = new User
        {
            Id = userId,
            Name = "Test User",
            Email = email,
            Secret = "user-secret",
            CreatedAt = DateTime.Now,
        };

        var command = new CreateSessionCommand { Email = email, Otp = otp };

        _mockUserRepository.Setup(x => x.GetUserByEmail(email)).ReturnsAsync(user);

        _mockOtpService.Setup(x => x.ValidateOtp(email, otp)).Returns(true);

        _mockJwtService.Setup(x => x.GenerateToken(userId.ToString(), email)).Returns(accessToken);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(accessToken, result.AccessToken);
        Assert.NotNull(result.User);
        Assert.Equal(email, result.User.Email);
        Assert.Null(result.User.Secret);

        _mockUserRepository.Verify(x => x.GetUserByEmail(email), Times.Once);
        _mockOtpService.Verify(x => x.ValidateOtp(email, otp), Times.Once);
        _mockOtpService.Verify(x => x.RemoveOtp(email), Times.Once);
        _mockJwtService.Verify(x => x.GenerateToken(userId.ToString(), email), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentUser_ShouldThrowException()
    {
        var email = "nonexistent@example.com";
        var otp = "123456";

        var command = new CreateSessionCommand { Email = email, Otp = otp };

        _mockUserRepository.Setup(x => x.GetUserByEmail(email)).ReturnsAsync((User?)null);

        var exception = await Assert.ThrowsAsync<Exception>(() =>
            _handler.Handle(command, CancellationToken.None)
        );

        Assert.Equal("UserNotExists", exception.Message);

        _mockUserRepository.Verify(x => x.GetUserByEmail(email), Times.Once);
        _mockOtpService.Verify(
            x => x.ValidateOtp(It.IsAny<string>(), It.IsAny<string>()),
            Times.Never
        );
        _mockOtpService.Verify(x => x.RemoveOtp(It.IsAny<string>()), Times.Never);
        _mockJwtService.Verify(
            x => x.GenerateToken(It.IsAny<string>(), It.IsAny<string>()),
            Times.Never
        );
    }

    [Fact]
    public async Task Handle_WithInvalidOtp_ShouldThrowException()
    {
        var userId = Guid.NewGuid();
        var email = "user@example.com";
        var invalidOtp = "999999";

        var user = new User
        {
            Id = userId,
            Name = "Test User",
            Email = email,
            Secret = "user-secret",
            CreatedAt = DateTime.Now,
        };

        var command = new CreateSessionCommand { Email = email, Otp = invalidOtp };

        _mockUserRepository.Setup(x => x.GetUserByEmail(email)).ReturnsAsync(user);

        _mockOtpService.Setup(x => x.ValidateOtp(email, invalidOtp)).Returns(false);

        var exception = await Assert.ThrowsAsync<Exception>(() =>
            _handler.Handle(command, CancellationToken.None)
        );

        Assert.Equal("InvalidOtp", exception.Message);

        _mockUserRepository.Verify(x => x.GetUserByEmail(email), Times.Once);
        _mockOtpService.Verify(x => x.ValidateOtp(email, invalidOtp), Times.Once);
        _mockOtpService.Verify(x => x.RemoveOtp(It.IsAny<string>()), Times.Never);
        _mockJwtService.Verify(
            x => x.GenerateToken(It.IsAny<string>(), It.IsAny<string>()),
            Times.Never
        );
    }

    [Fact]
    public async Task Handle_WhenJwtServiceFails_ShouldPropagateException()
    {
        var userId = Guid.NewGuid();
        var email = "user@example.com";
        var otp = "123456";

        var user = new User
        {
            Id = userId,
            Name = "Test User",
            Email = email,
            Secret = "user-secret",
            CreatedAt = DateTime.Now,
        };

        var command = new CreateSessionCommand { Email = email, Otp = otp };

        _mockUserRepository.Setup(x => x.GetUserByEmail(email)).ReturnsAsync(user);

        _mockOtpService.Setup(x => x.ValidateOtp(email, otp)).Returns(true);

        _mockJwtService
            .Setup(x => x.GenerateToken(userId.ToString(), email))
            .Throws(new Exception("JWT generation failed"));

        var exception = await Assert.ThrowsAsync<Exception>(() =>
            _handler.Handle(command, CancellationToken.None)
        );

        Assert.Equal("JWT generation failed", exception.Message);

        _mockUserRepository.Verify(x => x.GetUserByEmail(email), Times.Once);
        _mockOtpService.Verify(x => x.ValidateOtp(email, otp), Times.Once);
        _mockOtpService.Verify(x => x.RemoveOtp(email), Times.Once);
        _mockJwtService.Verify(x => x.GenerateToken(userId.ToString(), email), Times.Once);
    }

    [Fact]
    public async Task Handle_WithDifferentUsers_ShouldCreateSessionsIndependently()
    {
        var user1Id = Guid.NewGuid();
        var user2Id = Guid.NewGuid();
        var email1 = "user1@example.com";
        var email2 = "user2@example.com";
        var otp = "123456";
        var accessToken1 = "jwt-token-user1";
        var accessToken2 = "jwt-token-user2";

        var user1 = new User
        {
            Id = user1Id,
            Name = "User One",
            Email = email1,
            Secret = "secret1",
            CreatedAt = DateTime.Now,
        };

        var user2 = new User
        {
            Id = user2Id,
            Name = "User Two",
            Email = email2,
            Secret = "secret2",
            CreatedAt = DateTime.Now,
        };

        var command1 = new CreateSessionCommand { Email = email1, Otp = otp };
        var command2 = new CreateSessionCommand { Email = email2, Otp = otp };

        _mockUserRepository.Setup(x => x.GetUserByEmail(email1)).ReturnsAsync(user1);
        _mockUserRepository.Setup(x => x.GetUserByEmail(email2)).ReturnsAsync(user2);
        _mockOtpService.Setup(x => x.ValidateOtp(email1, otp)).Returns(true);
        _mockOtpService.Setup(x => x.ValidateOtp(email2, otp)).Returns(true);
        _mockJwtService
            .Setup(x => x.GenerateToken(user1Id.ToString(), email1))
            .Returns(accessToken1);
        _mockJwtService
            .Setup(x => x.GenerateToken(user2Id.ToString(), email2))
            .Returns(accessToken2);

        var result1 = await _handler.Handle(command1, CancellationToken.None);
        var result2 = await _handler.Handle(command2, CancellationToken.None);

        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(user1Id, result1.UserId);
        Assert.Equal(user2Id, result2.UserId);
        Assert.Equal(accessToken1, result1.AccessToken);
        Assert.Equal(accessToken2, result2.AccessToken);
        Assert.NotEqual(result1.AccessToken, result2.AccessToken);

        _mockUserRepository.Verify(x => x.GetUserByEmail(email1), Times.Once);
        _mockUserRepository.Verify(x => x.GetUserByEmail(email2), Times.Once);
        _mockOtpService.Verify(x => x.ValidateOtp(email1, otp), Times.Once);
        _mockOtpService.Verify(x => x.ValidateOtp(email2, otp), Times.Once);
        _mockOtpService.Verify(x => x.RemoveOtp(email1), Times.Once);
        _mockOtpService.Verify(x => x.RemoveOtp(email2), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldClearUserSecretInResponse()
    {
        var userId = Guid.NewGuid();
        var email = "user@email.com";
        var otp = "123456";
        var originalSecret = "original-secret";
        var accessToken = "jwt-token-12345";

        var user = new User
        {
            Id = userId,
            Name = "Test User",
            Email = email,
            Secret = originalSecret,
            CreatedAt = DateTime.Now,
        };

        var command = new CreateSessionCommand { Email = email, Otp = otp };

        _mockUserRepository.Setup(x => x.GetUserByEmail(email)).ReturnsAsync(user);
        _mockOtpService.Setup(x => x.ValidateOtp(email, otp)).Returns(true);
        _mockJwtService.Setup(x => x.GenerateToken(userId.ToString(), email)).Returns(accessToken);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result.User);
        Assert.Null(result.User.Secret);
        Assert.Null(user.Secret);
    }
}
