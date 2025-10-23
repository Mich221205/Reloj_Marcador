using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EjemploProyecto.Pages.Inconsistencias
{
    public class IndexModel : PageModel
    {
        private readonly IInconsistenciaService _service;
        private readonly IBitacoraService _bitacoraService;

        public IEnumerable<Inconsistencia> Inconsistencias { get; set; } = new List<Inconsistencia>();
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
                Inconsistencias = await _service.Listar(PaginaActual, TamañoPagina);
                TotalRegistros = await _service.Contar();

                await _bitacoraService.Registrar(1, 4, "El usuario consulta Inconsistencias", "CONSULTA");

                // Mensajes de retroalimentación
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
            catch (MySqlException ex)
            {
                await _bitacoraService.Registrar(1, 4, $"Error MySQL: {ex.Message}", "ERROR");
                TempData["ModalType"] = "error";
                TempData["ModalTitle"] = "Error de Base de Datos";
                TempData["ModalMessage"] = "Ocurrió un problema al consultar las inconsistencias.";
            }
            catch (Exception ex)
            {
                await _bitacoraService.Registrar(1, 4, $"Error general: {ex.Message}", "ERROR");
                TempData["ModalType"] = "error";
                TempData["ModalTitle"] = "Error inesperado";
                TempData["ModalMessage"] = "Ocurrió un error inesperado al cargar la información.";
            }
        }

        // 🔥 Acción POST para eliminar desde el modal
        public async Task<IActionResult> OnPostAsync(int id)
        {
            try
            {
                var inconsistencia = await _service.ObtenerPorId(id);
                if (inconsistencia != null)
                {
                    await _service.Eliminar(id);
                    await _bitacoraService.Registrar(1, 3, inconsistencia, "DELETE");
                    TempData["SuccessMessage"] = "Inconsistencia eliminada correctamente.";
                }
                else
                {
                    TempData["ErrorMessage"] = "No se encontró la inconsistencia a eliminar.";
                }
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
