using AquiEstoy.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Unica dependencia de datos de la capa Web: HTTP contra AquiEstoy.API.
// Ya no hay DbContext ni referencia a Infrastructure/Domain.
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"]
    ?? throw new InvalidOperationException(
        "Falta la clave ApiSettings:BaseUrl en appsettings.json; la capa Web no sabe donde esta la API.");

var apiClientBuilder = builder.Services.AddHttpClient<AquiEstoyApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

if (builder.Environment.IsDevelopment())
{
    // El certificado HTTPS de desarrollo de la API es autofirmado. Este bypass
    // esta acotado a Development a proposito: en produccion debe validarse.
    apiClientBuilder.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    });
}

builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", config =>
    {
        config.Cookie.Name = "AquiEstoy.AuthCookie";
        config.LoginPath = "/Account/Login";
        config.AccessDeniedPath = "/Account/AccessDenied";
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
