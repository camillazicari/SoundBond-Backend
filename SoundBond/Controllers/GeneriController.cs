using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoundBond.Data;
using SoundBond.DTOs.Generi;
using SoundBond.Models;
using SoundBond.Services;

namespace SoundBond.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GeneriController : ControllerBase
    {
        private readonly GeneriService _generiService;
        private readonly SoundBondDbContext _context;

        public GeneriController(GeneriService generiService, SoundBondDbContext context)
        {
            _generiService = generiService;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GeneriDto generiDto)
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

                var genere = new Generi()
                {
                    Nome = generiDto.Nome,
                    UserId = utente.Id
                };

                var result = await _generiService.Create(genere, email);

                return result ? Ok(new GeneriResponseDto { Message = "Genere aggiunto con successo." }) : BadRequest(new GeneriResponseDto { Message = "Errore nell'aggiunta del genere." });
            }
            catch
            {
                return BadRequest(new GeneriResponseDto { Message = "Qualcosa è andato storto." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetGeneri()
        {
            try
            {
                var user = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                var email = user!.Value;
                var utente = _context.ApplicationUsers.FirstOrDefault(u => u.Email == email);
                if (utente == null)
                {
                    return BadRequest(new GeneriResponseDto { Message = "Utente non trovato." });
                }

                var result = await _generiService.GetGeneri(email);
                if (result == null)
                {
                    return BadRequest(new GeneriResponseDto { Message = "Nessun genere trovato" });
                }

                var listaGeneri = result.Select(s => new GetGeneriDto
                {
                    Id = s.Id,
                    Nome = s.Nome,
                }).ToList();

                return Ok(new { message = "Generi trovati.", generi = listaGeneri });

            }
            catch
            {
                return BadRequest(new GeneriResponseDto { Message = "Qualcosa è andato storto." });
            }
        }

        [HttpPut("genere")]
        public async Task<IActionResult> Update([FromQuery] string nome, [FromBody] GeneriDto generiDto)
        {
            try
            {
                var user = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                var email = user!.Value;
                var utente = _context.ApplicationUsers.FirstOrDefault(u => u.Email == email);

                if (utente == null)
                {
                    return BadRequest(new GeneriResponseDto { Message = "Utente non trovato." });
                }

                var genereEsistente = await _context.Generi.FirstOrDefaultAsync(g => g.Nome == nome && g.UserId == utente.Id);

                if (genereEsistente == null)
                {
                    return BadRequest(new GeneriResponseDto { Message = "Genere da modificare non trovato." });
                }

                if (genereEsistente.Nome == generiDto.Nome) 
                {
                    return Ok(new GeneriResponseDto { Message = "Nessuna modifica apportata." });
                }

                var result = await _generiService.Update(nome, generiDto, email);

                return result ? Ok(new GeneriResponseDto { Message = "Genere aggiornato con successo!" }) : BadRequest(new GeneriResponseDto { Message = "Errore nella modifica del genere." });
            }
            catch
            {
                return BadRequest(new { message = "Qualcosa è andato storto." });
            }
        }

        [HttpDelete("genere")]
        public async Task<IActionResult> Delete([FromQuery] string nome)
        {
            try
            {
                var user = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                var email = user!.Value;
                var utente = _context.ApplicationUsers.FirstOrDefault(u => u.Email == email);
                if (utente == null)
                {
                    return BadRequest(new GeneriResponseDto { Message = "Utente non trovato." });
                }
                var result = await _generiService.Delete(nome, email);

                return result ? Ok(new GeneriResponseDto { Message = "Genere eliminato con successo!" }) : BadRequest(new { message = "Errore nell'eliminazione del genere." });
            }
            catch
            {
                return BadRequest(new GeneriResponseDto { Message = "Qualcosa è andato storto." });
            }
        }
    }
}