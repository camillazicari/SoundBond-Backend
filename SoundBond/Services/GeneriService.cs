using Microsoft.EntityFrameworkCore;
using SoundBond.Data;
using SoundBond.DTOs.Generi;
using SoundBond.Models;

namespace SoundBond.Services
{
    public class GeneriService
    {
        private readonly SoundBondDbContext _context;

        public GeneriService(SoundBondDbContext context)
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

        public async Task<bool> Create(Generi genere, string email)
        {
            try
            {
                var user = await _context.ApplicationUsers.FirstOrDefaultAsync(e => e.Email == email);
                if (user == null)
                {
                    return false;
                }

                var genereEsistente = await _context.Generi.FirstOrDefaultAsync(g => g.Nome == genere.Nome && g.UserId == user.Id);

                if (genereEsistente != null)
                {
                    return false;
                }

                _context.Generi.Add(genere);

                return await SaveAsync();
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Generi>> GetGeneri(string email)
        {
            try
            {
                var user = await _context.ApplicationUsers.FirstOrDefaultAsync(e => e.Email == email);
                if (user == null)
                {
                    return null!;
                }

                var generi = await _context.Generi.Where(g => g.UserId == user.Id).ToListAsync();

                if (generi == null)
                {
                    return null!;
                }

                return generi;
            }
            catch
            {
                return null!;
            }
        }

        public async Task<bool> Update(string nome, GeneriDto genere, string email)
        {
            try
            {
                var user = await _context.ApplicationUsers.FirstOrDefaultAsync(e => e.Email == email);
                if (user == null)
                {
                    return false;
                }

                var genereEsistente = await _context.Generi.FirstOrDefaultAsync(g => g.Nome == nome && g.UserId == user.Id);

                if (genereEsistente == null )
                {
                    return false;
                }

                if(genereEsistente.Nome == genere.Nome)
                {
                    return true;
                }

                genereEsistente.Nome = genere.Nome;

                var generePresente = await _context.Generi.FirstOrDefaultAsync(g => g.Nome == genere.Nome && g.UserId == user.Id);

                if (generePresente != null)
                {
                    return false;
                }

                return await SaveAsync();

            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Delete(string nome, string email)
        {
            try
            {
                var user = await _context.ApplicationUsers.FirstOrDefaultAsync(e => e.Email == email);
                if (user == null)
                {
                    return false;
                }

                var genereEsistente = await _context.Generi.FirstOrDefaultAsync(g => g.Nome == nome && g.UserId == user.Id);

                if (genereEsistente == null)
                {
                    return false;
                }

                _context.Generi.Remove(genereEsistente);

                return await SaveAsync();
            }
            catch
            {
                return false;
            }
        }
    }
}