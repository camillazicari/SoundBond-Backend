using System.ComponentModel.DataAnnotations;

namespace SoundBond.DTOs.Brani
{
    public class BraniDto
    {
        public string? Img { get; set; }
        public required string Titolo { get; set; }
        public required string Artista { get; set; }
    }
}
