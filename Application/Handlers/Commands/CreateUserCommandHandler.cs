using HcAgents.Domain.Abstractions;
using HcAgents.Domain.Commands;
using HcAgents.Domain.Entities;
using MediatR;

namespace HcAgents.Application.Handlers.Commands;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, User>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public CreateUserCommandHandler(IUnitOfWork unitOfWork, IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<User> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userAlreadyExists = await _unitOfWork.UserRepository.GetUserByEmail(request.Email);
            if (userAlreadyExists != null)
            {
                throw new Exception("UserAlreadyExists");
            }

            var user = await _unitOfWork.UserRepository.AddUser(
                new User
                {
                    Name = request.Name,
                    Email = request.Email,
                    Secret = Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.Now,
                }
            );

            await _unitOfWork.CommitAsync();

            return user;
        }
        catch (Exception)
        {
            throw;
        }
    }
}
