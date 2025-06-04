namespace DataAccessLayer.Data.Models
{
    public class Match
    {
        public int Id { get; set; }
        public int Player1Id { get; set; }
        public Player Player1 { get; set; }
        public int Player2Id { get; set; }
        public Player Player2 { get; set; }
        public BestOf BestOfSets { get; set; } 
        public DateTime MatchDate { get; set; }
        public int Player1Score { get; set; } 
        public int Player2Score { get; set; }
        public bool? IsP1FirstServer { get; set; }
        public bool IsFinished { get; set; }
        public bool IsActive { get; set; } = true; 
        public ICollection<Set> Sets { get; set; } 
    }

    public enum BestOf
    {
        BestOf3 = 3,
        BestOf5 = 5,
        BestOf7 = 7
    }
}