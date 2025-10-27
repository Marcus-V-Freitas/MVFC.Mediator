namespace MVFC.Mediator.ApiExample.Handlers;

public sealed class CreateUserCommandHandler(ILogger<CreateUserCommandHandler> logger) : ICommandHandler<CreateUserCommand, CreateUserResponse>
{
    private readonly ILogger<CreateUserCommandHandler> _logger = logger;

    public async ValueTask<CreateUserResponse> Handle(CreateUserCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Criando usuário: {Name}", command.Name);

        var userId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;

        await Task.Delay(TimeSpan.FromMilliseconds(1), cancellationToken);

        return new CreateUserResponse(userId, command.Name, command.Email, createdAt);
    }
}