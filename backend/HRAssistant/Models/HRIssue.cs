namespace HRAssistant.Models;

public class HRIssue
{
    public string Id { get; set; }
    public string EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Category { get; set; } // Benefits, Payroll, IT Request, Policy Question, etc.
    public string Priority { get; set; } // Low, Medium, High, Urgent
    public string Status { get; set; } // Open, In Progress, Resolved, Closed
    public DateTime SubmittedDate { get; set; }
    public DateTime? AssignedDate { get; set; }
    public DateTime? ResolvedDate { get; set; }
    public DateTime? ClosedDate { get; set; }
    public string? AssignedTo { get; set; }
    public string? Resolution { get; set; }
    public List<HRIssueComment> Comments { get; set; } = new();
    public List<string> Tags { get; set; } = new();
}

public class HRIssueComment
{
    public string Id { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public bool IsInternal { get; set; } = false; // Internal HR notes vs employee communication
}
