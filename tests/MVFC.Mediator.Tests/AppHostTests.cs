namespace MVFC.Mediator.Tests;

public sealed class AppHostTests(AppHostFixture fixture) : IClassFixture<AppHostFixture>
{
    private readonly AppHostFixture _fixture = fixture;

    [Fact]
    public async Task Should_Create_User_Correctly()
    {
        // Arrange
        var command = new CreateUserCommand("Marcus Freitas", "marcus@test.com", 30);

        // Act
        var response = await _fixture.PlaygroundApi.PostUser(command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);        
        response.Content.Should().NotBeNull();
        response.Content!.Name.Should().Be(command.Name);
        response.Content!.Email.Should().Be(command.Email);
    }

    [Fact]
    public async Task Should_Get_User_Correctly()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var response = await _fixture.PlaygroundApi.GetUser(userId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Should().NotBeNull();
        response.Content!.Id.Should().Be(userId);
        response.Content!.Message.Should().Be("Usuário encontrado");
    }

    [Theory]
    [InlineData("", "marcus@test.com", 30, "O nome é obrigatório")]
    [InlineData("Ma", "marcus@test.com", 30, "O nome deve ter no mínimo 3 caracteres")]
    [InlineData("Marcus", "", 30, "O email é obrigatório")]
    [InlineData("Marcus", "invalid-email", 30, "Email inválido")]
    [InlineData("Marcus", "marcus@test.com", 17, "A idade deve ser maior ou igual a 18 anos")]
    [InlineData("Marcus", "marcus@test.com", 121, "A idade deve ser menor ou igual a 120 anos")]
    public async Task Should_Return_UnprocessableEntity_When_Validation_Fails(string name, string email, int age, string expectedError)
    {
        // Arrange
        var command = new CreateUserCommand(name, email, age);

        // Act
        var response = await _fixture.PlaygroundApi.PostUser(command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        response.Error.Should().NotBeNull();
        response.Error!.Content.Should().Contain(expectedError);
    }
}
