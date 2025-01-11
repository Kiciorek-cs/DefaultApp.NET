using Demo.Domain.Entities.Permission;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demo.Infrastructure.Persistence.Configurations.Domain.PermissionEntity;

internal class TokenEntityConfiguration : IEntityTypeConfiguration<Token>
{
    public void Configure(EntityTypeBuilder<Token> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityColumn();

        builder.Property(x => x.TokenType).HasConversion<string>();
        builder.Property(x => x.StatusType).HasConversion<string>();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.ExpirationDate);
        builder.Property(x => x.Value).IsRequired();

        builder.HasOne(x => x.Login)
            .WithMany(x => x.Tokens)
            .HasForeignKey(b => b.LoginId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ParentToken)
            .WithMany(x => x.SubTokens)
            .HasForeignKey(x => x.ParentTokenId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}