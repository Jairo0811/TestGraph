using System.Text;
using System.Text.Json;
using TestGraph.Analysis.Reporting;

namespace TestGraph.Api;

public static class ExportEndpoints
{
    public static IEndpointRouteBuilder MapExportEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/analysis/export/json", (ExportRequest request) =>
        {
            try
            {
                var report = new AnalysisReportBuilder().Build(request.SourceCode);
                var json = JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true });
                return Results.File(Encoding.UTF8.GetBytes(json), "application/json", "testgraph-analysis.json");
            }
            catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
            {
                return Results.BadRequest(new { error = exception.Message });
            }
        });

        endpoints.MapPost("/api/analysis/export/matrix.csv", (ExportRequest request) =>
        {
            try
            {
                var report = new AnalysisReportBuilder().Build(request.SourceCode);
                return Results.File(Encoding.UTF8.GetBytes(report.Matrix.ToCsv()), "text/csv", "testgraph-matrix.csv");
            }
            catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
            {
                return Results.BadRequest(new { error = exception.Message });
            }
        });

        endpoints.MapPost("/api/analysis/report/markdown", (ExportRequest request) =>
        {
            try
            {
                var builder = new AnalysisReportBuilder();
                var markdown = builder.ToMarkdown(builder.Build(request.SourceCode));
                return Results.File(Encoding.UTF8.GetBytes(markdown), "text/markdown", "testgraph-report.md");
            }
            catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
            {
                return Results.BadRequest(new { error = exception.Message });
            }
        });

        return endpoints;
    }
}

public sealed record ExportRequest(string SourceCode);
