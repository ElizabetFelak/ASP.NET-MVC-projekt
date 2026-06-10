using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PokemonCollector.Web.Models
{
    public class AppUser : IdentityUser
    {
        [Required]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "OIB must be exactly 11 digits")]
        [RegularExpression("^[0-9]{11}$", ErrorMessage = "OIB must contain only digits")]
        [Display(Name = "OIB (Personal Identification Number)")]
        public string? OIB { get; set; }

        [Required]
        [StringLength(13, MinimumLength = 13, ErrorMessage = "JMBG must be exactly 13 digits")]
        [RegularExpression("^[0-9]{13}$", ErrorMessage = "JMBG must contain only digits")]
        [Display(Name = "JMBG (Unique Master Citizen Number)")]
        public string? JMBG { get; set; }
    }
}
