namespace MVFC.Mediator.Tests.Validator;

public sealed class TestCommandValidator : AbstractValidator<TestCommand>
{
    public TestCommandValidator()
    {
        RuleFor(x => x.Value)
            .NotEmpty()
            .MinimumLength(3);
    }
}