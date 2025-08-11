using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace TH.MyApp.DurableFunctionsSample;

public static class RunDurableFunctionsSample
{
    [Function(nameof(RunDurableFunctionsSample))]
    public static async Task<List<string>> RunOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        ILogger logger = context.CreateReplaySafeLogger(nameof(RunDurableFunctionsSample));
        // logger.LogInformation("Saying hello.");

        // TODO:アクティビティ1

        // TODO:アクティビティ2

        // TODO:アクティビティ3
        // // TODO:アクティビティ3-1
        // // TODO:アクティビティ3-2
        // // TODO:アクティビティ3-3(Direct Input)

        // TODO:アクティビティ4

        var outputs = new List<string>
        {
            // // Replace name and input with values relevant for your Durable Functions Activity
            // await context.CallActivityAsync<string>(nameof(SayHello), "Tokyo"),
            // await context.CallActivityAsync<string>(nameof(SayHello), "Seattle"),
            // await context.CallActivityAsync<string>(nameof(SayHello), "London")
        };

        // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
        return outputs;
    }

    [Function(nameof(SayHello))]
    public static string SayHello([ActivityTrigger] string name, FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger("SayHello");
        logger.LogInformation("Saying hello to {name}.", name);
        return $"Hello {name}!";
    }

    [Function("RunDurableFunctionsSample_HttpStart")]
    public static async Task<HttpResponseData> HttpStart(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
        [DurableClient] DurableTaskClient client,
        FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger("RunDurableFunctionsSample_HttpStart");

        // Function input comes from the request content.
        string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
            nameof(RunDurableFunctionsSample));

        logger.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

        // Returns an HTTP 202 response with an instance management payload.
        // See https://learn.microsoft.com/azure/azure-functions/durable/durable-functions-http-api#start-orchestration
        return await client.CreateCheckStatusResponseAsync(req, instanceId);
    }
}