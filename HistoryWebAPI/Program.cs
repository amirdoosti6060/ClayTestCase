using HistoryWebAPI.Interfaces;
using HistoryWebAPI.Models;
using HistoryWebAPI.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQServiceLib;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

try
{
    builder.Configuration.AddEnvironmentVariables();

    builder.Logging.ClearProviders();
    builder.Logging.AddSerilog(CreateSerilogLogger(builder.Configuration));

    // Add services to the container.
    builder.Services.AddTransient<IHistoryService, HistoryService>();

    builder.Services.AddSingleton(sp =>
        new RabbitBusBuilder()
            .HostName(builder.Configuration["RabbitMQ:Hostname"])
            .UserName(builder.Configuration["RabbitMQ:Username"])
            .Password(builder.Configuration["RabbitMQ:Password"])
            .build()
    );
    
    builder.Services.AddHostedService<RabbitListener>();

    builder.Services.AddDbContext<HistoryDbContext>(options =>
    {
        var connectionString = builder.Configuration["ConnectionStrings:MariaDB"];
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    });

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

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
