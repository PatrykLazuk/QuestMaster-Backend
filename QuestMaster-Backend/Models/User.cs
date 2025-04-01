using Amazon.DynamoDBv2.DataModel;
using System;

namespace QuestMaster_Backend.Models
{
    // Model reprezentujący użytkownika w systemie.
    // Atrybut [DynamoDBTable] wskazuje nazwę tabeli w DynamoDB.
    [DynamoDBTable("QuestMasterUsers")]
    public class User
    {
        // Klucz główny tabeli – unikalny identyfikator użytkownika.
        [DynamoDBHashKey(AttributeName = "userId")]
        public required string UserId { get; set; }
        
        // Email użytkownika.
        public required string Email { get; set; }
        
        // Zahashowane hasło użytkownika.
        public required string PasswordHash { get; set; }
        
        // Informacja o typie konta (np. "basic", "google", "apple").
        public required string Provider { get; set; }
    }
}
