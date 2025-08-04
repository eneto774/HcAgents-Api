using HcAgents.Domain.Abstractions;
using HcAgents.Domain.Entities;
using Moq;

namespace HcAgents.Tests.Handlers.Queries;

public class GetAllChatsByUserIdQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IChatRepository> _mockChatRepository;
    private readonly GetAllChatsByUserIdQueryHandler _handler;

    public GetAllChatsByUserIdQueryHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockChatRepository = new Mock<IChatRepository>();
        _mockUnitOfWork.Setup(x => x.ChatRepository).Returns(_mockChatRepository.Object);
        _handler = new GetAllChatsByUserIdQueryHandler(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_WithValidUserId_ShouldReturnChatsForUser()
    {
        var userId = Guid.NewGuid();
        var botId = Guid.NewGuid();
        var createdBy = Guid.NewGuid();
        var createdAt = DateTime.Now;

        var expectedChats = new List<Chat>
        {
            new Chat
            {
                Id = Guid.NewGuid(),
                Name = "Chat 1",
                Description = "First chat",
                BotId = botId,
                UserId = userId,
                CreatedBy = createdBy,
                CreatedAt = createdAt
            },
            new Chat
            {
                Id = Guid.NewGuid(),
                Name = "Chat 2",
                Description = "Second chat",
                BotId = botId,
                UserId = userId,
                CreatedBy = createdBy,
                CreatedAt = createdAt
            }
        };

        var query = new GetAllChatsByUserIdQuery
        {
            UserId = userId,
            Search = "",
            ItensPerPage = 10,
            Page = 1
        };

        _mockChatRepository
            .Setup(x => x.GetChatsByUserId(userId))
            .ReturnsAsync(expectedChats);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, chat => Assert.Equal(userId, chat.UserId));
        
        _mockChatRepository.Verify(x => x.GetChatsByUserId(userId), Times.Once);
    }

    [Fact]
    public async Task Handle_WithUserIdThatHasNoChats_ShouldReturnEmptyList()
    {
        var userId = Guid.NewGuid();
        var emptyChats = new List<Chat>();

        var query = new GetAllChatsByUserIdQuery
        {
            UserId = userId,
            Search = "",
            ItensPerPage = 10,
            Page = 1
        };

        _mockChatRepository
            .Setup(x => x.GetChatsByUserId(userId))
            .ReturnsAsync(emptyChats);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
        
        _mockChatRepository.Verify(x => x.GetChatsByUserId(userId), Times.Once);
    }

    [Fact]
    public async Task Handle_WithDifferentUserIds_ShouldOnlyReturnChatsForSpecificUser()
    {
        var targetUserId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var botId = Guid.NewGuid();
        var createdBy = Guid.NewGuid();
        var createdAt = DateTime.Now;

        var chatsForTargetUser = new List<Chat>
        {
            new Chat
            {
                Id = Guid.NewGuid(),
                Name = "Target User Chat",
                Description = "Chat for target user",
                BotId = botId,
                UserId = targetUserId,
                CreatedBy = createdBy,
                CreatedAt = createdAt
            }
        };

        var query = new GetAllChatsByUserIdQuery
        {
            UserId = targetUserId,
            Search = "",
            ItensPerPage = 10,
            Page = 1
        };

        _mockChatRepository
            .Setup(x => x.GetChatsByUserId(targetUserId))
            .ReturnsAsync(chatsForTargetUser);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.All(result, chat => Assert.Equal(targetUserId, chat.UserId));
        
        _mockChatRepository.Verify(x => x.GetChatsByUserId(targetUserId), Times.Once);
        _mockChatRepository.Verify(x => x.GetChatsByUserId(otherUserId), Times.Never);
    }
}
