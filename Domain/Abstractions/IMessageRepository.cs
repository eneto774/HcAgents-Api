using HcAgents.Domain.Entities;

namespace HcAgents.Domain.Abstractions;

public interface IMessageRepository
{
    Task<IEnumerable<Message>> GetMessages();
    Task<IEnumerable<Message>> GetMessagesByChatId(Guid chatId);
    Task<Message?> GetMessageById(Guid id);
    Task<Message> AddMessage(Message message);
    void UpdateMessage(Message message);
    Task<Message?> DeleteMessage(Guid id);
}
