using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoundBond.Models
{
    public class Recensione
    {
        [Key]
        public int Id { get; set; }
        public required string Testo { get; set; }
        [Range(0.5, 5, ErrorMessage = "Il voto deve essere compreso tra 1 e 5.")]
        public required double Voto { get; set; }
        public DateTime Data { get; set; } = DateTime.Now;
        public required string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser? ApplicationUser { get; set; }
    }
}
