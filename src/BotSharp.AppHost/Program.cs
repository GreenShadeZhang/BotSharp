var builder = DistributedApplication.CreateBuilder(args);


// Add a parameter
var pAdmin = builder.AddParameter("postgres-admin");
var admin = builder.AddParameter("admin");
var password = builder.AddParameter("admin-password", secret: true);

var qdrantApikey = builder.AddParameter("QdrantApikey", secret: true);

var postgres = builder
    .AddPostgres("postgresql", pAdmin, password, port: 5432)
    .WithImageTag("17-alpine3.21")
    .WithDataVolume("postgres17_data")
    .WithPgAdmin(
       c => c.WithImage("dpage/pgadmin4")
             .WithImageTag("9.4")
             .WithHostPort(5050)
    );

var qdrant = builder
    .AddQdrant("qdrant", qdrantApikey, grpcPort: 53751, httpPort: 53750)
    .WithDataVolume("qdrant_data");


var botsharpDb = postgres.AddDatabase("botsharp");

var apiService = builder.AddProject<Projects.WebStarter>("apiservice")
   .WithReference(botsharpDb) // Add a mysql reference
   .WithReference(qdrant)
   .WithExternalHttpEndpoints();

var mcpService = builder.AddProject<Projects.BotSharp_PizzaBot_MCPServer>("mcpservice")
   .WithExternalHttpEndpoints();

builder.AddNpmApp("BotSharpUI", "../../../BotSharp-UI")
    .WithReference(apiService)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();

