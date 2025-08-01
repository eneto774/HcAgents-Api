using HcAgents.Domain.Abstractions;
using HcAgents.Domain.Entities;
using HcAgents.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace HcAgents.Infrastructure.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly AppDbContext _context;

    public MessageRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Message>> GetMessages()
    {
        return await _context.Messages.ToListAsync();
    }

    public async Task<Message?> GetMessageById(Guid id)
    {
        return await _context.Messages.FindAsync(id);
    }

    public async Task<Message> AddMessage(Message Message)
    {
        await _context.Messages.AddAsync(Message);
        await _context.SaveChangesAsync();
        return Message;
    }

    public void UpdateMessage(Message Message)
    {
        _context.Messages.Update(Message);
    }

    public async Task<Message?> DeleteMessage(Guid id)
    {
        var Message = await _context.Messages.FindAsync(id);
        if (Message != null)
        {
            _context.Messages.Remove(Message);
            await _context.SaveChangesAsync();
            return Message;
        }

        return null;
    }

    public async Task<IEnumerable<Message>> GetMessagesByChatId(Guid chatId)
    {
        return await _context.Messages.Where(x => x.ChatId == chatId).ToListAsync();
    }
}
