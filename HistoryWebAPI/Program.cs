using HistoryWebAPI.Interfaces;
using HistoryWebAPI.Models;
using HistoryWebAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using RabbitMQServiceLib;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

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

var app = builder.Build();

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
