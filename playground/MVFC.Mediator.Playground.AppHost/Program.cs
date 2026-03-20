var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.MVFC_Mediator_Playground_Api>("api");

await builder.Build().RunAsync().ConfigureAwait(false);
