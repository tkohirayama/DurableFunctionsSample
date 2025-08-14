namespace TH.MyApp.DurableFunctionsSample.Activities
{
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    public class Activity2
    {
        private readonly IConfiguration _configuration;

        public Activity2(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Function("Activity2")]
        public async Task<Activity3Condition> RunActivity([ActivityTrigger] FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("RunDurableFunctionsSample");
            logger.LogInformation($"2 started");

            return await Task.Run(() => new Activity3Condition
            {
                Enabled3_1 = _configuration.GetValue("Activity3Condition:Enabled3_1", false),
                Count3_1 = _configuration.GetValue("Activity3Condition:Count3_1", 10),
                Enabled3_2 = _configuration.GetValue("Activity3Condition:Enabled3_2", false),
                Count3_2 = _configuration.GetValue("Activity3Condition:Count3_2", 10),
                Enabled3_3 = _configuration.GetValue("Activity3Condition:Enabled3_3", false),
                Count3_3 = _configuration.GetValue("Activity3Condition:Count3_3", 10),
            });
        }
    }
}