using HcAgents.Domain.Entities;
using HcAgents.Infrastructure.Mapping;
using Microsoft.EntityFrameworkCore;

namespace HcAgents.Infrastructure.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<Bot> Bots { get; set; }
    public DbSet<Message> Messages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging();
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration<User>(new UserMapping());
        modelBuilder.ApplyConfiguration<Chat>(new ChatMapping());
        modelBuilder.ApplyConfiguration<Bot>(new BotMapping());
        modelBuilder.ApplyConfiguration<Message>(new MessageMapping());
    }
}
