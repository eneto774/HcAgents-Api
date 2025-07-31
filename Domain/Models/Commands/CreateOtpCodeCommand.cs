using MediatR;

public class CreateOtpCodeCommand : IRequest<bool>
{
    public required string Email { get; set; }
}
