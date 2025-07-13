using ApiECommerce.Context;
using ApiECommerce.Dtos;
using ApiECommerce.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly IConfiguration _config;

        public UsersController(AppDbContext appDbContext, IConfiguration config)
        {
            _appDbContext = appDbContext;
            _config = config;
        }


        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            var checkUser = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (checkUser != null)
            {
                return BadRequest("A user with this email address already exists.");
            }

            _appDbContext.Users.Add(user);
            await _appDbContext.SaveChangesAsync();
            return StatusCode(StatusCodes.Status201Created);
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            var currentUser = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Email == user.Email && u.Password == user.Password);

            if (currentUser == null)
            {
                return NotFound("User not found");
            }

            var key = _config["JWT:Key"] ?? throw new ArgumentNullException("JWT:Key", "JWT:Key cannot be null.");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user.Email!)
            };

            var token = new JwtSecurityToken(
                issuer: _config["JWT:Issuer"],
                audience: _config["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(10),
                signingCredentials: credentials);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return new ObjectResult(new
            {
                AccessToken = jwt,
                TokenType = "bearer",
                UserId = currentUser.Id,
                UserName = currentUser.Name
            });
        }

        [Authorize]
        [HttpPost("uploadimage")]
        public async Task<IActionResult> UploadUserPhoto(IFormFile image)
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var user = await _appDbContext.Users.FirstOrDefaultAsync(U => U.Email == userEmail);

            if (user == null)
            {
                return NotFound("User not found");
            }

            if (image != null)
            {
                string uniqueFileName = $"{Guid.NewGuid().ToString()}_{image.FileName}";

                string filePath = Path.Combine("wwwroot/userimages", uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                user.UrlImage = $"/userimages/{uniqueFileName}";

                _appDbContext.Entry(user).State = EntityState.Modified;

                await _appDbContext.SaveChangesAsync();
                return Ok("Image uploaded successfully");
            }

            return BadRequest("No image was uploaded");
        }


        [Authorize]
        [HttpGet("userimage")]
        public async Task<IActionResult> GetUserImage()
        {
            // 1. Obter o email do utilizador a partir do token
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            // Se por alguma razão o token não tiver o email
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized("Email claim not found in token.");
            }

            // 2. Encontrar o utilizador na base de dados (pode simplificar a sua consulta)
            var user = await _appDbContext.Users
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
            {
                return NotFound("User not found");
            }

            // 3. Criar e preencher o DTO com a informação necessária
            var responseDto = new UserImageDto
            {
                UrlImage = user.UrlImage // Mapear o campo da base de dados para a propriedade do DTO
            };

            // 4. Retornar o DTO. O ASP.NET Core irá serializá-lo para JSON automaticamente.
            return Ok(responseDto);
        }
    }
}
