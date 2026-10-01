using TestGraph.Analysis.Complexity;
using TestGraph.Analysis.ControlFlow;
using TestGraph.Analysis.Parsing;
using TestGraph.Analysis.Paths;

namespace TestGraph.Api;

public static class AnalysisEndpoints
{
    public static IEndpointRouteBuilder MapAnalysisEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/analysis/complexity", (ComplexityRequest request) =>
        {
            if (string.IsNullOrWhiteSpace(request.SourceCode))
            {
                return Results.BadRequest(new
                {
                    error = "SourceCode is required."
                });
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

            return Results.Ok(new
            {
                complexity = complexity.Value,
                nodes = complexity.NodeCount,
                edges = complexity.EdgeCount,
                predicates = complexity.PredicateNodeCount,
                regions = complexity.RegionCount,
                connectedComponents = complexity.ConnectedComponentCount,
                formulas = new
                {
                    edgeNode = complexity.EdgeNodeComplexity,
                    predicate = complexity.PredicateComplexity,
                    region = complexity.RegionCount,
                    agree = complexity.FormulasAgree
                },
                level = complexity.Level.ToString()
            });
        });

        endpoints.MapPost("/api/analysis/paths", (ComplexityRequest request) =>
        {
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
            var result = new BasisPathAnalyzer().Analyze(graph);

            return Results.Ok(new
            {
                cyclomaticComplexity = result.CyclomaticComplexity,
                pathCount = result.Paths.Count,
                candidatePathCount = result.CandidatePathCount,
                complete = result.IsComplete,
                paths = result.Paths.Select(path => new
                {
                    number = path.Number,
                    nodeIds = path.NodeIds,
                    edgeIndexes = path.EdgeIndexes,
                    display = path.Display
                })
            });
        });

        return endpoints;
    }
}

public sealed record ComplexityRequest(string SourceCode);
