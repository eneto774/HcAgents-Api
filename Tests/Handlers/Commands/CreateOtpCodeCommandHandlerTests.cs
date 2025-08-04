using HcAgents.Domain.Abstractions;
using HcAgents.Domain.Entities;
using Moq;

namespace HcAgents.Tests.Handlers.Commands;

public class CreateOtpCodeCommandHandlerTests
{
    private readonly Mock<IOtpService> _mockOtpService;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly CreateOtpCodeCommandHandler _handler;

    public CreateOtpCodeCommandHandlerTests()
    {
        _mockOtpService = new Mock<IOtpService>();
        _mockEmailService = new Mock<IEmailService>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockUserRepository = new Mock<IUserRepository>();

        _mockUnitOfWork.Setup(x => x.UserRepository).Returns(_mockUserRepository.Object);

        _handler = new CreateOtpCodeCommandHandler(
            _mockOtpService.Object,
            _mockEmailService.Object,
            _mockUnitOfWork.Object
        );
    }

    [Fact]
    public async Task Handle_WithExistingUser_ShouldGenerateOtpAndSendEmailSuccessfully()
    {
        var email = "user@email.com";
        var generatedOtp = "123456";

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = email,
            Secret = "user-secret",
            CreatedAt = DateTime.Now,
        };

        var command = new CreateOtpCodeCommand { Email = email };

        _mockUserRepository.Setup(x => x.GetUserByEmail(email)).ReturnsAsync(user);

        _mockOtpService.Setup(x => x.GenerateOtp(email)).Returns(generatedOtp);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result);

        _mockUserRepository.Verify(x => x.GetUserByEmail(email), Times.Once);
        _mockOtpService.Verify(x => x.GenerateOtp(email), Times.Once);
        _mockEmailService.Verify(
            x => x.SendEmail(email, "HCAgents - OTP Code", $"Your OTP code is: {generatedOtp}"),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_WithNonExistentUser_ShouldReturnFalse()
    {
        var email = "nonexistent@email.com";

        var command = new CreateOtpCodeCommand { Email = email };

        _mockUserRepository.Setup(x => x.GetUserByEmail(email)).ReturnsAsync((User?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result);

        _mockUserRepository.Verify(x => x.GetUserByEmail(email), Times.Once);
        _mockOtpService.Verify(x => x.GenerateOtp(It.IsAny<string>()), Times.Never);
        _mockEmailService.Verify(
            x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never
        );
    }

    [Fact]
    public async Task Handle_WhenOtpServiceFails_ShouldReturnFalse()
    {
        var email = "user@email.com";

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = email,
            Secret = "user-secret",
            CreatedAt = DateTime.Now,
        };

        var command = new CreateOtpCodeCommand { Email = email };

        _mockUserRepository.Setup(x => x.GetUserByEmail(email)).ReturnsAsync(user);

        _mockOtpService.Setup(x => x.GenerateOtp(email)).Throws(new Exception("OTP service error"));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result);

        _mockUserRepository.Verify(x => x.GetUserByEmail(email), Times.Once);
        _mockOtpService.Verify(x => x.GenerateOtp(email), Times.Once);
        _mockEmailService.Verify(
            x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never
        );
    }

    [Fact]
    public async Task Handle_WhenEmailServiceFails_ShouldReturnFalse()
    {
        var email = "user@email.com";
        var generatedOtp = "123456";

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = email,
            Secret = "user-secret",
            CreatedAt = DateTime.Now,
        };

        var command = new CreateOtpCodeCommand { Email = email };

        _mockUserRepository.Setup(x => x.GetUserByEmail(email)).ReturnsAsync(user);

        _mockOtpService.Setup(x => x.GenerateOtp(email)).Returns(generatedOtp);

        _mockEmailService
            .Setup(x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Throws(new Exception("Email service error"));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result);

        _mockUserRepository.Verify(x => x.GetUserByEmail(email), Times.Once);
        _mockOtpService.Verify(x => x.GenerateOtp(email), Times.Once);
        _mockEmailService.Verify(
            x => x.SendEmail(email, "HCAgents - OTP Code", $"Your OTP code is: {generatedOtp}"),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_WithDifferentUsers_ShouldGenerateOtpForEachUser()
    {
        var email1 = "user1@email.com";
        var email2 = "user2@email.com";
        var otp1 = "123456";
        var otp2 = "654321";

        var user1 = new User
        {
            Id = Guid.NewGuid(),
            Name = "User One",
            Email = email1,
            Secret = "secret1",
            CreatedAt = DateTime.Now,
        };

        var user2 = new User
        {
            Id = Guid.NewGuid(),
            Name = "User Two",
            Email = email2,
            Secret = "secret2",
            CreatedAt = DateTime.Now,
        };

        var command1 = new CreateOtpCodeCommand { Email = email1 };
        var command2 = new CreateOtpCodeCommand { Email = email2 };

        _mockUserRepository.Setup(x => x.GetUserByEmail(email1)).ReturnsAsync(user1);
        _mockUserRepository.Setup(x => x.GetUserByEmail(email2)).ReturnsAsync(user2);
        _mockOtpService.Setup(x => x.GenerateOtp(email1)).Returns(otp1);
        _mockOtpService.Setup(x => x.GenerateOtp(email2)).Returns(otp2);

        var result1 = await _handler.Handle(command1, CancellationToken.None);
        var result2 = await _handler.Handle(command2, CancellationToken.None);

        Assert.True(result1);
        Assert.True(result2);

        _mockUserRepository.Verify(x => x.GetUserByEmail(email1), Times.Once);
        _mockUserRepository.Verify(x => x.GetUserByEmail(email2), Times.Once);
        _mockOtpService.Verify(x => x.GenerateOtp(email1), Times.Once);
        _mockOtpService.Verify(x => x.GenerateOtp(email2), Times.Once);
        _mockEmailService.Verify(
            x => x.SendEmail(email1, "HCAgents - OTP Code", $"Your OTP code is: {otp1}"),
            Times.Once
        );
        _mockEmailService.Verify(
            x => x.SendEmail(email2, "HCAgents - OTP Code", $"Your OTP code is: {otp2}"),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_ShouldUseCorrectEmailSubjectAndFormat()
    {
        var email = "user@email.com";
        var generatedOtp = "987654";

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = email,
            Secret = "user-secret",
            CreatedAt = DateTime.Now,
        };

        var command = new CreateOtpCodeCommand { Email = email };

        _mockUserRepository.Setup(x => x.GetUserByEmail(email)).ReturnsAsync(user);

        _mockOtpService.Setup(x => x.GenerateOtp(email)).Returns(generatedOtp);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result);

        _mockEmailService.Verify(
            x => x.SendEmail(email, "HCAgents - OTP Code", "Your OTP code is: 987654"),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_WhenUserRepositoryFails_ShouldReturnFalse()
    {
        var email = "user@email.com";

        var command = new CreateOtpCodeCommand { Email = email };

        _mockUserRepository
            .Setup(x => x.GetUserByEmail(email))
            .ThrowsAsync(new Exception("Database error"));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result);

        _mockUserRepository.Verify(x => x.GetUserByEmail(email), Times.Once);
        _mockOtpService.Verify(x => x.GenerateOtp(It.IsAny<string>()), Times.Never);
        _mockEmailService.Verify(
            x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never
        );
    }
}
