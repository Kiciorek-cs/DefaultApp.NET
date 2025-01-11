using System.Collections.Generic;

namespace Demo.Domain.Models.DTOModels.Role;

public class RoleDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<int> Resources { get; set; }
}