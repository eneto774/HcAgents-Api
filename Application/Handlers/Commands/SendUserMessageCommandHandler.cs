using HcAgents.Domain.Abstractions;
using HcAgents.Domain.Entities;
using MediatR;

public class SendUserMessageCommandHandler : IRequestHandler<SendUserMessageCommand, Message>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOpenAiService _openAiService;

    public SendUserMessageCommandHandler(IUnitOfWork unitOfWork, IOpenAiService openAiService)
    {
        _unitOfWork = unitOfWork;
        _openAiService = openAiService;
    }

    public async Task<Message> Handle(
        SendUserMessageCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var chat = await _unitOfWork.ChatRepository.GetChatById(request.ChatId);

            if (chat == null)
            {
                throw new Exception("ChatNotExists");
            }

            var message = await _unitOfWork.MessageRepository.AddMessage(
                new Message
                {
                    Content = request.Content,
                    ChatId = request.ChatId,
                    IsUserMessage = true,
                    CreatedAt = DateTime.Now,
                }
            );

            await _unitOfWork.CommitAsync();

            var botResponse = await _openAiService.GetBotResponse(
                chat.Description,
                message.Content,
                chat.Id
            );

            var createdBotMessage = await _unitOfWork.MessageRepository.AddMessage(
                new Message
                {
                    Content = botResponse,
                    ChatId = request.ChatId,
                    IsUserMessage = false,
                    CreatedAt = DateTime.Now,
                }
            );

            return createdBotMessage;
        }
        catch (Exception)
        {
            throw;
        }
    }
}
