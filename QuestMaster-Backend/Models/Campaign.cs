using Amazon.DynamoDBv2.DataModel;
using System;

namespace QuestMaster_Backend.Models
{
    // Model reprezentujący kampanię (sesję gry) utworzoną przez Mistrza Gry.
    [DynamoDBTable("QuestMasterCampaigns")]
    public class Campaign
    {
        // Unikalny identyfikator kampanii (klucz główny).
        [DynamoDBHashKey(AttributeName = "campaignId")]
        public required string CampaignId { get; set; }
        
        // Nazwa kampanii.
        public required string Name { get; set; }
        
        // Opis kampanii.
        public string? Description { get; set; }
        
        // ID użytkownika (Mistrza Gry), który utworzył kampanię.
        public required string OwnerUserId { get; set; }
        
        // Data utworzenia kampanii.
        public DateTime CreatedAt { get; set; }
    }
}
