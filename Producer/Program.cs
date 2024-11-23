using System.Text;
using System.Text.Json;
using Core;
using Microsoft.AspNetCore.Http.HttpResults;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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

app.MapPost("/Pedido", async () =>
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest",
        };

        await using var connection = await factory.CreateConnectionAsync();
        
        await using var channel = await connection.CreateChannelAsync();
        
        await channel.QueueDeclareAsync(
            queue: "pedidos",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
            
        var message = JsonSerializer.Serialize(new Pedido(1, new Usuario(1, "Gabriel", "gabriel@gmail.com")));
            
        var body = Encoding.UTF8.GetBytes(message);

        await channel.BasicPublishAsync("", "pedidos", body);

        return Results.Ok();
    })
    .WithName("PostPedido")
    .WithOpenApi();

app.Run();