using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System.Net;
using UserWebAPI.Helpers;
using UserWebAPI.Interfaces;
using UserWebAPI.Models;
using UserWebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

try
{
    builder.Configuration.AddEnvironmentVariables();

    builder.Logging.ClearProviders();
    builder.Logging.AddSerilog(CreateSerilogLogger(builder.Configuration));

    // Read JwtSettings from environment
    builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));


    builder.Services.AddControllers();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddDbContext<UserDbContext>(options =>
    {
        var connectionString = builder.Configuration["ConnectionStrings:MariaDB"];
        options.UseMySql(
            connectionString,
            ServerVersion.AutoDetect(connectionString)
            );
    });

    builder.Services.AddTransient<IAuthenticatorService, AuthenticatorService>();
    builder.Services.AddTransient<IUserService, UserService>();
}
catch (Exception ex)
{
    Log.Error(ex, "Initialization error.");
}

var app = builder.Build();

// Global Exception Handler Middleware
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "applicaton/json";

        var ex = context.Features.Get<IExceptionHandlerFeature>();
        if (ex != null)
        {
            var response = new GeneralResponse
            {
                Code = ex.Error.GetType().Name,
                Message = ex.Error.Message,
                Data = builder.Environment.IsDevelopment() ? ex.Error : ex.Error.Source
            };

            await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }
    });
});

// Create database
app.InitiateDatabase();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.

//app.UseHttpsRedirection();

app.MapControllers();

app.Run();

Serilog.Core.Logger CreateSerilogLogger(IConfiguration config)
{
    Serilog.Core.Logger logger;

    if (config["Logger:Name"]!.ToLower() == "elasticsearch")
    {
        var uri = new Uri(config["Logger:Url"]!);

        logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(config)
                    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(uri))
                    .CreateLogger();
    }
    else
    {
        logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(config)
                    .WriteTo.Console()
                    .CreateLogger();
    }

    return logger;
}

