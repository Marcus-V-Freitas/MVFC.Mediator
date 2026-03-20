namespace MVFC.Mediator.Tests.Validator;

public sealed class TestCommandValidatorB : AbstractValidator<TestCommand>
{
    public TestCommandValidatorB()
    {
        RuleFor(x => x.Value).NotEqual("forbidden").WithMessage("Value is forbidden.");
    }
}
