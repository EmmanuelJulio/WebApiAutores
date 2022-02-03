using WebApiAutores;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();
var servicioLogger = (ILogger < Startup >) app.Services.GetService(typeof(ILogger<Startup>));
startup.configure(app,app.Environment,servicioLogger);

app.Run();
