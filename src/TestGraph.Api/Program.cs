var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
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

app.MapGet("/api/health", () => Results.Ok(new
{
    service = "TestGraph.Api",
    status = "ok",
    version = "0.1.0"
}));

app.Run();
