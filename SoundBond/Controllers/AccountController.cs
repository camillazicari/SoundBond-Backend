using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SoundBond.Data;
using SoundBond.DTOs.Account;
using SoundBond.DTOs.Artisti;
using SoundBond.DTOs.Brani;
using SoundBond.DTOs.Generi;
using SoundBond.DTOs.Profilo;
using SoundBond.Models;
using SoundBond.Services;
using SoundBond.Settings;

namespace SoundBond.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly Jwt _jwtSettings;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly AccountService _accountService;
        private readonly SoundBondDbContext _context;

        public AccountController(IOptions<Jwt> jwtOptions, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager, AccountService accountService, SoundBondDbContext context)
        {
            _jwtSettings = jwtOptions.Value;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _accountService = accountService;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var existingUser = await _userManager.Users.FirstOrDefaultAsync(u => u.NomeUtente == registerDto.NomeUtente);

            if (existingUser != null)
            {
                return BadRequest(new { message = "Nome utente già in uso." });
            }

            var newUser = new ApplicationUser()
            {
                Email = registerDto.Email,
                UserName = registerDto.Email,
                Nome = registerDto.Nome,
                Cognome = registerDto.Cognome,
                DataNascita = registerDto.DataDiNascita,
                NomeUtente = registerDto.NomeUtente
            };

            var result = await _userManager.CreateAsync(newUser, registerDto.Password);

            if (!result.Succeeded)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByEmailAsync(newUser.Email);

            await _userManager.AddToRoleAsync(newUser, "User");

            return Ok((new { message = "Registrazione avvenuta con successo" }));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null)
            {
                return Unauthorized(new { message = "Email o password errati" });
            }

            var loginResult = await _signInManager.PasswordSignInAsync(user, loginDto.Password, false, false);

            if (!loginResult.Succeeded)
            {
                return Unauthorized(new { message = "Email o password errati" });
            }

            var roles = await _signInManager.UserManager.GetRolesAsync(user);

            List<Claim> claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
            claims.Add(new Claim(ClaimTypes.Name, $"{user.Nome} {user.Cognome}"));
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecurityKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresInMinutes);

            var token = new JwtSecurityToken(_jwtSettings.Issuer, _jwtSettings.Audience, claims, expires: expiry, signingCredentials: creds);

            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new TokenDto()
            {
                Token = tokenString,
                Expires = expiry
            });
        }

        [HttpGet("utenti")]
        [Authorize]
        public async Task<IActionResult> GetUtenti()
        {
            try
            {
                var user = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                var email = user.Value;
                var result = await _accountService.GetUsers(email);
                if (result == null)
                {
                    return NotFound(new { message = "Nessun utente trovato" });
                }

                var utentii = result.Select(u => new UtentiDto()
                {
                    Id = u.Id,
                    Nome = u.Nome,
                    Cognome = u.Cognome,
                    Email = u.Email,
                    DataDiNascita = u.DataNascita,
                    NomeUtente = u.NomeUtente,
                    Profilo = new ProfiloDto()
                    {
                        Bio = u.Profilo?.Bio,
                        Immagine = u.Profilo?.Immagine,
                    },
                    Generi = u.Generi?.Select(g => new GetGeneriDto()
                    {
                        Id = g.Id,
                        Nome = g.Nome
                    }).ToList(),
                    Artisti = u.Artisti?.Select(a => new GetArtistiDto()
                    {
                        Id = a.Id,
                        Nome = a.Nome,
                        Img = a.Img
                    }).ToList(),
                    Brani = u.Brani?.Select(b => new GetBraniDto()
                    {
                        Id = b.Id,
                        Titolo = b.Titolo,
                        Artista = b.Artista,
                        Img = b.Img
                    }).ToList()
                });

                return Ok(new { message = "Utenti trovati!", utenti = utentii });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("utentiGenerali")]
        public async Task<IActionResult> GetAllUtenti()
        {
            try
            {
                var result = await _accountService.GetAllUsers();
                if (result == null)
                {
                    return NotFound(new { message = "Nessun utente trovato" });
                }

                var utentii = result.Select(u => new UtentiGenerali()
                {
                    Id = u.Id,
                    Nome = u.Nome,
                    Cognome = u.Cognome,
                    Email = u.Email,
                    NomeUtente = u.NomeUtente,
                });

                return Ok(new { message = "Utenti trovati!", utenti = utentii });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("utente")]
        [Authorize]
        public async Task<IActionResult> GetUtenti([FromQuery] string nomeUtente)
        {
            try
            {
                var user = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                var email = user!.Value;
                var result = await _accountService.GetUserByNomeUtente(nomeUtente, email);
                if (result == null)
                {
                    return NotFound(new { message = "Nessun utente trovato" });
                }

                var utente = new UtentiDto()
                {
                    Id = result.Id,
                    Nome = result.Nome,
                    Cognome = result.Cognome,
                    Email = result.Email,
                    DataDiNascita = result.DataNascita,
                    NomeUtente = result.NomeUtente,
                    Profilo = new ProfiloDto()
                    {
                        Bio = result.Profilo?.Bio,
                        Immagine = result.Profilo?.Immagine,
                    },
                    Generi = result.Generi?.Select(g => new GetGeneriDto()
                    {
                        Id = g.Id,
                        Nome = g.Nome
                    }).ToList(),
                    Artisti = result.Artisti?.Select(a => new GetArtistiDto()
                    {
                        Id = a.Id,
                        Nome = a.Nome,
                        Img = a.Img
                    }).ToList(),
                    Brani = result.Brani?.Select(b => new GetBraniDto()
                    {
                        Id = b.Id,
                        Titolo = b.Titolo,
                        Artista = b.Artista,
                        Img = b.Img
                    }).ToList()
                };

                return Ok(new { message = "Utente trovato!", user = utente });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpDelete("utente")]
        [Authorize]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Token non valido o scaduto" });
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound(new { message = "Utente non trovato" });
            }

            var bonds1 = _context.Bonders.Where(b => b.UserId1 == userId);
            var bonds2 = _context.Bonders.Where(b => b.UserId2 == userId);
            _context.Bonders.RemoveRange(bonds1);
            _context.Bonders.RemoveRange(bonds2);

            await _context.SaveChangesAsync();

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(new { message = "Errore nella cancellazione dell'account" });
            }

            return Ok(new { message = "Account eliminato con successo" });
        }

        [HttpGet("userLogged")]
        [Authorize]
        public async Task<IActionResult> GetUserLogged()
        {
            try
            {
                var user = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                var email = user!.Value;
                var result = await _accountService.GetUserAttuale(email);
                if (result == null)
                {
                    return NotFound(new { message = "Nessun utente trovato" });
                }

                var utente = new UtentiDto()
                {
                    Id = result.Id,
                    Nome = result.Nome,
                    Cognome = result.Cognome,
                    Email = result.Email,
                    DataDiNascita = result.DataNascita,
                    NomeUtente = result.NomeUtente,
                    Profilo = new ProfiloDto()
                    {
                        Bio = result.Profilo?.Bio,
                        Immagine = result.Profilo?.Immagine,
                    },
                    Generi = result.Generi?.Select(g => new GetGeneriDto()
                    {
                        Id = g.Id,
                        Nome = g.Nome
                    }).ToList(),
                    Artisti = result.Artisti?.Select(a => new GetArtistiDto()
                    {
                        Id = a.Id,
                        Nome = a.Nome,
                        Img = a.Img
                    }).ToList(),
                    Brani = result.Brani?.Select(b => new GetBraniDto()
                    {
                        Id = b.Id,
                        Titolo = b.Titolo,
                        Artista = b.Artista,
                        Img = b.Img
                    }).ToList()
                };

                return Ok(new { message = "Utente trovato!", user = utente });
            }
            catch(Exception ex)
            {
                return BadRequest(new { message = "Qualcosa è andato storto.", error = ex.Message });
            }
        }

        [HttpPut("nomeUtente")]
        [Authorize]
        public async Task<IActionResult> UpdateNomeUtente([FromBody] UpdateNomeUtenteDto updateNomeUtenteDto)
        {
            try
            {
                var user = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                var email = user!.Value;
                var utente = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Email == email);
                if (utente == null)
                {
                    return BadRequest(new { message = "Utente non trovato." });
                }

                var vecchioNomeUtente = utente.NomeUtente;

                if (vecchioNomeUtente == updateNomeUtenteDto.NuovoNomeUtente)
                {
                    return Ok(new { message = "Nessun nome utente modificato." });
                }

                var nomeUtenteEsistente = await _context.ApplicationUsers
                    .AnyAsync(u => u.NomeUtente == updateNomeUtenteDto.NuovoNomeUtente && u.Email != email);

                if (nomeUtenteEsistente)
                {
                    return BadRequest(new { message = "Nome utente già in uso." });
                }

                var result = await _accountService.UpdateNomeUtente(updateNomeUtenteDto, email);

                return result
                    ? Ok(new { message = "Nome utente modificato con successo." })
                    : BadRequest(new { message = "Errore nella modifica del nome utente." });
            }
            catch
            {
                return BadRequest(new { message = "Qualcosa è andato storto." });
            }
        }

    }
}