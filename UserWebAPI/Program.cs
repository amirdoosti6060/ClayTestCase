using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using UserWebAPI.Interfaces;
using UserWebAPI.Models;
using UserWebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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

builder.Services.AddControllers();
builder.Services.AddDbContext<UserDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("MariaDB");
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
        );
});

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddTransient<IAuthenticatorService,AuthenticatorService>();
builder.Services.AddTransient<IUserService, UserService>();

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

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
