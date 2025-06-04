namespace DataAccessLayer.Data.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Gender Gender { get; set; }
        public DateOnly BirthDate { get; set; }
        public int Handicap { get; set; } = 0; 
        public bool IsActive { get; set; } = true; 

        public ICollection<Match> MatchesAsPlayer1 { get; set; } = new List<Match>();
        public ICollection<Match> MatchesAsPlayer2 { get; set; } = new List<Match>();


    }
    public enum Gender
    {
        Unspecified = 0,
        Male = 1,
        Female = 2,
        Other = 3
    }
}
