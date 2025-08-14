using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;


var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Services
    .AddAzureClients(clientBuilder =>
        {
            var blobConnectionString = builder.Configuration.GetConnectionString("BlobStorage");
            clientBuilder.AddBlobServiceClient(blobConnectionString);
        });

var app = builder.Build();
app.Run();
