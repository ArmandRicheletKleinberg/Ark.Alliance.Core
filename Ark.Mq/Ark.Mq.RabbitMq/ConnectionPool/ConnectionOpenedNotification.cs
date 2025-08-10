using Ark.Alliance.Core.Notifications;

namespace Ark.Net.RabbitMq;

/// <summary>
/// Notification raised when a RabbitMQ connection is opened.
/// </summary>
/// <param name="Host">Target host of the connection.</param>
public sealed record ConnectionOpenedNotification(string Host) : INotification;
