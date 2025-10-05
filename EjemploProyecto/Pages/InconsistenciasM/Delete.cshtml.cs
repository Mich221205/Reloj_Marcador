using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace EjemploProyecto.Pages.Inconsistencias
{
    public class DeleteModel : PageModel
    {
        private readonly IInconsistenciaService _service;
        private readonly IBitacoraService _bitacoraService;

        [BindProperty]
        public Inconsistencia Inconsistencia { get; set; }

        public DeleteModel(IInconsistenciaService service, IBitacoraService bitacoraService)
        {
            _service = service;
            _bitacoraService = bitacoraService;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Inconsistencia = await _service.ObtenerPorId(id);
            if (Inconsistencia == null)
                return RedirectToPage("Index");
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
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToPage("Index");
        }

    }
}

