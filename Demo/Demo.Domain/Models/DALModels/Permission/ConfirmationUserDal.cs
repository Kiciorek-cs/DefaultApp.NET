namespace Demo.Domain.Models.DALModels.Permission;

public class ConfirmationUserDal
{
    public int UserId { get; set; }
    public string ConfirmationToken { get; set; }
}