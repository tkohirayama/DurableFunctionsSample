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
        public async Task RunActivity([ActivityTrigger] string fileName, FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("RunDurableFunctionsSample");
            logger.LogInformation($"1 started. fileName: {fileName}");

            var connectionString = _configuration.GetConnectionString("DurableFunctionsSampleDB");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string insertSql = @"
                    INSERT INTO 
                        dfunc.ProcessStartLog (Id, FileName, StartTime)
                    VALUES 
                        (NEXT VALUE FOR dfunc.Seq_ProcessStart, @FileName, @StartTime);";

                using (SqlCommand cmd = new SqlCommand(insertSql, conn))
                {
                    cmd.Parameters.AddWithValue("@StartTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("@FileName", fileName);
                    int rows = await cmd.ExecuteNonQueryAsync();
                }
            }
            return;
        }
    }
}