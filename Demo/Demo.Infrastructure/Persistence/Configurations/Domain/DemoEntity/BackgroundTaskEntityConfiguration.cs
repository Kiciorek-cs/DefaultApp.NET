using Demo.Domain.Entities.Demo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demo.Infrastructure.Persistence.Configurations.Domain.DemoEntity;

internal class BackgroundTaskEntityConfiguration : IEntityTypeConfiguration<BackgroundTask>
{
    public void Configure(EntityTypeBuilder<BackgroundTask> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityColumn();

        builder.Property(x => x.InsertedAt).IsRequired();
        builder.Property(x => x.StatusType).HasConversion<string>();
        builder.Property(x => x.ActionBlockKey).IsRequired();
        builder.Property(x => x.Task).IsRequired();
        builder.Property(x => x.Delay).IsRequired();
    }
}