using HcAgents.Domain.Abstractions;
using HcAgents.Domain.Entities;
using HcAgents.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace HcAgents.Infrastructure.Repositories;

public class ChatRepository : IChatRepository
{
    private readonly AppDbContext _context;

    public ChatRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Chat>> GetChats()
    {
        return await _context.Chats.ToListAsync();
    }

    public async Task<Chat?> GetChatById(Guid id)
    {
        return await _context.Chats.FindAsync(id);
    }

    public async Task<Chat> AddChat(Chat chat)
    {
        await _context.Chats.AddAsync(chat);
        await _context.SaveChangesAsync();
        return chat;
    }

    public void UpdateChat(Chat chat)
    {
        _context.Chats.Update(chat);
    }

    public async Task<Chat?> DeleteChat(Guid id)
    {
        var Chat = await _context.Chats.FindAsync(id);
        if (Chat != null)
        {
            _context.Chats.Remove(Chat);
            await _context.SaveChangesAsync();
            return Chat;
        }

        return null;
    }

    public async Task<IEnumerable<Chat>> GetChatsByUserId(Guid userId)
    {
        return await _context.Chats.Where(x => x.UserId == userId).ToListAsync();
    }
}
