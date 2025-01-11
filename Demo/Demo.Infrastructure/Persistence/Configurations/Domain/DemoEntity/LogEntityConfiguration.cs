using Demo.Domain.Entities.Demo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demo.Infrastructure.Persistence.Configurations.Domain.DemoEntity;

internal class LogEntityConfiguration : IEntityTypeConfiguration<Log>
{
    public void Configure(EntityTypeBuilder<Log> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityColumn();

        builder.Property(x => x.InsertedBy).IsRequired();
        builder.Property(x => x.InsertedOn);
        builder.Property(x => x.LogType).HasConversion<string>();
        builder.Property(x => x.ActionType).HasConversion<string>();
        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.Description).IsRequired();
        builder.Property(x => x.MethodName).IsRequired();
        builder.Property(x => x.TableName).IsRequired();
        builder.Property(x => x.UniqueObjectId).IsRequired();
        builder.Property(x => x.TraceId).IsRequired();
    }
}