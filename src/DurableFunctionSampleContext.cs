using Microsoft.EntityFrameworkCore;
using TH.MyApp.DurableFunctionsSample.Domain;

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

