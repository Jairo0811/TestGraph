using System.Globalization;
using TestGraph.Analysis.Syntax;

namespace TestGraph.Analysis.Testing;

public sealed class AssistedTestCaseGenerator
{
    public AssistedTestGenerationResult Generate(CompilationUnitSyntax root)
    {
        ArgumentNullException.ThrowIfNull(root);

        var boundaries = new BoundaryAnalyzer().Analyze(root);
        var cases = new List<SuggestedTestCase>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var boundary in boundaries)
        {
            foreach (var value in boundary.Values)
            {
                var formattedValue = value.ToString("0.############################", CultureInfo.InvariantCulture);
                var key = $"{boundary.Variable}|{formattedValue}";

                if (!seen.Add(key))
                {
                    continue;
                }

                cases.Add(new SuggestedTestCase(
                    cases.Count + 1,
                    $"{boundary.Variable} = {formattedValue}",
                    new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        [boundary.Variable] = formattedValue
                    },
                    $"Verify behavior around condition {boundary.Variable} {boundary.Operator} {boundary.Threshold.ToString(CultureInfo.InvariantCulture)}.",
                    TestCaseTechnique.BoundaryValue,
                    $"Boundary value around {boundary.Variable} {boundary.Operator} {boundary.Threshold.ToString(CultureInfo.InvariantCulture)}.",
                    boundary.SourceLine));
            }
        }

        return new AssistedTestGenerationResult(boundaries, cases);
    }
}
