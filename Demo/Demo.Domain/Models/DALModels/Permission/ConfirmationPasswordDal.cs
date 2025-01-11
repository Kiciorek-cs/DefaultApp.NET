namespace Demo.Domain.Models.DALModels.Permission;

public class ConfirmationPasswordDal
{
    public string ResetToken { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}