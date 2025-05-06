using SoundBond.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SoundBond.DTOs.Recensioni
{
    public class RecensioneDto
    {
        public int Id { get; set; }
        public required string Testo { get; set; }
        [Range(0.5, 5, ErrorMessage = "Il voto deve essere compreso tra 1 e 5.")]
        public required double Voto { get; set; }
        public DateTime Data { get; set; } = DateTime.Now;
        public required string IdUser { get; set; }
        public required string NomeUser { get; set; }
        public required string CognomeUser { get; set; }
        public required string NomeUtenteUser { get; set; }
        public required string ImgUser { get; set; }
    }
}
