using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SoundBond.Controllers;
using SoundBond.Data;
using SoundBond.DTOs.Account;
using SoundBond.Models;

namespace SoundBond.Services
{
    public class AccountService
    {
        private readonly SoundBondDbContext _context;
        public AccountService(SoundBondDbContext context)
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

        public async Task<List<ApplicationUser>> GetUsers(string email)
        {
            try
            {
                var user = _context.ApplicationUsers.FirstOrDefault(e => e.Email == email);
                return await _context.Users.Where(u => u.Id != user.Id).Include(u => u.Generi).Include(u => u.Artisti).Include(u => u.Brani).Include(u => u.Profilo).ToListAsync();
            }
            catch
            {
                return null!;
            }
        }

        public async Task<List<ApplicationUser>> GetAllUsers()
        {
            try
            {
                return await _context.Users.ToListAsync();
            }
            catch
            {
                return null!;
            }
        }

        public async Task<ApplicationUser> GetUserByNomeUtente(string nomeUtente, string email)
        {
            try
            {
                var user = _context.ApplicationUsers.FirstOrDefault(e => e.Email == email);
                return await _context.ApplicationUsers.Where(u => u.Id != user.Id).Include(u => u.Generi).Include(u => u.Artisti).Include(u => u.Brani).Include(u => u.Profilo).AsSplitQuery().FirstOrDefaultAsync(u => u.NomeUtente == nomeUtente);
            }
            catch
            {
                return null!;
            }
        }

        public async Task<ApplicationUser> GetUserAttuale(string email)
        {
            try
            {
                return await _context.ApplicationUsers.Where(u => u.Email == email).Include(u => u.Generi).Include(u => u.Artisti).Include(u => u.Brani).Include(u => u.Profilo).AsSplitQuery().FirstOrDefaultAsync();
            }
            catch
            {
                return null!;
            }
        }

        public async Task<bool> UpdateNomeUtente(UpdateNomeUtenteDto updateNomeUtenteDto, string email)
        {
            try
            {
                var user = await _context.ApplicationUsers.FirstOrDefaultAsync(e => e.Email == email);
                if (user == null) return false;

                var nomeUtenteEsistente = await _context.ApplicationUsers
                    .AnyAsync(u => u.NomeUtente == updateNomeUtenteDto.NuovoNomeUtente && u.Email != email);

                if (nomeUtenteEsistente)
                {
                    return false;
                }

                user.NomeUtente = updateNomeUtenteDto.NuovoNomeUtente;
                return await SaveAsync();
            }
            catch
            {
                return false;
            }
        }


    }
}
