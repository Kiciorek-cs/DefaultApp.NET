using System.Collections.Generic;
using Demo.Domain.Enums;

namespace Demo.Domain.Models.DTOModels.BackgroundTask;

public class BackgroundTaskDto
{
    public List<BackgroundTaskInformationDto> DatabaseTasks { get; set; } = new();
    public List<BackgroundTaskInformationDto> WorkingTasks { get; set; } = new();
}

public class BackgroundTaskInformationDto
{
    public string Text { get; set; }
    public string Key { get; set; }
}