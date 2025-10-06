using EjemploCoreWeb.Repository;
using EjemploCoreWeb.Services;
using EjemploCoreWeb.Services.Abstract;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();

// Inyecciones de dependencias
builder.Services.AddScoped<IInconsistenciaRepository, InconsistenciaRepository>();
builder.Services.AddScoped<IInconsistenciaService, InconsistenciaService>();
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<RolRepository>();
builder.Services.AddScoped<IRolService, RolService>();
builder.Services.AddScoped<IdentificacionRepository>();
builder.Services.AddScoped<ITipoIdentificacionService, TipoIdentificacionService>();
builder.Services.AddScoped<IBitacoraService, BitacoraService>();

// Autenticación por cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/PRUEBA_Usuarios/LoginPrueba"; // login temporal
        options.AccessDeniedPath = "/PRUEBA_Usuarios/LoginPrueba";
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

var app = builder.Build();

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.UseSession(); // importante: antes de auth
app.UseAuthentication();
app.UseAuthorization();

// Redirección inicial al login
app.MapGet("/", context =>
{
    context.Response.Redirect("/PRUEBA_Usuarios/LoginPrueba");
    return Task.CompletedTask;
});

app.MapRazorPages();

app.Run();
