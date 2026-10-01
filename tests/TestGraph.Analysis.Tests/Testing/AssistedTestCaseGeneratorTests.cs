using TestGraph.Analysis.Parsing;
using TestGraph.Analysis.Testing;

namespace TestGraph.Analysis.Tests.Testing;

public sealed class AssistedTestCaseGeneratorTests
{
    [Fact]
    public void Generate_IntegerBoundary_UsesUnitStep()
    {
        var result = Generate("""
            Entero edad
            Si edad > 18 Entonces
                Escribir edad
            Fin Si
            """);

        var boundary = Assert.Single(result.Boundaries);

        Assert.Equal("edad", boundary.Variable);
        Assert.Equal([17m, 18m, 19m], boundary.Values);
        Assert.Contains(result.TestCases, test => test.Inputs["edad"] == "17");
        Assert.Contains(result.TestCases, test => test.Inputs["edad"] == "19");
    }

    [Fact]
    public void Generate_RealBoundary_UsesHundredthStep()
    {
        var result = Generate("""
            Real promedio
            Si promedio >= 9 Entonces
                Escribir promedio
            Fin Si
            """);

        var boundary = Assert.Single(result.Boundaries);

        Assert.Equal([8.99m, 9m, 9.01m], boundary.Values);
        Assert.Contains(result.TestCases, test => test.Inputs["promedio"] == "8.99");
        Assert.Contains(result.TestCases, test => test.Inputs["promedio"] == "9.01");
    }

    [Fact]
    public void Generate_ScholarshipFlow_DetectsAllNumericPredicates()
    {
        var result = Generate("""
            Entero edad
            Real promedio

            Si edad > 18 Entonces
                Si promedio >= 9 Entonces
                    Escribir 2000
                Sino Si promedio >= 7.5 Entonces
                    Escribir 1000
                Sino Si promedio >= 6 Entonces
                    Escribir 500
                Fin Si
            Sino
                Si promedio >= 9 Entonces
                    Escribir 3000
                Sino Si promedio >= 8 Entonces
                    Escribir 2000
                Sino Si promedio >= 6 Entonces
                    Escribir 100
                Fin Si
            Fin Si
            """);

        Assert.Equal(7, result.Boundaries.Count);
        Assert.Contains(result.Boundaries, boundary => boundary.Variable == "edad" && boundary.Threshold == 18m);
        Assert.Contains(result.Boundaries, boundary => boundary.Variable == "promedio" && boundary.Threshold == 7.5m);
        Assert.NotEmpty(result.TestCases);
    }

    [Fact]
    public void Generate_ReversedComparison_NormalizesOperator()
    {
        var result = Generate("""
            Entero edad
            Si 18 < edad Entonces
                Escribir edad
            Fin Si
            """);

        var boundary = Assert.Single(result.Boundaries);

        Assert.Equal(">", boundary.Operator);
        Assert.Equal(18m, boundary.Threshold);
    }

    private static AssistedTestGenerationResult Generate(string source)
    {
        var parse = Parser.Parse(source);
        Assert.False(parse.HasErrors);
        return new AssistedTestCaseGenerator().Generate(parse.Root);
    }
}
