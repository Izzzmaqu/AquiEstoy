using Microsoft.EntityFrameworkCore;
using AquiEstoy.Infrastructure.Data; // Ajusta el namespace si tu DbContext está en otra carpeta
using AquiEstoy.Web.Hubs;

var builder = WebApplication.CreateBuilder(args);

// 1. Obtener la cadena de conexión desde appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Registrar el DbContext con SQL Server
builder.Services.AddDbContext<AquiEstoyDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", config =>
    {
        config.Cookie.Name = "AquiEstoy.AuthCookie";
        config.LoginPath = "/Account/Login"; // Redirecciona si no ha iniciado sesión
        config.AccessDeniedPath = "/Account/AccessDenied"; // Redirecciona si no tiene permisos de rol
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Habilitar la Autenticación 
app.UseAuthentication();
app.UseAuthorization();

// Establecer el Login como la pantalla de inicio 
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

// MAPEAR LA RUTA DEL HUB DE SIGNALR
app.MapHub<ChatHub>("/chatHub");

app.Run();