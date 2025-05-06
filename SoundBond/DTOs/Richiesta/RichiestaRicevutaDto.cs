using SoundBond.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SoundBond.DTOs.Account;

namespace SoundBond.DTOs.Richiesta
{
    public class RichiestaRicevutaDto
    {
        public int Id { get; set; }

        public DateTime RequestDate { get; set; } = DateTime.UtcNow;

        public required UtentiDto Sender { get; set; } 

        public required BonderRichiestaUtenteDto Receiver { get; set; }
    }
}
