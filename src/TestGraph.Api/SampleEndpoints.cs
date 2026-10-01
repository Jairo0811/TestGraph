using TestGraph.Analysis.Samples;

namespace TestGraph.Api;

public static class SampleEndpoints
{
    public static IEndpointRouteBuilder MapSampleEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/samples", () =>
            Results.Ok(AcademicSampleCatalog.All.Select(sample => new
            {
                sample.Id,
                sample.Name,
                sample.OriginalAuthor,
                sample.Description,
                sample.ExpectedCyclomaticComplexity,
                sample.AnalyzerReady,
                sample.Limitation
            })));

        endpoints.MapGet("/api/samples/{id}", (string id) =>
        {
            var sample = AcademicSampleCatalog.Find(id);
            return sample is null ? Results.NotFound() : Results.Ok(sample);
        });

        return endpoints;
    }
}
