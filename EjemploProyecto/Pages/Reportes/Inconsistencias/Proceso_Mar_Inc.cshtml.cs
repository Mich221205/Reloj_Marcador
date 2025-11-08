using EjemploCoreWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EjemploProyecto.Pages.Reportes.Inconsistencias
{
    public class Proceso_Mar_IncModel : PageModel
    {


        private readonly Proceso_Generar_Inconsistencias_MarcasService _service;

        public Proceso_Mar_IncModel(Proceso_Generar_Inconsistencias_MarcasService service)
        {
            _service = service;
        }

        [BindProperty]
        public DateTime FechaInicio { get; set; } = DateTime.Today;
        [BindProperty]
        public DateTime FechaFin { get; set; } = DateTime.Today;
        public string Resultado { get; set; } = string.Empty;

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                await _service.EjecutarAsync(FechaInicio, FechaFin);
                Resultado = $"Proceso ejecutado correctamente del {FechaInicio:dd/MM/yyyy} al {FechaFin:dd/MM/yyyy}.";
            }
            catch (Exception ex)
            {
                Resultado = $"Error al ejecutar el proceso: {ex.Message}";
            }

            return Page();
        }


    }
}
