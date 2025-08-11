namespace HRAssistant.Models;

public class VacationRequest
{
    public string Id { get; set; } = string.Empty;
    public string EmployeeId { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Days { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = "pending"; // pending, approved, denied
    public Replacement? Replacement { get; set; }
    public DateTime SubmittedDate { get; set; }
    public DateTime? ReviewedDate { get; set; }
    public string? ReviewedBy { get; set; }
    public bool PeakTimeConflict { get; set; }
}
