using HcAgents.Domain.Abstractions;
using MediatR;

public class CreateOtpCodeCommandHandler : IRequestHandler<CreateOtpCodeCommand, bool>
{
    private readonly IOtpService _otpService;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOtpCodeCommandHandler(
        IOtpService otpService,
        IEmailService emailService,
        IUnitOfWork unitOfWork
    )
    {
        _otpService = otpService;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(
        CreateOtpCodeCommand request,
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

            var generatedOtp = _otpService.GenerateOtp(request.Email);
            _emailService.SendEmail(
                request.Email,
                "HCAgents - OTP Code",
                $"Your OTP code is: {generatedOtp}"
            );
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
