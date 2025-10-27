namespace MVFC.Mediator.ApiExample.Models;

public sealed record class CreateUserCommand(string Name, string Email, int Age) : ICommand<CreateUserResponse>;