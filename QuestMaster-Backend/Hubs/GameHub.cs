using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace QuestMaster_Backend.Hubs
{
    [Authorize]
    public class GameHub : Hub
    {
        // Metoda wywoływana przez klienta do rzutów kośćmi.
        public async Task RollDice(string diceType)
        {
            // Przykładowa logika: przekształć "d20" w liczbę 20 i wygeneruj losowy wynik
            int sides = int.Parse(diceType.TrimStart('d'));
            int result = new Random().Next(1, sides + 1);

            // Wyślij wynik do wszystkich połączonych klientów
            await Clients.All.SendAsync("DiceRolled", new { diceType, result, userId = Context.UserIdentifier });
        }

        // Możesz dodać metody OnConnectedAsync oraz OnDisconnectedAsync dla zarządzania połączeniami
        public override async Task OnConnectedAsync()
        {
            // Możesz np. przypisać klienta do konkretnej grupy
            await base.OnConnectedAsync();
        }
    }
}