using NoWasteOfMoney.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using NoWasteOfMoney.Interfaces;
using NoWasteOfMoney.Services;
using NoWasteOfMoney.Service.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DatabaseContext>(options =>
<<<<<<< Updated upstream
    options.UseInMemoryDatabase("NoWasteOfMoney"));
=======
    options.UseMySql(connectionString, serverVersion, b =>
        b.MigrationsAssembly("NoWasteOfMoney")));
>>>>>>> Stashed changes
builder.Services.AddControllers();

builder.Services.AddScoped<IPersonsService, PersonsService>();
builder.Services.AddScoped<IMovementService, MovementService>();
builder.Services.AddScoped<IMonthMovementService, MonthMovementsService>();
builder.Services.AddScoped<IUsersService, UserService>();
builder.Services.AddScoped<TokenService>();
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
