using Microsoft.EntityFrameworkCore;
using TaskConsumerAPI.Models;
using TaskConsumerAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<RegisterTokenContext>(opt =>
    opt.UseInMemoryDatabase("RegisterTokenList"));

builder.Services.AddHostedService<ConsumerService>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();
