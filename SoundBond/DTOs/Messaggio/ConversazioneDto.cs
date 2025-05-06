namespace SoundBond.DTOs.Messaggio
{
    public class ConversazioneDto
    {
        public required string ChatWithUserId { get; set; }
        public required string Nome { get; set; }
        public required string Cognome { get; set; }
        public required string Immagine { get; set; }
        public required string UltimoMessaggio { get; set; }
        public DateTime OraUltimoMessaggio { get; set; }
        public bool Letto { get; set; }
        public int MessaggiNonLetti { get; set; } = 0;
    }
}
