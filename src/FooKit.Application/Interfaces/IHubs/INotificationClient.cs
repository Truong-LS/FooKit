namespace FooKit.Application.Interfaces.IHubs;

/// <summary>
/// Defines methods the server can invoke on connected SignalR clients.
/// This is the strongly-typed hub contract for notification-related events.
/// </summary>
public interface INotificationClient
{
    /// <summary>
    /// Sends a notification message to the client.
    /// </summary>
    Task ReceiveNotification(string message);

    /// <summary>
    /// Sends a general message from a specific user to the client.
    /// </summary>
    Task ReceiveMessage(string user, string message);
}
