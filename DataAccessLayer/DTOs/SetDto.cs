namespace DataAccessLayer.DTO_s
{
    public class SetDto
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public int SetNumber { get; set; }
        public int Player1Points { get; set; }
        public int Player2Points { get; set; }
        public DateTime SetStartTime { get; set; }
        public TimeSpan? SetDuration { get; set; }
        public bool IsFinished { get; set; }

        public bool IsDeuce { get; set; }
        public bool IsP1Serving { get; set; }
        public bool IsSetBallPlayer1 { get; set; }
        public bool IsSetBallPlayer2 { get; set; }
        public bool IsMatchBallPlayer1 { get; set; }
        public bool IsMatchBallPlayer2 { get; set; }
    }

}
