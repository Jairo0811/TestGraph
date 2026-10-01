namespace TestGraph.Domain.Projects;

public sealed class AnalysisRecord
{
    private AnalysisRecord() { }

    public AnalysisRecord(Guid id, Guid projectId, string sourceCode, int cyclomaticComplexity)
    {
        if (string.IsNullOrWhiteSpace(sourceCode))
        {
            throw new ArgumentException("Source code is required.", nameof(sourceCode));
        }

        Id = id;
        ProjectId = projectId;
        SourceCode = sourceCode;
        CyclomaticComplexity = cyclomaticComplexity;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid ProjectId { get; private set; }
    public string SourceCode { get; private set; } = string.Empty;
    public int CyclomaticComplexity { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public Project? Project { get; private set; }
    public ICollection<PersistedGraphNode> GraphNodes { get; private set; } = new List<PersistedGraphNode>();
    public ICollection<PersistedGraphEdge> GraphEdges { get; private set; } = new List<PersistedGraphEdge>();
    public ICollection<PersistedExecutionPath> ExecutionPaths { get; private set; } = new List<PersistedExecutionPath>();
    public ICollection<PersistedTestCase> TestCases { get; private set; } = new List<PersistedTestCase>();
}

public sealed class PersistedGraphNode
{
    private PersistedGraphNode() { }

    public PersistedGraphNode(Guid id, Guid analysisId, int nodeNumber, string type, string label, int? sourceLine)
    {
        Id = id;
        AnalysisId = analysisId;
        NodeNumber = nodeNumber;
        Type = type;
        Label = label;
        SourceLine = sourceLine;
    }

    public Guid Id { get; private set; }
    public Guid AnalysisId { get; private set; }
    public int NodeNumber { get; private set; }
    public string Type { get; private set; } = string.Empty;
    public string Label { get; private set; } = string.Empty;
    public int? SourceLine { get; private set; }
    public AnalysisRecord? Analysis { get; private set; }
}

public sealed class PersistedGraphEdge
{
    private PersistedGraphEdge() { }

    public PersistedGraphEdge(Guid id, Guid analysisId, int sourceNodeNumber, int targetNodeNumber, string kind, string? label)
    {
        Id = id;
        AnalysisId = analysisId;
        SourceNodeNumber = sourceNodeNumber;
        TargetNodeNumber = targetNodeNumber;
        Kind = kind;
        Label = label;
    }

    public Guid Id { get; private set; }
    public Guid AnalysisId { get; private set; }
    public int SourceNodeNumber { get; private set; }
    public int TargetNodeNumber { get; private set; }
    public string Kind { get; private set; } = string.Empty;
    public string? Label { get; private set; }
    public AnalysisRecord? Analysis { get; private set; }
}

public sealed class PersistedExecutionPath
{
    private PersistedExecutionPath() { }

    public PersistedExecutionPath(Guid id, Guid analysisId, int pathNumber, string nodeSequenceJson)
    {
        Id = id;
        AnalysisId = analysisId;
        PathNumber = pathNumber;
        NodeSequenceJson = nodeSequenceJson;
    }

    public Guid Id { get; private set; }
    public Guid AnalysisId { get; private set; }
    public int PathNumber { get; private set; }
    public string NodeSequenceJson { get; private set; } = string.Empty;
    public AnalysisRecord? Analysis { get; private set; }
}

public sealed class PersistedTestCase
{
    private PersistedTestCase() { }

    public PersistedTestCase(Guid id, Guid analysisId, string name, string inputsJson, string expectedResult, string technique, int? linkedPathNumber)
    {
        Id = id;
        AnalysisId = analysisId;
        Name = name;
        InputsJson = inputsJson;
        ExpectedResult = expectedResult;
        Technique = technique;
        LinkedPathNumber = linkedPathNumber;
    }

    public Guid Id { get; private set; }
    public Guid AnalysisId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string InputsJson { get; private set; } = "{}";
    public string ExpectedResult { get; private set; } = string.Empty;
    public string Technique { get; private set; } = string.Empty;
    public int? LinkedPathNumber { get; private set; }
    public AnalysisRecord? Analysis { get; private set; }
}
