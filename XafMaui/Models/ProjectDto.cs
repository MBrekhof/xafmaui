namespace XafMaui.Models;

public class ProjectDto
{
    public int ID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ProjectStatus Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal BudgetHours { get; set; }
    public int? ClientID { get; set; }
    public string? ClientName { get; set; }
    public List<ProjectTaskDto> ProjectTasks { get; set; } = [];
}

public class ProjectTaskDto
{
    public int ID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ProjectTaskStatus Status { get; set; }
    public decimal EstimatedHours { get; set; }
    public int SortOrder { get; set; }
    public int ProjectID { get; set; }
    public decimal LoggedHours { get; set; }
}

public class ProjectAssignmentDto
{
    public int ID { get; set; }
    public int ProjectID { get; set; }
    public string? ProjectName { get; set; }
    public ProjectRole Role { get; set; }
}
