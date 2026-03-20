namespace MVFC.Mediator.Tests.Services;

internal interface IPlaygroundApiClient
{
    [Post("/api/users")]
    internal Task<ApiResponse<CreateUserResponse>> PostUser([Body] CreateUserCommand command);

    [Get("/api/users/{id}")]
    internal Task<ApiResponse<GetUserResponse>> GetUser(Guid id);
}
