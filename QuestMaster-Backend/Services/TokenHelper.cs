using QuestMaster_Backend.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QuestMaster_Backend.Services
{
    // Klasa pomocnicza do generowania tokenów JWT.
    public static class TokenHelper
    {
        // Generuje token JWT na podstawie danych użytkownika.
        // Parametry:
        //   user - obiekt reprezentujący użytkownika.
        //   jwtKey - klucz do podpisania tokenu (jako ciąg znaków).
        //   issuer - wydawca tokenu.
        //   audience - odbiorca tokenu.
        //   expiresInMinutes - czas ważności tokenu w minutach.
        public static string GenerateToken(User user, string jwtKey, string issuer, string audience, int expiresInMinutes)
        {
            // Definiujemy roszczenia (claims), które będą zawarte w tokenie.
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId),
                new Claim(ClaimTypes.Email, user.Email)
            };

            // Tworzymy symetryczny klucz zabezpieczający token.
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Tworzymy token JWT z określonymi parametrami.
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
                signingCredentials: creds
            );

            // Zwracamy token jako ciąg znaków.
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
