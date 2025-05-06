namespace SoundBond.DTOs.Brani
{
    public class GetBraniDto
    {
        public required int Id { get; set; }
        public string? Img { get; set; }
        public required string Titolo { get; set; }
        public required string Artista { get; set; }
    }
}
