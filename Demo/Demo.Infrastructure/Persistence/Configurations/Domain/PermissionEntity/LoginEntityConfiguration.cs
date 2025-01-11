using Demo.Domain.Entities.Permission;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demo.Infrastructure.Persistence.Configurations.Domain.PermissionEntity;

internal class LoginEntityConfiguration : IEntityTypeConfiguration<Login>
{
    public void Configure(EntityTypeBuilder<Login> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityColumn();

        builder.Property(x => x.LoginName).IsRequired();
        builder.Property(x => x.PasswordHash).IsRequired();
        builder.Property(x => x.PasswordSalt).IsRequired();
        builder.Property(x => x.EmailAddress).IsRequired();
        builder.Property(x => x.TokenGenerationTime).IsRequired();
        builder.Property(x => x.EmailValidationStatus).HasConversion<string>();
        builder.Property(x => x.Status).HasConversion<string>();
    }
}