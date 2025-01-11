using System.Collections.Generic;

namespace Demo.Domain.Models.DALModels.Role;

public class RoleDal
{
    public string Name { get; set; }
    public string Description { get; set; }
    public List<int> Actions { get; set; }
}