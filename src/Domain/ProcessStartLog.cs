namespace TH.MyApp.DurableFunctionsSample.Domain
{
    public class ProcessStartLog
    {
        public int Id { get; set; }
        public string FileName { get; set; } = null!;
        public DateTime StartTime { get; set; }
    }
}