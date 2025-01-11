using Demo.Domain.Enums;

namespace Demo.Domain.Models.HelperModels.PlcModels;

public class PlcReadModel
{
    public DataType DataType { get; init; }
    public ErrorType ErrorType { get; init; }
    public string Value { get; set; }
    public string DefaultValue { get; init; }
}