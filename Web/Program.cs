using Messages;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => OnboardNewCustomer.For("Jesse", "j.franceschini@gmail.com"));

app.Run();
