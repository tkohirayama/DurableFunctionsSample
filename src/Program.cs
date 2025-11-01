using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using TH.MyApp.DurableFunctionsSample;
using Microsoft.EntityFrameworkCore;


var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// TODO: ミドルウェア実装
// builder
// .UseMiddleware<ExceptionHandlingMiddleware>();
// .UseMiddleware<MyCustomMiddleware>()
// .UseWhen<StampHttpHeaderMiddleware>((context) =>
// {
//     // We want to use this middleware only for http trigger invocations.
//     return context.FunctionDefinition.InputBindings.Values
//                     .First(a => a.Type.EndsWith("Trigger")).Type == "httpTrigger";
// });

// TODO: 監視の構成
builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Services.AddHttpClient();

builder.Configuration.AddUserSecrets<Program>();
builder.Services
    .AddAzureClients(clientBuilder =>
        {
            var blobConnectionString = builder.Configuration.GetConnectionString("BlobStorage");
            clientBuilder.AddBlobServiceClient(blobConnectionString);
        });

var dbConnectionString = builder.Configuration.GetConnectionString("DurableFunctionsSampleDb");
builder.Services.AddDbContext<DurableFunctionsSampleContext>(options =>
    options.UseSqlServer(dbConnectionString));

var app = builder.Build();
app.Run();
