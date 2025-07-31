
using HcAgents.Domain.Entities;

namespace HcAgents.Domain.Abstractions;
public interface IUserRepository
{
    Task<IEnumerable<User>> GetUsers();
    Task<User?> GetUserById(int id);
    Task<User?> GetUserByEmail(string email);
    Task<User> AddUser(User User);
    void UpdateUser(User User);
    Task<User?> DeleteUser(int id);
}