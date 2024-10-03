using api.src.Data;
using api.src.Models;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// String de coenxion a base de datos
string connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") ?? "Data Source=api.db"; // True, false


// Se supone que no debemos tener credenciales en el codigo, por tanto, el nombre de la base de datos "Data Source-nombre.db"
builder.Services.AddDbContext<ApplicationDBContext>(opt => opt.UseSqlite(connectionString));


var app = builder.Build();

// Crea los Scope correspondientes para la base de datos
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDBContext>(); //Ingresa al ApDBContext que tengo guardado la bases de datos
    DataSeeder.Initialize(services); //Funcion Initialize carga todo lo de dentro, llena el Seeder de la base de datos.
}



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.Run();


