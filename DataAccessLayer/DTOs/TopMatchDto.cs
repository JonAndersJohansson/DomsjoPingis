using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DTOs
{
    public class TopMatchDto
    {
        public int MatchId { get; set; }
        public string Player1Name { get; set; }
        public string Player2Name { get; set; }
        public TimeSpan TotalDuration { get; set; }
        public DateTime MatchDate { get; set; }
    }

}
