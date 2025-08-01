using HcAgents.Domain.Entities;
using MediatR;

public class SendUserMessageCommand : IRequest<Message>
{
    public required Guid ChatId { get; set; }
    public required string Content { get; set; }
}
