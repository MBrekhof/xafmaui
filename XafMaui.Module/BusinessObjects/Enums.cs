namespace XafMaui.Module.BusinessObjects;

public enum ProjectStatus
{
    Draft,
    Active,
    OnHold,
    Completed,
    Archived
}

public enum ProjectTaskStatus
{
    Open,
    InProgress,
    Done
}

public enum TimeEntryStatus
{
    Draft,
    Submitted,
    Approved,
    Rejected
}

public enum ProjectRole
{
    Manager,
    Member
}

public enum AppRole
{
    Admin,
    ProjectManager,
    Consultant,
    BackOffice
}
