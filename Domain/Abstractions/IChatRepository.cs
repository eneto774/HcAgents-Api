using HcAgents.Domain.Entities;

namespace HcAgents.Domain.Abstractions;

public interface IChatRepository
{
    Task<IEnumerable<Chat>> GetChats();
    Task<IEnumerable<Chat>> GetChatsByUserId(Guid userId);
    Task<Chat?> GetChatById(Guid id);
    Task<Chat> AddChat(Chat chat);
    void UpdateChat(Chat chat);
    Task<Chat?> DeleteChat(Guid id);
}
