using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
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
        var group = endpoints.MapGroup("/api/projects").RequireAuthorization();

        group.MapGet("", async (ClaimsPrincipal principal, TestGraphDbContext db, CancellationToken ct) =>
        {
            var userId = GetUserId(principal);

            return Results.Ok(await db.Projects
                .AsNoTracking()
                .Where(project => project.OwnerUserId == userId)
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
                .ToListAsync(ct));
        });

        group.MapPost("", async (
            CreateProjectRequest request,
            ClaimsPrincipal principal,
            TestGraphDbContext db,
            CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Results.BadRequest(new { error = "Project name is required." });
            }

            var userId = GetUserId(principal);
            var project = new Project(Guid.NewGuid(), userId, request.Name, request.Description);
            db.Projects.Add(project);
            await db.SaveChangesAsync(ct);

            return Results.Created(
                $"/api/projects/{project.Id}",
                new { project.Id, project.Name, project.Description });
        });

        group.MapGet("/{id:guid}", async (
            Guid id,
            ClaimsPrincipal principal,
            TestGraphDbContext db,
            CancellationToken ct) =>
        {
            var userId = GetUserId(principal);
            var project = await db.Projects
                .AsNoTracking()
                .Where(item => item.Id == id && item.OwnerUserId == userId)
                .Select(item => new
                {
                    item.Id,
                    item.Name,
                    item.Description,
                    item.CreatedAt,
                    item.UpdatedAt,
                    analyses = item.Analyses
                        .OrderByDescending(analysis => analysis.CreatedAt)
                        .Select(analysis => new
                        {
                            analysis.Id,
                            analysis.CyclomaticComplexity,
                            analysis.CreatedAt
                        })
                        .ToList()
                })
                .SingleOrDefaultAsync(ct);

            return project is null ? Results.NotFound() : Results.Ok(project);
        });

        group.MapPost("/{id:guid}/analyses", async (
            Guid id,
            SaveAnalysisRequest request,
            ClaimsPrincipal principal,
            TestGraphDbContext db,
            CancellationToken ct) =>
        {
            var userId = GetUserId(principal);
            var projectExists = await db.Projects.AnyAsync(
                project => project.Id == id && project.OwnerUserId == userId,
                ct);

            if (!projectExists)
            {
                return Results.NotFound(new { error = "Project not found." });
            }

            if (string.IsNullOrWhiteSpace(request.SourceCode))
            {
                return Results.BadRequest(new { error = "SourceCode is required." });
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

            foreach (var testCase in request.TestCases ?? [])
            {
                if (string.IsNullOrWhiteSpace(testCase.Name) || string.IsNullOrWhiteSpace(testCase.ExpectedResult))
                {
                    return Results.BadRequest(new { error = "Persisted test cases require name and expectedResult." });
                }

                db.TestCases.Add(new PersistedTestCase(
                    Guid.NewGuid(),
                    analysis.Id,
                    testCase.Name.Trim(),
                    JsonSerializer.Serialize(testCase.Inputs ?? new Dictionary<string, string>()),
                    testCase.ExpectedResult.Trim(),
                    string.IsNullOrWhiteSpace(testCase.Technique) ? "Manual" : testCase.Technique.Trim(),
                    testCase.LinkedPathNumber));
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
                    paths = paths.Paths.Count,
                    testCases = request.TestCases?.Count ?? 0
                });
        });

        group.MapGet("/{projectId:guid}/analyses/{analysisId:guid}", async (
            Guid projectId,
            Guid analysisId,
            ClaimsPrincipal principal,
            TestGraphDbContext db,
            CancellationToken ct) =>
        {
            var userId = GetUserId(principal);
            var ownsProject = await db.Projects.AnyAsync(
                project => project.Id == projectId && project.OwnerUserId == userId,
                ct);

            if (!ownsProject)
            {
                return Results.NotFound();
            }

            var analysis = await db.Analyses
                .AsNoTracking()
                .Where(item => item.ProjectId == projectId && item.Id == analysisId)
                .Select(item => new
                {
                    item.Id,
                    item.ProjectId,
                    item.SourceCode,
                    item.CyclomaticComplexity,
                    item.CreatedAt,
                    graphNodes = item.GraphNodes
                        .OrderBy(node => node.NodeNumber)
                        .Select(node => new
                        {
                            node.NodeNumber,
                            node.Type,
                            node.Label,
                            node.SourceLine
                        })
                        .ToList(),
                    graphEdges = item.GraphEdges
                        .OrderBy(edge => edge.SourceNodeNumber)
                        .ThenBy(edge => edge.TargetNodeNumber)
                        .Select(edge => new
                        {
                            edge.SourceNodeNumber,
                            edge.TargetNodeNumber,
                            edge.Kind,
                            edge.Label
                        })
                        .ToList(),
                    executionPaths = item.ExecutionPaths
                        .OrderBy(path => path.PathNumber)
                        .Select(path => new
                        {
                            path.PathNumber,
                            path.NodeSequenceJson
                        })
                        .ToList(),
                    testCases = item.TestCases
                        .Select(testCase => new
                        {
                            testCase.Name,
                            testCase.InputsJson,
                            testCase.ExpectedResult,
                            testCase.Technique,
                            testCase.LinkedPathNumber
                        })
                        .ToList()
                })
                .SingleOrDefaultAsync(ct);

            return analysis is null ? Results.NotFound() : Results.Ok(analysis);
        });

        return endpoints;
    }

    private static Guid GetUserId(ClaimsPrincipal principal)
    {
        var raw = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(raw, out var id)
            ? id
            : throw new InvalidOperationException("Authenticated user identifier is missing.");
    }
}

public sealed record CreateProjectRequest(string Name, string? Description);
public sealed record SaveAnalysisRequest(string SourceCode, IReadOnlyList<SavedTestCaseRequest>? TestCases = null);
public sealed record SavedTestCaseRequest(
    string Name,
    IReadOnlyDictionary<string, string>? Inputs,
    string ExpectedResult,
    string Technique,
    int? LinkedPathNumber);
