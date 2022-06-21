using Flagship.Infrastructure.Extension.Container;
using Flagship.Infrastructure.Extension.ExceptionHandling;
using Flagship.Infrastructure.Extension.Security;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();
try {
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    builder.Services.AddCors(corsOptions => corsOptions.AddDefaultPolicy(policy => policy.WithOrigins(builder.Configuration["OriginConfiguration:AllowOrigins"]).AllowAnyHeader().AllowAnyMethod()));
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddMemoryCache();
    builder.Services.AddSingleton(builder.Configuration);
    DependencyContainer.RegisterServices(builder.Services);
    Authentication.AddAuthenication(builder.Services, builder.Configuration);

    var app = builder.Build();

    app.UseSerilogRequestLogging(configure => {
        configure.MessageTemplate = "HTTP {RequestMethod} {RequestPath} ({UserId}) responded {StatusCode} in {Elapsed:0.0000}ms";
    });
    app.UseMiddleware<GlobalExceptionHandling>();
    if (app.Environment.IsDevelopment()) {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();
    app.Run();
}
catch (Exception exception) {
    Log.Fatal(exception, "Host terminated unexpectedly");
    return 1;
}
finally {
    Log.CloseAndFlush();
}
return 0;