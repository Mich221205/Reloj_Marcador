using EjemploCoreWeb.Repository;
using EjemploCoreWeb.Services;
using EjemploCoreWeb.Services.Abstract;
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

app.UseSession();

app.UseAuthorization();

// ---------------------------
// Redirección inicial (ruta de pruebas)
// ---------------------------
app.MapGet("/", context =>
{
    context.Response.Redirect("/InconsistenciasM/Index");
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