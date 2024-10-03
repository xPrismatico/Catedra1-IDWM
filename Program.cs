using api.src.Data;
using api.src.Repositories;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// String de conexión a la base de datos
string connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") ?? "Data Source=api.db";

// Añadir contexto de base de datos
builder.Services.AddDbContext<ApplicationDBContext>(opt => opt.UseSqlite(connectionString));

// Registrar controladores
builder.Services.AddControllers();

// Registrar el repositorio para inyección de dependencias
builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

// Ejecutar migraciones al inicio
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDBContext>();

    // Asegurarse de que la base de datos esté actualizada
    context.Database.Migrate();

    // Llenar la base de datos con datos iniciales si es necesario
    DataSeeder.Initialize(services);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Mapear controladores
app.MapControllers();

app.Run();
