using HcAgents.Domain.Abstractions;
using HcAgents.Infrastructure.Repositories;

namespace HcAgents.Infrastructure.Database;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly IUserRepository? _userRepository;
    private readonly IBotRepository? _botRepository;
    private readonly IChatRepository? _chatRepository;
    private readonly IMessageRepository? _messageRepository;
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IUserRepository UserRepository
    {
        get { return _userRepository ?? new UserRepository(_context); }
    }

    public IBotRepository BotRepository
    {
        get { return _botRepository ?? new BotRepository(_context); }
    }

    public IChatRepository ChatRepository
    {
        get { return _chatRepository ?? new ChatRepository(_context); }
    }

    public IMessageRepository MessageRepository
    {
        get { return _messageRepository ?? new MessageRepository(_context); }
    }

    public async Task CommitAsync()
    {
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
