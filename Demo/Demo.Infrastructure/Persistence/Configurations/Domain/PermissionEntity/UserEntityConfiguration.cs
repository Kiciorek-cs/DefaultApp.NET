using Demo.Domain.Entities.Permission;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demo.Infrastructure.Persistence.Configurations.Domain.PermissionEntity;

internal class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityColumn();

        builder.Property(x => x.FirstName).IsRequired();
        builder.Property(x => x.FirstNameUnified).IsRequired();
        builder.Property(x => x.LastName).IsRequired();
        builder.Property(x => x.LastNameUnified).IsRequired();
        builder.Property(x => x.Gender).HasConversion<string>();
        builder.Property(x => x.DateOfBirth).IsRequired();

        builder.HasOne(x => x.Login)
            .WithOne(x => x.User)
            .HasForeignKey<Login>(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Role)
            .WithMany(x => x.Users)
            .HasForeignKey(b => b.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}