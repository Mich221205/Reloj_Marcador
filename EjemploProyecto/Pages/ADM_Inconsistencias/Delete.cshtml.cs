using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services;
using EjemploCoreWeb.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EjemploProyecto.Pages.ADM_Inconsistencias
{
    public class DeleteModel : PageModel
    {

        private readonly IMotivos_Inconsistencia _motivoService;

        public DeleteModel(IMotivos_Inconsistencia motivoService)
        {
            _motivoService = motivoService;
            Motivo = new Motivos_Inconsistencias();
        }

        [BindProperty]
        public Motivos_Inconsistencias? Motivo { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Motivo = await _motivoService.Cargar_Motivo_X_IDAsync(id);

            if (Motivo == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (Motivo != null)
            {
                await _motivoService.DeleteAsync(Motivo.ID_Motivo);
            }
            return RedirectToPage("Index_Inconsistencias");
        }

    }
}
