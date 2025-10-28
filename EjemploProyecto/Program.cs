using EjemploCoreWeb.Repository;
using EjemploCoreWeb.Repository.Interfaces;
using EjemploCoreWeb.Services;
using EjemploCoreWeb.Services.Abstract;
using EjemploCoreWeb.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Data;
using EjemploCoreWeb.Repository.Repositories;


using PersonaAbstr = EjemploCoreWeb.Services.Abstract;
using PersonaImpl = EjemploCoreWeb.Services;
using SvcIf = EjemploCoreWeb.Services.Interfaces;
using SvcImpl = EjemploCoreWeb.Services.Services;
using IRolAbstr = EjemploCoreWeb.Services.Abstract.IRolService;

using IUserCrudService = EjemploCoreWeb.Services.Interfaces.IUsuarioService;
using UsuarioCrudServiceImpl = EjemploCoreWeb.Services.UsuarioCrudService;
using IRolCrudService = EjemploCoreWeb.Services.Interfaces.IRolService;
using RolCrudServiceImpl = EjemploCoreWeb.Services.RolService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor(); // (ya lo usas, pero asegúrate)
builder.Services.AddSession();

builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();

builder.Services.AddScoped<IInconsistenciaRepository, InconsistenciaRepository>();
builder.Services.AddScoped<IInconsistenciaService, InconsistenciaService>();

builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<PersonaAbstr.IUsuarioService, PersonaImpl.UsuarioService>();

builder.Services.AddScoped<IBitacoraService, BitacoraService>();

builder.Services.AddScoped<AusenciasRepository>();
builder.Services.AddScoped<IMotivos_Ausencia, Motivos_Services>();

builder.Services.AddScoped<AdmHorariosRepository>();
builder.Services.AddScoped<IHorarios, HorariosServices>();

builder.Services.AddScoped<RolRepository>();
builder.Services.AddScoped<IRolAbstr, RolService>();

builder.Services.AddScoped<IdentificacionRepository>();
builder.Services.AddScoped<ITipoIdentificacionService, TipoIdentificacionService>();

builder.Services.AddScoped<
    EjemploCoreWeb.Repository.Interfaces.IUsuarioRepository,
    EjemploCoreWeb.Repository.Repositories.AdminUsuarioRepository>();

builder.Services.AddScoped<
    SvcIf.IUsuarioService,
    SvcImpl.AdminUsuarioService>();

builder.Services.AddScoped<IAreaRepository, AreaRepository>();
builder.Services.AddScoped<SvcIf.IAreaService, SvcImpl.AreaService>();

builder.Services.AddScoped<IUsuarioAreaRepository, UsuarioAreaRepository>();
builder.Services.AddScoped<SvcIf.IUsuarioAreaService, SvcImpl.UsuarioAreaService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/ADM_Login/Login";         
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

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<IUserCrudService, UsuarioCrudServiceImpl>();

builder.Services.AddScoped<
    EjemploProyecto.Services.KarinaBitacoraService>();

var app = builder.Build();

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
