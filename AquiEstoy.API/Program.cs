using AquiEstoy.API.Hubs;
using AquiEstoy.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddSignalR();

// El cliente MVC vive en otro origen, asi que SignalR necesita CORS explicito.
// AllowAnyOrigin es incompatible con AllowCredentials, y SignalR requiere credenciales:
// por eso los origenes se listan uno a uno desde configuracion.
const string CorsPolicyWeb = "PermitirClienteWeb";

var origenesWeb = builder.Configuration
    .GetSection("Cors:OrigenesPermitidos")
    .Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyWeb, policy =>
        policy.WithOrigins(origenesWeb)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Aplica las migraciones pendientes automáticamente en Development
    try
    {
        await app.Services.MigrateDatabaseAsync();
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex,
            "No se pudieron aplicar las migraciones. Verifique que SQL Server este accesible " +
            "en la cadena ConnectionStrings:DefaultConnection.");
        throw;
    }

    // Catalogos minimos para que el registro de pacientes y las alertas de riesgo funcionen. Idempotente.
    try
    {
        await app.Services.SeedDevelopmentDataAsync();
    }
    catch (Exception ex)
    {
        // Se falla rapido a proposito: sin catalogos el registro de pacientes no
        // puede funcionar.
        app.Logger.LogError(ex,
            "No se pudo sembrar la base de datos. Verifique que SQL Server este accesible " +
            "en la cadena ConnectionStrings:DefaultConnection.");
        throw;
    }
}

app.UseHttpsRedirection();

app.UseCors(CorsPolicyWeb);

app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/hubs/chat");

app.Run();
