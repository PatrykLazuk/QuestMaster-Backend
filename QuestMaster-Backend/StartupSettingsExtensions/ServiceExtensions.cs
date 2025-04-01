using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using QuestMaster_Backend.Services;

namespace QuestMaster_Backend.StartupSettingsExtensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Rejestracja kontrolerów i SignalR
            services.AddControllers();
            services.AddSignalR();

            // Rejestracja Swagger
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            // Rejestracja CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowAnyOrigin();
                });
            });

            // Rejestracja AWS DynamoDB
            services.AddAWSService<IAmazonDynamoDB>();
            services.AddSingleton<IDynamoDBContext, DynamoDBContext>();

            // Rejestracja serwisu do pobierania sekretu JWT
            services.AddSingleton<JwtSecretService>();

            // Pobranie sekretu JWT
            var jwtSecretService = services.BuildServiceProvider().GetRequiredService<JwtSecretService>();
            var jwtSecret = jwtSecretService.GetJwtSecretAsync().GetAwaiter().GetResult();

            // Dodajemy klucz do konfiguracji, aby był dostępny globalnie
            var inMemorySettings = new Dictionary<string, string>
            {
                {"Jwt:Key", jwtSecret}
            };
            ((IConfigurationBuilder)configuration).AddInMemoryCollection((IEnumerable<KeyValuePair<string, string?>>)inMemorySettings);

            var jwtIssuer = configuration["Jwt:Issuer"];
            var jwtAudience = configuration["Jwt:Audience"];
            var jwtKey = Encoding.UTF8.GetBytes(jwtSecret);

            // Konfiguracja uwierzytelniania JWT
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(jwtKey)
                };
            });

            return services;
        }
    }
}
