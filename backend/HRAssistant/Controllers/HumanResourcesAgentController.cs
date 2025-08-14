using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents;
using HRAssistant.Models;
using Raven.Client.Documents.AI;
using Newtonsoft.Json.Linq;
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

            foreach (var signature in request.Signatures ?? [])
            {
                conversation.AddActionResponse(signature.ToolId, signature.Content);
            }
            if (string.IsNullOrWhiteSpace(request.Message) is false)
            {
                conversation.SetUserPrompt(request.Message);
            }

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

            var response = new ChatResponse
            {
                ChatId = conversation.Id,
                Answer = result.Answer?.Answer,
                Followups = result.Answer?.Followups ?? [],
                GeneratedAt = DateTime.UtcNow
            };

            var requiredActions = conversation.RequiredActions();

            if (requiredActions.FirstOrDefault(act => act.Name == "SignDocument") is { } action)
            {
                using var session = _documentStore.OpenAsyncSession();
                var parameters = JObject.Parse(action.Arguments);
                var document = await session.LoadAsync<SignatureDocument>(
                                        parameters.Value<string>("Document")
                                    );
                response.DocumentsToSign.Add(new SignatureDocumentRequest
                {
                    ToolId = action.ToolId,
                    DocumentId = document.Id,
                    Title = document.Title,
                    Content = document.Content,
                    Version = document.Version
                });
            }

            return Ok(response);
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

        [HttpGet("employees/dropdown")]
        public async Task<ActionResult<List<object>>> GetEmployeesForDropdown()
        {
            using var session = _documentStore.OpenAsyncSession();
            var employees = await session.Query<Employee>()
                .Select(e => new
                {
                    e.Id,
                    e.Name,
                    e.Department,
                    e.JobTitle,
                    e.Email
                })
                .ToListAsync();
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

        [HttpGet("signature-documents")]
        public async Task<ActionResult<List<SignatureDocument>>> GetSignatureDocuments()
        {
            using var session = _documentStore.OpenAsyncSession();
            var documents = await session.Query<SignatureDocument>()
                .Where(d => d.IsActive)
                .OrderBy(d => d.Title)
                .ToListAsync();
            return Ok(documents);
        }

        [HttpGet("signature-documents/{documentId}")]
        public async Task<ActionResult<SignatureDocument>> GetSignatureDocument(string documentId)
        {
            using var session = _documentStore.OpenAsyncSession();
            var document = await session.LoadAsync<SignatureDocument>(documentId);
            if (document == null)
            {
                return NotFound();
            }
            return Ok(document);
        }

        [HttpGet("employees/{employeeId}/signed-documents")]
        public async Task<ActionResult<List<SignedDocument>>> GetEmployeeSignedDocuments(string employeeId)
        {
            using var session = _documentStore.OpenAsyncSession();
            var employee = await session.LoadAsync<Employee>($"employees/{employeeId}");
            if (employee == null)
            {
                return NotFound("Employee not found");
            }
            return Ok(employee.SignedDocuments);
        }

        [HttpPost("sign-document")]
        public async Task<ActionResult<ChatResponse>> SignDocument([FromBody] SignDocumentRequest request)
        {
            using var session = _documentStore.OpenAsyncSession();
            var employee = await session.LoadAsync<Employee>(request.EmployeeId);
            var document = await session.LoadAsync<SignatureDocument>(request.DocumentId);

            var attachmentName = request.DocumentId + "-signature.png";

            // Extract base64 data from data URL (format: data:image/png;base64,<base64data>)
            var base64Data = request.SignatureBlob ?? "";
            if (base64Data.StartsWith("data:"))
            {
                var commaIndex = base64Data.IndexOf(',');
                if (commaIndex > 0)
                {
                    base64Data = base64Data.Substring(commaIndex + 1);
                }
            }

            session.Advanced.Attachments.Store(
                employee,
                attachmentName,
                new MemoryStream(Convert.FromBase64String(base64Data)),
                "image/png"
            );

            var signedDocument = new SignedDocument
            {
                DocumentId = document.Id,
                DocumentTitle = document.Title,
                DocumentVersion = document.Version,
                SignedDate = DateTime.UtcNow,
                SignedBy = employee.Name,
                SignatureAttachmentName = attachmentName,
                SignatureMethod = "Digital"
            };

            employee.SignedDocuments.Add(signedDocument);
            await session.StoreAsync(employee);
            await session.SaveChangesAsync();

            return Ok(new
            {
                Id = session.Advanced.GetDocumentId(signedDocument)
            });
        }
    }
}