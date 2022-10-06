open System
open Microsoft.AspNetCore.Builder
open System.Threading.Tasks
open Messages
open Microsoft.AspNetCore.Http
open MassTransit
open Microsoft.Extensions.DependencyInjection

// module Lib =
//     open Rebus.Routing.TypeBased
//     open Messages
//     open Rebus.Config
//     open Microsoft.Extensions.Configuration

//     let configure (rebus: RebusConfigurer) (config: IConfiguration) =
//         let rmqConnection = config.GetConnectionString("RabbitMqConnection")
//         rebus.Logging(fun l -> l.Console()) |> ignore

//         rebus.Routing (fun r ->
//             r.TypeBased().Map<OnboardNewCustomer>("MainQueue")
//             |> ignore)
//         |> ignore

//         rebus.Transport (fun t ->
//             t.UseRabbitMq(rmqConnection, "MainQueue")
//             |> ignore)
//         |> ignore

//         rebus

[<EntryPoint>]
let main args =
  
    let builder = WebApplication.CreateBuilder(args)

    builder.Services.AddMassTransit (fun (x: IBusRegistrationConfigurator) ->
        x.UsingRabbitMq (fun context cfg ->
            cfg.Host(
                "localhost",
                "/",
                fun h ->
                    h.Username("guest") |> ignore
                    h.Password("guest") |> ignore
            )

            cfg.ConfigureEndpoints(context, KebabCaseEndpointNameFormatter.Instance)))
    |> ignore

    builder.Services.AddOptions<MassTransitHostOptions>()
      .Configure(fun options ->
                        options.WaitUntilStarted <- true
      ) |> ignore

    let app = builder.Build()

    app.MapGet("/", Func<string>(fun () -> "Hello World!"))
    |> ignore

    app.MapGet(
        "/hello",
        Func<IPublishEndpoint, Task<IResult>> (fun bus ->
            task {
                let message = OnboardNewCustomer.For "Jesse" "j.franceschini@gmail.com"
                do! bus.Publish(message) |> Async.AwaitTask
                return Results.Ok(message)
            })
    )
    |> ignore

    app.RunAsync() |> Async.AwaitTask |> Async.RunSynchronously

    0 // Exit code
