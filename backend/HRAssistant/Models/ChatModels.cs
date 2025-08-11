namespace HRAssistant.Models;

public class ChatRequest
{
    public string? ChatId { get; set; }
    public string Message { get; set; } = string.Empty;
    public string EmployeeId { get; set; } = string.Empty;
}

public class ChatResponse
{
    public string ChatId { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public string[] Followups { get; set; } = [];
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
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
    public string ChatId { get; set; } = string.Empty;
    public ChatMessage[] Messages { get; set; } = [];
    public string EmployeeId { get; set; } = string.Empty;
}
