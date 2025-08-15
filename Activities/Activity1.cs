namespace TH.MyApp.DurableFunctionsSample.Activities
{
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    public class Activity1
    {
        private readonly IConfiguration _configuration;

        public Activity1(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Function("Activity1")]
        public async Task RunActivity([ActivityTrigger] string input, FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("RunDurableFunctionsSample");
            logger.LogInformation($"1 started {input}");

            var connectionString = _configuration.GetConnectionString("DurableFunctionsSampleDB");

            // TODO: EF Core
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"
                    INSERT INTO 
                        dfunc.ProcessStartLog (Id, StartTime)
                    VALUES 
                        (NEXT VALUE FOR dfunc.Seq_ProcessStart, @StartTime);";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@StartTime", DateTime.Now);

                    int rows = await cmd.ExecuteNonQueryAsync();
                }
            }

            return;
        }
    }
}