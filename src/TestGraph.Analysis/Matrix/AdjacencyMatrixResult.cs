namespace TestGraph.Analysis.Matrix;

public sealed record AdjacencyMatrixResult(
    IReadOnlyList<int> NodeIds,
    IReadOnlyList<IReadOnlyList<int>> Rows)
{
    public int Size => NodeIds.Count;

    public int this[int row, int column] => Rows[row][column];

    public string ToCsv()
    {
        var lines = new List<string>
        {
            "," + string.Join(",", NodeIds)
        };

        for (var row = 0; row < Rows.Count; row++)
        {
            lines.Add($"{NodeIds[row]},{string.Join(",", Rows[row])}");
        }

        return string.Join(Environment.NewLine, lines);
    }
}
