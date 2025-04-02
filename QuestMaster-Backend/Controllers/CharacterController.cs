using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Amazon.DynamoDBv2.DataModel;
using QuestMaster_Backend.Models;
using System.Security.Claims;
using QuestMaster_Backend.DTOs;

namespace QuestMaster_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CharacterController : ControllerBase
    {
        private readonly IDynamoDBContext _dbContext;
        public CharacterController(IDynamoDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Endpoint GET do pobrania postaci przypisanych do danej kampanii.
        [HttpGet("{campaignId}")]
        public async Task<IActionResult> GetCharacters(string campaignId)
        {
            // Skanujemy tabelę postaci filtrując po CampaignId.
            var conditions = new List<ScanCondition>
            {
                new ScanCondition("CampaignId", Amazon.DynamoDBv2.DocumentModel.ScanOperator.Equal, campaignId)
            };
            var characters = await _dbContext.ScanAsync<Character>(conditions).GetRemainingAsync();
            return Ok(characters);
        }

        // Endpoint POST do tworzenia nowej postaci.
        // Klient przesyła tylko dane niezbędne do utworzenia postaci (Name, Stats, opcjonalnie CampaignId).
        [HttpPost]
        public async Task<IActionResult> CreateCharacter([FromBody] CreateCharacterRequest request)
        {
            // Pobieramy ID użytkownika z tokenu (zakładamy, że jest ustawione).
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            
            // Mapujemy dane z DTO na model domenowy.
            var character = new Character
            {
                CharacterId = Guid.NewGuid().ToString(),
                // Jeśli kampania nie została przekazana, przypisujemy domyślną wartość "NONE"
                CampaignId = request.CampaignId ?? "NONE",
                UserId = userId,
                Name = request.Name,
                Stats = request.Stats,
                CreatedAt = DateTime.UtcNow
            };

            // Zapisujemy postać w DynamoDB.
            await _dbContext.SaveAsync(character);
            return Ok(character);
        }
    }
}
