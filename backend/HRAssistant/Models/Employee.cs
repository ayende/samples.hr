namespace HRAssistant.Models;

public class Employee
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string DepartmentId { get; set; } = string.Empty;
    public string EmploymentType { get; set; } = string.Empty; // full-time, part-time
    public DateTime HireDate { get; set; }
    public bool CriticalRole { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Building { get; set; } = string.Empty;
    public VacationInfo Vacation { get; set; } = new();
    public List<SignedDocument> SignedDocuments { get; set; } = new();
}

public class VacationInfo
{
    public int AnnualEntitlement { get; set; } = 20;
    public decimal AccruedDays { get; set; }
    public int CarryOverDays { get; set; }
    public decimal Balance { get; set; }
    public int Cap { get; set; } = 30;
    public List<VacationHistory> History { get; set; } = new();
}

public class VacationHistory
{
    public int Year { get; set; }
    public int UsedDays { get; set; }
    public int CarryOverUsed { get; set; }
    public List<VacationRequestInfo> Requests { get; set; } = new();
}

public class VacationRequestInfo
{
    public string Id { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Days { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // pending, approved, denied
    public Replacement? Replacement { get; set; }
    public DateTime SubmittedDate { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public string? ApprovedBy { get; set; }
}

public class Replacement
{
    public string EmployeeId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
