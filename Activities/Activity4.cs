namespace TH.MyApp.DurableFunctionsSample.Activities
{
    using System.Reflection.Metadata;
    using Azure.Storage.Blobs;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Extensions.Logging;

    public class Activity4
    {
        private readonly BlobServiceClient _blobServiceClient;

        public Activity4(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        [Function("Activity4")]
        public Task RunActivity([ActivityTrigger] string fileName, FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("RunDurableFunctionsSample");
            logger.LogInformation($"4 started {_blobServiceClient.AccountName}");

            // Blobファイルダウンロード(一時領域へのダウンロード)
            logger.LogInformation($"4 download {fileName}");

            // Blobファイルアップロード（Blobコンテナへのアップロード）
            logger.LogInformation($"4 upload {fileName}");

            return Task.CompletedTask;
        }
    }
}