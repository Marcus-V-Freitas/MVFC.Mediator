namespace MVFC.Mediator.Playground.Api.Models;

public sealed record class CreateUserResponse(Guid UserId, string Name, string Email, DateTimeOffset CreatedAt);
