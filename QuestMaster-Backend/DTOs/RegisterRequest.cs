namespace QuestMaster_Backend.DTOs
{
    // Obiekt transferu danych używany podczas rejestracji użytkownika.
    public class RegisterRequest
    {
        // Email nowego użytkownika.
        public required string Email { get; set; }
        
        // Hasło nowego użytkownika.
        public required string Password { get; set; }
    }
}
