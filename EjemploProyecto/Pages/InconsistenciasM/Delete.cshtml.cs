using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;

namespace EjemploProyecto.Pages.Inconsistencias
{
    public class DeleteModel : PageModel
    {
        private readonly IInconsistenciaService _service;
        private readonly IBitacoraService _bitacoraService;

        [BindProperty]
        public Inconsistencia Inconsistencia { get; set; } = new Inconsistencia();

        public DeleteModel(IInconsistenciaService service, IBitacoraService bitacoraService)
        {
            _service = service;
            _bitacoraService = bitacoraService;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                Inconsistencia = await _service.ObtenerPorId(id);
                if (Inconsistencia == null)
                    return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                await _bitacoraService.Registrar(1, 3, $"Error al obtener inconsistencia ID {id}: {ex.Message}", "ERROR");
                TempData["ErrorMessage"] = "No se pudo cargar la inconsistencia a eliminar.";
                return RedirectToPage("Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var eliminado = await _service.ObtenerPorId(Inconsistencia.ID_Inconsistencia);
                await _service.Eliminar(Inconsistencia.ID_Inconsistencia);

                await _bitacoraService.Registrar(1, 3, eliminado, "DELETE");
                TempData["SuccessMessage"] = "Inconsistencia eliminada correctamente.";
            }
            catch (MySqlException ex)
            {
                await _bitacoraService.Registrar(1, 3, $"Error MySQL: {ex.Message}", "ERROR");
                TempData["ErrorMessage"] = "No se pudo eliminar la inconsistencia. Puede estar relacionada con otros registros.";
            }
            catch (Exception ex)
            {
                await _bitacoraService.Registrar(1, 3, $"Error general: {ex.Message}", "ERROR");
                TempData["ErrorMessage"] = "Ocurrió un error inesperado al intentar eliminar la inconsistencia.";
            }

            return RedirectToPage("Index");
        }
    }
}
