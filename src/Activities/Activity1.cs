namespace TH.MyApp.DurableFunctionsSample.Activities
{
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Extensions.Logging;

    public class Activity1
    {
        private readonly DurableFunctionsSampleContext _durableFunctionsSampleContext;

        public Activity1(DurableFunctionsSampleContext durableFunctionsSampleContext)
        {
            _durableFunctionsSampleContext = durableFunctionsSampleContext;
        }

        [Function("Activity1")]
        public async Task RunActivity([ActivityTrigger] string fileName, FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("RunDurableFunctionsSample");
            logger.LogInformation($"1 started. fileName: {fileName}");

            var log = new ProcessStartLog
            {
                FileName = fileName,
                StartTime = DateTime.Now
            };

            _durableFunctionsSampleContext.ProcessStartLogs.Add(log);
            await _durableFunctionsSampleContext.SaveChangesAsync();
        }
    }
}