using HcAgents.Application.Handlers.Commands;
using HcAgents.Domain.Abstractions;
using HcAgents.Domain.Commands;
using HcAgents.Domain.Entities;
using MediatR;
using Moq;

namespace HcAgents.Tests.Handlers.Commands;

public class CreateUserCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IMediator> _mockMediator;
    private readonly CreateUserCommandHandler _handler;

    public CreateUserCommandHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockMediator = new Mock<IMediator>();

        _mockUnitOfWork.Setup(x => x.UserRepository).Returns(_mockUserRepository.Object);

        _handler = new CreateUserCommandHandler(_mockUnitOfWork.Object, _mockMediator.Object);
    }

    [Fact]
    public async Task Handle_WithValidNewUser_ShouldCreateUserSuccessfully()
    {
        var command = new CreateUserCommand { Name = "Edgar Ribeiro", Email = "eneto774@gmail.com" };

        var createdUser = new User
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Email = command.Email,
            Secret = Guid.NewGuid().ToString(),
            CreatedAt = DateTime.Now,
        };

        _mockUserRepository.Setup(x => x.GetUserByEmail(command.Email)).ReturnsAsync((User?)null);

        _mockUserRepository.Setup(x => x.AddUser(It.IsAny<User>())).ReturnsAsync(createdUser);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(command.Name, result.Name);
        Assert.Equal(command.Email, result.Email);
        Assert.NotNull(result.Secret);
        Assert.NotEqual(Guid.Empty.ToString(), result.Secret);

        _mockUserRepository.Verify(x => x.GetUserByEmail(command.Email), Times.Once);
        _mockUserRepository.Verify(
            x =>
                x.AddUser(
                    It.Is<User>(u =>
                        u.Name == command.Name
                        && u.Email == command.Email
                        && !string.IsNullOrEmpty(u.Secret)
                    )
                ),
            Times.Once
        );
        _mockUnitOfWork.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WithExistingUserEmail_ShouldThrowException()
    {
        var command = new CreateUserCommand { Name = "Edgar Ribeiro", Email = "eneto774@gmail.com" };

        var existingUser = new User
        {
            Id = Guid.NewGuid(),
            Name = "Existing User",
            Email = command.Email,
            Secret = Guid.NewGuid().ToString(),
            CreatedAt = DateTime.Now.AddDays(-1),
        };

        _mockUserRepository.Setup(x => x.GetUserByEmail(command.Email)).ReturnsAsync(existingUser);

        var exception = await Assert.ThrowsAsync<Exception>(() =>
            _handler.Handle(command, CancellationToken.None)
        );

        Assert.Equal("UserAlreadyExists", exception.Message);

        _mockUserRepository.Verify(x => x.GetUserByEmail(command.Email), Times.Once);
        _mockUserRepository.Verify(x => x.AddUser(It.IsAny<User>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUserRepositoryAddFails_ShouldPropagateException()
    {
        var command = new CreateUserCommand { Name = "Edgar Ribeiro", Email = "eneto774@gmail.com" };

        _mockUserRepository.Setup(x => x.GetUserByEmail(command.Email)).ReturnsAsync((User?)null);

        _mockUserRepository
            .Setup(x => x.AddUser(It.IsAny<User>()))
            .ThrowsAsync(new Exception("Database error"));

        var exception = await Assert.ThrowsAsync<Exception>(() =>
            _handler.Handle(command, CancellationToken.None)
        );

        Assert.Equal("Database error", exception.Message);

        _mockUserRepository.Verify(x => x.GetUserByEmail(command.Email), Times.Once);
        _mockUserRepository.Verify(x => x.AddUser(It.IsAny<User>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_WithDifferentEmails_ShouldCreateUsersIndependently()
    {
        var command1 = new CreateUserCommand { Name = "User One", Email = "user1@email.com" };

        var command2 = new CreateUserCommand { Name = "User Two", Email = "user2@email.com" };

        var createdUser1 = new User
        {
            Id = Guid.NewGuid(),
            Name = command1.Name,
            Email = command1.Email,
            Secret = Guid.NewGuid().ToString(),
            CreatedAt = DateTime.Now,
        };

        var createdUser2 = new User
        {
            Id = Guid.NewGuid(),
            Name = command2.Name,
            Email = command2.Email,
            Secret = Guid.NewGuid().ToString(),
            CreatedAt = DateTime.Now,
        };

        _mockUserRepository.Setup(x => x.GetUserByEmail(command1.Email)).ReturnsAsync((User?)null);

        _mockUserRepository.Setup(x => x.GetUserByEmail(command2.Email)).ReturnsAsync((User?)null);

        _mockUserRepository
            .Setup(x => x.AddUser(It.Is<User>(u => u.Email == command1.Email)))
            .ReturnsAsync(createdUser1);

        _mockUserRepository
            .Setup(x => x.AddUser(It.Is<User>(u => u.Email == command2.Email)))
            .ReturnsAsync(createdUser2);

        var result1 = await _handler.Handle(command1, CancellationToken.None);
        var result2 = await _handler.Handle(command2, CancellationToken.None);

        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(command1.Email, result1.Email);
        Assert.Equal(command2.Email, result2.Email);
        Assert.NotEqual(result1.Id, result2.Id);

        _mockUserRepository.Verify(x => x.GetUserByEmail(command1.Email), Times.Once);
        _mockUserRepository.Verify(x => x.GetUserByEmail(command2.Email), Times.Once);
        _mockUserRepository.Verify(x => x.AddUser(It.IsAny<User>()), Times.Exactly(2));
        _mockUnitOfWork.Verify(x => x.CommitAsync(), Times.Exactly(2));
    }

    [Fact]
    public async Task Handle_ShouldGenerateUniqueSecretForEachUser()
    {
        var command = new CreateUserCommand { Name = "Test User", Email = "test@email.com" };

        var capturedUsers = new List<User>();

        _mockUserRepository.Setup(x => x.GetUserByEmail(command.Email)).ReturnsAsync((User?)null);

        _mockUserRepository
            .Setup(x => x.AddUser(It.IsAny<User>()))
            .Callback<User>(user => capturedUsers.Add(user))
            .ReturnsAsync((User user) => user);

        await _handler.Handle(command, CancellationToken.None);
        await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(2, capturedUsers.Count);
        Assert.NotEqual(capturedUsers[0].Secret, capturedUsers[1].Secret);
        Assert.All(capturedUsers, user => Assert.False(string.IsNullOrEmpty(user.Secret)));
    }
}
