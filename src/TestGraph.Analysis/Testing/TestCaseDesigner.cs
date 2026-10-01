using TestGraph.Analysis.Paths;

namespace TestGraph.Analysis.Testing;

public sealed class TestCaseDesigner
{
    public TestCaseDesignResult Design(
        IReadOnlyList<ExecutionPath> basisPaths,
        IReadOnlyList<TestCaseDraft> drafts)
    {
        ArgumentNullException.ThrowIfNull(basisPaths);
        ArgumentNullException.ThrowIfNull(drafts);

        var cases = new List<StructuralTestCase>();
        var diagnostics = new List<TestCaseDesignDiagnostic>();
        var pathByNumber = basisPaths.ToDictionary(path => path.Number);

        for (var index = 0; index < drafts.Count; index++)
        {
            var draft = drafts[index];

            if (string.IsNullOrWhiteSpace(draft.Name))
            {
                diagnostics.Add(new(index, "TGPL-TC001", "Test case name is required."));
                continue;
            }

            if (string.IsNullOrWhiteSpace(draft.ExpectedResult))
            {
                diagnostics.Add(new(index, "TGPL-TC002", "Expected result is required."));
                continue;
            }

            ExecutionPath? linkedPath = null;

            if (draft.LinkedPathNumber is int pathNumber)
            {
                if (!pathByNumber.TryGetValue(pathNumber, out linkedPath))
                {
                    diagnostics.Add(new(
                        index,
                        "TGPL-TC003",
                        $"Basis path {pathNumber} does not exist."));
                    continue;
                }
            }

            cases.Add(new StructuralTestCase(
                cases.Count + 1,
                draft.Name.Trim(),
                new Dictionary<string, string>(draft.Inputs, StringComparer.OrdinalIgnoreCase),
                draft.ExpectedResult.Trim(),
                draft.Technique,
                draft.LinkedPathNumber,
                linkedPath?.NodeIds.ToArray() ?? [],
                linkedPath?.EdgeIndexes.ToArray() ?? []));
        }

        return new TestCaseDesignResult(cases, diagnostics);
    }
}
