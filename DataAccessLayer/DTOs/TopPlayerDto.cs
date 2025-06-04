using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DTOs
{
    public class TopPlayerDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TotalSetsWon { get; set; }
        public int MatchesWon { get; set; }
    }

}
