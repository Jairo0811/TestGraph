using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TestGraph.Analysis.Complexity;
using TestGraph.Analysis.ControlFlow;
using TestGraph.Analysis.Parsing;
using TestGraph.Analysis.Paths;
using TestGraph.Domain.Projects;
using TestGraph.Infrastructure.Persistence;

namespace TestGraph.Api;

public static class ProjectEndpoints
{
    public static IEndpointRouteBuilder MapProjectEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/projects", async (TestGraphDbContext db, CancellationToken ct) =>
            Results.Ok(await db.Projects
                .AsNoTracking()
                .OrderByDescending(project => project.UpdatedAt)
                .Select(project => new
                {
                    project.Id,
                    project.Name,
                    project.Description,
                    project.CreatedAt,
                    project.UpdatedAt,
                    analysisCount = project.Analyses.Count
                })
                .ToListAsync(ct)));

        endpoints.MapPost("/api/projects", async (CreateProjectRequest request, TestGraphDbContext db, CancellationToken ct) =>
        {
            var project = new Project(Guid.NewGuid(), request.Name, request.Description);
            db.Projects.Add(project);
            await db.SaveChangesAsync(ct);
            return Results.Created($"/api/projects/{project.Id}", new { project.Id, project.Name, project.Description });
        });

        endpoints.MapGet("/api/projects/{id:guid}", async (Guid id, TestGraphDbContext db, CancellationToken ct) =>
        {
            var project = await db.Projects
                .AsNoTracking()
                .Include(project => project.Analyses)
                .SingleOrDefaultAsync(project => project.Id == id, ct);

            return project is null
                ? Results.NotFound()
                : Results.Ok(project);
        });

        endpoints.MapPost("/api/projects/{id:guid}/analyses", async (
            Guid id,
            SaveAnalysisRequest request,
            TestGraphDbContext db,
            CancellationToken ct) =>
        {
            var projectExists = await db.Projects.AnyAsync(project => project.Id == id, ct);
            if (!projectExists)
            {
                return Results.NotFound(new { error = "Project not found." });
            }

            var parseResult = Parser.Parse(request.SourceCode);
            if (parseResult.HasErrors)
            {
                return Results.BadRequest(new
                {
                    error = "TGPL source contains lexical or parser errors.",
                    lexerDiagnostics = parseResult.LexerDiagnostics,
                    parserDiagnostics = parseResult.Diagnostics
                });
            }

            var graph = new ControlFlowGraphBuilder().Build(parseResult.Root);
            var complexity = new CyclomaticComplexityAnalyzer().Analyze(graph);
            var paths = new BasisPathAnalyzer().Analyze(graph);

            var analysis = new AnalysisRecord(Guid.NewGuid(), id, request.SourceCode, complexity.Value);
            db.Analyses.Add(analysis);

            foreach (var node in graph.Nodes)
            {
                db.GraphNodes.Add(new PersistedGraphNode(
                    Guid.NewGuid(),
                    analysis.Id,
                    node.Id,
                    node.Kind.ToString(),
                    node.Label,
                    node.SourceSpan?.Start.Line));
            }

            foreach (var edge in graph.Edges)
            {
                db.GraphEdges.Add(new PersistedGraphEdge(
                    Guid.NewGuid(),
                    analysis.Id,
                    edge.SourceId,
                    edge.TargetId,
                    edge.Kind.ToString(),
                    edge.Label));
            }

            foreach (var path in paths.Paths)
            {
                db.ExecutionPaths.Add(new PersistedExecutionPath(
                    Guid.NewGuid(),
                    analysis.Id,
                    path.Number,
                    JsonSerializer.Serialize(path.NodeIds)));
            }

            await db.SaveChangesAsync(ct);

            return Results.Created(
                $"/api/projects/{id}/analyses/{analysis.Id}",
                new
                {
                    analysis.Id,
                    analysis.ProjectId,
                    analysis.CyclomaticComplexity,
                    nodes = graph.Nodes.Count,
                    edges = graph.Edges.Count,
                    paths = paths.Paths.Count
                });
        });

        endpoints.MapGet("/api/projects/{projectId:guid}/analyses/{analysisId:guid}", async (
            Guid projectId,
            Guid analysisId,
            TestGraphDbContext db,
            CancellationToken ct) =>
        {
            var analysis = await db.Analyses
                .AsNoTracking()
                .Include(item => item.GraphNodes)
                .Include(item => item.GraphEdges)
                .Include(item => item.ExecutionPaths)
                .Include(item => item.TestCases)
                .SingleOrDefaultAsync(item => item.ProjectId == projectId && item.Id == analysisId, ct);

            return analysis is null ? Results.NotFound() : Results.Ok(analysis);
        });

        return endpoints;
    }
}

public sealed record CreateProjectRequest(string Name, string? Description);
public sealed record SaveAnalysisRequest(string SourceCode);
