using SoundBond.DTOs.Artisti;
using SoundBond.DTOs.Brani;
using SoundBond.DTOs.Generi;
using SoundBond.DTOs.Profilo;

namespace SoundBond.DTOs.Account
{
    public class UtentiDto
    {
        public required string Id { get; set; }
        public required string Nome { get; set; }
        public required string Cognome { get; set; }
        public required string Email { get; set; }
        public DateOnly DataDiNascita { get; set; }
        public required string NomeUtente { get; set; }
        public ProfiloDto? Profilo { get; set; }
        public List<GetGeneriDto>? Generi { get; set; }
        public List<GetArtistiDto>? Artisti { get; set; }
        public List <GetBraniDto>? Brani { get; set; }
    }
}
