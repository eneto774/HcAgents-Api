using HcAgents.Domain.Abstractions;
using HcAgents.Domain.Entities;
using HcAgents.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace HcAgents.Infrastructure.Repositories;

public class BotRepository : IBotRepository
{
    private readonly AppDbContext _context;

    public BotRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Bot>> GetBots()
    {
        return await _context.Bots.ToListAsync();
    }

    public async Task<Bot?> GetBotById(Guid id)
    {
        return await _context.Bots.FindAsync(id);
    }

    public async Task<Bot> AddBot(Bot bot)
    {
        await _context.Bots.AddAsync(bot);
        await _context.SaveChangesAsync();
        return bot;
    }

    public void UpdateBot(Bot bot)
    {
        _context.Bots.Update(bot);
    }

    public async Task<Bot?> DeleteBot(Guid id)
    {
        var Bot = await _context.Bots.FindAsync(id);
        if (Bot != null)
        {
            _context.Bots.Remove(Bot);
            await _context.SaveChangesAsync();
            return Bot;
        }

        return null;
    }

    public async Task<IEnumerable<Bot>> GetBotsByUserId(Guid userId)
    {
        return await _context.Bots.Where(x => x.CreatedBy == userId).ToListAsync();
    }
}
