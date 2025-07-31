using HcAgents.Domain.Entities;
using MediatR;

namespace HcAgents.Domain.Commands;

public class CreateUserCommand : IRequest<User>
{
    public required string Name { get; set; }
    public required string Email { get; set; }
}
