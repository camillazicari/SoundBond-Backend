using SoundBond.DTOs.Account;

namespace SoundBond.DTOs.Bonder
{
    public class BonderDto
    {
        public int Id { get; set; }

        public DateTime ConnectionDate { get; set; } = DateTime.UtcNow;
        public required UtentiBondersDto OtherUser { get; set; }
    }
}
