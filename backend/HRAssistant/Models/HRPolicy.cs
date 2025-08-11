namespace HRAssistant.Models;

public class HRPolicy
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty; // Markdown content
    public DateTime LastUpdated { get; set; }
    public string UpdatedBy { get; set; } = string.Empty;
    public int Version { get; set; } = 1;
    public List<string> Tags { get; set; } = new();
}
