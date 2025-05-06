using Microsoft.EntityFrameworkCore;
using SoundBond.Data;
using SoundBond.Models;

namespace SoundBond.Services
{
    public class RichiestaService
    {
        private readonly SoundBondDbContext _context;
        public RichiestaService(SoundBondDbContext context)
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

        public async Task<bool> Create(Richiesta richiesta)
        {
            try
            {
                var receiverEsistente = await _context.ApplicationUsers
            .AnyAsync(u => u.Id == richiesta.ReceiverId);

                if (!receiverEsistente)
                {
                    return false;
                }

                var richiestaEsistente = await _context.Richieste
                    .FirstOrDefaultAsync(a => a.ReceiverId == richiesta.ReceiverId &&
                                            a.SenderId == richiesta.SenderId);

                if (richiestaEsistente != null)
                {
                    return false;
                }

                _context.Richieste.Add(richiesta);
                return await SaveAsync();
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Richiesta>> GetRichiesteInviate(string email)
        {
            try
            {
                var user = await _context.ApplicationUsers.FirstOrDefaultAsync(e => e.Email == email);
                if (user == null)
                {
                    return null!;
                }

                var richieste = await _context.Richieste
                    .Include(r => r.Sender)
                    .Include(r => r.Receiver)
                        .ThenInclude(u => u.Generi)
                    .Include(r => r.Receiver)
                        .ThenInclude(u => u.Artisti)
                    .Include(r => r.Receiver)
                        .ThenInclude(u => u.Brani)
                    .Include(r => r.Receiver)
                        .ThenInclude(u => u.Profilo)
                    .Where(r => r.SenderId == user.Id) 
                    .ToListAsync();


                return richieste;
            }
            catch
            {
                return null!;
            }
        }

        public async Task<List<Richiesta>> GetRichiesteRicevute(string email)
        {
            try
            {
                var user = await _context.ApplicationUsers.FirstOrDefaultAsync(e => e.Email == email);
                if (user == null)
                {
                    return null!;
                }

                var richieste = await _context.Richieste
                   .Include(r => r.Sender)
                        .ThenInclude(u => u.Generi)
                    .Include(r => r.Sender)
                        .ThenInclude(u => u.Artisti)
                    .Include(r => r.Sender)
                        .ThenInclude(u => u.Brani)
                    .Include(r => r.Sender)
                        .ThenInclude(u => u.Profilo)
                    .Include(r => r.Receiver)
                    .Where(r => r.ReceiverId == user.Id)
                    .ToListAsync();
                return richieste;
            }
            catch
            {
                return null!;
            }
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

                var richiesta = await _context.Richieste.FirstOrDefaultAsync(r => (r.ReceiverId == id || r.SenderId == id) &&
    (r.ReceiverId == user.Id || r.SenderId == user.Id));
                if (richiesta == null)
                {
                    return false;
                }

                
                _context.Richieste.Remove(richiesta);
                return await SaveAsync();
            }
            catch
            {
                return false;
            }
        }
    }
}
