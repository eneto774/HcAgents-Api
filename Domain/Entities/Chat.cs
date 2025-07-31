namespace HcAgents.Domain.Entities;

public class Chat : BaseEntity
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required Guid BotId { get; set; }
    public required Guid UserId { get; set; }
    public required Guid CreatedBy { get; set; }
}
