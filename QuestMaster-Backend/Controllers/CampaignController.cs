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
    public class CampaignController : ControllerBase
    {
        private readonly IDynamoDBContext _dbContext;
        public CampaignController(IDynamoDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Endpoint GET do pobrania kampanii utworzonych przez zalogowanego użytkownika (Mistrz Gry).
        [HttpGet]
        public async Task<IActionResult> GetCampaigns()
        {
            // Pobieramy ID użytkownika z tokenu.
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            // Skanujemy kampanie filtrując po polu OwnerUserId.
            var conditions = new List<ScanCondition>
            {
                new ScanCondition("OwnerUserId", Amazon.DynamoDBv2.DocumentModel.ScanOperator.Equal, userId)
            };
            var campaigns = await _dbContext.ScanAsync<Campaign>(conditions).GetRemainingAsync();
            return Ok(campaigns);
        }

        // Endpoint POST do tworzenia nowej kampanii.
        [HttpPost]
        public async Task<IActionResult> CreateCampaign([FromBody] CreateCampaignRequest request)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var campaign = new Campaign
            {
                CampaignId = Guid.NewGuid().ToString(),
                Name = request.Name,
                Description = request.Description,
                OwnerUserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _dbContext.SaveAsync(campaign);
            return Ok(campaign);
        }
    }
}
