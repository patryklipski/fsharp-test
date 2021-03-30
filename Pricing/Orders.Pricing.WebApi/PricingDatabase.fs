module Orders.Pricing.WebApi.PricingDatabase

open FSharp.Control
open FSharp.CosmosDb
open Orders.Pricing.WebApi.Client.Contracts.Types
open Orders.Pricing.WebApi.Configuration

type GetProductPriceError =
    | MoreThanOneElementError of string
    | NoProductFoundError of string

[<CLIMutable>]
type GetProductPriceQueryResult = {
    price: decimal
}

let cosmosDatabaseProductPriceQuery (appConfig:Configuration) (productCode:ProductCode) : Result<ProductPrice, GetProductPriceError> =  
    
    let rawProductCode =
        match productCode with
        | ProductCode x -> x
    
    let queryResults = 
        appConfig.AzurePricingDb.ConnectionString
        |> Cosmos.fromConnectionString
        |> Cosmos.database appConfig.AzurePricingDb.DatabaseName
        |> Cosmos.container appConfig.AzurePricingDb.ContainerName
        |> Cosmos.query "SELECT x.price FROM x WHERE x.id = @code"
        |> Cosmos.parameters ["@code", box rawProductCode]
        |> Cosmos.execAsync<GetProductPriceQueryResult>
        |> AsyncSeq.toListSynchronously
        
    match queryResults with
    | [] -> Error (NoProductFoundError $"Could not find product with code {productCode}")
    | [x] -> Ok (ProductPrice x.price)
    | head :: tail -> Error (MoreThanOneElementError $"There was more than one product with code {productCode}")
