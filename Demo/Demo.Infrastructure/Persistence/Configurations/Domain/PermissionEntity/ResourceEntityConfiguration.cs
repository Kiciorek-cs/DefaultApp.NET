﻿using Demo.Domain.Entities.Permission;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demo.Infrastructure.Persistence.Configurations.Domain.PermissionEntity;

internal class ResourceEntityConfiguration : IEntityTypeConfiguration<Resource>
{
    public void Configure(EntityTypeBuilder<Resource> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityColumn();

        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.Description).IsRequired();
        builder.Property(x => x.HttpMethod).HasConversion<string>();
    }
}