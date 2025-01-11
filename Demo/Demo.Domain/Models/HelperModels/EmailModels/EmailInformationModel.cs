using System;

namespace Demo.Domain.Models.HelperModels.EmailModels;

public class EmailInformationModel
{
    public string Description { get; set; }
    public DateTimeOffset? DateFrom { get; set; }
    public DateTimeOffset DateTo { get; set; }
}