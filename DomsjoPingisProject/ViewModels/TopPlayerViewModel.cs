namespace DomsjoPingisProject.ViewModels
{
    public class TopPlayerViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TotalSetsWon { get; set; }
        public int MatchesWon { get; set; }
        public int MatchesPlayed { get; set; }
        public int TopByWinRatio { get; set; }
        public string FirstName => Name?.Split(' ')[0] ?? "";
        public string LastName => Name?.Split(' ').Length > 1 ? Name.Split(' ')[1] : "";
    }
}
