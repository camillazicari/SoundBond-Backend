using System.Globalization;
using Microsoft.EntityFrameworkCore;
using SoundBond.Data;
using SoundBond.DTOs.Brani;
using SoundBond.Models;

namespace SoundBond.Services
{
    public class BraniService
    {
        private readonly SoundBondDbContext _context;

        public BraniService(SoundBondDbContext context)
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

        public async Task<bool> Create(Brani brano, string email)
        {
            try
            {
                var user = await _context.ApplicationUsers.FirstOrDefaultAsync(e => e.Email == email);

                if (user == null)
                {
                    return false;
                }

                var branoEsistente = await _context.Brani.FirstOrDefaultAsync(a => a.Titolo == brano.Titolo && a.Artista == brano.Artista && a.UserId == user.Id);
                if (branoEsistente != null)
                {
                    return false;
                }

                _context.Brani.Add(brano);
                return await SaveAsync();
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Brani>> GetBrani(string email)
        {
            try
            {
                var user = await _context.ApplicationUsers.FirstOrDefaultAsync(e => e.Email == email);

                if (user == null)
                {
                    return null!;
                }

                var brani = await _context.Brani.Where(b => b.UserId == user.Id).ToListAsync();

                return brani;
            }
            catch
            {
                return null!;
            }
        }

        public async Task<bool> Update(string titolo, string artista, BraniDto braniDto, string email)
        {
            try
            {
                var user = await _context.ApplicationUsers.FirstOrDefaultAsync(e => e.Email == email);

                if (user == null)
                {
                    return false!;
                }

                var branoEsistente = await _context.Brani.FirstOrDefaultAsync(b => b.Titolo == titolo && b.Artista == artista && b.UserId == user.Id);

                if (branoEsistente == null) 
                {
                    return false;
                }

                if (branoEsistente.Titolo == braniDto.Titolo && branoEsistente.Artista == braniDto.Artista )
                {
                    return true;
                }

                branoEsistente.Img = braniDto.Img;
                branoEsistente.Titolo = braniDto.Titolo;
                branoEsistente.Artista = braniDto.Artista;

                var branoPresente = await _context.Brani.FirstOrDefaultAsync(b => b.Titolo == braniDto.Titolo && b.Artista == braniDto.Artista && b.UserId == user.Id);

                if(branoPresente != null)
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

        public async Task<bool> Delete(string titolo, string artista, string email)
        {
            try
            {
                var user = await _context.ApplicationUsers.FirstOrDefaultAsync(e => e.Email == email);

                if (user == null)
                {
                    return false!;
                }

                var branoEsistente = await _context.Brani.FirstOrDefaultAsync(b => b.Titolo == titolo && b.Artista == artista && b.UserId == user.Id);

                if (branoEsistente == null)
                {
                    return false;
                }

                _context.Brani.Remove(branoEsistente);

                return await SaveAsync();
            }
            catch
            {
                return false;
            }
        }
    }
}
