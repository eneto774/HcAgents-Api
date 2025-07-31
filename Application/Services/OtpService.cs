using System.Security.Cryptography;
using HcAgents.Domain.Abstractions;

namespace HcAgents.Application.Services;

public class OtpService : IOtpService
{
    private readonly IMemoryCacheRepository _memoryCacheRepository;

    public OtpService(IMemoryCacheRepository memoryCacheRepository)
    {
        _memoryCacheRepository = memoryCacheRepository;
    }

    public string GenerateOtp(string key)
    {
        var otpBytes = new byte[4];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(otpBytes);
        }
        var otp = BitConverter.ToUInt32(otpBytes, 0) % 1_000_000;
        var otpString = otp.ToString("D6");

        _memoryCacheRepository.Set(key, otpString, TimeSpan.FromMinutes(5));

        return otpString;
    }

    public void RemoveOtp(string key)
    {
        _memoryCacheRepository.Remove(key);
    }

    public bool ValidateOtp(string key, string otp)
    {
        var storedOtp = _memoryCacheRepository.Get<string?>(key);
        if (!String.IsNullOrEmpty(storedOtp))
        {
            return string.Equals(otp, storedOtp, StringComparison.Ordinal);
        }
        return false;
    }
}
