using DoorWebAPI.Interfaces;
using DoorWebAPI.Models;
using DoorWebAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RabbitMQServiceLib;
using System.Net;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Read environment variables
builder.Host.ConfigureHostConfiguration(config =>
{
    config.AddEnvironmentVariables();
});

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
    //var connectionString = Environment.GetEnvironmentVariable("ConnectionString_MariaDB");
    var connectionString = builder.Configuration.GetConnectionString("MariaDB");
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
        );
});

builder.Services.AddSingleton(sp =>
    new RabbitBusBuilder()
        .HostName("localhost")
        .UserName("guest")
        .Password("guest")
        .build()
);

builder.Services.AddTransient<IPermissionService, PermissionService>();
builder.Services.AddTransient<IDoorService, DoorService>();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

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
                ErrorCode = ex.Error.GetType().Name,
                ErrorMessage = ex.Error.Message,
                Data = builder.Environment.IsDevelopment() ? ex.Error : ex.Error.Source
            };

            await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }
    });
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
//app.UseAuthorization();

app.MapControllers();

app.Run();
