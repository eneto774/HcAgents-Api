using HcAgents.Domain.Abstractions;
using HcAgents.Domain.Commands;
using HcAgents.Domain.Entities;
using Moq;

namespace HcAgents.Tests.Handlers.Commands;

public class CreateChatAndBotCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IBotRepository> _mockBotRepository;
    private readonly Mock<IChatRepository> _mockChatRepository;
    private readonly CreateChatAndBotCommandHandler _handler;

    public CreateChatAndBotCommandHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockBotRepository = new Mock<IBotRepository>();
        _mockChatRepository = new Mock<IChatRepository>();

        _mockUnitOfWork.Setup(x => x.BotRepository).Returns(_mockBotRepository.Object);
        _mockUnitOfWork.Setup(x => x.ChatRepository).Returns(_mockChatRepository.Object);

        _handler = new CreateChatAndBotCommandHandler(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateBotAndChatSuccessfully()
    {
        var userId = Guid.NewGuid();
        var botId = Guid.NewGuid();
        var chatId = Guid.NewGuid();
        var createdAt = DateTime.Now;

        var command = new CreateChatAndBotCommand
        {
            ChatName = "Test Chat",
            ChatDescription = "Test Chat",
            BotName = "Test Bot",
            BotDescription = "Você é um assistente de teste",
            UserId = userId,
        };

        var createdBot = new Bot
        {
            Id = botId,
            Name = command.BotName,
            Description = command.BotDescription,
            CreatedBy = userId,
            CreatedAt = createdAt,
        };

        var createdChat = new Chat
        {
            Id = chatId,
            Name = command.ChatName,
            Description = command.ChatDescription,
            BotId = botId,
            UserId = userId,
            CreatedBy = userId,
            CreatedAt = createdAt,
        };

        _mockBotRepository.Setup(x => x.AddBot(It.IsAny<Bot>())).ReturnsAsync(createdBot);

        _mockChatRepository.Setup(x => x.AddChat(It.IsAny<Chat>())).ReturnsAsync(createdChat);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(command.ChatName, result.Name);
        Assert.Equal(command.ChatDescription, result.Description);
        Assert.Equal(botId, result.BotId);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(userId, result.CreatedBy);

        _mockBotRepository.Verify(
            x =>
                x.AddBot(
                    It.Is<Bot>(b =>
                        b.Name == command.BotName
                        && b.Description == command.BotDescription
                        && b.CreatedBy == userId
                    )
                ),
            Times.Once
        );

        _mockChatRepository.Verify(
            x =>
                x.AddChat(
                    It.Is<Chat>(c =>
                        c.Name == command.ChatName
                        && c.Description == command.ChatDescription
                        && c.BotId == botId
                        && c.UserId == userId
                        && c.CreatedBy == userId
                    )
                ),
            Times.Once
        );

        _mockUnitOfWork.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenBotCreationFails_ShouldPropagateException()
    {
        var userId = Guid.NewGuid();

        var command = new CreateChatAndBotCommand
        {
            ChatName = "Test Chat",
            ChatDescription = "Test Chat",
            BotName = "Test Bot",
            BotDescription = "Você é um assistente de teste",
            UserId = userId,
        };

        _mockBotRepository
            .Setup(x => x.AddBot(It.IsAny<Bot>()))
            .ThrowsAsync(new Exception("Database error creating bot"));

        var exception = await Assert.ThrowsAsync<Exception>(() =>
            _handler.Handle(command, CancellationToken.None)
        );

        Assert.Equal("Database error creating bot", exception.Message);

        _mockBotRepository.Verify(x => x.AddBot(It.IsAny<Bot>()), Times.Once);

        _mockChatRepository.Verify(x => x.AddChat(It.IsAny<Chat>()), Times.Never);

        _mockUnitOfWork.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenChatCreationFails_ShouldPropagateException()
    {
        var userId = Guid.NewGuid();
        var botId = Guid.NewGuid();

        var command = new CreateChatAndBotCommand
        {
            ChatName = "Test Chat",
            ChatDescription = "Test Chat",
            BotName = "Test Bot",
            BotDescription = "Você é um assistente de teste",
            UserId = userId,
        };

        var createdBot = new Bot
        {
            Id = botId,
            Name = command.BotName,
            Description = command.BotDescription,
            CreatedBy = userId,
            CreatedAt = DateTime.Now,
        };

        _mockBotRepository.Setup(x => x.AddBot(It.IsAny<Bot>())).ReturnsAsync(createdBot);

        _mockChatRepository
            .Setup(x => x.AddChat(It.IsAny<Chat>()))
            .ThrowsAsync(new Exception("Database error creating chat"));

        var exception = await Assert.ThrowsAsync<Exception>(() =>
            _handler.Handle(command, CancellationToken.None)
        );

        Assert.Equal("Database error creating chat", exception.Message);

        _mockBotRepository.Verify(x => x.AddBot(It.IsAny<Bot>()), Times.Once);

        _mockChatRepository.Verify(x => x.AddChat(It.IsAny<Chat>()), Times.Once);

        _mockUnitOfWork.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_WithDifferentUserIds_ShouldCreateEntitiesWithCorrectUserId()
    {
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var botId = Guid.NewGuid();
        var chatId = Guid.NewGuid();

        var command = new CreateChatAndBotCommand
        {
            ChatName = "User2 Chat",
            ChatDescription = "Chat for user 2",
            BotName = "User2 Bot",
            BotDescription = "Bot for user 2",
            UserId = userId2,
        };

        var createdBot = new Bot
        {
            Id = botId,
            Name = command.BotName,
            Description = command.BotDescription,
            CreatedBy = userId2,
            CreatedAt = DateTime.Now,
        };

        var createdChat = new Chat
        {
            Id = chatId,
            Name = command.ChatName,
            Description = command.ChatDescription,
            BotId = botId,
            UserId = userId2,
            CreatedBy = userId2,
            CreatedAt = DateTime.Now,
        };

        _mockBotRepository.Setup(x => x.AddBot(It.IsAny<Bot>())).ReturnsAsync(createdBot);

        _mockChatRepository.Setup(x => x.AddChat(It.IsAny<Chat>())).ReturnsAsync(createdChat);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(userId2, result.UserId);
        Assert.Equal(userId2, result.CreatedBy);

        _mockBotRepository.Verify(
            x => x.AddBot(It.Is<Bot>(b => b.CreatedBy == userId2)),
            Times.Once
        );

        _mockChatRepository.Verify(
            x => x.AddChat(It.Is<Chat>(c => c.UserId == userId2 && c.CreatedBy == userId2)),
            Times.Once
        );

        _mockBotRepository.Verify(
            x => x.AddBot(It.Is<Bot>(b => b.CreatedBy == userId1)),
            Times.Never
        );
    }
}
