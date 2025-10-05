using EjemploCoreWeb.Repository;
using EjemploCoreWeb.Services;
using EjemploCoreWeb.Services.Abstract;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();

//Ejemplo del profe
//builder.Services.AddScoped<PersonaRepository>(); //cada solicitud crea un personaRepository
//builder.Services.AddScoped<IPersonaService, PersonaService>();

builder.Services.AddScoped<IInconsistenciaRepository, InconsistenciaRepository>();
builder.Services.AddScoped<IInconsistenciaService, InconsistenciaService>();
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

//SE LLAMA INYECCION DE DEPENDENCIAS BROTHER

//inyecciones para roles
builder.Services.AddScoped<RolRepository>();
builder.Services.AddScoped<IRolService, RolService>();

//inyecciones para tipos de identificacion
builder.Services.AddScoped<IdentificacionRepository>();
builder.Services.AddScoped<ITipoIdentificacionService, TipoIdentificacionService>();

// de vencimiento se session (5 minutos)

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5); //la cuenta
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// servicio de bit�cora
builder.Services.AddScoped<IBitacoraService, BitacoraService>();

var app = builder.Build();

app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();


//LEVANTAMIENTO DE PAGINA INDEX PARA PRUEBAS
app.MapGet("/", context =>
{
    context.Response.Redirect("/InconsistenciasM/Index");
    return Task.CompletedTask;
});


app.MapRazorPages();

app.Run();
