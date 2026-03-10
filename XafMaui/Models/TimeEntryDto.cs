namespace XafMaui.Models;

public class TimeEntryDto
{
    public int ID { get; set; }
    public DateTime Date { get; set; }
    public decimal Hours { get; set; }
    public string? Note { get; set; }
    public TimeEntryStatus Status { get; set; }
    public string? ReviewNote { get; set; }
    public int ProjectTaskID { get; set; }
    public TimeEntryProjectTaskDto? ProjectTask { get; set; }
}

public class TimeEntryProjectTaskDto
{
    public string Name { get; set; } = string.Empty;
    public TimeEntryProjectDto? Project { get; set; }
}

public class TimeEntryProjectDto
{
    public string Name { get; set; } = string.Empty;
}
