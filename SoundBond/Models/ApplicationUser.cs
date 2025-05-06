using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace SoundBond.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public required string Nome { get; set; }

        [Required]
        public required string Cognome { get; set; }

        [Required]
        public required string NomeUtente { get; set; }

        [Required]
        public DateOnly DataNascita { get; set; }

        public ICollection<ApplicationUserRole>? UserRoles { get; set; }

        public ICollection<Generi>? Generi { get; set; }

        public ICollection<Artisti>? Artisti { get; set; }
        public ICollection<Brani>? Brani { get; set; }
        public Profilo? Profilo { get; set; }
        public ICollection<Richiesta>? RichiesteInviate { get; set; }
        public ICollection<Richiesta>? RichiesteRicevute { get; set; }
        public Recensione? Recensione { get; set; }
    }
}
