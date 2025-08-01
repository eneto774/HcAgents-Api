using System.Data.SqlTypes;
using HcAgents.Domain.Entities;

namespace HcAgents.Domain.Abstractions;

public interface IBotRepository
{
    Task<IEnumerable<Bot>> GetBots();
    Task<Bot?> GetBotById(Guid id);
    Task<Bot> AddBot(Bot bot);
    void UpdateBot(Bot bot);
    Task<Bot?> DeleteBot(Guid id);
    Task<IEnumerable<Bot>> GetBotsByUserId(Guid userId);
}
