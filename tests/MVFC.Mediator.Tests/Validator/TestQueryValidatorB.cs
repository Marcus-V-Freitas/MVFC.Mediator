namespace MVFC.Mediator.Tests.Validator;

public sealed class TestQueryValidatorB : AbstractValidator<TestQuery>
{
    public TestQueryValidatorB()
    {
        RuleFor(x => x.Id).NotEqual("forbidden").WithMessage("Id is forbidden.");
    }
}
