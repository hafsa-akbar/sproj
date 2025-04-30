using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; 
using sproj.Data;

namespace sproj.Features.WorkHistory;

public class CloseJob : Endpoint<CloseJob.Request, EmptyResponse> {
    public required AppDbContext DbContext { get; set; }

    public override void Configure() {
        Delete("/jobs/{JobId}");
        Policies("Worker");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct) {
        var userId = int.Parse(User.FindFirst("user_id")!.Value);
        // Fetch all matching job rows for this jobId (e.g. two rows if it’s a couple job)
        var jobs = await DbContext.Jobs
          .Include(j => j.WorkerDetails)
          .Where(j => j.JobId == req.JobId 
                   && j.WorkerDetails.Any(w => w.UserId == userId))
          .ToListAsync(ct);
    
        if (!jobs.Any())
        {
            await SendNotFoundAsync();
            return;
        }
    
        // Remove all fetched rows
        DbContext.Jobs.RemoveRange(jobs);
        await DbContext.SaveChangesAsync(ct);
        await SendNoContentAsync();
    }


    public class Request {
      public int JobId { get; set; }
    }
} 