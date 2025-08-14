using Raven.Client.Documents;
using Raven.Client.Documents.Conventions;
using Raven.Client.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });
builder.Services.AddOpenApi();

builder.Services.AddSingleton<IDocumentStore>(provider =>
{
    var store = new DocumentStore
    {
        Urls = new[] { "http://localhost:8080" },
        Database = "HRAssistant",
    };

    store.Initialize();
    return store;
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

{
    var store = app.Services.GetRequiredService<IDocumentStore>();
    Console.WriteLine("Creating agent...");
    _ = HumanResourcesAgent.Create(store).ContinueWith(task =>
    {
        if (task.IsFaulted)
        {
            Console.WriteLine("Failed to create agent: " + task.Exception);
        }
        else
        {
            Console.WriteLine("Agent created successfully.");
        }
    });
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
