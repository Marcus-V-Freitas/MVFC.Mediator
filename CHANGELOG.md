# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.0.0] - 2026-03-19

### Added
- `IQuery<TResponse>` and `IQueryHandler<TQuery, TResponse>` — query support with the same
  zero-allocation dispatcher pattern used by commands
- `INotification` and `INotificationHandler<TNotification>` — fan-out publish support for
  multiple handlers via `NotificationDispatcher<T>`
- `IMediator.Query<TQuery, TResponse>()` and `IMediator.Publish<TNotification>()` methods
- `QueryDispatcher<TQuery, TResponse>` — static generic class with `Volatile.Read` +
  `Interlocked.CompareExchange` invoker cache, consistent with existing dispatchers
- `NotificationDispatcher<TNotification>` — optimised single-handler fast path, multi-handler
  fan-out on slow path
- Unit tests for `QueryTests` and `NotificationTests` covering happy path, validation,
  null guard and multi-handler fan-out scenarios
- BenchmarkDotNet benchmarks for `Query` and `Publish`

---

## [1.1.0] - 2025-11-16

### Added
- Multi-target support for `net9.0` and `net10.0`

---

## [1.0.1] - 2025-10-27

### Removed
- Test project excluded from NuGet package artifacts

---

## [1.0.0] - 2025-10-27

### Added
- Initial release
- `ICommand` and `ICommand<TResponse>` — base interfaces for void and typed commands
- `ICommandHandler<TCommand>` and `ICommandHandler<TCommand, TResponse>` — handler contracts
- `IMediator` — mediator contract with `Send` overloads
- `Mediator` — concrete implementation with DI-based handler resolution
- `VoidCommandDispatcher<TCommand>` — static generic dispatcher with `AggressiveInlining`,
  `Volatile.Read` and `Interlocked.CompareExchange` invoker cache
- `CommandDispatcherWithResponse<TCommand, TResponse>` — same pattern with typed response
- FluentValidation integration — single and multiple validator support with parallel execution
  on multiple validators
- `AddMediator()` extension method for `IServiceCollection`
- BenchmarkDotNet benchmarks: 1.364 μs (single request), 13.688 μs (10 parallel requests)

[2.0.0]: https://github.com/Marcus-V-Freitas/MVFC.Mediator/compare/v1.1.0...HEAD
[1.1.0]: https://github.com/Marcus-V-Freitas/MVFC.Mediator/compare/v1.0.1...v1.1.0
[1.0.1]: https://github.com/Marcus-V-Freitas/MVFC.Mediator/compare/v1.0.0...v1.0.1
[1.0.0]: https://github.com/Marcus-V-Freitas/MVFC.Mediator/releases/tag/v1.0.0
