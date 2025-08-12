namespace HRAssistant.Models;

public class PayStub
{
    public string Id { get; set; } = string.Empty;
    public string EmployeeId { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public DateTime PayPeriodStart { get; set; }
    public DateTime PayPeriodEnd { get; set; }
    public DateTime PayDate { get; set; }
    public decimal GrossPay { get; set; }
    public decimal NetPay { get; set; }
    public List<PayStubEarning> Earnings { get; set; } = new();
    public List<PayStubDeduction> Deductions { get; set; } = new();
    public List<PayStubTax> Taxes { get; set; } = new();
    public decimal YearToDateGross { get; set; }
    public decimal YearToDateNet { get; set; }
    public int PayPeriodNumber { get; set; }
    public string PayFrequency { get; set; } = "Bi-Weekly"; // Weekly, Bi-Weekly, Monthly
    public ACHBankDetails? DirectDeposit { get; set; }
}

public class PayStubEarning
{
    public string Type { get; set; } = string.Empty; // Salary, Overtime, Bonus, etc.
    public decimal Hours { get; set; }
    public decimal Rate { get; set; }
    public decimal Amount { get; set; }
    public decimal YearToDate { get; set; }
}

public class PayStubDeduction
{
    public string Type { get; set; } = string.Empty; // Health Insurance, 401k, etc.
    public decimal Amount { get; set; }
    public decimal YearToDate { get; set; }
    public bool PreTax { get; set; }
}

public class PayStubTax
{
    public string Type { get; set; } = string.Empty; // Federal, State, Social Security, etc.
    public decimal Amount { get; set; }
    public decimal YearToDate { get; set; }
    public decimal TaxableWages { get; set; }
}

public class ACHBankDetails
{
    public string BankName { get; set; } = string.Empty;
    public string RoutingNumber { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty; // Partially masked for security
    public string AccountType { get; set; } = "Checking"; // Checking, Savings
    public decimal DepositAmount { get; set; }
    public string DepositType { get; set; } = "Full Amount"; // Full Amount, Fixed Amount, Percentage
}
