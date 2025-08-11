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
                    Name = "Get Employee Info",
                    Description = "Retrieve employee details",
                    Query = "from Employees where Id = $employeeId"
                },
            ]
          });
    }
}