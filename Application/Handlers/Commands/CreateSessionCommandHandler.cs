using HcAgents.Domain.Abstractions;
using HcAgents.Domain.Models.Response;
using MediatR;

namespace HcAgents.Application.Handlers.Commands;

public class CreateSessionCommandHandler
    : IRequestHandler<CreateSessionCommand, CreateSessionResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOtpService _otpService;
    private readonly IJwtService _jwtService;

    public CreateSessionCommandHandler(
        IUnitOfWork unitOfWork,
        IOtpService otpService,
        IJwtService jwtService
    )
    {
        _unitOfWork = unitOfWork;
        _otpService = otpService;
        _jwtService = jwtService;
    }

    public async Task<CreateSessionResponse> Handle(
        CreateSessionCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var userExists = await _unitOfWork.UserRepository.GetUserByEmail(request.Email);

            if (userExists == null)
            {
                throw new Exception("UserNotExists");
            }

            if (!_otpService.ValidateOtp(request.Email, request.Otp))
            {
                throw new Exception("InvalidOtp");
            }

            _otpService.RemoveOtp(request.Email);

            var accessToken = _jwtService.GenerateToken(userExists.Id.ToString(), userExists.Email);

            userExists.Secret = null;

            return new CreateSessionResponse
            {
                UserId = userExists.Id,
                User = userExists,
                AccessToken = accessToken,
            };
        }
        catch (Exception)
        {
            throw;
        }
    }
}
