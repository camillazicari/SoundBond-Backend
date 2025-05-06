using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoundBond.Data;
using SoundBond.DTOs.Artisti;
using SoundBond.Models;
using SoundBond.Services;

namespace SoundBond.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ArtistiController : ControllerBase
    {
        private readonly ArtistiService _artistiService;
        private readonly SoundBondDbContext _context;

        public ArtistiController(ArtistiService artistiService, SoundBondDbContext context)
        {
            _artistiService = artistiService;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddArtisti([FromBody] ArtistiDto artisti)
        {
            try
            {
                var user = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                var email = user!.Value;
                var utente = _context.ApplicationUsers.FirstOrDefault(u => u.Email == email);
                if (utente == null)
                {
                    return BadRequest(new { message = "Utente non trovato." });
                }

                var artista = new Artisti()
                {
                    Nome = artisti.Nome,
                    Img = artisti.Img,
                    UserId = utente.Id
                };

                var result = await _artistiService.Add(artista, email);

                return result ? Ok(new ArtistiResponseDto { Message = "Artista aggiunto con successo." }) : BadRequest(new ArtistiResponseDto { Message = "Errore durante l'aggiunta degli artisti." });
            }
            catch
            {
                return BadRequest(new ArtistiResponseDto { Message = "Qualcosa è andato storto." });
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetArtisti()
        {
            try
            {
                var user = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                var email = user!.Value;
                var utente = _context.ApplicationUsers.FirstOrDefault(u => u.Email == email);

                if (utente == null)
                {
                    return BadRequest(new ArtistiResponseDto { Message = "Utente non trovato." });
                }
                var artisti = await _artistiService.GetArtisti(email);
                if (artisti == null)
                {
                    return BadRequest(new ArtistiResponseDto { Message = "Nessun artista trovato." });
                }

                var listaArtisti = artisti.Select(a => new GetArtistiDto
                {
                    Id = a.Id,
                    Img = a.Img,
                    Nome = a.Nome,
                }).ToList();

                return Ok(new { message = "Artisti trovati!", artisti = listaArtisti });
            }
            catch
            {
                return BadRequest(new ArtistiResponseDto { Message = "Qualcosa è andato storto." });
            }
        }

        [HttpPut("artista")]
        public async Task<IActionResult> Update([FromQuery] string nome,[FromBody] ArtistiDto artistiDto)
        {
            try
            {
                var user = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                var email = user!.Value;
                var utente = _context.ApplicationUsers.FirstOrDefault(u => u.Email == email);
                if (utente == null)
                {
                    return BadRequest(new ArtistiResponseDto { Message = "Utente non trovato." });
                }

                var vecchioArtista = await _context.Artisti.FirstOrDefaultAsync(a => a.Nome == nome && a.UserId == utente.Id);

                if (vecchioArtista == null)
                {
                    return BadRequest(new ArtistiResponseDto { Message = "Artista da modificare non trovato." });
                }

                if(vecchioArtista.Nome == artistiDto.Nome)
                {
                    return Ok(new ArtistiResponseDto { Message = "Nessun artista modificato." });
                }

                var result = await _artistiService.Update(nome, email, artistiDto);

                return result ? Ok(new ArtistiResponseDto { Message = "Artista modificato con successo." }) : BadRequest(new ArtistiResponseDto { Message = "Errore durante la modifica dell'artista." });

            }
            catch
            {
                return BadRequest(new ArtistiResponseDto { Message = "Qualcosa è andato storto." });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] string nome)
        {
            try
            {
                var user = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                var email = user!.Value;
                var utente = _context.ApplicationUsers.FirstOrDefault(u => u.Email == email);
                if (utente == null)
                {
                    return BadRequest(new ArtistiResponseDto { Message = "Utente non trovato." });
                }
                var result = await _artistiService.Delete(nome, email);
                return result ? Ok(new ArtistiResponseDto { Message = "Artista eliminato con successo." }) : BadRequest(new ArtistiResponseDto { Message = "Errore durante l'eliminazione dell'artista." });
            }
            catch
            {
                return BadRequest(new ArtistiResponseDto { Message = "Qualcosa è andato storto." });
            }
        }
    }
}