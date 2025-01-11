using System.Collections.Generic;
using System.Text.Json.Serialization;
using Demo.Domain.Enums;

namespace Demo.Domain.Entities.Permission;

public class Resource
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public HttpMethodType HttpMethod { get; set; }

    [JsonIgnore] public virtual ICollection<Role> Roles { get; set; }
}