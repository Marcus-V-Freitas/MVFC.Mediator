namespace MVFC.Mediator.Tests.Validator;

public class TestVoidCommandValidatorB : AbstractValidator<TestVoidCommand>
{
    public TestVoidCommandValidatorB()
    {
        RuleFor(x => x.Value).Must(v => !v.Contains("forbidden", StringComparison.Ordinal)).WithMessage("Value is forbidden");
    }
}
