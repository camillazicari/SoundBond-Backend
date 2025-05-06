using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace SoundBond.Models
{
    public class Profilo
    {
        [Key]
        public int Id { get; set; }
        public string Immagine { get; set; } = "https://cdn1.iconfinder.com/data/icons/avatars-55/100/avatar_profile_user_music_headphones_shirt_cool-512.png";
        public string Bio { get; set; } = "Music Lover";
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}
