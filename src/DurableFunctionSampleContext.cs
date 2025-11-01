using Microsoft.EntityFrameworkCore;

namespace TH.MyApp.DurableFunctionsSample;

public class DurableFunctionsSampleContext : DbContext
{
    public DurableFunctionsSampleContext(DbContextOptions<DurableFunctionsSampleContext> options)
        : base(options)
    {
    }

    // protected override void OnConfiguring(DbContextOptionsBuilder options)
    //     => options.UseSqlite($"Data Source={DbPath}");

    public DbSet<ProcessStartLog> ProcessStartLogs { get; set; } = null!;
}

public class ProcessStartLog
{
    public int Id { get; set; }
    public string FileName { get; set; } = null!;
    public DateTime StartTime { get; set; }
}