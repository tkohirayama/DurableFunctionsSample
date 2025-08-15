namespace TH.MyApp.DurableFunctionsSample.Activities
{
    using System.Reflection.Metadata;
    using Azure.Storage.Blobs;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Extensions.Logging;

    public class Activity4
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName = "dfunc";

        public Activity4(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        [Function("Activity4")]
        public async Task RunActivity([ActivityTrigger] string fileName, FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("RunDurableFunctionsSample");
            logger.LogInformation($"4 started {_blobServiceClient.AccountName}");

            // Blobファイルダウンロード(一時領域へのダウンロード)
            // logger.LogInformation($"4 download {fileName}");

            // Blobファイルアップロード（上書き）
            logger.LogInformation($"4 upload {fileName}");
            var container = _blobServiceClient.GetBlobContainerClient(_containerName);
            await container.CreateIfNotExistsAsync();

            var blob = container.GetBlobClient($"{fileName}.txt");
            await blob.UploadAsync(BinaryData.FromString($"{fileName} Contents"), true);
            return;
        }
    }
}