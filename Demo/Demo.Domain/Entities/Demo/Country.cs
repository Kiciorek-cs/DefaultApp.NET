namespace Demo.Domain.Entities.Demo;

public class Country
{
    public int Id { get; set; }
    public string EnglishName { get; set; }
    public string PolishName { get; set; }
    public string IsoCode { get; set; } // pl, en
    public byte[] Icon { get; set; }
    public string RegexPhone { get; set; }
    public string Capital { get; set; }
    public string Continent { get; set; }
    public string OfficialLanguage { get; set; }
    public string TimeZone { get; set; }
    public string Currency { get; set; }
    public string CurrencyCode { get; set; }
    public string CallingCode { get; set; }
    public string Description { get; set; }
}