using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using TH.MyApp.DurableFunctionsSample.Activities;

namespace TH.MyApp.DurableFunctionsSample;

public class RunDurableFunctionsSample
{
    // オーケストレータ関数
    [Function(nameof(RunDurableFunctionsSample))]
    public async Task<List<string>> RunOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        // ILogger logger = context.CreateReplaySafeLogger(nameof(RunDurableFunctionsSample));
        string fileName = context.GetInput<string>()!;

        await context.CallActivityAsync<string>(nameof(Activity1), fileName);

        var activity3Condition = await context.CallActivityAsync<Activity3Condition>(nameof(Activity2), fileName);

        var parallelTasks = new List<Task<Activity3Result>>();
        if (activity3Condition.Enabled3_1)
        {
            var param = new Activity3Param
            {
                TaskName = "Activity3-1",
                Count = activity3Condition.Count3_1,
                Status = "success"
            };
            parallelTasks.Add(context.CallActivityAsync<Activity3Result>(nameof(Activity3), param));
        }
        if (activity3Condition.Enabled3_2)
        {
            var param = new Activity3Param
            {
                TaskName = "Activity3-2",
                Count = activity3Condition.Count3_2,
                Status = "success"
            };
            parallelTasks.Add(context.CallActivityAsync<Activity3Result>(nameof(Activity3), param));
        }
        if (activity3Condition.Enabled3_3)
        {
            var param = new Activity3Param
            {
                TaskName = "Activity3-3",
                Count = activity3Condition.Count3_3,
                Status = "success"
            };
            parallelTasks.Add(context.CallActivityAsync<Activity3Result>(nameof(Activity3), param));
        }

        // 3-1〜3-3並列実行
        var results = new List<Activity3Result>();
        if (parallelTasks.Count > 0)
        {
            var completedResults = await Task.WhenAll(parallelTasks);
            results.AddRange(completedResults);
        }

        if (!results.TrueForAll(r => r.Status == "success"))
        {
            // TODO: 例外処理の確認
            throw new Exception("One or more 3-x activities failed.");
        }

        await context.CallActivityAsync<string>(nameof(Activity4), fileName);

        // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
        // TODO: オーケストレーション関数の戻り値

        return [];
    }

    // スターター関数
    [Function("RunDurableFunctionsSample_HttpStart")]
    public static async Task<HttpResponseData> HttpStart(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "start/{fileName}")] HttpRequestData req,
        [DurableClient] DurableTaskClient client,
        string fileName,
        FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger("RunDurableFunctionsSample_HttpStart");

        // Function input comes from the request content.
        string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
            nameof(RunDurableFunctionsSample), fileName);

        logger.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

        // Returns an HTTP 202 response with an instance management payload.
        // See https://learn.microsoft.com/azure/azure-functions/durable/durable-functions-http-api#start-orchestration
        return await client.CreateCheckStatusResponseAsync(req, instanceId);
    }
}
