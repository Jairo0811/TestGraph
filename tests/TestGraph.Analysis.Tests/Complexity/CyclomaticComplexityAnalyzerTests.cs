using TestGraph.Analysis.Complexity;
using TestGraph.Analysis.ControlFlow;
using TestGraph.Analysis.Parsing;

namespace TestGraph.Analysis.Tests.Complexity;

public sealed class CyclomaticComplexityAnalyzerTests
{
    [Fact]
    public void Analyze_StraightLineProgram_ReturnsOne()
    {
        var graph = BuildGraph("""
            Entero edad
            Leer edad
            Escribir edad
            """);

        var result = new CyclomaticComplexityAnalyzer().Analyze(graph);

        Assert.Equal(1, result.Value);
        Assert.Equal(0, result.PredicateNodeCount);
        Assert.Equal(1, result.PredicateComplexity);
        Assert.Equal(1, result.RegionCount);
        Assert.True(result.FormulasAgree);
        Assert.Equal(ComplexityLevel.Low, result.Level);
    }

    [Fact]
    public void Analyze_IfElse_ReturnsTwo()
    {
        var graph = BuildGraph("""
            Si edad >= 18 Entonces
                Escribir "Adulto"
            Sino
                Escribir "Menor"
            Fin Si
            """);

        var result = new CyclomaticComplexityAnalyzer().Analyze(graph);

        Assert.Equal(2, result.Value);
        Assert.Equal(1, result.PredicateNodeCount);
        Assert.Equal(2, result.PredicateComplexity);
        Assert.True(result.FormulasAgree);
    }

    [Fact]
    public void Analyze_WhileLoop_ReturnsTwo()
    {
        var graph = BuildGraph("""
            Mientras edad < 18 Hacer
                edad <- edad + 1
            Fin Mientras
            """);

        var result = new CyclomaticComplexityAnalyzer().Analyze(graph);

        Assert.Equal(2, result.Value);
        Assert.Equal(1, result.PredicateNodeCount);
        Assert.True(result.FormulasAgree);
    }

    [Fact]
    public void Analyze_ForLoop_ReturnsTwo()
    {
        var graph = BuildGraph("""
            Para i = 1 Hasta 10 Paso 1
                Escribir i
            Fin Para
            """);

        var result = new CyclomaticComplexityAnalyzer().Analyze(graph);

        Assert.Equal(2, result.Value);
        Assert.Equal(1, result.PredicateNodeCount);
        Assert.True(result.FormulasAgree);
    }

    [Fact]
    public void Analyze_FullAcademicScholarshipFlow_ReturnsEight()
    {
        var graph = BuildGraph("""
            Entero edad
            Real promedio
            Real beca

            Leer edad
            Leer promedio

            Si edad > 18 Entonces
                Si promedio >= 9 Entonces
                    beca <- 2000
                Sino Si promedio >= 7.5 Entonces
                    beca <- 1000
                Sino Si promedio >= 6 Entonces
                    beca <- 500
                Sino
                    beca <- 0
                Fin Si
            Sino
                Si promedio >= 9 Entonces
                    beca <- 3000
                Sino Si promedio >= 8 Entonces
                    beca <- 2000
                Sino Si promedio >= 6 Entonces
                    beca <- 100
                Sino
                    beca <- 0
                Fin Si
            Fin Si

            Escribir beca
            """);

        var result = new CyclomaticComplexityAnalyzer().Analyze(graph);

        Assert.Equal(8, result.Value);
        Assert.Equal(7, result.PredicateNodeCount);
        Assert.Equal(8, result.PredicateComplexity);
        Assert.Equal(8, result.RegionCount);
        Assert.Equal(ComplexityLevel.Moderate, result.Level);
        Assert.True(result.FormulasAgree);
    }

    [Fact]
    public void Analyze_NestedControlFlow_UsesGraphStructureNotSourceHeuristics()
    {
        var graph = BuildGraph("""
            Mientras activo Hacer
                Si promedio >= 9 Entonces
                    Escribir promedio
                Fin Si
            Fin Mientras
            """);

        var result = new CyclomaticComplexityAnalyzer().Analyze(graph);

        Assert.Equal(3, result.Value);
        Assert.Equal(2, result.PredicateNodeCount);
        Assert.True(result.FormulasAgree);
    }

    private static ControlFlowGraph BuildGraph(string source)
    {
        var parseResult = Parser.Parse(source);
        Assert.False(parseResult.HasErrors);

        return new ControlFlowGraphBuilder().Build(parseResult.Root);
    }
}
