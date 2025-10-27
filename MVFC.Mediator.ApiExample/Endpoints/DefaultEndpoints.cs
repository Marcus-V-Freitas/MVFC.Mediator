namespace MVFC.Mediator.ApiExample.Endpoints;

public static class EndpointsExtensions
{
    public static IEndpointRouteBuilder MapDefaultEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/users", async (IMediator mediator, CreateUserCommand command, CancellationToken ct) =>
        {

            var response = await mediator.Send<CreateUserCommand, CreateUserResponse>(command, ct);
            return Results.Created($"/api/users/{response.UserId}", response);
        })
        .Produces<CreateUserResponse>(StatusCodes.Status201Created)
        .Produces<ProblemDetails>(StatusCodes.Status422UnprocessableEntity);

        endpoints.MapGet("/api/users/{id:guid}", (Guid id) => Results.Ok(new { id, message = "Usuário encontrado" }));

        return endpoints;
    }
}