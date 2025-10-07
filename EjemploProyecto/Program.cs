using EjemploCoreWeb.Repository;
using EjemploCoreWeb.Repository.Interfaces;
using EjemploCoreWeb.Services;
using EjemploCoreWeb.Services.Abstract;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Data;
using EjemploCoreWeb.Repository.Repositories;

// Aliases ya usados
using PersonaAbstr = EjemploCoreWeb.Services.Abstract;
using PersonaImpl = EjemploCoreWeb.Services;
using SvcIf = EjemploCoreWeb.Services.Interfaces;
using SvcImpl = EjemploCoreWeb.Services.Services;

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
// Inyección de dependencias (DE ELLOS)
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

// ---------------------------
// HU7 / HU8 / HU9 — Repos
// ---------------------------
builder.Services.AddScoped<IAreaRepository, AreaRepository>();
builder.Services.AddScoped<IUsuarioAreaRepository, UsuarioAreaRepository>();

// ---------------------------
// HU7 / HU8 / HU9 — Services
// ---------------------------
builder.Services.AddScoped<SvcIf.IAreaService, SvcImpl.AreaService>();
builder.Services.AddScoped<SvcIf.IUsuarioAreaService, SvcImpl.UsuarioAreaService>();

// ⛔ IMPORTANTE: no agregar mapeos extra para IUsuarioService (Interfaces) aquí,
// porque tu UsuarioService implementa la interfaz de Abstract y ya está registrada.

// ---------------------------
// Autenticación por cookies
// ---------------------------
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/ADM_Login/Login"; // login temporal
        options.AccessDeniedPath = "/ADM_Login/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// (Segundo bloque de sesión se respeta)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ---------------------------
// Construcción del app
// ---------------------------
var app = builder.Build();

// ---------------------------
// Pipeline
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

app.MapGet("/", context =>
{
    context.Response.Redirect("/ADM_Login/Login");
    return Task.CompletedTask;
});

app.MapRazorPages();
app.Run();
