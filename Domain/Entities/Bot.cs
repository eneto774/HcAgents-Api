namespace HcAgents.Domain.Entities;

public class Bot : BaseEntity
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required bool Active { get; set; } = true;
    public required Guid CreatedBy { get; set; }
}
