using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;

namespace GoFoodBeverage.Services.Hubs
{
    public class KitchenSessionHub : Hub
    {
        private readonly IDictionary<string, UserConnectionModel> _connections;

        public KitchenSessionHub(IDictionary<string, UserConnectionModel> connections)
        {
            _connections = connections;
        }
        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnectionModel userConnection))
            {
                _connections.Remove(Context.ConnectionId);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public async Task JoinRoom(UserConnectionModel userConnection)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"{userConnection.BranchId}");
        }
    }
}
