namespace Demo.Domain.Models.DTOModels.Country;

public class CountryDto
{
    public int Id { get; set; }
    public string EnglishName { get; set; }
    public string PolishName { get; set; }
    public string IsoCode { get; set; }
    public byte[] Icon { get; set; }
    public string RegexPhone { get; set; }
    public string CallingCode { get; set; }
}