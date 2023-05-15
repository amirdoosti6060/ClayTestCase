using HistoryWebAPI.Models;
using HistoryWebAPI.Services;
using Microsoft.EntityFrameworkCore;
using RabbitMQServiceLib;
using System.Collections;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton(sp => 
    new RabbitBusBuilder()
        .HostName("localhost")
        .UserName("guest")
        .Password("guest")
        .build()
);

builder.Services.AddHostedService<RabbitListener>();
/*
builder.Services.AddSingleton<RabbitMQService>(provider =>
{
    string hostName = "localhost";
    string userName = "guest";
    string password = "guest";
    return new RabbitMQService(hostName, userName, password);
});

builder.Services.AddHostedService<MessageHandler>(provider =>
{
    var rabbitMQService = provider.GetRequiredService<RabbitMQService>();
    var requestQueue = "historyQueue";

    return new MessageHandler(rabbitMQService, requestQueue);
});
*/

builder.Services.AddDbContext<HistoryDbContext>(options =>
{
    //var connectionString = Environment.GetEnvironmentVariable("ConnectionString_MariaDB");
    var connectionString = builder.Configuration.GetConnectionString("MariaDB");
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
        );
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
