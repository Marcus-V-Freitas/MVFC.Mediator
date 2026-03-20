namespace MVFC.Mediator.Tests.Models;

public sealed record class CreateUserCommand(string Name, string Email, int Age) : ICommand<CreateUserResponse>;
