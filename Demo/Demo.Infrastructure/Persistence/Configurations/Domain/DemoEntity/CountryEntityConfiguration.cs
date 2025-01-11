using Demo.Domain.Entities.Demo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demo.Infrastructure.Persistence.Configurations.Domain.DemoEntity;

public class CountryEntityConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityColumn();

        builder.HasIndex(x => x.IsoCode);

        builder.Property(x => x.EnglishName).IsRequired();
        builder.Property(x => x.PolishName).IsRequired();
        builder.Property(x => x.IsoCode).IsRequired();
        builder.Property(x => x.Icon);
        builder.Property(x => x.RegexPhone).IsRequired();
        builder.Property(x => x.Capital).IsRequired();
        builder.Property(x => x.Continent).IsRequired();
        builder.Property(x => x.OfficialLanguage).IsRequired();
        builder.Property(x => x.TimeZone).IsRequired();
        builder.Property(x => x.Currency).IsRequired();
        builder.Property(x => x.CurrencyCode).IsRequired();
        builder.Property(x => x.CallingCode).IsRequired();
        builder.Property(x => x.Description);
    }
}