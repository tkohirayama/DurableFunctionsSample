namespace TH.MyApp.DurableFunctionsSample.Activities
{
    public class Activity3Result
    {
        public required string Status { get; init; }
        public Exception? e { get; init; }
    }
}