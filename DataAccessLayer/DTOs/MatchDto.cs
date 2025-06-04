using DataAccessLayer.Data.Models;

namespace DataAccessLayer.DTO_s
{
    public class MatchDto
    {
        public int Id { get; set; }

        public int Player1Id { get; set; }
        public string Player1Name { get; set; }
        public Gender Player1Gender { get; set; }

        public int Player2Id { get; set; }
        public string Player2Name { get; set; }
        public Gender Player2Gender { get; set; }

        public BestOf BestOfSets { get; set; } 

        public DateTime MatchDate { get; set; }

        public int Player1Score { get; set; }
        public int Player2Score { get; set; }
        public bool? IsP1FirstServer { get; set; }

        public bool IsFinished { get; set; }
        public TimeSpan? MatchDuration { get; set; }
        public bool IsActive { get; set; }

        public List<SetDto> Sets { get; set; } = new();
    }
}
