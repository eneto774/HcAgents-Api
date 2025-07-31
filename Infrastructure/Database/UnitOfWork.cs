using HcAgents.Domain.Abstractions;
using HcAgents.Infrastructure.Repositories;

namespace HcAgents.Infrastructure.Database;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly IUserRepository? _userRepository;
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IUserRepository UserRepository
    {
        get { return _userRepository ?? new UserRepository(_context); }
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
