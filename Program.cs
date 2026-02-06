using NoWasteOfMoney.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using NoWasteOfMoney.Interfaces;
using NoWasteOfMoney.Services;


var builder = WebApplication.CreateBuilder(args);

// Program.cs
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Detecta a versão do servidor automaticamente (Docker deve estar rodando)
var serverVersion = ServerVersion.AutoDetect(connectionString);

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseMySql(connectionString, serverVersion, b => 
        b.MigrationsAssembly("NoWasteOfMoney")));
builder.Services.AddControllers();

builder.Services.AddScoped<IPersonsService, PersonsService>();
builder.Services.AddScoped<IMovementService, MovementService>();
builder.Services.AddScoped<IMonthMovementService, MonthMovementsService>();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// dotnet add package Swashbuckle.AspNetCore.SwaggerUi -Version 9.0.6

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
