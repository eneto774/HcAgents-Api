using HcAgents.Domain.Abstractions;
using HcAgents.Domain.Entities;
using HcAgents.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace HcAgents.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetUsers()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User?> GetUserById(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        return await _context.Users.FirstAsync(x => x.Email == email);
    }

    public async Task<User> AddUser(User User)
    {
        await _context.Users.AddAsync(User);
        await _context.SaveChangesAsync();
        return User;
    }

    public void UpdateUser(User User)
    {
        _context.Users.Update(User);
    }

    public async Task<User?> DeleteUser(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return user;
        }

        return null;
    }
}
