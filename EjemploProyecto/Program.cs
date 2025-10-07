using EjemploCoreWeb.Repository;
using EjemploCoreWeb.Services;
using EjemploCoreWeb.Services.Abstract;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------
// Servicios principales
// ---------------------------
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();

// ---------------------------
// Conexión a base de datos
// ---------------------------
builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();

// ---------------------------
// Inyección de dependencias (repositories y services)
// ---------------------------
builder.Services.AddScoped<IInconsistenciaRepository, InconsistenciaRepository>();
builder.Services.AddScoped<IInconsistenciaService, InconsistenciaService>();

builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

builder.Services.AddScoped<IBitacoraService, BitacoraService>();

builder.Services.AddScoped<AusenciasRepository>();
builder.Services.AddScoped<IMotivos_Ausencia, Motivos_Services>();

builder.Services.AddScoped<AdmHorariosRepository>();
builder.Services.AddScoped<IHorarios, HorariosServices>();

builder.Services.AddScoped<RolRepository>();
builder.Services.AddScoped<IRolService, RolService>();

builder.Services.AddScoped<IdentificacionRepository>();
builder.Services.AddScoped<ITipoIdentificacionService, TipoIdentificacionService>();

// Autenticación por cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/ADM_Login/Login"; // login temporal
        options.AccessDeniedPath = "/ADM_Login/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(5); // expira tras 5 min de inactividad
        options.SlidingExpiration = true; // se renueva si hay actividad
    });

builder.Services.AddAuthorization();

// Vencimiento de sesión (middleware de session)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// ---------------------------
// Configuración de sesiones
// ---------------------------
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo de expiración
    options.Cookie.HttpOnly = true;                 // Solo accesible vía HTTP
    options.Cookie.IsEssential = true;              // Necesario para GDPR
});

// ---------------------------
// Construcción del app
// ---------------------------
var app = builder.Build();

// ---------------------------
//  Configuración del pipeline HTTP
// ---------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();

app.UseSession();

app.UseAuthorization();

// ---------------------------
// Redirección inicial (ruta de pruebas)
// ---------------------------
app.MapGet("/", context =>
{
    context.Response.Redirect("/ADM_Login/Login");
    return Task.CompletedTask;
});

// ---------------------------
// Razor Pages
// ---------------------------
app.MapRazorPages();

// ---------------------------
// Ejecutar la app
// ---------------------------
app.Run();