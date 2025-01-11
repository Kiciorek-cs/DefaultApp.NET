namespace Demo.Domain.Models.DTOModels.Address;

public class AddressDto
{
    public int Id { get; set; }
    public string City { get; set; }
    public string Street { get; set; }
    public string BuildingNumber { get; set; }
    public string LocalNumber { get; set; }
    public string PostCode { get; set; }
    public string CountryEnglishName { get; set; }
    public string CountryPolishName { get; set; }
    public string CountryId { get; set; }
}