namespace MVFC.Mediator.Playground.Api.Extensions;

public static partial class LogDefinitions
{
    [LoggerMessage(LogLevel.Information, "Creating user with name {Name}")]
    public static partial void LogCreatingUser(this ILogger logger, string name);
}
