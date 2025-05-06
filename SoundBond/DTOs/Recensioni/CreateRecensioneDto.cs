using System.ComponentModel.DataAnnotations;

namespace SoundBond.DTOs.Recensioni
{
    public class CreateRecensioneDto
    {
        public required string Testo { get; set; }
        [Range(0.5, 5, ErrorMessage = "Il voto deve essere compreso tra 1 e 5.")]
        public required double Voto { get; set; }
    }
}
