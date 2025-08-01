using HcAgents.Domain.Abstractions;
using HcAgents.Domain.Entities;
using MediatR;

public class GetAllMessagesHistoryByChatIdQueryHandler
    : IRequestHandler<GetAllMessagesHistoryByChatIdQuery, IEnumerable<Message>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllMessagesHistoryByChatIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Message>> Handle(
        GetAllMessagesHistoryByChatIdQuery query,
        CancellationToken cancellationToken
    )
    {
        return await _unitOfWork.MessageRepository.GetMessagesByChatId(query.ChatId);
    }
}
