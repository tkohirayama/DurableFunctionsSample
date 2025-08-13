namespace TH.MyApp.DurableFunctionsSample.Activities
{
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Extensions.Logging;

    public class Activity1
    {
        [Function("Activity1")]
        public static Task RunActivity1([ActivityTrigger] string input, FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("RunDurableFunctionsSample");
            logger.LogInformation($"1 started {input}");
            return Task.CompletedTask;
        }
    }
}