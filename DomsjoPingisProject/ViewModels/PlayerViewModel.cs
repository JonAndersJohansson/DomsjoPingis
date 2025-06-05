using DataAccessLayer.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace DomsjoPingisProject.ViewModels
{
    public class PlayerViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "För- och efternamn är obligatoriskt")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Namnet måste vara mellan 2 och 50 tecken")]
        [RegularExpression(@"^[^\d]*$", ErrorMessage = "Ditt namn får inte innehålla siffror")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Kön måste anges")]
        public Gender Gender { get; set; } // ENUM!

        [Required(ErrorMessage = "Födelsedatum måste anges")]
        [DataType(DataType.Date)]
        public DateOnly BirthDate { get; set; }
        public int Handicap { get; set; }
        public bool IsActive { get; set; }
    }

}
