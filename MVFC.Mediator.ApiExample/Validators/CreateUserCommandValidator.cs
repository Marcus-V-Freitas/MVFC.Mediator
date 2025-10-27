namespace MVFC.Mediator.ApiExample.Validators;

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome é obrigatório")
            .MinimumLength(3).WithMessage("O nome deve ter no mínimo 3 caracteres")
            .MaximumLength(100).WithMessage("O nome deve ter no máximo 100 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O email é obrigatório")
            .EmailAddress().WithMessage("Email inválido");

        RuleFor(x => x.Age)
            .GreaterThanOrEqualTo(18).WithMessage("A idade deve ser maior ou igual a 18 anos")
            .LessThanOrEqualTo(120).WithMessage("A idade deve ser menor ou igual a 120 anos");
    }
}