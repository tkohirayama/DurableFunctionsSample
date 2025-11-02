namespace TH.MyApp.DurableFunctionsSampleTests.UnitTests
{
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using TH.MyApp.DurableFunctionsSample;
    using TH.MyApp.DurableFunctionsSample.Activities;
    using TH.MyApp.DurableFunctionsSample.Domain;

    public class Activity1Test
    {
        private IServiceCollection _services;

        [SetUp]
        public void Setup()
        {
            _services = new ServiceCollection();
            _services.AddSingleton(
                b => LoggerFactory.Create(builder => builder.AddConsole()));
        }

        [Test]
        public async Task Test1()
        {
            // 関数初期化
            var processStartLogRepository = Substitute.For<IProcessStartLogRepository>();
            processStartLogRepository.Add(Arg.Any<ProcessStartLog>()).Returns(Task.FromResult(1));
            var func = new Activity1(processStartLogRepository);

            // 関数メソッド引数準備
            var contextMock = Substitute.For<FunctionContext>();
            var provider = _services.BuildServiceProvider();
            contextMock.InstanceServices.Returns(provider);

            // 関数実行
            await func.RunActivity("test-file.txt", contextMock);
        }
    }
}