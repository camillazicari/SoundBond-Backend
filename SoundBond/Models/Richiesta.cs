using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SoundBond.Models
{
    public class Richiesta
    {
        [Key]
        public int Id { get; set; }
        
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;

        [Required]
        public string? SenderId { get; set; }

        [Required]
        public string? ReceiverId { get; set; }

        [ForeignKey("SenderId")]
        public ApplicationUser Sender { get; set; } = null!;

        [ForeignKey("ReceiverId")]
        public ApplicationUser Receiver { get; set; } = null!;
    }
}
