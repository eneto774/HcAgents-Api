using HcAgents.Domain.Abstractions;
using HcAgents.Domain.Entities;
using Moq;

namespace HcAgents.Tests.Handlers.Commands;

public class SendUserMessageCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IChatRepository> _mockChatRepository;
    private readonly Mock<IMessageRepository> _mockMessageRepository;
    private readonly Mock<IOpenAiService> _mockOpenAiService;
    private readonly SendUserMessageCommandHandler _handler;

    public SendUserMessageCommandHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockChatRepository = new Mock<IChatRepository>();
        _mockMessageRepository = new Mock<IMessageRepository>();
        _mockOpenAiService = new Mock<IOpenAiService>();

        _mockUnitOfWork.Setup(x => x.ChatRepository).Returns(_mockChatRepository.Object);
        _mockUnitOfWork.Setup(x => x.MessageRepository).Returns(_mockMessageRepository.Object);

        _handler = new SendUserMessageCommandHandler(
            _mockUnitOfWork.Object,
            _mockOpenAiService.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidChatAndMessage_ShouldCreateUserMessageAndBotResponse()
    {
        var chatId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var botId = Guid.NewGuid();
        var messageContent = "Olá, como vai você?";
        var botResponseContent = "Estou bem, obrigado!";

        var chat = new Chat
        {
            Id = chatId,
            Name = "Test Chat",
            Description = "Você é um assistente utilitário",
            BotId = botId,
            UserId = userId,
            CreatedBy = userId,
            CreatedAt = DateTime.Now,
        };

        var userMessage = new Message
        {
            Id = Guid.NewGuid(),
            Content = messageContent,
            ChatId = chatId,
            IsUserMessage = true,
            CreatedAt = DateTime.Now,
        };

        var botMessage = new Message
        {
            Id = Guid.NewGuid(),
            Content = botResponseContent,
            ChatId = chatId,
            IsUserMessage = false,
            CreatedAt = DateTime.Now,
        };

        var command = new SendUserMessageCommand { ChatId = chatId, Content = messageContent };

        _mockChatRepository.Setup(x => x.GetChatById(chatId)).ReturnsAsync(chat);

        _mockMessageRepository
            .Setup(x => x.AddMessage(It.Is<Message>(m => m.IsUserMessage == true)))
            .ReturnsAsync(userMessage);

        _mockMessageRepository
            .Setup(x => x.AddMessage(It.Is<Message>(m => m.IsUserMessage == false)))
            .ReturnsAsync(botMessage);

        _mockOpenAiService
            .Setup(x => x.GetBotResponse(chat.Description, messageContent, chatId))
            .ReturnsAsync(botResponseContent);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(botResponseContent, result.Content);
        Assert.Equal(chatId, result.ChatId);
        Assert.False(result.IsUserMessage);

        _mockChatRepository.Verify(x => x.GetChatById(chatId), Times.Once);
        _mockMessageRepository.Verify(
            x =>
                x.AddMessage(
                    It.Is<Message>(m =>
                        m.Content == messageContent && m.ChatId == chatId && m.IsUserMessage == true
                    )
                ),
            Times.Once
        );
        _mockUnitOfWork.Verify(x => x.CommitAsync(), Times.Once);
        _mockOpenAiService.Verify(
            x => x.GetBotResponse(chat.Description, messageContent, chatId),
            Times.Once
        );
        _mockMessageRepository.Verify(
            x =>
                x.AddMessage(
                    It.Is<Message>(m =>
                        m.Content == botResponseContent
                        && m.ChatId == chatId
                        && m.IsUserMessage == false
                    )
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_WithNonExistentChat_ShouldThrowException()
    {
        var chatId = Guid.NewGuid();
        var messageContent = "Olá, como vai?";

        var command = new SendUserMessageCommand { ChatId = chatId, Content = messageContent };

        _mockChatRepository.Setup(x => x.GetChatById(chatId)).ReturnsAsync((Chat?)null);

        var exception = await Assert.ThrowsAsync<Exception>(() =>
            _handler.Handle(command, CancellationToken.None)
        );

        Assert.Equal("ChatNotExists", exception.Message);

        _mockChatRepository.Verify(x => x.GetChatById(chatId), Times.Once);
        _mockMessageRepository.Verify(x => x.AddMessage(It.IsAny<Message>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.CommitAsync(), Times.Never);
        _mockOpenAiService.Verify(
            x => x.GetBotResponse(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()),
            Times.Never
        );
    }

    [Fact]
    public async Task Handle_WhenOpenAiServiceFails_ShouldPropagateException()
    {
        var chatId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var botId = Guid.NewGuid();
        var messageContent = "Olá, como vai?";

        var chat = new Chat
        {
            Id = chatId,
            Name = "Test Chat",
            Description = "Você é um assistente utilitário",
            BotId = botId,
            UserId = userId,
            CreatedBy = userId,
            CreatedAt = DateTime.Now,
        };

        var userMessage = new Message
        {
            Id = Guid.NewGuid(),
            Content = messageContent,
            ChatId = chatId,
            IsUserMessage = true,
            CreatedAt = DateTime.Now,
        };

        var command = new SendUserMessageCommand { ChatId = chatId, Content = messageContent };

        _mockChatRepository.Setup(x => x.GetChatById(chatId)).ReturnsAsync(chat);

        _mockMessageRepository
            .Setup(x => x.AddMessage(It.Is<Message>(m => m.IsUserMessage == true)))
            .ReturnsAsync(userMessage);

        _mockOpenAiService
            .Setup(x => x.GetBotResponse(chat.Description, messageContent, chatId))
            .ThrowsAsync(new Exception("OpenAI API Error"));

        var exception = await Assert.ThrowsAsync<Exception>(() =>
            _handler.Handle(command, CancellationToken.None)
        );

        Assert.Equal("OpenAI API Error", exception.Message);

        _mockChatRepository.Verify(x => x.GetChatById(chatId), Times.Once);
        _mockMessageRepository.Verify(
            x => x.AddMessage(It.Is<Message>(m => m.IsUserMessage == true)),
            Times.Once
        );
        _mockUnitOfWork.Verify(x => x.CommitAsync(), Times.Once);
        _mockOpenAiService.Verify(
            x => x.GetBotResponse(chat.Description, messageContent, chatId),
            Times.Once
        );
        _mockMessageRepository.Verify(
            x => x.AddMessage(It.Is<Message>(m => m.IsUserMessage == false)),
            Times.Never
        );
    }
}
