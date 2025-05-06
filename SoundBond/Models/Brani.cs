using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoundBond.Models
{
    public class Brani
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Titolo { get; set; }

        [Required]
        public required string Artista { get; set; }

        public string? Img { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}
