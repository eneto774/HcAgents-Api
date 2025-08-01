using HcAgents.Domain.Abstractions;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;

namespace HcAgents.Application.Services;

public class OpenAiService : IOpenAiService
{
    private readonly ChatClient _chatClient;
    private readonly IUnitOfWork _unitOfWork;

    public OpenAiService(IConfiguration configuration, IUnitOfWork unitOfWork)
    {
        var apiKey = configuration["OpenAiToken"];
        _chatClient = new ChatClient("gpt-3.5-turbo", apiKey);
        _unitOfWork = unitOfWork;
    }

    public async Task<string> GetBotResponse(string initialContext, string prompt, Guid chatId)
    {
        var messages = new List<ChatMessage> { new SystemChatMessage(initialContext) };

        var historyMessages = await _unitOfWork.MessageRepository.GetMessagesByChatId(chatId);

        foreach (var message in historyMessages)
        {
            if (message.IsUserMessage)
            {
                messages.Add(new UserChatMessage(message.Content));
            }
            else
            {
                messages.Add(new SystemChatMessage(message.Content));
            }
        }

        messages.Add(new UserChatMessage(prompt));

        var completion = await _chatClient.CompleteChatAsync(messages);

        if (completion.Value.Content.Count > 1)
        {
            return string.Join(" ", completion.Value.Content.Select(x => x.Text));
        }

        return string.Empty;
    }
}
