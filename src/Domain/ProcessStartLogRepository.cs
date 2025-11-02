namespace TH.MyApp.DurableFunctionsSample.Domain
{
    public interface IProcessStartLogRepository
    {
        Task<int> Add(ProcessStartLog log);
    }

    public class ProcessStartLogRepository : IProcessStartLogRepository
    {
        private readonly DurableFunctionsSampleContext _context;

        public ProcessStartLogRepository(DurableFunctionsSampleContext context)
        {
            _context = context;
        }

        public async Task<int> Add(ProcessStartLog log)
        {
            _context.ProcessStartLogs.Add(log);
            await _context.SaveChangesAsync();
            return log.Id;
        }
    }
}