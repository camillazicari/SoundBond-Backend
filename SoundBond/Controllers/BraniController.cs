using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoundBond.Data;
using SoundBond.DTOs.Artisti;
using SoundBond.DTOs.Brani;
using SoundBond.Models;
using SoundBond.Services;

namespace SoundBond.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BraniController : ControllerBase
    {
        private readonly BraniService _braniService;
        private readonly SoundBondDbContext _context;

        public BraniController(BraniService braniService, SoundBondDbContext context)
        {
            _braniService = braniService;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BraniDto braniDto)
        {
            try
            {
                var user = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                var email = user!.Value;
                var utente = _context.ApplicationUsers.FirstOrDefault(u => u.Email == email);
                if (utente == null)
                {
                    return BadRequest(new BraniResponseDto { Message = "Utente non trovato." });
                }

                var brano = new Brani()
                {
                    Titolo = braniDto.Titolo,
                    Artista = braniDto.Artista,
                    Img = braniDto.Img,
                    UserId = utente.Id
                };

                var result = await _braniService.Create(brano, email);

                return result ? Ok(new BraniResponseDto { Message = "Brano aggiunto con successo." }) : BadRequest(new BraniResponseDto { Message = "Errore durante l'aggiunta degli artisti." });
            }
            catch
            {
                return BadRequest(new BraniResponseDto { Message = "Qualcosa è andato storto." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetBrani()
        {
            try
            {
                var user = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                var email = user!.Value;
                var utente = _context.ApplicationUsers.FirstOrDefault(u => u.Email == email);
                if (utente == null)
                {
                    return BadRequest(new BraniResponseDto { Message = "Utente non trovato." });
                }

                var brani = await _braniService.GetBrani(email);

                if (brani == null) 
                {
                    return BadRequest(new BraniResponseDto { Message = "Nessun brano trovato." });
                }

                var result = brani.Select(b => new GetBraniDto
                {
                    Id = b.Id,
                    Titolo = b.Titolo,
                    Artista = b.Artista,
                    Img = b.Img,
                }).ToList();

                return Ok(new {message = "Brani trovati!", brani = result});
            }
            catch
            {
                return BadRequest(new BraniResponseDto { Message = "Qualcosa è andato storto." });
            }
        }

        [HttpPut("titolo/{titolo}/artista/{artista}")]
        public async Task<IActionResult> Update(string titolo, string artista, BraniDto braniDto)
        {
            try
            {
                var user = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                var email = user!.Value;
                var utente = _context.ApplicationUsers.FirstOrDefault(u => u.Email == email);
                if (utente == null)
                {
                    return BadRequest(new BraniResponseDto { Message = "Utente non trovato." });
                }

                var vecchioBrano = await _context.Brani.FirstOrDefaultAsync(b => b.Titolo == titolo && b.Artista == artista && b.UserId == utente.Id);

                if (vecchioBrano == null)
                {
                    return BadRequest(new BraniResponseDto { Message = "Brano da modificare non trovato." });
                }

                if (vecchioBrano.Titolo == braniDto.Titolo && vecchioBrano.Artista == braniDto.Artista)
                {
                    return Ok(new BraniResponseDto { Message = "Nessun brano modificato." });
                }

                var result = await _braniService.Update(titolo, artista, braniDto, email);

                return result ? Ok(new BraniResponseDto { Message = "Brano modificato con successo." }) : BadRequest(new BraniResponseDto { Message = "Errore durante la modifica del brano." });
            }
            catch
            {
                return BadRequest(new BraniResponseDto { Message = "Qualcosa è andato storto." });
            }
        }

        [HttpDelete("titolo/{titolo}/artista/{artista}")]
        public async Task<IActionResult> Delete(string titolo, string artista)
        {
            try
            {
                var user = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                var email = user!.Value;
                var utente = _context.ApplicationUsers.FirstOrDefault(u => u.Email == email);
                if (utente == null)
                {
                    return BadRequest(new BraniResponseDto { Message = "Utente non trovato." });
                }

                var result = await _braniService.Delete(titolo, artista, email);

                return result ? Ok(new BraniResponseDto { Message = "Brano eliminato con successo!" }) : BadRequest(new BraniResponseDto { Message = "Errore nell'eliminazione del brano." });
            }
            catch
            {
                return BadRequest(new BraniResponseDto { Message = "Qualcosa è andato storto." });
            }
        }
    }
}
