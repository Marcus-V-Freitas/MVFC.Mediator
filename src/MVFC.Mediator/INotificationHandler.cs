namespace MVFC.Mediator;

/// <summary>
/// Interface para handlers de notificações.
/// </summary>
/// <typeparam name="TNotification">Tipo da notificação</typeparam>
public interface INotificationHandler<in TNotification> where TNotification : INotification
{
    public ValueTask Handle(TNotification notification, CancellationToken cancellationToken = default);
}
