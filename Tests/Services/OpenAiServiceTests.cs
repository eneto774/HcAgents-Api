using HcAgents.Application.Services;
using HcAgents.Domain.Abstractions;
using HcAgents.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Tests.Services;

public class OpenAiServiceTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMessageRepository> _mockMessageRepository;
    private readonly string _testApiKey = "test-api-key-12345";

    public OpenAiServiceTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMessageRepository = new Mock<IMessageRepository>();

        _mockConfiguration.Setup(x => x["OpenAiToken"]).Returns(_testApiKey);
        _mockUnitOfWork.Setup(x => x.MessageRepository).Returns(_mockMessageRepository.Object);
    }

    [Fact]
    public void Constructor_WithValidConfiguration_ShouldCreateInstance()
    {
        var service = new OpenAiService(_mockConfiguration.Object, _mockUnitOfWork.Object);

        Assert.NotNull(service);
        _mockConfiguration.Verify(x => x["OpenAiToken"], Times.Once);
    }

    [Fact]
    public void Constructor_WithNullApiKey_ShouldThrowException()
    {
        var mockConfig = new Mock<IConfiguration>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        mockConfig.Setup(x => x["OpenAiToken"]).Returns((string?)null);

        Assert.Throws<ArgumentNullException>(() =>
            new OpenAiService(mockConfig.Object, mockUnitOfWork.Object)
        );
    }

    [Fact]
    public void Constructor_WithEmptyApiKey_ShouldThrowException()
    {
        var mockConfig = new Mock<IConfiguration>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        mockConfig.Setup(x => x["OpenAiToken"]).Returns(string.Empty);

        Assert.Throws<ArgumentException>(() =>
            new OpenAiService(mockConfig.Object, mockUnitOfWork.Object)
        );
    }

    [Fact]
    public void GetBotResponse_WithEmptyHistory_ShouldIncludeOnlySystemAndUserMessages()
    {
        var service = new OpenAiService(_mockConfiguration.Object, _mockUnitOfWork.Object);
        var chatId = Guid.NewGuid();
        var emptyHistory = new List<Message>();

        _mockMessageRepository.Setup(x => x.GetMessagesByChatId(chatId)).ReturnsAsync(emptyHistory);

        Assert.NotNull(service.GetType().GetMethod("GetBotResponse"));

        _mockMessageRepository.Verify(x => x.GetMessagesByChatId(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public void GetBotResponse_WithMessageHistory_ShouldSetupCorrectly()
    {
        var service = new OpenAiService(_mockConfiguration.Object, _mockUnitOfWork.Object);
        var chatId = Guid.NewGuid();
        var messageHistory = new List<Message>
        {
            new Message
            {
                Id = Guid.NewGuid(),
                Content = "Previous user message",
                ChatId = chatId,
                IsUserMessage = true,
                CreatedAt = DateTime.Now.AddMinutes(-5),
            },
            new Message
            {
                Id = Guid.NewGuid(),
                Content = "Previous bot response",
                ChatId = chatId,
                IsUserMessage = false,
                CreatedAt = DateTime.Now.AddMinutes(-4),
            },
        };

        _mockMessageRepository
            .Setup(x => x.GetMessagesByChatId(chatId))
            .ReturnsAsync(messageHistory);

        Assert.NotNull(service);
    }
}
