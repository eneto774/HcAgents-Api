using HcAgents.Domain.Models.Response;
using MediatR;

public class CreateSessionCommand : IRequest<CreateSessionResponse>
{
    public required string Email { get; set; }
    public required string Otp { get; set; }
}
