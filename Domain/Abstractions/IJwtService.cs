namespace HcAgents.Domain.Abstractions;

public interface IJwtService
{
    string GenerateToken(string userId, string email);
}
