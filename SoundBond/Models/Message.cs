namespace SoundBond.Models
{
    public class Message
    {
        public int Id { get; set; }
        public required string SenderId { get; set; }
        public required string ReceiverId { get; set; }
        public required string Content { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public bool Letto { get; set; } = false;
    }
}
