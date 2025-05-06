namespace SoundBond.DTOs.Artisti
{
    public class GetArtistiDto
    {
        public int Id { get; set; }
        public string? Img { get; set; }
        public required string Nome { get; set; }

    }
}
