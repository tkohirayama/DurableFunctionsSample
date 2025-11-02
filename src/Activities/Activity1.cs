namespace TH.MyApp.DurableFunctionsSample.Activities
{
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Extensions.Logging;
    using TH.MyApp.DurableFunctionsSample.Domain;

    public class Activity1
    {
        private readonly IProcessStartLogRepository _processStartLogRepository;

        public Activity1(IProcessStartLogRepository processStartLogRepository)
        {
            _processStartLogRepository = processStartLogRepository;
        }

        [Function("Activity1")]
        public async Task RunActivity([ActivityTrigger] string fileName, FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("RunDurableFunctionsSample");
            logger.LogInformation($"1 started. fileName: {fileName}");

            var log = new ProcessStartLog
            {
                FileName = fileName,
                StartTime = DateTime.UtcNow
            };

            _ = await _processStartLogRepository.Add(log);
        }
    }
}