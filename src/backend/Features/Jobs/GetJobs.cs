using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using sproj.Data;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace sproj.Features.Jobs;

// TODO: Only selective data should be sent
public class GetJobs : Endpoint<EmptyRequest, EmptyResponse> {
    public required AppDbContext DbContext { get; set; }

    public override void Configure() {
        Get("/jobs");
        AllowAnonymous();
    }

    public override async Task HandleAsync(EmptyRequest _, CancellationToken ct) {
        var jobs = await DbContext.Jobs.ToListAsync();

        await SendResultAsync(Results.Ok(jobs));
    }
}
