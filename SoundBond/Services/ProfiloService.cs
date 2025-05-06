using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SoundBond.Data;
using SoundBond.DTOs.Profilo;
using SoundBond.Models;


namespace SoundBond.Services
{
    public class ProfiloService
    {
        private readonly SoundBondDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProfiloService(SoundBondDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
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

        public async Task<bool> Create(Profilo profilo)
        {
            try
            {
                _context.Profili.Add(profilo);
                return await SaveAsync();
            }
            catch
            {
                return false;
            }
        }

        public async Task<Profilo> GetProfilo(string email)
        {
            try
            {
                var user = _context.ApplicationUsers.FirstOrDefault(e => e.Email == email);
                var profiloEsistente = await _context.Profili.FirstOrDefaultAsync(a => a.UserId == user.Id);

                if(profiloEsistente == null)
                {
                    profiloEsistente = new Profilo()
                    {
                        Immagine = "https://cdn1.iconfinder.com/data/icons/avatars-55/100/avatar_profile_user_music_headphones_shirt_cool-512.png",
                        Bio = "Music lover"
                    };
                }

                return profiloEsistente!;
            }
            catch
            {
                return null!;
            }
        }

        public async Task<bool> Create(Profilo profilo, string email)
        {
            try
            {
                var user = _context.ApplicationUsers.FirstOrDefault(e => e.Email == email);
                var profiloEsistente = await _context.Profili.FirstOrDefaultAsync(a => a.UserId == user.Id);

                if (profiloEsistente != null)
                {
                    return false;
                }

                _context.Profili.Add(profilo);
                return await SaveAsync();
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateProfilo(string email, ProfiloDto profiloDto)
        {
            try
            {
                var user = _context.ApplicationUsers.FirstOrDefault(e => e.Email == email);
                var profiloEsistente = await _context.Profili.FirstOrDefaultAsync(a => a.UserId == user.Id);
                if (profiloEsistente == null)
                {
                    return false;
                }

                profiloEsistente.Bio = profiloDto.Bio;

                if (profiloDto.ImgFile != null && profiloDto.ImgFile.Length > 0)
                {
                    var imagesFolder = Path.Combine(_env.WebRootPath, "images");
                    if (!Directory.Exists(imagesFolder))
                        Directory.CreateDirectory(imagesFolder);
                    var uniqueName = $"{Guid.NewGuid()}{Path.GetExtension(profiloDto.ImgFile.FileName)}";
                    var filePath = Path.Combine(imagesFolder, uniqueName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await profiloDto.ImgFile.CopyToAsync(stream);
                    }
                    profiloEsistente.Immagine = $"/images/{uniqueName}";
                }
                else if (!string.IsNullOrEmpty(profiloDto.Immagine))
                {
                    profiloEsistente.Immagine = profiloDto.Immagine;
                }

                return await SaveAsync();
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
