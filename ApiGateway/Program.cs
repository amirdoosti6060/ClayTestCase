using ApiGateway.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System.Net;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

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


builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddOcelot(builder.Configuration);

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

app.UseAuthentication();
app.UseAuthorization();

await app.UseOcelot();

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
