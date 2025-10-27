namespace MVFC.Mediator.ApiExample.Models;

public sealed record class CreateUserResponse(Guid UserId, string Name, string Email, DateTime CreatedAt);