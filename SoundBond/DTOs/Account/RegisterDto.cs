using System.ComponentModel.DataAnnotations;

namespace SoundBond.DTOs.Account
{
    public class RegisterDto
    {
        public required string Nome { get; set; }

        public required string Cognome { get; set; }

        public required string Email { get; set; }

        public DateOnly DataDiNascita { get; set; }

        public required string NomeUtente { get; set; }

        public required string Password { get; set; }
    }
}
