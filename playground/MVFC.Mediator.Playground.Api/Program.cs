var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddMediator();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

app.MapDefaultEndpoints();
app.UseExceptionHandler();

await app.RunAsync().ConfigureAwait(false);
