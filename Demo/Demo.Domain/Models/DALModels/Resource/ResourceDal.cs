using Demo.Domain.Enums;

namespace Demo.Domain.Models.DALModels.Resource;

public class ResourceDal
{
    public string Name { get; set; }
    public string Description { get; set; }
    public HttpMethodType HttpMethod { get; set; }
}