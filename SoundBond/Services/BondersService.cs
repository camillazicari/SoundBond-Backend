using Microsoft.EntityFrameworkCore;
using SoundBond.Data;
using SoundBond.DTOs.Account;
using SoundBond.DTOs.Artisti;
using SoundBond.DTOs.Bonder;
using SoundBond.DTOs.Brani;
using SoundBond.DTOs.Generi;
using SoundBond.DTOs.Profilo;
using SoundBond.Models;

namespace SoundBond.Services
{
    public class BondersService
    {
        private readonly SoundBondDbContext _context;
        public BondersService(SoundBondDbContext context)
        {
            _context = context;
        }

        public async Task<bool> SaveAsync()
        {
            try
            {
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante il salvataggio: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> Create(Bonder bonder)
        {
            try
            {
                var bonderEsistente = await _context.ApplicationUsers.AnyAsync(u => u.Id == bonder.UserId2);

                if (!bonderEsistente)
                {
                    return false;
                }

                var bondingEsistente = await _context.Bonders
                    .FirstOrDefaultAsync(a => a.UserId2 == bonder.UserId2 &&
                                            a.UserId1 == bonder.UserId1);

                if (bondingEsistente != null)
                {
                    return false;
                }

                _context.Bonders.Add(bonder);
                return await SaveAsync();
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<BonderDto>> GetBonders(string email)
        {
            var user = await _context.ApplicationUsers
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return [];

            var userId = user.Id;

            return await _context.Bonders
                .Where(r => r.UserId1 == userId || r.UserId2 == userId)
                .Select(b => new BonderDto
                {
                    Id = b.Id,
                    ConnectionDate = b.ConnectionDate,
                    OtherUser = b.UserId1 == userId
                        ? new UtentiBondersDto
                        {
                            Id = b.User2.Id,
                            Nome = b.User2.Nome,
                            Cognome = b.User2.Cognome,
                            Email = b.User2.Email,
                            NomeUtente = b.User2.NomeUtente,
                            Profilo = b.User2.Profilo == null ? new ProfiloDto
                            {
                                Immagine = "https://cdn1.iconfinder.com/data/icons/avatars-55/100/avatar_profile_user_music_headphones_shirt_cool-512.png",
                                Bio = "Music Lover"
                            } : new ProfiloDto
                            {
                                Immagine = b.User2.Profilo.Immagine,
                                Bio = b.User2.Profilo.Bio
                            }
                        }
                        : new UtentiBondersDto
                        {
                            Id = b.User1.Id,
                            Nome = b.User1.Nome,
                            Cognome = b.User1.Cognome,
                            Email = b.User1.Email,
                            NomeUtente = b.User1.NomeUtente,
                            Profilo = b.User1.Profilo == null ? new ProfiloDto
                            {
                                Immagine = "https://cdn1.iconfinder.com/data/icons/avatars-55/100/avatar_profile_user_music_headphones_shirt_cool-512.png",
                                Bio = "Music Lover"
                            } : new ProfiloDto
                            {
                                Immagine = b.User1.Profilo.Immagine,
                                Bio = b.User1.Profilo.Bio
                            }
                        }
                })
                .ToListAsync();
        }


        public async Task<bool> Delete(string id, string email)
        {
            try
            {
                var user = await _context.ApplicationUsers.FirstOrDefaultAsync(e => e.Email == email);
                if (user == null)
                {
                    return false;
                }

                var bonding = await _context.Bonders.FirstOrDefaultAsync(b =>(b.UserId1 == id && b.UserId2 == user.Id) || (b.UserId1 == user.Id && b.UserId2 == id));

                if (bonding == null)
                {
                    return false;
                }

                if (bonding.UserId1 != user.Id && bonding.UserId2 != user.Id)
                {
                    return false;
                }

                _context.Bonders.Remove(bonding);
                return await SaveAsync();
            }
            catch
            {
                return false;
            }
        }
    }

}
