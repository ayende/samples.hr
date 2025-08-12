using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents;
using HRAssistant.Models;
using Raven.Client.Documents.AI;
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

            conversation.Handle<HumanResourcesAgent.RaiseIssueArgs>("RaiseIssue", async (args) =>
            {
                using var session = _documentStore.OpenAsyncSession();
                var issue = new HRIssue
                {
                    EmployeeId = request.EmployeeId,
                    Title = args.Title,
                    Description = args.Description,
                    Category = args.Category,
                    Priority = args.Priority,
                    SubmittedDate = DateTime.UtcNow,
                    Status = "Open"
                };
                await session.StoreAsync(issue);
                await session.SaveChangesAsync();

                return "Raised issue: " + issue.Id;
            });
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
                    .Where(m => m.content is not null && !m.content.StartsWith("AI Agent Parameters"))
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

        [HttpGet("employees")]
        public async Task<ActionResult<List<Employee>>> GetEmployees()
        {
            using var session = _documentStore.OpenAsyncSession();
            var employees = await session.Query<Employee>().ToListAsync();
            return Ok(employees);
        }


        [HttpGet("policies")]
        public async Task<ActionResult<List<HRPolicy>>> GetPolicies()
        {
            using var session = _documentStore.OpenAsyncSession();
            var policies = await session.Query<HRPolicy>().ToListAsync();
            return Ok(policies);
        }

        [HttpGet("vacations")]
        public async Task<ActionResult<List<VacationRequest>>> GetVacations()
        {
            using var session = _documentStore.OpenAsyncSession();
            var vacations = await session.Query<VacationRequest>().ToListAsync();
            return Ok(vacations);
        }

        [HttpGet("paystubs/{employeeId}")]
        public async Task<ActionResult<List<PayStub>>> GetPayStubs(string employeeId)
        {
            using var session = _documentStore.OpenAsyncSession();
            var payStubs = await session.Query<PayStub>()
                .Where(p => p.EmployeeId == $"employees/{employeeId}")
                .OrderByDescending(p => p.PayDate)
                .ToListAsync();
            return Ok(payStubs);
        }

        [HttpGet("issues")]
        public async Task<ActionResult<List<HRIssue>>> GetHRIssues()
        {
            using var session = _documentStore.OpenAsyncSession();
            var issues = await session.Query<HRIssue>().ToListAsync();
            return Ok(issues);
        }

        [HttpGet("issues/{employeeId}")]
        public async Task<ActionResult<List<HRIssue>>> GetEmployeeHRIssues(string employeeId)
        {
            using var session = _documentStore.OpenAsyncSession();
            var issues = await session.Query<HRIssue>()
                .Where(i => i.EmployeeId == $"employees/{employeeId}")
                .OrderByDescending(i => i.SubmittedDate)
                .ToListAsync();
            return Ok(issues);
        }
    }
}