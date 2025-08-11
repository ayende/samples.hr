using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents;
using HRAssistant.Models;
using Raven.Client.Documents.AI;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HRAssistant.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HumanResourcesAgentController : ControllerBase
    {
        private readonly IDocumentStore _documentStore;
        private readonly ILogger<HumanResourcesAgentController> _logger;

        public HumanResourcesAgentController(IDocumentStore documentStore, ILogger<HumanResourcesAgentController> logger)
        {
            _documentStore = documentStore;
            _logger = logger;
        }

        [HttpPost("chat")]
        public async Task<ActionResult<ChatResponse>> Chat([FromBody] ChatRequest request)
        {
            var chatId = request.ChatId ?? "hr/" + request.EmployeeId + "/" + DateTime.Today.ToString("yyyy-MM-dd");
            var conversation = _documentStore.AI.Conversation(
                agentId: "hr-assistant", chatId,
                new AiConversationCreationOptions
                {
                    Parameters = new Dictionary<string, object>
                    {
                        ["employeeId"] = request.EmployeeId
                    },
                    ExpirationInSec = 60 * 60 * 24 * 30 // 30 days
                });
            conversation.SetUserPrompt(request.Message);
            var result = await conversation.RunAsync<HumanResourcesAgent.Reply>();
            var answer = result.Answer;

            return Ok(new ChatResponse
            {
                ChatId = conversation.Id,
                Answer = answer.Answer,
                Followups = answer.Followups,
                GeneratedAt = DateTime.UtcNow
            });
        }

        public record Message(string content, string role, DateTime date);
        public record Conversation(List<Message> Messages);

        [HttpGet("chat/today/{*employeeId}")]
        public async Task<ActionResult<ChatHistoryResponse>> GetChatHistory(string employeeId)
        {
            var chatId = "hr/" + employeeId + "/" + DateTime.Today.ToString("yyyy-MM-dd");
            using var session = _documentStore.OpenAsyncSession();
            var chat = await session.LoadAsync<Conversation>(chatId);

            var messages = chat?.Messages ?? [];

            return Ok(new ChatHistoryResponse
            {
                ChatId = chatId,
                Messages = messages
                    // skip tool calls, system prompt, etc
                    .Where(m => m.role == "user" || m.role == "assistant")
                    // skip AI Agent Parameters
                    .Where(m => !m.content.StartsWith("AI Agent Parameters"))
                    .Select((m, i) => new ChatMessage
                    {
                        Id = chatId + "#" + i,
                        Text = m.role switch
                        {
                            "assistant" when m.content.StartsWith("{") => JsonConvert.DeserializeObject<HumanResourcesAgent.Reply>(m.content)!.Answer,
                            _ => m.content,
                        },
                        IsUser = m.role == "user",
                        Timestamp = m.date
                    }).ToArray(),
                EmployeeId = employeeId
            });
        }


        [HttpPost("seed")]
        public async Task<ActionResult> SeedData()
        {
            using var session = _documentStore.OpenAsyncSession();

            try
            {
                // Create sample departments
                var departments = new List<Department>
                {
                    new() { Id = "departments/it", Name = "Information Technology", Manager = "Alice Johnson", ManagerId = "employees/alice", Building = "A", Floor = "3", Description = "Technology infrastructure and support", ResponsibleFor = new() { "IT", "Technology", "Software", "Hardware" } },
                    new() { Id = "departments/hr", Name = "Human Resources", Manager = "Bob Smith", ManagerId = "employees/bob", Building = "B", Floor = "2", Description = "Employee relations and policies", ResponsibleFor = new() { "HR", "Policies", "Benefits", "Hiring" } },
                    new() { Id = "departments/finance", Name = "Finance", Manager = "Carol Wilson", ManagerId = "employees/carol", Building = "C", Floor = "1", Description = "Financial planning and accounting", ResponsibleFor = new() { "Finance", "Accounting", "Budgets", "Expenses" } },
                    new() { Id = "departments/marketing", Name = "Marketing", Manager = "David Brown", ManagerId = "employees/david", Building = "A", Floor = "2", Description = "Marketing and communications", ResponsibleFor = new() { "Marketing", "Communications", "Branding" } }
                };

                foreach (var dept in departments)
                {
                    await session.StoreAsync(dept);
                }

                // Create sample employees
                var employees = new List<Employee>
                {
                    new()
                    {
                        Id = "employees/alice",
                        Name = "Alice Johnson",
                        Department = "Information Technology",
                        EmploymentType = "full-time",
                        HireDate = new DateTime(2022, 3, 15),
                        CriticalRole = true,
                        JobTitle = "IT Manager",
                        Email = "alice.johnson@company.com",
                        Building = "A",
                        Vacation = new VacationInfo
                        {
                            AnnualEntitlement = 25,
                            AccruedDays = 18.5m,
                            CarryOverDays = 3,
                            Balance = 21.5m,
                            History = new() { new() { Year = 2025, UsedDays = 7, CarryOverUsed = 0, Requests = new() } }
                        }
                    },
                    new()
                    {
                        Id = "employees/bob",
                        Name = "Bob Smith",
                        Department = "Human Resources",
                        EmploymentType = "full-time",
                        HireDate = new DateTime(2021, 8, 1),
                        CriticalRole = true,
                        JobTitle = "HR Manager",
                        Email = "bob.smith@company.com",
                        Building = "B",
                        Vacation = new VacationInfo
                        {
                            AnnualEntitlement = 25,
                            AccruedDays = 20m,
                            CarryOverDays = 5,
                            Balance = 25m,
                            History = new() { new() { Year = 2025, UsedDays = 5, CarryOverUsed = 2, Requests = new() } }
                        }
                    },
                    new()
                    {
                        Id = "employees/carol",
                        Name = "Carol Wilson",
                        Department = "Finance",
                        EmploymentType = "full-time",
                        HireDate = new DateTime(2023, 1, 10),
                        CriticalRole = false,
                        JobTitle = "Finance Manager",
                        Email = "carol.wilson@company.com",
                        Building = "C",
                        Vacation = new VacationInfo
                        {
                            AnnualEntitlement = 20,
                            AccruedDays = 15m,
                            CarryOverDays = 0,
                            Balance = 15m,
                            History = new() { new() { Year = 2025, UsedDays = 8, CarryOverUsed = 0, Requests = new() } }
                        }
                    },
                    new()
                    {
                        Id = "employees/david",
                        Name = "David Brown",
                        Department = "Marketing",
                        EmploymentType = "full-time",
                        HireDate = new DateTime(2024, 6, 1),
                        CriticalRole = false,
                        JobTitle = "Marketing Manager",
                        Email = "david.brown@company.com",
                        Building = "A",
                        Vacation = new VacationInfo
                        {
                            AnnualEntitlement = 20,
                            AccruedDays = 12m,
                            CarryOverDays = 0,
                            Balance = 12m,
                            History = new() { new() { Year = 2025, UsedDays = 3, CarryOverUsed = 0, Requests = new() } }
                        }
                    },
                    new()
                    {
                        Id = "employees/john",
                        Name = "John Doe",
                        Department = "Engineering",
                        EmploymentType = "full-time",
                        HireDate = new DateTime(2024, 5, 1),
                        CriticalRole = true,
                        JobTitle = "Senior Developer",
                        Email = "john.doe@company.com",
                        Building = "A",
                        Vacation = new VacationInfo
                        {
                            AnnualEntitlement = 20,
                            AccruedDays = 15m,
                            CarryOverDays = 5,
                            Balance = 15m,
                            History = new() { new() { Year = 2025, UsedDays = 5, CarryOverUsed = 0, Requests = new() } }
                        }
                    }
                };

                foreach (var employee in employees)
                {
                    await session.StoreAsync(employee);
                }

                // Create HR Policies
                var policies = CreateSamplePolicies();
                foreach (var policy in policies)
                {
                    await session.StoreAsync(policy);
                }

                await session.SaveChangesAsync();
                return Ok(new { Message = "Sample data created successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding data");
                return StatusCode(500, new { Message = "Error creating sample data", Error = ex.Message });
            }
        }

        private List<HRPolicy> CreateSamplePolicies()
        {
            return new List<HRPolicy>
            {
                new()
                {
                    Id = "policies/code-of-conduct",
                    Title = "Code of Conduct Policy",
                    Category = "Ethics",
                    LastUpdated = DateTime.Now,
                    UpdatedBy = "HR Department",
                    Tags = new() { "ethics", "conduct", "behavior" },
                    Content = @"# Code of Conduct Policy

## Purpose
This policy establishes guidelines for professional behavior and ethical standards expected of all employees.

## Core Values
- **Integrity**: Act honestly and transparently in all business dealings
- **Respect**: Treat all colleagues, customers, and partners with dignity
- **Accountability**: Take responsibility for your actions and decisions
- **Excellence**: Strive for high-quality work and continuous improvement"
                },
                new()
                {
                    Id = "policies/attendance-leave",
                    Title = "Attendance and Leave Policy",
                    Category = "Time Off",
                    LastUpdated = DateTime.Now,
                    UpdatedBy = "HR Department",
                    Tags = new() { "attendance", "vacation", "sick leave", "time off" },
                    Content = @"# Attendance and Leave Policy

## Working Hours
- Standard business hours: 9:00 AM - 5:00 PM, Monday through Friday
- Core hours (required presence): 10:00 AM - 3:00 PM
- Flexible arrangements may be approved by supervisor"
                }
            };
        }

        [HttpGet("employees")]
        public async Task<ActionResult<List<Employee>>> GetEmployees()
        {
            using var session = _documentStore.OpenAsyncSession();
            var employees = await session.Query<Employee>().ToListAsync();
            return Ok(employees);
        }

        [HttpGet("departments")]
        public async Task<ActionResult<List<Department>>> GetDepartments()
        {
            using var session = _documentStore.OpenAsyncSession();
            var departments = await session.Query<Department>().ToListAsync();
            return Ok(departments);
        }

        [HttpGet("policies")]
        public async Task<ActionResult<List<HRPolicy>>> GetPolicies()
        {
            using var session = _documentStore.OpenAsyncSession();
            var policies = await session.Query<HRPolicy>().ToListAsync();
            return Ok(policies);
        }
    }
}