using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EjemploProyecto.Pages.Reportes.Inconsistencias
{
    public class IndexModel : PageModel
    {
        private readonly IInconsistenciaService _service;
        private readonly IBitacoraService _bitacoraService;

        public IEnumerable<Reporte_Inconsistencia> Reporte { get; set; } = new List<Reporte_Inconsistencia>();

        [BindProperty(SupportsGet = true)]
        public string? Identificacion { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? Fecha { get; set; }

        public int TotalRegistros { get; set; }
        public int PaginaActual { get; set; } = 1;
        public int TamañoPagina { get; set; } = 10;

        public IndexModel(IInconsistenciaService service, IBitacoraService bitacoraService)
        {
            _service = service;
            _bitacoraService = bitacoraService;
        }

        public async Task OnGetAsync(int pagina = 1)
        {
            try
            {
                PaginaActual = pagina;
                Reporte = await _service.Reporte_Inconsistencias(PaginaActual, TamañoPagina);
                
                Reporte = await _service.Reporte_Inconsistencias_Filtros(
                    PaginaActual,
                    TamañoPagina,
                    Identificacion,
                    Fecha
                );

                TotalRegistros = await _service.ContarReporteInconsistencias();


                await _bitacoraService.Registrar(1, 4, "El usuario consultó reporte de inconsistencias ", "CONSULTA");

            // Mensajes pasados desde Create/Edit/Delete
            if (TempData.ContainsKey("SuccessMessage"))
            {
                TempData["ModalType"] = "success";
                TempData["ModalTitle"] = "Éxito";
                TempData["ModalMessage"] = TempData["SuccessMessage"];
            }
            else if (TempData.ContainsKey("ErrorMessage"))
            {
                TempData["ModalType"] = "error";
                TempData["ModalTitle"] = "Error";
                TempData["ModalMessage"] = TempData["ErrorMessage"];
            }
        }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                // Error de base de datos (como el Check constraint)
                await _bitacoraService.Registrar(1, 4, $"Error MySQL: {ex.Message}", "ERROR");

        TempData["ModalType"] = "error";
                TempData["ModalTitle"] = "Error de Base de Datos";
                TempData["ModalMessage"] = "Ocurrió un problema al consultar las inconsistencias. Verifique los datos o contacte al administrador.";
            }
            catch (Exception ex)
            {
                // Cualquier otro tipo de error
                await _bitacoraService.Registrar(1, 4, $"Excepci�n general: {ex.Message}", "ERROR");

    TempData["ModalType"] = "error";
                TempData["ModalTitle"] = "Error inesperado";
                TempData["ModalMessage"] = "Ocurrió un error inesperado al cargar la información.";
            }
        }

    }
}
