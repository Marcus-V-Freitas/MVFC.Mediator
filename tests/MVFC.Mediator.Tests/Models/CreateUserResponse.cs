namespace MVFC.Mediator.Tests.Models;

public sealed record class CreateUserResponse(Guid UserId, string Name, string Email, DateTime CreatedAt);
