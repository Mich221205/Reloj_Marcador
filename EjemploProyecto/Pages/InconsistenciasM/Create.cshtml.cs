using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;

namespace EjemploProyecto.Pages.Inconsistencias
{
    public class CreateModel : PageModel
    {
        private readonly IInconsistenciaService _service;
        private readonly IBitacoraService _bitacoraService;

        [BindProperty]
        public Inconsistencia Inconsistencia { get; set; } = new Inconsistencia();

        public CreateModel(IInconsistenciaService service, IBitacoraService bitacoraService)
        {
            _service = service;
            _bitacoraService = bitacoraService;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            try
            {
                await _service.Crear(Inconsistencia);
                await _bitacoraService.Registrar(1, 1, Inconsistencia, "INSERT");

                TempData["SuccessMessage"] = "Inconsistencia creada correctamente.";
            }
            catch (MySqlException ex)
            {
                await _bitacoraService.Registrar(1, 1, $"Error MySQL: {ex.Message}", "ERROR");
                TempData["ErrorMessage"] = "Error al crear la inconsistencia. Verifique los datos ingresados.";
            }
            catch (Exception ex)
            {
                await _bitacoraService.Registrar(1, 1, $"Error general: {ex.Message}", "ERROR");
                TempData["ErrorMessage"] = "Ocurrió un error inesperado al registrar la inconsistencia.";
            }

            return RedirectToPage("Index");
        }
    }
}