using HcAgents.Domain.Entities;

namespace HcAgents.Domain.Models.Response;

public class CreateSessionResponse
{
    public required Guid UserId { get; set; }
    public required User User { get; set; }
    public required string AccessToken { get; set; }
}
