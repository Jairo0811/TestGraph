using Microsoft.EntityFrameworkCore;
using TestGraph.Domain.Projects;

namespace TestGraph.Infrastructure.Persistence;

public sealed class TestGraphDbContext(DbContextOptions<TestGraphDbContext> options)
    : DbContext(options)
{
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<AnalysisRecord> Analyses => Set<AnalysisRecord>();
    public DbSet<PersistedGraphNode> GraphNodes => Set<PersistedGraphNode>();
    public DbSet<PersistedGraphEdge> GraphEdges => Set<PersistedGraphEdge>();
    public DbSet<PersistedExecutionPath> ExecutionPaths => Set<PersistedExecutionPath>();
    public DbSet<PersistedTestCase> TestCases => Set<PersistedTestCase>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>(entity =>
        {
            entity.ToTable("Projects");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(160).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(1000);
            entity.HasMany(x => x.Analyses)
                .WithOne(x => x.Project)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AnalysisRecord>(entity =>
        {
            entity.ToTable("Analyses");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.SourceCode).HasColumnType("nvarchar(max)").IsRequired();
            entity.HasMany(x => x.GraphNodes).WithOne(x => x.Analysis).HasForeignKey(x => x.AnalysisId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(x => x.GraphEdges).WithOne(x => x.Analysis).HasForeignKey(x => x.AnalysisId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(x => x.ExecutionPaths).WithOne(x => x.Analysis).HasForeignKey(x => x.AnalysisId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(x => x.TestCases).WithOne(x => x.Analysis).HasForeignKey(x => x.AnalysisId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PersistedGraphNode>(entity =>
        {
            entity.ToTable("GraphNodes");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Type).HasMaxLength(40).IsRequired();
            entity.Property(x => x.Label).HasMaxLength(500).IsRequired();
        });

        modelBuilder.Entity<PersistedGraphEdge>(entity =>
        {
            entity.ToTable("GraphEdges");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Kind).HasMaxLength(40).IsRequired();
            entity.Property(x => x.Label).HasMaxLength(120);
        });

        modelBuilder.Entity<PersistedExecutionPath>(entity =>
        {
            entity.ToTable("ExecutionPaths");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.NodeSequenceJson).HasColumnType("nvarchar(max)").IsRequired();
        });

        modelBuilder.Entity<PersistedTestCase>(entity =>
        {
            entity.ToTable("TestCases");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.InputsJson).HasColumnType("nvarchar(max)").IsRequired();
            entity.Property(x => x.ExpectedResult).HasMaxLength(1000).IsRequired();
            entity.Property(x => x.Technique).HasMaxLength(80).IsRequired();
        });
    }
}
