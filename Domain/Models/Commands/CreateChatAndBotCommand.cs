using HcAgents.Domain.Entities;
using MediatR;

namespace HcAgents.Domain.Commands;

public class CreateChatAndBotCommand : IRequest<Chat>
{
    public required string ChatName { get; set; }
    public required string ChatDescription { get; set; }
    public required string BotName { get; set; }
    public required string BotDescription { get; set; }
    public required Guid UserId { get; set; }
}
