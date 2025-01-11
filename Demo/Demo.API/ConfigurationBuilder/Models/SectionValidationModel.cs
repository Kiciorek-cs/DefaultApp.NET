using System.Configuration;

namespace Demo.API.ConfigurationBuilder.Models;

public class SectionValidationModel
{
    public SectionValidationModel(string name, KeyValueConfigurationElement keyValueConfigurationElement)
    {
        Name = name;
        KeyValueConfigurationElement = keyValueConfigurationElement;
    }

    public string Name { get; set; }
    public KeyValueConfigurationElement KeyValueConfigurationElement { get; set; }
}