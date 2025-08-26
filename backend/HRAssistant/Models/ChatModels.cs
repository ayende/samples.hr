namespace HRAssistant.Models;

public class ChatRequest
{
    public string? ConversationId { get; set; }
    public string Message { get; set; } = string.Empty;
    public string EmployeeId { get; set; } = string.Empty;
    public SignatureResponse[] Signatures { get; set; } = [];
}

public class SignatureResponse
{
    public string ToolId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

public class ChatResponse
{
    public string ConversationId { get; set; } = string.Empty;
    public string? Answer { get; set; }
    public string[] Followups { get; set; } = [];
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public List<SignatureDocumentRequest> DocumentsToSign { get; set; } = [];
}

public class SignatureDocumentRequest
{
    public string ToolId { get; set; } = string.Empty;
    public string DocumentId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int Version { get; set; }
}

public class SignDocumentRequest
{
    public string ConversationId { get; set; } = string.Empty;
    public string EmployeeId { get; set; } = string.Empty;
    public string ToolId { get; set; } = string.Empty;
    public string DocumentId { get; set; } = string.Empty;
    public bool Confirmed { get; set; }
    public string? SignatureBlob { get; set; }
}

public class ChatMessage
{
    public string Id { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public bool IsUser { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class ChatHistoryResponse
{
    public string ConversationId { get; set; } = string.Empty;
    public ChatMessage[] Messages { get; set; } = [];
    public string EmployeeId { get; set; } = string.Empty;
}
