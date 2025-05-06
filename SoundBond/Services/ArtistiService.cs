using Microsoft.EntityFrameworkCore;
using SoundBond.Data;
using SoundBond.DTOs.Artisti;
using SoundBond.DTOs.Generi;
using SoundBond.Models;

namespace SoundBond.Services
{
    public class ArtistiService
    {
        private readonly SoundBondDbContext _context;

        public ArtistiService(SoundBondDbContext context)
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

        public async Task<bool> Add(Artisti artista, string email)
        {
            try
            {
                var user = await _context.ApplicationUsers.FirstOrDefaultAsync(e => e.Email == email);

                if (user == null)
                {
                    return false;
                }

                var artistaEsistente = await _context.Artisti.FirstOrDefaultAsync(a => a.Nome == artista.Nome && a.UserId == user.Id);
                if (artistaEsistente != null)
                {
                    return false;
                }

                _context.Artisti.Add(artista);
                return await SaveAsync();
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Artisti>> GetArtisti(string email)
        {
            try
            {
                var user = await _context.ApplicationUsers.FirstOrDefaultAsync(e => e.Email == email);
                if (user == null)
                {
                    return null!;
                }

                var artisti = await _context.Artisti.Where(a => a.UserId == user.Id).ToListAsync();

                if (artisti == null)
                {
                    return null!;
                }

                return artisti;
            }
            catch
            {
                return null!;
            }
        }

        public async Task<bool> Update(string nome, string email, ArtistiDto artista)
        {
            try
            {
                var user = await _context.ApplicationUsers.FirstOrDefaultAsync(e => e.Email == email);
                if (user == null)
                {
                    return false;
                }
                var artistaEsistente = await _context.Artisti.FirstOrDefaultAsync(a => a.Nome == nome && a.UserId == user.Id);

                if (artistaEsistente == null)
                {
                    return false;
                }

                if(artistaEsistente.Nome == artista.Nome)
                {
                    return true;
                }

                artistaEsistente.Img = artista.Img;
                artistaEsistente.Nome = artista.Nome;

                var artistaPresente = await _context.Artisti.FirstOrDefaultAsync(a => a.Nome == artista.Nome && a.UserId == user.Id);

                if (artistaPresente != null)
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
                var artista = await _context.Artisti.FirstOrDefaultAsync(a => a.Nome == nome && a.UserId == user.Id);
                if (artista == null)
                {
                    return false;
                }
                _context.Artisti.Remove(artista);
                return await SaveAsync();
            }
            catch
            {
                return false;
            }
        }
    }
}