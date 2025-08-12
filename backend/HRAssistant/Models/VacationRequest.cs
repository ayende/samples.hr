namespace HRAssistant.Models;

public class VacationRequest
{
    public string Id { get; set; } = string.Empty;
    public string EmployeeId { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int DaysRequested { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending"; // Pending, Approved, Denied
    public string VacationType { get; set; } = "Annual Leave"; // Annual Leave, Personal Day, Sick Leave
    public Replacement? ReplacementEmployee { get; set; }
    public DateTime SubmittedDate { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public string? ApprovedBy { get; set; }
    public bool PeakTimeConflict { get; set; }
}
