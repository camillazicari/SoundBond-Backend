using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SoundBond.Data;
using SoundBond.Services;
using SoundBond.DTOs.Recensioni;
using SoundBond.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SoundBond.DTOs.Artisti;

namespace SoundBond.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RecensioniController : ControllerBase
    {
        private readonly RecensioniService _recensioniService;
        private readonly SoundBondDbContext _context;

        public RecensioniController(SoundBondDbContext soundBondDbContext, RecensioniService recensioniService)
        {
            _context = soundBondDbContext;
            _recensioniService = recensioniService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRecensioneDto createRecensioneDto)
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

                var recensione = new Recensione()
                {
                    Testo = createRecensioneDto.Testo,
                    Voto = createRecensioneDto.Voto,
                    ApplicationUserId = utente.Id,
                };

                var result = await _recensioniService.Create(recensione, email);

                return result ? Ok(new RecensioneResponseDto { Message = "Recensione aggiunta con successo." }) : BadRequest(new RecensioneResponseDto { Message = "Errore durante l'aggiunta della recensione." });
            }
            catch
            {
                return BadRequest(new RecensioneResponseDto { Message = "Qualcosa è andato storto." });
            }
        }

        [HttpGet("recensioni")]
        public async Task<IActionResult> GetAllRecensioni()
        {
            try
            {
                var recensioni = await _recensioniService.GetRecensioni();

                if(recensioni == null || recensioni.Count == 0)
                {
                    return Ok(new { message = "Nessuna recensione trovata." });
                }

                var recensioniList = recensioni.Select(r => new RecensioneDto
                {
                    Id = r.Id,
                    Testo = r.Testo,
                    Voto = r.Voto,
                    Data = r.Data,
                    IdUser = r.ApplicationUser.Id,
                    NomeUser = r.ApplicationUser.Nome,
                    CognomeUser = r.ApplicationUser.Cognome,
                    NomeUtenteUser = r.ApplicationUser.NomeUtente,
                    ImgUser = r.ApplicationUser.Profilo.Immagine,

                }).ToList();

                return Ok(new { message = "Recensioni trovate!", recensioni = recensioniList });
            }
            catch
            {
                return BadRequest(new RecensioneResponseDto { Message = "Qualcosa è andato storto." });
            }
        }

        [HttpGet("recensione")]
        public async Task<IActionResult> GetPropriaRecensione()
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

                var recensione = await _recensioniService.GetPropriaRecensione(email);

                if(recensione == null)
                {
                    return Ok(new { message = "Recensione non trovata." });
                }

                var propriaRecensione = new RecensioneDto
                {
                    Id = recensione.Id,
                    Testo = recensione.Testo,
                    Voto = recensione.Voto,
                    Data = recensione.Data,
                    IdUser = recensione.ApplicationUser.Id,
                    NomeUser = recensione.ApplicationUser.Nome,
                    CognomeUser = recensione.ApplicationUser.Cognome,
                    NomeUtenteUser = recensione.ApplicationUser.NomeUtente,
                    ImgUser = recensione.ApplicationUser.Profilo.Immagine,
                };

                return Ok(new { message = "Recensione trovata!", recensione = propriaRecensione });
            }
            catch
            {
                return BadRequest(new RecensioneResponseDto { Message = "Qualcosa è andato storto." });
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CreateRecensioneDto createRecensioneDto)
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

                var vecchiaRecensione = await _context.Recensioni.FirstOrDefaultAsync(r => r.ApplicationUserId == utente.Id);

                if (vecchiaRecensione == null)
                {
                    return BadRequest(new ArtistiResponseDto { Message = "Artista da modificare non trovato." });
                }

                if (vecchiaRecensione.Testo == createRecensioneDto.Testo && vecchiaRecensione.Voto == createRecensioneDto.Voto)
                {
                    return Ok(new RecensioneResponseDto { Message = "Nessuna recensione modificata." });
                }

                var result = await _recensioniService.Update(createRecensioneDto, email);

                return result ? Ok(new ArtistiResponseDto { Message = "Artista modificato con successo." }) : BadRequest(new ArtistiResponseDto { Message = "Errore durante la modifica dell'artista." });
            }
            catch
            {
                return BadRequest(new RecensioneResponseDto { Message = "Qualcosa è andato storto." });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete()
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

                var result = await _recensioniService.Delete(email);
                return result ? Ok(new RecensioneResponseDto { Message = "Recensione eliminata con successo." }) : BadRequest(new RecensioneResponseDto { Message = "Errore durante l'eliminazione della recensione." });
            }
            catch
            {
                return BadRequest(new RecensioneResponseDto { Message = "Qualcosa è andato storto." });
            }
        }
    }
}
