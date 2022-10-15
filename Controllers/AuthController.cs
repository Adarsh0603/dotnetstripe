using dotnetstripe.Data;
using dotnetstripe.DTO;
using dotnetstripe.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace dotnetstripe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly AppDbContext _db;
        private IConfiguration _configuration;

        public AuthController(AppDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        [HttpPost, Route("register")]
        public async Task<ActionResult<Token>> Register(UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var existingUser = _db.Users.FirstOrDefault(u => u.Username == userDto.Username);
            if (existingUser != null)
            {
                return BadRequest("User already exist with this username");
            }
            byte[] passwordHash;
            byte[] passwordSalt;

            GenerateHash(userDto.Password, out passwordHash, out passwordSalt);

            var user = new User
            {
                Username = userDto.Username,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,

            };

            _db.Users.Add(user);
            _db.SaveChanges();
            var token = CreateToken(user);
            return new Token
            {
                token = token
            };
        }

        [HttpPost, Route("login")]
        public async Task<ActionResult<Token>> Login(UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = _db.Users.FirstOrDefault(u => u.Username == userDto.Username);
            if (user == null)
            {
                return NotFound("No user found with this username.");
            }
            if (!VerifyPasswordHash(user, userDto.Password))
            {
                return BadRequest("Invalid password");
            }
            var token = CreateToken(user);
            return new Token
            {
                token = token
            };
        }
        [HttpGet, Route("checktoken"), Authorize()]
        public ActionResult CheckToken()
        {
            return Ok();
        }

        private void GenerateHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim("username",user.Username),
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(claims: claims,
                expires: DateTime.Now.AddDays(2),
                signingCredentials: creds
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

        private bool VerifyPasswordHash(User user, string password)
        {
            using (var hmac = new HMACSHA512(user.PasswordSalt))
            {
                return user.PasswordHash.SequenceEqual(hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)));
            }
        }

        [HttpGet, Authorize()]
        public ActionResult<String> GetProtectedData()
        {
            return "Hello";
        }
    }
}
