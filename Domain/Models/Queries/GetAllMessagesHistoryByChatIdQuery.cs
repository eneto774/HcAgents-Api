using HcAgents.Domain.Entities;
using MediatR;

public class GetAllMessagesHistoryByChatIdQuery : IRequest<IEnumerable<Message>>
{
    public required Guid ChatId { get; set; }
    public int? ItensPerPage { get; set; } = 25;
    public int? Page { get; set; } = 1;
}
