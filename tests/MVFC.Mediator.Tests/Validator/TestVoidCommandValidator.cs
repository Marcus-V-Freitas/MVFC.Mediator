namespace MVFC.Mediator.Tests.Validator;

public class TestVoidCommandValidator : AbstractValidator<TestVoidCommand>
{
    public TestVoidCommandValidator()
    {
        RuleFor(x => x.Value).MinimumLength(3).WithMessage("Value must be at least 3 characters");
    }
}
