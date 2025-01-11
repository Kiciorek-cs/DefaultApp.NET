using System.Configuration;

namespace Demo.API.ConfigurationBuilder.Models;

public class DatabaseCustomSection : ConfigurationSection
{
    [ConfigurationProperty("Host", IsRequired = true)]
    public string Host
    {
        get => (string)this["Host"];
        set => this["Host"] = value;
    }
}