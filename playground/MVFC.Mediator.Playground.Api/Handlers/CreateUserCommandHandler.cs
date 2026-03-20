namespace MVFC.Mediator.Playground.Api.Handlers;

public sealed class CreateUserCommandHandler(ILogger<CreateUserCommandHandler> logger) : ICommandHandler<CreateUserCommand, CreateUserResponse>
{
    private readonly ILogger<CreateUserCommandHandler> _logger = logger;

    public async ValueTask<CreateUserResponse> Handle(CreateUserCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        _logger.LogCreatingUser(command.Name);

        var userId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;

        await Task.Delay(TimeSpan.FromMilliseconds(1), cancellationToken).ConfigureAwait(false);

        return new CreateUserResponse(userId, command.Name, command.Email, createdAt);
    }
}
