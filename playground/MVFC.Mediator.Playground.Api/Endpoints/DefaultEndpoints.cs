namespace MVFC.Mediator.Playground.Api.Endpoints;

public static class EndpointsExtensions
{
    public static IEndpointRouteBuilder MapDefaultEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/users", async (IMediator mediator, CreateUserCommand command, CancellationToken ct) =>
        {
            var response = await mediator.Send<CreateUserCommand, CreateUserResponse>(command, ct).ConfigureAwait(false);
            return Results.Created($"/api/users/{response.UserId}", response);
        })
        .Produces<CreateUserResponse>(StatusCodes.Status201Created)
        .Produces<ProblemDetails>(StatusCodes.Status422UnprocessableEntity);

        endpoints.MapGet("/api/users/{id:guid}", (Guid id) =>
            Results.Ok(new GetUserResponse(id, "Usuário encontrado")));

        return endpoints;
    }
}
