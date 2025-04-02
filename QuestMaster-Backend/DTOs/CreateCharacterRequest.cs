namespace QuestMaster_Backend.DTOs
{
    // Obiekt transferu danych przy tworzeniu nowej postaci.
    public class CreateCharacterRequest
    {
        // Nazwa postaci - wymagana
        public required string Name { get; set; }
        
        // Statystyki postaci - wymagana, np. {"Strength": 10, "Dexterity": 12}
        public required Dictionary<string, int> Stats { get; set; }
        
        // Opcjonalny identyfikator kampanii. Jeśli nie zostanie podany,
        // w kontrolerze przypiszemy domyślną wartość.
        public string? CampaignId { get; set; }
    }
}