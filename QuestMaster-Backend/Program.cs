using QuestMaster_Backend.StartupSettingsExtensions;

var builder = WebApplication.CreateBuilder(args);

// Rejestracja usług
builder.Services.AddCustomServices(builder.Configuration);

var app = builder.Build();

// Konfiguracja middleware
app.UseCustomMiddleware();

app.Run();