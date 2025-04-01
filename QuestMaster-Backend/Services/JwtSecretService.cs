using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

namespace QuestMaster_Backend.Services
{
    public class JwtSecretService
    {
        private readonly IConfiguration _configuration;
        public JwtSecretService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> GetJwtSecretAsync()
        {
            string secretName = _configuration["Jwt:SecretName"]!;
            string region = _configuration["AWS:Region"]!;

            using var client = new AmazonSecretsManagerClient(Amazon.RegionEndpoint.GetBySystemName(region));
            var request = new GetSecretValueRequest { SecretId = secretName };
            var response = await client.GetSecretValueAsync(request);
            
            if (!string.IsNullOrEmpty(response.SecretString))
            {
                var secretData = JsonSerializer.Deserialize<Dictionary<string, string>>(response.SecretString)!;
                return secretData["JwtKey"];
            }

            throw new Exception("Nie udało się pobrać sekretu JWT.");
        }
    }
}