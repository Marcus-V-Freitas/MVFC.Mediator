# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [3.4.0] - 2026-11-01

### Added

- Added `global.json` pinning the .NET 10 SDK version and configuring `Microsoft.Testing.Platform`.
- Added `dotnet-reportgenerator-globaltool` and `dotnet-coverage` to `.config/dotnet-tools.json` local tools manifest.
- Added `<IsTestProject>true</IsTestProject>` and configured analyzer/runtime asset metadata in `MVFC.Mediator.Tests.csproj`.
- Added OIDC authentication via `NuGet/login@v1` for publishing packages to NuGet.org.
- Added strict tag ancestry validation (`origin/main`) and concurrency controls in CI/CD workflow.

### Changed

- Renamed `Mediator` class to `MediatorService` and updated DI registration in `MediatorExtensions`.
- Updated `build.cake` to run tools via `dotnet tool run` instead of local tool installations.
- Replaced `SonarAnalyzer.CSharp` with `MVFC.Pack.Analyzers` in `Directory.Build.targets`.
- Enabled central package transitive pinning (`CentralPackageTransitivePinningEnabled`).
- Updated NuGet package dependencies (`FluentValidation` 12.2.0, `MinVer` 8.0.0, `Aspire` 13.6.4, `xunit.v3` 4.0.2, `Microsoft.NET.Test.Sdk` 18.11.0, `Microsoft.Extensions.DependencyInjection` 10.0.14).
- Renamed `DefaultEndpoints.cs` to `EndpointsExtensions.cs` in playground API.
- Code cleanup in command/query dispatchers removing redundant null-forgiving operators and clarifying exchange comparand.

### Removed

- Removed obsolete `cleanup-tag-on-failure` job in GitHub Actions CI workflow.

## [3.3.0] - 2026-06-24

### Changed

- Updated NuGet dependencies to their newer versions.

## [3.2.0] - 2026-04-05

### Added

- Rollback mechanism implementation in the CI pipeline.

### Changed

- Modernized project icon for a refreshed aesthetic.

## [3.1.0] - 2026-04-01

### Changed

- Standardized MSBuild file extensions and naming (`Directory.Build.props`, `Directory.Build.targets`).
- Enabled `latest` language version and recommended analysis mode across all projects.
- Disabled diagnostic S2326 (marker interfaces) in `.editorconfig`.
- Replaced `DateTime` with `DateTimeOffset` in `CreateUserResponse` and `CreateUserCommandHandler`.
- Refactored unit test handlers to encapsulate static call counts.
- Minor naming adjustments (e.g., `cancellationToken` instead of `ct` in `ValidationExceptionHandler`).

## [3.0.0] - 2026-04-01

### Added

- Integrated **MinVer** for automatic Semantic Versioning based on Git tags.
- Configured `MinVerTagPrefix` as `v` to align with existing project tagging conventions.

### Changed

- Simplified GitHub Actions CI workflow by removing manual version extraction and project patching scripts.
- Optimized MSBuild property functions in `Directory.Build.Props` for more robust project categorization and exclusion (e.g., playground projects).
- Improved code coverage reporting by explicitly excluding `tests` and `playground` projects in `codecov.yml` and `coverage.runsettings`.

## [2.0.2] - 2026-03-21

### Changed

- CI/CD workflow refinements for automated publishing and coverage reporting
- Minor adjustments to Codecov configuration for status checks precision

---

## [2.0.1] - 2026-03-19

### Added

- Comprehensive multi-language documentation (English and Portuguese) with detailed usage examples

---

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
- Full BenchmarkDotNet results in project READMEs, including environment specifications
- Validation integration tests in `AppHostTests.cs` covering multiple failure scenarios for `CreateUserCommand`

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

[3.4.0]: https://github.com/Marcus-V-Freitas/MVFC.Mediator/compare/v3.3.0...v3.4.0
[3.3.0]: https://github.com/Marcus-V-Freitas/MVFC.Mediator/compare/v3.2.0...v3.3.0
[3.2.0]: https://github.com/Marcus-V-Freitas/MVFC.Mediator/compare/v3.1.0...v3.2.0
[3.1.0]: https://github.com/Marcus-V-Freitas/MVFC.Mediator/compare/v3.0.0...v3.1.0
[3.0.0]: https://github.com/Marcus-V-Freitas/MVFC.Mediator/compare/v2.0.2...HEAD
[2.0.2]: https://github.com/Marcus-V-Freitas/MVFC.Mediator/compare/v2.0.1...HEAD
[2.0.1]: https://github.com/Marcus-V-Freitas/MVFC.Mediator/compare/v2.0.0...v2.0.1
[2.0.0]: https://github.com/Marcus-V-Freitas/MVFC.Mediator/compare/v1.1.0...v2.0.0
[1.1.0]: https://github.com/Marcus-V-Freitas/MVFC.Mediator/compare/v1.0.1...v1.1.0
[1.0.1]: https://github.com/Marcus-V-Freitas/MVFC.Mediator/compare/v1.0.0...v1.0.1
[1.0.0]: https://github.com/Marcus-V-Freitas/MVFC.Mediator/releases/tag/v1.0.0
