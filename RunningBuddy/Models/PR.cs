namespace RunningBuddy.Models 
{
    public class PR
    {
        public int Id { get; set; }
        public double Distance { get; set; }
        public TimeSpan BestTime { get; set; }
        public DateTime DateRan { get; set; }
    }
}