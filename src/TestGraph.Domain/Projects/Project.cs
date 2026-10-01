namespace TestGraph.Domain.Projects;

public sealed class Project
{
    private Project() { }

    public Project(Guid id, string name, string? description = null)
    {
        Id = id;
        Rename(name);
        Description = NormalizeOptional(description);
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public ICollection<AnalysisRecord> Analyses { get; private set; } = new List<AnalysisRecord>();

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Project name is required.", nameof(name));
        }

        Name = name.Trim();
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SetDescription(string? description)
    {
        Description = NormalizeOptional(description);
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
