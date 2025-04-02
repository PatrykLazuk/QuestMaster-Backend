using Microsoft.AspNetCore.Mvc;
using Amazon.DynamoDBv2.DataModel;
using QuestMaster_Backend.Models;
using QuestMaster_Backend.DTOs;
using QuestMaster_Backend.Services;

namespace QuestMaster_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IDynamoDBContext _dbContext;
        private readonly IConfiguration _configuration;
        
        public AuthController(IDynamoDBContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        // Endpoint POST do rejestracji użytkownika.
        // Odbiera obiekt RegisterRequest i zapisuje nowego użytkownika w DynamoDB.
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // Skanujemy tabelę, aby sprawdzić, czy email już istnieje.
            var conditions = new List<ScanCondition>
            {
                new ScanCondition("Email", Amazon.DynamoDBv2.DocumentModel.ScanOperator.Equal, request.Email)
            };
            var existingUsers = await _dbContext.ScanAsync<User>(conditions).GetRemainingAsync();
            if (existingUsers.Any())
                return BadRequest("Użytkownik o podanym emailu już istnieje.");

            // Tworzymy nowego użytkownika, hashując hasło.
            var user = new User
            {
                UserId = Guid.NewGuid().ToString(),
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Provider = "basic"
            };

            // Zapisujemy użytkownika w DynamoDB.
            await _dbContext.SaveAsync(user);
            return Ok(new { user.UserId });
        }

        // Endpoint POST do logowania użytkownika.
        // Weryfikuje dane logowania i generuje token JWT.
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Skanujemy tabelę, aby znaleźć użytkownika o podanym emailu.
            var conditions = new List<ScanCondition>
            {
                new ScanCondition("Email", Amazon.DynamoDBv2.DocumentModel.ScanOperator.Equal, request.Email)
            };
            var users = await _dbContext.ScanAsync<User>(conditions).GetRemainingAsync();
            var user = users.FirstOrDefault();
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Unauthorized("Nieprawidłowy email lub hasło.");
            }

            // Pobieramy ustawienia JWT z konfiguracji.
            // Pobieramy ustawienia JWT z konfiguracji, a jeśli któregoś brakuje, rzucamy wyjątkiem
            var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key is not configured.");
            var issuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT issuer is not configured.");
            var audience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT audience is not configured.");
            var expiresInMinutes = int.Parse(_configuration["Jwt:ExpiresInMinutes"] ?? "720");

            // Generujemy token JWT przy użyciu TokenHelper.
            var token = TokenHelper.GenerateToken(user, jwtKey, issuer, audience, expiresInMinutes);
            return Ok(new { token });
        }
    }
}
