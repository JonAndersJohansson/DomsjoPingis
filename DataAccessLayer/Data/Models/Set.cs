namespace DataAccessLayer.Data.Models
{
    public class Set
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public Match Match { get; set; }
        public int SetNumber { get; set; } 
        public int Player1Points { get; set; }
        public int Player2Points { get; set; }
        public DateTime SetStartTime { get; set; }
        public TimeSpan? SetDuration { get; set; }
        public bool IsFinished { get; set; }
    }
}
