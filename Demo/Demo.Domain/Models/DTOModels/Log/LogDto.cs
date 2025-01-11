using System;

namespace Demo.Domain.Models.DTOModels.Log;

public class LogDto
{
    public int Id { get; set; }
    public string Description { get; set; }
    public bool HoloLensReadLog { get; set; }
    public DateTimeOffset CreatedOn { get; set; }


    public int ProductionLineId { get; set; }
    public int ErrorId { get; set; }
    public string ErrorCode { get; set; }
    public string ScenarioNumber { get; set; }
    public string ErrorType { get; set; }
    public string ErrorMessage { get; set; }
}