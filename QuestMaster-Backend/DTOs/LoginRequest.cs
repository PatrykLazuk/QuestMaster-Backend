namespace QuestMaster_Backend.DTOs
{
    // Obiekt transferu danych używany podczas logowania użytkownika.
    public class LoginRequest
    {
        // Email użytkownika.
        public required string Email { get; set; }
        
        // Hasło użytkownika.
        public required string Password { get; set; }
    }
}
