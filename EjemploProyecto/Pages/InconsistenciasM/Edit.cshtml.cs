using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace EjemploProyecto.Pages.Inconsistencias
{
    public class EditModel : PageModel
    {
        private readonly IInconsistenciaService _service;
        private readonly IBitacoraService _bitacoraService;

        [BindProperty]
        public Inconsistencia Inconsistencia { get; set; }

        public EditModel(IInconsistenciaService service, IBitacoraService bitacoraService)
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
            if (!ModelState.IsValid)
                return Page();

            var anterior = await _service.ObtenerPorId(Inconsistencia.ID_Inconsistencia);

            await _service.Actualizar(Inconsistencia);

            await _bitacoraService.Registrar(1, 2, new { Antes = anterior, Despues = Inconsistencia }, "UPDATE");

            TempData["SuccessMessage"] = "Inconsistencia actualizada correctamente.";
            return RedirectToPage("Index");
        }
    }
}

