namespace TH.MyApp.DurableFunctionsSample.Activities
{
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Extensions.Logging;

    public class Activity4
    {
        [Function("Activity4")]
        public static Task RunActivity([ActivityTrigger] string fileName, FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("RunDurableFunctionsSample");
            logger.LogInformation($"4 started");

            // Blobファイルダウンロード(一時領域へのダウンロード)
            logger.LogInformation($"4 download {fileName}");

            // Blobファイルアップロード（Blobコンテナへのアップロード）
            logger.LogInformation($"4 upload {fileName}");

            return Task.CompletedTask;
        }
    }
}