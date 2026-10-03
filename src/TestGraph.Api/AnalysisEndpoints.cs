using TestGraph.Analysis.Complexity;
using TestGraph.Analysis.ControlFlow;
using TestGraph.Analysis.Parsing;
using TestGraph.Analysis.Paths;
using TestGraph.Analysis.Matrix;
using TestGraph.Analysis.Testing;

namespace TestGraph.Api;

public static class AnalysisEndpoints
{
    public static IEndpointRouteBuilder MapAnalysisEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/analysis", (ComplexityRequest request) =>
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
            var complexity = new CyclomaticComplexityAnalyzer().Analyze(graph);
            var paths = new BasisPathAnalyzer().Analyze(graph);
            var matrix = new AdjacencyMatrixBuilder().Build(graph);
            var suggestions = new AssistedTestCaseGenerator().Generate(parseResult.Root);

            return Results.Ok(new
            {
                graph = new
                {
                    nodes = graph.Nodes.Select(node => new
                    {
                        node.Id,
                        kind = node.Kind.ToString(),
                        node.Label,
                        sourceLine = node.SourceSpan?.Start.Line
                    }),
                    edges = graph.Edges.Select(edge => new
                    {
                        edge.SourceId,
                        edge.TargetId,
                        kind = edge.Kind.ToString(),
                        edge.Label
                    }),
                    graph.EntryNodeId,
                    graph.ExitNodeId
                },
                complexity = new
                {
                    value = complexity.Value,
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
                },
                paths = new
                {
                    cyclomaticComplexity = paths.CyclomaticComplexity,
                    pathCount = paths.Paths.Count,
                    candidatePathCount = paths.CandidatePathCount,
                    complete = paths.IsComplete,
                    items = paths.Paths.Select(path => new
                    {
                        number = path.Number,
                        nodeIds = path.NodeIds,
                        edgeIndexes = path.EdgeIndexes,
                        display = path.Display
                    })
                },
                matrix = new
                {
                    nodeIds = matrix.NodeIds,
                    rows = matrix.Rows,
                    size = matrix.Size
                },
                suggestions = new
                {
                    boundaries = suggestions.Boundaries,
                    testCases = suggestions.TestCases.Select(testCase => new
                    {
                        testCase.Number,
                        testCase.Name,
                        testCase.Inputs,
                        testCase.ExpectedResult,
                        technique = testCase.Technique.ToString(),
                        testCase.Rationale,
                        testCase.SourceLine
                    })
                }
            });
        });

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

        endpoints.MapPost("/api/analysis/matrix", (ComplexityRequest request) =>
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
            var matrix = new AdjacencyMatrixBuilder().Build(graph);

            return Results.Ok(new
            {
                nodeIds = matrix.NodeIds,
                rows = matrix.Rows,
                size = matrix.Size
            });
        });

        endpoints.MapPost("/api/analysis/test-cases/design", (TestCaseDesignRequest request) =>
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
            var paths = new BasisPathAnalyzer().Analyze(graph).Paths;
            var drafts = new List<TestCaseDraft>();

            foreach (var item in request.TestCases)
            {
                if (!Enum.TryParse<TestCaseTechnique>(item.Technique, true, out var technique))
                {
                    return Results.BadRequest(new
                    {
                        error = $"Unknown test-case technique '{item.Technique}'."
                    });
                }

                drafts.Add(new TestCaseDraft(
                    item.Name,
                    item.Inputs ?? new Dictionary<string, string>(),
                    item.ExpectedResult,
                    technique,
                    item.LinkedPathNumber));
            }

            var result = new TestCaseDesigner().Design(paths, drafts);
            return Results.Ok(result);
        });

        endpoints.MapPost("/api/analysis/test-cases/suggest", (ComplexityRequest request) =>
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

            var result = new AssistedTestCaseGenerator().Generate(parseResult.Root);
            return Results.Ok(result);
        });

        return endpoints;
    }
}

public sealed record ComplexityRequest(string SourceCode);

public sealed record TestCaseDesignRequest(string SourceCode, IReadOnlyList<TestCaseDraftRequest> TestCases);
public sealed record TestCaseDraftRequest(string Name, IReadOnlyDictionary<string, string>? Inputs, string ExpectedResult, string Technique, int? LinkedPathNumber);
