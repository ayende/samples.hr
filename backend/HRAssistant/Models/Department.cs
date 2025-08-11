namespace HRAssistant.Models;

public class Department
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Manager { get; set; } = string.Empty;
    public string ManagerId { get; set; } = string.Empty;
    public string Building { get; set; } = string.Empty;
    public string Floor { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> ResponsibleFor { get; set; } = new();
}
