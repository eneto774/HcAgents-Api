using HcAgents.Domain.Abstractions;
using HcAgents.Domain.Commands;
using HcAgents.Domain.Entities;
using MediatR;

public class CreateChatAndBotCommandHandler : IRequestHandler<CreateChatAndBotCommand, Chat>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateChatAndBotCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Chat> Handle(
        CreateChatAndBotCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var bot = await _unitOfWork.BotRepository.AddBot(
                new Bot
                {
                    Name = request.BotName,
                    Description = request.BotDescription,
                    CreatedBy = request.UserId,
                    CreatedAt = DateTime.Now,
                }
            );

            var chat = await _unitOfWork.ChatRepository.AddChat(
                new Chat
                {
                    Name = request.ChatName,
                    Description = request.ChatDescription,
                    BotId = bot.Id,
                    UserId = request.UserId,
                    CreatedBy = request.UserId,
                    CreatedAt = DateTime.Now,
                }
            );

            await _unitOfWork.CommitAsync();

            return chat;
        }
        catch (Exception)
        {
            throw;
        }
    }
}
