using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Demo.Domain.Entities.Permission;

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsDefaultRole { get; set; }

    [JsonIgnore] public virtual ICollection<Resource> Resources { get; set; }

    [JsonIgnore] public virtual ICollection<User> Users { get; set; }
}