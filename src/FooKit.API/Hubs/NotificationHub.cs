using Microsoft.AspNetCore.SignalR;
using MyProject.Application.Interfaces.IHubs;

namespace MyProject.API.Hubs;

/// <summary>
/// Sample notification hub. Extend with additional methods as needed.
/// Uncomment [Authorize] when authentication is required for connections.
/// </summary>
// [Authorize]
public class NotificationHub : Hub<INotificationClient>
{
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Broadcasts a message from a user to all connected clients.
    /// </summary>
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.ReceiveMessage(user, message);
    }
}
