# MVFC.Mediator

> 🇺🇸 [Read in English](README.md)

[![CI](https://github.com/Marcus-V-Freitas/MVFC.Mediator/actions/workflows/ci.yml/badge.svg)](https://github.com/Marcus-V-Freitas/MVFC.Mediator/actions/workflows/ci.yml)
[![codecov](https://codecov.io/gh/Marcus-V-Freitas/MVFC.Mediator/branch/main/graph/badge.svg)](https://codecov.io/gh/Marcus-V-Freitas/MVFC.Mediator)
[![License](https://img.shields.io/badge/license-Apache--2.0-blue)](LICENSE)
![Platform](https://img.shields.io/badge/.NET-9%20%7C%2010-blue)
![NuGet Version](https://img.shields.io/nuget/v/MVFC.Mediator)

Uma implementação de alto desempenho e alocação zero do padrão Mediator para .NET 9 e 10. Centralize sua lógica de negócio, desacople componentes e escale com confiança.

## Motivação

Em aplicações .NET complexas, dependências diretas entre serviços geralmente levam a:

- **Acoplamento Forte**: Componentes difíceis de testar e manter.
- **Boilerplate**: Lógica repetitiva para validação e orquestração.
- **Baixa Escalabilidade**: Dificuldade em evoluir sistemas à medida que crescem.
- **Sobrecarga de Desempenho**: Bibliotecas comuns costumam introduzir alocações ocultas e custos de reflexão (reflection).

**MVFC.Mediator** resolve isso fornecendo um mediator leve e ultra-rápido com um padrão de dispatcher de alocação zero. Ele utiliza cache genérico estático e técnicas avançadas de IL para garantir que o roteamento de mensagens seja tão rápido quanto uma chamada de método direta.

## Arquitetura

- **Dispatcher de Alocação Zero**: Utiliza `Volatile.Read` e `Interlocked` para invocação de handlers segura para threads e sem reflexão.
- **Integração Fluida**: Conecta-se perfeitamente com `Microsoft.Extensions.DependencyInjection`.
- **Validação Integrada**: Suporte nativo para `FluentValidation` com execução paralela para múltiplos validadores.
- **Suporte a Fan-out**: Sistema de notificações nativo para transmitir eventos para múltiplos handlers.

---

## Instalação

Instale o pacote via NuGet:

```sh
dotnet add package MVFC.Mediator
```

## Início Rápido

### 1. Registro com Injeção de Dependência

```csharp
builder.Services.AddMediator();
```

### 2. Comando Básico (Sem Resposta)

```csharp
// Defina o comando
public record DeleteUserCommand(Guid UserId) : ICommand;

// Implemente o handler
public class DeleteUserHandler : ICommandHandler<DeleteUserCommand>
{
    public ValueTask Handle(DeleteUserCommand command, CancellationToken ct)
    {
        // Lógica para deletar o usuário
        return ValueTask.CompletedTask;
    }
}

// Uso
await mediator.Send(new DeleteUserCommand(uid));
```

### 3. Comando com Resposta e Validação

```csharp
// Modelo de resposta
public record UserCreated(Guid Id);

// Comando com TResponse
public record CreateUser(string Name, string Email) : ICommand<UserCreated>;

// Validador (executado automaticamente se registrado)
public class CreateUserValidator : AbstractValidator<CreateUser>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MinimumLength(3);
        RuleFor(x => x.Email).EmailAddress();
    }
}

// Handler
public class CreateUserHandler : ICommandHandler<CreateUser, UserCreated>
{
    public ValueTask<UserCreated> Handle(CreateUser command, CancellationToken ct)
    {
        return new ValueTask<UserCreated>(new UserCreated(Guid.NewGuid()));
    }
}

// Uso
var result = await mediator.Send<CreateUser, UserCreated>(new CreateUser("John", "john@test.com"));
```

### 4. Query (Lógica de Leitura)

```csharp
public record GetUserQuery(Guid Id) : IQuery<UserDto>;

public class GetUserHandler : IQueryHandler<GetUserQuery, UserDto>
{
    public async ValueTask<UserDto> Handle(GetUserQuery query, CancellationToken ct)
    {
        return await _repository.GetById(query.Id);
    }
}

// Uso
var user = await mediator.Query<GetUserQuery, UserDto>(new GetUserQuery(uid));
```

### 5. Notificações (Broadcasting de Eventos)

```csharp
public record UserRegistered(Guid Id) : INotification;

// Múltiplos handlers para a mesma notificação
public class LoggingHandler : INotificationHandler<UserRegistered> { ... }
public class WelcomeEmailHandler : INotificationHandler<UserRegistered> { ... }

// Uso (transmitindo para todos os handlers)
await mediator.Publish(new UserRegistered(uid));
```

---

## Desempenho

Construído para cenários de alto rendimento, benchmarked no .NET 9.

```text

BenchmarkDotNet v0.15.4, Windows 11 (10.0.26200.6901)
12th Gen Intel Core i5-12500H 2.50GHz, 1 CPU, 16 logical and 12 physical cores
.NET SDK 9.0.302
  [Host]     : .NET 9.0.7 (9.0.7, 9.0.725.31616), X64 RyuJIT x86-64-v3
  Job-NTRUNJ : .NET 9.0.7 (9.0.7, 9.0.725.31616), X64 RyuJIT x86-64-v3

IterationCount=5  WarmupCount=3  

```
| Method                                  | Mean      | Error     | StdDev    | Gen0   | Allocated |
|---------------------------------------- |----------:|----------:|----------:|-------:|----------:|
| 'Simula request HTTP individual'        |  1.364 μs | 0.2325 μs | 0.0604 μs | 0.4272 |   3.97 KB |
| 'Simula múltiplos requests em paralelo' | 13.688 μs | 2.2724 μs | 0.3517 μs | 4.3335 |  40.15 KB |


## Referência da API

| Interface | Método | Caso de Uso |
|---|---|---|
| `ICommand` | `Send(command)` | Ação sem retorno (efeitos colaterais). |
| `ICommand<T>` | `Send<C, T>(command)` | Ação com retorno. |
| `IQuery<T>` | `Query<Q, T>(query)` | Recuperação de dados. |
| `INotification` | `Publish(event)` | Transmissão de eventos (um para muitos). |

## Estrutura do Projeto

```text
src/
  MVFC.Mediator/      # Lógica central e Dispatchers
playground/
  Playground.Api/     # Exemplos de uso e testes de integração
tests/
  MVFC.Mediator.Tests # Testes unitários e de arquitetura
```

---

## Requisitos

- .NET 9 ou .NET 10

---

## Contribuindo

Veja [CONTRIBUTING.md](CONTRIBUTING.md).

---

## Licença

[Apache-2.0](LICENSE)
