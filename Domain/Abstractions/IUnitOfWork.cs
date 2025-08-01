namespace HcAgents.Domain.Abstractions;

public interface IUnitOfWork
{
    IUserRepository UserRepository { get; }
    IChatRepository ChatRepository { get; }
    IBotRepository BotRepository { get; }
    IMessageRepository MessageRepository { get; }
    Task CommitAsync();
}
