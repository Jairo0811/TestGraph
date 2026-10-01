using TestGraph.Api;
using TestGraph.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddTestGraphPersistence(builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

app.UseExceptionHandler();
app.UseCors("Frontend");

app.MapAnalysisEndpoints();
app.MapProjectEndpoints();

app.MapGet("/api/health", () => Results.Ok(new
{
    service = "TestGraph.Api",
    status = "ok",
    version = "0.1.0"
}));

app.Run();
