public interface IOtpService
{
    string GenerateOtp(string key);
    bool ValidateOtp(string key, string otp);
    void RemoveOtp(string key);
}
