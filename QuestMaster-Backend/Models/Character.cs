using Amazon.DynamoDBv2.DataModel;

namespace QuestMaster_Backend.Models
{
    // Model reprezentujący postać w kampanii.
    // Tabela posiada klucz główny (CharacterId) oraz klucz sortujący (CampaignId).
    [DynamoDBTable("QuestMasterCharacters")]
    public class Character
    {
        // Unikalny identyfikator postaci.
        [DynamoDBHashKey(AttributeName = "characterId")]
        public required string CharacterId { get; set; }
        
        // Klucz sortujący – identyfikator kampanii, do której należy postać.
        // Aby spełnić wymogi DynamoDB, wartość ta musi być nie-null. Dlatego, gdy postać nie jest przypisana,
        // przypiszemy domyślną wartość np. "NONE".
        [DynamoDBRangeKey(AttributeName = "campaignId")]
        public required string CampaignId { get; set; }
        
        // ID użytkownika (gracza), do którego należy postać.
        public required string UserId { get; set; }
        
        // Nazwa postaci.
        public required string Name { get; set; }
        
        // Statystyki postaci, np. {"Strength": 10, "Dexterity": 12}.
        public required Dictionary<string, int> Stats { get; set; }
        
        // Data utworzenia postaci.
        public DateTime CreatedAt { get; set; }
    }
}
