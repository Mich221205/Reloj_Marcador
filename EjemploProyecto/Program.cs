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

builder.Services.AddScoped<IInconsistenciaRepository, InconsistenciaRepository>();
builder.Services.AddScoped<IInconsistenciaService, InconsistenciaService>();
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

builder.Services.AddScoped<AusenciasRepository>();
builder.Services.AddScoped<IMotivos_Ausencia, Motivos_Services>();

builder.Services.AddScoped<AdmHorariosRepository>();
builder.Services.AddScoped<IHorarios, HorariosServices>();

//SE LLAMA INYECCION DE DEPENDENCIAS BROTHER

// servicio de bitácora
builder.Services.AddScoped<IBitacoraService, BitacoraService>();

var app = builder.Build();

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
