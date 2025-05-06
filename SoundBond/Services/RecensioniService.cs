using Microsoft.EntityFrameworkCore;
using SoundBond.Data;
using SoundBond.DTOs.Recensioni;
using SoundBond.Models;

namespace SoundBond.Services
{
    public class RecensioniService
    {
        private readonly SoundBondDbContext _context;

        public RecensioniService(SoundBondDbContext context)
        {
            _context = context;
        }

        public async Task<bool> SaveAsync()
        {
            try
            {
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;

            }
        }

        public async Task<bool> Create(Recensione recensione, string email)
        {
            try
            {
                var user = await _context.ApplicationUsers.FirstOrDefaultAsync(e => e.Email == email);

                if (user == null)
                {
                    return false;
                }

                var recensioneEsistente = await _context.Recensioni.FirstOrDefaultAsync(a => a.ApplicationUserId == user.Id);
                if (recensioneEsistente != null)
                {
                    return false;
                }

                _context.Recensioni.Add(recensione);
                return await SaveAsync();
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Recensione>> GetRecensioni()
        {
            try
            {
                var recensioni = await _context.Recensioni.Include(r => r.ApplicationUser).ThenInclude(a => a.Profilo).ToListAsync();

                if (recensioni == null)
                {
                    return null!;
                }

                return recensioni;
            }
            catch
            {
                return null!;
            }
        }

        public async Task<Recensione> GetPropriaRecensione(string email)
        {
            try
            {
                var user = await _context.ApplicationUsers.Include(a => a.Profilo).FirstOrDefaultAsync(e => e.Email == email);

                if (user == null)
                {
                    return null!;
                }

                var recensione = await _context.Recensioni.Include(r => r.ApplicationUser).FirstOrDefaultAsync(a => a.ApplicationUserId == user.Id);

                if (recensione == null)
                {
                    return null;
                }

                return recensione;
            }
            catch
            {
                return null!;
            }
        }

        public async Task<bool> Update(CreateRecensioneDto createRecensioneDto, string email)
        {
            try
            {
                var user = await _context.ApplicationUsers.FirstOrDefaultAsync(e => e.Email == email);

                if (user == null)
                {
                    return false;
                }

                var recensione = await _context.Recensioni.FirstOrDefaultAsync(r => r.ApplicationUserId == user.Id);

                if (recensione == null)
                {
                    return false;
                }

                recensione.Testo = createRecensioneDto.Testo;
                recensione.Voto = createRecensioneDto.Voto;

                return await SaveAsync();
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Delete(string email)
        {
            try 
            {
                var user = await _context.ApplicationUsers.FirstOrDefaultAsync(e => e.Email == email);

                if (user == null)
                {
                    return false;
                }

                var recensione = await _context.Recensioni.FirstOrDefaultAsync(r => r.ApplicationUserId == user.Id);

                if (recensione == null)
                {
                    return false;
                }

                _context.Recensioni.Remove(recensione);
                return await SaveAsync();
            }
            catch
            {
                return false;
            }
        }
    }
}