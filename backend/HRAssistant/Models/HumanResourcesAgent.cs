using Newtonsoft.Json;
using Raven.Client.Documents;
using Raven.Client.Documents.Operations.AI.Agents;
public static class HumanResourcesAgent
{
    public class Reply
    {
        public string Answer { get; set; } = string.Empty;
        public string[] Followups { get; set; } = [];
    }

    public class RaiseIssueArgs
    {
        required public string Title { get; set; }
        required public string Description { get; set; }
        required public string Category { get; set; }
        required public string Priority { get; set; }
    }

    public static Task Create(IDocumentStore store)
    {
        return store.AI.CreateAgentAsync(
          new AiAgentConfiguration
          {
              Name = "HR Assistant",
              Identifier = "hr-assistant",
              ConnectionStringName = "HR's Open AI",
              SystemPrompt = @"You are an HR assistant. 
Provide info on benefits, policies, and departments. 
Be professional and cheery.

You can answer in markdown format, make sure to use ticks (`) whenever you discuss identifiers.
Do not suggest actions that are not explicitly allowed by the tools available to you.

Do NOT discuss non-HR topics. Answer only for the current employee.
",
              Parameters = [new AiAgentParameter("employeeId", "Employee ID; answer only for this employee")],
              SampleObject = JsonConvert.SerializeObject(new Reply
              {
                  Answer = "Detailed answer to query",
                  Followups = ["Likely follow-ups"],
              }),
              Queries = [
                new AiAgentToolQuery
                {
                    Name = "GetEmployeeInfo",
                    Description = "Retrieve employee details",
                    Query = "from Employees where id() = $employeeId",
                    ParametersSampleObject = "{}"
                },
                new AiAgentToolQuery
                {
                    Name = "GetVacations",
                    Description = "Retrieve recent employee vacation details",
                    Query = @"
                    from VacationRequests 
                    where EmployeeId = $employeeId 
                    order by SubmittedDate desc
                    limit 5",
                    ParametersSampleObject = "{}"
                },
                new AiAgentToolQuery
                {
                    Name = "GetPayStubs",
                    Description = "Retrieve employee's paystubs within a given date range",
                    Query = @"
                    from PayStubs 
                    where EmployeeId = $employeeId 
                        and PayDate between $startDate and $endDate
                    order by PayDate desc
                    select PayPeriodStart, PayPeriodEnd, PayDate, GrossPay, NetPay, 
                            Earnings, Deductions, Taxes, YearToDateGross, YearToDateNet, 
                            PayPeriodNumber, PayFrequency
                    limit 5",
                    ParametersSampleObject = "{\"startDate\": \"yyyy-MM-dd\", \"endDate\": \"yyyy-MM-dd\"}"
                },
new AiAgentToolQuery
{
    Name = "FindIssues",
    Description = "Semantic search for employee's issues",
    Query = @"
    from HRIssues
    where EmployeeId = $employeeId 
        and (vector.search(embedding.text(Title), $query) or vector.search(embedding.text(Description), $query))
    order by SubmittedDate desc
    limit 5",
    ParametersSampleObject = "{\"query\": [\"query terms to find matching issue\"]}"
},
new AiAgentToolQuery
{
    Name = "FindPolicies",
    Description = "Semantic search for employer's policies",
    Query = @"
    from HRPolicies
    where (vector.search(embedding.text(Title), $query) or vector.search(embedding.text(Content), $query))
    limit 5",
    ParametersSampleObject = "{\"query\": [\"query terms to find matching issue\"]}"
},
            ],
              Actions = [
                new AiAgentToolAction
                {
                    Name = "RaiseIssue",
                    Description = "Raise a new HR issue for the employee (full details)",
                    ParametersSampleObject = JsonConvert.SerializeObject(new RaiseIssueArgs{
                        Title = "Clear & short title describing the issue",
                        Category = "Payroll | Facilities | Onboarding | Benefits",
                        Description = "Full description, with all relevant context",
                        Priority = "Low | Medium | High | Critical"
                    })
                },
            ]
          });
    }
}