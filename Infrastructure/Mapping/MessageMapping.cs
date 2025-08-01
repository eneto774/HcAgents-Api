using HcAgents.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HcAgents.Infrastructure.Mapping;

public class MessageMapping : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("message");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnType("char(36)").IsRequired();
        builder.Property(x => x.IsUserMessage).HasColumnName("is_user_message").IsRequired();
        builder.Property(x => x.Content).IsRequired();
        builder.Property(x => x.ChatId).HasColumnName("chat_id").IsRequired();
        builder
            .Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();
    }
}
