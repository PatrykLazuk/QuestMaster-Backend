using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Amazon.DynamoDBv2.DataModel;
using QuestMaster_Backend.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System;

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
        [HttpPost]
        public async Task<IActionResult> CreateCharacter([FromBody] Character character)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            character.CharacterId = Guid.NewGuid().ToString();
            character.UserId = userId;
            character.CreatedAt = DateTime.UtcNow;

            // Zapisujemy postać w DynamoDB.
            await _dbContext.SaveAsync(character);
            return Ok(character);
        }
    }
}
