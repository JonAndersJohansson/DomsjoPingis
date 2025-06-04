namespace DomsjoPingisProject.ViewModels
{
    public class PlayerStatisticsViewModel
    {
        public List<PlayerViewModel> AllPlayers { get; set; }
        public string SelectedPlayerName { get; set; }
        public int TotalWins { get; set; }
        public int TotalLosses { get; set; }
        public int WinRation { get; set; }
        public int TotalMatches { get; set; }
        public string PlayerAName { get; set; }
        public string PlayerBName { get; set; }
        public int PlayerAWins { get; set; }
        public int PlayerBWins { get; set; }
        public string BestPerformance { get; set; }
        public string WorstPerformance { get; set; }
        public TimeSpan LongestMatchDuration { get; set; }
        public string LongestMatchSummary { get; set; }
        public TimeSpan ShortestMatchDuration { get; set; }
        public string ShortestMatchSummary { get; set; }
        public TimeSpan AverageMatchDuration { get; set; }
        public string AverageMatchSummary { get; set; }
    }
}
