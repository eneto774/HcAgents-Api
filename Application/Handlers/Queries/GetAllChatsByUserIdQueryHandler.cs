using HcAgents.Domain.Abstractions;
using HcAgents.Domain.Entities;
using MediatR;

public class GetAllChatsByUserIdQueryHandler
    : IRequestHandler<GetAllChatsByUserIdQuery, IEnumerable<Chat>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllChatsByUserIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Chat>> Handle(
        GetAllChatsByUserIdQuery request,
        CancellationToken cancellationToken
    )
    {
        return await _unitOfWork.ChatRepository.GetChatsByUserId(request.UserId);
    }
}
