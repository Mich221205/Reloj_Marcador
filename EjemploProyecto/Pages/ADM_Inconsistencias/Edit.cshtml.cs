using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services;
using EjemploCoreWeb.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EjemploProyecto.Pages.ADM_Inconsistencias
{
    public class EditModel : PageModel
    {
        private readonly IMotivos_Inconsistencia _motivoService;

        public EditModel(IMotivos_Inconsistencia motivoService)
        {
            _motivoService = motivoService;
            Motivo = new Motivos_Inconsistencias();
        }

        [BindProperty]
        public Motivos_Inconsistencias Motivo { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Motivo = await _motivoService.Cargar_Motivo_X_IDAsync(id);

            if (Motivo == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _motivoService.UpdateAsync(Motivo);

            return RedirectToPage("Index_Inconsistencias");
        }


    }
}
