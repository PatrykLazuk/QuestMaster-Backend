using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using QuestMaster_Backend.Hubs;

namespace QuestMaster_Backend.StartupSettingsExtensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder app)
        {
            // Konfiguracja środowiska deweloperskiego: Swagger tylko w development
            var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Wymuszamy przekierowanie do HTTPS
            app.UseHttpsRedirection();

            // Konfiguracja CORS
            app.UseCors("AllowAll");

            // Dodajemy middleware routingu. To konieczne, aby kolejne middleware, 
            // które opierają się na endpointach (np. autoryzacja), działały poprawnie.
            app.UseRouting();

            // Uwierzytelnianie i autoryzacja - middleware korzystające z routingu
            app.UseAuthentication();
            app.UseAuthorization();

            // Mapowanie endpointów za pomocą UseEndpoints.
            // Tutaj mapujemy kontrolery oraz SignalR Hub.
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<GameHub>("/gamehub");
            });

            return app;
        }
    }
}
