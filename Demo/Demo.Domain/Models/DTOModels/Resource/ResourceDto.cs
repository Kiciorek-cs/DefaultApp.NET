using Demo.Domain.Enums;

namespace Demo.Domain.Models.DTOModels.Resource;

public class ResourceDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public HttpMethodType HttpMethod { get; set; }
}