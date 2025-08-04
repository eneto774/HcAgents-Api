using HcAgents.Domain.Abstractions;
using HcAgents.Domain.Entities;
using Moq;

namespace HcAgents.Tests.Handlers.Queries;

public class GetAllMessagesHistoryByChatIdQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMessageRepository> _mockMessageRepository;
    private readonly GetAllMessagesHistoryByChatIdQueryHandler _handler;

    public GetAllMessagesHistoryByChatIdQueryHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMessageRepository = new Mock<IMessageRepository>();
        _mockUnitOfWork.Setup(x => x.MessageRepository).Returns(_mockMessageRepository.Object);
        _handler = new GetAllMessagesHistoryByChatIdQueryHandler(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_WithValidChatId_ShouldReturnMessagesForChat()
    {
        var chatId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var createdAt = DateTime.Now;

        var expectedMessages = new List<Message>
        {
            new Message
            {
                Id = Guid.NewGuid(),
                Content = "Olá como vai você?",
                ChatId = chatId,
                IsUserMessage = true,
                CreatedAt = createdAt
            },
            new Message
            {
                Id = Guid.NewGuid(),
                Content = "Estou bem, obrigado! Como posso ajudar você hoje?",
                ChatId = chatId,
                IsUserMessage = false,
                CreatedAt = createdAt.AddMinutes(1)
            },
            new Message
            {
                Id = Guid.NewGuid(),
                Content = "Você pode me ajudar com um problema de codigo?",
                ChatId = chatId,
                IsUserMessage = true,
                CreatedAt = createdAt.AddMinutes(2)
            }
        };

        var query = new GetAllMessagesHistoryByChatIdQuery
        {
            ChatId = chatId,
            ItensPerPage = 25,
            Page = 1
        };

        _mockMessageRepository
            .Setup(x => x.GetMessagesByChatId(chatId))
            .ReturnsAsync(expectedMessages);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
        Assert.All(result, message => Assert.Equal(chatId, message.ChatId));
        
        var messagesList = result.ToList();
        Assert.True(messagesList[0].IsUserMessage);
        Assert.False(messagesList[1].IsUserMessage);
        Assert.True(messagesList[2].IsUserMessage);

        _mockMessageRepository.Verify(x => x.GetMessagesByChatId(chatId), Times.Once);
    }

    [Fact]
    public async Task Handle_WithChatIdThatHasNoMessages_ShouldReturnEmptyList()
    {
        var chatId = Guid.NewGuid();
        var emptyMessages = new List<Message>();

        var query = new GetAllMessagesHistoryByChatIdQuery
        {
            ChatId = chatId,
            ItensPerPage = 25,
            Page = 1
        };

        _mockMessageRepository
            .Setup(x => x.GetMessagesByChatId(chatId))
            .ReturnsAsync(emptyMessages);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
        
        _mockMessageRepository.Verify(x => x.GetMessagesByChatId(chatId), Times.Once);
    }

    [Fact]
    public async Task Handle_WithDifferentChatIds_ShouldOnlyReturnMessagesForSpecificChat()
    {
        var targetChatId = Guid.NewGuid();
        var otherChatId = Guid.NewGuid();
        var createdAt = DateTime.Now;

        var messagesForTargetChat = new List<Message>
        {
            new Message
            {
                Id = Guid.NewGuid(),
                Content = "Mensagem para o Chat",
                ChatId = targetChatId,
                IsUserMessage = true,
                CreatedAt = createdAt
            },
            new Message
            {
                Id = Guid.NewGuid(),
                Content = "Resposta do Chat Bot para o Chat",
                ChatId = targetChatId,
                IsUserMessage = false,
                CreatedAt = createdAt.AddMinutes(1)
            }
        };

        var query = new GetAllMessagesHistoryByChatIdQuery
        {
            ChatId = targetChatId,
            ItensPerPage = 25,
            Page = 1
        };

        _mockMessageRepository
            .Setup(x => x.GetMessagesByChatId(targetChatId))
            .ReturnsAsync(messagesForTargetChat);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, message => Assert.Equal(targetChatId, message.ChatId));
        
        _mockMessageRepository.Verify(x => x.GetMessagesByChatId(targetChatId), Times.Once);
        _mockMessageRepository.Verify(x => x.GetMessagesByChatId(otherChatId), Times.Never);
    }

    [Fact]
    public async Task Handle_WithMixedMessageTypes_ShouldReturnBothUserAndBotMessages()
    {
        var chatId = Guid.NewGuid();
        var createdAt = DateTime.Now;

        var mixedMessages = new List<Message>
        {
            new Message
            {
                Id = Guid.NewGuid(),
                Content = "User message 1",
                ChatId = chatId,
                IsUserMessage = true,
                CreatedAt = createdAt
            },
            new Message
            {
                Id = Guid.NewGuid(),
                Content = "Bot response 1",
                ChatId = chatId,
                IsUserMessage = false,
                CreatedAt = createdAt.AddMinutes(1)
            },
            new Message
            {
                Id = Guid.NewGuid(),
                Content = "User message 2",
                ChatId = chatId,
                IsUserMessage = true,
                CreatedAt = createdAt.AddMinutes(2)
            },
            new Message
            {
                Id = Guid.NewGuid(),
                Content = "Bot response 2",
                ChatId = chatId,
                IsUserMessage = false,
                CreatedAt = createdAt.AddMinutes(3)
            }
        };

        var query = new GetAllMessagesHistoryByChatIdQuery
        {
            ChatId = chatId,
            ItensPerPage = 25,
            Page = 1
        };

        _mockMessageRepository
            .Setup(x => x.GetMessagesByChatId(chatId))
            .ReturnsAsync(mixedMessages);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(4, result.Count());
        
        var messagesList = result.ToList();
        var userMessages = messagesList.Where(m => m.IsUserMessage).ToList();
        var botMessages = messagesList.Where(m => !m.IsUserMessage).ToList();
        
        Assert.Equal(2, userMessages.Count);
        Assert.Equal(2, botMessages.Count);
        
        Assert.All(userMessages, m => Assert.True(m.IsUserMessage));
        Assert.All(botMessages, m => Assert.False(m.IsUserMessage));
        
        _mockMessageRepository.Verify(x => x.GetMessagesByChatId(chatId), Times.Once);
    }

    [Fact]
    public async Task Handle_WithPaginationParameters_ShouldStillCallRepositoryCorrectly()
    {
        var chatId = Guid.NewGuid();
        var messages = new List<Message>
        {
            new Message
            {
                Id = Guid.NewGuid(),
                Content = "Test message",
                ChatId = chatId,
                IsUserMessage = true,
                CreatedAt = DateTime.Now
            }
        };

        var query = new GetAllMessagesHistoryByChatIdQuery
        {
            ChatId = chatId,
            ItensPerPage = 10,
            Page = 2
        };

        _mockMessageRepository
            .Setup(x => x.GetMessagesByChatId(chatId))
            .ReturnsAsync(messages);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result);
        
        _mockMessageRepository.Verify(x => x.GetMessagesByChatId(chatId), Times.Once);
    }
}
