using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoundBond.Data;
using SoundBond.DTOs.Account;
using SoundBond.DTOs.Artisti;
using SoundBond.DTOs.Bonder;
using SoundBond.DTOs.Brani;
using SoundBond.DTOs.Generi;
using SoundBond.DTOs.Profilo;
using SoundBond.Models;
using SoundBond.Services;

namespace SoundBond.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BondersController : ControllerBase
    {
        private readonly SoundBondDbContext _context;
        private readonly BondersService _bondersService;

        public BondersController(SoundBondDbContext context, BondersService bondersService)
        {
            _context = context;
            _bondersService = bondersService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBonderDto createBonderDto)
        {
            try
            {
                var user1Email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var user1 = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Email == user1Email);

                if (user1 == null)
                    return BadRequest("Utente1 non trovato.");

                var user2 = await _context.ApplicationUsers
                    .FirstOrDefaultAsync(u => u.Id == createBonderDto.UserId2);

                if (user2 == null)
                    return BadRequest("Utente destinatario non trovato.");

                var bonding = new Bonder()
                {
                    UserId1 = user1.Id,
                    UserId2 = createBonderDto.UserId2,
                    ConnectionDate = DateTime.UtcNow
                };

                var result = await _bondersService.Create(bonding);

                return result ?
                    Ok(new BonderResponseDto { Message = "Bonder aggiunto con successo." }) :
                    BadRequest(new BonderResponseDto { Message = "Errore durante l'aggiunta del bonder o bonder già esistente." });
            }
            catch
            {
                return BadRequest(new BonderResponseDto { Message = "Qualcosa è andato storto." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetBonders()
        {
            try
            {
                var email = User.FindFirstValue(ClaimTypes.Email);
                if (string.IsNullOrEmpty(email))
                    return BadRequest(new { message = "Email non valida." });

                var bonders = await _bondersService.GetBonders(email);

                return Ok(new
                {
                    message = bonders.Any() ? "Bonders trovati" : "Nessun bonder trovato",
                    bonders
                });
            }
            catch
            {
                return BadRequest(new BonderResponseDto { Message = "Qualcosa è andato storto." });
            }
        }


        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] string id)
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

                var result = await _bondersService.Delete(id, email);

                return result ? Ok(new BonderResponseDto { Message = "Bonder eliminato con successo." }) : BadRequest(new BonderResponseDto { Message = "Errore nell'eliminazione del bonder." });
            }
            catch
            {
                return BadRequest(new BonderResponseDto { Message = "Qualcosa è andato storto." });
            }
        }
    }
}
