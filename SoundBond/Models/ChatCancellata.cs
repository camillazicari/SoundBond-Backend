namespace SoundBond.Models
{
    public class ChatCancellata
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public required string OtherUserId { get; set; }
        public DateTime DeletedAt { get; set; }
    }
}
