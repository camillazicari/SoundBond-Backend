using SoundBond.DTOs.Account;

namespace SoundBond.DTOs.Richiesta
{
    public class RichiestaInviataDto
    {
        public int Id { get; set; }

        public DateTime RequestDate { get; set; } = DateTime.UtcNow;

        public required BonderRichiestaUtenteDto Sender { get; set; }

        public required UtentiDto Receiver { get; set; }
    }
}
