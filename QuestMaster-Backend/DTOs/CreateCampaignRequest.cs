namespace QuestMaster_Backend.DTOs
{
    public class CreateCampaignRequest
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}