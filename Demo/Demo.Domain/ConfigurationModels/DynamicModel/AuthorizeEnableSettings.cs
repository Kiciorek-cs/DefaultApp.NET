using System.Collections.Generic;

namespace Demo.Domain.ConfigurationModels.DynamicModel;

public class AuthorizeEnableSettings
{
    public bool Authorize { get; set; } = true;

    public bool DeploymentMode { get; set; }
    public List<string> DeploymentModeLogins { get; set; } = new();
}