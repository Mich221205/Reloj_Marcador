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
// Servicios existentes del equipo
// ---------------------------
builder.Services.AddScoped<IInconsistenciaRepository, InconsistenciaRepository>();
builder.Services.AddScoped<IInconsistenciaService, InconsistenciaService>();

builder.Services.AddScoped<UsuarioRepository>();
// ⇨ Servicio “viejo” para Login/Cambio de clave (mantener).
builder.Services.AddScoped<PersonaAbstr.IUsuarioService, PersonaImpl.UsuarioService>();

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
// HU7 / HU8 / HU9 — Repositorios y servicios ADMIN
// ---------------------------

// Repositorio ADMIN para CRUD de usuarios (1 sola vez; elimina duplicados)
builder.Services.AddScoped<
    EjemploCoreWeb.Repository.Interfaces.IUsuarioRepository,
    EjemploCoreWeb.Repository.Repositories.AdminUsuarioRepository>();

// Servicio ADMIN de usuarios (CRUD, paginación, etc.)
builder.Services.AddScoped<
    SvcIf.IUsuarioService,
    SvcImpl.AdminUsuarioService>();

// Áreas
builder.Services.AddScoped<IAreaRepository, AreaRepository>();
builder.Services.AddScoped<SvcIf.IAreaService, SvcImpl.AreaService>();

// Usuario–Área
builder.Services.AddScoped<IUsuarioAreaRepository, UsuarioAreaRepository>();
builder.Services.AddScoped<SvcIf.IUsuarioAreaService, SvcImpl.UsuarioAreaService>();


// REPORTE DE MARCAS
builder.Services.AddScoped<IMarca, MarcaRepository>();
builder.Services.AddScoped<IMarcaService, MarcaService>();


// PROCESO DE INCONSISTENCIAS VS HORARIOS
builder.Services.AddScoped<Proceso_Generar_Inconsistencias_Marcas>();
builder.Services.AddScoped<Proceso_Generar_Inconsistencias_MarcasService>();


builder.Services.AddHostedService<PROC1_Automatizado>();



// ---------------------------
// Autenticación por cookies
// ---------------------------
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/ADM_Login/Login";           // login temporal
        options.AccessDeniedPath = "/ADM_Login/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

// ---------------------------
// Sesión (dejamos ambos bloques tal como estaban)
// ---------------------------
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

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
