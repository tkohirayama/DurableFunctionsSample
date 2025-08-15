namespace TH.MyApp.DurableFunctionsSample.Activities
{
    using System.Reflection.Metadata;
    using Azure.Storage.Blobs;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Extensions.Logging;

    public class Activity4
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _containerName = "dfunc";

        public Activity4(BlobServiceClient blobServiceClient, IHttpClientFactory httpClientFactory)
        {
            _blobServiceClient = blobServiceClient;
            _httpClientFactory = httpClientFactory;
        }

        [Function("Activity4")]
        public async Task RunActivity([ActivityTrigger] string fileName, FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("RunDurableFunctionsSample");
            logger.LogInformation($"4 started {_blobServiceClient.AccountName}");

            // https://example.com へのアクセス
            var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.SendAsync(request);

            // Blobファイルアップロード（上書き）
            logger.LogInformation($"4 upload {fileName}");
            var container = _blobServiceClient.GetBlobContainerClient(_containerName);
            await container.CreateIfNotExistsAsync();

            var blob = container.GetBlobClient($"{fileName}.txt");
            var content = await response.Content.ReadAsStreamAsync();
            await blob.UploadAsync(content, true);
            return;
        }
    }
}