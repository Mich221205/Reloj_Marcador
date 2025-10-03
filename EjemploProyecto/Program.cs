using EjemploCoreWeb.Repository;
using EjemploCoreWeb.Services;
using EjemploCoreWeb.Services.Abstract;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();

//Ejemplo del profe
builder.Services.AddScoped<PersonaRepository>(); //cada solicitud crea un personaRepository
builder.Services.AddScoped<IPersonaService, PersonaService>();

builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

builder.Services.AddScoped<MotivosRepository>();
builder.Services.AddScoped<IMotivos_Inconsistencia, Motivos_Services>();

//SE LLAMA INYECCION DE DEPENDENCIAS BROTHER

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
