using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoundBond.Data;
using SoundBond.DTOs.Messaggio;
using SoundBond.Models;

namespace SoundBond.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly SoundBondDbContext _context;

        public MessagesController(SoundBondDbContext context)
        {
            _context = context;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserChats(string userId)
        {
            var user = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            var email = user!.Value;
            var currentUser = _context.ApplicationUsers.FirstOrDefault(u => u.Email == email);
            if (currentUser == null)
            {
                return BadRequest(new { message = "Utente non trovato." });
            }

            var lastDeletion = await _context.ChatCancellate
                .Where(d => d.UserId == currentUser.Id && d.OtherUserId == userId)
                .OrderByDescending(d => d.DeletedAt)
                .FirstOrDefaultAsync();

            var messagesQuery = _context.Messages
                .Where(m => (m.SenderId == userId && m.ReceiverId == currentUser.Id) ||
                            (m.ReceiverId == userId && m.SenderId == currentUser.Id));

            if (lastDeletion != null)
            {
                messagesQuery = messagesQuery.Where(m => m.Timestamp > lastDeletion.DeletedAt);
            }

            var messages = await messagesQuery
                .OrderBy(m => m.Timestamp)
                .ToListAsync();

            return Ok(new { message = "Messaggi trovati!", messaggi = messages });
        }

        [HttpGet("conversazioni")]
        public async Task<IActionResult> GetUserConversations()
        {
            var user = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            var email = user!.Value;
            var currentUser = _context.ApplicationUsers.FirstOrDefault(u => u.Email == email);
            if (currentUser == null)
            {
                return BadRequest(new { message = "Utente non trovato." });
            }
            var messages = await _context.Messages
                .Where(m => (m.SenderId == currentUser.Id || m.ReceiverId == currentUser.Id))
                .Where(m =>
                    !_context.ChatCancellate.Any(c =>
                        c.UserId == currentUser.Id &&
                        c.OtherUserId == (m.SenderId == currentUser.Id ? m.ReceiverId : m.SenderId) &&
                        m.Timestamp <= c.DeletedAt))
                .ToListAsync();
            var groupedMessages = messages
                .GroupBy(m => m.SenderId == currentUser.Id ? m.ReceiverId : m.SenderId)
                .Where(g => g.Any())
                .Select(g => new
                {
                    ChatWithUserId = g.Key,
                    UltimoMessaggio = g.OrderByDescending(m => m.Timestamp).FirstOrDefault(),
                    MessaggiNonLetti = g.Count(m =>
                        m.ReceiverId == currentUser.Id &&
                        m.SenderId == g.Key &&
                        !m.Letto)
                })
                .ToList();
            var result = new List<ConversazioneDto>();
            foreach (var chat in groupedMessages)
            {
                var utente = await _context.Users
                    .Include(u => u.Profilo)
                    .FirstOrDefaultAsync(u => u.Id == chat.ChatWithUserId);
                if (utente != null && chat.UltimoMessaggio != null)
                {
                    bool isRead = chat.MessaggiNonLetti == 0 || chat.UltimoMessaggio.SenderId == currentUser.Id;

                    result.Add(new ConversazioneDto
                    {
                        ChatWithUserId = utente.Id,
                        Nome = utente.Nome,
                        Cognome = utente.Cognome,
                        Immagine = utente.Profilo?.Immagine ?? "https://cdn1.iconfinder.com/data/icons/avatars-55/100/avatar_profile_user_music_headphones_shirt_cool-512.png",
                        UltimoMessaggio = chat.UltimoMessaggio.Content,
                        OraUltimoMessaggio = chat.UltimoMessaggio.Timestamp,
                        Letto = isRead,
                        MessaggiNonLetti = chat.MessaggiNonLetti
                    });
                }
            }
            return Ok(result);
        }

        [HttpPost("deleteChat/{otherUserId}")]
        public async Task<IActionResult> DeleteChat(string otherUserId)
        {
            var user = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            var email = user!.Value;
            var currentUser = _context.ApplicationUsers.FirstOrDefault(u => u.Email == email);
            if (currentUser == null)
            {
                return BadRequest(new { message = "Utente non trovato." });
            }


            var deleteRecord = new ChatCancellata
            {
                UserId = currentUser.Id,
                OtherUserId = otherUserId,
                DeletedAt = DateTime.UtcNow
            };

            _context.ChatCancellate.Add(deleteRecord);
            await _context.SaveChangesAsync();

            return Ok("Chat eliminata con successo.");
        }

    }

}
