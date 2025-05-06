using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoundBond.Models
{
    public class Bonder
    {
        [Key]
        public int Id { get; set; }
        public DateTime ConnectionDate { get; set; }
        public string? UserId1 { get; set; }
        public string? UserId2 { get; set; }
        [ForeignKey("UserId1")]
        public ApplicationUser User1 { get; set; }

        [ForeignKey("UserId2")]
        public ApplicationUser User2 { get; set; }
    }
}
