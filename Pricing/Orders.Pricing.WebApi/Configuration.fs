module Orders.Pricing.WebApi.Configuration

open System.IO
open Microsoft.Extensions.Configuration

[<CLIMutable>]
type CosmosDbAzureConnection = {
    ConnectionString: string
    DatabaseName: string
    ContainerName: string
}

[<CLIMutable>]
type Configuration = {
    AzurePricingDb: CosmosDbAzureConnection
}

let fetchConfiguration =
    let builder = (new ConfigurationBuilder())
                      .SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("appsettings.json", true, true)
                      .AddEnvironmentVariables(); 
    let configurationRoot = builder.Build();
    configurationRoot.Get<Configuration>()