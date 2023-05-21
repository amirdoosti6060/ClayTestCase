using DoorWebAPI.Helpers;
using DoorWebAPI.Interfaces;
using DoorWebAPI.Models;
using DoorWebAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RabbitMQServiceLib;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System.Net;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
try
{
    builder.Configuration.AddEnvironmentVariables();

    builder.Logging.ClearProviders();
    builder.Logging.AddSerilog(CreateSerilogLogger(builder.Configuration));

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                ValidAudience = builder.Configuration["JwtSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
            };
        });

    builder.Services.AddDbContext<DoorDbContext>(options =>
    {
        bool connected = false;

        while (!connected)
        {
            try
            {
                var connectionString = builder.Configuration["ConnectionStrings:MariaDB"];
                options.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString)
                    );
                connected = true;
            }
            catch
            {
                Task.Delay(1000);
            }
        }
    });

    builder.Services.AddSingleton(sp =>
        new RabbitBusBuilder()
            .HostName(builder.Configuration["RabbitMQ:Hostname"])
            .UserName(builder.Configuration["RabbitMQ:Username"])
            .Password(builder.Configuration["RabbitMQ:Password"])
            .build()
    );

    builder.Services.Configure<LockHandlerSettings>(builder.Configuration.GetSection("LockHandler"));
    builder.Services.AddTransient<IPermissionService, PermissionService>();
    builder.Services.AddTransient<IDoorService, DoorService>();

    builder.Services.AddControllers();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}
catch (Exception ex)
{
    Log.Error(ex, "Initialization error.");
}

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "applicaton/json";

        var ex = context.Features.Get<IExceptionHandlerFeature>();
        Log.Error(ex!.Error, "Error catched in global error handler.");

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

//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthentication();

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
