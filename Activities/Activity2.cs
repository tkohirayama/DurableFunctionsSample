namespace TH.MyApp.DurableFunctionsSample.Activities
{
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    public class Activity2
    {

        [Function("Activity2")]
        // public static Task<Activity3Condition> RunActivity2([ActivityTrigger] IConfiguration configuration, FunctionContext executionContext)
        public static async Task<Activity3Condition> RunActivity2([ActivityTrigger] FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("RunDurableFunctionsSample");
            logger.LogInformation($"2 started");

            return await Task.Run(() => new Activity3Condition
            {
                // TODO: IConfigurationからの設定値読込
                // Enabled3_1 = configuration.GetValue("Activity3Condition:Enabled3_1", false),
                // Count3_1 = configuration.GetValue("Activity3Condition:Count3_1", 10),
                // Enabled3_2 = configuration.GetValue("Activity3Condition:Enabled3_2", false),
                // Count3_2 = configuration.GetValue("Activity3Condition:Count3_2", 10),
                // Enabled3_3 = configuration.GetValue("Activity3Condition:Enabled3_3", false),
                // Count3_3 = configuration.GetValue("Activity3Condition:Count3_3", 10),

                Enabled3_1 = true,
                Count3_1 = 10,
                Enabled3_2 = true,
                Count3_2 = 15,
                Enabled3_3 = true,
                Count3_3 = 20
            });
        }
    }
}