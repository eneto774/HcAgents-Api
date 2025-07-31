namespace HcAgents.Domain.Entities;

public class Message : BaseEntity
{
    public required string Content { get; set; }
    public required Guid ChatId { get; set; }
    public required bool IsUserMessage { get; set; }
}
