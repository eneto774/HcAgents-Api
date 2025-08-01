using HcAgents.Domain.Entities;
using MediatR;

public class GetAllChatsByUserIdQuery : IRequest<IEnumerable<Chat>>
{
    public required Guid UserId { get; set; }
    public string Search { get; set; } = string.Empty;
    public int? ItensPerPage { get; set; } = 10;
    public int? Page { get; set; } = 1;
}
