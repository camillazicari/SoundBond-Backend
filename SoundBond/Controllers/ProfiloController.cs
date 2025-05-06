using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SoundBond.Data;
using SoundBond.DTOs.Artisti;
using SoundBond.DTOs.Profilo;
using SoundBond.Models;
using SoundBond.Services;

namespace SoundBond.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfiloController : ControllerBase
    {
        private readonly ProfiloService _profiloService;
        private readonly SoundBondDbContext _context;
        public ProfiloController(SoundBondDbContext soundBondDbContext, ProfiloService profiloService)
        {
            _context = soundBondDbContext;
            _profiloService = profiloService;
        }

        [HttpPost]
        public async Task<IActionResult> Create()
        {
            var user = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            var email = user!.Value;
            var utente = _context.ApplicationUsers.FirstOrDefault(u => u.Email == email);

            if (utente == null)
            {
                return BadRequest(new ProfiloResponseDto { Message = "Utente non trovato." });
            }

            var profilo = new Profilo
            {
                UserId = utente.Id,
                Immagine = "https://cdn1.iconfinder.com/data/icons/avatars-55/100/avatar_profile_user_music_headphones_shirt_cool-512.png",
                Bio = "Music lover"
            };

            var result = await _profiloService.Create(profilo, email);

            return result ? Ok(new ProfiloResponseDto { Message = "Profilo creato con successo." }) : BadRequest(new ProfiloResponseDto { Message = "Errore durante la creazione del profilo." });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromForm] ProfiloDto profiloDto)
        {
            try
            {
                if (ModelState.ContainsKey("ImgFile") && ModelState["ImgFile"].Errors.Count > 0)
                {
                    if (!string.IsNullOrEmpty(profiloDto.Immagine))
                    {
                        ModelState.Remove("ImgFile");
                    }
                }

                if (!ModelState.IsValid)
                {
                    return ValidationProblem(ModelState);
                }

                var user = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                if (user == null)
                {
                    return Unauthorized(new ProfiloResponseDto { Message = "Utente non autenticato." });
                }

                var email = user.Value;
                var utente = _context.ApplicationUsers.FirstOrDefault(u => u.Email == email);
                if (utente == null)
                {
                    return BadRequest(new ProfiloResponseDto { Message = "Utente non trovato." });
                }

                var profiloEsistente = _context.Profili.FirstOrDefault(p => p.UserId == utente.Id);
                if (profiloEsistente == null)
                {
                    return BadRequest(new ProfiloResponseDto { Message = "Profilo non trovato." });
                }

                if (profiloEsistente.Bio == profiloDto.Bio &&
                    profiloEsistente.Immagine == profiloDto.Immagine &&
                    profiloDto.ImgFile == null)
                {
                    return Ok(new ProfiloResponseDto { Message = "Nessun dato del profilo modificato." });
                }

                var result = await _profiloService.UpdateProfilo(email, profiloDto);
                return result
                    ? Ok(new ProfiloResponseDto { Message = "Profilo modificato con successo." })
                    : BadRequest(new ProfiloResponseDto { Message = "Errore durante la modifica del profilo." });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errore durante l'aggiornamento del profilo");
                return BadRequest(new ProfiloResponseDto { Message = $"Qualcosa è andato storto: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetProfilo()
        {
            try
            {
                var user = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                var email = user!.Value;
                var profilo = await _profiloService.GetProfilo(email);

                if(profilo == null)
                {
                    return BadRequest(new ProfiloResponseDto { Message = "nessun utente autenticato." });
                }

                var profiloDto = new ProfiloDto()
                {
                    Bio = profilo.Bio,
                    Immagine = profilo.Immagine
                };

                return profiloDto != null ? Ok(new { message = "Profilo trovato!", profiloUtente = profiloDto }) : BadRequest(new ProfiloResponseDto { Message = "Errore durante il recupero del profilo." });
            }
            catch
            {
                return BadRequest(new ProfiloResponseDto { Message = "Qualcosa è andato storto." });
            }
        }
    }
}