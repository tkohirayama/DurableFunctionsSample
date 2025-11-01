namespace TH.MyApp.DurableFunctionsSample
{
    using Microsoft.EntityFrameworkCore;

    public class DurableFunctionsSampleContextFactory
    {
        public DurableFunctionsSampleContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DurableFunctionsSampleContext>();
            optionsBuilder.UseSqlite(args[0]);

            return new DurableFunctionsSampleContext(optionsBuilder.Options);
        }
    }
}