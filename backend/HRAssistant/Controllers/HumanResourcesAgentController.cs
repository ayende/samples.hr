using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents;
using HRAssistant.Models;
using Raven.Client.Documents.AI;
using System.Text.Json;

namespace HRAssistant.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HumanResourcesAgentController : ControllerBase
    {
        private readonly IDocumentStore _documentStore;
        private readonly ILogger<HumanResourcesAgentController> _logger;
        private readonly System.Text.Json.JsonSerializerOptions _jsonOptions;

        public HumanResourcesAgentController(IDocumentStore documentStore, ILogger<HumanResourcesAgentController> logger, Microsoft.Extensions.Options.IOptions<Microsoft.AspNetCore.Mvc.JsonOptions> jsonOptions)
        {
            _documentStore = documentStore;
            _logger = logger;
            _jsonOptions = jsonOptions.Value.JsonSerializerOptions;
        }

        [HttpPost("chat")]
        public async Task Chat([FromBody] ChatRequest request)
        {
            var documentsToSign = new List<SignatureDocumentRequest>();
            var conversationId = request.ConversationId ?? "hr/" + request.EmployeeId + "/" + DateTime.Today.ToString("yyyy-MM-dd");
            var conversation = _documentStore.AI.Conversation(
                agentId: "hr-assistant", conversationId,
                new AiConversationCreationOptions
                {
                    Parameters = new Dictionary<string, object>
                    {
                        ["employeeId"] = request.EmployeeId
                    },
                    ExpirationInSec = 60 * 60 * 24 * 30 // 30 days
                });

            conversation.Receive<HumanResourcesAgent.SignDocumentArgs>("SignDocument", async (req, args) =>
            {
                using var session = _documentStore.OpenAsyncSession();
                var document = await session.LoadAsync<SignatureDocument>(
                                        args.Document
                                    );
                documentsToSign.Add(new SignatureDocumentRequest
                {
                    ToolId = req.ToolId,
                    DocumentId = document.Id,
                    Title = document.Title,
                    Content = document.Content,
                    Version = document.Version
                });
            });

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

            foreach (var signature in request.Signatures ?? [])
            {
                conversation.AddActionResponse(signature.ToolId, signature.Content);
            }
            if (string.IsNullOrWhiteSpace(request.Message) is false)
            {
                conversation.SetUserPrompt(request.Message);
            }

            Response.Headers["Content-Type"] = "text/event-stream";
            await using var writer = new StreamWriter(Response.Body);
            var result = await conversation.StreamAsync<HumanResourcesAgent.Reply>("Answer", async chunk =>
            {
                await writer.WriteAsync("data: ");
                await writer.WriteAsync(JsonSerializer.Serialize(chunk, _jsonOptions));
                await writer.WriteAsync("\n\n");
                await writer.FlushAsync();
            });

            // Send the final result as a custom event
            var finalResponse = new ChatResponse
            {
                ConversationId = conversation.Id,
                Answer = result.Answer?.Answer,
                Followups = result.Answer?.Followups ?? [],
                GeneratedAt = DateTime.UtcNow,
                DocumentsToSign = documentsToSign
            };
            await writer.WriteAsync($"event: final\ndata: ");
            await writer.WriteAsync(JsonSerializer.Serialize(finalResponse, _jsonOptions));
            await writer.WriteAsync("\n\n");
            await writer.FlushAsync();
        }

        public record Message(string content, string role, DateTime date);
        public record Conversation(List<Message> Messages);

        [HttpGet("chat/today/{*employeeId}")]
        public async Task<ActionResult<ChatHistoryResponse>> GetChatHistory(string employeeId)
        {
            var conversationId = "hr/" + employeeId + "/" + DateTime.Today.ToString("yyyy-MM-dd");
            using var session = _documentStore.OpenAsyncSession();
            var chat = await session.LoadAsync<Conversation>(conversationId);

            var messages = chat?.Messages ?? [];

            return Ok(new ChatHistoryResponse
            {
                ConversationId = conversationId,
                Messages = messages
                    // skip tool calls, system prompt, etc
                    .Where(m => m.role == "user" || m.role == "assistant")
                    // skip AI Agent Parameters
                    .Where(m => m.content is not null && !m.content.StartsWith("AI Agent Parameters"))
                    .Select((m, i) => new ChatMessage
                    {
                        Id = conversationId + "#" + i,
                        Text = m.role switch
                        {
                            "assistant" when m.content.StartsWith("{") => JsonSerializer.Deserialize<HumanResourcesAgent.Reply>(m.content)!.Answer,
                            _ => m.content,
                        },
                        IsUser = m.role == "user",
                        Timestamp = m.date
                    }).ToArray(),
                EmployeeId = employeeId
            });
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