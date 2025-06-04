using DataAccessLayer.Data.Models;

namespace DataAccessLayer.DTO_s
{
    public class PlayerDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Gender Gender { get; set; } 
        public DateOnly BirthDate { get; set; }
        public int Handicap { get; set; }
        public bool IsActive { get; set; }
    }


}
