namespace RohanPingisProject.ViewModels
{
    public class TopMatchViewModel
    {
        public int MatchId { get; set; }
        public string Player1Name { get; set; }
        public string Player2Name { get; set; }
        public TimeSpan TotalDuration { get; set; }
        public DateTime MatchDate { get; set; }

        public string MatchDateFormatted => MatchDate.ToShortDateString();

        public string FormattedDuration =>
            (TotalDuration.TotalHours >= 1
                ? $"{(int)TotalDuration.TotalHours}h {TotalDuration.Minutes}m"
                : $"{TotalDuration.Minutes}min {TotalDuration.Seconds}s");
    }
}
