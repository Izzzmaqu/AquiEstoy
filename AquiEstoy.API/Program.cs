using AquiEstoy.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Catalogos minimos para que el registro de pacientes y las alertas de riesgo funcionen. Idempotente.
    try
    {
        await app.Services.SeedDevelopmentDataAsync();
    }
    catch (Exception ex)
    {
        // Se falla rapido a proposito: sin catalogos el registro de pacientes no
        // puede funcionar. El mensaje dice que revisar en vez de soltar solo el stack.
        app.Logger.LogError(ex,
            "No se pudo sembrar la base de datos. Verifique que SQL Server este accesible " +
            "en la cadena ConnectionStrings:DefaultConnection y que las migraciones esten aplicadas " +
            "(dotnet ef database update --project AquiEstoy.Infrastructure --startup-project AquiEstoy.API).");
        throw;
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
