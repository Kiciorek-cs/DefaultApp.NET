namespace Demo.Domain.Models.HelperModels.TokenModels;

public class UserClaimModel
{
    public int UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public string LoginName { get; set; } = string.Empty;
}