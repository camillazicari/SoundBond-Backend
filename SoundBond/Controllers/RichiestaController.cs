using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoundBond.Data;
using SoundBond.DTOs.Richiesta;
using SoundBond.DTOs.Account;
using SoundBond.Models;
using SoundBond.Services;
using Microsoft.EntityFrameworkCore;
using SoundBond.DTOs.Brani;
using SoundBond.DTOs.Generi;
using SoundBond.DTOs.Profilo;
using SoundBond.DTOs.Artisti;

namespace SoundBond.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RichiestaController : ControllerBase
    {
        private readonly SoundBondDbContext _context;
        private readonly RichiestaService _richiestaService;

        public RichiestaController(SoundBondDbContext context, RichiestaService richiestaService)
        {
            _context = context;
            _richiestaService = richiestaService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRichiestaDto createRichiestaDto)
        {
            try
            {
                var senderEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var sender = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Email == senderEmail);

                if (sender == null)
                    return BadRequest("Utente mittente non trovato.");

                var receiver = await _context.ApplicationUsers
                    .FirstOrDefaultAsync(u => u.Id == createRichiestaDto.ReceiverId);

                if (receiver == null)
                    return BadRequest("Utente destinatario non trovato.");

                var richiesta = new Richiesta()
                {
                    SenderId = sender.Id,
                    ReceiverId = createRichiestaDto.ReceiverId,
                    RequestDate = DateTime.UtcNow
                };

                var result = await _richiestaService.Create(richiesta);

                return result ?
                    Ok(new RichiestaResponseDto { Message = "Richiesta aggiunta con successo." }) :
                    BadRequest(new RichiestaResponseDto { Message = "Errore durante l'aggiunta della richiesta o richiesta già esistente." });
            }
            catch
            {
                return BadRequest(new RichiestaResponseDto { Message = "Qualcosa è andato storto." });
            }
        }

        [HttpGet("inviate")]
        public async Task<IActionResult> GetRichiesteInviate()
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
                var richieste = await _richiestaService.GetRichiesteInviate(email);
                if (richieste == null)
                {
                    return NotFound(new RichiestaResponseDto { Message = "Nessuna richiesta trovata." });
                }

                var listaRichiesta = richieste.Select(r => new RichiestaInviataDto
                {
                    Id = r.Id,
                    RequestDate = r.RequestDate,
                    Sender = new BonderRichiestaUtenteDto
                    {
                        Id = r.Sender.Id,
                        Nome = r.Sender.Nome,
                        Cognome = r.Sender.Cognome,
                        Email = r.Sender.Email,
                    },
                    Receiver = new UtentiDto
                    {
                        Id = r.Receiver.Id,
                        Nome = r.Receiver.Nome,
                        Cognome = r.Receiver.Cognome,
                        Email = r.Receiver.Email,
                        NomeUtente = r.Receiver.NomeUtente,
                        Profilo = new ProfiloDto
                        {
                            Immagine = r.Receiver.Profilo?.Immagine ?? "https://cdn1.iconfinder.com/data/icons/avatars-55/100/avatar_profile_user_music_headphones_shirt_cool-512.png",
                            Bio = r.Receiver.Profilo?.Bio ?? "Music Lover"
                        },
                        Brani = r.Receiver.Brani?.Select(b => new GetBraniDto
                        {
                            Id = b.Id,
                            Titolo = b.Titolo,
                            Img = b.Img,
                            Artista = b.Artista
                        }).ToList(),
                        Generi = r.Receiver.Generi?.Select(g => new GetGeneriDto
                        {
                            Id = g.Id,
                            Nome = g.Nome
                        }).ToList(),
                        Artisti = r.Receiver.Artisti?.Select(a => new GetArtistiDto
                        {
                            Id = a.Id,
                            Nome = a.Nome,
                            Img = a.Img
                        }).ToList()
                    }
                }).ToList();
                return Ok(new
                {
                    message = "Richieste trovate",
                    richiesteInviate = listaRichiesta
                });
            }
            catch
            {
                return BadRequest(new RichiestaResponseDto { Message = "Qualcosa è andato storto." });
            }
        }

        [HttpGet("ricevute")]
        public async Task<IActionResult> GetRichiesteRicevute()
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
                var richieste = await _richiestaService.GetRichiesteRicevute(email);
                if (richieste == null)
                {
                    return NotFound(new RichiestaResponseDto { Message = "Nessuna richiesta trovata." });
                }

                var listaRichiesta = richieste.Select(r => new RichiestaRicevutaDto
                {
                    Id = r.Id,
                    RequestDate = r.RequestDate,
                    Sender = new UtentiDto
                    {
                        Id = r.Sender.Id,
                        Nome = r.Sender.Nome,
                        Cognome = r.Sender.Cognome,
                        Email = r.Sender.Email,
                        NomeUtente = r.Sender.NomeUtente,
                        Profilo = new ProfiloDto
                        {
                            Immagine = r.Sender.Profilo?.Immagine ?? "https://cdn1.iconfinder.com/data/icons/avatars-55/100/avatar_profile_user_music_headphones_shirt_cool-512.png",
                            Bio = r.Sender.Profilo?.Bio ?? "Music Lover"
                        },
                        Brani = r.Sender.Brani?.Select(b => new GetBraniDto
                        {
                            Id = b.Id,
                            Titolo = b.Titolo,
                            Img = b.Img,
                            Artista = b.Artista
                        }).ToList(),
                        Generi = r.Sender.Generi?.Select(g => new GetGeneriDto
                        {
                            Id = g.Id,
                            Nome = g.Nome
                        }).ToList(),
                        Artisti = r.Sender.Artisti?.Select(a => new GetArtistiDto
                        {
                            Id = a.Id,
                            Nome = a.Nome,
                            Img = a.Img
                        }).ToList()
                    },
                    Receiver = new BonderRichiestaUtenteDto
                    {
                        Id = r.Receiver.Id,
                        Nome = r.Receiver.Nome,
                        Cognome = r.Receiver.Cognome,
                        Email = r.Receiver.Email,
                    }
                }).ToList();

                return Ok(new
                {
                    message = "Richieste trovate",
                    richiesteRicevute = listaRichiesta,
                });
            }
            catch
            {
                return BadRequest(new RichiestaResponseDto { Message = "Qualcosa è andato storto." });
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

                var result = await _richiestaService.Delete(id, email);

                return result ? Ok(new RichiestaResponseDto { Message = "Richiesta eliminata con successo." }) : BadRequest(new RichiestaResponseDto { Message = "Errore nell'eliminazione della richiesta." });
            }
            catch
            {
                return BadRequest(new RichiestaResponseDto { Message = "Qualcosa è andato storto." });
            }
        }
    }
}