namespace XafMaui.Models;

public class TimeEntryDto
{
    public int ID { get; set; }
    public DateTime Date { get; set; }
    public decimal Hours { get; set; }
    public string? Note { get; set; }
    public TimeEntryStatus Status { get; set; }
    public int ProjectTaskID { get; set; }
    public string? ProjectTaskName { get; set; }
    public string? ProjectName { get; set; }
    public bool IsPendingSync { get; set; }
    public bool IsLocalOnly { get; set; }
}
