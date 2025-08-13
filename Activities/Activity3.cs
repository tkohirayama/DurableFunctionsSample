namespace TH.MyApp.DurableFunctionsSample.Activities
{
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Extensions.Logging;

    public class Activity3
    {
        [Function("Activity3")]
        public static async Task<Activity3Result> RunActivity3_1([ActivityTrigger] Activity3Param param, FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("RunDurableFunctionsSample");
            logger.LogInformation($"{param.TaskName} started");
            for (int i = 1; i <= param.Count; i++)
            {
                await Task.Delay(1000); // 1秒待機
                logger.LogInformation($"Activity3_1: {i}秒経過");
            }
            logger.LogInformation($"{param.TaskName} finish");
            return new Activity3Result
            {
                Status = param.Status
            };
        }
    }

    public class Activity3Param
    {
        public required string TaskName { get; set; } = "Activity3_x";
        public required int Count { get; set; } = 10;
        public required string Status { get; set; } = "success";
    }
}