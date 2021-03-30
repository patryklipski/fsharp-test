namespace Orders.Pricing.WebApi.Controllers

open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open Orders.Pricing.WebApi.Client.Contracts.Types
open Orders.Pricing.WebApi.PricingDatabase
open Orders.Pricing.WebApi.Configuration

[<ApiController>]
[<Route("[controller]")>]
type PricingController (logger : ILogger<PricingController>) =
    inherit ControllerBase()
    
    [<HttpGet>]
    [<Route("{productCode}")>]
    member this.GetProductPrice(productCode: string) : IActionResult =
        let getProductPrice =
            cosmosDatabaseProductPriceQuery fetchConfiguration
        
        let productPriceResult
            = getProductPrice (ProductCode productCode)
        
        match productPriceResult with
        | Ok productPrice ->
            this.Ok productPrice
            :> IActionResult
        | Error error ->
            this.BadRequest error
            :> IActionResult
        
