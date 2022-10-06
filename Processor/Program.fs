namespace Processor

open System.Threading.Tasks
open MassTransit
open Messages
open System
open System.Reflection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection

type SubmitOrderConsumer() =
    interface IConsumer<OnboardNewCustomer> with
        member this.Consume(context: ConsumeContext<OnboardNewCustomer>) =
            task { printfn "Onboarding Customer %s" context.Message.Name }


module Program =
    open System.Threading

    let assembly = Assembly.GetEntryAssembly()

    //let createHostBuilder args =
    //    Host
    //        .CreateDefaultBuilder(args)
    //        .ConfigureServices(fun hostContext services ->
    //            services.AddMassTransit (fun (x: IBusRegistrationConfigurator) ->
    //                x.UsingRabbitMq(fun context cfg ->
    //                  cfg.ReceiveEndpoint("event-listener", fun (e: IRabbitMqReceiveEndpointConfigurator) ->
    //                    e.Consumer<SubmitOrderConsumer>()
    //                  ))
    //                //x.AddConsumer<SubmitOrderConsumer>(typeof<SubmitOrderConsumer>) |> ignore
    //            ) |> ignore

    //            services.AddHostedService<Worker>() |> ignore

    //            services |> ignore)

    //    [<EntryPoint>]
//    let main args =
    //let builder = createHostBuilder (args)

    //let app = builder.Build()

    //app.RunAsync() |> Async.AwaitTask |> Async.RunSynchronously
    let busControl = Bus.Factory.CreateUsingRabbitMq(fun cfg ->
      cfg.ReceiveEndpoint("event-listener", fun (e: IRabbitMqReceiveEndpointConfigurator) ->
        e.Consumer<SubmitOrderConsumer>()
      )
    )
    let source = new CancellationTokenSource(TimeSpan.FromSeconds(10))

    busControl.StartAsync(source.Token) |> Async.AwaitTask |> Async.RunSynchronously |> ignore

    Console.WriteLine("Press enter to exit")

    try
      Task.Run(fun () -> Console.ReadLine())
      |> Async.AwaitTask
      |> Async.RunSynchronously
      |> ignore
    finally
      busControl.StopAsync() |> Async.AwaitTask |> Async.RunSynchronously |> ignore
// 0 // exit code
