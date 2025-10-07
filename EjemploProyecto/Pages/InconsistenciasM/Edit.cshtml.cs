using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;

namespace EjemploProyecto.Pages.Inconsistencias
{
    public class EditModel : PageModel
    {
        private readonly IInconsistenciaService _service;
        private readonly IBitacoraService _bitacoraService;

        [BindProperty]
        public Inconsistencia Inconsistencia { get; set; } = new Inconsistencia();

        public EditModel(IInconsistenciaService service, IBitacoraService bitacoraService)
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
                await _bitacoraService.Registrar(1, 2, $"Error al obtener inconsistencia ID {id}: {ex.Message}", "ERROR");
                TempData["ErrorMessage"] = "No se pudo cargar la inconsistencia seleccionada.";
                return RedirectToPage("Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            try
            {
                var anterior = await _service.ObtenerPorId(Inconsistencia.ID_Inconsistencia);
                await _service.Actualizar(Inconsistencia);

                await _bitacoraService.Registrar(1, 2, new { Antes = anterior, Despues = Inconsistencia }, "UPDATE");
                TempData["SuccessMessage"] = "Inconsistencia actualizada correctamente.";
            }
            catch (MySqlException ex)
            {
                await _bitacoraService.Registrar(1, 2, $"Error MySQL: {ex.Message}", "ERROR");
                TempData["ErrorMessage"] = "Error al actualizar la inconsistencia. Verifique los datos.";
            }
            catch (Exception ex)
            {
                await _bitacoraService.Registrar(1, 2, $"Error general: {ex.Message}", "ERROR");
                TempData["ErrorMessage"] = "Ocurrió un error inesperado al actualizar la inconsistencia.";
            }

            return RedirectToPage("Index");
        }
    }
}
