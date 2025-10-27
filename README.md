# MVFC.Mediator

MVFC.Mediator é uma biblioteca .NET que implementa o padrão Mediator, promovendo a comunicação desacoplada entre componentes de uma aplicação. O Mediator centraliza o envio de comandos e eventos, facilitando a manutenção e evolução do código.

## Visão Geral

O padrão Mediator permite que objetos se comuniquem sem depender diretamente uns dos outros, reduzindo o acoplamento e tornando o sistema mais modular. MVFC.Mediator abstrai essa comunicação por meio de interfaces e handlers.

## Principais Componentes

- **ICommand**: Interface base para comandos, representando uma solicitação de ação.
- **ICommandHandler**: Interface para handlers responsáveis por processar comandos específicos.
- **IMediator**: Interface que define o contrato para envio de comandos e publicação de eventos.
- **Mediator**: Implementação concreta do IMediator, responsável por resolver e invocar os handlers.

## Recursos

- Suporte à injeção de dependências para handlers
- Facilidade de integração em projetos .NET (Console, WebAPI, etc.)
- Extensível para eventos, notificações e queries
- Benchmarks e testes automatizados

## Instalação

Adicione o projeto como referência em sua solução ou instale via NuGet (se disponível):

```shell
dotnet add package MVFC.Mediator
```

## Exemplo de Uso

Adicionar injeção de independência

```csharp
builder.Services.AddMediator();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
```

Definir o Retorno

```csharp
public sealed record class CreateUserResponse(Guid UserId, string Name, string Email, DateTime CreatedAt);
```

Defina um comando:

```csharp
public sealed record class CreateUserCommand(string Name, string Email, int Age) : 
    ICommand<CreateUserResponse>;
```

Implemente o handler:

```csharp
public sealed class CreateUserCommandHandler(ILogger<CreateUserCommandHandler> logger) : 
ICommandHandler<CreateUserCommand, CreateUserResponse>
{
    private readonly ILogger<CreateUserCommandHandler> _logger = logger;

    public async ValueTask<CreateUserResponse> Handle(CreateUserCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Criando usuário: {Name}", command.Name);

        var userId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;

        await Task.Delay(TimeSpan.FromMilliseconds(1), cancellationToken);

        return new CreateUserResponse(userId, command.Name, command.Email, createdAt);
    }
}
```

Envie o comando via Mediator:

```csharp
endpoints.MapPost("/api/users", async (IMediator mediator, CreateUserCommand command, CancellationToken ct) =>
{
    var response = await mediator.Send<CreateUserCommand, CreateUserResponse>(command, ct);
    return Results.Created($"/api/users/{response.UserId}", response);
})
.Produces<CreateUserResponse>(StatusCodes.Status201Created)
.Produces<ProblemDetails>(StatusCodes.Status422UnprocessableEntity);
```

## Estrutura do Projeto

- [`ICommand`](ICommand.cs): Interface para comandos.
- [`ICommandHandler`](ICommandHandler.cs): Interface para handlers de comandos.
- [`IMediator`](IMediator.cs): Contrato do mediator.

## Testes e Benchmarks

Os testes e benchmarks estão no [Report](./BenchmarkDotNet.Artifacts/results/MVFC.Mediator.Tests.RealWorldScenarioBenchmark-report-github.md), garantindo performance e confiabilidade.

```

BenchmarkDotNet v0.15.4, Windows 11 (10.0.26200.6901)
12th Gen Intel Core i5-12500H 2.50GHz, 1 CPU, 16 logical and 12 physical cores
.NET SDK 9.0.302
  [Host]     : .NET 9.0.7 (9.0.7, 9.0.725.31616), X64 RyuJIT x86-64-v3
  Job-NTRUNJ : .NET 9.0.7 (9.0.7, 9.0.725.31616), X64 RyuJIT x86-64-v3

IterationCount=5  WarmupCount=3  

```
| Method                                  | Mean      | Error     | StdDev    | Gen0   | Allocated |
|---------------------------------------- |----------:|----------:|----------:|-------:|----------:|
| &#39;Simula request HTTP individual&#39;        |  1.364 μs | 0.2325 μs | 0.0604 μs | 0.4272 |   3.97 KB |
| &#39;Simula múltiplos requests em paralelo&#39; | 13.688 μs | 2.2724 μs | 0.3517 μs | 4.3335 |  40.15 KB |


## Licença

Este projeto está sob a licença MIT.