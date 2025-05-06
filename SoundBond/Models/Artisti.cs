using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoundBond.Models
{
    public class Artisti
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Nome { get; set; }
        public string? Img { get; set; }
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}
